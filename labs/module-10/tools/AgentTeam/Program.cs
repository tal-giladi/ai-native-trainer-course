// AgentTeam - Module 10 lab tool. Runs single- and multi-agent topologies on the Module 7 task set and
// writes run folders in EvalHarness's format, so `EvalHarness grade / compare / gate` work unchanged.
//   run       <tasks.json> --repo <dir> --out <dir> --topology single|pw|pwr|specialists|route   one pipeline per task and trial
//   parallel  <tasks.json> --repo <dir> --out <dir> --only T18,T19,T20 --merge naive|detect|rerun|serialize
//   trace     <runs-dir> [--task T18 --trial 1] [--results <csv> --config <name>]              timeline or aggregate
//   estimate  <pipeline.json> [--set reviewer.recall=0.3]                                       expected success, cost, latency
//   efficiency <results.csv> --a <config> --b <config>                                          paired cost and latency
// Agents: --agent claude (headless Claude Code, costs money) or --agent fake:<scenario.json> (scripted, offline).
// Read-only on your repository: code tasks run in copies under the output folder.
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

if (args.Length < 1) return Usage();
var o = Opts.Parse(args.Skip(1).ToArray());
try
{
    return args[0] switch
    {
        "run" => Commands.Run(o),
        "parallel" => Commands.Parallel(o),
        "trace" => Commands.Trace(o),
        "estimate" => Commands.Estimate(o),
        "efficiency" => Commands.Efficiency(o),
        _ => Usage()
    };
}
catch (UsageException e) { Console.Error.WriteLine(e.Message); return Usage(); }

static int Usage()
{
    Console.Error.WriteLine("""
        usage:
          AgentTeam run        <tasks.json> --repo <dir> --out <runs-dir> --topology single|pw|pwr|specialists|route
                               [--trials 3] [--only T18,T21] [--max-rounds 2] [--budget-usd 1.0] [--on-stall escalate|continue]
                               [--route code=pwr,qa=single] [--roles roles] [--agent claude|fake:<scenario.json>]
                               [--agent-exe claude] [--model <id>] [--seed 7] [--keep true]
          AgentTeam parallel   <tasks.json> --repo <dir> --out <dir> --only T18,T19,T20 --merge naive|detect|rerun|serialize
                               [--roles roles] [--agent claude|fake:<scenario.json>]
          AgentTeam trace      <runs-dir> [--task T18 --trial 1] [--results <results.csv> --config <name>]
          AgentTeam estimate   <pipeline.json> [--set reviewer.recall=0.3,worker.p=0.8]
          AgentTeam efficiency <results.csv> --a <config> --b <config>
        """);
    return 2;
}

sealed class UsageException(string m) : Exception(m);

sealed class Opts
{
    public List<string> Pos { get; } = new();
    public Dictionary<string, string> Named { get; } = new();
    public static Opts Parse(string[] a)
    {
        var o = new Opts();
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i].StartsWith("--"))
            {
                var key = a[i][2..];
                if (i + 1 < a.Length && !a[i + 1].StartsWith("--")) o.Named[key] = a[++i];
                else o.Named[key] = "true";
            }
            else o.Pos.Add(a[i]);
        }
        return o;
    }
    public string P(int i, string what) => i < Pos.Count ? Pos[i] : throw new UsageException($"missing argument: {what}");
    public string S(string k, string d) => Named.TryGetValue(k, out var v) ? v : d;
    public string Req(string k) => Named.TryGetValue(k, out var v) ? v : throw new UsageException($"missing option --{k}");
    public double D(string k, double d) => Named.TryGetValue(k, out var v) ? double.Parse(v, CultureInfo.InvariantCulture) : d;
    public int I(string k, int d) => Named.TryGetValue(k, out var v) ? int.Parse(v, CultureInfo.InvariantCulture) : d;
    public bool B(string k) => Named.TryGetValue(k, out var v) && v is "true" or "1" or "yes";
    public HashSet<string> List(string k) => S(k, "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToHashSet();
}

// ---------------------------------------------------------------- tasks (the EvalHarness tasks-v1 format)

sealed record TaskDef(string Id, string Kind, string Split, string[] Tags, string Prompt, string Grader, string Tests, string Filter, string[] Scope, string Target);

