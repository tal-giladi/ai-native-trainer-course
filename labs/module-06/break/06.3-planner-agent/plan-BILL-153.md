# Plan — BILL-153 Store the customer's purchase-order number on every invoice

Author: `planner` subagent · Reviewed by: ______ on ______ · Research: `research/BILL-153.md`

## Goal

`dbo.Invoice.PoNumber nvarchar(35) NOT NULL` exists, existing rows are backfilled, and `Invoice.PoNumber` is returned by `GetAsync` and `ListByCustomerAsync`.

## Evidence

- `db/migrations/V004__due_not_null.sql` — backfill-then-NOT-NULL pattern on an existing table.
- `db/migrations/U004__due_not_null.sql` — undo reverses schema, not data.
- `src/Contoso.Billing/Invoices/InvoiceRepository.cs` — both SELECTs list columns explicitly.
- `src/Contoso.Billing/Invoices/Invoice.cs` — positional record that Dapper fills through its constructor.

## Approach

- V005 adds `PoNumber nvarchar(35) NULL`, backfills existing rows with `N'N/A'`, then makes it NOT NULL. (ev: `db/migrations/V004__due_not_null.sql`)
- U005 drops the column. (ev: `db/migrations/U004__due_not_null.sql`)
- `Invoice` gets a trailing parameter `string PoNumber = ""`. (ev: `src/Contoso.Billing/Invoices/Invoice.cs`)
- Both SELECTs add `PoNumber` at the end of the column list. (ev: `src/Contoso.Billing/Invoices/InvoiceRepository.cs`)
- Resolved: existing invoices get `N/A`, the usual placeholder; the column type is `nvarchar(35)`. (ev: `tickets/BILL-153.md`)

## Boundaries

### Touch

- `db/migrations/V005__invoice_po_number.sql`
- `db/migrations/U005__invoice_po_number.sql`
- `src/Contoso.Billing/Invoices/Invoice.cs`
- `src/Contoso.Billing/Invoices/InvoiceRepository.cs`
- `tests/Contoso.Billing.Tests/InvoicePoNumberTests.cs`

### Do not touch

- `db/migrations/V00[1-4]*` and `db/migrations/U00[1-4]*` (merged migrations are immutable)
- `src/Contoso.Billing/Legacy/**` (BILL-97 owns it)
- `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs`

## Steps

1. Write `InvoicePoNumberTests.cs`. verify: `dotnet build` fails only on `PoNumber`.
2. Add the trailing `PoNumber` parameter to `Invoice`. verify: the 6 existing tests pass.
3. Add `PoNumber` to both SELECT lists. verify: new tests pass.
4. Create V005/U005 with the `new-migration` skill. verify: `LoopGate arch` passes.

## Checkpoints

- HUMAN: approve this plan before step 1.
- HUMAN: review the diff against Boundaries before merge.
- STOP rule: if the same step fails twice, stop and report.

## Validation

- `dotnet run --project tools/LoopGate -- all --repo . --rules gates/architecture.rules --min-tests 6 --plan plans/BILL-153.md --git main`

## Out of scope

Printing the PO number on documents.
