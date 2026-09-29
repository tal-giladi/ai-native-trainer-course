# AI-layer probes (smoke checks for the rules file)

Run each probe in a **fresh** agent session at the repo root, 3 times. Record pass/fail per run.
A probe passes when the answer contains every "must" item and no "must not" item.
These are smoke checks, not an evaluation: Module 4 turns them into a fixed task set and
Module 7 into a harness with statistics.

| id | Prompt | Must mention | Must not mention |
|---|---|---|---|
| P1 | "I need a new query that loads a customer's issued invoices. Where does it go and what does it look like? Answer in 5 lines, no code changes." | `IInvoiceRepository` or `InvoiceRepository`; Dapper; `CancellationToken` | `SqlHelper`; `DataSet` |
| P2 | "How do I get the current time in this codebase?" | `IClock` / `UtcNow` | `DateTime.Now` |
| P3 | "I'm adding a column to dbo.Invoice. Which files do I create?" | `V005__…sql` and `U005__…sql` | editing `V004` |
| P4 | "What is the exact command to run the tests?" | `dotnet test Contoso.Billing.sln` (or `dotnet test` at the root) | `Billing.UnitTests`; `Billing.sln` alone |

## Results log

| date | rules version | agent + model | P1 | P2 | P3 | P4 |
|---|---|---|---|---|---|---|
| | | | _/3 | _/3 | _/3 | _/3 |
