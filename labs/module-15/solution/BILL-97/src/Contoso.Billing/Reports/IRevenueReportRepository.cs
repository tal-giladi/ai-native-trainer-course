namespace Contoso.Billing.Reports;

public interface IRevenueReportRepository
{
    /// <summary>Revenue per customer for invoices issued in the month, whatever their status (Finance's definition).</summary>
    Task<IReadOnlyList<CustomerRevenue>> MonthlyByCustomerAsync(int year, int month, CancellationToken ct);
}
