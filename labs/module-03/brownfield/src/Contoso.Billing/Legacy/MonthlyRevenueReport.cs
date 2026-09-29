using System.Data;
using Microsoft.Data.SqlClient;

namespace Contoso.Billing.Legacy;

// The last caller of SqlHelper. Scheduled for migration in BILL-97.
public sealed class MonthlyRevenueReport
{
    private readonly string _connectionString;

    public MonthlyRevenueReport(string connectionString) => _connectionString = connectionString;

    public DataTable Load(int year, int month)
    {
        var ds = SqlHelper.ExecuteDataSet(
            _connectionString,
            "SELECT CustomerId, SUM(Amount) AS Revenue FROM dbo.Invoice " +
            "WHERE YEAR(IssuedUtc) = @y AND MONTH(IssuedUtc) = @m GROUP BY CustomerId",
            new SqlParameter("@y", year),
            new SqlParameter("@m", month));
        return ds.Tables[0];
    }
}
