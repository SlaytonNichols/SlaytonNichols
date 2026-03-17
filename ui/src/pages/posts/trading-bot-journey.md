---
title: "ML on Market Microstructure: What I Learned"
summary: "What I learned building algorithmic trading systems from scratch — data engineering, model architecture, and live execution."
date: 2026-03-17
tags:
  - trading
  - ai
  - ml
  - data-engineering
  - python
draft: false
pinned: false
---

## ML on Market Microstructure: What I Learned

I've spent the past 8 months building algorithmic crypto trading systems targeting BTC/USDT perpetual futures on Bybit. Two repos, ~33,000 lines of Python, dozens of ML models trained on a billion rows of orderbook data, and multiple live deployments. This is what I learned about the intersection of machine learning and market microstructure.

---

### The Thesis

The target: pass a $200K prop firm challenge and earn a funded account. The core idea was to forecast **microstructure response patterns** at the limit-order-book (L2) level — absorption, liquidity vacuums, exhaustion — rather than predicting price direction outright. Edges of a few basis points per trade, maker-only execution, high frequency.

---

### Handcrafted L2 Signals

The first approach came from market microstructure research. Five signals, each encoding a specific orderbook hypothesis:

- **Absorption** — detect "freeze points" where aggressive orders get absorbed without price movement, then fade the aggression
- **Liquidity Vacuum** — model depth decay and compression zones, capture snap price expansions
- **Pullback Continuation** — identify impulse legs followed by controlled retracements
- **Exhaustion Reversal** — detect blowoff prints and depth inversion, enter reversals
- **Cross-Asset Lead/Lag** — exploit BTC→ETH dynamics and volatility spillover

Validated on 8-day rolling walkforwards. Three out of five showed promise. The main limitation: I was encoding my *assumptions* about which patterns matter rather than letting the data decide.

