# Contoso Billing — agent rules

Owner: @contoso/billing-leads · Log: `AI-LAYER-CHANGELOG.md` · Context audit: `context/context-audit.md`
Every rule names its evidence as `(ev: …)`. Map of the code: `docs/ai/topology.md` (read it when you need to find where something lives).

## Build and test

- Test: `dotnet test Contoso.Billing.sln`; all tests must pass before you say you are done. (ev: `Contoso.Billing.sln`)
- `tests/Contoso.Billing.Tests/ConventionTests.cs` encodes house rules; if it fails, fix the code, never the test.

## Data access

- New data access: Dapper in a repository behind an interface, following `IInvoiceRepository` / `InvoiceRepository`; async methods take a `CancellationToken`. (ev: `docs/adr/0007-dapper-repositories.md`)
- Never call `SqlHelper` from new code; only `src/Contoso.Billing/Legacy/` may use it. (ev: `src/Contoso.Billing/Legacy/SqlHelper.cs`)
- `docs/ARCHITECTURE.md` is stale (2021) on data access, dates and build; trust the ADR and the code. (ev: `docs/history-excerpt.txt`)

## Time

- "Now" comes from `IClock.UtcNow`; never `DateTime.Now`. Columns are `datetimeoffset(0)` in UTC. (ev: `src/Contoso.Billing/Common/IClock.cs`)

## Database migrations

- New migration = `db/migrations/V###__name.sql` plus a matching `U###__name.sql` undo script; a PR without the `U###` file is rejected. (ev: `db/migrations/V003__utc_offsets_and_status.sql`)
- Never edit a `V###` script that is already merged; add a new one. (ev: `docs/history-excerpt.txt`)
- Details (types, index names): `.claude/rules/migrations.md` loads when you open files in `db/migrations/`.
