// ContextLab - Module 4 lab helper. Read-only: it never modifies the repository.
//   budget  <repo> [options]            what loads when, in tokens, and what is left of the window
//   audit   <repo>                      pathology leads: redundancy, contradiction, stale, irrelevant, generic, size
//   prompts <tasks.json> [--only ids]   prints "id<TAB>prompt" lines for the run-tasks scripts
//   grade   <tasks.json> <runs-dir> [--label L] [--out results.csv] [--rules-tokens N]
//   report  <results.csv>               before/after table from the rows that grade appended
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.ML.Tokenizers;

Console.OutputEncoding = System.Text.Encoding.UTF8;
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

if (args.Length < 2) return Usage();
var opts = Opts.Parse(args.Skip(1).ToArray());
return args[0] switch
{
    "budget" => Budget.Run(Path.GetFullPath(opts.Pos(0)), opts),
    "audit" => Audit.Run(Path.GetFullPath(opts.Pos(0))),
    "prompts" => Tasks.Prompts(opts.Pos(0), opts.Str("only", "")),
    "grade" when opts.Positional.Count >= 2 => Tasks.Grade(opts.Pos(0), opts.Pos(1), opts),
    "report" => Tasks.Report(opts.Pos(0)),
    _ => Usage()
};

static int Usage()
{
    Console.Error.WriteLine("""
        usage:
          ContextLab budget <repo> [--agent claude|agents] [--window 200000] [--system 4200] [--tools 1000]
                                   [--history 0] [--reserve 20000] [--price 3.00] [--turns 10] [--sessions 1440]
          ContextLab audit <repo>
          ContextLab prompts <tasks.json> [--only T04,T05]
          ContextLab grade <tasks.json> <runs-dir> [--label before] [--out results.csv] [--rules-tokens N]
          ContextLab report <results.csv>
        """);
    return 2;
}

sealed class Opts
{
    public List<string> Positional { get; } = new();
    public Dictionary<string, string> Named { get; } = new();
    public static Opts Parse(string[] a)
    {
        var o = new Opts();
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i].StartsWith("--") && i + 1 < a.Length) o.Named[a[i][2..]] = a[++i];
            else o.Positional.Add(a[i]);
        }
        return o;
    }
    public string Pos(int i) => i < Positional.Count ? Positional[i] : throw new ArgumentException("missing argument");
    public double Num(string k, double d) => Named.TryGetValue(k, out var v) ? double.Parse(v, CultureInfo.InvariantCulture) : d;
    public string Str(string k, string d) => Named.TryGetValue(k, out var v) ? v : d;
}

static class Tok
{
    static readonly Tokenizer O200k = TiktokenTokenizer.CreateForEncoding("o200k_base");
    public static int Count(string s) => O200k.CountTokens(s);
}

/// <summary>One instruction file and when an agent loads it.</summary>
sealed record CtxFile(string Rel, string Full, string Load, string Why, string Text)
{
    public int Lines => Text.Length == 0 ? 0 : Text.TrimEnd('\n').Split('\n').Length;
    public int Tokens => Tok.Count(Text);
}

static class Layer
{
    static readonly string[] Skip = { "bin", "obj", ".git", ".vs", "node_modules" };

    public static string Rel(string root, string p) => Path.GetRelativePath(root, p).Replace('\\', '/');

    static IEnumerable<string> AllFiles(string root) =>
        Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Where(f => !Rel(root, f).Split('/').Any(part => Skip.Contains(part)));

    static string Read(string f) => File.ReadAllText(f).Replace("\r\n", "\n");

