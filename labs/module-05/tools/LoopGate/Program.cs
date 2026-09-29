// LoopGate - Module 5 lab helper: deterministic gates for the research -> plan -> implement -> validate loop.
//   plan-lint <plan.md> --repo <dir>                         is the plan reviewable? (sections, evidence, boundaries, verify steps)
//   scope     <plan.md> --repo <dir> --base <dir>|--git <ref>  did the change stay inside the plan's boundaries?
//   arch      --repo <dir> --rules <file>                    does the code respect the architecture rules?
//   tests     --repo <dir> --min-tests <n>                   build + test; fails on red, skipped, or fewer tests than before
//   all       --repo <dir> --rules <file> --min-tests <n> [--plan <plan.md>] [--base <dir>|--git <ref>]
// No dependencies. It never modifies the repository.
using System.Diagnostics;
using System.Text.RegularExpressions;

Console.OutputEncoding = System.Text.Encoding.UTF8;
var opt = Args.Parse(args);
if (opt.Command is null) return Args.Usage();
try
{
    return opt.Command switch
    {
        "plan-lint" => Report.Print("plan-lint", PlanLint.Run(opt.Need(0, "plan"), opt.Repo)),
        "scope" => Report.Print("scope", Scope.Run(opt.Need(0, "plan"), opt.Repo, opt.Get("--base"), opt.Get("--git"))),
        "arch" => Report.Print("arch", Arch.Run(opt.Repo, opt.Get("--rules") ?? throw new ArgumentException("--rules is required"))),
        "tests" => Report.Print("tests", Tests.Run(opt.Repo, opt.MinTests)),
        "all" => All(opt),
        _ => Args.Usage()
    };
}
catch (ArgumentException e)
{
    Console.Error.WriteLine("error: " + e.Message);
    return 2;
}

static int All(Args o)
{
    var failed = 0;
    if (o.Get("--plan") is { } plan)
    {
        failed += Report.Print("plan-lint", PlanLint.Run(plan, o.Repo));
        if (o.Get("--base") is not null || o.Get("--git") is not null)
            failed += Report.Print("scope", Scope.Run(plan, o.Repo, o.Get("--base"), o.Get("--git")));
    }
    failed += Report.Print("arch", Arch.Run(o.Repo, o.Get("--rules") ?? throw new ArgumentException("--rules is required")));
    failed += Report.Print("tests", Tests.Run(o.Repo, o.MinTests));
    Console.WriteLine(failed == 0 ? "ALL GATES PASSED" : $"{failed} GATE(S) FAILED");
    return failed == 0 ? 0 : 1;
}

sealed class Args
{
    public string? Command;
    public List<string> Positional = new();
    public Dictionary<string, string> Named = new();

    public static Args Parse(string[] a)
    {
        var o = new Args { Command = a.Length > 0 ? a[0] : null };
        for (var i = 1; i < a.Length; i++)
        {
            if (a[i].StartsWith("--") && i + 1 < a.Length) o.Named[a[i]] = a[++i];
            else o.Positional.Add(a[i]);
        }
        return o;
    }

    /// <summary>Named option; paths are made absolute, except the git ref.</summary>
    public string? Get(string name) =>
        !Named.TryGetValue(name, out var v) ? null : name == "--git" ? v : Path.GetFullPath(v);
    public string Repo => Get("--repo") ?? throw new ArgumentException("--repo is required");
    public int MinTests => Named.TryGetValue("--min-tests", out var v) ? int.Parse(v) : 0;
    public string Need(int i, string what) =>
        Positional.Count > i ? Path.GetFullPath(Positional[i]) : throw new ArgumentException($"<{what}> is required");

    public static int Usage()
    {
        Console.Error.WriteLine("""
            usage: LoopGate plan-lint <plan.md> --repo <dir>
                   LoopGate scope <plan.md> --repo <dir> (--base <pristine-dir> | --git <ref>)
                   LoopGate arch --repo <dir> --rules <file>
                   LoopGate tests --repo <dir> [--min-tests <n>]
                   LoopGate all --repo <dir> --rules <file> [--min-tests <n>] [--plan <plan.md>] [--base <dir> | --git <ref>]
            """);
        return 2;
    }
}

