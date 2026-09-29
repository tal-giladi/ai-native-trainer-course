# AI layer changelog (Contoso Billing) — reference: the same five weeks with the policy followed

Compare with `break/AI-LAYER-CHANGELOG.md` (as found). Here the v1.4.1 incident produced T25, so the v1.5.0
tidy-up failed its gate instead of silently deleting the rule, and the BILL-171 recurrence never happened.

One entry per change to AGENTS.md, CLAUDE.md, `.claude/`, `.mcp.json`, agent workflows and their settings.
Fields: Class, Changed, Why, Incident, Regression task, Verified, Reviewed by. See governance.md.
Checked in CI: `AgentOps evolution AI-LAYER-CHANGELOG.md --tasks <tasks-v1.json> --tasks tasks-m11.json`.

## 2026-09-18 — v1.5.1

- **Class:** C (agent workflow and settings)
- **Changed:** agent-review.yml uses review-v2.md, posts only severity >= medium, at most 3 per PR; agent-settings.json denies `gh` and `git push`.
- **Why:** CI review precision 9/64 = 14% on 20 historical PRs; developers had stopped reading it.
- **Verified:** 20 PRs: precision 11/14 = 79%, recall 11/14 = 79%; holdout 4/6; see reports/ci-review-2026-09.md.
- **Reviewed by:** @contoso/billing-leads, @contoso/security

## 2026-09-12 — v1.5.0

- **Class:** A (wording)
- **Changed:** tidied CLAUDE.md, 212 -> 151 lines. The first attempt also removed the merged-migration tidy-up line as a "duplicate"; the smoke gate failed on T25 (1/3), so the line was moved into AGENTS.md next to the migration rule instead of deleted.
- **Why:** context budget (Module 4 audit).
- **Verified:** smoke 9 tasks x 3 + T25 x 3: T25 3/3, T06 3/3; GATE PASSED.
- **Reviewed by:** @contoso/billing-leads

## 2026-09-10 — v1.4.2

- **Class:** D (agent version)
- **Changed:** CLAUDE_CODE_VERSION bumped (pinned in the repository variable).
- **Why:** scheduled quarterly upgrade.
- **Verified:** full suite 24 x 5 A/B, paired diff +1.7 pts [-3.3, +6.7]; baseline refreshed in the same PR.
- **Reviewed by:** @contoso/billing-leads

## 2026-09-03 — v1.4.1

- **Class:** B (behavior rule)
- **Changed:** "a merged migration is never edited, including tidy-ups and formatting; propose a new V###/U### pair" added to CLAUDE.md as an emergency change.
- **Why:** incident — asked to "tidy" V004__due_not_null.sql to match V003 in BILL-166, the agent edited the merged migration; the reviewer caught it. T06 (bug-fix wording) had passed all along.
- **Incident:** INC-2026-009
- **Regression task:** T25 (new, golden, from the BILL-166 wording) failed 2/5 on v1.4.0 before the change.
- **Verified:** T25 5/5 after; T06 5/5; GATE PASSED. Emergency path: merged by on-call, owner review within 24 h.
- **Reviewed by:** @contoso/billing-leads (retrospective, 2026-09-04)

## 2026-08-28 — v1.4.0

- **Class:** B (behavior rule)
- **Changed:** topology map for src/ added to AGENTS.md.
- **Why:** research phase kept missing Collections/ (Module 5 notes).
- **Verified:** full suite 24x5 vs v1.3: +7.5 pts [-3.8, +18.8]; T06 5/5; GATE PASSED
- **Reviewed by:** @contoso/billing-leads
