---
title: "Debugging in Production When the Spec Is Wrong"
summary: "Patterns I've developed for tracking down ambiguous failures in distributed systems — when the data is wrong, the requirements are vague, and the logs don't help yet."
date: 2026-03-20
tags:
  - debugging
  - backend
  - reliability
  - systems
draft: true
---

## Debugging in Production When the Spec Is Wrong

Most of my backend work has been on systems where the failure mode isn't a stack trace — it's data that looks fine but isn't, requirements that contradict themselves, or upstream services that change behavior without telling anyone. Over several years of this, I've landed on a set of patterns that work.

---

### Start With What You Can Observe

When something breaks and the logs don't explain it, the first instinct is to add logging and redeploy. That works, but it's slow. Before touching code:

1. **Reproduce the failure with a specific input.** Not "it's broken for some users" — find one record, one request, one transaction that fails. Everything starts from a concrete example.
2. **Trace the data path manually.** Follow that one input through every service and transformation. Where does the actual value diverge from the expected value? The system isn't a black box — it's a pipeline, and the bug is at a specific stage.
3. **Check what changed.** Not just your deploys — upstream data shapes, third-party API behavior, config changes, infrastructure updates. The most confusing bugs are usually caused by something that changed outside your codebase.

---

### The Spec Is Part of the System

I've spent a lot of time working with business logic that exists as tribal knowledge, partially documented rules, or requirements that made sense for the original case but don't cover the current one.

When the spec is wrong or incomplete, debugging becomes translating between two worlds:

- **What the business thinks should happen** (often described in domain language, with implicit assumptions)
- **What the code actually does** (which faithfully implements some version of the spec, possibly an old one)

The fix is usually not a code change — it's a conversation or better documentation. "Here's what the system does with this input. Is that right?" Half the time the answer is "no, it should do X instead" and you get a requirement that actually works. The other half, the system is correct and the expectation was wrong.

**I document these things.** Every edge case that gets surfaced and resolved becomes a test case or a section in the documentation. This is the real value — not fixing the bug, but capturing the business rule so the next person doesn't have to rediscover it.

---

### Make Problems Visible Before Users Find Them

The systems I've worked on handle records that pass through multiple stages of transformation and validation. A malformed record at ingestion can silently produce a wrong result three services downstream. By the time someone notices, the trail is cold.

What's worked:

- **Structured logging with correlation IDs.** Every record gets a trace ID at ingestion that follows it through the pipeline. When something fails, I can reconstruct the full history without guessing.
- **Data quality assertions at stage boundaries.** Not just "did this service return 200" — actual checks on the data: are required fields present? Are values within expected ranges? Does this record's state make sense given its history?
- **Anomaly detection on output distributions.** If the ratio of a particular classification suddenly shifts 20% from baseline, something changed upstream. This catches problems that no individual record-level check would find.

The goal is never zero bugs — it's fast detection and a clear path from symptom to root cause.

---

### Running Old and New Side by Side

The highest-risk changes I've shipped weren't new features — they were migrations. Replacing a data provider, rewriting a rules engine, changing how records flow through a pipeline.

The pattern that's saved me repeatedly:

1. **Run both paths in parallel.** The old system continues to produce real results. The new system processes the same inputs but its output goes to a comparison table, not production.
2. **Diff the outputs systematically.** Not just "do they match" — categorize the differences. Expected divergences (the new system handles a known edge case differently) vs. unexpected divergences (a bug or a missing rule).
3. **Ramp gradually.** Route 1% of traffic to the new path, then 5%, then 25%. At each step, verify the diff rate is acceptable.

This is slower than a cut-over. It's also how you avoid the "we migrated and broke 10,000 records" incident.

---

### What I've Learned

- **The most dangerous bugs are the ones where the system doesn't crash.** Silent data corruption is harder to detect, harder to diagnose, and harder to bound the impact of.
- **Defensive programming isn't overkill — it's having seen what upstream data actually looks like.** Null handling, boundary conditions, graceful degradation when the input doesn't match the schema.
- **Invest in the debugging workflow, not just the fix.** If diagnosing a problem takes 4 hours and fixing it takes 20 minutes, the leverage is in making diagnosis faster — better logging, better tooling, better documentation of system behavior.
- **The best technical solution usually comes from understanding the domain.** Spend time with the people closest to the problem. The right abstraction falls out of understanding what they actually need.
