using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Contoso.Billing.Legacy;

// Migrated from SqlHelper to Dapper for consistency with the new reminder repository (BILL-150).
public sealed class MonthlyRevenueReport
{
    private readonly string _connectionString;

    public MonthlyRevenueReport(string connectionString) => _connectionString = connectionString;

    public DataTable Load(int year, int month)
    {
        var from = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero);
        using var connection = new SqlConnection(_connectionString);
        using var reader = connection.ExecuteReader(
            "SELECT CustomerId, SUM(Amount) AS Revenue FROM dbo.Invoice " +
            "WHERE IssuedUtc >= @from AND IssuedUtc < @to GROUP BY CustomerId",
            new { from, to = from.AddMonths(1) });
        var table = new DataTable();
        table.Load(reader);
        return table;
    }
}
