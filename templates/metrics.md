# Metrics template (`metrics.md`)

A metric dictionary for one team: what each number means, where its clock starts and stops, where the data comes from, and what gaming or confounding would look like. Write it **before** you compare anything. Introduced in [13.1](../lessons/module-13/lesson-01.md); used by the [experiment design template](experiment-design.md) and the [experiment report template](experiment-report.md).

Rules: one definition per metric, with explicit start and stop events. Durations are skewed: report the median and p75/p90, analyze on the log scale. Every speed metric is paired with at least one quality guardrail. Never define a metric the treatment itself can move without the work changing (lines of code, PR count, suggestions accepted).

## Team and period

| Field | Value |
|---|---|
| Team / repository | |
| Period covered | |
| Tracker and states (e.g. Jira: To Do → In Progress → In Review → Done) | |
| Source control and CI | |
| Where agent usage is recorded (ticket label, PR label, gateway log) | |
| Owner of this file | |

## Metric dictionary

| Metric | Unit | Start event | Stop event | Source (query or export) | Summary | Guardrail paired with | How it can be gamed or confounded |
|---|---|---|---|---|---|---|---|
| Cycle time | hours | ticket → In Progress | PR merged | tracker history + `gh pr list` | median, p75, p90; geometric mean for analysis | escaped defects, rework | splitting tickets; starting the clock late; choosing easy tickets |
| Lead time for changes (DORA) | hours | commit | deployed to production | CI/CD logs | median | change fail rate | batching; deploy definition |
| PR review time | hours | PR opened (not draft) | PR merged | `gh pr list --json createdAt,mergedAt` | median | review defects found | opening PRs later; rubber-stamp reviews |
| Rework | commits or % tickets | PR opened | 21 days after merge | git log on the ticket's files | share of tickets with ≥1 fix commit | — | squash merges hide it |
| Escaped defects | count per ticket | merge | 30 days after deploy | bug tickets linked to the ticket | share of tickets with ≥1 | — | unlinked bugs; short observation window |
| Throughput | tickets / week | — | ticket Done | tracker | per week, with size mix | escaped defects | ticket splitting |
| Change fail rate (DORA) | % deployments | deployment | needs immediate intervention | incident / rollback log | ratio | — | incident definition |
| Failed deployment recovery time (DORA) | hours | failure detected | service restored | incident log | median | — | — |
| Deployment rework rate (DORA) | % deployments | — | unplanned deployment caused by an incident | deploy log | ratio | — | — |
| Human effort | hours | first work on ticket | done | timer or time log | median | — | recall bias if not timed |
| Cost per successful task | currency | — | — | loaded hourly rate × effort + agent spend (gateway / billing) ÷ tickets that passed the quality bar | per arm | quality bar definition | excluding failures from the denominator |

## Quality-adjusted throughput

Define the quality bar in one sentence (for example "merged, no rework commit within 21 days, no escaped defect within 30 days"):

- Quality bar: […]
- Quality-adjusted throughput = tickets meeting the bar ÷ weeks.
- Cost per successful task = (human hours × loaded rate + agent and tooling spend) ÷ tickets meeting the bar.

## Data checks before any comparison

- [ ] Every row has size (or story points) recorded **before** the work started.
- [ ] Every row records which arm it was in and whether the agent was actually used.
- [ ] Agent, model and AI-layer versions are recorded per row (or per period).
- [ ] Clock start and stop events are identical in both arms.
- [ ] Rows with missing timestamps are counted and reported, not silently dropped.
