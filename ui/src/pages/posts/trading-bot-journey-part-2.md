---
title: "Trading Bot Journey, Part 2: What Survived Contact With Reality"
summary: "Part 2 of the journey: what happened after rebuilding the data pipeline, which hypotheses failed, why I stopped forcing L2 direction models, and how HMS-30 emerged from documented experiment results."
date: 2026-04-25
tags:
  - trading
  - quant
  - systems
  - experimentation
draft: false
pinned: true
---

## Trading Bot Journey, Part 2: What Survived Contact With Reality

In [Part 1](/posts/trading-bot-journey), I ended on an open question: now that the data pipeline was fixed, which path was actually worth pursuing?

This post is the answer.

The short version: I stopped trying to rescue a strategy class that kept failing its own tests, and I moved to one that cleared the cost constraints with room to spare.

---

### What changed after Part 1

The biggest change was not a model change. It was process.

I rebuilt the workflow around a simple rule: if a hypothesis can't survive a direct, documented falsification test, it doesn't get another cycle.

That mindset produced three phases:

1. **Leg A**: test whether the existing direction-model lineage could be saved.
2. **Leg B**: test the two fastest "maybe it's just the objective/features" alternatives.
3. **Leg C**: test whether the underlying L2 directional substrate was still worth further investment.

Then came the pivot.

---

### Leg A: the old direction lineage failed the audit

The key result from the direction-head audit was straightforward:

- Direction AUC across 6 walk-forward folds: **0.552 +/- 0.01**.
- Even with oracle direction in the deployed gate setting, expected edge was still **-0.82 bps/trade**.

Those two numbers came from the same decision chain that had been running in practice, not from a new toy setup.

Interpretation: this wasn't a calibration tweak away from working. The core edge was too small relative to costs.

So Leg A ended with a kill decision on that lineage.

---

### Leg B: fast alternatives also failed

I ran the cheapest high-value checks next.

A signed-edge variant that was supposed to better align direction and payoff still landed in a weak range:

- Direction AUC across tested folds: **0.518-0.532**.

That was below the pass bar I needed to justify continuing that class of approach. So this leg was also closed.

---

### Leg C: broad substrate checks looked promising, then broke in walk-forward

This was the "one more honest try" phase.

At one point, an expanded feature run (C6) showed a best test cell around **+2.32 bps** at a long horizon/deep quantile. On its own, that looked encouraging.

But stability testing (C7) was the deciding test, and it failed:

- At the same long-horizon/deep-quantile slice, walk-forward median was **+0.93 bps**.
- Two folds were negative (**min -0.21 bps**).

That is not robust enough for the objective. A period-specific win is not the same as a durable edge.

So Leg C ended with the same conclusion: stop spending cycles on this directional L2 line.

---

### The pivot: stop asking L2 to predict direction

After that sequence, the path was clear.

Instead of forcing directional prediction on BTC microstructure, I shifted to a spread-based statistical approach on perp pairs.

The strategy that survived that process is now called **HMS-30**.

What mattered was not novelty. What mattered was whether the strategy could satisfy risk constraints and still show meaningful expected return under documented assumptions.

---

### What the HMS-30 backtest actually showed

For the 30-pair portfolio simulation over 161 days, the documented headline figures were:

- Mean daily P&L: **+$11,544**
- Worst single trade: **-1.68%**
- Worst daily drawdown: **-0.66%**
- Days with drawdown breach: **0**

Those are simulation numbers, not guarantees.

The reason they mattered to me: they cleared the practical constraints with margin, which is exactly what was missing in the prior direction-model work.

---

### Where it stands right now

As of 2026-04-25, the live demo status in the plan was:

- **9** closed trades
- **78%** win rate
- **+$180** total

That sample is still small. I am not treating it as proof.

The current decision gate is to collect more live days and only commit to the challenge purchase if the live behavior stays within the defined risk/compliance bounds.

That is deliberately boring, and that's the point.

---

### What Part 1 got right (and what Part 2 added)

Part 1 was right that data correctness and training/live parity were non-negotiable.

Part 2 added a second requirement: **strategy-class discipline**.

If repeated, documented tests say a class of edge does not clear costs, the correct move is to stop defending it and move on.

The real progress here was not "finding a better model." It was getting faster and more honest about killing ideas that didn't survive evidence.

---

### What I learned from this phase

1. Feasibility math is a gate, not a post-hoc explanation.
2. A promising single-period result is not enough; walk-forward stability decides.
3. Negative results can be the highest-ROI work if they stop months of drift.
4. Simplicity matters less than margin: a simpler strategy with a larger cushion beats a complex one near break-even.

If you read Part 1 as the cleanup arc, this is the selection arc.

Part 3, if it comes, should be about one thing only: whether live performance validates the thesis over enough time to matter.
