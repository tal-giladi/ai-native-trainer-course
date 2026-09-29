# Experiment report template (`experiment-01.md`)

The honest write-up of one controlled comparison. It reports what the pre-registration promised, in the order it promised, including what the result does **not** show. Introduced in [13.6](../lessons/module-13/lesson-06.md); design in the [experiment design template](experiment-design.md); metric definitions in the [metrics template](metrics.md).

## Summary (3 sentences, no adjectives)

On [n] tickets over [weeks] weeks, [treatment] changed [primary metric] by [estimate]% (95% CI [lo]% to [hi]%) compared with [control]. [Guardrail] was [value] vs [value] (difference [x] points, 95% CI [lo] to [hi]). This [supports / does not support] the decision rule stated on [pre-registration date].

## Design as run

| Field | Planned | Actual |
|---|---|---|
| Tickets per arm | | |
| Unit and blocking | | |
| Dates | | |
| Versions (agent, model, AI layer) | | |
| Deviations | — | link to the deviations table |

## Results

- Balance check (`ImpactStats balance`): […]
- Primary metric, stratified, intention to treat: ratio [..], 95% bootstrap CI [..], permutation p [..], Hedges g [..].
- Per-stratum table: [paste].
- Guardrails: [paste, with Wilson intervals per arm and the difference interval].
- Sensitivity analyses (per-protocol, without flagged outliers, by agent version): [one line each].

## What this does not show

- Generalization limits (team, repository, ticket types, tool versions): […]
- Threats not fully controlled and their likely direction: […]
- Metrics not measured (for example long-term maintainability, learning, developer experience): […]

## Claim you may make (copy exactly into slides and posts)

> "[One sentence with the estimate, the interval, the sample and the setting.]"

## Money (only if asked; as a range)

Hours saved per ticket = baseline median × (1 − ratio), computed at the interval's two ends; × tickets per month × loaded rate − added agent and tooling spend. Report the range, never the point alone.

## Data and code

- `data/experiment-01.csv` (anonymized), generator of any synthetic parts, exact commands run, tool version.
