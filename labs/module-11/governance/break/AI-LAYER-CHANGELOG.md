# AI layer changelog (Contoso Billing) — AS FOUND at the start of lesson 11.5

One entry per change to AGENTS.md, CLAUDE.md, `.claude/`, `.mcp.json`, agent workflows and their settings.
Fields: Class, Changed, Why, Incident, Regression task, Verified, Reviewed by. See governance.md.

## 2026-09-26 — v1.6.0

- **Class:** B (behavior rule)
- **Changed:** added "never edit a merged migration" to AGENTS.md again.
- **Why:** asked to "tidy" V004__due_not_null.sql in BILL-171, the agent edited the merged migration — the same mistake as BILL-166 three weeks ago. Caught in review.
- **Incident:** INC-2026-014
- **Regression task:** TBD
- **Verified:** looks right now
- **Reviewed by:** @contoso/billing-leads

## 2026-09-12 — v1.5.0

- **Class:** A (wording)
- **Changed:** tidied CLAUDE.md: removed duplicated and "obvious" lines, 212 -> 148 lines (including the v1.4.1 line, which looked like a duplicate of the AGENTS.md migration rule).
- **Why:** context budget (Module 4 audit).
- **Verified:** smoke gate 9 tasks x 3: GATE PASSED
- **Reviewed by:** @contoso/billing-leads

## 2026-09-03 — v1.4.1 (hotfix)

- **Class:** B (behavior rule)
- **Changed:** added "do not modify merged migrations" to CLAUDE.md.
- **Why:** asked to "tidy" V004__due_not_null.sql to match V003 in BILL-166, the agent edited the merged migration (incident INC-2026-009); the reviewer caught it.
- **Regression task:** none (urgent)
- **Verified:** tried it once, the agent now refuses.
- **Reviewed by:** pushed by @dev-on-call (admin bypass)

## 2026-08-28 — v1.4.0

- **Class:** B (behavior rule)
- **Changed:** topology map for src/ added to AGENTS.md.
- **Why:** research phase kept missing Collections/ (Module 5 notes).
- **Verified:** full suite 24x5 vs v1.3: +7.5 pts [-3.8, +18.8]; T06 5/5; GATE PASSED
- **Reviewed by:** @contoso/billing-leads
