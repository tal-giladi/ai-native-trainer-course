using Contoso.Billing.Reports;
using Xunit;

namespace Contoso.Billing.Tests;

public class MonthlyRevenueReportTests
{
    private sealed class InMemoryRevenue(params (int Year, int Month, CustomerRevenue Row)[] rows) : IRevenueReportRepository
    {
        public Task<IReadOnlyList<CustomerRevenue>> MonthlyByCustomerAsync(int year, int month, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<CustomerRevenue>>(
                rows.Where(r => r.Year == year && r.Month == month).Select(r => r.Row).ToList());
    }

    [Fact]
    public async Task Two_customers_in_one_month_are_returned_in_customer_order()
    {
        var sut = new MonthlyRevenueReport(new InMemoryRevenue(
            (2026, 8, new CustomerRevenue(9, 50.00m)),
            (2026, 8, new CustomerRevenue(7, 120.10m)),
            (2026, 9, new CustomerRevenue(7, 999m))));

        var rows = await sut.LoadAsync(2026, 8, CancellationToken.None);

        Assert.Equal([new CustomerRevenue(7, 120.10m), new CustomerRevenue(9, 50.00m)], rows);
    }

    [Fact]
    public async Task An_empty_month_returns_no_rows()
    {
        var sut = new MonthlyRevenueReport(new InMemoryRevenue());
        Assert.Empty(await sut.LoadAsync(2026, 2, CancellationToken.None));
    }

    // Characterization test: Finance's number is defined by this filter. If you change it, you change
    // revenue; get Finance's written yes first (PRD Finance close Q4, non-goals).
    [Fact]
    public void The_filter_is_the_2016_filter_and_counts_every_status()
    {
        Assert.Contains("SUM(Amount)", RevenueReportRepository.Sql);
        Assert.Contains("WHERE YEAR(IssuedUtc) = @y AND MONTH(IssuedUtc) = @m GROUP BY CustomerId", RevenueReportRepository.Sql);
        Assert.DoesNotContain("Status", RevenueReportRepository.Sql);
    }
}
