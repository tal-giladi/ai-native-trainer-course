# Experiments (illustrative)

> **Illustrative.** One row per completed experiment report. The student in this exercise ran the Module 13 comparison; the numbers are those of the worked example built from the illustrative Contoso data, not evidence about any real agent.

| ID | Date reported | Design | Primary result | Guardrails and secondary | Report |
|---|---|---|---|---|---|
| EXP-01 | 2026-07-27 | 96 tickets, 12 weeks, 4 developers, randomized within developer × size | cycle time −16% (95% CI −25% to −5%) | escaped defects +12.5 points (95% CI 0 to +25), guardrail failed; PR review time −49% (95% CI −55% to −42%); time to PR +35% (95% CI +15% to +57%) | [worked example](../../module-13/reports/worked-example-experiment-01.md) |

## What EXP-01 does not show

- Four developers, one repository, one ticket mix, one agent (two minor versions).
- Whether the defect increase is real: the interval runs from 0 to +25 points.
- Anything about S tickets alone: the S stratum's ratio (0.87) was not pre-registered or reported with its own interval.