sealed record TaskSet(string Dir, string Version, string Sha, List<TaskDef> Tasks)
{
    public static TaskSet Load(string path)
    {
        var full = Path.GetFullPath(path);
        var text = File.ReadAllText(full);
        using var doc = JsonDocument.Parse(text);
        static string Str(JsonElement e, string k, string d = "") => e.TryGetProperty(k, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString()! : d;
        static string[] Arr(JsonElement e, string k) => e.TryGetProperty(k, out var v) && v.ValueKind == JsonValueKind.Array ? v.EnumerateArray().Select(x => x.GetString() ?? "").ToArray() : Array.Empty<string>();
        var tasks = new List<TaskDef>();
        foreach (var t in doc.RootElement.GetProperty("tasks").EnumerateArray())
        {
            var g = t.TryGetProperty("grader", out var gg) && gg.ValueKind == JsonValueKind.Object ? gg : default;
            bool hasG = g.ValueKind == JsonValueKind.Object;
            tasks.Add(new TaskDef(Str(t, "id"), Str(t, "kind", "qa"), Str(t, "split", "dev"), Arr(t, "tags"), Str(t, "prompt"),
                hasG ? Str(g, "type", "regex") : "regex", hasG ? Str(g, "tests") : "", hasG ? Str(g, "filter") : "",
                hasG ? Arr(g, "scope") : Array.Empty<string>(), hasG ? Str(g, "target", "tests/Contoso.Billing.Tests/Golden") : ""));
        }
        return new TaskSet(Path.GetDirectoryName(full)!, Str(doc.RootElement, "version", "unversioned"), Util.Sha(text), tasks);
    }
    public string Abs(string rel) => Path.GetFullPath(Path.Combine(Dir, rel));
}

// ---------------------------------------------------------------- utilities (same conventions as EvalHarness)

static class Util
{
    public static readonly string[] SkipDirs = { "bin", "obj", ".git", ".vs", "TestResults", "node_modules" };
    public static string Sha(string s) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(s.Replace("\r\n", "\n"))))[..12].ToLowerInvariant();
    public static string Rel(string root, string p) => Path.GetRelativePath(root, p).Replace('\\', '/');
    public static IEnumerable<string> Files(string root) =>
        Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories).Where(f => !Rel(root, f).Split('/').Any(SkipDirs.Contains));
    public static void CopyDir(string from, string to)
    {
        foreach (var f in Files(from))
        {
            var dest = Path.Combine(to, Rel(from, f));
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            File.Copy(f, dest, true);
        }
    }
    public static void Fresh(string dir, string from) { if (Directory.Exists(dir)) Directory.Delete(dir, true); CopyDir(from, dir); }
    public static Dictionary<string, string> Snapshot(string root) =>
        Files(root).ToDictionary(f => Rel(root, f), f => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(f))));
    public static List<string> Changed(Dictionary<string, string> before, Dictionary<string, string> after) =>
        after.Where(kv => !before.TryGetValue(kv.Key, out var h) || h != kv.Value).Select(kv => kv.Key)
            .Concat(before.Keys.Where(k => !after.ContainsKey(k))).OrderBy(x => x).ToList();
    public static List<string> LayerFiles(string repo) =>
        Files(repo).Select(f => Rel(repo, f)).Where(r =>
            Path.GetFileName(r) is "CLAUDE.md" or "AGENTS.md" or "CLAUDE.local.md" ||
            r.StartsWith(".claude/") || r.StartsWith(".cursor/") || r == ".github/copilot-instructions.md" ||
            r.StartsWith(".github/instructions/") || r.StartsWith("docs/ai/") || r == ".mcp.json").OrderBy(x => x).ToList();
    public static string LayerSha(string repo) => Sha(string.Concat(LayerFiles(repo).Select(r => r + "\n" + File.ReadAllText(Path.Combine(repo, r)))));
    public static string DirSha(string dir) => Directory.Exists(dir)
        ? Sha(string.Concat(Directory.EnumerateFiles(dir, "*.md").OrderBy(f => f).Select(f => Path.GetFileName(f) + "\n" + File.ReadAllText(f)))) : "none";

    public static (int Exit, string Out, string Err, long Ms) Exec(string exe, IEnumerable<string> args, string cwd, int timeoutSec = 900)
    {
        var psi = new ProcessStartInfo(exe) { WorkingDirectory = cwd, RedirectStandardOutput = true, RedirectStandardError = true, RedirectStandardInput = true, UseShellExecute = false };
        foreach (var a in args) psi.ArgumentList.Add(a);
        var sw = Stopwatch.StartNew();
        using var p = Process.Start(psi) ?? throw new InvalidOperationException($"cannot start {exe}");
        p.StandardInput.Close();
        var so = p.StandardOutput.ReadToEndAsync();
        var se = p.StandardError.ReadToEndAsync();
        if (!p.WaitForExit(timeoutSec * 1000)) { try { p.Kill(true); } catch { } return (-1, so.IsCompleted ? so.Result : "", "timeout", sw.ElapsedMilliseconds); }
        p.WaitForExit();
        return (p.ExitCode, so.Result, se.Result, sw.ElapsedMilliseconds);
    }

    public static List<Dictionary<string, string>> ReadCsv(string path)
    {
        var lines = File.ReadAllLines(path).Where(l => l.Trim().Length > 0).ToList();
        if (lines.Count == 0) return new();
        var head = Split(lines[0]);
        return lines.Skip(1).Select(l => { var c = Split(l); return head.Select((h, i) => (h, v: i < c.Count ? c[i] : "")).ToDictionary(x => x.h, x => x.v); }).ToList();
    }
    static List<string> Split(string line)
    {
        var r = new List<string>(); var sb = new StringBuilder(); bool q = false;
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (q) { if (c == '"' && i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; } else if (c == '"') q = false; else sb.Append(c); }
            else if (c == '"') q = true; else if (c == ',') { r.Add(sb.ToString()); sb.Clear(); } else sb.Append(c);
        }
        r.Add(sb.ToString());
        return r;
    }
    public static string Pct(double x) => double.IsNaN(x) ? "n/a" : (x * 100).ToString("F0") + "%";
    public static string Norm(string s) => Regex.Replace(s.ToLowerInvariant(), @"[^a-z0-9]+", " ").Trim();
    public static JsonSerializerOptions Indented { get; } = new() { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
    public static JsonSerializerOptions Compact { get; } = new() { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    public static double T95(int df) => df switch
    {
        <= 0 => double.NaN, 1 => 12.706, 2 => 4.303, 3 => 3.182, 4 => 2.776, 5 => 2.571, 6 => 2.447, 7 => 2.365, 8 => 2.306, 9 => 2.262,
        10 => 2.228, 11 => 2.201, 12 => 2.179, 13 => 2.160, 14 => 2.145, 15 => 2.131, 16 => 2.120, 17 => 2.110, 18 => 2.101, 19 => 2.093,
        20 => 2.086, <= 24 => 2.064, <= 29 => 2.045, <= 39 => 2.021, <= 59 => 2.000, <= 119 => 1.980, _ => 1.960
    };
    public static (double Mean, double Lo, double Hi) PairedCi(IReadOnlyList<double> d)
    {
        if (d.Count == 0) return (double.NaN, double.NaN, double.NaN);
        double m = d.Average();
        if (d.Count < 2) return (m, double.NaN, double.NaN);
        double sd = Math.Sqrt(d.Sum(x => (x - m) * (x - m)) / (d.Count - 1)), se = sd / Math.Sqrt(d.Count), t = T95(d.Count - 1);
        return (m, m - t * se, m + t * se);
    }
    public static double Quantile(List<double> xs, double q)
    {
        if (xs.Count == 0) return double.NaN;
        var s = xs.OrderBy(x => x).ToList();
        double pos = q * (s.Count - 1); int lo = (int)Math.Floor(pos), hi = (int)Math.Ceiling(pos);
        return s[lo] + (s[hi] - s[lo]) * (pos - lo);
    }
}

// ---------------------------------------------------------------- golden tests (same procedure as EvalHarness)

static class Golden
{
    public sealed record Result(bool Pass, int Total, int Failed, int Skipped, string Summary);

    public static Result Run(TaskSet ts, TaskDef t, string work)
    {
        var target = Path.Combine(work, t.Target);
        Directory.CreateDirectory(target);
        foreach (var f in Directory.EnumerateFiles(ts.Abs(t.Tests), "*.cs")) File.Copy(f, Path.Combine(target, Path.GetFileName(f)), true);
        var sln = Directory.EnumerateFiles(work, "*.sln").FirstOrDefault();
        var a = new List<string> { "test" };
        if (sln is not null) a.Add(sln);
        a.AddRange(new[] { "--filter", t.Filter, "--nologo" });
        var (exit, stdout, stderr, _) = Util.Exec("dotnet", a, work, 600);
        int total = 0, failed = 0, skipped = 0;
        foreach (Match m in Regex.Matches(stdout, @"Failed:\s*(\d+),\s*Passed:\s*(\d+),\s*Skipped:\s*(\d+),\s*Total:\s*(\d+)"))
        { failed += int.Parse(m.Groups[1].Value); skipped += int.Parse(m.Groups[3].Value); total += int.Parse(m.Groups[4].Value); }
        string summary = total == 0
            ? (Regex.IsMatch(stdout + stderr, @"error CS\d+") ? "build failed: " + Regex.Match(stdout + stderr, @"error CS\d+[^\r\n\[]*").Value.Trim() : $"no golden tests ran (exit {exit})")
            : $"golden tests: {total - failed - skipped}/{total} passed, {failed} failed, {skipped} skipped";
        return new Result(exit == 0 && total > 0 && failed == 0 && skipped == 0, total, failed, skipped, summary);
    }

    /// <summary>The repository's own tests, no filter: what a CI build of the merged branch would report.</summary>
    public static string Suite(string work)
    {
        var sln = Directory.EnumerateFiles(work, "*.sln").FirstOrDefault();
        var a = new List<string> { "test" };
        if (sln is not null) a.Add(sln);
        a.Add("--nologo");
        var (exit, stdout, stderr, _) = Util.Exec("dotnet", a, work, 600);
        var m = Regex.Match(stdout, @"(Passed|Failed)!\s*-\s*Failed:\s*(\d+),\s*Passed:\s*(\d+),\s*Skipped:\s*(\d+),\s*Total:\s*(\d+)");
        if (m.Success) return $"{(exit == 0 ? "GREEN" : "RED")}: {m.Groups[3].Value}/{m.Groups[5].Value} passed, {m.Groups[2].Value} failed";
        return Regex.IsMatch(stdout + stderr, @"error CS\d+") ? "RED: build failed: " + Regex.Match(stdout + stderr, @"error CS\d+[^\r\n\[]*").Value.Trim() : $"exit {exit}";
    }

    public static List<string> OutOfScope(IEnumerable<string> changed, string[] scope) =>
        scope.Length == 0 ? new() : changed.Where(c => !scope.Any(g => Glob(g).IsMatch(c))).ToList();
    public static Regex Glob(string g) => new("^" + Regex.Escape(g).Replace(@"\*\*", "\u0001").Replace(@"\*", "[^/]*").Replace("\u0001", ".*") + "$", RegexOptions.IgnoreCase);
}

// ---------------------------------------------------------------- agents

sealed record AgentCall(string Task, int Trial, string Role, int Round, string Prompt, string System, bool Edit, string Cwd);
sealed record AgentResult(string Text, bool IsError, double Cost, long InputTokens, long OutputTokens, long Ms);

interface IAgent { string Version { get; } AgentResult Call(AgentCall c); }

sealed class ClaudeAgent(string exe, string model) : IAgent
{
    public string Version { get; } = TryVersion(exe);

    static string TryVersion(string exe)
    {
        try { return Util.Exec(exe, new[] { "--version" }, Directory.GetCurrentDirectory(), 60).Out.Trim().Split('\n')[0].Trim(); }
        catch (Exception e) { throw new UsageException($"cannot run '{exe} --version' ({e.Message}). Install the agent CLI, pass --agent-exe, or use --agent fake:<scenario.json>."); }
    }

    // Isolation (checked 2026-09-29): --setting-sources project,local keeps ~/.claude/settings.json and personal skills out,
    // but NOT ~/.claude/CLAUDE.md. claudeMdExcludes in a --settings file keeps that out; auto memory is switched off too.
    // Personal instructions change results, and runs must be reproducible on anyone's machine.
    static readonly Lazy<string> IsolationSettings = new(() =>
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).Replace('\\', '/');
        var p = Path.Combine(Path.GetTempPath(), $"claude-isolation-{Environment.ProcessId}.json");
        File.WriteAllText(p, JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["claudeMdExcludes"] = new[] { $"{home}/.claude/CLAUDE.md", $"{home}/.claude/rules/**" },
            ["autoMemoryEnabled"] = false,
        }));
        return p;
    });

    public AgentResult Call(AgentCall c)
    {
        // Read-only roles run in dontAsk mode (reads allowed, everything that would prompt is denied);
        // editing roles may edit files in their copy and run only build and test commands.
        // --setting-sources + --settings: your ~/.claude settings, skills and CLAUDE.md stay out (see IsolationSettings).
        var a = new List<string> { "-p", c.Prompt, "--output-format", "json", "--permission-mode", c.Edit ? "acceptEdits" : "dontAsk",
            "--setting-sources", "project,local", "--settings", IsolationSettings.Value };
        if (c.Edit) { a.Add("--allowedTools"); a.Add("Bash(dotnet build *),Bash(dotnet test *)"); }
        if (c.System.Length > 0) { a.Add("--append-system-prompt"); a.Add(c.System); }
        if (model.Length > 0) { a.Add("--model"); a.Add(model); }
        var r = Util.Exec(exe, a, c.Cwd, c.Edit ? 1800 : 900);
        try
        {
            using var doc = JsonDocument.Parse(r.Out);
            var e = doc.RootElement;
            string text = e.TryGetProperty("result", out var res) && res.ValueKind == JsonValueKind.String ? res.GetString()! : "";
            bool err = e.TryGetProperty("is_error", out var ie) && ie.ValueKind == JsonValueKind.True;
            double cost = e.TryGetProperty("total_cost_usd", out var tc) && tc.ValueKind == JsonValueKind.Number ? tc.GetDouble() : 0;
            long input = 0, output = 0;
            if (e.TryGetProperty("usage", out var u) && u.ValueKind == JsonValueKind.Object)
            {
                foreach (var k in new[] { "input_tokens", "cache_read_input_tokens", "cache_creation_input_tokens" })
                    if (u.TryGetProperty(k, out var v) && v.ValueKind == JsonValueKind.Number) input += v.GetInt64();
                if (u.TryGetProperty("output_tokens", out var ot) && ot.ValueKind == JsonValueKind.Number) output = ot.GetInt64();
            }
            return new AgentResult(text, err, cost, input, output, r.Ms);
        }
        catch (JsonException) { return new AgentResult(r.Err.Length > 0 ? r.Err : r.Out, true, 0, 0, 0, r.Ms); }
    }
}

