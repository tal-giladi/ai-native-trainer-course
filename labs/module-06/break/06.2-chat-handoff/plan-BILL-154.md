# Plan — BILL-154 Customer statement

Author: agent (plan-feature 0.9.0, after `/clear`) · Reviewed by: ______ on ______

## Goal

A new `CustomerStatementService.GetStatementAsync(customerId)` returns a customer's unpaid invoices, oldest due first, with a running balance.

## Evidence

- `tickets/BILL-154.md` — the acceptance criteria.
- `src/Contoso.Billing/Invoices/IInvoiceRepository.cs` — `ListByCustomerAsync` loads a customer's invoices.

## Approach

- New `Statements/CustomerStatementService.cs`, so the statement logic stays out of `InvoiceService`. (ev: `tickets/BILL-154.md`)
- Unpaid invoices are those with `Status != InvoiceStatus.Paid`. (ev: `src/Contoso.Billing/Invoices/IInvoiceRepository.cs`)
- Running balance accumulated in a `decimal`. (ev: `tickets/BILL-154.md`)

## Boundaries

### Touch

- `src/Contoso.Billing/Statements/**`
- `tests/Contoso.Billing.Tests/CustomerStatementServiceTests.cs`

### Do not touch

- `src/Contoso.Billing/Legacy/**` (BILL-97)

## Steps

1. Write the 3 tests from the ticket. verify: `dotnet build` fails on the missing class.
2. Add `CustomerStatementService`. verify: the tests pass.

## Checkpoints

- HUMAN: approve this plan before step 1.
- STOP rule: if the same step fails twice, stop and report.

## Validation

- `dotnet run --project tools/LoopGate -- all --repo . --rules gates/architecture.rules --min-tests 6 --plan plans/BILL-154.md --git main`

## Out of scope

PDF rendering of the statement.
