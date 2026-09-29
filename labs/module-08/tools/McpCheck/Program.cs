// McpCheck - Module 8 lab tool. Read-only static checks for an MCP setup.
//   config <.mcp.json>                          inline secrets, unpinned packages and images, risky flags
//   tools  <tools.json> [--expect read-only]    classify each tool: read / write / destructive / open-ended
//   tokens <tools.json>                         rough context cost of the tool definitions (chars / 4)
// tools.json is a saved tools/list response: {"result":{"tools":[...]}}, {"tools":[...]} or [...].
// Heuristics, not proof: names and annotations come from the server, and the MCP spec says annotations
// are untrusted unless the server is. A clean report means "nothing obvious", not "safe".
using System.Text.Json;
using System.Text.RegularExpressions;

Console.OutputEncoding = System.Text.Encoding.UTF8;
if (args.Length < 2) return Usage();
try
{
    return args[0] switch
    {
        "config" => Config(args[1]),
        "tools" => Tools(args[1], args.Contains("--expect") && args.SkipWhile(a => a != "--expect").Skip(1).FirstOrDefault() == "read-only"),
        "tokens" => Tokens(args[1]),
        _ => Usage()
    };
}
catch (Exception e) when (e is IOException or JsonException)
{
    Console.Error.WriteLine($"error: {e.Message}");
    return 2;
}

static int Usage()
{
    Console.Error.WriteLine("usage: McpCheck config <.mcp.json> | tools <tools.json> [--expect read-only] | tokens <tools.json>");
    return 2;
}

static int Config(string path)
{
    using var doc = JsonDocument.Parse(File.ReadAllText(path));
    if (!doc.RootElement.TryGetProperty("mcpServers", out var servers))
    {
        Console.Error.WriteLine("no mcpServers object"); return 2;
    }
    var secret = new Regex(@"(ghp_[A-Za-z0-9]{20,}|github_pat_[A-Za-z0-9_]{20,}|xox[abp]-[A-Za-z0-9-]+|sk-[A-Za-z0-9_-]{20,}|AKIA[0-9A-Z]{16}|ATATT[A-Za-z0-9_=-]{20,}|(?i:password|pwd)\s*=\s*[^;${}\s]+|Bearer\s+(?!\$\{)[A-Za-z0-9._-]{16,})");
    int problems = 0, n = 0;
    foreach (var s in servers.EnumerateObject())
    {
        n++;
        var name = s.Name;
        var all = s.Value.GetRawText();
        void Flag(string m) { problems++; Console.WriteLine($"FAIL {name}: {m}"); }

        foreach (Match m in secret.Matches(all))
            Flag($"literal secret in config ({Mask(m.Value)}); use ${{ENV_VAR}} and keep the value out of the repository");

        var type = s.Value.TryGetProperty("type", out var t) ? t.GetString() : (s.Value.TryGetProperty("url", out _) ? "http" : "stdio");
        var command = s.Value.TryGetProperty("command", out var c) ? c.GetString() ?? "" : "";
        var argv = s.Value.TryGetProperty("args", out var a) && a.ValueKind == JsonValueKind.Array
            ? a.EnumerateArray().Select(x => x.GetString() ?? "").ToList() : new List<string>();

        if (command is "npx" or "uvx" or "pnpm" or "bunx")
        {
            var pkg = argv.FirstOrDefault(x => !x.StartsWith("-")) ?? "";
            var at = pkg.LastIndexOf('@');
            if (at <= 0 || pkg.EndsWith("@latest")) Flag($"package '{pkg}' is not pinned to a version; a new release runs without review");
        }
        if (command == "docker")
        {
            var image = argv.LastOrDefault(x => x.Contains('/') && !x.StartsWith("-")) ?? "";
            if (image.Length > 0 && !image.Contains('@') && (!image.Split('/')[^1].Contains(':') || image.EndsWith(":latest")))
                Flag($"image '{image}' is not pinned to a tag or digest");
        }
        if (all.Contains("--toolsets") && Regex.IsMatch(all, @"--toolsets[""\s,=]+all\b") || Regex.IsMatch(all, @"TOOLSETS""\s*:\s*""all"""))
            Flag("all toolsets enabled; enable only the toolsets the task needs");
        if (Regex.IsMatch(all, @"(?i)(db_owner|sysadmin|User\s*Id\s*=\s*sa\b)"))
            Flag("database identity is an owner/admin; give the server a login with only the rights its tools need");
        if (type == "http" && s.Value.TryGetProperty("url", out var u) && u.GetString() is { } url && url.StartsWith("http://") && !url.Contains("localhost") && !url.Contains("127.0.0.1"))
            Flag($"remote server over plain http: {url}");

        var readOnly = Regex.IsMatch(all, @"(?i)(--read-only|READ_ONLY""\s*:\s*""(1|true)""|X-MCP-Readonly|/readonly)");
        Console.WriteLine($"info {name}: {type}{(command.Length > 0 ? $" ({command})" : "")}{(readOnly ? ", read-only mode on" : "")}");
    }
    Console.WriteLine(problems == 0 ? $"PASS config: {n} server(s)" : $"FAIL config: {problems} problem(s) in {n} server(s)");
    return problems == 0 ? 0 : 1;
}

