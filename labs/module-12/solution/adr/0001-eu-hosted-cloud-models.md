# ADR-0001: EU-hosted cloud models for coding agents

Status: Accepted
Date: 2026-09-02
Topic: model-hosting
Deciders: Head of Platform Engineering, CISO delegate
Consulted: Legal (data protection), Procurement, team leads of billing and payments
Answers: Q01, Q02, Q03, Q18

## Context

Fabrikam has 400 developers in eight teams working on .NET and SQL Server services. Legal requires that source code and prompts are processed and stored only in the EU (group data-protection policy, section 4.2). The hosting decision must also survive the next model generation: whichever model we pick today will be replaced within a year.

We measured candidate options on our own 50-task set with the Module 7 harness (`fabrikam/hosting-options.json`, `ArchCheck hosting`). Volume estimate: 120,000 agent attempts per month; a failed attempt costs about $15 of developer review time.

## Options considered

### Option A — Vendor's own SaaS API
Best feature coverage and the newest models first. As of 2026-09 its inference geography controls offer `us` and `global` only, so it fails the EU hard constraint.

### Option B — Hyperscaler A, EU geographic inference profile
Same model family, routed only across EU regions of the cloud we already use for production; billed through our existing cloud account; IAM, audit trail and private endpoints we already operate. Some API features lag the vendor's own API.

### Option C — Hyperscaler B, EU multi-region endpoint
Same model family, EU-only routing, about 10% price premium over that provider's global endpoint, and fewer API features than Option B. A second cloud account to operate.

### Option D — Self-hosted open-weights model on our EU GPUs
Full control of the data path. The mid-size model scored 46% (95% CI 33–60%) on our tasks, below the 60% floor; the larger one scored 62% but costs $10.55 per successful task against $6.11 for Option B.

## Decision

We will use Option B as the primary route and Option C as the fallback route, both behind the model gateway (ADR-0007). Option A was eliminated by the residency constraint before any weighing; Option D by pass rate and cost per successful task. Between B and C, B wins on cost per success ($6.11 vs $6.19), on features, and because we already run its identity, network and audit controls.

## Consequences

- Positive: code and prompts stay in the EU on both routes; one bill through an existing account.
- Negative: new model versions reach the EU profile later than the vendor API; we accept a lag of weeks.
- Negative: two providers to operate for the fallback; two sets of quotas to request.
- Risk: if a provider's EU profile drops a model we depend on, the fallback may serve a different model; the eval gate (ADR-0006) must pass before any switch becomes permanent.
- Follow-up: platform team pins model versions in managed settings; procurement confirms no-training terms in both contracts.

## Confirmation

`ArchCheck lint solution/architecture.json --rules RES` passes in CI on every change to the architecture file. Quarterly, re-run `ArchCheck hosting` with fresh pass rates and prices; if the vendor API adds an EU inference geography, reopen this ADR.
