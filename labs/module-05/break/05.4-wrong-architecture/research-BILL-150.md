# Research brief — BILL-150 Record that a payment reminder was sent

Researcher: built-in Explore sub-agent, prompt "find how to persist reminders", 4 minutes. Date: 2026-09-28.

## Question

How do we persist reminders?

## Findings

- `docs/ARCHITECTURE.md` says all data access goes through `SqlHelper.ExecuteDataSet`, but `SqlHelper` is `[Obsolete]`, so that document is out of date.
- `src/Contoso.Billing/Invoices/InvoiceRepository.cs` only reads (`GetAsync`, `ListByCustomerAsync`). There is no write pattern anywhere in the codebase.
- Because there is no established way to write, the modern .NET default is a reasonable choice: EF Core with a `DbContext`, which also gives us change tracking and an in-memory provider for tests.
- Migrations: `db/migrations/V###__*.sql`, with `U###` undo scripts from V003 on.

## Existing capabilities to reuse

- `InvoiceService.IsOverdue`.

## Open questions

None.
