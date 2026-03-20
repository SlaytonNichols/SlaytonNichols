---
title: "Building a 24/7 WebSocket Data Recorder"
summary: "How I built a reliable ingestion system that accumulated 363M+ orderbook rows — and the engineering problems that surfaced at scale."
date: 2026-03-20
tags:
  - data-engineering
  - python
  - systems
draft: true
hidden: true
---

## Building a 24/7 WebSocket Data Recorder

The [trading project](/posts/trading-bot-journey) needed full limit-order-book depth snapshots at 100ms resolution, 24/7. No vendor offered this at the granularity I needed, so I built my own recorder. Over several months it accumulated **363M+ orderbook rows**.

---

### The Design

Single Python process maintaining a persistent WebSocket connection to Bybit's L2 depth-50 stream (~31 msg/sec sustained).

**Write format: JSONL.gz with 10-minute rotation.** Messages buffer in memory and flush to gzip-compressed JSON Lines files at rotation boundaries. JSONL is append-friendly (crash mid-write loses one line), gzip compresses well (orderbook data is repetitive), and Spark reads `*.jsonl.gz` natively.

---

### What Went Wrong

**WebSocket disconnects.** Early versions reconnected silently. Without timestamps on reconnection events, correlating data gaps with model behavior was painful. Lesson: log every reconnection.

**Memory pressure.** Buffered too aggressively on a constrained VM → OOM killer. Fix: flush on a time interval with a hard buffer cap.

**Silent data corruption.** Bybit sends **full snapshots** and **delta updates**. The recorder initially treated deltas as full snapshots — producing an orderbook that loaded cleanly but was structurally wrong. Spreads inflated 32–118×, bids and asks inverted. This [took months to catch](/posts/debugging-training-data).

---

### Downstream Pipeline

Raw `.jsonl.gz` files land in ADLS Gen2 ([setup here](/posts/databricks-setup)), then flow through Delta Lake:

1. **Raw → Bronze:** Load as-is. One row per message.
2. **Bronze → Silver:** Stateful book reconstruction — apply deltas incrementally on top of snapshots. This was the broken step.
3. **Silver → Features:** Spread, depth imbalance, queue dynamics, temporal features.
4. **Features → Labels:** Forward-looking outcomes for supervised learning.

---

### Memory Engineering at Scale

An orderbook tensor is 50 × 40 × 3 = 6,000 floats/row. Spark's `ArrayType(FloatType())` creates ~168KB/row overhead — **840 GB** for 5M rows. Storing as `BinaryType` with `np.frombuffer()` reconstruction: **~50 MB**. Not queryable in SQL, but the training loop doesn't need that.

---

### What I'd Do Differently

- **Log everything from day one.** Reconnections, message counts, gap durations.
- **Validate at the recording layer.** "Is best bid < best ask?" would have caught the snapshot/delta bug immediately.
- **Design for replay.** The raw archive is the most valuable artifact. Invest in checksums, manifests, and gap inventories early.

---

| Metric | Value |
|---|---|
| Total rows | 363M+ |
| Message rate | ~31 msg/sec |
| Depth | 50 bid + 50 ask levels |
| Resolution | 100ms |
| Rotation | 10 minutes |
| Format | JSONL.gz |
| Compressed storage | ~120 GB |
