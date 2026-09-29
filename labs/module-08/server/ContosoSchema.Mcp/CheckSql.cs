using System.Text.RegularExpressions;

namespace ContosoSchema.Mcp;

public sealed record SqlFinding(string File, int Line, string Message);

/// <summary>
/// A deliberately small check for lesson 08.4: finds SQL in C# string literals and reports tables,
/// columns and procedures the catalog does not know. It understands the SQL Contoso writes
/// (SELECT/INSERT/UPDATE/DELETE with dbo.X, aliases, APPLY, TOP, functions, procedure names),
/// not all of T-SQL. It is only as good as the catalog it checks against, which is why the caller
/// refuses to run it against a stale source.
/// </summary>
public static class CheckSql
{
    // "…" (regular), @"…" (verbatim), """…""" (raw); adjacent literals joined by + are one statement.
    const string Lit = "(?:\"\"\"[\\s\\S]*?\"\"\"|@\"(?:[^\"]|\"\")*\"|\"(?:\\\\.|[^\"\\\\\\n])*\")";
    static readonly Regex Statement = new($"{Lit}(?:\\s*\\+\\s*{Lit})*", RegexOptions.Compiled);
    static readonly Regex OneLit = new(Lit, RegexOptions.Compiled);
    static readonly Regex Dml = new(@"\b(SELECT|INSERT|UPDATE|DELETE|MERGE)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    static readonly Regex DboRef = new(@"\bdbo\.(\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    static readonly Regex ProcOnly = new(@"^\s*(?:EXEC(?:UTE)?\s+)?dbo\.(usp_\w+)\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    static readonly HashSet<string> Keywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "select","from","where","and","or","not","null","is","in","as","on","join","inner","left","right","full","outer",
        "cross","apply","order","by","group","having","top","distinct","insert","into","values","update","set","delete",
        "merge","using","matched","then","when","case","else","end","asc","desc","exists","between","like","union","all",
        "with","nolock","over","partition","output","inserted","deleted","offset","fetch","next","rows","row","only",
        "exec","execute","declare","begin","commit","transaction","tran","if","return","nocount","collate","escape",
        "int","bigint","smallint","tinyint","bit","decimal","numeric","money","nvarchar","varchar","nchar","char",
        "datetime","datetime2","datetimeoffset","date","time","uniqueidentifier","max","true","false","dbo","current_timestamp"
    };

    public static List<SqlFinding> CheckFile(string path, SchemaCatalog catalog)
    {
        var text = File.ReadAllText(path);
        var findings = new List<SqlFinding>();
        foreach (Match st in Statement.Matches(text))
        {
            var sql = string.Concat(OneLit.Matches(st.Value).Select(m => Unquote(m.Value)));
            var line = text[..st.Index].Count(ch => ch == '\n') + 1;
            foreach (var msg in CheckStatement(sql, catalog))
                findings.Add(new SqlFinding(path, line, msg));
        }
        return findings;
    }

    public static IEnumerable<string> CheckStatement(string sql, SchemaCatalog catalog)
    {
        var proc = ProcOnly.Match(sql);
        if (proc.Success)
        {
            if (catalog.FindProc("dbo." + proc.Groups[1].Value) is null)
                yield return $"unknown procedure dbo.{proc.Groups[1].Value}";
            yield break;
        }
        if (!Dml.IsMatch(sql) || !DboRef.IsMatch(sql)) yield break;

        var body = Regex.Replace(sql, @"'(?:[^']|'')*'", " ");   // string constants
        body = Regex.Replace(body, @"@\w+", " ");                 // parameters

        var tables = new List<TableInfo>();
        var aliases = new Dictionary<string, TableInfo?>(StringComparer.OrdinalIgnoreCase);
        foreach (Match m in Regex.Matches(body, @"\bdbo\.(\w+)(?:\s+(?:AS\s+)?(\w+))?", RegexOptions.IgnoreCase))
        {
            var name = "dbo." + m.Groups[1].Value;
            var t = catalog.FindTable(name);
            if (t is null)
            {
                if (catalog.FindProc(name) is null) yield return $"unknown table {name}";
                continue;
            }
            tables.Add(t);
            aliases[m.Groups[1].Value] = t;
            if (m.Groups[2].Success && !Keywords.Contains(m.Groups[2].Value)) aliases[m.Groups[2].Value] = t;
        }
        // Derived tables and CTEs: ") AS n", ") n", "x AS (" — names, not columns.
        foreach (Match m in Regex.Matches(body, @"\)\s*(?:AS\s+)?(\w+)|\b(\w+)\s+AS\s*\(", RegexOptions.IgnoreCase))
        {
            var a = m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value;
            if (!Keywords.Contains(a)) aliases.TryAdd(a, null);
        }
        if (tables.Count == 0) yield break;

        var outputNames = new HashSet<string>(
            Regex.Matches(body, @"\bAS\s+(\w+)", RegexOptions.IgnoreCase).Select(m => m.Groups[1].Value),
            StringComparer.OrdinalIgnoreCase);
        var scan = Regex.Replace(body, @"\bdbo\.\w+", " ");
        var reported = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (Match m in Regex.Matches(scan, @"\b([A-Za-z_]\w*)(?:\.([A-Za-z_]\w*))?(\s*\()?"))
        {
            var first = m.Groups[1].Value;
            if (m.Groups[3].Success && !m.Groups[2].Success) continue;   // function call: COUNT(, TOP (
            if (m.Groups[2].Success)
            {
                var col = m.Groups[2].Value;
                if (aliases.TryGetValue(first, out var owner))
                {
                    if (owner is not null && !HasColumn(owner, col) && reported.Add(owner.Name + "." + col))
                        yield return $"unknown column {col} (not in {owner.Name})";
                    continue;
                }
                continue;   // schema-qualified function or something this check does not model
            }
            if (Keywords.Contains(first) || aliases.ContainsKey(first) || outputNames.Contains(first)) continue;
            if (!tables.Any(t => HasColumn(t, first)) && reported.Add(first))
                yield return $"unknown column {first} (not in {string.Join(", ", tables.Select(t => t.Name).Distinct())})";
        }
    }

    static bool HasColumn(TableInfo t, string col) =>
        t.Columns.Any(c => string.Equals(c.Name, col, StringComparison.OrdinalIgnoreCase));

    static string Unquote(string lit)
    {
        if (lit.StartsWith("\"\"\"")) return lit[3..^3];
        if (lit.StartsWith("@\"")) return lit[2..^1].Replace("\"\"", "\"");
        return Regex.Unescape(lit[1..^1]);
    }
}
