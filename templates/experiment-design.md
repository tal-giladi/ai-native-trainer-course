# Experiment design template (pre-registration + threat register)

Write this **before** the first ticket is assigned, commit it, and do not edit the analysis section afterwards (add a dated "deviations" entry instead). Introduced in [13.3](../lessons/module-13/lesson-03.md) and [13.4](../lessons/module-13/lesson-04.md); the results go in the [experiment report template](experiment-report.md). Metric definitions come from your [metrics dictionary](metrics.md).

## 1. Question

- Decision this experiment informs (who decides what): […]
- Treatment (exactly what changes; agent, model, AI-layer version): […]
- Control (exactly what the comparison arm does; is any AI allowed?): […]
- Population the result should generalize to (team, repository, ticket types): […]

## 2. Design

| Field | Value |
|---|---|
| Unit of randomization | ticket / developer / team |
| Blocking (strata) | e.g. developer × size (size set in refinement, before assignment) |
| Assignment method | `ImpactStats assign --block developer,size --seed <N>`; seed recorded here: |
| Arms and allocation | 1:1 |
| Duration | start date – end date (3–6 weeks) |
| Tickets per arm (target) | from the power calculation below |
| Versions pinned | agent […], model […], AI layer commit […] |
| Who can see the assignment, when | e.g. developer sees it when the ticket moves to In Progress |

## 3. Metrics (fixed now)

- **Primary** (one): […] — estimand: ratio of geometric means, AI / control, stratified by […].
- **Guardrails** (must not get worse beyond a stated margin): escaped defects […], rework […], review time […].
- **Secondary** (reported, not used for the decision): […].

## 4. Analysis plan (fixed now)

- Estimate and 95% interval: `ImpactStats compare <data.csv> --metric <primary> --strata <blocks> --boot 10000 --seed 13`.
- Analyze by **assigned** arm (intention to treat). Per-protocol (`--arm used_ai`) only as a secondary check.
- Outliers: kept in the primary analysis (the log scale limits their weight); a sensitivity analysis without tickets blocked by external factors, listed with reasons.
- Decision rule: adopt if […] (for example: the whole interval is below −5% and no guardrail's interval is above +10 points); otherwise […].
- Minimum effect worth acting on (practical significance): […]%.

## 5. Sample size

- SD of log(primary metric) within strata, from baseline data: […] (`ImpactStats compare` on baseline tickets, or 0.35–0.6 if unknown).
- `ImpactStats power --sd <sd>` → tickets per arm for the minimum effect: […].
- If that is not feasible in the time available: what you will be able to claim instead (for example, "rule out effects larger than X%").

## 6. Threat register

| Threat | How it could bias the result (direction) | Mitigation in the design | Check in the analysis | Residual risk |
|---|---|---|---|---|
| Selection (who/what gets the treatment) | | randomize within blocks | `balance` | |
| Task difficulty | | block on size set before assignment | per-stratum table | |
| Developer differences | | block on developer | per-developer check | |
| Learning effects | | alternate arms over time | early vs late halves | |
| Hawthorne / novelty | | run ≥3 weeks; same observation in both arms | first vs last week | |
| Regression to the mean | | do not select teams/periods for being extreme | baseline from ≥2 prior periods | |
| Model / agent version change | | pin; record per row | `--where agent_version=…` | |
| Contamination (control uses AI) | | explicit rule; record `used_ai` | ITT vs per-protocol | |
| Measurement (clock definitions, missing data) | | same clock in both arms | count missing rows | |

## 7. Deviations (dated, appended during the run)

| Date | What changed | Why | Effect on analysis |
|---|---|---|---|
