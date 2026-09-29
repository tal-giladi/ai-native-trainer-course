# Plan — BILL-97

Brief: `research/BILL-97.md`

## Touch

- `src/Contoso.Billing/Reports/CustomerRevenue.cs` (new)
- `src/Contoso.Billing/Reports/IRevenueReportRepository.cs` (new)
- `src/Contoso.Billing/Reports/RevenueReportRepository.cs` (new, the 2016 SQL unchanged)
- `src/Contoso.Billing/Reports/MonthlyRevenueReport.cs` (moved from `Legacy/`)
- `tests/Contoso.Billing.Tests/MonthlyRevenueReportTests.cs` (new)

## Do not touch

- `src/Contoso.Billing/Legacy/SqlHelper.cs` (stays; deleting it is a separate ticket)
- `db/migrations/**`, `tests/Contoso.Billing.Tests/ConventionTests.cs`, any `*.csproj`

## Steps

1. Test first: the three tests from the acceptance criteria, including the filter characterization test.
2. Repository and record; report on top of it; delete the `Legacy/` copy.
3. `dotnet test`: 6 tests before, 9 after.

## Checkpoints

- HUMAN: the two open questions go into the PR description for Finance, unanswered.
