// AgentHooks - Module 8 lab: Claude Code hooks for Contoso Billing (lesson 08.4).
//   guard    [--rules gates/architecture.rules]   PreToolUse on Edit|Write|MultiEdit: deny edits that break the
//                                                 architecture rules or change a merged migration. Fails CLOSED.
//   audit                                         PostToolUse / PostToolUseFailure: append one redacted JSON line per
//                                                 tool call to the audit log. Fails OPEN (never blocks work).
//   report   <log.jsonl>                          summarize an audit log
//   selftest <payload-dir> [--rules <file>]       run the guard against recorded payloads named *.deny.json / *.defer.json
// Input: the hook payload on stdin. Project root: $CLAUDE_PROJECT_DIR, else the payload's cwd.
// Audit log: $AGENT_AUDIT_LOG, else <root>/.claude/audit/tool-calls.jsonl (keep it out of git).
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

var cmd = args.Length > 0 ? args[0] : "";
var opt = (string k, string d) => args.SkipWhile(a => a != "--" + k).Skip(1).FirstOrDefault() ?? d;

switch (cmd)
{
    case "guard":
    {
        string raw = Console.In.ReadToEnd();
        var d = Guard.Decide(raw, opt("rules", "gates/architecture.rules"), Environment.GetEnvironmentVariable("CLAUDE_PROJECT_DIR"));
        if (d.Deny)
        {
            Console.WriteLine(Guard.DenyJson(d.Reason));
            Audit.TryWrite(raw, "PreToolUse", decision: "deny", reason: d.Reason);
        }
        return 0;   // no output = defer to the normal permission flow; the guard never says "allow"
    }
    case "audit":
        Audit.TryWrite(Console.In.ReadToEnd(), null, null, null);
        return 0;
    case "report":
        return Audit.Report(args.Length > 1 ? args[1] : ".claude/audit/tool-calls.jsonl");
    case "selftest":
        return Guard.SelfTest(args.Length > 1 ? args[1] : "payloads", opt("rules", "gates/architecture.rules"));
    default:
        Console.Error.WriteLine("usage: AgentHooks guard [--rules <file>] | audit | report <log.jsonl> | selftest <payload-dir> [--rules <file>]");
        return 2;
}

sealed record Decision(bool Deny, string Reason);

static class Guard
{
    static readonly string[] EditTools = ["Edit", "Write", "MultiEdit"];
    static readonly Regex Migration = new(@"^db/migrations/[VU]\d{3,}__[^/]+\.sql$", RegexOptions.IgnoreCase);

    public static string DenyJson(string reason) => new JsonObject
    {
        ["hookSpecificOutput"] = new JsonObject
        {
            ["hookEventName"] = "PreToolUse",
            ["permissionDecision"] = "deny",
            ["permissionDecisionReason"] = reason
        }
    }.ToJsonString();

    /// <summary>Any exception becomes a deny: an enforcement hook that crashes must not wave the edit through.</summary>
    public static Decision Decide(string raw, string rulesPath, string? projectDir)
    {
        try
        {
            var p = JsonNode.Parse(raw)?.AsObject() ?? throw new FormatException("empty payload");
            var tool = p["tool_name"]?.GetValue<string>() ?? throw new FormatException("no tool_name");
            if (!EditTools.Contains(tool)) return new(false, "");
            var input = p["tool_input"]?.AsObject() ?? throw new FormatException("no tool_input");
            var file = input["file_path"]?.GetValue<string>() ?? throw new FormatException("no tool_input.file_path");
            var root = projectDir ?? p["cwd"]?.GetValue<string>() ?? throw new FormatException("no project root");
            var full = Path.IsPathRooted(file) ? file : Path.Combine(root, file);
            var rel = Path.GetRelativePath(root, full).Replace('\\', '/');
            if (rel.StartsWith("../")) return new(false, "");   // outside the project: not this guard's business

            if (Migration.IsMatch(rel) && File.Exists(full) && IsMerged(root, rel))
                return new(true, $"{rel} is a merged migration and must not change (V003 convention; databases already ran it). " +
                                 "Write a new V###/U### pair instead (the new-migration skill numbers it).");

            var proposed = string.Join("\n", NewText(tool, input));
            var rules = Path.IsPathRooted(rulesPath) ? rulesPath : Path.Combine(root, rulesPath);
            if (!File.Exists(rules)) throw new FileNotFoundException($"rules file not found: {rulesPath}");
            foreach (var r in Rules.Load(rules))
            {
                if (r.Kind == "forbid-text" && Rules.Glob(r.Include, rel) && (r.Exclude.Length == 0 || !Rules.Glob(r.Exclude, rel)))
                {
                    var hit = proposed.Split('\n').FirstOrDefault(l => l.Contains(r.Pattern, StringComparison.Ordinal) && !l.TrimStart().StartsWith("//"));
                    if (hit is not null)
                        return new(true, $"{rel}: the new text uses `{r.Pattern}` ({r.Reason}). Rewrite it; do not work around this hook.");
                }
                if (r.Kind == "forbid-package" && rel.EndsWith(".csproj"))
                    foreach (Match m in Regex.Matches(proposed, @"PackageReference\s+Include=""([^""]+)"""))
                        if (Regex.IsMatch(m.Groups[1].Value, r.Pattern))
                            return new(true, $"{rel}: package {m.Groups[1].Value} is not allowed ({r.Reason}).");
            }
            return new(false, "");
        }
        catch (Exception e)
        {
            return new(true, $"guard hook could not check this call ({e.GetType().Name}: {e.Message}). Failing closed; fix the hook or its configuration.");
        }
    }