    /// <summary>Discovers instruction files the way Claude Code (or an AGENTS.md agent) loads them, as of 2026-09.</summary>
    public static List<CtxFile> Discover(string root, string agent)
    {
        var result = new List<CtxFile>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void Add(string full, string load, string why)
        {
            if (!File.Exists(full) || !seen.Add(Path.GetFullPath(full))) return;
            var text = Read(full);
            result.Add(new CtxFile(Rel(root, full), full, load, why, text));
            if (agent == "claude" && load == "always") foreach (var imp in Imports(full, text)) Add(imp, "always", $"@import in {Rel(root, full)}");
        }

        var files = AllFiles(root).ToList();
        if (agent == "claude")
        {
            var roots = new[] { "CLAUDE.md", ".claude/CLAUDE.md", "CLAUDE.local.md" }.Select(p => Path.Combine(root, p)).Where(File.Exists).ToList();
            if (roots.Count == 0) roots.Add(Path.Combine(root, "AGENTS.md"));
            foreach (var r in roots) Add(r, "always", "root instructions");
            foreach (var rule in files.Where(f => Rel(root, f).StartsWith(".claude/rules/") && f.EndsWith(".md")).OrderBy(f => f))
            {
                var paths = Regex.Match(Read(rule), @"\A---\n(?<fm>.*?)\n---", RegexOptions.Singleline).Groups["fm"].Value;
                var globs = Regex.Matches(paths, @"^\s*-\s*[""']?([^""'\n]+)", RegexOptions.Multiline).Select(m => m.Groups[1].Value.Trim()).ToList();
                if (paths.Contains("paths:")) Add(rule, "on-demand", "paths: " + string.Join(", ", globs));
                else Add(rule, "always", "rule without paths:");
            }
            foreach (var skill in files.Where(f => Path.GetFileName(f) == "SKILL.md").OrderBy(f => f))
                Add(skill, "on-demand", "skill body (only its description loads at start)");
        }
        else
        {
            Add(Path.Combine(root, "AGENTS.md"), "always", "root AGENTS.md");
        }
        foreach (var nested in files.Where(f => (Path.GetFileName(f) is "CLAUDE.md" or "AGENTS.md") && Path.GetDirectoryName(Rel(root, f)) is { Length: > 0 } d && !d.StartsWith(".claude")).OrderBy(f => f))
        {
            if (agent != "claude" && Path.GetFileName(nested) != "AGENTS.md") continue;
            Add(nested, "on-demand", $"nested; loads when files in {Path.GetDirectoryName(Rel(root, nested))!.Replace('\\', '/')}/ are read");
        }
        if (agent == "claude" && !result.Any(r => r.Rel == "AGENTS.md") && File.Exists(Path.Combine(root, "AGENTS.md")))
            result.Add(new CtxFile("AGENTS.md", Path.Combine(root, "AGENTS.md"), "not loaded", "CLAUDE.md exists and does not import it", Read(Path.Combine(root, "AGENTS.md"))));
        return result;
    }

    /// <summary>@path imports outside code fences and inline code.</summary>
    static IEnumerable<string> Imports(string full, string text)
    {
        var noCode = Regex.Replace(text, @"```.*?```", "", RegexOptions.Singleline);
        noCode = Regex.Replace(noCode, @"`[^`\n]*`", "");
        foreach (Match m in Regex.Matches(noCode, @"(?<![\w@])@([\w./\\-]+\.\w+)"))
            yield return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(full)!, m.Groups[1].Value));
    }
}

