---
title: "Debugging Corrupt Training Data"
summary: "Three simultaneous data bugs went undetected for months — because the model trained on bad data still looked great."
date: 2026-03-20
tags:
  - data-engineering
  - debugging
  - python
draft: true
hidden: true
---

## Debugging Corrupt Training Data

The most dangerous failure mode in a data pipeline isn't a crash. It's silent corruption — data that loads cleanly, produces models with strong metrics, but doesn't represent reality. This is how three simultaneous bugs went undetected in my [orderbook data pipeline](/posts/trading-bot-journey) for months.

---

### The Symptoms

The V3.0 model was making decisions that didn't correspond to any recognizable market behavior. I iterated through three variants — adjusting learning rates, layers, training windows. Each produced reasonable validation metrics and unreasonable live behavior.

The problem wasn't the model. It was the data.

---

### Three Bugs, One Root Cause

**Bug #1: Inverted Bid/Ask.** A field mapping error swapped bids and asks. Every spread, depth imbalance, and pressure ratio was computed on a mirror image of reality.

**Bug #2: Spread Normalization Off by 3000×.** Spread was calculated in raw price units with a broken denominator. Training data showed 0.5–1.3 bps; live reality was ~0.015 bps.

**Bug #3: Wrong Book Depth Indexing.** The snapshot-vs-delta handling shifted level alignment. The model thought the best bid was several levels deep.

**Why none were caught:** Each bug was *consistent* across training and test data. The model learned the distorted patterns, and validation on identically distorted test data looked fine — **87%+ accuracy, low loss, smooth learning curves.**

**The root cause:** The [WebSocket recorder](/posts/websocket-recorder) captures full snapshots and delta updates. The pipeline treated every message as a full snapshot, so deltas (containing only changed levels) were interpreted as the entire book. Bids/asks scrambled, spreads inflated 32–118×, level ordering broken.

---

### How the Corruption Propagated

```
Raw data (ok) → Stateful reconstruction (broken) → Features (wrong)
→ Labels (wrong) → Training (learned distortion) 
→ Validation (same distortion → metrics look great)
→ Live (real data → nonsensical behavior)
```

Standard validation — train/test split, cross-validation, holdout sets — assumes the data distribution is real. Uniform corruption passes every check.

---

### What I Changed

- **One shared code path** for feature computation between training (Spark) and inference (Python). Eliminates drift.
- **Normalization stats saved with the model checkpoint.** Inference loads from the checkpoint, not its own computation.
- **Integration tests for pipeline parity.** Same raw input through both paths, assert identical outputs.
- **Invariant checks at every stage:** best bid < best ask, spread within expected range, monotonic level ordering, no all-zero tensors, feature distributions within 2σ of historical.

---

### Takeaways

1. **Validate the data, not just the model.** If the entire dataset is corrupted uniformly, standard ML validation passes.
2. **Shared code paths between training and inference are not optional.**
3. **The most dangerous model is the one that succeeds on bad data.**
4. **Simple invariant checks catch complex bugs.** "Is best bid < best ask?" would have caught this on day one.
5. **Live deployment is the only real validation.** The gap between "works on test data" and "works in production" is where the most instructive failures live.
