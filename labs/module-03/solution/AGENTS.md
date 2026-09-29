# Contoso Billing — agent rules

Owner: @contoso/billing-leads · Changes need owner review (see `CODEOWNERS`) · Log: `AI-LAYER-CHANGELOG.md`
Every rule names its evidence as `(ev: …)`. No evidence, no rule.

## Build and test

- Build: `dotnet build Contoso.Billing.sln` (ev: `Contoso.Billing.sln`)
- Test: `dotnet test Contoso.Billing.sln`; all tests must pass before you say you are done. (ev: `tests/Contoso.Billing.Tests/Contoso.Billing.Tests.csproj`)
- `tests/Contoso.Billing.Tests/ConventionTests.cs` encodes house rules; if it fails, fix the code, never the test.

## Data access

- New data access: Dapper in a repository behind an interface, following `IInvoiceRepository` / `InvoiceRepository`. (ev: `docs/adr/0007-dapper-repositories.md`)
- Never call `SqlHelper` from new code; it is `[Obsolete]` and only `src/Contoso.Billing/Legacy/` may use it. (ev: `src/Contoso.Billing/Legacy/SqlHelper.cs`)
- `docs/ARCHITECTURE.md` is stale (2021) on data access, dates and build; trust the ADR and the code. (ev: `docs/history-excerpt.txt`)
- Repositories take `Func<IDbConnection>`; every async method takes a `CancellationToken`. (ev: `src/Contoso.Billing/Invoices/InvoiceRepository.cs`)
- Filter in SQL, not in memory, when the ticket says so; the index `IX_Invoice_CustomerId_Status` covers `(CustomerId, Status)`. (ev: `db/migrations/V003__utc_offsets_and_status.sql`)

## Time and money

- "Now" comes from `IClock.UtcNow`; never `DateTime.Now`. Columns are `datetimeoffset(0)` in UTC. (ev: `src/Contoso.Billing/Common/IClock.cs`)
- Money is `decimal` in C# and `decimal(19,4)` in SQL Server; never `double`. (ev: `src/Contoso.Billing/Invoices/Invoice.cs`)

## Database migrations

- New migration = `db/migrations/V###__name.sql` plus a matching `U###__name.sql` undo script; a PR without the `U###` file is rejected. (ev: `db/migrations/V003__utc_offsets_and_status.sql`)
- Never edit a `V###` script that is already merged; add a new one. (ev: `docs/history-excerpt.txt`)