static class Budget
{
    public static int Run(string root, Opts o)
    {
        if (!Directory.Exists(root)) { Console.Error.WriteLine($"not a directory: {root}"); return 2; }
        var agent = o.Str("agent", "claude");
        var files = Layer.Discover(root, agent);
        Console.WriteLine($"Context layer of {Path.GetFileName(root)} as loaded by: {agent}");
        Console.WriteLine("Tokens are o200k_base counts, a proxy: Claude's tokenizer gives different numbers (use the count_tokens endpoint from 02.1 for exact ones).\n");
        Console.WriteLine($"{"LOAD",-11} {"LINES",6} {"TOKENS",7}  FILE");
        foreach (var f in files.OrderBy(f => f.Load == "always" ? 0 : f.Load == "on-demand" ? 1 : 2))
            Console.WriteLine($"{f.Load,-11} {f.Lines,6} {f.Tokens,7:N0}  {f.Rel}  ({f.Why})");

        var always = files.Where(f => f.Load == "always").ToList();
        int R = always.Sum(f => f.Tokens), lines = always.Sum(f => f.Lines);
        var onDemand = files.Where(f => f.Load == "on-demand").ToList();
        Console.WriteLine($"\nAlways-loaded layer R: {lines:N0} lines, {R:N0} tokens");
        Console.WriteLine($"On-demand (only when triggered): {onDemand.Sum(f => f.Lines):N0} lines, {onDemand.Sum(f => f.Tokens):N0} tokens");

        double W = o.Num("window", 200_000), S = o.Num("system", 4_200), T = o.Num("tools", 1_000),
               H = o.Num("history", 0), O = o.Num("reserve", 20_000), price = o.Num("price", 3.00),
               turns = o.Num("turns", 10), sessions = o.Num("sessions", 1_440);
        double usable = W - S - R - T - H - O;
        Console.WriteLine("\nBudget per request = W - S - R - T - H - O");
        Console.WriteLine($"  W  context window          {W,10:N0}");
        Console.WriteLine($"  S  system prompt           {S,10:N0}");
        Console.WriteLine($"  R  rules / memory files    {R,10:N0}");
        Console.WriteLine($"  T  tool definitions        {T,10:N0}");
        Console.WriteLine($"  H  conversation history    {H,10:N0}");
        Console.WriteLine($"  O  output reserve          {O,10:N0}");
        Console.WriteLine($"  =  left for code, tool results, reasoning: {usable:N0} ({usable / W * 100:F1}% of W)");

        double monthly = R * turns * sessions;
        double uncached = monthly / 1e6 * price;
        double cached = sessions * (R * 1.25 + R * 0.10 * (turns - 1)) / 1e6 * price;
        Console.WriteLine($"\nR is re-sent on every request: {turns} requests/session x {sessions:N0} sessions/month = {monthly / 1e6:N1}M tokens/month");
        Console.WriteLine($"  uncached at ${price:N2}/M input: ${uncached:N2}/month");
        Console.WriteLine($"  with a warm prompt cache (1.25x write once, 0.1x reads; multipliers as of 2026-09): ${cached:N2}/month");
        if (lines > 300) Console.WriteLine($"\nOVER BUDGET: always-loaded layer is {lines} lines (course cap 300, warning at 200).");
        return 0;
    }
}

static class Audit
{
    static readonly Regex Neg = new(@"\b(never|don't|do not|avoid|must not|no longer|not allowed|forbidden|deprecated|instead of|stop using)\b", RegexOptions.IgnoreCase);
    static readonly Regex Pos = new(@"\b(use|uses|always|must|goes through|go through|prefer|should|is done with|comes from)\b", RegexOptions.IgnoreCase);
    static readonly Regex Generic = new(@"\b(clean code|clean, maintainable|SOLID|best practices|step by step|be careful|senior|world-class|expert|high[- ]quality|think carefully|readable code)\b", RegexOptions.IgnoreCase);

    // keyword -> evidence that would make it relevant (file extensions / names, or the word appearing in code)
    static readonly (string Word, string[] Evidence)[] Tech =
    {
        ("React", new[] { ".tsx", ".jsx", "package.json" }), ("Angular", new[] { "angular.json" }), ("npm", new[] { "package.json" }),
        ("TypeScript", new[] { ".ts", ".tsx" }), ("Kubernetes", new[] { "Chart.yaml", ".yaml" }), ("Helm", new[] { "Chart.yaml" }),
        ("kubectl", new[] { "Chart.yaml" }), ("Terraform", new[] { ".tf" }), ("Python", new[] { ".py" }), ("pytest", new[] { ".py" }),
        ("Java", new[] { ".java", "pom.xml" }), ("Maven", new[] { "pom.xml" }), ("Gradle", new[] { "build.gradle" }),
        ("MongoDB", Array.Empty<string>()), ("Redis", Array.Empty<string>()), ("RabbitMQ", Array.Empty<string>()), ("Kafka", Array.Empty<string>()),
        ("GraphQL", Array.Empty<string>()), ("Entity Framework", Array.Empty<string>()), ("EF Core", Array.Empty<string>()),
        ("Blazor", new[] { ".razor" }), ("Storybook", new[] { "package.json" }), ("Tailwind", new[] { "package.json" }),
    };

    sealed record Line(string File, int No, string Text, bool Loaded);

