using System.ComponentModel;
using System.Text;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace ContosoSchema.Mcp;

public enum StalePolicy { Refuse, Warn }

/// <summary>Loads the catalog once per call and attaches freshness to it.</summary>
public sealed class SchemaService(ICatalogSource source, string? migrationsDir, double maxAgeHours, StalePolicy onStale)
{
    public StalePolicy OnStale => onStale;

    public (SchemaCatalog Catalog, Freshness Freshness) Load()
    {
        var c = source.Load();
        return (c, Freshness.Evaluate(c, migrationsDir, maxAgeHours, DateTimeOffset.UtcNow));
    }
}

/// <summary>
/// Four read-only tools. Every answer starts with the freshness header; a stale source is refused
/// (isError) so the model has to act on it instead of skimming past it.
/// </summary>
[McpServerToolType]
public sealed class SchemaTools(SchemaService schema)
{
    [McpServerTool(Name = "list_tables", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
    [Description("Lists the user tables of the Contoso Billing SQL Server database with their column counts. " +
                 "Use before writing SQL or a repository method. Read-only; returns schema, never row data.")]
    public CallToolResult ListTables() => Answer((c, sb) =>
    {
        foreach (var t in c.Tables.OrderBy(t => t.Name))
            sb.AppendLine($"{t.Name} ({t.Columns.Count} columns)");
    });

    [McpServerTool(Name = "describe_table", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
    [Description("Returns the columns of one table (name, SQL Server type, NULL or NOT NULL). " +
                 "Use before writing SQL that reads or writes the table. Read-only; returns schema, never row data.")]
    public CallToolResult DescribeTable(
        [Description("Table name, e.g. dbo.Invoice or Invoice")] string table) => Answer((c, sb) =>
    {
        var t = c.FindTable(table) ?? throw new ToolInputException(
            $"No table '{table}'. Known tables: {string.Join(", ", c.Tables.Select(x => x.Name))}.");
        sb.AppendLine(t.Name);
        foreach (var col in t.Columns)
            sb.AppendLine($"  {col.Name,-14} {col.Type,-18} {(col.Nullable ? "NULL" : "NOT NULL")}");
    });

    [McpServerTool(Name = "list_procedures", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
    [Description("Lists the stored procedures in the Contoso Billing database. Use to find existing data access " +
                 "before writing new SQL. Read-only.")]
    public CallToolResult ListProcedures() => Answer((c, sb) =>
    {
        if (c.Procedures.Count == 0) sb.AppendLine("(no stored procedures)");
        foreach (var p in c.Procedures.OrderBy(p => p.Name))
            sb.AppendLine($"{p.Name} ({p.Parameters.Count} parameters)");
    });

    [McpServerTool(Name = "describe_procedure", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false)]
    [Description("Returns the parameters of one stored procedure (name, type, OUTPUT). Read-only.")]
    public CallToolResult DescribeProcedure(
        [Description("Procedure name, e.g. dbo.usp_InvoiceNote_LatestByCustomer")] string procedure) => Answer((c, sb) =>
    {
        var p = c.FindProc(procedure) ?? throw new ToolInputException(
            $"No procedure '{procedure}'. Known procedures: {(c.Procedures.Count == 0 ? "none" : string.Join(", ", c.Procedures.Select(x => x.Name)))}.");
        sb.AppendLine(p.Name);
        foreach (var pa in p.Parameters)
            sb.AppendLine($"  {pa.Name,-14} {pa.Type,-18}{(pa.Output ? " OUTPUT" : "")}");
    });

    CallToolResult Answer(Action<SchemaCatalog, StringBuilder> body)
    {
        var sb = new StringBuilder();
        try
        {
            var (catalog, fresh) = schema.Load();
            sb.AppendLine(fresh.Header);
            if (fresh.Status == FreshnessStatus.Stale && schema.OnStale == StalePolicy.Refuse)
            {
                sb.AppendLine($"REFUSED: {fresh.Reason}.");
                if (fresh.NotInSource.Count > 0)
                    sb.AppendLine("Read these migration files for the current shape: " +
                                  string.Join(", ", fresh.NotInSource.Select(f => "db/migrations/" + f)) + ".");
                sb.AppendLine("Or refresh the snapshot: ContosoSchema.Mcp snapshot --source sql --out <file>.");
                return Result(sb, isError: true);
            }
            if (fresh.Status != FreshnessStatus.Fresh)
                sb.AppendLine($"WARNING: {fresh.Reason}.");
            body(catalog, sb);
            return Result(sb, isError: false);
        }
        catch (ToolInputException e)
        {
            sb.AppendLine(e.Message);
            return Result(sb, isError: true);
        }
        catch (Exception e)
        {
            // Do not leak connection strings or stack traces into the model's context.
            Console.Error.WriteLine(e);
            sb.AppendLine($"The schema source is unavailable ({e.GetType().Name}). Do not guess the schema; read db/migrations instead.");
            return Result(sb, isError: true);
        }
    }

    static CallToolResult Result(StringBuilder sb, bool isError) => new()
    {
        Content = [new TextContentBlock { Text = sb.ToString().Replace("\r\n", "\n").TrimEnd() }],
        IsError = isError
    };
}

public sealed class ToolInputException(string message) : Exception(message);