/// <summary>Scripted agent: replays steps from a scenario file and applies their edits. No model, no network.</summary>
sealed class FakeAgent : IAgent
{
    readonly List<JsonObject> _steps;
    readonly string _dir;
    public string Version { get; }

    public FakeAgent(string path)
    {
        var full = Path.GetFullPath(path);
        if (!File.Exists(full)) throw new UsageException($"scenario not found: {full}");
        _dir = Path.GetDirectoryName(full)!;
        var root = JsonNode.Parse(File.ReadAllText(full))!.AsObject();
        Version = root["agent_version"]?.GetValue<string>() ?? "fake " + Path.GetFileNameWithoutExtension(full);
        _steps = root["steps"]!.AsArray().Select(n => n!.AsObject()).ToList();
    }

    public AgentResult Call(AgentCall c)
    {
        // The step for this task and role with the highest round <= the call's round: a scripted agent repeats itself.
        var step = _steps.Where(s => S(s, "task") == c.Task && S(s, "role") == c.Role && (s["round"]?.GetValue<int>() ?? 1) <= c.Round)
            .OrderByDescending(s => s["round"]?.GetValue<int>() ?? 1).FirstOrDefault();
        if (step is null) return new AgentResult($"(fake agent: no scripted step for {c.Task} role {c.Role} round {c.Round})", true, 0, 0, 0, 1000);
        var notes = new List<string>();
        if (step["writes"] is JsonArray writes)
            foreach (var w in writes.Select(x => x!.AsObject()))
            {
                var dest = Path.Combine(c.Cwd, S(w, "path"));
                Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                var content = w["content_file"] is { } cf ? File.ReadAllText(Path.Combine(_dir, cf.GetValue<string>())) : S(w, "content");
                File.WriteAllText(dest, content.Replace("\r\n", "\n"));
            }
        if (step["edits"] is JsonArray edits)
            foreach (var ed in edits.Select(x => x!.AsObject()))
            {
                var file = Path.Combine(c.Cwd, S(ed, "path"));
                var text = File.Exists(file) ? File.ReadAllText(file).Replace("\r\n", "\n") : "";
                var old = S(ed, "old"); int at = text.IndexOf(old, StringComparison.Ordinal);
                if (at < 0) { notes.Add($"(edit failed: anchor not found in {S(ed, "path")})"); continue; }
                File.WriteAllText(file, text[..at] + S(ed, "new") + text[(at + old.Length)..]);
            }
        var body = S(step, "text") + (notes.Count > 0 ? "\n" + string.Join("\n", notes) : "");
        return new AgentResult(body, step["is_error"]?.GetValue<bool>() ?? false, step["cost"]?.GetValue<double>() ?? 0,
            step["input_tokens"]?.GetValue<long>() ?? 0, step["output_tokens"]?.GetValue<long>() ?? 0, step["ms"]?.GetValue<long>() ?? 1000);
    }

    static string S(JsonObject o, string k) => o[k]?.GetValue<string>() ?? "";
}

// ---------------------------------------------------------------- tracing (span names follow the OpenTelemetry GenAI conventions)

sealed class Tracer(string traceId)
{
    readonly List<JsonObject> _spans = new();
    int _n;
    public long Now;          // virtual clock: sequential calls add their duration, a parallel group adds its slowest
    public double Cost;
    public long Input;
    public int Calls;

    public JsonObject Span(string role, int round, long start, AgentResult r, string status, string? verdict = null, string? finding = null,
        IEnumerable<string>? files = null, string? note = null)
    {
        var s = new JsonObject
        {
            ["trace_id"] = traceId, ["span_id"] = $"s{++_n}", ["parent_id"] = "s0", ["name"] = $"invoke_agent {role}", ["role"] = role, ["round"] = round,
            ["start_ms"] = start, ["duration_ms"] = r.Ms, ["cost_usd"] = Math.Round(r.Cost, 4), ["input_tokens"] = r.InputTokens, ["output_tokens"] = r.OutputTokens,
            ["status"] = status
        };
        if (verdict is not null) s["verdict"] = verdict;
        if (finding is not null) s["finding"] = finding;
        if (files is not null) s["files"] = new JsonArray(files.Select(f => (JsonNode)f!).ToArray());
        if (note is not null) s["note"] = note;
        lock (_spans) { _spans.Add(s); Cost += r.Cost; Input += r.InputTokens; Calls++; }
        return s;
    }

    public void Write(string path, string topology, string stop, int rounds, string? note)
    {
        var root = new JsonObject
        {
            ["trace_id"] = traceId, ["span_id"] = "s0", ["parent_id"] = null, ["name"] = $"invoke_workflow {topology}", ["role"] = "orchestrator",
            ["round"] = rounds, ["start_ms"] = 0, ["duration_ms"] = Now, ["cost_usd"] = Math.Round(Cost, 4), ["input_tokens"] = Input,
            ["calls"] = Calls, ["stop_reason"] = stop, ["status"] = stop is "budget" or "error" ? "error" : "ok"
        };
        if (note is not null) root["note"] = note;
        File.WriteAllLines(path, new[] { root }.Concat(_spans.OrderBy(s => s["start_ms"]!.GetValue<long>()).ThenBy(s => s["span_id"]!.GetValue<string>()))
            .Select(s => s.ToJsonString(Util.Compact)));
    }
}

