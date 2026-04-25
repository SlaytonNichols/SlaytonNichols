---
title: "Trading Bot Journey, Part 2: Killing Ideas That Don't Work"
summary: "What happened after fixing the data pipeline. Three diagnostic experiments that killed the entire L2 direction-prediction strategy class. A strategic pivot to stat-arb. And the HMS-30 spread-momentum strategy that cleared the cost barrier."
date: 2026-04-25
tags:
  - trading
  - quant
  - systems
  - experimentation
draft: false
pinned: true
---

## Trading Bot Journey, Part 2: Killing Ideas That Don't Work

In [Part 1](/posts/trading-bot-journey), the story ended with a decision: rebuild the data pipeline correctly, retrain the V3.7 architecture on clean data, and then decide whether the L2 directional approach was salvageable.

This post is what I learned from running three systematic tests that said "no."

---

### Part 1 recap: the data pipeline was corrupted

The [previous post](/posts/trading-bot-journey) documented the V3.7 architecture (Transformer + MoE): 850K parameters, 89 features, 7 output heads. Trained on Databricks, validated on 1.5M 100ms samples from BTC/USDT orderbook data. The model showed promise in theory.

Then I discovered three simultaneous data bugs:

- Inverted bid/ask on snapshots
- 32–118× spread inflation from treating delta messages as full snapshots
- Wrong book depth indexing

The model had trained on corrupted data *and still reported good metrics*. A model that fails obviously is easy to catch. One that looks great on flawed data ships to production. I rebuilt the pipeline and tried again on correct data.

---

### The rebuild: hyro-trader from ground zero

I started a new repo (`hyro-trader`) with three hard rules:

1. **No Databricks in the runtime path.** Research can use it; live execution cannot depend on it.
2. **One feature code path.** The same function that produces a feature in training must be imported directly into the live runner. No re-implementations.
3. **Versioned model artifacts.** Every model gets a SemVer tag. No "is this from last Tuesday?"

I ported the cleaned-up local pipeline — about 8K lines running on my laptop. Then I retrained V3.7 on the corrected data and asked one question:

**If the data was the entire problem, does the model work now?**

---

### The three-test framework

I designed three diagnostics to isolate which constraint was binding:

1. **Leg A**: Can perfect direction picking save it? (oracle test)
2. **Leg B**: Do better features or loss functions help? (cheap alternatives)
3. **Leg C**: Can any L2-only substrate generate an edge? (substrate audit)

If all three fail, the conclusion is: **this strategy class is fundamentally constrained, not just tuned wrong.**

---

### Leg A: oracle direction test

**Setup:** Take the deployed model's actual predictions, but assume perfect direction picking. Keep everything else (entry timing, position sizing, exit logic) from the live run.

**Direction accuracy across 6 walk-forward folds: 0.552 ± 0.01**

That's noise floor. The model can't tell buy from sell.

**Then I ran the oracle test:** assume the direction is always right, but use the model's own entry probability, exit logic, TP/SL thresholds, and fee assumptions.

**Result at the deployed entry gate (0.50): −0.82 bps per trade.**

But then I swept the entry gate. At gate=0.65, the same oracle test returned **+2.94 to +6.44 bps per trade** (pessimistic to optimistic bounds). That clears the 4 bps maker-maker round-trip fee floor with margin.

**The catch:** Gate=0.65 collapsed from 8.7 trades/day to 1.3 trades/day. You can catch the big, clean moves — but there are almost no big, clean moves.

**Conclusion:** The direction signal is real, but starved for throughput at any profitable threshold. Even when you raise the bar high enough to be profitable per trade, the number of qualifying trades becomes so small that the strategy can't sustain capital deployment.

---

### Leg B: cheaper alternatives

**B1 — signed-edge regression head.** Maybe the dual `(direction, edge)` heads were fighting each other. What if a single signed-edge regression head unified them?

Result: AUC 0.518–0.532 across 3 folds (before I halted to save compute). **Worse than baseline.**

**B4 — order-flow-only features.** Maybe the 73-feature set was noisy. What if I stripped to 15 raw order-flow primitives (imbalance, queue decay, trade intensity)?

Result: Still failed the walk-forward pass bar.

**Conclusion:** The constraint is not the loss function or feature engineering. It's the underlying prediction problem. The data doesn't carry enough signal.

---

### Leg C: L2 substrate audit

Before declaring L2 dead, I audited whether *any* feature combination could clear the 4 bps maker-maker fee floor at any horizon.

**C1b — horizon decay:** Best L2 feature (book imbalance) at best horizon and quantile: +0.44 bps per trigger. Below the fee floor.

**C1c — tape-derived features:** Added taker buy/sell vol, aggressor imbalance, trade count at multiple windows. Best payoff: +0.06 bps. Tape features were weaker than L2 imbalance and showed the same mean-reversion ceiling.

**C2 — multi-minute horizons:** Resampled to 1-minute bars, tested 5/15/60-minute prediction horizons. Pass bar was 6 bps (2bps fees + 50% buffer). No feature crossed it.

**C4 — 100ms substrate re-audit:** The 100ms L2 data did show real signal (book imbalance at deep quantiles was sp=+0.33 at 1s horizon), but the per-trigger payoff was only +0.7–0.9 bps. Not enough to clear taker fees.

**C6 — expanded features:** Added L2 deltas, realised vol, time-of-day, tape intensity. Best test result: **+2.32 bps at a deep quantile and long horizon.** This actually looked promising.

**C7 — walk-forward stability check on C6:** Reran the same model on 5 independent calendar folds. Median payoff dropped to +0.93 bps. Two folds went negative. The +2.32 was period-specific luck, not durable.

