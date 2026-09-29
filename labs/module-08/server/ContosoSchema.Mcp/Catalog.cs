using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;

namespace ContosoSchema.Mcp;

public sealed record ColumnInfo(string Name, string Type, bool Nullable);
public sealed record TableInfo(string Name, List<ColumnInfo> Columns);
public sealed record ParamInfo(string Name, string Type, bool Output);
public sealed record ProcInfo(string Name, List<ParamInfo> Parameters);

/// <summary>What the server knows about the database, and where and when it learned it.</summary>
public sealed record SchemaCatalog(
    string Source,
    DateTimeOffset CapturedUtc,
    int SchemaVersion,
    bool Live,
    List<TableInfo> Tables,
    List<ProcInfo> Procedures)
{
    public TableInfo? FindTable(string name) =>
        Tables.FirstOrDefault(t => string.Equals(t.Name, Normalize(name), StringComparison.OrdinalIgnoreCase));

    public ProcInfo? FindProc(string name) =>
        Procedures.FirstOrDefault(p => string.Equals(p.Name, Normalize(name), StringComparison.OrdinalIgnoreCase));

    /// <summary>"Invoice", "dbo.Invoice" and "[dbo].[Invoice]" all mean dbo.Invoice.</summary>
    public static string Normalize(string name)
    {
        var n = name.Replace("[", "").Replace("]", "").Trim();
        return n.Contains('.') ? n : "dbo." + n;
    }

    public static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };
}

public interface ICatalogSource
{
    string Describe { get; }
    SchemaCatalog Load();
}

/// <summary>A JSON file written earlier by <c>snapshot</c>. Cheap and offline, and only as fresh as its last refresh.</summary>
public sealed class SnapshotSource(string path) : ICatalogSource
{
    public string Describe => "snapshot:" + Path.GetFileName(path);

    public SchemaCatalog Load()
    {
        var c = JsonSerializer.Deserialize<SchemaCatalog>(File.ReadAllText(path), SchemaCatalog.Json)
                ?? throw new InvalidDataException($"empty snapshot: {path}");
        return c with { Source = Describe, Live = false };
    }
}

/// <summary>
/// Reads SQL Server's catalog views. The login needs VIEW DEFINITION and SELECT on dbo.SchemaHistory only:
/// it can see the shape of every table and cannot read a single invoice.
/// </summary>
public sealed class SqlSource(string connectionString) : ICatalogSource
{
    public string Describe
    {
        get
        {
            var b = new SqlConnectionStringBuilder(connectionString); // never echo the password
            return $"sql:{b.DataSource}/{b.InitialCatalog}";
        }
    }

    public SchemaCatalog Load()
    {
        using var cn = new SqlConnection(connectionString);
        cn.Open();
        var tables = new Dictionary<string, TableInfo>(StringComparer.OrdinalIgnoreCase);
        using (var cmd = new SqlCommand("""
            SELECT s.name + '.' + t.name, c.name, ty.name, c.max_length, c.precision, c.scale, c.is_nullable
            FROM sys.tables t
            JOIN sys.schemas s ON s.schema_id = t.schema_id
            JOIN sys.columns c ON c.object_id = t.object_id
            JOIN sys.types ty ON ty.user_type_id = c.user_type_id
            WHERE t.is_ms_shipped = 0 AND t.name <> 'SchemaHistory'
            ORDER BY 1, c.column_id
            """, cn))
        using (var r = cmd.ExecuteReader())
            while (r.Read())
            {
                var name = r.GetString(0);
                if (!tables.TryGetValue(name, out var t)) tables[name] = t = new TableInfo(name, new());
                t.Columns.Add(new ColumnInfo(r.GetString(1),
                    FormatType(r.GetString(2), r.GetInt16(3), r.GetByte(4), r.GetByte(5)), r.GetBoolean(6)));
            }

        var procs = new Dictionary<string, ProcInfo>(StringComparer.OrdinalIgnoreCase);
        using (var cmd = new SqlCommand("""
            SELECT s.name + '.' + p.name, pa.name, ty.name, pa.max_length, pa.precision, pa.scale, pa.is_output
            FROM sys.procedures p
            JOIN sys.schemas s ON s.schema_id = p.schema_id
            LEFT JOIN sys.parameters pa ON pa.object_id = p.object_id
            LEFT JOIN sys.types ty ON ty.user_type_id = pa.user_type_id
            WHERE p.is_ms_shipped = 0
            ORDER BY 1, pa.parameter_id
            """, cn))
        using (var r = cmd.ExecuteReader())
            while (r.Read())
            {
                var name = r.GetString(0);
                if (!procs.TryGetValue(name, out var p)) procs[name] = p = new ProcInfo(name, new());
                if (!r.IsDBNull(1))
                    p.Parameters.Add(new ParamInfo(r.GetString(1),
                        FormatType(r.GetString(2), r.GetInt16(3), r.GetByte(4), r.GetByte(5)), r.GetBoolean(6)));
            }

        using var v = new SqlCommand("SELECT ISNULL(MAX(Version), 0) FROM dbo.SchemaHistory", cn);
        var version = Convert.ToInt32(v.ExecuteScalar());
        return new SchemaCatalog(Describe, DateTimeOffset.UtcNow, version, Live: true,
            tables.Values.ToList(), procs.Values.ToList());
    }

    static string FormatType(string type, short maxLength, byte precision, byte scale) => type switch
    {
        "nvarchar" or "nchar" => $"{type}({(maxLength == -1 ? "max" : (maxLength / 2).ToString())})",
        "varchar" or "char" or "varbinary" or "binary" => $"{type}({(maxLength == -1 ? "max" : maxLength.ToString())})",
        "decimal" or "numeric" => $"{type}({precision},{scale})",
        "datetimeoffset" or "datetime2" or "time" => $"{type}({scale})",
        _ => type
    };
}
