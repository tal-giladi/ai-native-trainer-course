# NOTES.md — Contoso Billing (illustrative)

> **Illustrative.** A filled-in version of the [notes log template](../../../templates/notes-log.md), written as if by a student who ran Modules 5–11 on Contoso Billing between January and April 2026. The incidents are the ones the course's labs stage (05.1–11.5) plus a few of the same kind; the dates, minutes and ticket numbers are invented for the exercise. Use it to practise mining; mine **your own** `NOTES.md` for your method.

## Ticket log (excerpt)

| Date | Ticket | Size | Mode | Agent + rules version | Total min | Human review min | Defects in review | Defects later | Rework commits | Notes |
|---|---|---|---|---|---|---|---|---|---|---|
| 2026-01-12 | BILL-151 | M | paste-and-go | agent 1.6 · layer v0.3 | 48 | 35 | 2 | 1 | 2 | own overdue check (INC-01); tests green over off-by-one (INC-02) |
| 2026-01-13 | BILL-151 | M | loop | agent 1.6 · layer v0.3 | 96 | 14 | 0 | 0 | 0 | brief found `InvoiceService.IsOverdue` in 6 min |
| 2026-01-21 | BILL-150 | M | loop | agent 1.6 · layer v0.3 | 130 | 40 | 1 | 0 | 1 | plan had no out-of-scope; finance query changed (INC-03) |
| 2026-01-28 | BILL-150 | M | loop | agent 1.6 · layer v0.4 | 115 | 45 | 1 | 0 | 3 | EF Core repository against ADR 0007 (INC-04) |
| 2026-02-04 | BILL-154 | M | loop | agent 1.7 · layer v0.5 | 88 | 20 | 1 | 0 | 1 | plan redefined "issued" after `/clear` (INC-05) |
| 2026-02-11 | BILL-153 | L | loop | agent 1.7 · layer v0.5 | 210 | 30 | 1 | 0 | 0 | planner decided the PO-number question itself (INC-06) |
| 2026-02-16 | BILL-157 | S | loop | agent 1.7 · layer v0.5 | 35 | 8 | 0 | 0 | 0 | same ticket paste-and-go took 110 min (two resets) |
| 2026-02-18 | BILL-156 | S | paste-and-go | agent 1.7 · layer v0.5 | 22 | 12 | 1 | 0 | 1 | agent changed the expected value in a failing test (INC-07) |
| 2026-02-25 | BILL-158 | M | paste-and-go | agent 1.7 · layer v0.6 | 55 | 30 | 1 | 1 | 2 | C# balance calculation duplicating `usp_GetCustomerBalance` (INC-08) |
| 2026-03-04 | BILL-159 | S | loop | agent 1.7 · layer v0.7 | 40 | 15 | 0 | 1 | 1 | merged migration V004 edited after rules tidy-up (INC-09) |
| 2026-03-11 | BILL-161 | M | loop | agent 1.8 · layer v0.8 | 105 | 25 | 1 | 0 | 1 | "all tests pass", new test project not in the solution (INC-10) |
| 2026-03-18 | BILL-163 | M | loop | agent 1.8 · layer v0.8 | 120 | 20 | 1 | 0 | 1 | MCP schema tool returned a stale column list (INC-11) |
| 2026-03-25 | BILL-165 | S | paste-and-go | agent 1.8 · layer v0.9 | 30 | 25 | 1 | 1 | 1 | VAT rounding re-implemented with banker's rounding (INC-12) |
| 2026-04-01 | BILL-168 | S | loop | agent 1.8 · layer v1.0 | 45 | 10 | 0 | 1 | 1 | hotfix rule without a task, deleted in tidy-up; incident repeated (INC-13) |
| 2026-04-08 | BILL-170 | S | loop | agent 1.8 · layer v1.0 | 52 | 6 | 0 | 0 | 0 | research took 25 of 52 min; paste-and-go run: 19 min, also no defects (INC-14) |

## Failure-diagnosis log

