# Plan — BILL-150 Record that a payment reminder was sent

Author: agent (plan mode). Reviewed by: tech lead, approved in 3 minutes. Research: `research-BILL-150.md`.

## Goal

`InvoiceService.RecordReminderAsync(invoiceId)` records a reminder for an overdue invoice unless one
was recorded less than 7 days ago; reminders are stored in a new table `dbo.InvoiceReminder`.

## Evidence

- `src/Contoso.Billing/Invoices/InvoiceRepository.cs` — read-only repository; no write pattern exists.
- `src/Contoso.Billing/Invoices/InvoiceService.cs` — `IsOverdue` to reuse.
- `db/migrations/V003__utc_offsets_and_status.sql` — V###/U### convention.

## Approach

- New table `dbo.InvoiceReminder`; `dbo.Invoice` unchanged. (ev: `db/migrations/V003__utc_offsets_and_status.sql`)
- Introduce EF Core (`BillingDbContext`) for writes, since the repositories have no write pattern to follow. (ev: `src/Contoso.Billing/Invoices/InvoiceRepository.cs`)
- `BillingDbContext` is an optional third constructor argument so existing tests compile. (ev: `src/Contoso.Billing/Invoices/InvoiceService.cs`)
- "Overdue" is `IsOverdue`; "now" is `IClock.UtcNow`. (ev: `src/Contoso.Billing/Invoices/InvoiceService.cs`)

## Boundaries

### Touch

- `db/migrations/V005__invoice_reminder.sql`
- `db/migrations/U005__invoice_reminder.sql`
- `src/Contoso.Billing/Data/**`
- `src/Contoso.Billing/Invoices/InvoiceReminder.cs`
- `src/Contoso.Billing/Invoices/InvoiceService.cs`
- `src/Contoso.Billing/Contoso.Billing.csproj`
- `tests/Contoso.Billing.Tests/Contoso.Billing.Tests.csproj`
- `tests/Contoso.Billing.Tests/InvoiceReminderTests.cs`

### Do not touch

- `src/Contoso.Billing/Legacy/**`
- `tests/Contoso.Billing.Tests/ConventionTests.cs`

## Steps

1. Add EF Core packages and `BillingDbContext`. verify: `dotnet build` succeeds.
2. Add `RecordReminderAsync` and tests with the EF in-memory provider. verify: all tests pass.
3. Add V005 and U005. verify: files exist as a pair.

## Checkpoints

- HUMAN: approve this plan before step 1.

## Validation

- `dotnet test Contoso.Billing.sln`
