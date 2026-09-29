// EvalHarness - Module 7 lab tool. Formalizes the Module 4 tasks-v0 check into an evaluation harness.
//   validate  <tasks.json> [--repo <dir>]                  every task is well-formed; references pass, counterexamples fail
//   run       <tasks.json> --repo <dir> --out <dir>        N trials per task, fresh headless agent session per trial
//   judge     <rubric.md> <answers-dir> --out <csv>        LLM-as-judge verdicts, one per answer file
//   grade     <tasks.json> <runs-dir> --config <name> --out <results.csv> [--verdicts <csv>]
//   stats     <results.csv> [--config <name>] [--k 3]      pass rate, Wilson interval, pass@k, pass^k, clustered interval
//   compare   <results.csv> --a <config> --b <config>      paired comparison by task
//   calibrate <human-labels.csv> <verdicts.csv> [--answers <dir>]   grader precision / recall / bias
//   gate      <results.csv> --baseline <config> --candidate <config>  regression gate for CI (exit code)
//   leak      <tasks.json> <repo>                          task prompts or answers copied into the AI layer
//   runs      --p 0.7 --halfwidth 0.1 [--delta 0.15]       how many runs you need
// Read-only on your repository: code tasks run in a copy under the output folder.
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
        "validate" => Commands.Validate(o),
        "run" => Commands.Run(o),
        "judge" => Commands.Judge(o),
        "grade" => Commands.Grade(o),
        "stats" => Commands.Stats(o),
        "compare" => Commands.Compare(o),
        "calibrate" => Commands.Calibrate(o),
        "gate" => Commands.Gate(o),
        "leak" => Commands.Leak(o),
        "runs" => Commands.RunsNeeded(o),
        _ => Usage()
    };
}
catch (UsageException e) { Console.Error.WriteLine(e.Message); return Usage(); }