// ---------------------------------------------------------------- commands

static class Commands
{
    static IAgent MakeAgent(Opts o)
    {
        var agent = o.S("agent", "claude");
        return agent.StartsWith("fake:") ? new FakeAgent(agent[5..]) : new ClaudeAgent(o.S("agent-exe", "claude"), o.S("model", ""));
    }

    static string Role(string rolesDir, string name)
    {
        var p = Path.Combine(rolesDir, name + ".md");
        return File.Exists(p) ? File.ReadAllText(p) : throw new UsageException($"role file not found: {p}");
    }

    // ------------------------------------------------------------ run

    public static int Run(Opts o)
    {
        var ts = TaskSet.Load(o.P(0, "tasks.json"));
        var repo = Path.GetFullPath(o.Req("repo"));
        var outDir = Path.GetFullPath(o.Req("out"));
        var topology = o.Req("topology");
        if (topology is not ("single" or "pw" or "pwr" or "specialists" or "route")) throw new UsageException($"unknown topology '{topology}'");
        int trials = o.I("trials", 3), seed = o.I("seed", 7), maxRounds = o.I("max-rounds", 2);
        double budget = o.D("budget-usd", 0);
        bool escalate = o.S("on-stall", "escalate") == "escalate";
        var rolesDir = Path.GetFullPath(o.S("roles", "roles"));
        var route = o.S("route", "code=pwr,qa=single").Split(',').Select(x => x.Split('=')).ToDictionary(x => x[0].Trim(), x => x[1].Trim());
        var only = o.List("only");
        var tasks = ts.Tasks.Where(t => only.Count == 0 || only.Contains(t.Id)).ToList();
        if (tasks.Count == 0) throw new UsageException("no task matches --only");
        if (!Directory.Exists(repo)) throw new UsageException($"repo not found: {repo}");
        Directory.CreateDirectory(outDir);
        var agent = MakeAgent(o);

        var manifest = new Dictionary<string, object?>
        {
            ["started_utc"] = DateTime.UtcNow.ToString("o"), ["tasks"] = ts.Version, ["tasks_sha"] = ts.Sha, ["repo"] = repo,
            ["layer_sha"] = Util.LayerSha(repo), ["layer_files"] = Util.LayerFiles(repo), ["agent"] = o.S("agent", "claude"),
            ["agent_version"] = agent.Version, ["model"] = o.S("model", "") is { Length: > 0 } m ? m : "(agent default)",
            ["trials"] = trials, ["seed"] = seed, ["order"] = "interleaved rounds, shuffled per round",
            ["topology"] = topology, ["route"] = topology == "route" ? o.S("route", "code=pwr,qa=single") : null,
            ["roles_sha"] = Util.DirSha(rolesDir), ["max_rounds"] = maxRounds, ["budget_usd"] = budget, ["on_stall"] = escalate ? "escalate" : "continue"
        };
        var manifestPath = Path.Combine(outDir, "manifest.json");
        if (File.Exists(manifestPath))
        {
            using var old = JsonDocument.Parse(File.ReadAllText(manifestPath));
            foreach (var k in new[] { "tasks_sha", "layer_sha", "agent_version", "model", "topology", "roles_sha" })
                if (old.RootElement.TryGetProperty(k, out var v) && v.ToString() != manifest[k]?.ToString())
                    Console.WriteLine($"WARN  resuming into {outDir} but {k} changed: {v} -> {manifest[k]}. Use a new --out folder for a new configuration.");
        }
        File.WriteAllText(manifestPath, JsonSerializer.Serialize(manifest, Util.Indented));
        Console.WriteLine($"{ts.Version}: {tasks.Count} tasks x {trials} trials | topology {topology} | agent {agent.Version} | roles {manifest["roles_sha"]} | out {outDir}");

        var rng = new Random(seed);
        int done = 0, skipped = 0;
        for (int k = 1; k <= trials; k++)
            foreach (var t in tasks.OrderBy(_ => rng.Next()).ToList())
            {
                if (File.Exists(Path.Combine(outDir, $"{t.Id}.r{k}.json"))) { skipped++; continue; }
                var topo = topology == "route" ? route.GetValueOrDefault(t.Kind, "single") : topology;
                Trial(ts, t, k, topo, repo, outDir, agent, rolesDir, maxRounds, budget, escalate, o.B("keep"));
                done++;
            }
        Console.WriteLine($"done: {done} new trial(s), {skipped} already present, in {outDir}");
        return 0;
    }

