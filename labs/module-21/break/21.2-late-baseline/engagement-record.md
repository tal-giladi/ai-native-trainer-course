# Engagement record — C-01 (break 21.2: the late baseline)

> **Deliberately flawed.** The audit was done from the repository's docs, and the "baseline" was taken in weeks 3–4, when the pilot was already running. Run `EngageCheck baseline` on it, then compare with `solution/engagement-record.md`.

- Client code: C-01
- Consultant: Student (S)
- Build start: 2026-10-19

## 2. Audit and baseline

### Findings

| Finding | Evidence (rungs) | Confirmed | Severity | Owner |
|---|---|---|---|---|
| Data access goes through SqlHelper.ExecuteDataSet | docs/ARCHITECTURE.md (5) | yes | high | |
| The solution is Billing.sln, tests in Billing.UnitTests | docs/ARCHITECTURE.md (5) | yes | low | |
| Timestamps use DateTime.Now | docs/ARCHITECTURE.md (5), Yossi said so (7) | yes | medium | |
| Migrations have no rollback convention | V001 and V002 have no undo (2) | yes | medium | |

### Baseline

| Metric | Definition | Source | Window | n | Value | Spread | Captured |
|---|---|---|---|---|---|---|---|
| PR review time | Mean hours from PR opened to merged, per developer | GitHub | 2 weeks | 9 | 11.2 h | | 2026-10-30 |
| Cycle time | Hours per ticket | Jira | | 9 | 20 h | | 2026-10-30 |