**Conclusion:** The L2 substrate is exhausted. There is no feature combination that will clear fees reliably.

---

### Strategic pivot: stop asking L2 to predict price

When every documented test says a strategy class doesn't work, the correct move is not to iterate harder. It's to ask a different question.

**Old question:** Can I predict BTC's next-30s move from L2 data?  
**New question:** Which *pair* of prices has a more predictable relative move than either's absolute move?

This opens two strategy classes:

1. **Carry harvesting** (E10): Collect funding rate payments on perpetual swaps, hedge delta to zero. Result: 1.5% APR on BTC-USDT. Real edge, but too small.
2. **Statistical arbitrage** (E12+): Find cointegrated perp pairs where the spread mean-reverts. Backtest on historical pairs.

I implemented both. Carry was too slow. But the stat-arb approach on pairs like BTC-SOL showed traction.

---

### E13: momentum on spreads, not mean-reversion

The key insight: cointegrated pairs that *break* from their mean don't immediately snap back. They often have momentum first.

**Thesis:** When a pair's log-spread exceeds 2σ, short-term participants pile into the dislocating leg. The breakout carries momentum before mean-reversion kicks in. Enter in the direction of the breakout, trail out as the z-score retraces 1.0 from the peak.

On 5 seed pairs over 161 days under HyroTrader risk caps (4% daily DD, 3% per-trade SL):

- Mean daily P&L: positive
- Worst daily DD: −0.66%
- No losing days
- Regime split: passes independently in bull, bear, sideways
- 3× slippage stress: still profitable

I could not break it. So I scaled the pair universe.

---

### E14: 30-pair portfolio

Engle-Granger cointegration screen across all Bybit USDT-M pairs, ranked by half-life and stationarity, validated as a portfolio under HyroTrader constraints with 3.0× sizing:

| Metric | Value |
|---|---|
| Mean daily P&L | +$11,544 |
| Worst single trade | −1.68% |
| Worst daily DD | −0.66% |
| Days with breach | 0 |

**Critical difference from the L2 work:** This strategy clears the cost barrier with *order-of-magnitude margin*. The 11+ bps mean edge covers 11 bps round-trip fees and leaves real room.

---

### HMS-30 deployment and live operations

Hyro Momentum Spread 30 (HMS-30 is the proper name; legacy code still uses E13) runs on one Azure Container Instance. The strategy:

- Download 1h klines for all 18 symbols (30 pairs, some symbols shared across pairs).
- Every 1h bar, compute rolling 48-bar OLS beta for each pair.
- Z-score the log-spread.
- Enter when |z| > 2.0 (breakout direction); exit when z retraces 1.0 from peak.
- All entries/exits are taker market orders. SL attached within seconds of every fill (HyroTrader compliance).
- State persists to Azure File Share for crash recovery.

Cost: under $20/month. No GPUs, no model retraining, no Databricks dependency.

Initial backtest-to-live transition showed some demo-account quirks (Bybit occasionally resets demo positions), but after a fresh account + state reset:

- 3 pairs open at 4.0× sizing (bumped from 3.0× after 26 hours to increase per-trade P&L)
- 0 compliance violations
- All SLs attached and verified hourly

---

### Lessons from two months of iteration

This work was **not a failure**. It was a phenomenal playground for building a complete ML system in a constrained domain. Here's what I actually learned:

1. **How to build an end-to-end ML pipeline.** Data ingestion → feature engineering → model training → backtesting → live-parity testing → deployment orchestration. Every layer uncovered new requirements. The cleanup discovered three data bugs that would have shipped to production. The parity tests revealed edge cases in execution. This pipeline is now the template for the next idea.

2. **Feasibility math kills bad ideas fast.** Before training anything, I should compute: edge per trade − fees − adverse selection. If that's negative, no amount of learning-rate tuning changes it. The L2 work had a real signal (55% directional accuracy), but the problem's intrinsic edge was small and costs were tight relative to that edge: about 4 bps maker-maker in the cleanest path, higher whenever execution mixed in taker flow. So this wasn't "always impossible" on paper; it was usually too tight to be robust after slippage, adverse selection, and throughput constraints. This gate should front-load every project.

3. **Throughput is as constraining as edge.** Leg A showed the oracle could clear fees at gate=0.65 (+2.94 to +6.44 bps per trade), but the number of qualifying setups collapsed from 8.7/day to 1.3/day. A profitable signal that fires once a week doesn't move capital. This taught me to think about expected trades/day, not just edge conditional on entry.

4. **Walk-forward validation is non-negotiable.** C6 hit +2.32 bps, looked great, and then failed across independent folds (median +0.93 bps). Single-period wins are false leads. This forced me to build proper train/val/test splits and stay honest about out-of-sample results.

5. **Systematic rejection is the real skill.** Leg A, B, and C each took a day or two but killed entire strategy classes. Instead of 20 more model tweaks, I ran three orthogonal tests, they all returned the same conclusion, and I pivoted. This is better than 3 months of local optimization.

6. **Market microstructure is learnable.** I went in with textbook knowledge of bid/ask spreads and order flow. I exited understanding why maker-entry adverse selection can wipe out half-spread savings, why LOB features decay in predictiveness past 5s, why funding rates compress faster than spot perp basis on perps. That intuition now applies to the next venue and asset class.

---

### What's next

As of 2026-04-25:

- HMS-30 live on demo with 4.0× sizing
- Decision gate: 5–7 days of clean data validates risk bounds → buy HyroTrader challenge
- If challenge clears: funded account, then Tier 2 work (maker-first execution, LightGBM entry filter)
- If challenge fails: postmortem, but the negative results from this phase are already the highest-value output

Part 3, if it comes, will be: **did live validation match the backtest?** That's the only question that matters anymore.
