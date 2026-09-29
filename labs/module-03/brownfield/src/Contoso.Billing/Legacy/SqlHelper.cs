using System.Data;
using Microsoft.Data.SqlClient;

namespace Contoso.Billing.Legacy;

// Written in 2016. Kept only for the monthly revenue report until BILL-97 migrates it.
// New code must not call this class: see docs/adr/0007-dapper-repositories.md.
[Obsolete("Use a Dapper repository behind an interface. See docs/adr/0007-dapper-repositories.md.")]
public static class SqlHelper
{
    public static DataSet ExecuteDataSet(string connectionString, string sql, params SqlParameter[] parameters)
    {
        using var connection = new SqlConnection(connectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddRange(parameters);
        using var adapter = new SqlDataAdapter(command);
        var result = new DataSet();
        adapter.Fill(result);
        return result;
    }
}
