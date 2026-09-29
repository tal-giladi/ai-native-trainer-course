# Automation card — template

One card per recurring agent job (triage, changelog, dependency fixes, test generation, docs, CI review). Keep it next to the workflow (`.github/automation/<job>.md`). Lessons: [11.3 · Recurring automation](../lessons/module-11/lesson-03.md) (sections 1–4) and [11.4 · Operating agents](../lessons/module-11/lesson-04.md) (sections 5–7).

## 1. What and why

| Field | Value |
|---|---|
| Job name / workflow file | |
| Owner (a person or team, not "the bot") | |
| Trigger | event / schedule (cron, off the hour) / manual |
| Chore it replaces, in one sentence | |

## 2. Is it worth it?

| $n$ proposals / month | $a$ acceptance | $s$ min saved per accepted | $r$ min review per proposal | Net $n(as - r)$ | Break-even $a^* = r/s$ |
|---|---|---|---|---|---|
| estimate: | | | | | |
| measured (month 1): | | | | | |

## 3. Contract

| Stage | Detail |
|---|---|
| Inputs and their trust level | e.g. issue text: untrusted (outside users) |
| Propose: agent tools | e.g. `--tools ""` (none) / `Read,Grep,Glob` / `Edit(./CHANGELOG.md)` |
| Propose: output schema | file |
| Validate: policy and checks | e.g. `AgentOps triage --policy`, scope, tests, mutation check |
| Apply: step and token | e.g. separate job, `issues: write` only |
| Rejections go to | e.g. `needs-human` label, owner notified |
| Idempotency key | e.g. issue id + `triaged` label / ISO week branch |

## 4. What it must never do

- e.g. close issues, apply reserved labels, push to `main`, post links or mentions.

## 5. Limits and ceilings (11.4)

| Limit | Value |
|---|---|
| `--max-turns` | |
| `--max-budget-usd` per run | |
| `timeout-minutes` | |
| Max runs per day | |
| Daily cost ceiling (alert / auto-disable) | |
| Retry policy | e.g. retry once on `error_during_execution`; never on caps or validation rejects |

## 6. Operating

| Item | Value |
|---|---|
| Approval | e.g. draft PR + CODEOWNERS / environment reviewer |
| Kill switch | `vars.AGENTS_ENABLED` (all agents) and `vars.<JOB>_ENABLED` (this one) |
| Rollback | e.g. revert the layer tag / pin previous `CLAUDE_CODE_VERSION` / close drafts |
| Ledger / telemetry | where the per-run line goes; OTel on or off |
| Alert on | failure, ceiling, and **absence** (no run for __ hours) |

## 7. Log

| Date | Runs | Accepted | Edited | Rejected | Cost | Notes |
|---|---|---|---|---|---|---|
| | | | | | | |
