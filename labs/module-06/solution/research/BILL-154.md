# Research brief — BILL-154 Customer statement

Researcher: `researcher` subagent (prime 1.1.0), read-only · ~26k tokens explored · brief ~750 tokens
Rules version: AGENTS.md (Module 4 layer) · Date: 2026-09-28
Schema change: no

## Question

Which existing definition of "issued" must the statement reuse, and where does a per-customer read already exist?

## Existing capabilities to reuse

| Need in the ticket | Already exists | Evidence (path) |
|---|---|---|
| "Issued" (criterion 2) | the filter in `InvoiceService.OutstandingAsync`: `Status == InvoiceStatus.Issued` | `src/Contoso.Billing/Invoices/InvoiceService.cs` |
| A customer's invoices | `IInvoiceRepository.ListByCustomerAsync` | `src/Contoso.Billing/Invoices/IInvoiceRepository.cs` |
| Money type | `decimal`, never `double` | `src/Contoso.Billing/Invoices/Invoice.cs` |
| Test doubles | `FixedClock`, `InMemoryInvoices` (private to the test class) | `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs` |

Nothing builds a statement today (searched `Statement`, `RunningBalance`, `Balance`: 0 hits in `src/`).

## Architecture facts that constrain the design

| Fact | Evidence (strongest first) | Rung |
|---|---|---|
| "Issued" is defined once, inside `OutstandingAsync`; a second filter would drift (see BILL-151) | `src/Contoso.Billing/Invoices/InvoiceService.cs` | code |
| New data access is a Dapper repository; this ticket needs none (existing list is enough) | `docs/adr/0007-dapper-repositories.md` | ADR |
| Money is `decimal` in C# | `src/Contoso.Billing/Invoices/Invoice.cs` | code |

## Dependencies and blast radius

- `OutstandingAsync` callers: tests only. Extracting its filter into a private helper changes no behavior; its test `Outstanding_counts_only_issued_invoices` guards it.
- No schema, no config.

## Open questions (for the plan checkpoint)

1. Ties on due date: order by `InvoiceId` as a second key? Proposed: yes, for a stable order.

## What I did not look at

Real statement volumes; whether ordering should happen in SQL (the list is small per customer).