static class Report
{
    public static int Print(string gate, List<string> problems)
    {
        foreach (var p in problems) Console.WriteLine($"  FAIL [{gate}] {p}");
        Console.WriteLine(problems.Count == 0 ? $"PASS {gate}" : $"FAIL {gate}: {problems.Count} problem(s)");
        return problems.Count == 0 ? 0 : 1;
    }
}

static class Paths
{
    static readonly string[] Skip = { "bin", "obj", ".git", ".vs", "TestResults", "node_modules" };

    public static string Rel(string root, string file) => Path.GetRelativePath(root, file).Replace('\\', '/');

    public static IEnumerable<string> Files(string root) =>
        Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Select(f => Rel(root, f))
            .Where(r => !r.Split('/').Any(part => Skip.Contains(part)));

    /// <summary>Glob with `**` (any depth), `*` (within a segment), `?` and `[1-4]` character classes.</summary>
    public static bool Match(string glob, string rel)
    {
        var rx = "^" + Regex.Escape(glob.Trim().TrimStart('/'))
            .Replace(@"\*\*/", "(?:.*/)?").Replace(@"\*\*", ".*").Replace(@"\*", "[^/]*").Replace(@"\?", "[^/]").Replace(@"\[", "[") + "$";
        return Regex.IsMatch(rel, rx, RegexOptions.IgnoreCase);
    }
}

/// <summary>A plan in the course template: `## Section` headings, backticked paths and globs.</summary>
sealed class Plan
{
    public Dictionary<string, List<string>> Sections = new(StringComparer.OrdinalIgnoreCase);

    public static Plan Load(string path)
    {
        var plan = new Plan();
        List<string>? current = null;
        foreach (var line in File.ReadAllLines(path))
        {
            var h = Regex.Match(line, @"^#{2,3}\s+(.+?)\s*$");
            if (h.Success) { current = new List<string>(); plan.Sections[h.Groups[1].Value] = current; continue; }
            current?.Add(line);
        }
        return plan;
    }

    public List<string> Lines(string section) => Sections.TryGetValue(section, out var l) ? l : new List<string>();
    public List<string> Bullets(string section) =>
        Lines(section).Where(l => Regex.IsMatch(l, @"^\s*(?:[-*]|\d+\.)\s+")).ToList();
    public static IEnumerable<string> Ticks(string line) =>
        Regex.Matches(line, "`([^`]+)`").Select(m => m.Groups[1].Value);
    public List<string> Globs(string section) => Bullets(section).SelectMany(Ticks).ToList();
}

static class PlanLint
{
    static readonly string[] Required = { "Goal", "Evidence", "Approach", "Touch", "Do not touch", "Steps", "Checkpoints", "Validation" };

    public static List<string> Run(string planPath, string repo)
    {
        var p = new List<string>();
        var plan = Plan.Load(planPath);
        foreach (var s in Required)
            if (!plan.Sections.ContainsKey(s)) p.Add($"missing section '{s}'");
            else if (plan.Lines(s).All(string.IsNullOrWhiteSpace)) p.Add($"section '{s}' is empty");

        // Evidence: every backticked path must exist today.
        foreach (var t in plan.Bullets("Evidence").SelectMany(Plan.Ticks).Where(LooksLikePath))
            if (!Exists(repo, t)) p.Add($"Evidence cites `{t}`, which does not exist in the repository");
        if (plan.Bullets("Evidence").Count(b => Plan.Ticks(b).Any(LooksLikePath)) < 2)
            p.Add("Evidence lists fewer than 2 files; research that read one file is not research");

        // Approach: every design decision names its evidence, and the evidence exists.
        foreach (var b in plan.Bullets("Approach"))
        {
            var ev = Regex.Match(b, @"\(ev:\s*([^)]*)\)");
            if (!ev.Success) { p.Add($"Approach decision has no (ev: ...): {b.Trim()}"); continue; }
            foreach (var t in Plan.Ticks(ev.Groups[1].Value).Where(LooksLikePath))
                if (!Exists(repo, t)) p.Add($"Approach cites `{t}`, which does not exist");
        }

        if (plan.Globs("Touch").Count == 0) p.Add("Touch lists no backticked paths or globs");
        if (plan.Globs("Do not touch").Count == 0) p.Add("Do not touch lists no backticked paths or globs");
        foreach (var s in plan.Bullets("Steps").Where(s => !s.Contains("verify:", StringComparison.OrdinalIgnoreCase)))
            p.Add($"Step has no 'verify:' clause: {s.Trim()}");
        if (!plan.Lines("Checkpoints").Any(l => l.Contains("HUMAN", StringComparison.Ordinal)))
            p.Add("Checkpoints has no HUMAN checkpoint");
        if (!plan.Lines("Validation").SelectMany(Plan.Ticks).Any(t => t.StartsWith("dotnet ") || t.Contains("LoopGate")))
            p.Add("Validation names no runnable command");
        return p;
    }

