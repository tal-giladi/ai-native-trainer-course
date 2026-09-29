# ADR 0007 — Dapper repositories replace SqlHelper

- **Status:** Accepted
- **Date:** 2024-04-22
- **Deciders:** Billing team (owner: @contoso/billing-leads)

## Context

`SqlHelper.ExecuteDataSet` returns untyped `DataSet`s. Three production incidents in 2023
(BILL-40, BILL-44, BILL-52) came from misspelled column names that compiled fine and failed at
runtime. The helper also opened a connection per call with no cancellation support.

## Decision

- New data access uses Dapper in a repository class behind an interface
  (example: `IInvoiceRepository` / `InvoiceRepository`).
- SQL stays parameterized; string concatenation of values into SQL is rejected in review.
- Repositories take a `Func<IDbConnection>` so tests can substitute the connection.
- Every async method takes a `CancellationToken`.
- `SqlHelper` is marked `[Obsolete]` and stays only for `MonthlyRevenueReport` until BILL-97.

## Consequences

- Typed results; column-name mistakes surface in tests.
- One remaining legacy caller. `ConventionTests` fails the build if new code calls `SqlHelper`.
- `docs/ARCHITECTURE.md` still describes the old approach and was not updated (known debt).
