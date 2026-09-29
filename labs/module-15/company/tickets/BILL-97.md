# BILL-97 — Move the monthly revenue report off SqlHelper

**Type:** Tech debt · **Priority:** Low · **Reporter:** Avi Ben-David · **Created:** 2024-06-02

## Description

`MonthlyRevenueReport` is the last caller of `SqlHelper` (ADR 0007). Until it moves, `SqlHelper`
cannot be deleted, and the new month-end reporting job (PRD Finance close Q4, requirement 1) cannot
call the report without a connection string.

Finance reconciles this report against the ledger on the first business days of every month.
Nobody wants to be the person who changes Finance's number, which is why this ticket is two years old.

## Acceptance criteria

1. The report gets its data from a Dapper repository behind an interface, as ADR 0007 describes,
   and every async method takes a `CancellationToken`.
2. No code outside `Legacy/` calls `SqlHelper`, and the report no longer calls it either.
   Do not delete `SqlHelper` in this ticket.
3. The report returns the same rows as today for the same month: same filter, same grouping, same
   statuses, one row per customer with `CustomerId` and `Revenue`. If you think today's numbers are
   wrong, write it down as an open question for Finance; do not change them.
4. Unit tests cover: two customers in one month, an empty month, and a test that fails if the
   report's filter changes.
5. `dotnet test` passes and the number of tests goes up.