    static bool LooksLikePath(string t) => !t.Contains(' ') && (t.Contains('/') || Regex.IsMatch(t, @"\.(cs|csproj|sln|sql|md|json)$"));
    static bool Exists(string repo, string t) => File.Exists(Path.Combine(repo, t)) || Directory.Exists(Path.Combine(repo, t));
}

static class Scope
{
    static readonly string[] LoopArtifacts = { "plans/**", "research/**", "NOTES.md" };

    public static List<string> Run(string planPath, string repo, string? baseDir, string? gitRef)
    {
        var plan = Plan.Load(planPath);
        var touch = plan.Globs("Touch");
        var never = plan.Globs("Do not touch");
        var all = baseDir is not null ? DiffDirs(baseDir, repo) : DiffGit(repo, gitRef ?? throw new ArgumentException("--base or --git is required"));
        // The loop's own artifacts (plan, research brief, notes log) travel with the change but are not code.
        var changed = all.Where(f => !LoopArtifacts.Any(g => Paths.Match(g, f))).ToList();
        var p = new List<string>();
        foreach (var f in changed)
        {
            var hit = never.FirstOrDefault(g => Paths.Match(g, f));
            if (hit is not null) p.Add($"{f} changed, but the plan says do not touch `{hit}`");
            else if (!touch.Any(g => Paths.Match(g, f))) p.Add($"{f} changed, but it is not in the plan's Touch list");
        }
        Console.WriteLine($"  scope: {changed.Count} changed file(s): {string.Join(", ", changed)}");
        return p;
    }

    static List<string> DiffDirs(string a, string b)
    {
        var left = Paths.Files(a).ToHashSet();
        var right = Paths.Files(b).ToHashSet();
        return left.Union(right)
            .Where(f => !left.Contains(f) || !right.Contains(f) ||
                        File.ReadAllText(Path.Combine(a, f)).ReplaceLineEndings() != File.ReadAllText(Path.Combine(b, f)).ReplaceLineEndings())
            .OrderBy(f => f).ToList();
    }

    static List<string> DiffGit(string repo, string gitRef)
    {
        var (code, output) = Proc.Run("git", $"diff --name-only {gitRef} --", repo, stdoutOnly: true);
        if (code != 0) throw new ArgumentException("git diff failed: " + output.Trim());
        var (_, untracked) = Proc.Run("git", "ls-files --others --exclude-standard", repo, stdoutOnly: true);
        return (output + "\n" + untracked).Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct().OrderBy(f => f).ToList();
    }
}