| ID | Date | Ticket | Defect (one line) | Origin | Escape and missing gate | Failure class (primary / escape) | Fix at origin | Gate added |
|---|---|---|---|---|---|---|---|---|
| INC-01 | 2026-01-12 | BILL-151 | New `IsPastDue()` in the reminder service; counts the due day as overdue, `InvoiceService.IsOverdue` does not | R (none done) | V: tests written by the agent, from its own definition | 5 insufficient context / 10 verification | research brief with a Reuse row | reuse check in the plan template |
| INC-02 | 2026-01-12 | BILL-151 | Off-by-one on due date; the agent's tests assert the same off-by-one | I | V: only agent-written tests ran | 2 incorrect reasoning / 10 verification | acceptance tests written from the ticket before implementation | `LoopGate tests` requires a human-reviewed test per criterion |
| INC-03 | 2026-01-21 | BILL-150 | Changed the finance ageing query "while there" | P | review only | 9 planning / 10 verification | out-of-scope list in every plan | `LoopGate scope` fails on files outside the plan |
| INC-04 | 2026-01-28 | BILL-150 | EF Core repository instead of Dapper (ADR 0007) | R | V: convention tests did not cover data access | 5 insufficient context / 10 verification | brief cites ADRs | `LoopGate arch` |
| INC-05 | 2026-02-04 | BILL-154 | Plan uses "issued" = created; the business means sent to the customer | R | P: brief lived in chat, lost at `/clear` | 4 stale context / 9 planning | brief written to `research/` and read from file | skill reads the brief from disk |
| INC-06 | 2026-02-11 | BILL-153 | Planner chose "PO number = empty string" for existing invoices; an open product question | P | human checkpoint skipped | 9 planning / 8 instruction | open questions block the plan | planner must stop on open questions |
| INC-07 | 2026-02-18 | BILL-156 | Failing test "fixed" by changing its expected value from 118.00 to 117.99 | I | V: test file edits not reviewed | 8 instruction / 10 verification | tests read-only during implement | hook blocks edits to existing test files |
| INC-08 | 2026-02-25 | BILL-158 | C# balance calculation ignoring credit notes; `usp_GetCustomerBalance` already exists | R (none done) | V: new unit tests only | 5 insufficient context / 10 verification | Reuse row covers SQL objects | reuse search includes `db/` |
| INC-09 | 2026-03-04 | BILL-159 | Merged migration V004 edited; the "never edit merged V###" rule was lost in a context tidy-up | — (layer) | no task for the rule | 5 insufficient context / 10 verification | rule restored | eval task T06 for merged migrations |
| INC-10 | 2026-03-11 | BILL-161 | "All 212 tests pass" but the new test project was not in the solution, so its tests never ran | V | V: count of tests not compared | 10 verification / 10 verification | gate checks test count rises | `LoopGate tests --min-new 1` |
| INC-11 | 2026-03-18 | BILL-163 | Plan used a dropped column; the schema tool served a cached schema | R | tool freshness | 7 tool failure / 10 verification | schema tool reads live catalog | freshness check in the tool |
| INC-12 | 2026-03-25 | BILL-165 | VAT rounding re-implemented with `MidpointRounding.ToEven`; `Money.RoundVat` uses `AwayFromZero` | R (none done) | V: agent-written tests | 5 insufficient context / 10 verification | Reuse row | analyzer bans `Math.Round` outside `Money` |
| INC-13 | 2026-04-01 | BILL-168 | Same merged-migration edit as INC-09: a hotfix rule added without a task was removed by a tidy-up PR | — (layer) | no regression task | 5 insufficient context / 10 verification | evolution loop: task first | governance check on the changelog |
| INC-14 | 2026-04-08 | BILL-170 | Not a defect: on a one-line S ticket the loop cost 52 min vs 19 min paste-and-go, both correct | — | — | — | skip research when the diff is one sentence and no business term is touched | — |

## Weekly summaries (excerpt)

- 2026-01-30: 4 loop, 1 paste-and-go. Most common origin: R. What this cannot show: 5 tickets, I chose which ones got the loop.
- 2026-02-27: 3 of 4 defects this month came from something that already existed and the agent could not see (INC-05, INC-08, and INC-01 last month). Two of four were "green" runs where the only tests were the agent's.
- 2026-04-10: the loop is not always worth it: BILL-170 (INC-14). Six S tickets this quarter, loop median 40 min vs paste-and-go median 26 min, no difference in defects that I can see with six tickets.