static string Mask(string s) => s.Length <= 8 ? "****" : s[..6] + "…" + $"({s.Length} chars)";

static List<JsonElement> LoadTools(string path)
{
    var root = JsonDocument.Parse(File.ReadAllText(path)).RootElement;
    if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("result", out var r)) root = r;
    if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("tools", out var t)) root = t;
    return root.EnumerateArray().ToList();
}

static int Tools(string path, bool expectReadOnly)
{
    var writeVerb = new Regex(@"(?i)(^|_|\b)(create|update|write|delete|remove|merge|push|add|close|transition|assign|set|edit|post|send|run|execute|exec|drop|insert|upload|dismiss|rerun|cancel|fork)(_|$|\b)");
    var destructiveVerb = new Regex(@"(?i)(^|_)(delete|remove|drop|merge|force|truncate|purge)(_|$)");
    var openParam = new Regex(@"(?i)^(sql|jql|cql|command|cmd|script|code|statement|expression|shell)$");
    // What the tool says it does, in its first word: "Deletes test invoices…" (not "Use before writing SQL…").
    var writeWords = new Regex(@"(?i)^\s*(deletes|removes|updates|writes|creates|drops|merges|executes|runs any|modifies|closes)\b");
    int write = 0, destructive = 0, open = 0;
    var tools = LoadTools(path);
    Console.WriteLine($"{"tool",-32} {"class",-12} notes");
    foreach (var tool in tools)
    {
        var name = tool.GetProperty("name").GetString() ?? "?";
        bool? ro = null, de = null;
        if (tool.TryGetProperty("annotations", out var an))
        {
            if (an.TryGetProperty("readOnlyHint", out var r)) ro = r.GetBoolean();
            if (an.TryGetProperty("destructiveHint", out var d)) de = d.GetBoolean();
        }
        var parms = tool.TryGetProperty("inputSchema", out var sc) && sc.TryGetProperty("properties", out var p)
            ? p.EnumerateObject().Select(x => x.Name).ToList() : new List<string>();
        var notes = new List<string>();
        var desc = tool.TryGetProperty("description", out var ds) ? ds.GetString() ?? "" : "";
        var contradicts = ro == true && (writeVerb.IsMatch(name) || writeWords.IsMatch(desc));
        var isWrite = ro == false || (ro != true && writeVerb.IsMatch(name)) || contradicts;
        if (contradicts) notes.Add($"annotated read-only but {(writeVerb.IsMatch(name) ? "the name" : $"the description (\"{writeWords.Match(desc).Value}\")")} says it writes; annotations are untrusted");
        var isDestructive = isWrite && (de == true || destructiveVerb.IsMatch(name));
        var openEnded = parms.Where(x => openParam.IsMatch(x)).ToList();
        if (openEnded.Count > 0) notes.Add($"open-ended input: {string.Join(", ", openEnded)}");
        if (ro is null) notes.Add("no readOnlyHint");
        var cls = isDestructive ? "DESTRUCTIVE" : isWrite ? "WRITE" : "read";
        if (isWrite) write++;
        if (isDestructive) destructive++;
        if (openEnded.Count > 0) open++;
        Console.WriteLine($"{name,-32} {cls,-12} {string.Join("; ", notes)}");
    }
    Console.WriteLine($"\n{tools.Count} tools: {tools.Count - write} read, {write} write (of which {destructive} destructive), {open} with open-ended input");
    if (expectReadOnly && write > 0)
    {
        Console.WriteLine($"FAIL expected a read-only surface, found {write} tool(s) that can write");
        return 1;
    }
    if (expectReadOnly) Console.WriteLine("PASS read-only surface");
    return 0;
}

static int Tokens(string path)
{
    var tools = LoadTools(path);
    var sizes = tools.Select(t => (Name: t.GetProperty("name").GetString() ?? "?", Tok: (int)Math.Ceiling(t.GetRawText().Length / 4.0)))
                     .OrderByDescending(x => x.Tok).ToList();
    foreach (var s in sizes) Console.WriteLine($"{s.Name,-32} ~{s.Tok,5} tokens");
    var total = sizes.Sum(s => s.Tok);
    Console.WriteLine($"\n{tools.Count} tools, ~{total} tokens of definitions (chars/4; the model's tokenizer differs).");
    Console.WriteLine($"Loaded up front, that is ~{total} tokens re-sent on every request of a session; deferred loading (tool search) pays it only for tools actually fetched.");
    return 0;
}
