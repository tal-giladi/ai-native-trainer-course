namespace Contoso.Billing.Reports;

/// <summary>One row of the monthly revenue report. Money is decimal, never double.</summary>
public sealed record CustomerRevenue(int CustomerId, decimal Revenue);
