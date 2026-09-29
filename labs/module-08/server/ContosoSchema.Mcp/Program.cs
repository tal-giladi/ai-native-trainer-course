// ContosoSchema.Mcp - Module 8 lab server: a read-only SQL Server schema and stored-procedure catalog over MCP.
//   serve     --source sql|snapshot:<file> [--migrations <dir>] [--max-age-hours 24] [--on-stale refuse|warn]
//             (default command; stdio transport; the MCP host starts it)
//   snapshot  --source sql --out <file>                        write the catalog to JSON (what a nightly job does)
//   info      --source ... [--migrations <dir>]                print the freshness header and exit
//   check-sql --source ... --migrations <dir> <file.cs>...     SQL in C# string literals vs the catalog (lesson 08.4)
//   check-sql ... --hook                                       same, reading a Claude Code PostToolUse payload on stdin
// --source sql reads the connection string from the CONTOSO_SCHEMA_CONN environment variable, never from arguments.
// stdout belongs to the protocol in serve mode: all logging goes to stderr.
using System.Text.Json;
using ContosoSchema.Mcp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var command = args.Length > 0 && !args[0].StartsWith("--") ? args[0] : "serve";
var rest = command == "serve" && (args.Length == 0 || args[0].StartsWith("--")) ? args : args.Skip(1).ToArray();
var opts = Options.Parse(rest);

try
{
    switch (command)
    {
        case "serve":
            return await Serve(opts);
        case "snapshot":
        {
            var c = opts.Source().Load();
            var outPath = opts.Required("out");
            File.WriteAllText(outPath, JsonSerializer.Serialize(c with { Source = "snapshot", Live = false }, SchemaCatalog.Json));
            Console.WriteLine($"wrote {outPath}: V{c.SchemaVersion:D3}, {c.Tables.Count} tables, {c.Procedures.Count} procedures, captured {c.CapturedUtc:O}");
            return 0;
        }
        case "info":
        {
            var (_, f) = opts.Service().Load();
            Console.WriteLine(f.Header);
            Console.WriteLine(f.Reason);
            return f.Status is FreshnessStatus.Fresh ? 0 : 1;
        }
        case "check-sql":
            return opts.Flag("hook") ? CheckHook(opts) : CheckFiles(opts, opts.Positional);
        default:
            Console.Error.WriteLine("usage: ContosoSchema.Mcp [serve|snapshot|info|check-sql] --source sql|snapshot:<file> [--migrations <dir>] ...");
            return 2;
    }
}
catch (UsageException e)
{
    Console.Error.WriteLine(e.Message);
    return 2;
}

static async Task<int> Serve(Options opts)
{
    var service = opts.Service();
    var builder = Host.CreateApplicationBuilder();
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);   // stdout is the protocol
    builder.Services.AddSingleton(service);
    builder.Services
        .AddMcpServer(o => o.ServerInfo = new() { Name = "contoso-schema", Version = "1.1.0" })
        .WithStdioServerTransport()
        .WithTools<SchemaTools>();
    await builder.Build().RunAsync();
    return 0;
}

static int CheckFiles(Options opts, IReadOnlyList<string> files)
{
    var (catalog, fresh) = opts.Service().Load();
    if (fresh.Status is FreshnessStatus.Stale or FreshnessStatus.Unknown)
    {
        Console.Error.WriteLine($"Cannot validate SQL against this schema source: {fresh.Reason}.");
        Console.Error.WriteLine(fresh.Header);
        return 2;
    }
    var findings = files.SelectMany(f => CheckSql.CheckFile(f, catalog)).ToList();
    foreach (var f in findings)
        Console.Error.WriteLine($"{f.File}:{f.Line}: {f.Message}");
    if (findings.Count == 0)
    {
        Console.WriteLine($"PASS check-sql: {files.Count} file(s) against V{catalog.SchemaVersion:D3} ({fresh.Source})");
        return 0;
    }
    Console.Error.WriteLine($"FAIL check-sql: {findings.Count} problem(s) against V{catalog.SchemaVersion:D3} ({fresh.Source}). " +
                            "The database does not have these objects; check db/migrations and use describe_table.");
    return 2;
}

/// <summary>PostToolUse adapter: exit 2 puts stderr in front of the agent; the edit has already happened.</summary>
static int CheckHook(Options opts)
{
    using var doc = JsonDocument.Parse(Console.In.ReadToEnd());
    var root = doc.RootElement;
    if (!root.TryGetProperty("tool_input", out var input) || !input.TryGetProperty("file_path", out var fp)) return 0;
    var path = fp.GetString() ?? "";
    var norm = path.Replace('\\', '/');
    if (!norm.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) || !norm.Contains("/src/") && !norm.StartsWith("src/")) return 0;
    if (!Path.IsPathRooted(path) && root.TryGetProperty("cwd", out var cwd)) path = Path.Combine(cwd.GetString() ?? "", path);
    return File.Exists(path) ? CheckFiles(opts, [path]) : 0;
}

sealed class UsageException(string m) : Exception(m);

sealed class Options
{
    readonly Dictionary<string, string> _named = new();
    public List<string> Positional { get; } = new();

    public static Options Parse(string[] a)
    {
        var o = new Options();
        for (var i = 0; i < a.Length; i++)
        {
            if (a[i].StartsWith("--"))
            {
                var key = a[i][2..];
                o._named[key] = i + 1 < a.Length && !a[i + 1].StartsWith("--") ? a[++i] : "true";
            }
            else o.Positional.Add(a[i]);
        }
        return o;
    }

    public bool Flag(string k) => _named.TryGetValue(k, out var v) && v == "true";
    public string? Get(string k) => _named.TryGetValue(k, out var v) ? v : null;
    public string Required(string k) => Get(k) ?? throw new UsageException($"missing --{k}");

    public ICatalogSource Source()
    {
        var s = Required("source");
        if (s == "sql")
            return new SqlSource(Environment.GetEnvironmentVariable("CONTOSO_SCHEMA_CONN")
                                 ?? throw new UsageException("--source sql needs the CONTOSO_SCHEMA_CONN environment variable"));
        if (s.StartsWith("snapshot:")) return new SnapshotSource(s["snapshot:".Length..]);
        throw new UsageException($"unknown --source '{s}' (use sql or snapshot:<file>)");
    }

    public SchemaService Service() => new(
        Source(),
        Get("migrations"),
        double.Parse(Get("max-age-hours") ?? "24", System.Globalization.CultureInfo.InvariantCulture),
        Get("on-stale") == "warn" ? StalePolicy.Warn : StalePolicy.Refuse);
}
