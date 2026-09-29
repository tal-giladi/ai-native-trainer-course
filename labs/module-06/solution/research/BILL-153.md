# Research brief — BILL-153 Store the customer's purchase-order number on every invoice

Researcher: `researcher` subagent (prime 1.1.0), read-only · ~41k tokens explored · brief ~950 tokens
Rules version: AGENTS.md (Module 4 layer) · Date: 2026-09-28
Schema change: yes

## Question

What must change for `dbo.Invoice` to carry a PO number that is never NULL, and what already reads that table?

## Existing capabilities to reuse

| Need in the ticket | Already exists | Evidence (path) |
|---|---|---|
| Invoice shape | `Invoice` positional record, 6 parameters | `src/Contoso.Billing/Invoices/Invoice.cs` |
| Read one / list by customer (criterion 2) | `InvoiceRepository.GetAsync`, `ListByCustomerAsync`, explicit column lists | `src/Contoso.Billing/Invoices/InvoiceRepository.cs` |
| Adding a NOT NULL column to a table with rows | V004: backfill, then `ALTER COLUMN ... NOT NULL` | `db/migrations/V004__due_not_null.sql` |
| Test doubles | `InMemoryInvoices`, `Inv(...)` helper builds `Invoice` positionally | `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs` |

## Architecture facts that constrain the design

| Fact | Evidence (strongest first) | Rung |
|---|---|---|
| Every migration from V003 on ships with a `U###` undo script; next free number is 005 | `db/migrations/V003__utc_offsets_and_status.sql`, `db/migrations/U004__due_not_null.sql` | code + files |
| Never edit a merged `V###` | `docs/history-excerpt.txt` | history |
| Data access is Dapper in repositories; SQL lists columns explicitly | `docs/adr/0007-dapper-repositories.md`, `src/Contoso.Billing/Invoices/InvoiceRepository.cs` | ADR + code |
| Dapper fills `Invoice` through its constructor; a new column needs a constructor parameter | `src/Contoso.Billing/Invoices/Invoice.cs` | code |
| An undo reverses schema, not data | `db/migrations/U004__due_not_null.sql` | code |

## Dependencies and blast radius

- SQL that reads `dbo.Invoice` (searched `FROM dbo.Invoice` in `src/`): `SELECT ... FROM dbo.Invoice WHERE InvoiceId = @invoiceId` and `... WHERE CustomerId = @customerId` in `InvoiceRepository.cs`; `SELECT CustomerId, SUM(Amount) ... FROM dbo.Invoice` in `Legacy/MonthlyRevenueReport.cs` (aggregate, unaffected; BILL-97 owns it).
- Callers of `new Invoice(...)`: `Inv(...)` in `InvoiceServiceTests.cs` (6 existing tests). A trailing parameter with a default keeps them compiling.
- Index `IX_Invoice_CustomerId_Status` INCLUDE list: not needed for the PO number unless it is searched (not in this ticket).

## Open questions (for the plan checkpoint)

1. What PO number do the existing invoices get? The ticket says "visible with a PO number" and "never NULL" but gives no value. Proposed assumption: `N'UNKNOWN'`, pending Finance.
2. Column type: 35 characters of what? Proposed: `nvarchar(35)`, since customers' PO formats are not specified.

## What I did not look at

No database; the migration was not run. `SET NOEXEC ON` is left to validation. No data on how many rows `dbo.Invoice` has in production.