Tech stack: Python on [QuantConnect LEAN](https://www.quantconnect.com/lean), Bybit WebSocket for L2 depth-50 at 100ms, and a 24/7 recorder accumulating 363M+ orderbook rows.

---

### The Neural Network: Train on Outcomes, Not Patterns

The key insight that drove the pivot to neural networks: **train on outcomes, not pattern labels.** Instead of labeling data with "this is an absorption pattern," label each row with what *actually happened* — did price move favorably? By how much? Did you get adversely selected? The network discovers which patterns lead to profitable trades on its own.

**Transformer encoder + Mixture of Experts:**
- 4-layer Transformer (8 attention heads, 512 FFN dim) → 8-expert MoE with top-2 routing
- ~850K parameters, 89 input features (60 L2 + 14 queue dynamics + 10 temporal + 5 seasonality)
- 7 output heads: entry probability, direction, edge (bps), MFE, MAE, adverse selection probability, fill probability
- Trained on Databricks with V100 GPUs, ~1.5M samples at 100ms resolution

Parallel architecture — **DirectionCNNGRU** — consumed raw orderbook tensors (50 timesteps × 40 price levels × 3 features) through Conv1D → GRU. ~235K parameters, <2ms CPU inference. Designed to learn directly from book shape rather than engineered features.

#### The Cont-de Larrard Queue Model

One of the more elegant technical pieces. The [Cont-de Larrard (2013)](https://doi.org/10.1007/s11579-013-0104-7) analytical model frames a limit order fill as a *race* between:

1. Orders ahead draining → you reach front of queue → fill
2. Opposite side's queue depleting → price moves away → adverse selection

O(1) computation, 14 output features capturing queue health, fill probability, and adverse timing risk. These consistently ranked among the most important features across every model variant.

---

### Live: What the Market Teaches You

The V3.7 model went live on January 31, 2026. Over 437 trades across 6 days:

- **40.5% win rate** with positive overall PnL
- **~2 second median fill time** for maker limit orders at best bid/ask
- Average hold time of ~2 minutes, average peak MFE of 17.6 bps

The execution data revealed something important: **fast fills are a warning sign.** Orders that filled in under 5 seconds averaged negative PnL, while slower fills (where you genuinely provided liquidity) were profitable. Getting filled quickly usually means someone is running through your level — textbook adverse selection.

This reframed the problem entirely. The question isn't "which direction will price move?" It's "can I get filled in a spot where I'm providing genuine liquidity rather than getting picked off?"

---

### What a Billion Rows of Orderbook Data Taught Me

After 40+ experiments, two neural architectures, and a LightGBM classifier across 7.2M training rows:

**L2 data tells you *when*, not *what*.** Every directional model converged to ~53% binary accuracy — noise level. But a LightGBM gate model (AUC 0.68, running at 40Hz) reliably identifies *favorable microstructure conditions*. The orderbook is an execution timing overlay, not a directional signal.

**Fill dynamics dominate everything.** Your signal can be right 70% of the time, but if limit orders fill only when you're about to get run over, net expectancy is negative. Modeling the fill process (queue position, adverse selection probability, fill latency) matters more than modeling price direction.

**Training/live parity is the hardest ML engineering problem.** Multiple deployments failed because of normalization, scaling, or data format mismatches between training and inference. The fix: one shared code path for feature computation, normalization stats saved alongside the model checkpoint, and integration tests that verify both pipelines produce identical outputs for the same input.

**The most dangerous model is the one that succeeds on bad data.** At one point, three simultaneous data bugs (inverted bid/ask, 3000× spread mismatch, wrong book depth indexing) went undetected because the model trained on corrupted data still reported strong validation metrics — on test data with the same corruption. A model that fails obviously is easy to catch. One that looks great on flawed data ships to production.

---

### After V3.7: The Pivot Spiral

V3.7 was the most successful deployment, but it had real problems. Several output heads weren't contributing meaningfully — fill probability clustered at 0.90 and adverse selection probability at 0.10 on every tick, providing no useful gating signal. The MFE/MAE heads predicted excursions that never materialized in live (MFE=90 bps predicted vs actual peaks of ~17.6 bps), which set unrealistic take-profit and stop-loss levels. The architecture itself — Transformer + MoE — was sound, but the head-level training needed work.

Rather than fixing those specific heads, I kept pivoting:

**V3.8** tried to address the head issues directly — removed the broken Softplus activation on the risk head, masked regression losses to exclude neutral rows, made the edge head direction-aware. Validation accuracy dropped to 86.3% (from 87.7%) but the metrics were more honest. An incremental step in the right direction, but I moved on before proving it out live.

**V2.0–V2.4** reverted to earlier checkpoints with different TP/SL configurations. V2.0 cut winners too early (TP=10 bps). V2.4 tried balanced class resampling in training and overfit badly — 13% win rate over 53 trades. These were parameter search when the problem was upstream data quality.

**V3.0 (DirectionCNNGRU)** was a complete architecture pivot — abandon engineered features, feed raw orderbook tensors (50 timesteps × 40 levels × 3 features) through Conv1D → GRU. ~235K parameters, <2ms inference. The idea was to let the network learn its own features from book shape. The model itself was reasonable, but three simultaneous data bugs in the training pipeline (inverted bids/asks, 3000× spread normalization mismatch, wrong book depth indexing) meant it never had a fair shot. Iterating through V3.0a–V3.0c chased symptoms without fixing the root cause.

**The Databricks rebuild** — stepping back from models entirely, I discovered the production feature tables themselves were corrupt. The pipeline was treating orderbook delta messages as full snapshots instead of statefully reconstructing the book, inflating spreads 32–118× (0.5–1.3 bps in training vs 0.015 bps in live). Every model trained on these features had learned distorted reality.

**The LLM agent pivot** was the most radical departure — replace ML direction prediction with Claude Opus strategic reasoning via OpenClaw, relegate the orderbook to an execution timing overlay, and target longer horizons (80–500 bps) where fees don't matter.

In hindsight, V3.7's architecture was the right foundation. The problems were fixable: retrain the underperforming heads, enforce feature parity between training and live, and clean up the data pipeline. Instead of iterating on what was working, I kept starting over.

---

### Data Engineering: The Real Work

The most time-intensive part wasn't models — it was data:

- **363M+ orderbook rows** via a 24/7 WebSocket recorder (depth-50, 100ms, ~31 msg/sec, JSONL.gz with 10-minute rotation)
- **Databricks Delta Lake pipeline:** raw → silver (stateful book reconstruction) → features → labels
- **Memory engineering:** The orderbook tensor (50×40×3 = 6,000 floats/row) causes OOM at scale. Spark's `ArrayType(FloatType())` creates 168KB per-row overhead — 840 GB for 5M rows. Storing as `BinaryType` with `np.frombuffer()` reconstruction dropped peak memory to ~50 MB.
- **Stateful book reconstruction:** The raw data contains snapshots and deltas. Treating deltas as full snapshots inflated spreads 32–118×. Correct reconstruction requires maintaining book state and applying deltas incrementally.

---

### Where It Stands Now

The data pipeline has been rebuilt correctly — stateful book reconstruction produces spreads of ~0.015 bps (matching live), and the corrected silver table is validated. The blocking work right now is regenerating features and labels from this clean foundation.

From here, there are a few directions worth exploring:

**Revive the V3.7 architecture on clean data.** The Transformer + MoE design was marked "REPLAY" in the migration plan — worth benchmarking as a retrained model on the corrected feature set. The architecture produced a 40.5% win rate with positive PnL despite training on bad data and having multiple broken output heads. Fixing the data, retraining the heads that weren't contributing, and enforcing feature parity could be the shortest path to a working system.

**Train a new model at longer horizons.** The research showed price is not a random walk (Hurst 0.64–0.99, strongly trending at all horizons). L2 signal decays to near-zero at 5+ minutes, but trend-based labeling at 180s/300s/600s horizons could work with different feature inputs — multi-timeframe trend alignment, sentiment, volume confirmation — where the target move dwarfs fees.

**LLM-orchestrated trading.** The two-layer architecture (fast Sentinel for execution timing + slow LLM for strategic reasoning) is built and the skill framework is defined. This is the highest-risk, highest-optionality path — using Claude for multi-timeframe analysis across 700+ pairs rather than trying to teach a neural network to predict direction from L2 data alone.

None of these paths are mutually exclusive. The gate model survives across all of them as an execution timing overlay.

---

### Key Takeaways

1. **Start with feasibility math.** Expected edge minus fees minus adverse selection minus fill slippage — is it positive? Do that calculation before writing training code.
2. **The orderbook is an execution tool, not an alpha source.** L2 data excels at timing entries, not predicting direction.
3. **Adverse selection is the dominant force in limit order trading.** Model it explicitly or it will eat your edge silently.
4. **Data pipeline correctness is a prerequisite, not a feature.** Corrupt training data produces confident, wrong models... obviously.
5. **Live markets are the only real validation.** Backtests and validation metrics exist to give you confidence to go live, not to tell you your system works.

---

### What's Next

The prop firm challenge is the next milestone. The data pipeline is being rebuilt correctly, the strategy has shifted to horizons where edge significantly exceeds costs, and an LLM agent approach is being tested for strategic reasoning.

Whether it works or not, I'll document it here.
