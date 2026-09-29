# Research brief — BILL-150 Record that a payment reminder was sent

Researcher: Explore sub-agent, read-only, 11 minutes, ~38k tokens explored, this brief ~900 tokens.
Rules version: AGENTS.md v1.1.0. Date: 2026-09-28.

## Question

Where does "record a reminder" belong in Contoso Billing, which existing code must it reuse, and
what does the schema change have to look like to be accepted?

## Existing capabilities to reuse

| Need in the ticket | Already exists | Evidence |
|---|---|---|
| "Overdue" (criterion 2) | `InvoiceService.IsOverdue(Invoice)` | `src/Contoso.Billing/Invoices/InvoiceService.cs` |
| Load one invoice | `IInvoiceRepository.GetAsync` | `src/Contoso.Billing/Invoices/IInvoiceRepository.cs` |
| "Now" in UTC | `IClock.UtcNow` | `src/Contoso.Billing/Common/IClock.cs` |
| Test doubles for the clock and repository | `FixedClock`, `InMemoryInvoices` (private to the test class) | `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs` |

Nothing records a reminder today (searched `Reminder`, `Remind`, `Dunning`: 0 hits in `src/`, `db/`).

## Architecture facts that constrain the design

| Fact | Evidence (strongest first) | Rung |
|---|---|---|
| New data access = Dapper repository behind an interface; `Func<IDbConnection>`; `CancellationToken` everywhere | `docs/adr/0007-dapper-repositories.md`, `src/Contoso.Billing/Invoices/InvoiceRepository.cs` | ADR + code |
| No write method exists in any repository yet; this is the first INSERT | `src/Contoso.Billing/Invoices/InvoiceRepository.cs` | code |
| Every migration from V003 on ships with a `U###` undo script | `db/migrations/V003__utc_offsets_and_status.sql` | code comment + files |
| Never edit a merged `V###` | `docs/history-excerpt.txt` | history |
| `docs/ARCHITECTURE.md` is stale (2021, SqlHelper, DateTime.Now) | `docs/history-excerpt.txt`, `tests/Contoso.Billing.Tests/ConventionTests.cs` | tests beat docs |
| `InvoiceService` is constructed with 2 arguments in 3 existing tests | `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs` | code |

## Dependencies and blast radius

- Callers of `InvoiceService` constructor: tests only (no composition root in this library).
- Schema: new table only. `dbo.Invoice` and index `IX_Invoice_CustomerId_Status` untouched.
- `src/Contoso.Billing/Legacy/` is not involved (BILL-97 owns its migration).

## Open questions (for the plan checkpoint)

1. "Within 7 days": is exactly 7 days allowed? Ticket gives 3 (refuse) and 8 (allow). Proposed: `now - last < 7 days` is refused, so exactly 7 days is allowed. Needs a product answer or an explicit assumption.
2. Should an unknown invoice id throw or return false? Proposed: return false, same as "not overdue".

## What I did not look at

No database was available; the migration was not executed. `SET NOEXEC ON` compile check is left to validation.
