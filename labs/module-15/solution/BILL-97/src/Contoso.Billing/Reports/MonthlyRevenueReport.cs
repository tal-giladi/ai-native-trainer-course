namespace Contoso.Billing.Reports;

/// <summary>Finance's monthly revenue per customer. Reconciled against the ledger every month.</summary>
public sealed class MonthlyRevenueReport
{
    private readonly IRevenueReportRepository _repository;

    public MonthlyRevenueReport(IRevenueReportRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<CustomerRevenue>> LoadAsync(int year, int month, CancellationToken ct)
    {
        if (month is < 1 or > 12) throw new ArgumentOutOfRangeException(nameof(month));
        var rows = await _repository.MonthlyByCustomerAsync(year, month, ct);
        return rows.OrderBy(r => r.CustomerId).ToList();
    }
}
