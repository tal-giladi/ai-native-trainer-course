# Plan — BILL-153 Store the customer's purchase-order number on every invoice

Author: agent (plan-feature 1.0.0) · Reviewed by: ______ on ______ · Research: `research/BILL-153.md`

## Goal

`dbo.Invoice` has a `PoNumber` column that is never NULL, and `Invoice.PoNumber` is returned by `GetAsync` and `ListByCustomerAsync`.

## Evidence

- `src/Contoso.Billing/Invoices/Invoice.cs` — the record to extend.
- `src/Contoso.Billing/Invoices/InvoiceRepository.cs` — both SELECTs.
- `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs` — `Inv(...)` helper.

## Approach

- Add `PoNumber nvarchar(35) NOT NULL` to `dbo.Invoice` in a new migration. (ev: `src/Contoso.Billing/Invoices/InvoiceRepository.cs`)
- Add `string PoNumber` to `Invoice` and to both SELECT lists. (ev: `src/Contoso.Billing/Invoices/Invoice.cs`)
- Update the `Inv(...)` helper to pass a PO number. (ev: `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs`)

## Boundaries

### Touch

- `db/migrations/V005__invoice_po_number.sql`
- `src/Contoso.Billing/Invoices/Invoice.cs`
- `src/Contoso.Billing/Invoices/InvoiceRepository.cs`
- `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs`
- `tests/Contoso.Billing.Tests/InvoicePoNumberTests.cs`

### Do not touch

- `src/Contoso.Billing/Legacy/**` (BILL-97 owns it)
- `tests/Contoso.Billing.Tests/ConventionTests.cs`

## Steps

1. Write `InvoicePoNumberTests.cs`. verify: `dotnet build` fails only on `PoNumber`.
2. Add `PoNumber` to `Invoice` and update `Inv(...)`. verify: all existing tests pass.
3. Add `PoNumber` to both SELECT lists. verify: new tests pass.
4. Write `V005__invoice_po_number.sql` with `ALTER TABLE dbo.Invoice ADD PoNumber nvarchar(35) NOT NULL`. verify: `dotnet build`.

## Checkpoints

- HUMAN: approve this plan before step 1.
- HUMAN: review the diff against Boundaries before merge.
- STOP rule: if the same step fails twice, stop and report.

## Validation

- `dotnet run --project tools/LoopGate -- all --repo . --rules gates/architecture.rules --min-tests 6 --plan plans/BILL-153.md --git main`

## Out of scope

Printing the PO number on documents.