    static void Trial(TaskSet ts, TaskDef t, int k, string topo, string repo, string outDir, IAgent agent, string rolesDir,
        int maxRounds, double budget, bool escalate, bool keep)
    {
        bool code = t.Kind == "code";
        var id = $"{t.Id}.r{k}";
        var work = code ? Path.Combine(outDir, "work", id) : repo;
        if (code) Util.Fresh(work, repo);
        var before = code ? Util.Snapshot(work) : new();
        var handoff = Path.Combine(outDir, "handoff", id);
        if (Directory.Exists(handoff)) Directory.Delete(handoff, true);
        Directory.CreateDirectory(handoff);
        var tr = new Tracer(id);
        int step = 0;
        void Save(string role, int round, string text) => File.WriteAllText(Path.Combine(handoff, $"{++step:D2}-{role}-r{round}.md"), text);

        AgentResult Call(string role, int round, string prompt, bool edit, string system, string cwd, out JsonObject span, string? verdict = null)
        {
            long start = tr.Now;
            var r = agent.Call(new AgentCall(t.Id, k, role, round, prompt, system, edit, cwd));
            tr.Now += r.Ms;
            Save(role, round, r.Text);
            span = tr.Span(role, round, start, r, r.IsError ? "error" : "ok", verdict);
            return r;
        }
        bool OverBudget() => budget > 0 && tr.Cost >= budget;

        string answer = "", stop = "done", note = "";
        int rounds = 1;
        bool isError = false;
        Console.WriteLine($"{id} ({t.Kind}) {topo}");

        if (topo == "single")
        {
            var r = Call("solo", 1, t.Prompt, code, Role(rolesDir, "solo"), work, out _);
            answer = r.Text; isError = r.IsError; stop = r.IsError ? "error" : "done";
        }
        else
        {
            var planPrompt = $"## Task\n{t.Prompt}\n\nWrite the plan for the worker as your instructions say.";
            var p = Call("planner", 1, planPrompt, false, Role(rolesDir, "planner"), work, out var pspan);
            var plan = p.Text;
            bool contractOk = plan.Contains("Touch:") && (!code || plan.Contains("## Interfaces"));
            if (!contractOk) pspan["note"] = "handoff contract violated: plan lacks Touch: or ## Interfaces";
            if (p.IsError) { isError = true; stop = "error"; }
            else if (topo == "specialists")
            {
                // Coder and tester work in parallel, each in its own copy, from the same plan. Like most orchestrator-worker
                // designs, each gets its part of the work (the plan), not the ticket; neither sees the other.
                var copies = new[] { "coder", "tester" }.Select(role => (role, dir: code ? Path.Combine(outDir, "work", id + "-" + role) : work)).ToList();
                if (code) foreach (var c in copies) Util.Fresh(c.dir, work);
                long start = tr.Now;
                var results = new ConcurrentDictionary<string, AgentResult>();
                System.Threading.Tasks.Parallel.ForEach(copies, c =>
                    results[c.role] = agent.Call(new AgentCall(t.Id, k, c.role, 1, $"## Your part of the work (plan from the planner)\n{plan}", Role(rolesDir, c.role), code, c.dir)));
                foreach (var c in copies)
                {
                    var r = results[c.role];
                    var changed = code ? Util.Changed(before, Util.Snapshot(c.dir)) : new List<string>();
                    Save(c.role, 1, r.Text);
                    tr.Span(c.role, 1, start, r, r.IsError ? "error" : "ok", files: changed);
                    if (code) foreach (var f in changed) { var dest = Path.Combine(work, f); Directory.CreateDirectory(Path.GetDirectoryName(dest)!); File.Copy(Path.Combine(c.dir, f), dest, true); }
                    isError |= r.IsError;
                }
                tr.Now = start + results.Values.Max(r => r.Ms);
                var overlap = code ? copies.SelectMany(c => Util.Changed(before, Util.Snapshot(c.dir))).GroupBy(f => f).Where(g => g.Count() > 1).Select(g => g.Key).ToList() : new();
                if (overlap.Count > 0) note = "both specialists changed: " + string.Join(", ", overlap) + " (last copy wins)";
                if (code && !keep) foreach (var c in copies) try { Directory.Delete(c.dir, true); } catch { }
                answer = results["coder"].Text; stop = isError ? "error" : "done";
            }
            else
            {
                string findings = "", prevNorm = "";
                bool lastKept = false;
                for (int round = 1; ; round++)
                {
                    rounds = round;
                    var wp = new StringBuilder($"## Task\n{t.Prompt}\n\n## Plan (from the planner)\n{plan}\n");
                    if (round > 1) wp.Append($"\n## Reviewer findings (round {round - 1})\n{findings}\n");
                    if (round > 1 && !code) wp.Append($"\n## Your previous answer\n{answer}\n");
                    var w = Call("worker", round, wp.ToString(), code, Role(rolesDir, "worker"), work, out var wspan);
                    if (code) wspan["files"] = new JsonArray(Util.Changed(before, Util.Snapshot(work)).Select(f => (JsonNode)f!).ToArray());
                    answer = w.Text;
                    if (w.IsError) { isError = true; stop = "error"; break; }
                    if (OverBudget()) { stop = "budget"; isError = true; break; }
                    if (topo == "pw") { stop = "done"; break; }

                    var rp = new StringBuilder($"## Task\n{t.Prompt}\n\n## Plan\n{plan}\n\n");
                    rp.Append(code ? ReviewDiff(repo, work, before) : $"## Worker's answer\n<answer>\n{answer}\n</answer>\n");
                    var rv = Call("reviewer", round, rp.ToString(), false, Role(rolesDir, "reviewer"), work, out var rspan);
                    var vm = Regex.Matches(rv.Text, @"VERDICT:\s*(APPROVE|REQUEST_CHANGES)", RegexOptions.IgnoreCase).LastOrDefault();
                    findings = Findings(rv.Text);
                    if (vm is null) { rspan["verdict"] = "none"; stop = "reviewer_contract"; note = "reviewer output had no VERDICT line; shipped as is"; break; }
                    var verdict = vm.Groups[1].Value.ToUpperInvariant();
                    rspan["verdict"] = verdict;
                    if (findings.Length > 0) rspan["finding"] = findings.Length > 200 ? findings[..200] : findings;
                    if (verdict == "APPROVE") { stop = "approved"; break; }
                    var norm = Util.Norm(findings);
                    if (escalate && lastKept && norm == prevNorm)
                    {
                        // Oscillation: the planner rejected this exact finding last round and the reviewer raised it again.
                        stop = "stalled"; note = $"escalated to a human: the planner kept its plan and the reviewer repeated the same finding ({(findings.Length > 120 ? findings[..120] : findings).Replace('\n', ' ')})";
                        break;
                    }
                    prevNorm = norm;
                    if (OverBudget()) { stop = "budget"; isError = true; note = $"budget ${tr.Cost:F2} reached after {round} round(s) without approval"; break; }
                    if (maxRounds > 0 && round >= maxRounds) { stop = "max_rounds"; note = $"not approved after {round} round(s); last output shipped for a human to decide"; break; }
                    if (round >= 50) { stop = "max_rounds"; note = "safety cap of 50 rounds"; break; }

                    var rev = Call("planner", round + 1, $"## Task\n{t.Prompt}\n\n## Your previous plan\n{plan}\n\n## Reviewer findings\n{findings}\n\nRevise the plan, or reply PLAN: KEEP with the criterion that outranks the findings.",
                        false, Role(rolesDir, "planner"), work, out var rvspan);
                    bool kept = rev.Text.TrimStart().StartsWith("PLAN: KEEP", StringComparison.OrdinalIgnoreCase);
                    rvspan["verdict"] = kept ? "KEEP" : "REVISED";
                    if (!kept) plan = rev.Text;
                    lastKept = kept;
                    if (OverBudget()) { stop = "budget"; isError = true; note = $"budget ${tr.Cost:F2} reached after {round} round(s) without approval"; break; }
                }
            }
        }

        tr.Write(Path.Combine(outDir, $"{id}.trace.jsonl"), topo, stop, rounds, note.Length > 0 ? note : null);
        var result = new JsonObject
        {
            ["type"] = "result", ["is_error"] = isError, ["result"] = isError && stop == "budget" ? $"(stopped: {note})\n{answer}" : answer,
            ["total_cost_usd"] = Math.Round(tr.Cost, 4), ["usage"] = new JsonObject { ["input_tokens"] = tr.Input },
            ["topology"] = topo, ["stop_reason"] = stop, ["rounds"] = rounds, ["calls"] = tr.Calls
        };
        File.WriteAllText(Path.Combine(outDir, $"{id}.json"), result.ToJsonString(Util.Indented));
        var check = new JsonObject { ["duration_ms"] = tr.Now, ["exit"] = isError ? 1 : 0, ["topology"] = topo, ["stop_reason"] = stop, ["rounds"] = rounds, ["calls"] = tr.Calls };
        if (note.Length > 0) check["note"] = note;
        if (code)
        {
            var changed = Util.Changed(before, Util.Snapshot(work));
            var g = Golden.Run(ts, t, work);
            check["changed"] = new JsonArray(changed.Select(f => (JsonNode)f!).ToArray());
            check["out_of_scope"] = new JsonArray(Golden.OutOfScope(changed, t.Scope).Select(f => (JsonNode)f!).ToArray());
            check["tests"] = new JsonObject { ["pass"] = g.Pass, ["total"] = g.Total, ["failed"] = g.Failed, ["skipped"] = g.Skipped, ["summary"] = g.Summary };
            Console.WriteLine($"  {g.Summary}; changed {changed.Count} file(s)");
            if (!keep) try { Directory.Delete(work, true); } catch { }
        }
        File.WriteAllText(Path.Combine(outDir, $"{id}.check.json"), check.ToJsonString(Util.Indented));
        Console.WriteLine($"  stop {stop} after {rounds} round(s), {tr.Calls} call(s), ${tr.Cost:F4}, {tr.Now / 1000.0:F0} s{(note.Length > 0 ? " | " + note : "")}");
    }

    static string Findings(string review)
    {
        var lines = review.Replace("\r\n", "\n").Split('\n');
        return string.Join("\n", lines.Select(l => l.Trim()).Where(l => l.StartsWith("- ") || l.StartsWith("No findings", StringComparison.OrdinalIgnoreCase)));
    }

    static string ReviewDiff(string repo, string work, Dictionary<string, string> before)
    {
        var sb = new StringBuilder("## Changed files (before and after)\n");
        var changed = Util.Changed(before, Util.Snapshot(work));
        if (changed.Count == 0) sb.Append("(no files changed)\n");
        foreach (var f in changed.Take(8))
        {
            string Read(string root) { var p = Path.Combine(root, f); if (!File.Exists(p)) return "(absent)"; var l = File.ReadAllLines(p); return string.Join("\n", l.Take(300)) + (l.Length > 300 ? "\n(truncated)" : ""); }
            sb.Append($"\n### {f}\nBEFORE:\n```\n{Read(repo)}\n```\nAFTER:\n```\n{Read(work)}\n```\n");
        }
        return sb.ToString();
    }

    // ------------------------------------------------------------ parallel workers on one repository

