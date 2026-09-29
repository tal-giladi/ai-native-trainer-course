using System.Data;
using Microsoft.Data.SqlClient;

namespace Contoso.Billing.Legacy;

// Shared data access helper. Use this for all database calls.
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