    public static int Run(string root)
    {
        if (!Directory.Exists(root)) { Console.Error.WriteLine($"not a directory: {root}"); return 2; }
        var ctx = Layer.Discover(root, "claude");
        var allFiles = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Select(f => Layer.Rel(root, f)).Where(r => !r.Split('/').Any(p => p is "bin" or "obj" or ".git")).ToList();
        var codeFiles = allFiles.Where(r => Regex.IsMatch(r, @"\.(cs|sql|csproj|sln|json)$")).ToList();
        var code = string.Join("\n", codeFiles.Select(r => File.ReadAllText(Path.Combine(root, r))));
        var obsolete = Regex.Matches(code, @"\[Obsolete[^\]]*\]\s*(?:(?:public|internal|static|sealed)\s+)*class\s+(\w+)").Select(m => m.Groups[1].Value).ToHashSet();

        var lines = new List<Line>();
        foreach (var f in ctx)
        {
            bool inFence = false;
            var raw = f.Text.Split('\n');
            for (int i = 0; i < raw.Length; i++)
            {
                var t = raw[i].Trim();
                if (t.StartsWith("```")) { inFence = !inFence; continue; }
                if (inFence || t.Length == 0 || t.StartsWith('#') || t.StartsWith("---") || t.StartsWith('|') && t.Contains("---")) continue;
                lines.Add(new Line(f.Rel, i + 1, t, f.Load != "not loaded"));
            }
        }

        var findings = new Dictionary<string, List<string>>
        {
            ["redundancy"] = new(), ["contradiction"] = new(), ["stale"] = new(), ["irrelevant"] = new(), ["generic"] = new(), ["size"] = new()
        };

        // Redundancy: exact duplicates after normalisation, then near-duplicates by word overlap.
        static string Norm(string s) => Regex.Replace(Regex.Replace(s.ToLowerInvariant(), @"^[-*\d.\s]+", ""), @"[^\w\s]", " ").Trim();
        static HashSet<string> Words(string s) => Norm(s).Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(w => w.Length > 2).ToHashSet();
        var candidates = lines.Where(l => Words(l.Text).Count >= 3).ToList();
        var reported = new HashSet<int>();
        for (int i = 0; i < candidates.Count; i++)
        {
            if (reported.Contains(i)) continue;
            var wi = Words(candidates[i].Text);
            var dups = new List<Line>();
            for (int j = i + 1; j < candidates.Count; j++)
            {
                if (reported.Contains(j)) continue;
                var wj = Words(candidates[j].Text);
                double jac = (double)wi.Intersect(wj).Count() / wi.Union(wj).Count();
                bool shortLine = Math.Min(wi.Count, wj.Count) < 5;
                if (shortLine ? jac == 1.0 : jac >= 0.7) { dups.Add(candidates[j]); reported.Add(j); }
            }
            if (dups.Count > 0)
                findings["redundancy"].Add($"{candidates[i].File}:{candidates[i].No} repeated {dups.Count}x: {string.Join(", ", dups.Select(d => $"{d.File}:{d.No}"))}  \"{Short(candidates[i].Text)}\"");
        }

        // Contradiction: the same backticked name recommended in one line and forbidden in another.
        var polarity = new Dictionary<string, List<(Line L, bool Positive)>>();
        foreach (var l in lines)
        {
            foreach (var clause in Regex.Split(l.Text, @"(?<=[;.:])\s+|\s+(?:but|except)\s+"))
            {
                foreach (Match m in Regex.Matches(clause, @"`([^`]+)`"))
                {
                    var key = Regex.Match(m.Groups[1].Value, @"^[^.(]+").Value.Trim();
                    if (key.Length < 3) continue;
                    // only the text since the previous backticked token counts as "before" this one
                    var before = clause[..m.Index];
                    var lastTick = before.LastIndexOf('`');
                    if (lastTick >= 0) before = before[(lastTick + 1)..];
                    bool? pol = Neg.IsMatch(before) ? false : Pos.IsMatch(clause) && !Neg.IsMatch(clause) ? true : null;
                    if (pol is null) continue;
                    if (!polarity.TryGetValue(key, out var list)) polarity[key] = list = new();
                    list.Add((l, pol.Value));
                }
            }
        }
        foreach (var (key, list) in polarity)
        {
            var p = list.FirstOrDefault(x => x.Positive); var n = list.FirstOrDefault(x => !x.Positive);
            if (p.L is not null && n.L is not null && p.L != n.L)
                findings["contradiction"].Add($"`{key}`: recommended at {p.L.File}:{p.L.No} \"{Short(p.L.Text)}\"  vs  forbidden at {n.L.File}:{n.L.No} \"{Short(n.L.Text)}\"");
        }

        // Stale: backticked paths and solutions that do not exist, symbols absent from code, [Obsolete] types recommended.
        foreach (var l in lines)
        {
            foreach (Match m in Regex.Matches(l.Text, @"`([^`]+)`"))
            {
                var tok = m.Groups[1].Value.Trim();
                string? problem = null;
                if (allFiles.Any(f => Path.GetFileName(f) == tok) || Regex.IsMatch(tok, @"^(feature|bugfix|hotfix|release)/")) continue;
                var sln = Regex.Match(tok, @"^dotnet \w+\s+(\S+\.(?:sln|slnx|csproj))");
                if (sln.Success) { if (!allFiles.Any(f => f == sln.Groups[1].Value || f.EndsWith("/" + sln.Groups[1].Value))) problem = $"`{sln.Groups[1].Value}` does not exist"; }
                else if (!tok.Contains(' ') && !tok.Contains('*') && !tok.Contains('#') && (tok.Contains('/') || Regex.IsMatch(tok, @"\.(md|cs|sql|sln|csproj|json|txt|yml|yaml)$")))
                { if (!allFiles.Any(f => f == tok.TrimEnd('/') || f.StartsWith(tok.TrimEnd('/') + "/") || f.EndsWith("/" + tok))) problem = $"path `{tok}` does not exist"; }
                else if (Regex.IsMatch(tok, @"^[A-Z][A-Za-z0-9_]*(\.[A-Z][A-Za-z0-9_]*)*$") && tok.Length > 3)
                {
                    var first = tok.Split('.')[0];
                    if (!Regex.IsMatch(code, $@"\b{Regex.Escape(first)}\b")) problem = $"`{tok}` not found in repo code (stale, invented, or a framework name?)";
                    else if (obsolete.Contains(first) && !Neg.IsMatch(l.Text)) problem = $"`{tok}` is [Obsolete] but recommended here";
                }
                if (problem is not null) findings["stale"].Add($"{l.File}:{l.No} {problem}  \"{Short(l.Text)}\"");
            }
        }

        // Irrelevant: technology named in the rules with no trace of it in the repository.
        foreach (var l in lines)
        {
            foreach (var (word, ev) in Tech)
            {
                if (!Regex.IsMatch(l.Text, $@"\b{Regex.Escape(word)}\b")) continue;
                bool present = allFiles.Any(f => ev.Any(e => e.StartsWith('.') ? f.EndsWith(e) : Path.GetFileName(f) == e))
                               || Regex.IsMatch(code, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase);
                if (!present) { findings["irrelevant"].Add($"{l.File}:{l.No} mentions {word}; no trace of it in the repo  \"{Short(l.Text)}\""); break; }
            }
        }

        // Generic: advice with nothing the agent can act on differently.
        foreach (var l in lines.Where(l => Generic.IsMatch(l.Text) && !l.Text.Contains('`')))
            findings["generic"].Add($"{l.File}:{l.No} \"{Short(l.Text)}\"");

        // Size.
        var always = ctx.Where(f => f.Load == "always").ToList();
        foreach (var f in always.Where(f => f.Lines > 200)) findings["size"].Add($"{f.Rel}: {f.Lines} lines (warning above 200 per file)");
        int total = always.Sum(f => f.Lines);
        if (total > 300) findings["size"].Add($"always-loaded layer: {total} lines, {always.Sum(f => f.Tokens):N0} tokens (course cap 300 lines)");
        var notLoaded = ctx.Where(f => f.Load == "not loaded").ToList();
        foreach (var f in notLoaded) findings["contradiction"].Add($"{f.Rel} exists but Claude Code does not load it ({f.Why}); other agents read it instead");

        Console.WriteLine($"Context audit leads for {Path.GetFileName(root)} ({ctx.Count} instruction files, {lines.Count} content lines)");
        Console.WriteLine("Leads, not verdicts: confirm each against the code, then decide keep / move / delete.\n");
        foreach (var (k, v) in findings)
        {
            Console.WriteLine($"## {k} ({v.Count})");
            foreach (var s in v.Take(40)) Console.WriteLine("- " + s);
            if (v.Count > 40) Console.WriteLine($"- ... {v.Count - 40} more");
            Console.WriteLine();
        }
        Console.WriteLine(string.Join(" | ", findings.Select(kv => $"{kv.Value.Count} {kv.Key}")));
        return findings.Values.Sum(v => v.Count) == 0 ? 0 : 1;
    }

