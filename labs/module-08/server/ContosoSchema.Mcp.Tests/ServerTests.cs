using ContosoSchema.Mcp;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Xunit;

namespace ContosoSchema.Mcp.Tests;

/// <summary>Offline tests: the two sample snapshots and throwaway migration folders. No database needed.</summary>
public class ServerTests
{
    static readonly string Samples = FindUp("samples");
    static readonly string Nightly = Path.Combine(Samples, "catalog-nightly.json");   // V005, 2026-08-18
    static readonly string Current = Path.Combine(Samples, "catalog-v006.json");      // V006, 2026-09-28
    static readonly DateTimeOffset Now = new(2026, 9, 28, 12, 0, 0, TimeSpan.Zero);

    static string FindUp(string name)
    {
        for (var d = new DirectoryInfo(AppContext.BaseDirectory); d is not null; d = d.Parent)
            if (Directory.Exists(Path.Combine(d.FullName, name))) return Path.Combine(d.FullName, name);
        throw new DirectoryNotFoundException(name);
    }

    static string Migrations(int head)
    {
        var dir = Directory.CreateTempSubdirectory("m8-mig-").FullName;
        for (var v = 1; v <= head; v++) File.WriteAllText(Path.Combine(dir, $"V{v:D3}__m{v}.sql"), "-- test");
        File.WriteAllText(Path.Combine(dir, "U006__not_a_version.sql"), "-- undo scripts are ignored");
        return dir;
    }

    static SchemaCatalog Load(string path) => new SnapshotSource(path).Load();

    [Fact]
    public void Snapshot_behind_the_repository_is_stale_and_names_the_missing_migration()
    {
        var f = Freshness.Evaluate(Load(Nightly), Migrations(6), 24, Now);
        Assert.Equal(FreshnessStatus.Stale, f.Status);
        Assert.Equal(["V006__m6.sql"], f.NotInSource);
        Assert.Contains("status=STALE", f.Header);
    }

    [Fact]
    public void Without_a_migrations_folder_freshness_is_unknown()
    {
        Assert.Equal(FreshnessStatus.Unknown, Freshness.Evaluate(Load(Nightly), null, 24, Now).Status);
    }

    [Fact]
    public void Matching_version_within_max_age_is_fresh_and_too_old_is_stale()
    {
        Assert.Equal(FreshnessStatus.Fresh, Freshness.Evaluate(Load(Current), Migrations(6), 24, Now).Status);
        Assert.Equal(FreshnessStatus.Stale, Freshness.Evaluate(Load(Current), Migrations(6), 24, Now.AddDays(3)).Status);
    }

    [Fact]
    public void Database_ahead_of_the_branch_is_reported()
    {
        Assert.Equal(FreshnessStatus.Ahead, Freshness.Evaluate(Load(Current), Migrations(5), 24, Now).Status);
    }

    [Fact]
    public void Stale_source_is_refused_with_isError_and_a_pointer_to_the_migration()
    {
        var tools = new SchemaTools(new SchemaService(new SnapshotSource(Nightly), Migrations(6), 24 * 365, StalePolicy.Refuse));
        var r = tools.DescribeTable("dbo.Invoice");
        var text = ((TextContentBlock)r.Content[0]).Text;
        Assert.True(r.IsError);
        Assert.Contains("REFUSED", text);
        Assert.Contains("db/migrations/V006__m6.sql", text);
        Assert.DoesNotContain("Notes", text);
    }

    [Fact]
    public void Fresh_source_describes_the_current_table_with_a_header()
    {
        var tools = new SchemaTools(new SchemaService(new SnapshotSource(Current), Migrations(6), 24 * 365, StalePolicy.Refuse));
        var r = tools.DescribeTable("Invoice");
        var text = ((TextContentBlock)r.Content[0]).Text;
        Assert.False(r.IsError);
        Assert.StartsWith("[schema] source=snapshot:catalog-v006.json version=V006", text);
        Assert.Contains("DueUtc", text);
        Assert.DoesNotContain("Notes", text);
    }

