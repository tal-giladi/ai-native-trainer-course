# Plan — BILL-153 Store the customer's purchase-order number on every invoice

Author: agent (plan-feature 1.1.0) · Reviewed by: ______ on ______ · Research: `research/BILL-153.md`

## Goal

`dbo.Invoice.PoNumber nvarchar(35) NOT NULL` exists, existing rows are backfilled, and `Invoice.PoNumber` is returned by `GetAsync` and `ListByCustomerAsync`.

## Evidence

- `db/migrations/V004__due_not_null.sql` — backfill-then-NOT-NULL pattern on an existing table.
- `db/migrations/U004__due_not_null.sql` — undo reverses schema, not data.
- `src/Contoso.Billing/Invoices/InvoiceRepository.cs` — both SELECTs list columns explicitly.
- `src/Contoso.Billing/Invoices/Invoice.cs` — positional record that Dapper fills through its constructor.
- `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs` — `Inv(...)` builds `Invoice` positionally in 6 tests.

## Approach

- V005 adds `PoNumber nvarchar(35) NULL`, backfills existing rows with the agreed value, then makes it NOT NULL. (ev: `db/migrations/V004__due_not_null.sql`)
- U005 drops the column; the backfilled data is not restored. (ev: `db/migrations/U004__due_not_null.sql`)
- `Invoice` gets a trailing parameter `string PoNumber = ""` so existing `new Invoice(...)` calls compile unchanged. (ev: `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs`)
- Both SELECTs add `PoNumber` at the end of the column list, matching the constructor order. (ev: `src/Contoso.Billing/Invoices/InvoiceRepository.cs`)
- Assumption A1: existing invoices are backfilled with `N'UNKNOWN'` (the ticket gives no value). (ev: `tickets/BILL-153.md`)
- Assumption A2: `nvarchar(35)`. (ev: `tickets/BILL-153.md`)

## Boundaries

### Touch

- `db/migrations/V005__invoice_po_number.sql`
- `db/migrations/U005__invoice_po_number.sql`
- `src/Contoso.Billing/Invoices/Invoice.cs`
- `src/Contoso.Billing/Invoices/InvoiceRepository.cs`
- `tests/Contoso.Billing.Tests/InvoicePoNumberTests.cs`

### Do not touch

- `db/migrations/V00[1-4]*` and `db/migrations/U00[1-4]*` (merged migrations are immutable)
- `src/Contoso.Billing/Legacy/**` (BILL-97 owns it; its aggregate query is unaffected)
- `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs` and `tests/Contoso.Billing.Tests/ConventionTests.cs`
- `*.csproj` (no new packages)

## Steps

1. Write `InvoicePoNumberTests.cs` (read with PO number; list includes it); they fail to compile. verify: `dotnet build` fails only on `PoNumber`.
2. Add the trailing `PoNumber` parameter to `Invoice`. verify: the 6 existing tests still pass.
3. Add `PoNumber` to both SELECT lists. verify: new tests pass; `LoopGate arch` passes.
4. Create V005/U005 with the `new-migration` skill; write the backfill with the value approved at the checkpoint. verify: `LoopGate arch` migration-pair passes; optional `SET NOEXEC ON` compile.

## Checkpoints

- HUMAN: approve the backfill value for existing invoices (A1, `N'UNKNOWN'` proposed) and the column type (A2) with Finance before step 4.
- HUMAN: review the diff against Boundaries before merge; `LoopGate scope` must be clean.
- STOP rule: if the same step fails twice, stop and report instead of trying a third fix.

## Validation

- `dotnet run --project tools/LoopGate -- all --repo . --rules gates/architecture.rules --min-tests 6 --plan plans/BILL-153.md --git main`
- Expected: 8 tests (6 old + 2 new), 0 skipped, arch clean (pair present), scope clean. NOEXEC does not prove the backfill runs; run V005 and U005 against a dev copy with rows.

## Out of scope

Printing the PO number on documents; searching by PO number (no index); the monthly revenue report (BILL-97).