    public static int Parallel(Opts o)
    {
        var ts = TaskSet.Load(o.P(0, "tasks.json"));
        var repo = Path.GetFullPath(o.Req("repo"));
        var outDir = Path.GetFullPath(o.Req("out"));
        var merge = o.Req("merge");
        if (merge is not ("naive" or "detect" or "rerun" or "serialize")) throw new UsageException($"unknown merge policy '{merge}'");
        var only = o.List("only");
        var tasks = ts.Tasks.Where(t => only.Contains(t.Id) && t.Kind == "code").ToList();
        if (tasks.Count < 2) throw new UsageException("--only must name at least two code tasks");
        var rolesDir = Path.GetFullPath(o.S("roles", "roles"));
        var agent = MakeAgent(o);
        Directory.CreateDirectory(outDir);
        var merged = Path.Combine(outDir, "merged");
        Util.Fresh(merged, repo);
        var baseSnap = Util.Snapshot(merged);
        var tr = new Tracer("parallel-" + merge);
        var owner = new Dictionary<string, string>();          // file -> task whose version is in merged/
        var status = tasks.ToDictionary(t => t.Id, _ => "merged");
        Console.WriteLine($"{tasks.Count} workers on one repository | merge policy: {merge} | agent {agent.Version}");

        (TaskDef T, AgentResult R, List<string> Changed, string Dir) Work(TaskDef t, int round, string from, string extra)
        {
            var dir = Path.Combine(outDir, "work", $"{t.Id}-r{round}");
            Util.Fresh(dir, from);
            var snap = Util.Snapshot(dir);
            var r = agent.Call(new AgentCall(t.Id, 1, "worker", round, $"## Task\n{t.Prompt}\n{extra}", Role(rolesDir, "worker"), true, dir));
            return (t, r, Util.Changed(snap, Util.Snapshot(dir)), dir);
        }
        void Accept((TaskDef T, AgentResult R, List<string> Changed, string Dir) w)
        {
            foreach (var f in w.Changed) { var dest = Path.Combine(merged, f); Directory.CreateDirectory(Path.GetDirectoryName(dest)!); File.Copy(Path.Combine(w.Dir, f), dest, true); owner[f] = w.T.Id; }
        }
        static string RebasePrompt(TaskDef t) => "\n## Note from the orchestrator\nAnother worker's change to the same files was merged first. Apply your change on top of the current files; keep theirs.\n";

        if (merge == "serialize")
        {
            // Leases from declared scope: tasks whose scopes can touch the same files run one after another, the rest in parallel.
            var waves = new List<List<TaskDef>>();
            foreach (var t in tasks)
            {
                var wave = waves.FirstOrDefault(w => w.All(x => !ScopesOverlap(x, t)));
                if (wave is null) waves.Add(new List<TaskDef> { t }); else wave.Add(t);
            }
            Console.WriteLine($"leases: {waves.Count} wave(s): " + string.Join(" -> ", waves.Select(w => "[" + string.Join(" ", w.Select(t => t.Id)) + "]")));
            foreach (var wave in waves)
            {
                long start = tr.Now;
                var rs = new ConcurrentBag<(TaskDef T, AgentResult R, List<string> Changed, string Dir)>();
                System.Threading.Tasks.Parallel.ForEach(wave, t => rs.Add(Work(t, 1, merged, "")));
                foreach (var w in rs.OrderBy(x => x.R.Ms)) { tr.Span("worker " + w.T.Id, 1, start, w.R, w.R.IsError ? "error" : "ok", files: w.Changed); Accept(w); }
                tr.Now = start + rs.Max(x => x.R.Ms);
            }
        }
        else
        {
            long start = tr.Now;
            var rs = new ConcurrentBag<(TaskDef T, AgentResult R, List<string> Changed, string Dir)>();
            System.Threading.Tasks.Parallel.ForEach(tasks, t => rs.Add(Work(t, 1, merged, "")));
            tr.Now = start + rs.Max(x => x.R.Ms);
            var rerun = new List<TaskDef>();
            foreach (var w in rs.OrderBy(x => x.R.Ms))   // merge in completion order
            {
                var span = tr.Span("worker " + w.T.Id, 1, start, w.R, w.R.IsError ? "error" : "ok", files: w.Changed);
                var clash = w.Changed.Where(owner.ContainsKey).ToList();
                if (clash.Count == 0 || merge == "naive")
                {
                    if (clash.Count > 0) span["note"] = "overwrote " + string.Join(", ", clash.Select(f => $"{f} (from {owner[f]})"));
                    Accept(w);
                    Console.WriteLine($"  {w.T.Id} finished at {(start + w.R.Ms) / 1000.0:F0} s: merged {w.Changed.Count} file(s)");
                }
                else
                {
                    span["note"] = "CONFLICT on " + string.Join(", ", clash.Select(f => $"{f} (already changed by {owner[f]})"));
                    Console.WriteLine($"  {w.T.Id} finished at {(start + w.R.Ms) / 1000.0:F0} s: CONFLICT on {string.Join(", ", clash)} (already changed by {string.Join(", ", clash.Select(f => owner[f]).Distinct())})");
                    if (merge == "detect") status[w.T.Id] = "conflict (not merged)";
                    else rerun.Add(w.T);
                }
            }
            foreach (var t in rerun)   // rerun: redo the losing worker on top of what is merged, one at a time
            {
                long s2 = tr.Now;
                var w = Work(t, 2, merged, RebasePrompt(t));
                tr.Now += w.R.Ms;
                var span = tr.Span("worker " + t.Id, 2, s2, w.R, w.R.IsError ? "error" : "ok", files: w.Changed);
                span["note"] = "rerun on top of the merged state";
                Accept(w);
                status[t.Id] = "merged after rerun";
                Console.WriteLine($"  {t.Id} rerun on top of merged state: merged {w.Changed.Count} file(s)");
            }
        }

        Console.WriteLine($"\n| task | merge | golden tests on the merged result |");
        Console.WriteLine("|---|---|---|");
        int pass = 0;
        foreach (var t in tasks)
        {
            var chk = Path.Combine(outDir, "check", t.Id);   // golden tests run on a copy, so merged/ stays what the team produced
            Util.Fresh(chk, merged);
            var g = Golden.Run(ts, t, chk);
            if (g.Pass) pass++;
            Console.WriteLine($"| {t.Id} | {status[t.Id]} | {(g.Pass ? "PASS" : "FAIL")}: {g.Summary} |");
        }
        var suite = Golden.Suite(merged);
        tr.Write(Path.Combine(outDir, $"parallel-{merge}.trace.jsonl"), "parallel-" + merge, "done", 1, null);
        Console.WriteLine($"\nthe merged branch's own test suite (what CI would run): {suite}");
        Console.WriteLine($"{pass}/{tasks.Count} tasks correct on the merged result (golden tests) | wall-clock {tr.Now / 1000.0:F0} s | agent calls {tr.Calls} | cost ${tr.Cost:F2}");
        Console.WriteLine($"trace: {Path.Combine(outDir, $"parallel-{merge}.trace.jsonl")}");
        return pass == tasks.Count ? 0 : 1;
    }

    static bool ScopesOverlap(TaskDef a, TaskDef b) =>
        a.Scope.Any(x => b.Scope.Any(y => x == y || Golden.Glob(x).IsMatch(y.Replace("**", "x").Replace("*", "x")) || Golden.Glob(y).IsMatch(x.Replace("**", "x").Replace("*", "x"))));

    // ------------------------------------------------------------ trace

