---
title: "Trading Bot Journey"
summary: "A year building a Transformer + Mixture-of-Experts model for crypto microstructure trading on Bybit. The architecture, the training pipeline, the diagnostic experiments that ruled out the strategy class, and what I came away with."
date: 2026-04-29
tags:
  - ml
  - trading
  - data-engineering
  - python
draft: false
pinned: true
---

## Trading Bot Journey

This is a writeup of the machine-learning project I spent most of the last year on: an attempt to forecast short-horizon outcomes from limit-order-book data on BTC/USDT perpetual futures, using a Transformer encoder feeding a Mixture-of-Experts head. Repo here: [hyro-trader-ml-project](https://github.com/SlaytonNichols/hyro-trader-ml-project).

The trading P&L on this project was mixed. The system and the methodology I came out the other side of it with are the durable artifacts, and that's what this post is about. A companion post, [A Working Toolkit for Ambiguous, Data-Heavy Problems](/posts/trading-bot-journey-part-2), pulls the methodology pieces out of the trading context.

---

### The thesis

The goal was to pass a $200K prop-firm challenge. The original approach was to forecast **microstructure response patterns** at the limit-order-book (L2) level — absorption, liquidity vacuums, exhaustion — instead of trying to predict price direction directly. Edges of a few basis points per trade, maker-only execution, high frequency.

I started with handcrafted L2 signals on QuantConnect LEAN, built a 24/7 Bybit WebSocket recorder that eventually accumulated 363M+ orderbook rows, and validated five signal hypotheses on rolling walk-forwards. Three of the five looked promising. The limitation was that I was encoding *my own assumptions* about which patterns mattered, rather than letting the data identify them. That pushed me toward neural networks.

---

### Train on outcomes, not on patterns

The reframing was: instead of labeling a row "this is an absorption pattern," label it with what actually happened next — did price move favorably, by how much, did you get adversely selected, did the order even fill? Then let the model learn which input patterns lead to which outcomes.

The architecture I ran live (V3.7) is in `models/` and `training/` in the repo:

- 4-layer Transformer encoder, ~850K parameters
- 8-expert Mixture-of-Experts head with a learned gating network
- 89 input features across L2 depth, queue dynamics, temporal, and seasonality groups (`features/`)
- 7 output heads — entry probability, direction, expected edge in bps, MFE, MAE, adverse-selection probability, fill probability (`labels/`)
- Trained on Databricks with V100 GPUs over ~1.5M samples at 100ms resolution

It went live on January 31, 2026. Across 437 trades over six days it ran a 40.5% win rate with positive overall P&L. The most useful observation from those six days had nothing to do with the architecture: **fast fills were a warning sign.** Orders filled in under 5 seconds averaged negative P&L; slower fills, where I was actually providing liquidity, were profitable. Fast fills usually meant somebody was running through my level — adverse selection. That observation reframed the whole problem from "which way is price going" to "can I get filled in a place where I'm providing genuine liquidity instead of getting picked off."

---

### Then I noticed the data was wrong

While the model was live, I went back and audited the training pipeline. I found three simultaneous bugs:

- Bid and ask were inverted on snapshots.
- Spreads were inflated 32–118× because the pipeline was treating delta messages as full orderbook snapshots instead of statefully reconstructing the book (`data/book_parser.py` is the corrected version).
- Book depth indexing was returning the wrong levels.

The model was scoring well on validation despite all of this, because the validation data was corrupted in the same way. A model that fails obviously is easy to catch. One that looks good on flawed data ships.

I stopped iterating on architecture and committed to three constraints I'd been working without:

1. **No Databricks in the runtime path.** Research can use it; live execution cannot depend on it.
2. **One feature code path.** The function that produces a feature in training is the function imported by the live runner. Same code, not a "ported" version.
3. **Versioned model artifacts.** Every checkpoint gets a SemVer tag, with normalization stats, feature list, training config, and git SHA bundled in (see `docs/VERSIONING.md`).

Then I rebuilt the pipeline correctly, regenerated features and labels on clean data, retrained, and asked one question: with the data fixed, does the model work?

---

### Three diagnostic tests that ruled out the strategy class

Rather than tweak the model and hope, I designed three orthogonal experiments. The point was: if all three say the same thing, the strategy class itself is the constraint, not the implementation.

**Leg A — oracle direction test.** Take the model's actual predictions but assume perfect direction picking. At the deployed entry threshold, the result was −0.82 bps per trade. Sweeping the threshold up, I could find a setting that returned +2.94 to +6.44 bps per trade — but the trade count collapsed from ~9/day to ~1/day. There was a profitable signal in the data, just not enough qualifying setups to deploy capital against.

**Leg B — cheaper alternatives.** I tested whether the dual direction/edge heads were fighting each other (a unified signed-edge regression head) and whether the 73-feature set was too noisy (a stripped-down 15-feature order-flow-only model). Both performed worse than the baseline.

**Leg C — substrate audit.** Before declaring L2 dead, I tested whether *any* feature combination at *any* horizon could clear the 4 bps maker-maker fee floor. Best L2 feature at best horizon: +0.44 bps. Adding tape-derived features: +0.06 bps. Resampling to 1-minute bars and testing 5/15/60-minute horizons: nothing crossed. One expanded feature set hit +2.32 bps on a single test split, then collapsed to a +0.93 bps median across five independent walk-forward folds, with two folds going negative — period-specific luck, not durable edge.

The full writeups are in `docs/` (`LEG_A_*`, `LEG_B_*`, `LEG_C_*`, `SUBSTRATE_VERDICT_*`).

Three orthogonal tests, same conclusion. At retail latency and retail fees, the L2 substrate doesn't carry enough signal to support a price-prediction strategy. That conclusion was worth more than another quarter of architecture iteration.

---

### A brief detour: trading relationships instead of direction

Once the L2 direction approach was off the table, the natural next question was whether some *pair* of prices has a more predictable relative move than either's absolute move. Two cointegrated perps drift around a stable spread; when the spread dislocates, you can trade the dislocation without taking a directional view on either leg.

I built and deployed a spread-momentum system across 30 cointegrated perp pairs (z-score entries on the rolling log-spread, momentum exits). It ran on Azure Container Instances under $20/month with no GPU dependency — a cleaner deployment than V3.7 in every respect. The signal was real; under honest cost assumptions it wasn't strong enough to justify the prop-firm challenge cost against it. So I parked it.

That work lives in a separate repo. The point for this post is that the diagnostic discipline from the L2 work — feasibility math first, walk-forward folds over single splits, design experiments that could prove you wrong — transferred directly and saved months.

---

### What I came away with

This was my first end-to-end ML system, and a lot of what I'd carry into the next one is engineering and methodology rather than architecture. Some of these will read as obvious — they're on the list because they were genuine lessons the first time through, and now they're things I'd build in from the start.

1. **One feature code path across training and live.** Computing the same feature in three places — Spark notebook, local backtest, live runner — guarantees they will diverge. Pin one implementation, import it everywhere.

2. **Training/serving parity tests on every deploy.** Push the same raw input through the training feature path and the live feature path; assert the resulting tensors are equal. This catches an entire class of "the model worked yesterday and is broken today" before it happens.

3. **Cheap data-sanity checks before any model sees the data.** Things like "ask price should be greater than bid price" or "spread shouldn't be 100x normal" are one-line assertions. If you put them at the point where data enters the pipeline, the bad data fails loudly instead of silently corrupting training. The three bugs from this project would have all been caught by checks of this kind.

4. **Versioned, immutable model artifacts.** SemVer the checkpoint. Bundle normalization stats, feature list, training config, and git SHA into the artifact itself.

5. **Test on multiple time periods, not just one.** If you only evaluate a model on one slice of data, a good result might just be luck — that particular slice happened to suit the model. The fix is to repeat the evaluation across several different time windows and look at the *spread* of results. In this project, an experiment looked great on one slice (+2.32 bps) and almost broke even when re-tested across five slices (+0.93 bps median, two of them losing money). The single-slice number was the lie. Catching that before deploying capital is much cheaper than catching it after.

6. **Live as the honest evaluator.** Backtests and validation metrics exist to give you confidence to *go* live, not to certify that the system works. Adverse selection, fill latency, requote dynamics, exchange quirks — these only show up under real conditions.

7. **Feasibility math at the start, not after the model.** Expected edge minus fees minus adverse selection minus slippage. If that isn't positive with margin, no amount of model tuning fixes it. The L2 work had a real signal but ~4 bps of best-case fees against a similar-magnitude per-trigger payoff. A 20-minute calculation up front sets the right expectations.

The transferable version of these — pulled out of the trading context — is in [A Working Toolkit for Ambiguous, Data-Heavy Problems](/posts/trading-bot-journey-part-2).