    /// <summary>Every shape the edit tools use: Edit.new_string, Write.content, MultiEdit.edits[].new_string.</summary>
    static IEnumerable<string> NewText(string tool, JsonObject input)
    {
        if (input["new_string"] is JsonValue ns) yield return ns.GetValue<string>();
        if (input["content"] is JsonValue c) yield return c.GetValue<string>();
        if (input["edits"] is JsonArray edits)
            foreach (var e in edits)
                if (e?["new_string"] is JsonValue v) yield return v.GetValue<string>();
    }

    /// <summary>Tracked in git = merged enough to be immutable. Outside a git repository, existing = merged.</summary>
    static bool IsMerged(string root, string rel)
    {
        try
        {
            var psi = new ProcessStartInfo("git", $"ls-files --error-unmatch -- \"{rel}\"")
            { WorkingDirectory = root, RedirectStandardOutput = true, RedirectStandardError = true };
            using var proc = Process.Start(psi)!;
            proc.WaitForExit(5000);
            var err = proc.StandardError.ReadToEnd();
            if (err.Contains("not a git repository")) return true;
            return proc.ExitCode == 0;
        }
        catch { return true; }   // no git: be strict
    }

    public static int SelfTest(string dir, string rulesPath)
    {
        var root = Directory.CreateTempSubdirectory("agenthooks-").FullName;
        Directory.CreateDirectory(Path.Combine(root, "db", "migrations"));
        File.WriteAllText(Path.Combine(root, "db", "migrations", "V004__due_not_null.sql"), "-- fixture");
        var rules = Path.GetFullPath(rulesPath);
        int pass = 0, fail = 0;
        foreach (var f in Directory.EnumerateFiles(dir, "*.json").OrderBy(x => x))
        {
            var expectDeny = f.EndsWith(".deny.json");
            var raw = File.ReadAllText(f).Replace("{root}", root.Replace("\\", "\\\\"));
            var sw = Stopwatch.StartNew();
            var d = Decide(raw, rules, null);
            var ok = d.Deny == expectDeny;
            if (ok) pass++; else fail++;
            Console.WriteLine($"{(ok ? "ok  " : "FAIL")} {Path.GetFileName(f),-40} {(d.Deny ? "deny" : "defer"),-6} {sw.ElapsedMilliseconds,4} ms  {Trim(d.Reason, 90)}");
        }
        Console.WriteLine($"{pass} passed, {fail} failed");
        return fail == 0 ? 0 : 1;
    }

    static string Trim(string s, int n) => s.Length <= n ? s : s[..(n - 1)] + "…";
}

sealed record Rule(string Kind, string Pattern, string Include, string Exclude, string Reason);

static class Rules
{
    /// <summary>LoopGate's format (labs/module-05): kind | pattern | include-glob | exclude-glob | reason.</summary>
    public static List<Rule> Load(string path) => File.ReadAllLines(path)
        .Select(l => l.Split('#')[0].Trim())
        .Where(l => l.Length > 0)
        .Select(l => l.Split('|').Select(x => x.Trim()).ToArray())
        .Where(c => c.Length == 5)
        .Select(c => new Rule(c[0], c[1], c[2], c[3], c[4]))
        .ToList();

    public static bool Glob(string glob, string rel)
    {
        var rx = "^" + Regex.Escape(glob.Trim().TrimStart('/'))
            .Replace(@"\*\*/", "(?:.*/)?").Replace(@"\*\*", ".*").Replace(@"\*", "[^/]*").Replace(@"\?", "[^/]") + "$";
        return Regex.IsMatch(rel, rx);
    }
}

static class Audit
{
    static readonly Regex Secret = new(
        @"(ghp_[A-Za-z0-9]{20,}|github_pat_[A-Za-z0-9_]{20,}|sk-[A-Za-z0-9_-]{20,}|AKIA[0-9A-Z]{16}|xox[abp]-[A-Za-z0-9-]+|ATATT[A-Za-z0-9_=-]{20,}|-----BEGIN [A-Z ]*PRIVATE KEY-----|(?i:(password|pwd|secret|token|api[_-]?key)\s*[=:]\s*)[^;&\s""']+|(?i:bearer\s+)[A-Za-z0-9._~+/=-]{12,}|(?<=\s-P\s+)\S+)");

