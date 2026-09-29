using System.Data;
using Dapper;

namespace Contoso.Billing.Reports;

// BILL-97: moved off SqlHelper (ADR 0007). The SQL is the 2016 query, unchanged on purpose:
// Finance reconciles this number every month, and it includes every status (see the PRD non-goal).
public sealed class RevenueReportRepository : IRevenueReportRepository
{
    public const string Sql =
        "SELECT CustomerId, SUM(Amount) AS Revenue FROM dbo.Invoice " +
        "WHERE YEAR(IssuedUtc) = @y AND MONTH(IssuedUtc) = @m GROUP BY CustomerId";

    private readonly Func<IDbConnection> _connectionFactory;

    public RevenueReportRepository(Func<IDbConnection> connectionFactory) => _connectionFactory = connectionFactory;

    public async Task<IReadOnlyList<CustomerRevenue>> MonthlyByCustomerAsync(int year, int month, CancellationToken ct)
    {
        using var connection = _connectionFactory();
        var rows = await connection.QueryAsync<CustomerRevenue>(new CommandDefinition(
            Sql, new { y = year, m = month }, cancellationToken: ct));
        return rows.AsList();
    }
}