static class Arch
{
    // Rules file: one rule per line, `kind | pattern | include-glob | exclude-glob | reason`. `#` starts a comment.
    //   forbid-text     literal text that must not appear in included files (outside excluded ones)
    //   forbid-package  regex for a PackageReference Include in any *.csproj
    //   migration-pair  from migration number <pattern> on, every V###__x.sql in <include> needs U###__x.sql
    public static List<string> Run(string repo, string rulesPath)
    {
        var p = new List<string>();
        var files = Paths.Files(repo).ToList();
        foreach (var raw in File.ReadAllLines(rulesPath))
        {
            // Only whole-line comments: '#' inside a rule (regexes, "V###") is data.
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;
            var c = line.Split('|').Select(x => x.Trim()).ToArray();
            if (c.Length != 5) throw new ArgumentException($"bad rule (need 5 fields): {raw}");
            var (kind, pattern, include, exclude, reason) = (c[0], c[1], c[2], c[3], c[4]);
            switch (kind)
            {
                case "forbid-text":
                    foreach (var f in files.Where(f => Paths.Match(include, f) && (exclude.Length == 0 || !Paths.Match(exclude, f))))
                    {
                        var lines = File.ReadAllLines(Path.Combine(repo, f));
                        for (var i = 0; i < lines.Length; i++)
                            if (lines[i].Contains(pattern, StringComparison.Ordinal) && !lines[i].TrimStart().StartsWith("//"))
                                p.Add($"{f}:{i + 1} uses `{pattern}` ({reason})");
                    }
                    break;
                case "forbid-package":
                    foreach (var f in files.Where(f => f.EndsWith(".csproj")))
                        foreach (Match m in Regex.Matches(File.ReadAllText(Path.Combine(repo, f)), @"PackageReference\s+Include=""([^""]+)"""))
                            if (Regex.IsMatch(m.Groups[1].Value, pattern))
                                p.Add($"{f} references package {m.Groups[1].Value} ({reason})");
                    break;
                case "migration-pair":
                    var from = int.Parse(pattern);
                    foreach (var f in files.Where(f => Paths.Match(include + "/V*.sql", f)))
                    {
                        var m = Regex.Match(Path.GetFileName(f), @"^V(\d{3})__(.+)\.sql$");
                        if (!m.Success || int.Parse(m.Groups[1].Value) < from) continue;
                        var undo = $"{include}/U{m.Groups[1].Value}__{m.Groups[2].Value}.sql";
                        if (!files.Contains(undo)) p.Add($"{f} has no undo script {undo} ({reason})");
                    }
                    break;
                default:
                    throw new ArgumentException($"unknown rule kind '{kind}'");
            }
        }
        return p;
    }
}

static class Tests
{
    public static List<string> Run(string repo, int minTests)
    {
        var p = new List<string>();
        var sln = Directory.EnumerateFiles(repo, "*.sln").FirstOrDefault()
                  ?? throw new ArgumentException($"no .sln file in {repo}");
        var (buildCode, buildOut) = Proc.Run("dotnet", $"build \"{sln}\" --nologo -v q", repo);
        if (buildCode != 0)
        {
            p.Add("build failed:\n" + string.Join("\n", buildOut.Split('\n').Where(l => l.Contains("error")).Take(10)));
            return p;
        }
        var (testCode, testOut) = Proc.Run("dotnet", $"test \"{sln}\" --no-build --nologo -v q", repo);
        int failed = 0, passed = 0, skipped = 0, total = 0;
        foreach (Match m in Regex.Matches(testOut, @"Failed:\s*(\d+),\s*Passed:\s*(\d+),\s*Skipped:\s*(\d+),\s*Total:\s*(\d+)"))
        {
            failed += int.Parse(m.Groups[1].Value); passed += int.Parse(m.Groups[2].Value);
            skipped += int.Parse(m.Groups[3].Value); total += int.Parse(m.Groups[4].Value);
        }
        Console.WriteLine($"  tests: total {total}, passed {passed}, failed {failed}, skipped {skipped} (minimum {minTests})");
        if (total == 0) p.Add("no test results found in `dotnet test` output (exit code " + testCode + ")");
        if (failed > 0 || testCode != 0) p.Add($"{failed} test(s) failed");
        if (skipped > 0) p.Add($"{skipped} test(s) skipped; a skipped test is a hidden failure until someone justifies it");
        if (total < minTests) p.Add($"only {total} tests ran, fewer than the {minTests} that existed before the change");
        return p;
    }
}

static class Proc
{
    /// <summary>Runs a process; Output is stdout, plus stderr unless stdoutOnly.</summary>
    public static (int Code, string Output) Run(string file, string arguments, string cwd, bool stdoutOnly = false)
    {
        var psi = new ProcessStartInfo(file, arguments)
        {
            WorkingDirectory = cwd, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false
        };
        using var proc = Process.Start(psi)!;
        var stdout = proc.StandardOutput.ReadToEndAsync();
        var stderr = proc.StandardError.ReadToEndAsync();
        proc.WaitForExit();
        return (proc.ExitCode, stdoutOnly ? stdout.Result : stdout.Result + stderr.Result);
    }
}