    [Fact]
    public void Unknown_table_is_a_tool_error_that_lists_known_tables()
    {
        var tools = new SchemaTools(new SchemaService(new SnapshotSource(Current), Migrations(6), 24 * 365, StalePolicy.Refuse));
        var r = tools.DescribeTable("dbo.Customer");
        Assert.True(r.IsError);
        Assert.Contains("dbo.InvoiceNote", ((TextContentBlock)r.Content[0]).Text);
    }

    [Theory]
    [InlineData("SELECT InvoiceId, Notes AS Body FROM dbo.Invoice WHERE CustomerId = @customerId AND Notes IS NOT NULL", "unknown column Notes")]
    [InlineData("SELECT i.InvoiceId, i.Notes FROM dbo.Invoice AS i", "unknown column Notes")]
    [InlineData("SELECT Body FROM dbo.InvoiceNotes", "unknown table dbo.InvoiceNotes")]
    [InlineData("dbo.usp_InvoiceNotes_Latest", "unknown procedure dbo.usp_InvoiceNotes_Latest")]
    public void Check_flags_what_the_database_does_not_have(string sql, string expected)
    {
        Assert.Contains(CheckSql.CheckStatement(sql, Load(Current)), m => m.StartsWith(expected));
    }

    [Theory]
    [InlineData("SELECT InvoiceId, CustomerId, Amount, IssuedUtc, DueUtc, Status FROM dbo.Invoice WHERE InvoiceId = @invoiceId")]
    [InlineData("SELECT CustomerId, SUM(Amount) AS Revenue FROM dbo.Invoice WHERE YEAR(IssuedUtc) = @y AND MONTH(IssuedUtc) = @m GROUP BY CustomerId")]
    [InlineData("SELECT i.InvoiceId, n.Body FROM dbo.Invoice AS i CROSS APPLY (SELECT TOP (1) x.Body FROM dbo.InvoiceNote AS x WHERE x.InvoiceId = i.InvoiceId ORDER BY x.CreatedUtc DESC) AS n WHERE i.Status = 1")]
    [InlineData("dbo.usp_InvoiceNote_LatestByCustomer")]
    public void Check_passes_valid_contoso_sql(string sql)
    {
        Assert.Empty(CheckSql.CheckStatement(sql, Load(Current)));
    }

    [Fact]
    public void The_stale_snapshot_would_have_passed_the_broken_query()
    {
        // Why the check must refuse a stale source: against V005, Notes exists and the bug looks fine.
        Assert.Empty(CheckSql.CheckStatement("SELECT InvoiceId, Notes FROM dbo.Invoice", Load(Nightly)));
    }

    [Fact]
    public async Task Over_stdio_the_server_lists_four_read_only_tools_and_answers_a_call()
    {
        var dll = typeof(SchemaTools).Assembly.Location;
        var transport = new StdioClientTransport(new StdioClientTransportOptions
        {
            Name = "contoso-schema",
            Command = "dotnet",
            Arguments = [dll, "--source", "snapshot:" + Current, "--migrations", Migrations(6), "--max-age-hours", "1000000"]
        });
        await using var client = await McpClient.CreateAsync(transport);

        var tools = await client.ListToolsAsync();
        Assert.Equal(["describe_procedure", "describe_table", "list_procedures", "list_tables"], tools.Select(t => t.Name).Order());
        Assert.All(tools, t => Assert.True(t.ProtocolTool.Annotations?.ReadOnlyHint));

        var r = await client.CallToolAsync("describe_procedure", new Dictionary<string, object?> { ["procedure"] = "dbo.usp_InvoiceNote_LatestByCustomer" });
        Assert.NotEqual(true, r.IsError);
        Assert.Contains("@customerId", ((TextContentBlock)r.Content[0]).Text);
    }
}
