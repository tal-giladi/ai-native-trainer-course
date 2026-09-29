# Experiment 01 — coding agent vs no agent on Contoso Billing (worked example)

> **Worked example for lesson 13.6**, filled in from the [experiment report template](../../../templates/experiment-report.md). The data is the **illustrative** `data/contoso-randomized.csv`; every number below was produced by the commands at the end. It shows the shape and tone of an honest report. It is not evidence about any real agent.

## Summary

On 96 tickets over 12 weeks (4 developers, arms randomized within developer × size), using the coding agent changed cycle time by **−16% (95% CI −25% to −5%)** compared with working without it. Tickets with an escaped defect were 10 of 48 with the agent vs 4 of 48 without (difference +12.5 points, 95% CI 0 to +25 points). This meets the pre-registered speed criterion (whole interval below 0%) but **fails the quality guardrail** (interval upper bound above +10 points), so the pre-registered decision is: do not roll out yet; run a follow-up focused on defects.

## Design as run

| Field | Planned | Actual |
|---|---|---|
| Tickets per arm | 48 | 48 |
| Unit and blocking | ticket, blocked by developer × size | as planned; balance check: 20/20 S, 20/20 M, 8/8 L |
| Dates | weeks 1–12 | weeks 1–12 |
| Versions | agent 1.8 pinned | agent updated to 1.9 in week 7 (deviation 1) |
| Contamination | none allowed | 3 manual-assigned M tickets used the agent for tests (deviation 2) |
| Outliers | keep; sensitivity analysis | BILL-503 (manual, L): 170 h, blocked 5 days on DBA sign-off |

## Results

Primary metric, cycle time (In Progress → merged), stratified by size, intention to treat:

| Size | n manual | n agent | median manual | median agent | ratio of geometric means |
|---|---|---|---|---|---|
| S | 20 | 20 | 5.0 h | 4.9 h | 0.87 |
| M | 20 | 20 | 16.2 h | 15.6 h | 0.90 |
| L | 8 | 8 | 47.0 h | 39.8 h | 0.66 |
| **Stratified** | 48 | 48 | | | **0.844 [0.748, 0.948]**, permutation p = 0.010, Hedges g = −0.54 |

Sensitivity analyses (each one line, none changes the conclusion):

- Without BILL-503: −13% [−22%, −3%].
- Per protocol (by `used_ai` instead of assignment): −13% [−23%, −2%].
- Agent 1.8 weeks only: −17% [−31%, −2%]; agent 1.9 weeks only: −15% [−27%, −1%].
- Not stratified: −16% [−40%, +19%] — same estimate, interval 2.5× wider; blocking is what made the experiment informative.

Guardrails:

| Guardrail | Manual | Agent | Difference (stratified) |
|---|---|---|---|
| Escaped defects (tickets with ≥1) | 4/48 = 8% [3%, 20%] | 10/48 = 21% [12%, 34%] | +12.5 pt [0, +25], p = 0.13 |
| Rework (tickets with ≥1 fix commit) | 21/48 = 44% | 21/48 = 44% | 0 pt [−19, +19] |

Secondary: PR review time (PR opened → merged) −49% [−55%, −42%] and time to PR +35% [+15%, +57%] — with the agent, developers open PRs later. PR review time alone is therefore **not** a valid speed metric for this comparison.

## What this does not show

- Four developers on one repository and one ticket mix; the 95% interval says nothing about other teams, legacy modules outside Contoso Billing, or other agents.
- Developers knew their arm (no blinding is possible); some effort may have shifted to manual tickets or away from them.
- 48 tickets per arm can detect a 15–20% effect on cycle time, but cannot rule out a defect increase of up to 25 points or confirm there is none.
- Long-term effects (maintainability, onboarding, skill development) were not measured.

## Claim you may make

> "In a 12-week randomized comparison on 96 Contoso Billing tickets, the coding agent reduced cycle time by about 16% (95% CI 5% to 25%); escaped defects may have increased (8% vs 21% of tickets, interval 0 to +25 points), so we are investigating quality before rolling out."

## Money (range, only if asked)

Baseline median 13.8 h per ticket. Hours saved per ticket at the interval ends: 13.8 × 0.05 = 0.7 h to 13.8 × 0.25 = 3.5 h. For a team closing 40 tickets a month at $95/h: $2,600 to $13,100 per month, before agent spend and before the cost of any extra escaped defects.

## Commands

```bash
H="dotnet run --project tools/ImpactStats --"
$H balance data/contoso-randomized.csv --arm assigned --by size
$H compare data/contoso-randomized.csv --metric cycle_hours --strata size --mean
$H compare data/contoso-randomized.csv --metric cycle_hours --strata size --exclude ticket=BILL-503
$H compare data/contoso-randomized.csv --metric cycle_hours --strata size --arm used_ai --a no --b yes
$H compare data/contoso-randomized.csv --metric cycle_hours --strata size --where "agent_version=agent 1.8"
$H compare data/contoso-randomized.csv --metric cycle_hours --strata size --where "agent_version=agent 1.9"
$H compare data/contoso-randomized.csv --metric cycle_hours
$H compare data/contoso-randomized.csv --metric escaped_defects --binary --strata size
$H compare data/contoso-randomized.csv --metric rework_commits --binary --strata size
$H compare data/contoso-randomized.csv --metric review_hours --strata size
$H compare data/contoso-randomized.csv --metric hours_to_pr --strata size
```
