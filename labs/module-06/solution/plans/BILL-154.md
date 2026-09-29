# Plan — BILL-154 Customer statement

Author: agent (plan-feature 1.1.0) · Reviewed by: ______ on ______ · Research: `research/BILL-154.md`

## Goal

`InvoiceService.StatementAsync(customerId, ct)` returns the customer's issued invoices, oldest due first, each with a cumulative `decimal` balance; "issued" is the same filter `OutstandingAsync` uses.

## Evidence

- `src/Contoso.Billing/Invoices/InvoiceService.cs` — the "issued" filter in `OutstandingAsync`.
- `src/Contoso.Billing/Invoices/IInvoiceRepository.cs` — `ListByCustomerAsync`.
- `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs` — the existing test that pins `OutstandingAsync`.

## Approach

- Extract the filter into one private `Issued(IEnumerable<Invoice>)` helper used by `OutstandingAsync` and `StatementAsync`, so "issued" has one definition. (ev: `src/Contoso.Billing/Invoices/InvoiceService.cs`)
- New record `StatementLine(long InvoiceId, DateTimeOffset DueUtc, decimal Amount, decimal RunningBalance)`. (ev: `src/Contoso.Billing/Invoices/Invoice.cs`)
- Order by `DueUtc`, then `InvoiceId` (Assumption A1 from the brief). (ev: `tickets/BILL-154.md`)

## Boundaries

### Touch

- `src/Contoso.Billing/Invoices/InvoiceService.cs`
- `src/Contoso.Billing/Invoices/StatementLine.cs`
- `tests/Contoso.Billing.Tests/InvoiceStatementTests.cs`

### Do not touch

- `src/Contoso.Billing/Legacy/**` (BILL-97 owns it)
- `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs` and `tests/Contoso.Billing.Tests/ConventionTests.cs`
- `db/**` (no schema change)

## Steps

1. Write `InvoiceStatementTests.cs` with the 3 cases from the ticket. verify: `dotnet build` fails only on `StatementAsync`.
2. Extract `Issued(...)` and use it in `OutstandingAsync`. verify: the 6 existing tests still pass.
3. Add `StatementLine` and `StatementAsync`. verify: 9 tests pass; `LoopGate arch` passes.

## Checkpoints

- HUMAN: approve this plan, especially the extraction in step 2 and Assumption A1, before step 1.
- HUMAN: review the diff against Boundaries before merge; `LoopGate scope` must be clean.
- STOP rule: if the same step fails twice, stop and report instead of trying a third fix.

## Validation

- `dotnet run --project tools/LoopGate -- all --repo . --rules gates/architecture.rules --min-tests 6 --plan plans/BILL-154.md --git main`
- Expected: 9 tests (6 old + 3 new), 0 skipped, arch clean, scope clean.

## Out of scope

Rendering the statement (PDF, email); days overdue on the statement (PRD requirement 5, not in this ticket).