    public static int Trace(Opts o)
    {
        var dir = Path.GetFullPath(o.P(0, "runs-dir"));
        var files = Directory.EnumerateFiles(dir, "*.trace.jsonl").OrderBy(f => f).ToList();
        if (files.Count == 0) { Console.Error.WriteLine($"no *.trace.jsonl in {dir}"); return 2; }
        var traces = files.Select(f => File.ReadAllLines(f).Where(l => l.Trim().Length > 0).Select(l => JsonNode.Parse(l)!.AsObject()).ToList()).ToList();
        static string S(JsonObject s, string k) => s[k]?.ToString() ?? "";
        static double Dn(JsonObject s, string k) => s[k] is JsonValue v && v.TryGetValue<double>(out var d) ? d : 0;

        if (o.Named.TryGetValue("task", out var task))
        {
            var id = $"{task}.r{o.I("trial", 1)}";
            var t = traces.FirstOrDefault(x => S(x[0], "trace_id") == id) ?? throw new UsageException($"no trace {id} in {dir}");
            var root = t[0];
            Console.WriteLine($"{id}  {S(root, "name")}  stop: {S(root, "stop_reason")}  rounds: {S(root, "round")}  calls: {S(root, "calls")}  cost: ${Dn(root, "cost_usd"):F4}  wall: {Dn(root, "duration_ms") / 1000:F0} s");
            if (root["note"] is not null) Console.WriteLine($"note: {S(root, "note")}");
            Console.WriteLine($"\n| start s | dur s | span | round | cost $ | in tokens | verdict | detail |");
            Console.WriteLine("|---|---|---|---|---|---|---|---|");
            double busy = 0;
            foreach (var s in t.Skip(1))
            {
                busy += Dn(s, "duration_ms");
                var detail = string.Join(" ", new[] { s["files"] is JsonArray fa && fa.Count > 0 ? "files: " + string.Join(", ", fa.Select(x => x!.ToString())) : "", S(s, "finding").Replace('\n', ' '), S(s, "note") }.Where(x => x.Length > 0));
                Console.WriteLine($"| {Dn(s, "start_ms") / 1000:F0} | {Dn(s, "duration_ms") / 1000:F0} | {S(s, "name")} | {S(s, "round")} | {Dn(s, "cost_usd"):F4} | {Dn(s, "input_tokens"):N0} | {S(s, "verdict")} | {(detail.Length > 110 ? detail[..110] + "..." : detail)} |");
            }
            double wall = Dn(root, "duration_ms");
            Console.WriteLine($"\nagent time {busy / 1000:F0} s over {wall / 1000:F0} s wall-clock (parallelism {(wall > 0 ? busy / wall : 0):F2}x)");
            var chk = Path.Combine(dir, $"{id}.check.json");
            if (File.Exists(chk) && JsonNode.Parse(File.ReadAllText(chk))?["tests"] is JsonObject tests) Console.WriteLine($"outcome: {tests["summary"]}");
            return 0;
        }

        // Aggregate over every trace in the folder.
        var passBy = new Dictionary<string, bool>();
        if (o.Named.TryGetValue("results", out var rpath))
        {
            var cfg = o.Req("config");
            foreach (var r in Util.ReadCsv(rpath).Where(r => r["config"] == cfg)) passBy[$"{r["task"]}.r{r["trial"]}"] = r["pass"] == "1";
            if (passBy.Count == 0) throw new UsageException($"no rows for config '{cfg}' in {rpath}");
        }
        var spans = traces.SelectMany(t => t.Skip(1)).ToList();
        var roots = traces.Select(t => t[0]).ToList();
        double totalCost = spans.Sum(s => Dn(s, "cost_usd")), totalMs = spans.Sum(s => Dn(s, "duration_ms"));
        Console.WriteLine($"{traces.Count} traces, {spans.Count} agent calls, ${totalCost:F2}, {totalMs / 3600000:F1} agent-hours\n");
        Console.WriteLine("| role | calls | calls/trace | cost share | time share | mean input tokens |");
        Console.WriteLine("|---|---|---|---|---|---|");
        foreach (var g in spans.GroupBy(s => Regex.Replace(S(s, "role"), @" T\d+$", "")).OrderByDescending(g => g.Sum(s => Dn(s, "cost_usd"))))
            Console.WriteLine($"| {g.Key} | {g.Count()} | {(double)g.Count() / traces.Count:F2} | {Util.Pct(g.Sum(s => Dn(s, "cost_usd")) / totalCost)} | {Util.Pct(g.Sum(s => Dn(s, "duration_ms")) / totalMs)} | {g.Average(s => Dn(s, "input_tokens")):N0} |");

        Console.WriteLine("\nrounds per trace: " + string.Join(", ", roots.GroupBy(r => (int)Dn(r, "round")).OrderBy(g => g.Key).Select(g => $"{g.Key}: {g.Count()}")));
        Console.WriteLine("stop reasons:     " + string.Join(", ", roots.GroupBy(r => S(r, "stop_reason")).OrderByDescending(g => g.Count()).Select(g => $"{g.Key} {g.Count()}")));
        var rv = spans.Where(s => S(s, "role") == "reviewer").ToList();
        if (rv.Count > 0)
            Console.WriteLine($"reviewer verdicts: APPROVE {rv.Count(s => S(s, "verdict") == "APPROVE")}, REQUEST_CHANGES {rv.Count(s => S(s, "verdict") == "REQUEST_CHANGES")}, none {rv.Count(s => S(s, "verdict") is "none" or "")}");
        var costs = roots.Select(r => Dn(r, "cost_usd")).ToList(); var walls = roots.Select(r => Dn(r, "duration_ms") / 1000).ToList();
        Console.WriteLine($"per trace: cost median ${Util.Quantile(costs, 0.5):F3}, p90 ${Util.Quantile(costs, 0.9):F3}, max ${costs.Max():F3} | wall median {Util.Quantile(walls, 0.5):F0} s, p90 {Util.Quantile(walls, 0.9):F0} s");

        if (passBy.Count > 0)
        {
            // Where did failures originate? Classify each failed trace by what the pipeline did.
            var rows = new List<(string Kind, bool Pass)>();
            foreach (var t in traces)
            {
                var id = S(t[0], "trace_id");
                if (!passBy.TryGetValue(id, out var pass)) continue;
                var revs = t.Where(s => S(s, "role") == "reviewer").OrderBy(s => Dn(s, "round")).ToList();
                bool contract = t.Any(s => S(s, "note").Contains("contract"));
                string kind = S(t[0], "stop_reason") switch
                {
                    "approved" when revs.Count == 1 => "approved in round 1",
                    "approved" => "approved after rework",
                    "max_rounds" => "stopped at max rounds",
                    "stalled" => "stalled, escalated",
                    "budget" => "budget stop",
                    "error" => "agent error",
                    var x => x
                };
                if (contract) kind += " (+ plan contract violated)";
                rows.Add((kind, pass));
            }
            Console.WriteLine("\n| pipeline outcome | traces | passed | failed |");
            Console.WriteLine("|---|---|---|---|");
            foreach (var g in rows.GroupBy(r => r.Kind).OrderByDescending(g => g.Count()))
                Console.WriteLine($"| {g.Key} | {g.Count()} | {g.Count(r => r.Pass)} | {g.Count(r => !r.Pass)} |");
            int escaped = rows.Count(r => r.Kind.StartsWith("approved") && !r.Pass);
            int reworkWins = rows.Count(r => r.Kind.StartsWith("approved after") && r.Pass);
            Console.WriteLine($"\nreviewer approved a failing result: {escaped} | rework after a finding ended in a pass: {reworkWins}");
        }
        return 0;
    }

    // ------------------------------------------------------------ estimate