    public static string Redact(string s, out int n)
    {
        var count = 0;
        var r = Secret.Replace(s, m => { count++; return m.Groups[2].Success ? m.Groups[2].Value + "=[REDACTED]" : "[REDACTED]"; });
        n = count;
        return r;
    }

    /// <summary>Never throws and never blocks: a broken audit hook must not stop the work, but it says so on stderr.</summary>
    public static void TryWrite(string raw, string? eventOverride, string? decision, string? reason)
    {
        try
        {
            var p = JsonNode.Parse(raw)!.AsObject();
            var root = Environment.GetEnvironmentVariable("CLAUDE_PROJECT_DIR") ?? p["cwd"]?.GetValue<string>() ?? ".";
            var log = Environment.GetEnvironmentVariable("AGENT_AUDIT_LOG") ?? Path.Combine(root, ".claude", "audit", "tool-calls.jsonl");
            var tool = p["tool_name"]?.GetValue<string>() ?? "?";
            var input = p["tool_input"]?.ToJsonString() ?? "{}";
            var target = p["tool_input"] is JsonObject ti
                ? (ti["file_path"] ?? ti["command"] ?? ti["url"] ?? ti["pattern"])?.ToString() ?? Short(input, 200)
                : "";
            target = Redact(Short(target, 200), out var n1);
            var args = tool.StartsWith("mcp__") ? Redact(Short(input, 300), out var n2) : null;
            var ev = eventOverride ?? p["hook_event_name"]?.GetValue<string>() ?? "?";
            var isError = ev == "PostToolUseFailure" || p["tool_response"] is JsonObject tr && (tr["isError"]?.GetValue<bool>() ?? false);
            var line = new JsonObject
            {
                ["ts"] = DateTimeOffset.UtcNow.ToString("O"),
                ["session"] = p["session_id"]?.GetValue<string>(),
                ["tool_use_id"] = p["tool_use_id"]?.GetValue<string>(),
                ["event"] = ev,
                ["tool"] = tool,
                ["server"] = tool.StartsWith("mcp__") ? tool.Split("__")[1] : null,
                ["target"] = target,
                ["args"] = args,
                ["input_sha256"] = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)))[..16].ToLowerInvariant(),
                ["decision"] = decision ?? (isError ? "error" : "ran"),
                ["reason"] = reason,
                ["redactions"] = args is null ? n1 : Secret.Matches(Short(input, 300)).Count
            };
            Directory.CreateDirectory(Path.GetDirectoryName(log)!);
            File.AppendAllText(log, line.ToJsonString(Relaxed) + "\n");
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"audit hook failed ({e.GetType().Name}); this call was not logged");
        }
    }

    static string Short(string s, int n) => s.Length <= n ? s : s[..n] + "…";

    static readonly JsonSerializerOptions Relaxed = new() { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    public static int Report(string path)
    {
        if (!File.Exists(path)) { Console.Error.WriteLine($"no audit log at {path}"); return 2; }
        var rows = File.ReadLines(path).Where(l => l.Trim().Length > 0).Select(l => JsonNode.Parse(l)!.AsObject()).ToList();
        string S(JsonObject o, string k) => o[k]?.ToString() ?? "";
        Console.WriteLine($"{rows.Count} entries, {rows.Select(r => S(r, "session")).Distinct().Count()} session(s), " +
                          $"{S(rows.FirstOrDefault() ?? new(), "ts")} .. {S(rows.LastOrDefault() ?? new(), "ts")}");
        Console.WriteLine("\nby tool:");
        foreach (var g in rows.GroupBy(r => S(r, "tool")).OrderByDescending(g => g.Count()))
            Console.WriteLine($"  {g.Key,-40} {g.Count(),4}  denied {g.Count(r => S(r, "decision") == "deny"),3}  errors {g.Count(r => S(r, "decision") == "error"),3}");
        var mcp = rows.Where(r => S(r, "server").Length > 0).ToList();
        Console.WriteLine($"\nMCP calls: {mcp.Count} ({string.Join(", ", mcp.GroupBy(r => S(r, "server")).Select(g => $"{g.Key} {g.Count()}"))})");
        Console.WriteLine("\ndenied:");
        foreach (var r in rows.Where(r => S(r, "decision") == "deny"))
            Console.WriteLine($"  {S(r, "ts")[..19]}  {S(r, "tool"),-10} {S(r, "target")}\n      {S(r, "reason")}");
        var red = rows.Sum(r => (int?)r["redactions"] ?? 0);
        Console.WriteLine($"\nredactions: {red}");
        return 0;
    }
}
