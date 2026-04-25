---
title: "From Models to Mechanics: The HMS-30 Pivot"
summary: "What happened after the L2 ML lineage was killed — a clean repo, feasibility math first, 14 numbered experiments, and a no-ML spread-momentum strategy that finally cleared the prop firm rules."
date: 2026-04-25
tags:
  - trading
  - quant
  - python
  - data-engineering
draft: false
pinned: true
---

## From Models to Mechanics: The HMS-30 Pivot

The [previous post](/posts/trading-bot-journey) ended with a rebuilt data pipeline, a V3.7 architecture worth replaying on clean features, and three plausible directions: revive V3.7, train at longer horizons, or pivot to LLM orchestration. I tried two of those directly, killed both, then took a fourth path that wasn't on the list — a no-ML spread-momentum strategy on 30 cointegrated perp pairs.

This is the story of the six weeks between "the data is clean now" and "the bot is live with a strategy I can actually defend."

---

### Step zero: a new repo

The old repo (`bybit-l2-bot`) carried two years of architectural assumptions — Databricks Delta Lake, Spark feature jobs, a `live/` folder that imported transitively from a `training/` folder that imported from `databricks/notebooks/`. Every change touched four layers. Every deploy was a leap of faith because the training-vs-live parity surface was enormous.

I started a clean repo, `hyro-trader`, with three rules:

- **No Databricks in the runtime path.** Research can use it; live cannot depend on it.
- **One feature code path.** The function that produces a feature in training is literally the same function imported into the live runner. No re-implementations.
- **Versioned model artifacts.** Every model gets a SemVer tag baked into the file name and the Docker image tag. No more "is this the model that was live last Tuesday?"

The first commit was a port of the cleaned-up local pipeline — kline loader, feature builder, label generator, trainer, backtest harness, live runner — about 8K lines, all running on a laptop. No cloud dependency for research. Cloud only for deploy.

That last constraint matters more than it sounds. With the old repo I'd spend a day spinning up a Databricks cluster to test a feature change. With `hyro-trader` I can iterate on a feature in a Jupyter cell, run the backtest, and have a verdict in ten minutes. The throughput difference is what made fourteen numbered experiments tractable.

---

### The bugs I made sure to carry the fix forward for

The previous post documented three simultaneous data bugs in the old pipeline: inverted bid/ask, 32–118× spread inflation from treating delta messages as snapshots, and wrong book depth indexing. The new repo had to inherit the fixes, not the bugs.

What I actually built into `hyro-trader` from day one:

