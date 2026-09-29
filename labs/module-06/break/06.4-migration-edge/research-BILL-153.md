# Research brief — BILL-153 Store the customer's purchase-order number on every invoice

Researcher: prime 1.0.0 (forked Explore agent), read-only · ~22k tokens explored · brief ~700 tokens
Rules version: n/a · Date: 2026-09-28

## Question

Where does a PO number belong on invoices, and which code returns invoices today?

## Existing capabilities to reuse

| Need in the ticket | Already exists | Evidence (path) |
|---|---|---|
| Invoice shape | `Invoice` record | `src/Contoso.Billing/Invoices/Invoice.cs` |
| Read one / list by customer (criterion 2) | `InvoiceRepository.GetAsync`, `ListByCustomerAsync` | `src/Contoso.Billing/Invoices/InvoiceRepository.cs` |
| Test doubles | `InMemoryInvoices`, `Inv(...)` helper | `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs` |

## Architecture facts that constrain the design

| Fact | Evidence (strongest first) | Rung |
|---|---|---|
| Data access is Dapper in repositories behind an interface | `src/Contoso.Billing/Invoices/InvoiceRepository.cs` | code |
| Async repository methods take a `CancellationToken` | `src/Contoso.Billing/Invoices/IInvoiceRepository.cs` | code |
| House rules are enforced by convention tests (no `SqlHelper` outside Legacy, no local clock) | `tests/Contoso.Billing.Tests/ConventionTests.cs` | test |

## Dependencies and blast radius

- `IInvoiceRepository` is implemented once and faked once (`InMemoryInvoices`).
- `Invoice` is constructed by the `Inv(...)` test helper.
- The database needs a new `PoNumber` column on `dbo.Invoice`.

## Open questions (for the plan checkpoint)

1. Should the PO number be validated (format, length)? Proposed: max 35 characters, no format check.

## What I did not look at

The database.