    public static int Estimate(Opts o)
    {
        var path = o.P(0, "pipeline.json");
        var root = JsonNode.Parse(File.ReadAllText(path))!.AsObject();
        foreach (var kv in o.S("set", "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var (key, val) = (kv.Split('=')[0], double.Parse(kv.Split('=')[1], CultureInfo.InvariantCulture));
            var parts = key.Split('.');
            if (parts.Length == 1) root[parts[0]] = val; else root[parts[0]]![parts[1]] = val;
        }
        double G(string a, string b, double d = 0) => root[a]?[b] is JsonValue v && v.TryGetValue<double>(out var x) ? x : d;
        double pp = G("planner", "p", 1), pw = G("worker", "p"), pbad = G("worker", "p_after_bad_plan", 0), pfix = G("worker", "p_fix", pw), dmg = G("worker", "damage", 0);
        double r = G("reviewer", "recall"), f = G("reviewer", "false_alarm");
        double cp = G("planner", "cost_usd"), cw = G("worker", "cost_usd"), cr = G("reviewer", "cost_usd");
        double tp = G("planner", "seconds"), tw = G("worker", "seconds"), trv = G("reviewer", "seconds");
        int R = root["max_rounds"] is JsonValue mv ? (int)mv.GetValue<double>() : 2;
        bool replan = root["replan"]?.GetValue<bool>() ?? true;
        double ps = G("single", "p"), cs = G("single", "cost_usd"), tsn = G("single", "seconds");

        Console.WriteLine($"{root["label"]?.ToString() ?? path}");
        if (root["note"] is not null) Console.WriteLine($"({root["note"]})");
        Console.WriteLine($"inputs: single p={ps}; planner p={pp}; worker p={pw}, after bad plan {pbad}, fix {pfix}, damage {dmg}; reviewer recall={r}, false alarm={f}; max rounds {R}, replan {replan}\n");

        // Chain planner -> worker: success needs a sound plan AND a correct implementation (independence assumed).
        double q1 = pp * pw + (1 - pp) * pbad;
        double cPw = cp + cw, tPw = tp + tw;

        // Planner -> worker -> reviewer loop, exact over rounds: track probability mass of correct (c) and wrong (w) outputs.
        double c = q1, w = 1 - q1, success = 0, escapedWrong = 0, atMax = 0, cost = cp + cw, time = tp + tw, rounds = 1;
        for (int k = 1; ; k++)
        {
            double alive = c + w;
            cost += alive * cr; time += alive * trv;
            success += c * (1 - f); escapedWrong += w * (1 - r);
            double fc = c * f, fwr = w * r;
            if (R > 0 && k >= R || k >= 50) { success += fc; atMax += fc + fwr; break; }
            double again = fc + fwr;
            if (again < 1e-12) break;
            cost += again * ((replan ? cp : 0) + cw); time += again * ((replan ? tp : 0) + tw); rounds += again;
            c = fc * (1 - dmg) + fwr * pfix; w = again - c;
        }
        Console.WriteLine("| design | P(success) | expected cost | expected latency | cost per success |");
        Console.WriteLine("|---|---|---|---|---|");
        if (ps > 0) Console.WriteLine($"| single agent | {ps:F3} | ${cs:F3} | {tsn:F0} s | ${cs / ps:F3} |");
        Console.WriteLine($"| planner -> worker | {q1:F3} | ${cPw:F3} | {tPw:F0} s | ${cPw / q1:F3} |");
        Console.WriteLine($"| planner -> worker -> reviewer (max {R}) | {success:F3} | ${cost:F3} | {time:F0} s | ${cost / success:F3} |");
        Console.WriteLine($"\nreview loop: expected rounds {rounds:F2}; P(stopped at max rounds) {atMax:F3}; P(reviewer approved a wrong result) {escapedWrong:F3}");
        Console.WriteLine($"reviewer's contribution over planner -> worker: {(success - q1) * 100:+0.0;-0.0} pts for {(cost - cPw) / cPw * 100:+0;-0}% cost");
        if (ps > 0) Console.WriteLine($"pipeline vs single agent: {(success - ps) * 100:+0.0;-0.0} pts, cost x{cost / cs:F2}, latency x{time / tsn:F2}");
        if (root["chain"] is JsonArray chain && chain.Count > 0)
        {
            var ps2 = chain.Select(x => x!.GetValue<double>()).ToList();
            Console.WriteLine($"\nchain of {ps2.Count} stages [{string.Join(", ", ps2)}]: all succeed with probability {ps2.Aggregate(1.0, (a, b) => a * b):F3} (independence assumed)");
        }
        Console.WriteLine("\nThe model assumes the reviewer's misses are independent of the worker's mistakes. A reviewer on the same model with the same context shares blind spots: rerun with a lower recall (--set reviewer.recall=0.3).");
        return 0;
    }

    // ------------------------------------------------------------ efficiency

    public static int Efficiency(Opts o)
    {
        var rows = Util.ReadCsv(o.P(0, "results.csv"));
        string a = o.Req("a"), b = o.Req("b");
        static double Num(Dictionary<string, string> r, string k) => double.TryParse(r.GetValueOrDefault(k, ""), NumberStyles.Float, CultureInfo.InvariantCulture, out var x) ? x : 0;
        var ra = rows.Where(r => r["config"] == a).ToList(); var rb = rows.Where(r => r["config"] == b).ToList();
        if (ra.Count == 0 || rb.Count == 0) throw new UsageException($"no rows for config '{(ra.Count == 0 ? a : b)}'");
        var common = ra.Select(r => r["task"]).Intersect(rb.Select(r => r["task"])).OrderBy(x => x).ToList();
        var dropped = ra.Select(r => r["task"]).Union(rb.Select(r => r["task"])).Except(common).Distinct().ToList();
        if (dropped.Count > 0) Console.WriteLine($"WARN  tasks in only one arm were left out: {string.Join(", ", dropped)}");
        foreach (var k in new[] { "agent_version", "model", "tasks_sha" })
        {
            var va = ra.Select(r => r.GetValueOrDefault(k, "")).Distinct().ToList(); var vb = rb.Select(r => r.GetValueOrDefault(k, "")).Distinct().ToList();
            if (va.Count != 1 || vb.Count != 1 || va[0] != vb[0]) Console.WriteLine($"WARN  {k} differs between or within arms ({a}: {string.Join("|", va)}; {b}: {string.Join("|", vb)})");
        }
        double Mean(List<Dictionary<string, string>> rs, string t, Func<Dictionary<string, string>, double> f) => rs.Where(r => r["task"] == t).Average(f);
        Func<Dictionary<string, string>, double> pass = r => r["pass"] == "1" ? 1 : 0, cost = r => Num(r, "cost_usd"), secs = r => Num(r, "duration_ms") / 1000;

        Console.WriteLine($"\n{common.Count} paired tasks | {a}: {ra.Count(r => common.Contains(r["task"]))} trials | {b}: {rb.Count(r => common.Contains(r["task"]))} trials\n");
        Console.WriteLine($"| metric | {a} | {b} | paired diff ({b} - {a}) | 95% CI | ratio |");
        Console.WriteLine("|---|---|---|---|---|---|");
        foreach (var (name, f, fmt) in new (string, Func<Dictionary<string, string>, double>, Func<double, string>)[]
            { ("pass rate", pass, x => (x * 100).ToString("F1") + "%"), ("cost per trial", cost, x => "$" + x.ToString("F4")), ("latency per trial", secs, x => x.ToString("F0") + " s") })
        {
            var ma = common.Select(t => Mean(ra, t, f)).ToList(); var mb = common.Select(t => Mean(rb, t, f)).ToList();
            var (m, lo, hi) = Util.PairedCi(ma.Zip(mb, (x, y) => y - x).ToList());
            double A = ma.Average(), B = mb.Average();
            string D(double x) => name == "pass rate" ? (x >= 0 ? "+" : "") + (x * 100).ToString("F1") + " pts" : (x >= 0 ? "+" : "-") + fmt(Math.Abs(x));
            Console.WriteLine($"| {name} | {fmt(A)} | {fmt(B)} | {D(m)} | [{D(lo)}, {D(hi)}] | {(name == "pass rate" ? "" : (A > 0 ? (B / A).ToString("F2") + "x" : "n/a"))} |");
        }
        foreach (var (cfg, rs) in new[] { (a, ra), (b, rb) })
        {
            var inCommon = rs.Where(r => common.Contains(r["task"])).ToList();
            int ok = inCommon.Count(r => r["pass"] == "1");
            var lat = inCommon.Select(secs).ToList();
            Console.WriteLine($"{cfg}: cost per passing trial {(ok == 0 ? "n/a" : "$" + (inCommon.Sum(cost) / ok).ToString("F4"))} | latency median {Util.Quantile(lat, 0.5):F0} s, p90 {Util.Quantile(lat, 0.9):F0} s");
        }
        Console.WriteLine("\nPass-rate interval here is the same paired-by-task interval as EvalHarness compare; use compare for the per-task table and comparability checks.");
        return 0;
    }
}
