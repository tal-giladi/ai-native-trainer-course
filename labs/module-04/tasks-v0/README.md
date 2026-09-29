# tasks-v0 — the first fixed task set (Contoso Billing)

Ten short, read-only questions about the Contoso Billing repository from `labs/module-03/brownfield`.
Each one depends on one critical fact (F1–F10). A deterministic grader checks each answer with
regular expressions, so the same answer always gets the same grade.

This is the seed of your `agent-evals/tasks-v0/`. Module 7 turns it into a proper evaluation
dataset with graders you have calibrated and confidence intervals; here it is a regression guard for
context changes: **a context change that lowers the pass rate is not an improvement, however many tokens it saves.**

| Task | Fact | What it checks | Inferable from code alone? |
|---|---|---|---|
| T01 | F1 | test command is `dotnet test Contoso.Billing.sln` | partly — old docs say `Billing.sln` |
| T02 | F2 | new query = repository method with `CancellationToken`, filtered in SQL; no `SqlHelper`/`DataSet` | partly — `ARCHITECTURE.md` says the opposite |
| T03 | F3 | "now" = `IClock.UtcNow` via an injected clock | yes, if the agent reads `InvoiceService` |
| T04 | F4 | new column = `V005` **and** `U005` scripts | no — half the existing scripts have no undo |
| T05 | F5 | money = `decimal(19,4)` | yes (`V001`) |
| T06 | F6 | a merged migration is fixed by a new `V005`, never by editing `V004` | no |
| T07 | F7 | overdue = `IsOverdue`: Issued and past due | yes |
| T08 | F8 | `IX_Invoice_CustomerId_Status` exists | yes, if `V003` is read |
| T09 | F9 | don't migrate `MonthlyRevenueReport` as a drive-by; BILL-97 owns it | yes (code comment + ADR), if read |
| T10 | F10 | a failing convention test means fix the code | partly |

## Files

- `tasks.json` — prompts and grading patterns (`must`: all must match; `mustNot`: none may match; case-insensitive).
- Run it with `../scripts/run-tasks.sh` (or `.ps1`), grade with `ContextLab grade`. See `../README.md`.

## Known grader limits

- Patterns check wording, not meaning. A correct answer phrased differently fails (a false negative);
  an answer that name-drops the right symbol in a wrong sentence can pass (a false positive).
  Read every failing answer before you believe a failure. Module 7 measures grader precision and recall.
- The prompts ask for terse answers ("paths only", "code only") to keep grading reliable. That is a
  deliberate trade-off: real tickets are longer.
- 10 tasks × 3 runs cannot detect small differences. A change from 27/30 to 28/30 is noise; from 18/30
  to 28/30 is a signal worth investigating. Module 7 gives you the intervals.
