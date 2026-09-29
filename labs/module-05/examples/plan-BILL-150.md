# Plan — BILL-150 Record that a payment reminder was sent

Author: agent (plan mode), reviewed by: ______ on ______. Research: `research-BILL-150.md`.

## Goal

`InvoiceService.RecordReminderAsync(invoiceId)` records a reminder for an overdue invoice unless one
was recorded less than 7 days ago; reminders are stored in a new table `dbo.InvoiceReminder`.

## Evidence

- `docs/adr/0007-dapper-repositories.md` — data-access pattern for new code.
- `src/Contoso.Billing/Invoices/InvoiceRepository.cs` — the repository to imitate.
- `src/Contoso.Billing/Invoices/InvoiceService.cs` — `IsOverdue` to reuse; constructor to extend.
- `db/migrations/V003__utc_offsets_and_status.sql` — V###/U### convention.
- `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs` — 2-argument constructor used by existing tests.

## Approach

- New table `dbo.InvoiceReminder (InvoiceReminderId, InvoiceId FK, SentUtc datetimeoffset(0))`; `dbo.Invoice` unchanged. (ev: `db/migrations/V003__utc_offsets_and_status.sql`)
- New `IInvoiceReminderRepository` + `InvoiceReminderRepository` with Dapper, `Func<IDbConnection>`, `CancellationToken`. (ev: `docs/adr/0007-dapper-repositories.md`)
- Reminder repository is an optional third constructor argument so the 3 existing tests compile unchanged. (ev: `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs`)
- "Overdue" is `IsOverdue`, not a new condition; "now" is `IClock.UtcNow`. (ev: `src/Contoso.Billing/Invoices/InvoiceService.cs`)
- Assumption A1: exactly 7 days since the last reminder is allowed (`now - last < 7 days` refuses). (ev: `tickets/BILL-150.md`)

## Boundaries

### Touch

- `db/migrations/V005__invoice_reminder.sql`
- `db/migrations/U005__invoice_reminder.sql`
- `src/Contoso.Billing/Invoices/IInvoiceReminderRepository.cs`
- `src/Contoso.Billing/Invoices/InvoiceReminderRepository.cs`
- `src/Contoso.Billing/Invoices/InvoiceService.cs`
- `tests/Contoso.Billing.Tests/InvoiceReminderTests.cs`

### Do not touch

- `src/Contoso.Billing/Legacy/**` (BILL-97 owns it)
- `db/migrations/V00[1-4]*` and `db/migrations/U00[1-4]*` (merged migrations are immutable)
- `tests/Contoso.Billing.Tests/ConventionTests.cs`
- `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs`
- `*.csproj` (no new packages)

## Steps

1. Write `InvoiceReminderTests.cs` with the 4 cases from the ticket; they fail to compile. verify: `dotnet build` fails only on the missing members.
2. Add the interface and `RecordReminderAsync`. verify: the 4 new tests pass; the 6 old tests still pass.
3. Add `InvoiceReminderRepository` following `InvoiceRepository`. verify: `LoopGate arch` passes.
4. Add V005 and U005. verify: `LoopGate arch` migration-pair passes; optional `SET NOEXEC ON` compile against a dev database.

## Checkpoints

- HUMAN: approve this plan, especially Assumption A1 and the new table, before step 1.
- HUMAN: review the diff against Boundaries before merge; `LoopGate scope` must be clean.
- STOP rule: if the same step fails twice, stop and report instead of trying a third fix.

## Validation

- `dotnet run --project tools/LoopGate -- all --repo . --rules gates/architecture.rules --min-tests 6 --plan plan-BILL-150.md --git main`
- Expected: 10 tests (6 old + 4 new), 0 skipped, arch clean, scope clean.

## Out of scope

Sending the email; a reminder history screen; migrating `MonthlyRevenueReport` (BILL-97).