static int Usage()
{
    Console.Error.WriteLine("""
        usage:
          EvalHarness validate  <tasks.json> [--repo <brownfield-dir>]
          EvalHarness run       <tasks.json> --repo <dir> --out <runs-dir> [--trials 3] [--only T01,T02]
                                [--tags golden,regression] [--agent claude|replay:<dir>] [--agent-exe claude] [--model <id>] [--seed 7] [--keep true]
          EvalHarness judge     <rubric.md> <answers-dir> --out <verdicts.csv> [--only T21] [--agent-exe claude] [--model <id>]
          EvalHarness grade     <tasks.json> <runs-dir> --config <name> --out <results.csv> [--verdicts <csv>] [--replace true]
          EvalHarness stats     <results.csv> [--config <name>] [--k 3]
          EvalHarness compare   <results.csv> --a <config> --b <config>
          EvalHarness calibrate <human-labels.csv> <verdicts.csv> [--answers <dir>] [--min-precision 0.9] [--min-recall 0.8]
          EvalHarness gate      <results.csv> --baseline <config> --candidate <config> [--margin 0.05]
                                [--golden-min 0.8] [--golden-drop 1] [--max-cost-ratio 1.25] [--allow-drift true] [--aggregate-only true] [--subset true]
          EvalHarness leak      <tasks.json> <repo>
          EvalHarness runs      --p 0.7 --halfwidth 0.1 [--delta 0.15]
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
}

// ---------------------------------------------------------------- task model

sealed record TaskDef(
    string Id, string Fact, string Kind, string Split, string[] Tags, string Source, string Prompt,
    string Grader, string[] Must, string[] MustNot, string Tests, string Filter, string[] Scope, string Target,
    string Rubric, string Reference, string[] Counterexamples)
{
    public bool Has(string tag) => Tags.Contains(tag, StringComparer.OrdinalIgnoreCase);
}

sealed record TaskSet(string Path, string Dir, string Version, string Sha, List<TaskDef> Tasks)
{
    public static TaskSet Load(string path)
    {
        var full = System.IO.Path.GetFullPath(path);
        var text = File.ReadAllText(full);
        using var doc = JsonDocument.Parse(text);
        var root = doc.RootElement;
        static string Str(JsonElement e, string k, string d = "") => e.TryGetProperty(k, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString()! : d;
        static string[] Arr(JsonElement e, string k) => e.TryGetProperty(k, out var v) && v.ValueKind == JsonValueKind.Array ? v.EnumerateArray().Select(x => x.GetString() ?? "").ToArray() : Array.Empty<string>();
        var tasks = new List<TaskDef>();
        foreach (var t in root.GetProperty("tasks").EnumerateArray())
        {
            var g = t.TryGetProperty("grader", out var gg) ? gg : default;
            bool hasG = g.ValueKind == JsonValueKind.Object;
            // tasks-v0 compatibility: top-level must/mustNot means a regex grader
            string type = hasG ? Str(g, "type", "regex") : "regex";
            tasks.Add(new TaskDef(
                Str(t, "id"), Str(t, "fact"), Str(t, "kind", "qa"), Str(t, "split", "dev"), Arr(t, "tags"), Str(t, "source"), Str(t, "prompt"),
                type, hasG ? Arr(g, "must") : Arr(t, "must"), hasG ? Arr(g, "mustNot") : Arr(t, "mustNot"),
                hasG ? Str(g, "tests") : "", hasG ? Str(g, "filter") : "", hasG ? Arr(g, "scope") : Array.Empty<string>(),
                hasG ? Str(g, "target", "tests/Contoso.Billing.Tests/Golden") : "", hasG ? Str(g, "rubric") : "",
                Str(t, "reference"), Arr(t, "counterexamples")));
        }
        return new TaskSet(full, System.IO.Path.GetDirectoryName(full)!, Str(root, "version", "unversioned"), Util.Sha(text), tasks);
    }

    public string Abs(string rel) => System.IO.Path.GetFullPath(System.IO.Path.Combine(Dir, rel));
}

static class Grading
{
    const RegexOptions RO = RegexOptions.IgnoreCase | RegexOptions.Multiline;

    /// <summary>null = pass; otherwise the first reason it failed.</summary>
    public static string? Regex(TaskDef t, string answer) =>
        t.Must.Where(p => !System.Text.RegularExpressions.Regex.IsMatch(answer, p, RO)).Select(p => $"missing /{p}/").FirstOrDefault()
        ?? t.MustNot.Where(p => System.Text.RegularExpressions.Regex.IsMatch(answer, p, RO)).Select(p => $"contains /{p}/").FirstOrDefault();

    /// <summary>Changed files must match one of the scope globs (** = any path, * = within one segment).</summary>
    public static List<string> OutOfScope(IEnumerable<string> changed, string[] scope) =>
        scope.Length == 0 ? new() : changed.Where(c => !scope.Any(g => Glob(g).IsMatch(c))).ToList();

    static Regex Glob(string g) => new("^" + System.Text.RegularExpressions.Regex.Escape(g).Replace(@"\*\*", "\u0001").Replace(@"\*", "[^/]*").Replace("\u0001", ".*") + "$", RegexOptions.IgnoreCase);
}

// ---------------------------------------------------------------- small utilities

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

    public static Dictionary<string, string> Snapshot(string root) =>
        Files(root).ToDictionary(f => Rel(root, f), f => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(f))));

    public static List<string> Changed(Dictionary<string, string> before, Dictionary<string, string> after) =>
        after.Where(kv => !before.TryGetValue(kv.Key, out var h) || h != kv.Value).Select(kv => kv.Key)
            .Concat(before.Keys.Where(k => !after.ContainsKey(k))).OrderBy(x => x).ToList();

    /// <summary>The AI layer: the files that steer the agent. Its hash goes into every manifest.</summary>
    public static List<string> LayerFiles(string repo)
    {
        var all = Files(repo).Select(f => Rel(repo, f)).ToList();
        return all.Where(r =>
            Path.GetFileName(r) is "CLAUDE.md" or "AGENTS.md" or "CLAUDE.local.md" ||
            r.StartsWith(".claude/") || r.StartsWith(".cursor/") || r == ".github/copilot-instructions.md" ||
            r.StartsWith(".github/instructions/") || r.StartsWith("docs/ai/") || r == ".mcp.json").OrderBy(x => x).ToList();
    }

    public static string LayerSha(string repo) =>
        Sha(string.Concat(LayerFiles(repo).Select(r => r + "\n" + File.ReadAllText(Path.Combine(repo, r)))));

    public static (int Exit, string Out, string Err, long Ms) Exec(string exe, IEnumerable<string> args, string cwd, int timeoutSec = 900)
    {
        var psi = new ProcessStartInfo(exe) { WorkingDirectory = cwd, RedirectStandardOutput = true, RedirectStandardError = true, RedirectStandardInput = true, UseShellExecute = false };
        foreach (var a in args) psi.ArgumentList.Add(a);
        var sw = Stopwatch.StartNew();
        using var p = Process.Start(psi) ?? throw new InvalidOperationException($"cannot start {exe}");
        p.StandardInput.Close(); // nothing on stdin: the agent must not wait for input
        var so = p.StandardOutput.ReadToEndAsync();
        var se = p.StandardError.ReadToEndAsync();
        if (!p.WaitForExit(timeoutSec * 1000)) { try { p.Kill(true); } catch { } return (-1, so.IsCompleted ? so.Result : "", "timeout", sw.ElapsedMilliseconds); }
        p.WaitForExit();
        return (p.ExitCode, so.Result, se.Result, sw.ElapsedMilliseconds);
    }

    public static string Csv(string s) => s.Contains(',') || s.Contains('"') || s.Contains('\n') ? "\"" + s.Replace("\"", "\"\"").Replace('\n', ' ') + "\"" : s;

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

    public static string Pct(double x) => (x * 100).ToString("F0") + "%";
    public static string F(double x) => x.ToString("F3");
}

// ---------------------------------------------------------------- statistics

static class Stat
{
    /// <summary>Wilson score interval for k successes in n trials (Wilson 1927; Brown, Cai and DasGupta 2001).</summary>
    public static (double Lo, double Hi) Wilson(int k, int n, double z = 1.96)
    {
        if (n == 0) return (0, 1);
        double p = (double)k / n, z2 = z * z, den = 1 + z2 / n;
        double centre = (p + z2 / (2 * n)) / den;
        double half = z * Math.Sqrt(p * (1 - p) / n + z2 / (4.0 * n * n)) / den;
        return (Math.Max(0, centre - half), Math.Min(1, centre + half));
    }

    /// <summary>Unbiased pass@k from n trials with c passes (Chen et al. 2021): 1 - C(n-c,k)/C(n,k).</summary>
    public static double PassAtK(int n, int c, int k)
    {
        if (n - c < k) return 1.0;
        double prod = 1;
        for (int i = n - c + 1; i <= n; i++) prod *= 1.0 - (double)k / i;
        return 1 - prod;
    }

    /// <summary>pass^k: chance that k trials drawn from the n all pass, C(c,k)/C(n,k) (tau-bench style).</summary>
    public static double PassHatK(int n, int c, int k)
    {
        if (c < k) return 0;
        double prod = 1;
        for (int i = 0; i < k; i++) prod *= (double)(c - i) / (n - i);
        return prod;
    }

    /// <summary>Two-sided 95% t critical value.</summary>
    public static double T95(int df) => df switch
    {
        <= 0 => double.NaN, 1 => 12.706, 2 => 4.303, 3 => 3.182, 4 => 2.776, 5 => 2.571, 6 => 2.447, 7 => 2.365, 8 => 2.306, 9 => 2.262,
        10 => 2.228, 11 => 2.201, 12 => 2.179, 13 => 2.160, 14 => 2.145, 15 => 2.131, 16 => 2.120, 17 => 2.110, 18 => 2.101, 19 => 2.093,
        20 => 2.086, <= 24 => 2.064, <= 29 => 2.045, <= 39 => 2.021, <= 59 => 2.000, <= 119 => 1.980, _ => 1.960
    };

    public static (double Mean, double Sd) MeanSd(IReadOnlyList<double> xs)
    {
        if (xs.Count == 0) return (double.NaN, double.NaN);
        double m = xs.Average();
        double sd = xs.Count < 2 ? 0 : Math.Sqrt(xs.Sum(x => (x - m) * (x - m)) / (xs.Count - 1));
        return (m, sd);
    }
}

sealed record Row(string Config, string Task, string Split, string Tags, int Trial, bool Pass, string Reason, double Cost, long Input, long Ms,
    string Agent, string Model, string Layer, string TasksSha, double JudgeCost = 0)
{
    public static List<Row> Load(string path) => Util.ReadCsv(path).Select(d => new Row(
        d["config"], d["task"], d.GetValueOrDefault("split", "dev"), d.GetValueOrDefault("tags", ""), int.Parse(d["trial"]), d["pass"] == "1",
        d.GetValueOrDefault("reason", ""), double.TryParse(d.GetValueOrDefault("cost_usd", ""), out var c) ? c : 0,
        long.TryParse(d.GetValueOrDefault("input_tokens", ""), out var i) ? i : 0, long.TryParse(d.GetValueOrDefault("duration_ms", ""), out var ms) ? ms : 0,
        d.GetValueOrDefault("agent_version", ""), d.GetValueOrDefault("model", ""), d.GetValueOrDefault("layer_sha", ""), d.GetValueOrDefault("tasks_sha", ""),
        double.TryParse(d.GetValueOrDefault("judge_cost_usd", ""), out var jc) ? jc : 0)).ToList();

    public const string Header = "config,task,split,tags,trial,pass,reason,cost_usd,input_tokens,duration_ms,agent_version,model,layer_sha,tasks_sha,judge_cost_usd";
    public string ToCsv() => string.Join(",", Config, Task, Split, Util.Csv(Tags), Trial, Pass ? 1 : 0, Util.Csv(Reason), Cost.ToString("F4"), Input, Ms,
        Util.Csv(Agent), Util.Csv(Model), Layer, TasksSha, JudgeCost.ToString("F4"));
}

sealed record Paired(List<(string Task, double A, double B, int NA, int NB)> Tasks, double Mean, double Sd, double Se, double Lo, double Hi,
    double PA, double PB, int NA, int NB, double ULo, double UHi, double CLo, double CHi, List<string> OnlyOne)
{
    public static Paired Of(List<Row> rows, string a, string b)
    {
        var ra = rows.Where(r => r.Config == a).ToList();
        var rb = rows.Where(r => r.Config == b).ToList();
        if (ra.Count == 0 || rb.Count == 0) throw new UsageException($"no rows for config '{(ra.Count == 0 ? a : b)}'");
        var ta = ra.GroupBy(r => r.Task).ToDictionary(g => g.Key, g => g.ToList());
        var tb = rb.GroupBy(r => r.Task).ToDictionary(g => g.Key, g => g.ToList());
        var common = ta.Keys.Intersect(tb.Keys).OrderBy(x => x).ToList();
        var only = ta.Keys.Union(tb.Keys).Except(common).OrderBy(x => x).ToList();
        var per = common.Select(t => (t, ta[t].Average(r => r.Pass ? 1.0 : 0), tb[t].Average(r => r.Pass ? 1.0 : 0), ta[t].Count, tb[t].Count)).ToList();
        var d = per.Select(x => x.Item3 - x.Item2).ToList();
        var (m, sd) = Stat.MeanSd(d);
        double se = d.Count > 1 ? sd / Math.Sqrt(d.Count) : double.NaN, t = Stat.T95(d.Count - 1);
        var ca = common.SelectMany(x => ta[x]).ToList(); var cb = common.SelectMany(x => tb[x]).ToList();
        double pa = ca.Average(r => r.Pass ? 1.0 : 0), pb = cb.Average(r => r.Pass ? 1.0 : 0);
        double use = Math.Sqrt(pa * (1 - pa) / ca.Count + pb * (1 - pb) / cb.Count);
        // Unpaired but task-clustered: each arm's mean of task rates, with its own task-level SE.
        var (_, sa) = Stat.MeanSd(per.Select(x => x.Item2).ToList());
        var (_, sb) = Stat.MeanSd(per.Select(x => x.Item3).ToList());
        double cse = Math.Sqrt((sa * sa + sb * sb) / Math.Max(1, per.Count));
        return new Paired(per, m, sd, se, m - t * se, m + t * se, pa, pb, ca.Count, cb.Count, pb - pa - 1.96 * use, pb - pa + 1.96 * use,
            m - t * cse, m + t * cse, only);
    }
}

// ---------------------------------------------------------------- commands

static class Commands
{
    public static int Validate(Opts o)
    {
        var ts = TaskSet.Load(o.P(0, "tasks.json"));
        var errors = new List<string>(); var warns = new List<string>();
        var ids = new HashSet<string>();
        foreach (var t in ts.Tasks)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(t.Id, @"^T\d{2,3}$")) errors.Add($"{t.Id}: id must look like T01");
            if (!ids.Add(t.Id)) errors.Add($"{t.Id}: duplicate id");
            if (t.Prompt.Length < 20) errors.Add($"{t.Id}: prompt missing or too short");
            if (t.Split is not ("dev" or "holdout")) errors.Add($"{t.Id}: split must be dev or holdout");
            if (t.Kind is not ("qa" or "code")) errors.Add($"{t.Id}: kind must be qa or code");
            if (t.Source.Length == 0) warns.Add($"{t.Id}: no source (where did this task come from?)");
            switch (t.Grader)
            {
                case "regex":
                    if (t.Must.Length == 0) errors.Add($"{t.Id}: regex grader needs at least one 'must' pattern");
                    foreach (var p in t.Must.Concat(t.MustNot))
                        try { _ = new Regex(p); } catch (ArgumentException e) { errors.Add($"{t.Id}: bad pattern /{p}/: {e.Message}"); }
                    if (t.Reference.Length == 0) errors.Add($"{t.Id}: no reference answer (write the correct answer before the first run)");
                    else if (Grading.Regex(t, t.Reference) is { } why) errors.Add($"{t.Id}: the grader FAILS the reference answer ({why})");
                    if (t.Counterexamples.Length == 0) warns.Add($"{t.Id}: no counterexample (a wrong answer the grader must reject)");
                    foreach (var c in t.Counterexamples)
                        if (Grading.Regex(t, c) is null) errors.Add($"{t.Id}: the grader PASSES the counterexample \"{Short(c)}\"");
                    break;
                case "judge":
                    if (!File.Exists(ts.Abs(t.Rubric))) errors.Add($"{t.Id}: rubric not found: {t.Rubric}");
                    if (t.Reference.Length == 0) errors.Add($"{t.Id}: no reference answer for the rubric");
                    break;
                case "tests":
                    if (!Directory.Exists(ts.Abs(t.Tests)) || !Directory.EnumerateFiles(ts.Abs(t.Tests), "*.cs").Any()) errors.Add($"{t.Id}: no golden tests in {t.Tests}");
                    if (t.Filter.Length == 0) errors.Add($"{t.Id}: tests grader needs a filter");
                    if (!Directory.Exists(ts.Abs(t.Reference))) errors.Add($"{t.Id}: reference overlay not found: {t.Reference}");
                    if (t.Kind != "code") errors.Add($"{t.Id}: tests grader only makes sense for kind 'code'");
                    break;
                default: errors.Add($"{t.Id}: unknown grader type '{t.Grader}'"); break;
            }
        }

        if (o.Named.TryGetValue("repo", out var repo))
        {
            repo = Path.GetFullPath(repo);
            foreach (var t in ts.Tasks.Where(t => t.Grader == "tests"))
            {
                var baseline = GoldenCheck(ts, t, repo, overlay: null);
                if (baseline.Pass) errors.Add($"{t.Id}: golden tests already PASS on the unmodified repository; the task measures nothing");
                else Console.WriteLine($"{t.Id}: unmodified repo fails the golden tests as expected ({baseline.Why})");
                var reference = GoldenCheck(ts, t, repo, overlay: ts.Abs(t.Reference));
                if (!reference.Pass) errors.Add($"{t.Id}: the reference solution FAILS its golden tests ({reference.Why})");
                else Console.WriteLine($"{t.Id}: reference solution passes the golden tests ({reference.Why})");
            }
        }

        var n = ts.Tasks.Count;
        int holdout = ts.Tasks.Count(t => t.Split == "holdout");
        Console.WriteLine($"\n{ts.Version} ({ts.Sha}): {n} tasks | dev {n - holdout}, holdout {holdout} | " +
            string.Join(", ", ts.Tasks.GroupBy(t => t.Kind + "/" + t.Grader).Select(g => $"{g.Key} {g.Count()}")));
        Console.WriteLine("tags: " + string.Join(", ", ts.Tasks.SelectMany(t => t.Tags).GroupBy(x => x).OrderBy(g => g.Key).Select(g => $"{g.Key} {g.Count()}")));
        if (n < 20) warns.Add($"only {n} tasks; aim for at least 20 drawn from real work and real failures");
        if (holdout < Math.Ceiling(0.2 * n)) warns.Add($"holdout is {holdout}/{n}; keep at least 20% of tasks out of sight while you tune the layer");
        if (!ts.Tasks.Any(t => t.Has("golden"))) warns.Add("no task is tagged golden (tasks that must never regress)");
        foreach (var w in warns) Console.WriteLine("WARN  " + w);
        foreach (var e in errors) Console.WriteLine("ERROR " + e);
        Console.WriteLine($"{errors.Count} errors, {warns.Count} warnings");
        return errors.Count == 0 ? 0 : 1;
    }

    static string Short(string s) => s.Length <= 60 ? s.Replace('\n', ' ') : s[..57].Replace('\n', ' ') + "...";

    /// <summary>Copies the repo (optionally with an overlay), adds golden tests, runs them.</summary>
    static (bool Pass, string Why) GoldenCheck(TaskSet ts, TaskDef t, string repo, string? overlay)
    {
        var work = Path.Combine(Path.GetTempPath(), "evalharness-" + Guid.NewGuid().ToString("N")[..8]);
        try
        {
            Util.CopyDir(repo, work);
            if (overlay is not null) Util.CopyDir(overlay, work);
            var r = RunGolden(ts, t, work);
            return (r.Pass, r.Summary);
        }
        finally { try { Directory.Delete(work, true); } catch { } }
    }

    sealed record GoldenResult(bool Pass, int Exit, int Total, int Failed, int Skipped, string Summary);

    static GoldenResult RunGolden(TaskSet ts, TaskDef t, string work)
    {
        var target = Path.Combine(work, t.Target);
        Directory.CreateDirectory(target);
        foreach (var f in Directory.EnumerateFiles(ts.Abs(t.Tests), "*.cs")) File.Copy(f, Path.Combine(target, Path.GetFileName(f)), true);
        var sln = Directory.EnumerateFiles(work, "*.sln").FirstOrDefault();
        var args = new List<string> { "test" };
        if (sln is not null) args.Add(sln);
        args.AddRange(new[] { "--filter", t.Filter, "--nologo" });
        var (exit, stdout, stderr, _) = Util.Exec("dotnet", args, work, 600);
        int total = 0, failed = 0, skipped = 0;
        foreach (Match m in System.Text.RegularExpressions.Regex.Matches(stdout, @"Failed:\s*(\d+),\s*Passed:\s*(\d+),\s*Skipped:\s*(\d+),\s*Total:\s*(\d+)"))
        { failed += int.Parse(m.Groups[1].Value); skipped += int.Parse(m.Groups[3].Value); total += int.Parse(m.Groups[4].Value); }
        string summary = total == 0
            ? (System.Text.RegularExpressions.Regex.IsMatch(stdout + stderr, @"error CS\d+") ? "build failed: " + System.Text.RegularExpressions.Regex.Match(stdout + stderr, @"error CS\d+[^\r\n\[]*").Value.Trim() : $"no golden tests ran (exit {exit})")
            : $"golden tests: {total - failed - skipped}/{total} passed, {failed} failed, {skipped} skipped";
        return new GoldenResult(exit == 0 && total > 0 && failed == 0 && skipped == 0, exit, total, failed, skipped, summary);
    }

    // ------------------------------------------------------------ run

    public static int Run(Opts o)
    {
        var ts = TaskSet.Load(o.P(0, "tasks.json"));
        var repo = Path.GetFullPath(o.Req("repo"));
        var outDir = Path.GetFullPath(o.Req("out"));
        int trials = o.I("trials", 3), seed = o.I("seed", 7);
        var agent = o.S("agent", "claude");
        var exe = o.S("agent-exe", "claude");
        var model = o.S("model", "");
        var only = o.S("only", "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToHashSet();
        var tags = o.S("tags", "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var tasks = ts.Tasks.Where(t => (only.Count == 0 || only.Contains(t.Id)) && (tags.Length == 0 || tags.Any(t.Has))).ToList();
        if (tasks.Count == 0) throw new UsageException("no task matches --only / --tags");
        if (!Directory.Exists(repo)) throw new UsageException($"repo not found: {repo}");
        Directory.CreateDirectory(outDir);

        bool replay = agent.StartsWith("replay:");
        string agentVersion = replay ? agent : TryVersion(exe);
        var manifestPath = Path.Combine(outDir, "manifest.json");
        var manifest = new Dictionary<string, object?>
        {
            ["started_utc"] = DateTime.UtcNow.ToString("o"), ["tasks"] = ts.Version, ["tasks_sha"] = ts.Sha,
            ["repo"] = repo, ["layer_sha"] = Util.LayerSha(repo), ["layer_files"] = Util.LayerFiles(repo),
            ["agent"] = agent, ["agent_version"] = agentVersion, ["model"] = model == "" ? "(agent default)" : model,
            ["trials"] = trials, ["seed"] = seed, ["order"] = "interleaved rounds, shuffled per round",
            ["host"] = Environment.MachineName.Length > 0 ? "recorded" : "", ["dotnet"] = Environment.Version.ToString()
        };
        if (File.Exists(manifestPath))
        {
            using var old = JsonDocument.Parse(File.ReadAllText(manifestPath));
            foreach (var k in new[] { "tasks_sha", "layer_sha", "agent_version", "model" })
                if (old.RootElement.TryGetProperty(k, out var v) && v.ToString() != manifest[k]!.ToString())
                    Console.WriteLine($"WARN  resuming into {outDir} but {k} changed: {v} -> {manifest[k]}. Use a new --out folder for a new configuration.");
        }
        File.WriteAllText(manifestPath, JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"{ts.Version}: {tasks.Count} tasks x {trials} trials | agent {agentVersion} | layer {manifest["layer_sha"]} | out {outDir}");

        var rng = new Random(seed);
        int done = 0, skipped = 0;
        for (int k = 1; k <= trials; k++)
        {
            // Interleave: every task's trial k runs before any task's trial k+1, in a fresh random order,
            // so slow drift (model updates, rate limits, time of day) spreads over all tasks evenly.
            foreach (var t in tasks.OrderBy(_ => rng.Next()).ToList())
            {
                var file = Path.Combine(outDir, $"{t.Id}.r{k}.json");
                if (File.Exists(file)) { skipped++; continue; } // resumable
                if (replay) { if (Replay(agent[7..], t, k, outDir)) done++; continue; }
                Console.WriteLine($"{t.Id} trial {k}/{trials} ({t.Kind})");
                if (t.Kind == "code") RunCode(ts, t, k, repo, outDir, exe, model, o.B("keep"));
                else
                {
                    var r = Util.Exec(exe, AgentArgs(t.Prompt, "dontAsk", model, null), repo);
                    File.WriteAllText(file, r.Out.Length > 0 ? r.Out : JsonSerializer.Serialize(new { type = "result", is_error = true, result = r.Err }));
                    WriteCheck(outDir, t.Id, k, new { duration_ms = r.Ms, exit = r.Exit });
                }
                done++;
            }
        }
        Console.WriteLine($"done: {done} new trial(s), {skipped} already present, in {outDir}");
        return 0;
    }

    static string TryVersion(string exe)
    {
        try { var r = Util.Exec(exe, new[] { "--version" }, Directory.GetCurrentDirectory(), 60); return r.Out.Trim().Split('\n')[0].Trim(); }
        catch (Exception e) { throw new UsageException($"cannot run '{exe} --version' ({e.Message}). Install the agent CLI, pass --agent-exe <path>, or use --agent replay:<dir>."); }
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

    static List<string> AgentArgs(string prompt, string mode, string model, string? allowed)
    {
        // --setting-sources + --settings: your ~/.claude settings, skills and CLAUDE.md stay out (see IsolationSettings).
        var a = new List<string> { "-p", prompt, "--output-format", "json", "--permission-mode", mode,
            "--setting-sources", "project,local", "--settings", IsolationSettings.Value };
        if (allowed is not null) { a.Add("--allowedTools"); a.Add(allowed); }
        if (model.Length > 0) { a.Add("--model"); a.Add(model); }
        return a;
    }

    static void WriteCheck(string outDir, string id, int k, object check) =>
        File.WriteAllText(Path.Combine(outDir, $"{id}.r{k}.check.json"), JsonSerializer.Serialize(check, new JsonSerializerOptions { WriteIndented = true }));

    static bool Replay(string dir, TaskDef t, int k, string outDir)
    {
        var src = Path.Combine(Path.GetFullPath(dir), $"{t.Id}.r{k}.json");
        if (!File.Exists(src)) return false;
        File.Copy(src, Path.Combine(outDir, $"{t.Id}.r{k}.json"), true);
        var chk = Path.ChangeExtension(src, null) + ".check.json";
        if (File.Exists(chk)) File.Copy(chk, Path.Combine(outDir, $"{t.Id}.r{k}.check.json"), true);
        return true;
    }

    static void RunCode(TaskSet ts, TaskDef t, int k, string repo, string outDir, string exe, string model, bool keep)
    {
        var work = Path.Combine(outDir, "work", $"{t.Id}.r{k}");
        if (Directory.Exists(work)) Directory.Delete(work, true);
        Util.CopyDir(repo, work);
        var before = Util.Snapshot(work);
        // Edits allowed in the copy; only build and test commands may run. Nothing touches your repository.
        var r = Util.Exec(exe, AgentArgs(t.Prompt, "acceptEdits", model, "Bash(dotnet build *),Bash(dotnet test *)"), work, 1800);
        File.WriteAllText(Path.Combine(outDir, $"{t.Id}.r{k}.json"), r.Out.Length > 0 ? r.Out : JsonSerializer.Serialize(new { type = "result", is_error = true, result = r.Err }));
        var changed = Util.Changed(before, Util.Snapshot(work));
        var golden = RunGolden(ts, t, work);
        WriteCheck(outDir, t.Id, k, new
        {
            duration_ms = r.Ms, exit = r.Exit, changed, out_of_scope = Grading.OutOfScope(changed, t.Scope),
            tests = new { pass = golden.Pass, total = golden.Total, failed = golden.Failed, skipped = golden.Skipped, summary = golden.Summary }
        });
        Console.WriteLine($"  {golden.Summary}; changed {changed.Count} file(s)");
        if (!keep) try { Directory.Delete(work, true); } catch { }
    }

    // ------------------------------------------------------------ judge

    public static int Judge(Opts o)
    {
        var rubricPath = o.P(0, "rubric.md");
        var rubric = File.ReadAllText(rubricPath);
        var dir = Path.GetFullPath(o.P(1, "answers-dir"));
        var outCsv = o.Req("out");
        var exe = o.S("agent-exe", "claude");
        var model = o.S("model", "");
        var only = o.S("only", "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var done = File.Exists(outCsv) ? Util.ReadCsv(outCsv).Select(r => r["file"]).ToHashSet() : new HashSet<string>();
        if (!File.Exists(outCsv)) File.WriteAllText(outCsv, "file,verdict,reason\n");
        // The judge runs in an empty folder so no CLAUDE.md, rules or code can leak into its verdict.
        var empty = Path.Combine(Path.GetTempPath(), "evalharness-judge-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(empty);
        var files = Directory.EnumerateFiles(dir).Where(f => System.Text.RegularExpressions.Regex.IsMatch(f, @"\.(json|txt|md)$") && !f.EndsWith(".check.json") && !f.EndsWith(".judge.json") && Path.GetFileName(f) != "manifest.json")
            .Where(f => only.Length == 0 || only.Any(p => Path.GetFileName(f).StartsWith(p + "."))).OrderBy(f => f).ToList();
        int n = 0; double judgeCost = 0;
        foreach (var f in files)
        {
            var name = Path.GetFileName(f);
            if (done.Contains(name)) continue;
            var answer = AnswerText(f);
            var prompt = rubric + "\n\n## Answer to grade\n<answer>\n" + answer + "\n</answer>\n";
            var r = Util.Exec(exe, AgentArgs(prompt, "dontAsk", model, null), empty, 300);
            // Keep the judge's raw JSON next to the answer (<answer>.judge.json) so its cost and usage can be summed later.
            File.WriteAllText(JudgeFile(f), r.Out.Length > 0 ? r.Out : JsonSerializer.Serialize(new { type = "result", is_error = true, result = r.Err }));
            judgeCost += ReadRun(JudgeFile(f)).Cost;
            var text = AnswerText(r.Out, fromJson: true);
            var m = System.Text.RegularExpressions.Regex.Matches(text, @"VERDICT:\s*(PASS|FAIL)", RegexOptions.IgnoreCase).LastOrDefault();
            var verdict = m is null ? "error" : m.Groups[1].Value.ToLowerInvariant();
            var reason = text.Split('\n').Select(l => l.Trim()).FirstOrDefault(l => l.Length > 0 && !l.StartsWith("VERDICT", StringComparison.OrdinalIgnoreCase)) ?? "";
            File.AppendAllText(outCsv, $"{name},{verdict},{Util.Csv(reason.Length > 160 ? reason[..160] : reason)}\n");
            Console.WriteLine($"{name}: {verdict}");
            n++;
        }
        try { Directory.Delete(empty, true); } catch { }
        Console.WriteLine($"{n} new verdict(s) appended to {outCsv}" + (judgeCost > 0 ? $"; judge cost ${judgeCost:F4}" : ""));
        return 0;
    }

    static string JudgeFile(string answer) => Path.Combine(Path.GetDirectoryName(answer)!, Path.GetFileNameWithoutExtension(answer) + ".judge.json");

    static string AnswerText(string pathOrJson, bool fromJson = false)
    {
        var raw = fromJson ? pathOrJson : File.ReadAllText(pathOrJson);
        if (!fromJson && !pathOrJson.EndsWith(".json")) return raw.Trim();
        try
        {
            using var doc = JsonDocument.Parse(raw);
            return doc.RootElement.TryGetProperty("result", out var res) && res.ValueKind == JsonValueKind.String ? res.GetString()!.Trim() : "";
        }
        catch (JsonException) { return raw.Trim(); }
    }

    // ------------------------------------------------------------ grade

    public static int Grade(Opts o)
    {
        var ts = TaskSet.Load(o.P(0, "tasks.json"));
        var runs = Path.GetFullPath(o.P(1, "runs-dir"));
        var config = o.Req("config");
        var outCsv = o.Req("out");
        if (!Directory.Exists(runs)) throw new UsageException($"not a directory: {runs}");

        string agentV = "", model = "", layer = "", tasksSha = ts.Sha;
        var mf = Path.Combine(runs, "manifest.json");
        if (File.Exists(mf))
        {
            using var m = JsonDocument.Parse(File.ReadAllText(mf));
            string G(string k) => m.RootElement.TryGetProperty(k, out var v) ? v.ToString() : "";
            agentV = G("agent_version"); model = G("model"); layer = G("layer_sha");
            if (G("tasks_sha") is { Length: > 0 } s && s != ts.Sha) Console.WriteLine($"WARN  runs were made with tasks {s}, grading with {ts.Sha}: prompts or graders changed since the run.");
        }
        else Console.WriteLine("WARN  no manifest.json in the runs folder; versions will be blank and compare cannot check comparability.");

        var verdicts = o.Named.TryGetValue("verdicts", out var vp) ? Util.ReadCsv(vp).ToDictionary(r => r["file"], r => r["verdict"]) : new Dictionary<string, string>();
        var rows = new List<Row>();
        int ungraded = 0;
        var notRun = new List<string>();
        Console.WriteLine($"{"TASK",-5} {"SPLIT",-8} {"GRADER",-6} {"PASS",6}  FIRST FAILURE");
        foreach (var t in ts.Tasks)
        {
            var files = Directory.EnumerateFiles(runs, $"{t.Id}.r*.json").Where(f => System.Text.RegularExpressions.Regex.IsMatch(Path.GetFileName(f), $@"^{t.Id}\.r\d+\.json$"))
                .OrderBy(f => f).ToList();
            if (files.Count == 0) { notRun.Add(t.Id); continue; }
            int ok = 0, n = 0; string? first = null;
            foreach (var f in files)
            {
                var name = Path.GetFileName(f);
                int trial = int.Parse(System.Text.RegularExpressions.Regex.Match(name, @"\.r(\d+)\.json$").Groups[1].Value);
                var (text, isError, cost, input) = ReadRun(f);
                var chkPath = Path.Combine(runs, $"{t.Id}.r{trial}.check.json");
                long ms = 0; JsonElement? chk = null; JsonDocument? cd = null;
                if (File.Exists(chkPath)) { cd = JsonDocument.Parse(File.ReadAllText(chkPath)); chk = cd.RootElement; if (chk.Value.TryGetProperty("duration_ms", out var d)) ms = d.GetInt64(); }
                string? why; double judgeCost = 0;
                if (isError) why = "the run reported an error";
                else if (t.Grader == "regex") why = Grading.Regex(t, text);
                else if (t.Grader == "judge")
                {
                    if (!verdicts.TryGetValue(name, out var v)) { ungraded++; cd?.Dispose(); continue; }
                    why = v == "pass" ? null : $"judge: {v}";
                    if (File.Exists(JudgeFile(f))) judgeCost = ReadRun(JudgeFile(f)).Cost;
                }
                else
                {
                    if (chk is null || !chk.Value.TryGetProperty("tests", out var tests)) why = "no check file (code task not run by the harness?)";
                    else if (!tests.GetProperty("pass").GetBoolean()) why = tests.GetProperty("summary").GetString();
                    else if (chk.Value.TryGetProperty("out_of_scope", out var oos) && oos.GetArrayLength() > 0) why = "out of scope: " + string.Join(" ", oos.EnumerateArray().Select(x => x.GetString()));
                    else why = null;
                }
                cd?.Dispose();
                n++; if (why is null) ok++; else first ??= $"{name}: {why}";
                rows.Add(new Row(config, t.Id, t.Split, string.Join(";", t.Tags), trial, why is null, why ?? "", cost, input, ms, agentV, model, layer, tasksSha, judgeCost));
            }
            if (n > 0) Console.WriteLine($"{t.Id,-5} {t.Split,-8} {t.Grader,-6} {ok,2}/{n,-3}  {first ?? ""}");
            else Console.WriteLine($"{t.Id,-5} {t.Split,-8} {t.Grader,-6} {"-",6}  no verdicts (run 'judge' and pass --verdicts)");
        }
        if (notRun.Count > 0) Console.WriteLine($"not run: {string.Join(", ", notRun)}");
        if (rows.Count == 0) { Console.Error.WriteLine($"no gradable runs in {runs}"); return 2; }
        if (ungraded > 0) Console.WriteLine($"WARN  {ungraded} judge-graded run(s) have no verdict and were left out.");

        int passes = rows.Count(r => r.Pass);
        var (lo, hi) = Stat.Wilson(passes, rows.Count);
        Console.WriteLine($"\n{config}: {passes}/{rows.Count} = {Util.Pct((double)passes / rows.Count)}  (95% Wilson {Util.Pct(lo)}-{Util.Pct(hi)}; see 'stats' for the task-clustered interval)");

        var existing = File.Exists(outCsv) ? File.ReadAllLines(outCsv).Skip(1).Where(l => l.Length > 0).ToList() : new List<string>();
        bool clash = existing.Any(l => l.StartsWith(config + ","));
        if (clash && !o.B("replace")) { Console.Error.WriteLine($"{outCsv} already has rows for config '{config}'. Use another --config name or --replace true."); return 2; }
        var keepRows = existing.Where(l => !l.StartsWith(config + ","));
        File.WriteAllLines(outCsv, new[] { Row.Header }.Concat(keepRows).Concat(rows.Select(r => r.ToCsv())));
        Console.WriteLine($"{(clash ? "replaced" : "wrote")} {rows.Count} rows for '{config}' in {outCsv}");
        return 0;
    }

    static (string Text, bool IsError, double Cost, long Input) ReadRun(string f)
    {
        var raw = File.ReadAllText(f);
        try
        {
            using var doc = JsonDocument.Parse(raw);
            var r = doc.RootElement;
            string text = r.TryGetProperty("result", out var res) && res.ValueKind == JsonValueKind.String ? res.GetString()! : "";
            bool err = r.TryGetProperty("is_error", out var e) && e.ValueKind == JsonValueKind.True;
            double cost = r.TryGetProperty("total_cost_usd", out var c) && c.ValueKind == JsonValueKind.Number ? c.GetDouble() : 0;
            long input = 0;
            if (r.TryGetProperty("usage", out var u) && u.ValueKind == JsonValueKind.Object)
                foreach (var k in new[] { "input_tokens", "cache_read_input_tokens", "cache_creation_input_tokens" })
                    if (u.TryGetProperty(k, out var v) && v.ValueKind == JsonValueKind.Number) input += v.GetInt64();
            return (text, err, cost, input);
        }
        catch (JsonException) { return (raw, true, 0, 0); }
    }

    // ------------------------------------------------------------ stats

    public static int Stats(Opts o)
    {
        var rows = Row.Load(o.P(0, "results.csv"));
        int k = o.I("k", 3);
        var configs = o.Named.TryGetValue("config", out var c) ? new List<string> { c } : rows.Select(r => r.Config).Distinct().ToList();
        foreach (var cfg in configs)
        {
            var rs = rows.Where(r => r.Config == cfg).ToList();
            if (rs.Count == 0) { Console.Error.WriteLine($"no rows for '{cfg}'"); return 2; }
            Console.WriteLine($"## {cfg}  (agent {First(rs, r => r.Agent)}, model {First(rs, r => r.Model)}, layer {First(rs, r => r.Layer)})");
            Line("all", rs);
            foreach (var g in rs.GroupBy(r => r.Split).OrderBy(g => g.Key)) Line(g.Key, g.ToList());
            var golden = rs.Where(r => r.Tags.Split(';').Contains("golden")).ToList();
            if (golden.Count > 0) Line("golden", golden);

            var tasks = rs.GroupBy(r => r.Task).OrderBy(g => g.Key).Select(g => (Id: g.Key, N: g.Count(), C: g.Count(r => r.Pass))).ToList();
            Console.WriteLine($"\n| task | passes | pass@1 | pass@{k} | pass^{k} |");
            Console.WriteLine("|---|---|---|---|---|");
            foreach (var t in tasks)
                Console.WriteLine(t.N >= k
                    ? $"| {t.Id} | {t.C}/{t.N} | {Util.F((double)t.C / t.N)} | {Util.F(Stat.PassAtK(t.N, t.C, k))} | {Util.F(Stat.PassHatK(t.N, t.C, k))} |"
                    : $"| {t.Id} | {t.C}/{t.N} | {Util.F((double)t.C / t.N)} | n<k | n<k |");
            var eligible = tasks.Where(t => t.N >= k).ToList();
            if (eligible.Count > 0)
                Console.WriteLine($"\nmean over {eligible.Count} tasks: pass@1 {Util.F(eligible.Average(t => (double)t.C / t.N))} | pass@{k} {Util.F(eligible.Average(t => Stat.PassAtK(t.N, t.C, k)))} | pass^{k} {Util.F(eligible.Average(t => Stat.PassHatK(t.N, t.C, k)))}");
            else Console.WriteLine($"\npass@{k} / pass^{k} need at least {k} trials per task.");

            // Task-clustered interval: the unit of sampling is the task, not the trial (Miller 2024).
            var rates = tasks.Select(t => (double)t.C / t.N).ToList();
            var (m, sd) = Stat.MeanSd(rates);
            if (rates.Count >= 2)
            {
                double se = sd / Math.Sqrt(rates.Count), t95 = Stat.T95(rates.Count - 1);
                double p = rs.Average(r => r.Pass ? 1.0 : 0), naive = Math.Sqrt(p * (1 - p) / rs.Count);
                Console.WriteLine($"task-clustered: mean of {rates.Count} task rates {Util.Pct(m)}, SE {Util.F(se)}, 95% CI {Util.Pct(Math.Max(0, m - t95 * se))}-{Util.Pct(Math.Min(1, m + t95 * se))}" +
                    (naive > 0 ? $" | naive per-trial SE {Util.F(naive)} (ratio {se / naive:F1}x)" : ""));
            }
            var cost = rs.Sum(r => r.Cost); int passes = rs.Count(r => r.Pass);
            if (cost > 0) Console.WriteLine($"cost: total ${cost:F4}, mean ${cost / rs.Count:F4}/trial, per passing trial {(passes == 0 ? "n/a" : "$" + (cost / passes).ToString("F4"))}");
            var jcost = rs.Sum(r => r.JudgeCost);
            if (jcost > 0) Console.WriteLine($"judge cost: ${jcost:F4} over {rs.Count(r => r.JudgeCost > 0)} verdict(s); agent + judge ${cost + jcost:F4}");
            Console.WriteLine();
        }
        return 0;

        static void Line(string label, List<Row> rs)
        {
            int p = rs.Count(r => r.Pass);
            var (lo, hi) = Stat.Wilson(p, rs.Count);
            Console.WriteLine($"{label,-8} {p,4}/{rs.Count,-4} = {Util.Pct((double)p / rs.Count),4}   95% Wilson [{Util.Pct(lo)}, {Util.Pct(hi)}]   width {Util.Pct(hi - lo)}");
        }
    }

    static string First(List<Row> rs, Func<Row, string> f)
    {
        var d = rs.Select(f).Where(x => x.Length > 0).Distinct().ToList();
        return d.Count == 0 ? "?" : d.Count == 1 ? d[0] : "MIXED(" + string.Join(" | ", d) + ")";
    }

    // ------------------------------------------------------------ compare

    public static int Compare(Opts o)
    {
        var rows = Row.Load(o.P(0, "results.csv"));
        string a = o.Req("a"), b = o.Req("b");
        var p = Paired.Of(rows, a, b);
        Comparability(rows, a, b, out var problems);
        foreach (var x in problems) Console.WriteLine("WARN  " + x);
        if (p.OnlyOne.Count > 0) Console.WriteLine($"WARN  tasks in only one arm were left out of the paired analysis: {string.Join(", ", p.OnlyOne)}");

        Console.WriteLine($"\n| task | {a} | {b} | diff |");
        Console.WriteLine("|---|---|---|---|");
        foreach (var t in p.Tasks.OrderBy(t => t.B - t.A).ThenBy(t => t.Task))
            Console.WriteLine($"| {t.Task} | {Util.F(t.A)} ({t.NA}) | {Util.F(t.B)} ({t.NB}) | {(t.B - t.A >= 0 ? "+" : "")}{Util.F(t.B - t.A)} |");
        int better = p.Tasks.Count(t => t.B > t.A), worse = p.Tasks.Count(t => t.B < t.A);
        Console.WriteLine($"\n{p.Tasks.Count} paired tasks: {b} better on {better}, worse on {worse}, equal on {p.Tasks.Count - better - worse}");
        Console.WriteLine($"pass rate {a} {Util.Pct(p.PA)} ({p.NA} trials) -> {b} {Util.Pct(p.PB)} ({p.NB} trials)");
        Console.WriteLine($"paired (by task):   diff {Signed(p.Mean)}, SE {Util.F(p.Se)}, 95% CI [{Signed(p.Lo)}, {Signed(p.Hi)}]");
        Console.WriteLine($"unpaired by task:   diff {Signed(p.Mean)}, 95% CI [{Signed(p.CLo)}, {Signed(p.CHi)}]  (ignores that both arms ran the same tasks)");
        Console.WriteLine($"naive per trial:    diff {Signed(p.PB - p.PA)}, 95% CI [{Signed(p.ULo)}, {Signed(p.UHi)}]  (treats every trial as independent)");
        foreach (var cfg in new[] { a, b })
        {
            var rs = rows.Where(r => r.Config == cfg && p.Tasks.Any(t => t.Task == r.Task)).ToList();
            double cost = rs.Sum(r => r.Cost); int ok = rs.Count(r => r.Pass);
            if (cost > 0) Console.WriteLine($"cost {cfg}: ${cost / rs.Count:F4}/trial, ${(ok == 0 ? double.NaN : cost / ok):F4} per passing trial, mean input tokens {rs.Average(r => (double)r.Input):N0}");
        }
        string verdict = double.IsNaN(p.Lo) ? "not enough tasks for an interval"
            : p.Lo > 0 ? $"{b} is better: the 95% paired interval excludes 0"
            : p.Hi < 0 ? $"{b} is worse: the 95% paired interval excludes 0"
            : $"no detectable difference at this sample size: the interval [{Signed(p.Lo)}, {Signed(p.Hi)}] includes 0";
        Console.WriteLine($"\nverdict: {verdict}");
        return 0;
    }

    static string Signed(double x) => double.IsNaN(x) ? "n/a" : (x >= 0 ? "+" : "") + (x * 100).ToString("F1") + " pts";

    static void Comparability(List<Row> rows, string a, string b, out List<string> problems)
    {
        problems = new List<string>();
        foreach (var (name, f) in new (string, Func<Row, string>)[] { ("agent_version", r => r.Agent), ("model", r => r.Model), ("tasks_sha", r => r.TasksSha) })
        {
            var va = rows.Where(r => r.Config == a).Select(f).Distinct().ToList();
            var vb = rows.Where(r => r.Config == b).Select(f).Distinct().ToList();
            if (va.Count > 1 || vb.Count > 1) problems.Add($"{name} is not constant within an arm ({a}: {string.Join("|", va)}; {b}: {string.Join("|", vb)})");
            else if (va[0] != vb[0]) problems.Add($"{name} differs between arms: {a}={va[0]}, {b}={vb[0]}. Anything but the treatment must be equal.");
        }
        var la = rows.Where(r => r.Config == a).Select(r => r.Layer).Distinct().ToList();
        var lb = rows.Where(r => r.Config == b).Select(r => r.Layer).Distinct().ToList();
        if (la.Count == 1 && lb.Count == 1 && la[0] == lb[0] && la[0].Length > 0)
            problems.Add($"layer_sha is identical in both arms ({la[0]}): fine for a model or prompt comparison or an A/A test, wrong if the AI layer was the treatment.");
    }

    // ------------------------------------------------------------ calibrate

    public static int Calibrate(Opts o)
    {
        var human = Util.ReadCsv(o.P(0, "human-labels.csv")).ToDictionary(r => r["file"], r => r["label"].Trim().ToLowerInvariant());
        var grader = Util.ReadCsv(o.P(1, "verdicts.csv")).ToDictionary(r => r["file"], r => r["verdict"].Trim().ToLowerInvariant());
        var both = human.Keys.Intersect(grader.Keys).OrderBy(x => x).ToList();
        if (both.Count == 0) { Console.Error.WriteLine("no file appears in both CSVs"); return 2; }
        int tp = 0, fp = 0, fn = 0, tn = 0, errs = 0;
        foreach (var f in both)
        {
            if (grader[f] is not ("pass" or "fail")) { errs++; continue; }
            bool h = human[f] == "pass", g = grader[f] == "pass";
            if (g && h) tp++; else if (g) fp++; else if (h) fn++; else tn++;
        }
        int n = tp + fp + fn + tn;
        double prec = tp + fp == 0 ? double.NaN : (double)tp / (tp + fp);
        double rec = tp + fn == 0 ? double.NaN : (double)tp / (tp + fn);
        double fpr = fp + tn == 0 ? double.NaN : (double)fp / (fp + tn);
        double acc = (double)(tp + tn) / n;
        double pe = ((double)(tp + fp) / n) * ((double)(tp + fn) / n) + ((double)(fn + tn) / n) * ((double)(fp + tn) / n);
        double kappa = pe >= 1 ? double.NaN : (acc - pe) / (1 - pe);
        Console.WriteLine($"{both.Count} answers labelled by both ({errs} grader errors left out). Positive = \"pass\".\n");
        Console.WriteLine("|                | human pass | human fail |");
        Console.WriteLine("|---|---|---|");
        Console.WriteLine($"| grader pass    | TP {tp,3}     | FP {fp,3}     |");
        Console.WriteLine($"| grader fail    | FN {fn,3}     | TN {tn,3}     |\n");
        Console.WriteLine($"precision TP/(TP+FP) = {Util.F(prec)}   how far to trust a \"pass\"");
        Console.WriteLine($"recall    TP/(TP+FN) = {Util.F(rec)}   share of good answers it lets through");
        Console.WriteLine($"false-positive rate FP/(FP+TN) = {Util.F(fpr)}   share of bad answers it lets through");
        Console.WriteLine($"accuracy {Util.F(acc)} | Cohen's kappa {Util.F(kappa)} (agreement beyond chance)");
        double hRate = (double)(tp + fn) / n, gRate = (double)(tp + fp) / n;
        Console.WriteLine($"on this set: human pass rate {Util.Pct(hRate)}, grader pass rate {Util.Pct(gRate)} (bias {Signed(gRate - hRate)})");
        if (!double.IsNaN(rec) && !double.IsNaN(fpr) && rec - fpr > 0.05)
            Console.WriteLine($"correction: an observed grader pass rate q estimates the true rate as (q - {Util.F(fpr)}) / ({Util.F(rec)} - {Util.F(fpr)})");
        else Console.WriteLine("correction: recall - FPR is near 0; this grader carries almost no information.");

        if (o.Named.TryGetValue("answers", out var dir))
        {
            var len = both.ToDictionary(f => f, f => File.Exists(Path.Combine(dir, f)) ? AnswerText(Path.Combine(dir, f)).Length : -1);
            var known = both.Where(f => len[f] >= 0 && grader[f] is "pass" or "fail").ToList();
            if (known.Count >= 4)
            {
                var sorted = known.Select(f => len[f]).OrderBy(x => x).ToList();
                double median = sorted.Count % 2 == 1 ? sorted[sorted.Count / 2] : (sorted[sorted.Count / 2 - 1] + sorted[sorted.Count / 2]) / 2.0;
                string Rate(IEnumerable<string> fs) { var l = fs.ToList(); return l.Count == 0 ? "n/a" : $"{l.Count(f => grader[f] == "pass")}/{l.Count}"; }
                var longW = known.Where(f => len[f] > median && human[f] == "fail"); var shortW = known.Where(f => len[f] <= median && human[f] == "fail");
                var longR = known.Where(f => len[f] > median && human[f] == "pass"); var shortR = known.Where(f => len[f] <= median && human[f] == "pass");
                Console.WriteLine($"\nlength check (median {median:F0} chars): grader passes");
                Console.WriteLine($"  wrong answers: long {Rate(longW)} vs short {Rate(shortW)}");
                Console.WriteLine($"  right answers: long {Rate(longR)} vs short {Rate(shortR)}");
                double Frac(IEnumerable<string> fs) { var l = fs.ToList(); return l.Count == 0 ? 0 : (double)l.Count(f => grader[f] == "pass") / l.Count; }
                double gap = (Frac(longW) + Frac(longR)) / 2 - (Frac(shortW) + Frac(shortR)) / 2;
                if (gap >= 0.3) Console.WriteLine($"  LENGTH BIAS: at equal human labels, long answers pass {gap * 100:F0} points more often than short ones.");
            }
        }
        double minP = o.D("min-precision", 0.9), minR = o.D("min-recall", 0.8);
        bool ok = prec >= minP && rec >= minR;
        Console.WriteLine($"\n{(ok ? "CALIBRATED" : "NOT CALIBRATED")}: needs precision >= {minP} and recall >= {minR}.");
        return ok ? 0 : 1;
    }

    // ------------------------------------------------------------ gate

    public static int Gate(Opts o)
    {
        var rows = Row.Load(o.P(0, "results.csv"));
        string a = o.Req("baseline"), b = o.Req("candidate");
        double margin = o.D("margin", 0.05), goldenMin = o.D("golden-min", 0.8), maxCost = o.D("max-cost-ratio", 1.25);
        int goldenDrop = o.I("golden-drop", 1);
        bool aggregateOnly = o.B("aggregate-only");
        var fails = new List<string>(); var notes = new List<string>();
        var p = Paired.Of(rows, a, b);

        Comparability(rows, a, b, out var problems);
        foreach (var x in problems.Where(x => !x.StartsWith("layer_sha")))
            if (o.B("allow-drift")) notes.Add(x); else fails.Add("not comparable: " + x);
        var candTasks = rows.Where(r => r.Config == b).Select(r => r.Task).ToHashSet();
        var missing = p.OnlyOne.Where(t => candTasks.Contains(t) || !o.B("subset")).ToList();
        if (missing.Count > 0) fails.Add($"task sets differ between arms: {string.Join(", ", missing)}");
        else if (p.OnlyOne.Count > 0) notes.Add($"subset run: {p.OnlyOne.Count} baseline task(s) not in the candidate were ignored");

        if (double.IsNaN(p.Lo)) fails.Add("fewer than 2 paired tasks: no interval");
        else if (p.Lo < -margin) fails.Add($"aggregate: paired 95% CI lower bound {Signed(p.Lo)} is below -{margin * 100:F0} pts (diff {Signed(p.Mean)})");
        else notes.Add($"aggregate: diff {Signed(p.Mean)}, 95% CI [{Signed(p.Lo)}, {Signed(p.Hi)}], lower bound above -{margin * 100:F0} pts");

        var goldenTasks = aggregateOnly ? new List<string>() : rows.Where(r => r.Config == b && r.Tags.Split(';').Contains("golden")).Select(r => r.Task).Distinct().OrderBy(x => x).ToList();
        foreach (var t in goldenTasks)
        {
            var rb = rows.Where(r => r.Config == b && r.Task == t).ToList();
            var ra = rows.Where(r => r.Config == a && r.Task == t).ToList();
            double rate = rb.Average(r => r.Pass ? 1.0 : 0);
            string was = ra.Count == 0 ? "n/a" : $"{ra.Count(r => r.Pass)}/{ra.Count}";
            int lost = ra.Count == rb.Count ? ra.Count(r => r.Pass) - rb.Count(r => r.Pass) : 0;
            if (rate + 1e-9 < goldenMin) fails.Add($"golden {t}: {rb.Count(r => r.Pass)}/{rb.Count} (baseline {was}); golden tasks need >= {Util.Pct(goldenMin)}");
            else if (lost > goldenDrop) fails.Add($"golden {t}: {rb.Count(r => r.Pass)}/{rb.Count} (baseline {was}); lost {lost} passes, at most {goldenDrop} allowed");
            else notes.Add($"golden {t}: {rb.Count(r => r.Pass)}/{rb.Count} (baseline {was})");
        }
        if (aggregateOnly) notes.Add("per-task golden checks disabled (--aggregate-only): only the aggregate is checked");
        else if (goldenTasks.Count == 0) notes.Add("no golden-tagged tasks in the candidate run");

        double ca = rows.Where(r => r.Config == a).Average(r => r.Cost), cb = rows.Where(r => r.Config == b).Average(r => r.Cost);
        if (ca > 0 && cb / ca > maxCost) fails.Add($"cost per trial rose {cb / ca:F2}x (${ca:F4} -> ${cb:F4}); ceiling {maxCost:F2}x");
        else if (ca > 0) notes.Add($"cost per trial {cb / ca:F2}x baseline");

        foreach (var n in notes) Console.WriteLine("ok    " + n);
        foreach (var f in fails) Console.WriteLine("FAIL  " + f);
        Console.WriteLine(fails.Count == 0 ? $"GATE PASSED: {b} may replace {a}" : $"GATE FAILED: {fails.Count} problem(s)");
        return fails.Count == 0 ? 0 : 1;
    }

    // ------------------------------------------------------------ leak

    public static int Leak(Opts o)
    {
        var ts = TaskSet.Load(o.P(0, "tasks.json"));
        var repo = Path.GetFullPath(o.P(1, "repo"));
        var files = Util.LayerFiles(repo);
        var layer = files.ToDictionary(f => f, f => Norm(File.ReadAllText(Path.Combine(repo, f))));
        Console.WriteLine($"Scanning {files.Count} AI-layer file(s) in {repo} for the {ts.Tasks.Count} tasks of {ts.Version}:");
        foreach (var f in files) Console.WriteLine("  " + f);
        int leaks = 0;
        foreach (var t in ts.Tasks)
        {
            var sh = Shingles(Norm(t.Prompt), 6);
            foreach (var (f, text) in layer)
            {
                var have = Shingles(text, 6);
                double share = sh.Count == 0 ? 0 : (double)sh.Count(have.Contains) / sh.Count;
                var refN = Norm(t.Reference);
                bool answer = t.Kind == "qa" && refN.Length >= 12 && text.Contains(refN);
                if (share >= 0.3) { leaks++; Console.WriteLine($"LEAK  {t.Id}: {share * 100:F0}% of the prompt's 6-word phrases appear in {f}{(answer ? ", together with the reference answer" : "")}"); }
                else if (answer && share >= 0.1) { leaks++; Console.WriteLine($"LEAK  {t.Id}: the reference answer sits next to phrases from the prompt in {f}"); }
                else if (answer) Console.WriteLine($"info  {t.Id}: the answer is stated as a fact in {f} (fine if the agent needs that fact; the task then tests retrieval, not inference)");
                if (System.Text.RegularExpressions.Regex.IsMatch(text, $@"\b{t.Id.ToLowerInvariant()}\b") && System.Text.RegularExpressions.Regex.IsMatch(text, @"\b(eval|task|grader|benchmark)s?\b"))
                    Console.WriteLine($"WARN  {t.Id}: the task id appears in {f} next to eval vocabulary");
            }
        }
        Console.WriteLine(leaks == 0 ? "no leaks found (this check only sees copied text: the holdout split is the real test)" : $"{leaks} leak(s): the eval now partly measures whether the agent can read its own answer key");
        return leaks == 0 ? 0 : 1;
    }

    static string Norm(string s) => System.Text.RegularExpressions.Regex.Replace(s.ToLowerInvariant(), @"[^a-z0-9_#]+", " ").Trim();

    static HashSet<string> Shingles(string s, int k)
    {
        var w = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var set = new HashSet<string>();
        for (int i = 0; i + k <= w.Length; i++) set.Add(string.Join(' ', w, i, k));
        return set;
    }

    // ------------------------------------------------------------ runs needed

    public static int RunsNeeded(Opts o)
    {
        double p = o.D("p", 0.7), h = o.D("halfwidth", 0.1);
        const double z = 1.96;
        int n = (int)Math.Ceiling(z * z * p * (1 - p) / (h * h));
        Console.WriteLine($"To estimate a pass rate near {Util.Pct(p)} to within +/-{h * 100:F0} pts (95%): about {n} independent trials (n = z^2 p(1-p) / h^2).");
        Console.WriteLine("Trials of the same task are not independent: spread them over many tasks, not many repeats of a few.\n");
        Console.WriteLine("| trials | 95% Wilson interval at this rate | width |");
        Console.WriteLine("|---|---|---|");
        foreach (var t in new[] { 5, 10, 20, 30, 50, 100, 200, 400 })
        {
            var (lo, hi) = Stat.Wilson((int)Math.Round(p * t), t);
            Console.WriteLine($"| {t} | {Util.Pct(lo)}-{Util.Pct(hi)} | {Util.Pct(hi - lo)} |");
        }
        if (o.Named.ContainsKey("delta"))
        {
            double d = o.D("delta", 0.15), p2 = Math.Min(0.999, p + d);
            int per = (int)Math.Ceiling(Math.Pow(1.96 + 0.84, 2) * (p * (1 - p) + p2 * (1 - p2)) / (d * d));
            Console.WriteLine($"\nTo detect {Util.Pct(p)} -> {Util.Pct(p2)} with 80% power (two independent arms, 95% two-sided): about {per} trials per arm.");
            Console.WriteLine("A paired design on the same tasks usually needs fewer, because task difficulty cancels out.");
        }
        return 0;
    }
}
