# Research brief — BILL-97 Move the monthly revenue report off SqlHelper

Schema change: no

## Reuse

- Repository pattern: `src/Contoso.Billing/Invoices/InvoiceRepository.cs` (Dapper, `Func<IDbConnection>`, `CommandDefinition` with the token).
- Test pattern: in-memory repository in `tests/Contoso.Billing.Tests/InvoiceServiceTests.cs`.

## Constraints

- ADR 0007: Dapper behind an interface; `SqlHelper` only in `Legacy/` (`docs/adr/0007-dapper-repositories.md`).
- `tests/Contoso.Billing.Tests/ConventionTests.cs` fails on `SqlHelper.` outside `Legacy/` and on `DateTime.Now`.
- `docs/ARCHITECTURE.md` says the opposite (SqlHelper for everything); it is from 2021 and stale.
- Finance's definition: every status counts (`prd/PRD-finance-close-q4.md`, non-goals; `company/glossary.md`).

## Blast radius

- `src/Contoso.Billing/Legacy/MonthlyRevenueReport.cs` is the only caller of `SqlHelper`; no caller of the report in this repository (the month-end job lives elsewhere).

## Open questions

1. Finance: should void and draft invoices count as revenue? Today they do. Not changed here.
2. `YEAR()/MONTH()` on a UTC `datetimeoffset`: an invoice issued at 23:30 Israel time on the last day of a month counts in the next month. Not changed here.