- **Stateful book reconstruction in the data loader itself.** Snapshots seed state, deltas mutate it, and the test fixture replays a known sequence and asserts the reconstructed top-of-book matches a hand-computed reference. If anyone ever swaps in a new parser, the test fails before merge.
- **A parity test runner.** [`live_hms30/parity_test.py`](https://github.com/SlaytonNichols/hyro-trader/blob/main/live_hms30/parity_test.py) replays a window of historical bars through the live engine and asserts the resulting trades match the backtest sim trade-for-trade, side, size, and entry/exit price. Any drift between research and production fails CI.
- **One feature module, two callers.** Both the training notebook and the live runner import from the same module. Normalisation stats live next to the model checkpoint as a sidecar JSON, loaded by both paths.
- **Deterministic seeds everywhere a random number is drawn.** Reproducibility was the surprise prerequisite — without it, "I retrained and the metrics moved 2 bps" is unrepairable.

None of this is glamorous. All of it was the actual lesson from the prior repo.

---

### Feasibility math, before any training code

The biggest single change in approach: I refused to train another model until I'd done the cost arithmetic on paper.

For Bybit perpetuals at VIP 0:

- Maker rebate: −1.0 bps per side (paid to you)
- Taker fee: +5.5 bps per side
- Adverse selection on filled maker orders: empirically 4–6 bps in the live data from V3.7
- Slippage on taker market orders: 0.5–2 bps depending on size

A round-trip maker-only trade at the cheapest plausible cost still has to overcome about **8 bps** of adverse selection net of rebates before the strategy makes a cent. A round-trip taker trade is at **11 bps** of fees alone.

So the question became, for any candidate strategy: what is the predicted edge per trade, and is it bigger than the fee+adverse stack with comfortable margin?

The 30-second L2 prediction work from V3.7 had a realised median edge of about 17 bps gross — bigger than the fee stack, but not by much, and the variance across trades was enormous. A strategy whose edge sits at 1.5× its cost barrier needs a directional accuracy that L2 alone could not deliver (every model in the prior post converged to ~53% accuracy — noise-floor on the binary direction question).

The math told me, before I retrained anything, that V3.7 was a borderline-profitable architecture trying to clear a margin that didn't exist. I tried it anyway, because I had to be sure.

---

### v0.0.1 → v0.2.4: the retrain on clean data

The first model lineage in the new repo was a faithful retrain of V3.7's Transformer + MoE on the corrected silver table.

- **v0.0.1** — initial trained model under the new versioning scheme, baseline parity check passed, deployed to demo.
- **v0.1.0** — added isotonic calibration on the entry-probability head. Live calibration matched offline within 1 percentage point — the parity work paid off immediately.
- **v0.2.0** — full inference-layer rewrite: gate model, edge head, calibration, and a meta-classifier that ranked candidate trades. Phase 8a–8e in the worklog.
- **v0.2.1** — log-ingest pipeline plus execution fixes from the second live trade (a bad fill that exposed an order-state bug).
- **v0.2.2** — throughput-loosening profile (gate 0.40, edge 3 bps, meta 0.30) to get more trades per day. Reverted four hours later.
- **v0.2.3** — reverted v0.2.2's loosening, added shadow logging so I could see what the loose profile *would* have done without taking the trades.
- **v0.2.4** — sizing 50× plus 3% hard risk cap plus a WebSocket-first order path. The architectural ceiling of the v0.2.x line.

v0.2.4 ran live on demo for two weeks. It made trades. The trades did not, on average, make money. So I stopped iterating on parameters and built the offline backtest harness to answer one specific question: is the model ever going to be profitable, even with perfect execution?

That work became Leg A.

---

### Leg A: kill the lineage on backtest before spending more compute

Three pieces of analysis, each of which independently would have been enough to stop:

**A1 — backtest harness mirroring the live decision chain.** Re-ran the model's published predictions through the same gate/floor/meta thresholds the live bot uses, against the realised next-30s mid moves. Net P&L per trade across 6 walk-forward folds: **−0.82 bps**. Negative. At every threshold and every execution config I tested.

**A4 — direction-head audit.** Isolated just the directional prediction, scored AUC across the same 6 folds: **0.55 ± 0.01**. A coin-flip-class model with a tiny tilt — the same ceiling every L2 direction model in the previous post hit.

**A5 — oracle bound.** Re-ran the backtest assuming the model picked direction correctly 100% of the time, but kept its own entry-probability and edge predictions for selecting *when* to trade. Result: still net-negative after fees, because the model was confidently entering at the wrong moments — high entry-probability did not correlate with high realised edge.

The verdict in [`docs/LEG_A_BACKTEST_v021_2026-04-22.md`](https://github.com/SlaytonNichols/hyro-trader/blob/main/docs/LEG_A_BACKTEST_v021_2026-04-22.md) was: **kill v0.2.x**. No more retraining on this architecture. The substrate (30s horizon, L2-only features) cannot produce an edge larger than the cost of trading it.

That decision saved months. The temptation when something doesn't work is to tune. The lesson from V3.7 was that tuning a marginal architecture produces marginal improvements that don't survive walk-forward. I needed to find an architecture that worked, not a parameter set that masked the failure.

---

### Leg B: the cheap shots

Two parallel one-GPU-day experiments designed to answer two diagnostic questions about the AUC=0.55 failure:

**B1 — signed-edge regression.** Hypothesis: the loss function was the problem. Replaced BCE direction + Huber edge with a single signed-edge regression head, so conviction and direction shared one scalar. Result: 3-fold walk-forward AUC ≤ 0.532. **Killed.**

**B4 — order-flow-only features.** Hypothesis: the feature set was the problem. Stripped from 73 features to ≤15 raw order-flow primitives — signed taker volume, cancel rate, queue decay, microprice deviation, realised vol, top-5 book imbalance, spread. Trained direction-head only. Result: failed to clear the bar.

Two diagnostic questions, two clear nos. When both cheap shots fail you've learned something specific: it's not the loss, it's not the feature engineering — the **30-second direction prediction problem on local-book features is intractable**. Every model converges to noise floor.

That's a useful negative result. It rules out an entire family of strategies.

---

### Leg C: was the substrate even alive?

Before pivoting to a different strategy class entirely, I wanted to be sure the L2 substrate hadn't been poisoned by something else I'd missed.

| | What | Verdict |
|---|---|---|
| **C1** | v0.2.1 feature-by-feature signal audit | dead at h=30s |
| **C1b** | Signal decay by horizon (1s → 10000s) | ceiling ~+1 bp at h=10000 |
| **C1c** | Tape (taker imbalance / signed vol) added to features | tape adds no signal |
| **C2** | Minute-bar horizon audit (R9 swing model) | dead |
| **C3b** | Funding / OI / basis / cross-venue | dead |
| **C4** | L2 100ms re-audit on cleaned silver | signal alive at deep quantiles |
| **C5** | GBT stack on 17.6M rows of 100ms data | converges, signal real, no lift |
| **C6** | Feature expansion (deltas + tape + vol + tod) | +2.32 bps at h=10000 q=0.999 |
| **C7** | Walk-forward stability check on C6 | C6 was period luck — killed |

C7 was the one that hurt. C6 looked like it had cleared the maker-fee gate at the deepest quantile of conviction — a real, exploitable edge. Walk-forward broke it across folds. The signal was a single-period artefact, not a stable pattern.

After C7 I stopped trying to make L2-derived features work for directional prediction, regardless of horizon, regardless of feature set, regardless of architecture. The substrate is exhausted for that class of question.

---

### The pivot: stop trying to predict price

By the end of Leg C the conclusion was clear: any strategy that depends on predicting BTC's next move from L2 features is fighting an edge-vs-cost ratio that doesn't close. The two remaining categories of edge that *don't* require a directional prediction are:

1. **Carry / funding-rate harvesting** — collect a known cash flow, hedge the directional exposure to zero.
2. **Statistical arbitrage** — find two assets whose relative price is more predictable than either's absolute price.

I worked through both.

**E10 — delta-neutral funding harvest.** Long perp + short spot (or vice versa, depending on the funding sign), capture the 8h funding payment. Backtest result: **1.52% APR maker, 0.95% APR taker.** Real edge, but an order of magnitude too small to clear a $200K prop firm in any reasonable time. Killed for the prop-firm goal; kept the code as a future capital deployment idea.

**E11a/b/c — multi-symbol funding screen and inverse-vol basket.** Tried building a basket of the highest-funding symbols, inverse-vol weighted. Sharpe 6.79 in the basket sim, MDD −6 bps. Beautiful numbers, tiny absolute returns — about 4% APR. Same problem as E10, just more elegant. Killed.

**E12 — statistical arbitrage on Bybit perps.** Cointegration screen across the top 50 USDT-M perps. One pair survived with an edge worth trading at the prop-firm size: **BTC-SOL at 1h bars, z-entry 4 / z-exit 8** — and only as a mean-reversion strategy. Marginal.

E12 was the moment I realised the screen itself was the discovery, not BTC-SOL specifically. If a brute-force cointegration sweep finds *any* tradable pair on perps — an asset class that's supposed to be fully arbitraged — then a more careful sweep across more pairs and a different signal direction might find more.

---

### E13: stop fading, start chasing

The next experiment changed the result entirely. Instead of trading the cointegrated pairs as mean-reversion (the textbook approach), I tested entering **in the direction of the spread breakout** — momentum, not reversion.

The thesis: when a cointegrated pair's log-spread snaps past 2σ, other participants pile into the dislocating leg before mean-reversion sets in. The breakout has its own short-horizon momentum signature, even when the longer-term relationship is mean-reverting. Trail out as the z-score retraces from its peak; you keep most of the breakout move and avoid waiting through the slow drift back to the mean.

E13 the experiment was: implement that on a 5-pair seed universe, walk-forward across 161 days, check whether the portfolio P&L survives HyroTrader's 4% / 6% drawdown caps.

It did. Mean daily P&L positive, worst drawdown well inside the rules, no losing days in the simulation. I spent a week trying to break it with adversarial tests:

- **E13e — regime split.** Bull, bear, sideways sub-periods, each tested independently. Every sub-period passed independently. The strategy isn't a regime artefact.
- **E13f — slippage stress.** Doubled, then tripled, the slippage assumption. Still profitable at 3× estimated slippage. Spread-momentum is structurally slippage-tolerant because the entry signal is the spread's own move; even a few extra bps of execution cost is small relative to the breakout magnitude.

I could not break it. So I scaled it.

---

### E14: 30 pairs, not 5

The five-pair universe was a sanity check. The full version sweeps every plausible USDT-M perp pair on Bybit, runs an Engle-Granger cointegration test on log-prices over a rolling window, ranks by half-life and stationarity, and selects the top 30 that survive a portfolio-level drawdown screen.

Most of the universe is uninteresting — BTC and ETH against everything else mostly co-move with crypto-beta, not with each other in a useful way. The pairs that work are predominantly **altcoin-vs-altcoin**: SUI-NEAR, LINK-AVAX, DOGE-SHIB, that flavour. They share sector flows, but the lead/lag is short enough that the breakout is tradable before the mean-reversion arrives.

E14b validated the 30-pair portfolio under HyroTrader rules with a 3.0× sizing multiplier (calibrated so the historical 5th-percentile worst trade equals 2% of the per-pair allocation, comfortably under the 3% per-trade hard cap). The headline numbers from the 161-day backtest:

| Metric | Value |
|---|---|
| Daily P&L (mean) | +$11,544 |
| Worst single trade | −1.68% |
| Worst daily DD | −0.66% |
| Days with drawdown breach | 0 |

Those are simulated numbers, with realistic taker-fee assumptions and a slippage stress passed. They are not promises about live performance. But the backtest cleared the cost barrier with an order of magnitude of margin, which is the right side of the feasibility math for the first time in eight months of work.

---

### Landing the strategy: HMS-30

The internal experiment label "E13" is a useless name for the strategy that's now running production capital. So I renamed it: **Hyro Momentum Spread 30**, or HMS-30. Thirty pairs. Spread momentum. Hyro because the prop firm is HyroTrader.

The whole strategy fits in about 800 lines of Python:

- `live_hms30/spread_engine.py` — for each pair, every 1h bar: rolling 48-bar OLS beta of `log(price_A)` on `log(price_B)`, form the log-spread, z-score it, return a target side when `|z| > 2.0`.
- `live_hms30/data_fetcher.py` — Bybit REST `/v5/market/kline`, fetch the last 96 1h bars per symbol. No WebSockets, no L2, no historical store.
- `live_hms30/order_layer.py` — atomic dual-leg market entry, exchange SL attachment within seconds of fill, mark-price fallback for stops on profitable positions.
- `live_hms30/live_runner.py` — wake at each bar close + 5s, evaluate all 30 pairs, manage state, enforce daily/total drawdown halts, persist to a mounted Azure file share for crash recovery.

It runs as a single Azure Container Instance in `southeastasia` (lowest Bybit API latency from any AWS/Azure region I tested). It sleeps 99% of the wall clock; it wakes once an hour for a few seconds of Python. The ACI bill is under $20/month. There is no GPU. There is no model. There is nothing to retrain.

The thing I built for eight months had a Transformer encoder, a Mixture of Experts, a CNN-GRU sibling, an analytical queue model, an isotonic calibrator, and a meta-classifier ranking layer. The thing that's actually live is rolling regression and a z-score.

---

### What I'd do differently from the start

- **Feasibility math first.** Round-trip cost, required edge, plausible edge from your hypothesis, margin between the two. If you can't write that on a napkin and feel comfortable about the margin, training a model isn't going to discover one.
- **Negative results are progress.** Leg A killed v0.2.x in one document. Leg B's two failures killed an entire class of architectures. Leg C's eight experiments killed the L2 substrate as a directional source. Each "killed" was hours of work that saved weeks of additional dead-end iteration.
- **Local-first research.** A pipeline that runs on a laptop and produces a verdict in ten minutes will produce ten times the experiments of a Databricks pipeline, regardless of which one is "more powerful."
- **The simplest strategy that clears the cost barrier wins.** A complex strategy with a marginal edge loses to a simple strategy with comfortable margin every single time, because the complex one breaks in production in ways the backtest didn't anticipate.
- **Statistical relationships beat directional prediction.** If you can find two prices whose relative move is more predictable than either's absolute move, you've moved the goalposts from "predict the market" to "predict the spread" — a much easier problem at the cost of being capacity-limited.

---

### Where it stands now

HMS-30 has been live on Bybit demo for several days under the canonical name (`hyro-trader-hms30` ACI container, single-instance, file-share-persistent state). The infrastructure migration from the legacy E13 naming completed cleanly: state files copied, container redeployed, old artefacts deleted. The bot is reconciling 7 open positions against the exchange, running its hourly evaluation loop, and writing trades to `/logs/hyro-trader-hms30/live_trades.csv`.

The decision criterion for buying the actual prop-firm challenge is 5–7 days of live data with a daily mean inside one standard deviation of the backtest, and zero compliance violations (no trade larger than the per-trade cap, no SL detachments, no daily DD breach). If those hold, real money goes on it.

If they don't, I still have a clean repo, a working research pipeline, and fifteen experiments worth of negative results to draw on for the next attempt. That's a much better starting position than I had eight months ago.

The previous post ended with "whether it works or not, I'll document it here." This is the documentation. Whether HMS-30 passes the challenge is now a question about live execution, not a question about whether the strategy is real. I'll write the next post when I know.

---
