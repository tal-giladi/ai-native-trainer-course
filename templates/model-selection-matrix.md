# Model selection matrix

Introduced in [lesson 02.5](../lessons/module-02/lesson-05.md). Fill it from measurements (ModelBench `results.csv` or your own harness), not from leaderboards. Everything here rots: write the date on every number.

**Decision owner:** · **Date measured:** · **Prices copied on:** · **Runs per task (N):**

## 1. Tasks

| Task id | Real example (link) | Pass criterion, written before any run | Volume per month | Cost of one failure (review time × rate) |
|---|---|---|---|---|
| | | | | |

## 2. Hard constraints (eliminate first, do not weight)

| Constraint | Requirement | Model A | Model B | Model C |
|---|---|---|---|---|
| Data handling / retention terms | e.g. no training on inputs; approved by security | | | |
| Residency / hosting | e.g. EU region, private networking, or self-hosted | | | |
| Required API features | e.g. tool use, strict schemas, streaming, batch | | | |
| Context actually needed | e.g. 60k tokens of input per task | | | |
| Licence (open weights) | e.g. commercial use allowed | | | |

A model that fails any row is out, whatever it scores below.

## 3. Measured results (survivors only)

| Model (id, provider) | Task | Pass k/N | p50 latency (s) | p95 latency (s) | $/attempt | $/success (tokens only) | $/success incl. failure cost |
|---|---|---|---|---|---|---|---|
| | | | | | | | |

$/success incl. failure cost $= \dfrac{\text{\$/attempt}}{p} + \left(\dfrac{1}{p} - 1\right) \times \text{cost of one failure}$, assuming failures are detected and retried.

## 4. Weighted criteria (optional, survivors only)

| Criterion | Weight | Model A | Model B | Model C |
|---|---|---|---|---|
| Pass rate on our tasks | | | | |
| $/success incl. failure cost | | | | |
| Latency for interactive use | | | | |
| Behaviour on our language mix (e.g. Hebrew tickets) | | | | |
| Tool-use reliability (malformed calls per 100) | | | | |
| **Weighted total** | 100% | | | |

## 5. Decision

- **Chosen per task:**
- **Why (one sentence per task, citing rows above):**
- **What would change the decision:** (price change of X%, new model, pass rate below Y)
- **Re-measure on:** (date, at most one quarter out)
