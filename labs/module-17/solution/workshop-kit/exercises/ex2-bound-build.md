# Hands-on 2 — Bound plan and Build for BILL-180 (14 minutes)

> Reference (illustrative). Objective O3. Handout page 5.

- Start: your brief from Hands-on 1, or `ws-1-ground`.
- Task: run `/plan-feature BILL-180`. Before you reply `approve`, edit `plans/BILL-180.md` so that it has a do-not-touch list and one check per acceptance criterion. Then let the agent build, one plan step at a time.
- Done when: the plan has (a) a do-not-touch list naming files, the public contract and the SQL object, (b) a check for each of the four acceptance criteria, at least one the agent cannot edit; and `dotnet test` is green or you have stopped at 1:19.
- Time: 14 minutes; one-minute warning at 13. Odd tables start the agent now, even tables one minute later (T-05).
- Hints:
  1. What must *not* change for Collections, Finance and BILL-155? Start the do-not-touch list there.
  2. AC 3 says "the rule lives in one place". Which check proves that, and can the agent's own tests prove it?
  3. The convention tests and `gates/architecture.rules` already exist; the agent cannot edit them under the hooks.
- Extension: add a check that fails if a second void rule appears later (a convention test that searches for `InvoiceStatus.Void` comparisons outside `InvoiceService`).
- Catch-up: `git checkout ws-2-bound` (plan approved, build done); read the plan's do-not-touch list before Hands-on 3.
- Paper path: handout page 6 has the agent's unedited plan; write the do-not-touch list and the checks in the margin.

## Reference do-not-touch list and checks (instructor only)

- Do not touch: `Invoice.cs` (`InvoiceStatus` values are stored as tinyint), `IInvoiceRepository` signatures, `db/migrations/*` (no schema change), `tickets/BILL-155.md` scope (no void action in this ticket), `Legacy/`.
- AC 1 → a test calls `InvoiceService.CanVoid` (agent-written, reviewed by you).
- AC 2 → four status cases; the draft case marked pending Finance's answer.
- AC 3 → convention test: no `InvoiceStatus.Void` comparison outside `InvoiceService` (not editable by the agent).
- AC 4 → test count rises from 6 by at least 4; the existing 6 unchanged (`git diff --stat tests/` shows only additions).
