# CI review report — template

Use this to decide whether an agent reviewer may comment on pull requests, and with which policy. Lesson: [11.2 · CI review: signal vs noise](../lessons/module-11/lesson-02.md). One report per reviewer version; keep it next to the workflow (`reports/ci-review-YYYY-MM.md`) and link it from the workflow's header comment.

## 1. Reviewer under test

| Field | Value |
|---|---|
| Reviewer version / prompt file | e.g. `review-v2.md` @ commit |
| Agent and model (pinned) | |
| Tools allowed | e.g. `Read,Grep,Glob` only |
| Output schema | e.g. `review-schema.json` |
| Date scored | |

## 2. Data set

| Field | Value |
|---|---|
| PRs (N) and period | e.g. 20 PRs, 2026-06-01 to 2026-08-31 |
| Selection rule | e.g. every merged PR touching `src/` or `db/`, no drafts, no bots |
| Split (fixed before tuning) | dev: __ PRs · holdout: __ PRs |
| Known defects (D) | total __ = review __ + 30-day follow-up __ + adjudicated __ |
| Match rule | same PR and file, line within ±3, each defect once, extra matches = duplicates |

## 3. Results

| Configuration | Split | C | per PR | TP | Precision (95% Wilson) | Recall (95% Wilson) | Noise/PR | Ceiling D/C |
|---|---|---|---|---|---|---|---|---|
| baseline prompt, no filter | dev | | | | | | | |
| candidate prompt, no filter | dev | | | | | | | |
| candidate + policy | dev | | | | | | | |
| **candidate + policy** | **holdout (run once)** | | | | | | | |

Misses (false negatives) worth naming, and why the reviewer missed them:

-

## 4. Policy

| Setting | Value | Chosen on |
|---|---|---|
| Minimum severity | | dev |
| Categories never posted | e.g. style, naming, docs | dev |
| Max comments per PR | | dev |
| Evidence required | yes / no | |
| Paths in scope | e.g. `src/**`, `db/**` | |

Team bar (write it before scoring): precision ≥ ____ on holdout, recall ≥ ____, ≤ ____ comments per PR.

## 5. Decision

- [ ] Ship with the policy above · [ ] Ship on risky paths only · [ ] Do not ship
- Reason (two sentences, citing the holdout numbers and intervals):
- Production signal to track: acted-on rate (comment resolved with a code change) and thumbs-down rate, reviewed on (date):
- Owner:

## 6. Limitations

- Interval width and what sample size would halve it.
- Ground-truth gaps (defects nobody has found yet, adjudication coverage).
- Anything that changed during the period (agent version, rules, team).
