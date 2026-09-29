# HANDOFF — BILL-142 overdue invoices (example)

Written by the agent at the user's request before `/clear`. A fresh session reads this file first.
Keep it under ~40 lines; it is task context, not a rules file. Delete it when the ticket is merged.

## Goal

BILL-142: `InvoiceService` returns one customer's overdue invoices, oldest due first, with whole days overdue (UTC).

## Constraints (verbatim from the user or ticket)

- "New migrations in this task are numbered V010/U010; V005-V009 are reserved by the reporting branch." (user, session 1)
- "Do not load paid or void invoices into memory." (ticket AC 4)

## Decisions

- New repository method `ListOverdueAsync(int customerId, DateTimeOffset nowUtc, CancellationToken ct)` on `IInvoiceRepository`; SQL filters `Status = 1 AND DueUtc < @nowUtc`, ordered by `DueUtc`.
- Days overdue = `(int)Math.Floor((clock.UtcNow - DueUtc).TotalDays)` in `InvoiceService`.
- No schema change needed: `IX_Invoice_CustomerId_Status` covers the query.

## Done

- Interface method + Dapper implementation (`src/Contoso.Billing/Invoices/`).
- Tests: no invoices, one overdue (both green).

## Next

1. Test: a paid invoice past its due date is not returned (AC 5, third case).
2. `dotnet test Contoso.Billing.sln`; all green before saying done.

## Open questions

- Should a Draft invoice past its due date count? Ticket is silent; `IsOverdue` says no. Asked the reporter.