    static string Short(string s) => s.Length <= 70 ? s : s[..67] + "...";
}

sealed record TaskDef(string Id, string Fact, string Prompt, string[] Must, string[] MustNot);

static class Tasks
{
    static List<TaskDef> Load(string path)
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(path));
        return doc.RootElement.GetProperty("tasks").EnumerateArray().Select(t => new TaskDef(
            t.GetProperty("id").GetString()!,
            t.TryGetProperty("fact", out var f) ? f.GetString()! : "",
            t.GetProperty("prompt").GetString()!,
            t.GetProperty("must").EnumerateArray().Select(x => x.GetString()!).ToArray(),
            t.TryGetProperty("mustNot", out var n) ? n.EnumerateArray().Select(x => x.GetString()!).ToArray() : Array.Empty<string>())).ToList();
    }

    public static int Prompts(string path, string only)
    {
        var ids = only.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToHashSet();
        foreach (var t in Load(path).Where(t => ids.Count == 0 || ids.Contains(t.Id)))
            Console.WriteLine($"{t.Id}\t{t.Prompt.Replace('\n', ' ')}");
        return 0;
    }

    sealed record Run(string File, string Text, bool IsError, double Cost, long InputTokens, long OutputTokens);

    static Run ReadRun(string f)
    {
        var raw = File.ReadAllText(f);
        if (!f.EndsWith(".json")) return new Run(f, raw, false, 0, 0, 0);
        try
        {
            using var doc = JsonDocument.Parse(raw);
            var r = doc.RootElement;
            string text = r.TryGetProperty("result", out var res) && res.ValueKind == JsonValueKind.String ? res.GetString()! : "";
            bool isError = r.TryGetProperty("is_error", out var e) && e.ValueKind == JsonValueKind.True;
            double cost = r.TryGetProperty("total_cost_usd", out var c) && c.ValueKind == JsonValueKind.Number ? c.GetDouble() : 0;
            long input = 0, output = 0;
            if (r.TryGetProperty("usage", out var u) && u.ValueKind == JsonValueKind.Object)
            {
                foreach (var k in new[] { "input_tokens", "cache_read_input_tokens", "cache_creation_input_tokens" })
                    if (u.TryGetProperty(k, out var v) && v.ValueKind == JsonValueKind.Number) input += v.GetInt64();
                if (u.TryGetProperty("output_tokens", out var ot) && ot.ValueKind == JsonValueKind.Number) output = ot.GetInt64();
            }
            return new Run(f, text, isError, cost, input, output);
        }
        catch (JsonException) { return new Run(f, raw, true, 0, 0, 0); }
    }

    public static int Grade(string tasksPath, string runsDir, Opts o)
    {
        var tasks = Load(tasksPath);
        if (!Directory.Exists(runsDir)) { Console.Error.WriteLine($"not a directory: {runsDir}"); return 2; }
        var files = Directory.EnumerateFiles(runsDir).Where(f => Regex.IsMatch(f, @"\.(json|txt|md)$")).OrderBy(f => f).ToList();
        int runs = 0, passes = 0; double cost = 0; long input = 0;
        Console.WriteLine($"{"TASK",-5} {"FACT",-4} {"PASS",6}  FIRST FAILURE");
        foreach (var t in tasks)
        {
            var mine = files.Where(f => Path.GetFileName(f).StartsWith(t.Id + ".") || Path.GetFileNameWithoutExtension(f) == t.Id).Select(ReadRun).ToList();
            int ok = 0; string? firstFail = null;
            foreach (var r in mine)
            {
                var why = r.IsError ? "run reported an error"
                    : t.Must.Where(p => !Regex.IsMatch(r.Text, p, RegexOptions.IgnoreCase | RegexOptions.Multiline)).Select(p => $"missing /{p}/").FirstOrDefault()
                      ?? t.MustNot.Where(p => Regex.IsMatch(r.Text, p, RegexOptions.IgnoreCase | RegexOptions.Multiline)).Select(p => $"contains /{p}/").FirstOrDefault();
                if (why is null) ok++; else firstFail ??= $"{Path.GetFileName(r.File)}: {why}";
                cost += r.Cost; input += r.InputTokens;
            }
            runs += mine.Count; passes += ok;
            if (mine.Count == 0) { Console.WriteLine($"{t.Id,-5} {t.Fact,-4} {"-",6}  not run"); continue; }
            Console.WriteLine($"{t.Id,-5} {t.Fact,-4} {ok,2}/{mine.Count,-3}  {(mine.Count == 0 ? "no runs found" : firstFail ?? "")}");
        }
        if (runs == 0) { Console.Error.WriteLine($"no run files in {runsDir}"); return 2; }
        double rate = (double)passes / runs;
        Console.WriteLine($"\npass rate {passes}/{runs} = {rate * 100:F1}%");
        if (input > 0) Console.WriteLine($"mean input tokens per run {input / runs:N0} (input + cache read + cache write)");
        if (cost > 0) Console.WriteLine($"total cost ${cost:N4} | mean ${cost / runs:N4}/run | cost per passing answer {(passes == 0 ? "n/a" : "$" + (cost / passes).ToString("N4"))}");
        if (o.Named.TryGetValue("out", out var outCsv))
        {
            bool header = !File.Exists(outCsv);
            using var w = new StreamWriter(outCsv, append: true);
            if (header) w.WriteLine("label,date,rules_tokens,runs,passes,pass_rate,mean_input_tokens,mean_cost_usd,cost_per_pass_usd");
            w.WriteLine(string.Join(",", o.Str("label", "run"), DateTime.UtcNow.ToString("yyyy-MM-dd"), o.Str("rules-tokens", ""),
                runs, passes, rate.ToString("F3"), runs == 0 ? 0 : input / runs, (cost / runs).ToString("F4"), passes == 0 ? "" : (cost / passes).ToString("F4")));
            Console.WriteLine($"appended to {outCsv}");
        }
        return passes == runs ? 0 : 1;
    }

    public static int Report(string csv)
    {
        var rows = File.ReadAllLines(csv).Skip(1).Where(l => l.Length > 0).Select(l => l.Split(',')).ToList();
        if (rows.Count == 0) { Console.Error.WriteLine("no rows"); return 2; }
        Console.WriteLine($"| {"label",-14} | rules tokens | pass rate | mean input tokens | mean cost | cost per pass |");
        Console.WriteLine("|---|---|---|---|---|---|");
        foreach (var r in rows)
            Console.WriteLine($"| {r[0],-14} | {r[2]} | {double.Parse(r[5]) * 100:F0}% ({r[4]}/{r[3]}) | {r[6]} | ${r[7]} | {(r[8] == "" ? "n/a" : "$" + r[8])} |");
        if (rows.Count >= 2)
        {
            var a = rows[0]; var b = rows[^1];
            static string Delta(string x, string y) => double.TryParse(x, out var dx) && double.TryParse(y, out var dy) && dx != 0 ? $"{(dy - dx) / dx:+0%;-0%;0%}" : "n/a";
            Console.WriteLine($"\n{b[0]} vs {a[0]}: rules tokens {Delta(a[2], b[2])}, pass rate {double.Parse(a[5]) * 100:F0}% -> {double.Parse(b[5]) * 100:F0}%, mean input tokens {Delta(a[6], b[6])}, mean cost {Delta(a[7], b[7])}");
        }
        return 0;
    }
}
