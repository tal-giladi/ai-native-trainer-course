// AgentOps — Module 11 lab tool. Dependency-free (.NET 8+, System.Text.Json only). Read-only: never modifies files.
//   result     classify headless agent results (claude -p --output-format json)        lesson 11.1
//   wflint     lint agent workflows for the CI failure modes this module teaches        lessons 11.1, 11.3, 11.4
//   review     precision / recall of CI review comments against human-found defects     lesson 11.2
//   triage     validate a triage agent's structured output before anything is applied  lesson 11.3
//   ledger     cost, success and retry report over run telemetry (JSON lines)           lesson 11.4
//   evolution  check that AI-layer changelog entries follow the system-evolution policy lesson 11.5
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

if (args.Length == 0 || args[0] is "-h" or "--help" or "help")
{
    Console.WriteLine("""
    AgentOps <command> [options]
      result <result.json>... [--max-cost 1.50]
      wflint <workflow.yml>...
      review <prs.json> <comments.json> [--adjudicated f.json] [--split all|dev|holdout]
             [--min-severity nit|low|medium|high] [--drop cat,cat] [--max-per-pr N] [--by-category] [--misses]
      triage <output.json> --policy <policy.json> [--seen <applied.txt>]
      ledger <runs.jsonl> [--daily-ceiling 25] [--run-ceiling 2]
      evolution <AI-LAYER-CHANGELOG.md> --tasks <tasks.json> [--tasks <more.json>] [--since yyyy-mm-dd]
    Exit code 1 when a check fails.
    """);
    return 0;
}

try
{
    var rest = args.Skip(1).ToArray();
    return args[0] switch
    {
        "result" => Result.Run(rest),
        "wflint" => WfLint.Run(rest),
        "review" => Review.Run(rest),
        "triage" => Triage.Run(rest),
        "ledger" => Ledger.Run(rest),
        "evolution" => Evolution.Run(rest),
        _ => Fail($"unknown command '{args[0]}'")
    };
}
catch (Exception e) when (e is IOException or JsonException or ArgumentException or KeyNotFoundException or FormatException)
{
    return Fail(e.Message);
}

static int Fail(string msg) { Console.Error.WriteLine("error: " + msg); return 2; }

static class Opt
{
    public static string? Get(string[] a, string name) { var i = Array.IndexOf(a, name); return i >= 0 && i + 1 < a.Length ? a[i + 1] : null; }
    public static List<string> All(string[] a, string name) { var r = new List<string>(); for (int i = 0; i < a.Length - 1; i++) if (a[i] == name) r.Add(a[i + 1]); return r; }
    public static bool Has(string[] a, string name) => a.Contains(name);
    public static List<string> Positional(string[] a)
    {
        var r = new List<string>();
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i].StartsWith("--")) { if (i + 1 < a.Length && !a[i + 1].StartsWith("--") && !Flags.Contains(a[i])) i++; continue; }
            r.Add(a[i]);
        }
        return r;
    }
    static readonly HashSet<string> Flags = new() { "--by-category", "--misses" };
    public static string Pct(double x) => (x * 100).ToString("0") + "%";
    public static (double lo, double hi) Wilson(int k, int n, double z = 1.96)
    {
        if (n == 0) return (0, 1);
        double p = (double)k / n, z2 = z * z, den = 1 + z2 / n;
        double c = (p + z2 / (2 * n)) / den, h = z * Math.Sqrt(p * (1 - p) / n + z2 / (4.0 * n * n)) / den;
        return (Math.Max(0, c - h), Math.Min(1, c + h));
    }
    public static string Rate(int k, int n)
    {
        if (n == 0) return "n/a (0 of 0)";
        var (lo, hi) = Wilson(k, n);
        return $"{k}/{n} = {Pct((double)k / n)}  95% Wilson [{Pct(lo)}, {Pct(hi)}]";
    }
}

// ---------------------------------------------------------------- 11.1 result
static class Result
{
    public static int Run(string[] a)
    {
        var files = Opt.Positional(a);
        if (files.Count == 0) throw new ArgumentException("result: give at least one result JSON file");
        double? maxCost = Opt.Get(a, "--max-cost") is { } m ? double.Parse(m) : null;
        int bad = 0;
        foreach (var f in files)
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(f));
            var r = doc.RootElement;
            string subtype = Str(r, "subtype") ?? "(none)";
            bool isError = r.TryGetProperty("is_error", out var ie) && ie.ValueKind == JsonValueKind.True;
            int turns = r.TryGetProperty("num_turns", out var nt) && nt.ValueKind == JsonValueKind.Number ? nt.GetInt32() : -1;
            double cost = r.TryGetProperty("total_cost_usd", out var tc) && tc.ValueKind == JsonValueKind.Number ? tc.GetDouble() : 0;
            var denials = new List<string>();
            if (r.TryGetProperty("permission_denials", out var pd) && pd.ValueKind == JsonValueKind.Array)
                foreach (var d in pd.EnumerateArray()) denials.Add(Str(d, "tool_name") ?? "?");

            var problems = new List<string>();
            string klass = "ok";
            if (subtype is "error_max_turns" or "error_max_budget_usd") { klass = "limit"; problems.Add($"{subtype}: the run hit its cap; do not retry unchanged (raise the cap deliberately or narrow the task)"); }
            else if (subtype == "error_during_execution") { klass = "transient?"; problems.Add("error_during_execution: interrupted; one retry is reasonable, then alert"); }
            else if (subtype == "error_max_structured_output_retries") { klass = "output"; problems.Add("no valid structured output; treat as a failed run, never parse free text instead"); }
            else if (subtype != "success") { klass = "unknown"; problems.Add($"unexpected subtype '{subtype}'"); }
            if (isError && subtype == "success") problems.Add("is_error is true");
            if (denials.Count > 0)
            {
                if (klass == "ok") klass = "incomplete";
                var groups = denials.GroupBy(x => x).Select(g => $"{g.Key} x{g.Count()}");
                problems.Add($"{denials.Count} tool call(s) denied ({string.Join(", ", groups)}): a 'success' with denials did not do the task it was asked to do");
            }
            if (maxCost is { } mc && cost > mc) problems.Add($"cost ${cost:0.00} above the per-run ceiling ${mc:0.00}");

            var verdict = problems.Count == 0 ? "OK  " : "FAIL";
            Console.WriteLine($"{verdict} {Path.GetFileName(f)}: subtype={subtype} turns={turns} cost=${cost:0.00} class={klass}");
            foreach (var p in problems) Console.WriteLine($"     - {p}");
            if (problems.Count > 0) bad++;
        }
        Console.WriteLine(bad == 0 ? "ALL RUNS CLEAN" : $"{bad} of {files.Count} run(s) need attention");
        return bad == 0 ? 0 : 1;
    }
    static string? Str(JsonElement e, string name) => e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
}

// ---------------------------------------------------------------- 11.1 / 11.3 / 11.4 wflint
static class WfLint
{
    public static int Run(string[] a)
    {
        var files = Opt.Positional(a);
        if (files.Count == 0) throw new ArgumentException("wflint: give at least one workflow file");
        int errors = 0, warns = 0;
        foreach (var f in files)
        {
            var text = File.ReadAllText(f).Replace("\r\n", "\n");
            var code = string.Join("\n", text.Split('\n').Select(l => { var i = l.IndexOf(" #", StringComparison.Ordinal); return l.TrimStart().StartsWith('#') ? "" : i >= 0 ? l[..i] : l; }));
            var name = Path.GetFileName(f);
            var found = new List<(string lvl, string rule, string msg)>();
            void E(string r, string m) => found.Add(("ERROR", r, m));
            void W(string r, string m) => found.Add(("WARN ", r, m));

            bool agent = Regex.IsMatch(code, @"claude\s+(--bare\s+)?-p\b|claude\s+.*--print\b|anthropics/claude-code-action");
            bool prt = Regex.IsMatch(code, @"^\s*pull_request_target\s*:", RegexOptions.Multiline) || Regex.IsMatch(code, @"on:\s*\[?[^\n]*pull_request_target");
            bool headCheckout = Regex.IsMatch(code, @"ref:\s*\$\{\{\s*github\.event\.pull_request\.head\.(sha|ref)|refs/pull/");
            if (prt && headCheckout) E("pwn-request", "pull_request_target + checkout of the PR head: untrusted code runs with secrets and a write token");
            else if (prt && agent) W("pull-request-target", "pull_request_target gives the job secrets; an agent reading PR text is an injection target. Prefer pull_request, or split unprivileged analysis from a privileged comment step");
            if (Regex.IsMatch(code, @"dangerously-skip-permissions|bypassPermissions"))
                E("bypass-permissions", "permission checks disabled; use --permission-mode dontAsk with an explicit --allowedTools list");
            if (!Regex.IsMatch(code, @"^\s*permissions\s*:", RegexOptions.Multiline))
                E("token-permissions", "no permissions: block; the GITHUB_TOKEN gets the repository default, which may be write");
            if (Regex.IsMatch(code, @"permissions\s*:\s*write-all"))
                E("token-permissions", "permissions: write-all");
            if (!Regex.IsMatch(code, @"timeout-minutes\s*:"))
                E("timeout", "no timeout-minutes: a stuck run holds a runner (and a budget) for up to 6 hours");
            foreach (Match m in Regex.Matches(code, @"^\s*run:.*\$\{\{\s*github\.event\.(issue\.(title|body)|comment\.body|pull_request\.(title|body)|review\.body)[^}]*\}\}", RegexOptions.Multiline))
                E("script-injection", "untrusted event text interpolated into a shell command; pass it through an env: variable or a file");
            // multi-line run blocks: look for the expression anywhere after 'run: |'
            foreach (Match m in Regex.Matches(code, @"run:\s*\|[^\n]*\n((?:[ \t]+[^\n]*\n?)+)"))
                if (Regex.IsMatch(m.Groups[1].Value, @"\$\{\{\s*github\.event\.(issue\.(title|body)|comment\.body|pull_request\.(title|body)|review\.body)"))
                    E("script-injection", "untrusted event text interpolated inside a run: block; pass it through an env: variable or a file");

            if (agent)
            {
                if (Regex.IsMatch(code, @"install\.sh\s*\|\s*bash\s*($|\n)") || Regex.IsMatch(code, @"npm\s+(i|install)\s+(-g\s+)?@anthropic-ai/claude-code(\s|$)"))
                    E("unpinned-agent", "agent installed without a version: results drift with every release (pin it, set DISABLE_AUTOUPDATER=1)");
                if (!Regex.IsMatch(code, @"--max-turns"))
                    E("no-turn-cap", "agent runs without --max-turns");
                if (!Regex.IsMatch(code, @"--max-budget-usd"))
                    W("no-budget-cap", "agent runs without --max-budget-usd");
                if (Regex.IsMatch(code, @"claude\s+-p\b") && !Regex.IsMatch(code, @"--bare"))
                    W("not-bare", "claude -p without --bare loads the checkout's hooks, .mcp.json and CLAUDE.md; pass what the job needs explicitly");
                if (Regex.IsMatch(code, @"claude\s+(--bare\s+)?-p\b") && !Regex.IsMatch(code, @"--output-format\s+(json|stream-json)"))
                    W("no-json", "no --output-format json: the job cannot see subtype, cost or permission denials");
                if (Regex.IsMatch(code, @"--allowedTools\s+""[^""]*(Bash\(gh |Bash\(git push|mcp__github)"))
                    W("agent-write-tool", "the agent itself holds a state-changing tool (gh, git push, GitHub MCP); if it reads untrusted text, prefer propose -> validate -> apply");
                if (!Regex.IsMatch(code, @"concurrency\s*:"))
                    W("no-concurrency", "no concurrency group: superseded runs keep spending");
                if (!Regex.IsMatch(code, @"vars\.AGENTS_ENABLED"))
                    W("no-kill-switch", "no kill switch (vars.AGENTS_ENABLED): stopping the agent needs a code change");
            }
            // workflow-level env exposing secrets to every step
            var top = code.Split("\njobs:")[0];
            if (Regex.IsMatch(top, @"^env:\s*\n(?:[ \t]+[^\n]*\n)*?[ \t]+[A-Z_]+:\s*\$\{\{\s*secrets\.", RegexOptions.Multiline))
                W("secret-scope", "secret in workflow-level env: every step of every job sees it; scope it to the one step that needs it");

            foreach (var (lvl, rule, msg) in found.Distinct()) Console.WriteLine($"{lvl} {name}: {rule}: {msg}");
            int e = found.Distinct().Count(x => x.lvl == "ERROR"), w = found.Distinct().Count(x => x.lvl != "ERROR");
            if (found.Count == 0) Console.WriteLine($"ok    {name}");
            errors += e; warns += w;
        }
        Console.WriteLine($"{errors} error(s), {warns} warning(s)");
        return errors == 0 ? 0 : 1;
    }
}

// ---------------------------------------------------------------- 11.2 review
static class Review
{
    record Defect(string Id, string Pr, string File, int Line, string Category, string Summary, string FoundBy);
    record Comment(string Id, string Pr, string File, int Line, string Severity, string Category, string Body);
    static readonly string[] Sev = { "nit", "low", "medium", "high" };

    public static int Run(string[] a)
    {
        var pos = Opt.Positional(a);
        if (pos.Count < 2) throw new ArgumentException("review: need <prs.json> <comments.json>");
        using var prsDoc = JsonDocument.Parse(File.ReadAllText(pos[0]));
        using var cDoc = JsonDocument.Parse(File.ReadAllText(pos[1]));
        var split = Opt.Get(a, "--split") ?? "all";
        var prSplit = prsDoc.RootElement.GetProperty("prs").EnumerateArray().ToDictionary(p => p.GetProperty("id").GetString()!, p => p.GetProperty("split").GetString()!);
        bool InSplit(string pr) => split == "all" || prSplit.GetValueOrDefault(pr) == split;
        var prs = prSplit.Keys.Where(InSplit).ToList();

        var defects = ReadDefects(prsDoc.RootElement);
        if (Opt.Get(a, "--adjudicated") is { } adj)
        {
            using var ad = JsonDocument.Parse(File.ReadAllText(adj));
            defects.AddRange(ReadDefects(ad.RootElement));
        }
        defects = defects.Where(d => InSplit(d.Pr)).ToList();

        var comments = cDoc.RootElement.GetProperty("comments").EnumerateArray().Select(c => new Comment(
            c.GetProperty("id").GetString()!, c.GetProperty("pr").GetString()!, c.GetProperty("file").GetString()!, c.GetProperty("line").GetInt32(),
            c.GetProperty("severity").GetString()!, c.GetProperty("category").GetString()!, c.GetProperty("body").GetString()!)).Where(c => InSplit(c.Pr)).ToList();
        int raw = comments.Count;

        var filters = new List<string>();
        if (Opt.Get(a, "--min-severity") is { } ms)
        {
            int min = Array.IndexOf(Sev, ms); if (min < 0) throw new ArgumentException($"--min-severity must be one of {string.Join("|", Sev)}");
            comments = comments.Where(c => Array.IndexOf(Sev, c.Severity) >= min).ToList(); filters.Add($"severity >= {ms}");
        }
        if (Opt.Get(a, "--drop") is { } drop)
        {
            var set = drop.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToHashSet();
            comments = comments.Where(c => !set.Contains(c.Category)).ToList(); filters.Add($"drop {drop}");
        }
        if (Opt.Get(a, "--max-per-pr") is { } mp)
        {
            int n = int.Parse(mp);
            comments = comments.GroupBy(c => c.Pr).SelectMany(g => g.OrderByDescending(c => Array.IndexOf(Sev, c.Severity)).ThenBy(c => c.Id).Take(n)).ToList();
            filters.Add($"max {n} per PR");
        }
        comments = comments.OrderBy(c => c.Id).ToList();

        // Match: same PR and file, |line diff| <= 3, each defect once; later matches are duplicates.
        var matched = new Dictionary<string, string>(); // defect -> comment
        var verdict = new Dictionary<string, string>(); // comment -> TP:Dxx | DUP:Dxx | FP
        foreach (var c in comments)
        {
            var d = defects.Where(d => d.Pr == c.Pr && d.File == c.File && Math.Abs(d.Line - c.Line) <= 3).OrderBy(d => Math.Abs(d.Line - c.Line)).FirstOrDefault();
            if (d is null) verdict[c.Id] = "FP";
            else if (matched.ContainsKey(d.Id)) verdict[c.Id] = "DUP:" + d.Id;
            else { matched[d.Id] = c.Id; verdict[c.Id] = "TP:" + d.Id; }
        }
        int tp = matched.Count, c_ = comments.Count, fp = c_ - tp, dups = verdict.Values.Count(v => v.StartsWith("DUP")), dn = defects.Count;
        double p = c_ == 0 ? 0 : (double)tp / c_;

        Console.WriteLine($"comments: {Path.GetFileName(pos[1])}   split: {split} ({prs.Count} PRs)   filters: {(filters.Count == 0 ? "none" : string.Join(", ", filters))}");
        Console.WriteLine($"posted        {c_} comments ({(double)c_ / Math.Max(1, prs.Count):0.00} per PR){(c_ != raw ? $", {raw - c_} suppressed" : "")}");
        Console.WriteLine($"true pos.     {tp}   (known defects in split: {dn})");
        Console.WriteLine($"false pos.    {fp}   (of which duplicates: {dups})");
        Console.WriteLine($"precision     {Opt.Rate(tp, c_)}");
        Console.WriteLine($"recall        {Opt.Rate(tp, dn)}");
        Console.WriteLine($"noise per PR  {(double)fp / Math.Max(1, prs.Count):0.00} false comments   false alarms per real finding {(tp == 0 ? "inf" : ((double)fp / tp).ToString("0.0"))}");
        if (c_ > 0) Console.WriteLine($"ceiling       at this volume precision cannot exceed {Opt.Pct(Math.Min(1, (double)dn / c_))} (known defects / comments)");

        if (Opt.Has(a, "--by-category"))
        {
            Console.WriteLine("\ncategory        posted  true  precision");
            foreach (var g in comments.GroupBy(c => c.Category).OrderByDescending(g => g.Count()))
            {
                int t = g.Count(c => verdict[c.Id].StartsWith("TP"));
                Console.WriteLine($"{g.Key,-15} {g.Count(),6} {t,5}  {Opt.Pct((double)t / g.Count()),9}");
            }
        }
        if (Opt.Has(a, "--misses"))
        {
            Console.WriteLine("\nmissed defects (false negatives):");
            foreach (var d in defects.Where(d => !matched.ContainsKey(d.Id)))
                Console.WriteLine($"  {d.Id} {d.Pr} {d.Category,-12} {d.Summary}");
        }
        return 0;
    }

    static List<Defect> ReadDefects(JsonElement root) => root.GetProperty("defects").EnumerateArray().Select(d => new Defect(
        d.GetProperty("id").GetString()!, d.GetProperty("pr").GetString()!, d.GetProperty("file").GetString()!, d.GetProperty("line").GetInt32(),
        d.GetProperty("category").GetString()!, d.GetProperty("summary").GetString()!, d.GetProperty("found_by").GetString()!)).ToList();
}

// ---------------------------------------------------------------- 11.3 triage
static class Triage
{
    public static int Run(string[] a)
    {
        var pos = Opt.Positional(a);
        var policyPath = Opt.Get(a, "--policy") ?? throw new ArgumentException("triage: --policy is required");
        if (pos.Count < 1) throw new ArgumentException("triage: need <output.json>");
        using var pdoc = JsonDocument.Parse(File.ReadAllText(policyPath));
        var pol = pdoc.RootElement;
        var labels = pol.GetProperty("labels").EnumerateArray().Select(x => x.GetString()!).ToHashSet();
        var reserved = pol.GetProperty("reserved_labels").EnumerateArray().Select(x => x.GetString()!).ToHashSet();
        var priorities = pol.GetProperty("priorities").EnumerateArray().Select(x => x.GetString()!).ToHashSet();
        var humanPriorities = pol.GetProperty("priorities_need_human").EnumerateArray().Select(x => x.GetString()!).ToHashSet();
        var fields = pol.GetProperty("fields").EnumerateArray().Select(x => x.GetString()!).ToHashSet();
        int maxLabels = pol.GetProperty("max_labels").GetInt32();
        int maxSummary = pol.GetProperty("max_summary_chars").GetInt32();

        using var odoc = JsonDocument.Parse(File.ReadAllText(pos[0]));
        var root = odoc.RootElement;
        // Accept either the raw object or a claude -p result with structured_output.
        if (root.TryGetProperty("type", out var t) && t.GetString() == "result")
        {
            if (!root.TryGetProperty("structured_output", out var so) || so.ValueKind != JsonValueKind.Object)
            { Console.WriteLine("REJECT: result has no structured_output (never fall back to parsing free text)"); return 1; }
            root = so;
        }
        var errs = new List<string>();
        foreach (var p in root.EnumerateObject()) if (!fields.Contains(p.Name)) errs.Add($"field '{p.Name}' is not in the output contract (the agent may only propose labels, priority, duplicate and a summary)");
        string issue = root.TryGetProperty("issue", out var iv) ? iv.GetString() ?? "" : "";
        if (!Regex.IsMatch(issue, @"^[A-Z]+-\d+$")) errs.Add($"issue id '{issue}' is malformed");
        var lab = root.TryGetProperty("labels", out var lv) && lv.ValueKind == JsonValueKind.Array ? lv.EnumerateArray().Select(x => x.GetString() ?? "").ToList() : new();
        foreach (var l in lab)
        {
            if (reserved.Contains(l)) errs.Add($"label '{l}' is reserved for humans");
            else if (!labels.Contains(l)) errs.Add($"label '{l}' is not in the taxonomy");
        }
        if (lab.Count > maxLabels) errs.Add($"{lab.Count} labels, policy allows {maxLabels}");
        string pr = root.TryGetProperty("priority", out var prv) ? prv.GetString() ?? "" : "";
        bool needsHuman = root.TryGetProperty("needs_human", out var nh) && nh.ValueKind == JsonValueKind.True;
        if (!priorities.Contains(pr)) errs.Add($"priority '{pr}' is not allowed");
        else if (humanPriorities.Contains(pr) && !needsHuman) errs.Add($"priority '{pr}' must be routed to a human (needs_human=true)");
        string summary = root.TryGetProperty("summary", out var sv) ? sv.GetString() ?? "" : "";
        if (summary.Length > maxSummary) errs.Add($"summary is {summary.Length} chars, policy allows {maxSummary}");
        if (Regex.IsMatch(summary, @"https?://|www\.|@[A-Za-z0-9-]+/|`|\$\(")) errs.Add("summary contains a link, mention or command: it would turn the triage comment into an outbound channel");
        if (root.TryGetProperty("duplicate_of", out var dv) && dv.ValueKind == JsonValueKind.String && !Regex.IsMatch(dv.GetString()!, @"^[A-Z]+-\d+$")) errs.Add("duplicate_of is malformed");

        var key = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(root.GetRawText()))).ToLowerInvariant()[..12];
        var idem = $"{issue}:{key}";
        if (errs.Count > 0)
        {
            Console.WriteLine($"REJECT {issue}: {errs.Count} problem(s); nothing is applied");
            foreach (var e in errs) Console.WriteLine($"  - {e}");
            return 1;
        }
        if (Opt.Get(a, "--seen") is { } seen && File.Exists(seen) && File.ReadAllLines(seen).Any(l => l.Trim().StartsWith(issue + ":")))
        {
            Console.WriteLine($"SKIP   {issue}: already triaged (idempotency key present in {Path.GetFileName(seen)})");
            return 0;
        }
        Console.WriteLine($"ACCEPT {issue}  key {idem}");
        Console.WriteLine($"  plan: gh issue edit {issue} {string.Join(" ", lab.Select(l => "--add-label " + l))} --add-label priority:{pr}{(needsHuman ? " --add-label needs-human" : "")}");
        Console.WriteLine($"  then: append '{idem}' to the applied log");
        return 0;
    }
}

// ---------------------------------------------------------------- 11.4 ledger
static class Ledger
{
    record Rec(DateTime Ts, string Job, string RunId, string Subtype, int Turns, double Cost, long Ms, string Outcome, int Attempt);

    public static int Run(string[] a)
    {
        var pos = Opt.Positional(a);
        if (pos.Count < 1) throw new ArgumentException("ledger: need <runs.jsonl>");
        double daily = double.Parse(Opt.Get(a, "--daily-ceiling") ?? "25");
        double perRun = double.Parse(Opt.Get(a, "--run-ceiling") ?? "2");
        var runs = new List<Rec>();
        foreach (var line in File.ReadLines(pos[0]).Where(l => l.Trim().Length > 0))
        {
            using var d = JsonDocument.Parse(line);
            var r = d.RootElement;
            runs.Add(new Rec(DateTime.Parse(r.GetProperty("ts").GetString()!, null, DateTimeStyles.AdjustToUniversal), r.GetProperty("job").GetString()!, r.GetProperty("run_id").GetString()!,
                r.GetProperty("subtype").GetString()!, r.GetProperty("num_turns").GetInt32(), r.GetProperty("cost_usd").GetDouble(),
                r.GetProperty("duration_ms").GetInt64(), r.GetProperty("outcome").GetString()!, r.GetProperty("attempt").GetInt32()));
        }
        var days = runs.Select(r => r.Ts.Date).Distinct().Count();
        Console.WriteLine($"{runs.Count} runs over {days} days, total ${runs.Sum(r => r.Cost):0.00}\n");
        Console.WriteLine("job               runs  ok-runs  useful  cost      $/run  $/useful  p95 min  limit-fails  retried-limit");
        int problems = 0;
        var notes = new List<string>();
        foreach (var g in runs.GroupBy(r => r.Job).OrderBy(g => g.Key))
        {
            int n = g.Count(), ok = g.Count(r => r.Subtype == "success"), useful = g.Count(r => r.Outcome is "merged" or "applied");
            double cost = g.Sum(r => r.Cost);
            var durs = g.Select(r => r.Ms).OrderBy(x => x).ToList();
            double p95 = durs[Math.Min(durs.Count - 1, (int)Math.Ceiling(0.95 * durs.Count) - 1)] / 60000.0;
            int limit = g.Count(r => r.Subtype is "error_max_turns" or "error_max_budget_usd");
            // a retry of a limit failure: same run_id, attempt > 1, previous attempt hit a limit
            int retriedLimit = g.GroupBy(r => r.RunId).Sum(rg => rg.OrderBy(r => r.Attempt).Zip(rg.OrderBy(r => r.Attempt).Skip(1)).Count(p => p.First.Subtype is "error_max_turns" or "error_max_budget_usd"));
            Console.WriteLine($"{g.Key,-16} {n,5} {ok,8} {useful,7}  ${cost,7:0.00}  {cost / n,5:0.00}  {(useful == 0 ? "   n/a" : (cost / useful).ToString("0.00").PadLeft(6))}  {p95,7:0.0}  {limit,11}  {retriedLimit,13}");
            if (retriedLimit > 0) { problems++; notes.Add($"RETRY   {g.Key}: {retriedLimit} retries of runs that had hit a turn or budget cap; a cap is not a transient error"); }
        }
        Console.WriteLine();
        foreach (var n in notes) Console.WriteLine(n);
        foreach (var d in runs.GroupBy(r => r.Ts.Date).OrderBy(g => g.Key))
        {
            var total = d.Sum(r => r.Cost);
            if (total > daily)
            {
                problems++;
                var top = d.GroupBy(r => r.Job).OrderByDescending(j => j.Sum(r => r.Cost)).First();
                Console.WriteLine($"CEILING {d.Key:yyyy-MM-dd}: ${total:0.00} > daily ${daily:0.00}; {top.Key} spent ${top.Sum(r => r.Cost):0.00} in {top.Count()} runs");
            }
        }
        foreach (var r in runs.Where(r => r.Cost > perRun).OrderBy(r => r.Ts))
        { problems++; Console.WriteLine($"RUN     {r.Ts:yyyy-MM-dd HH:mm} {r.Job} {r.RunId} attempt {r.Attempt}: ${r.Cost:0.00} > per-run ${perRun:0.00} ({r.Subtype}, {r.Turns} turns)"); }
        Console.WriteLine(problems == 0 ? "within ceilings" : $"{problems} ceiling/retry finding(s)");
        return problems == 0 ? 0 : 1;
    }
}

// ---------------------------------------------------------------- 11.5 evolution
static class Evolution
{
    public static int Run(string[] a)
    {
        var pos = Opt.Positional(a);
        if (pos.Count < 1) throw new ArgumentException("evolution: need <AI-LAYER-CHANGELOG.md>");
        var taskIds = new HashSet<string>();
        foreach (var tf in Opt.All(a, "--tasks"))
        {
            using var d = JsonDocument.Parse(File.ReadAllText(tf));
            foreach (var t in d.RootElement.GetProperty("tasks").EnumerateArray()) taskIds.Add(t.GetProperty("id").GetString()!);
        }
        if (taskIds.Count == 0) throw new ArgumentException("evolution: give at least one --tasks file");
        DateTime? since = Opt.Get(a, "--since") is { } s ? DateTime.Parse(s) : null;

        var text = File.ReadAllText(pos[0]).Replace("\r\n", "\n");
        var entries = Regex.Split(text, @"\n(?=## )").Where(e => e.StartsWith("## ")).ToList();
        int errors = 0, checkedN = 0;
        foreach (var e in entries)
        {
            var head = e.Split('\n')[0][3..].Trim();
            var dm = Regex.Match(head, @"\d{4}-\d{2}-\d{2}");
            if (since is { } sd && dm.Success && DateTime.Parse(dm.Value) < sd) continue;
            checkedN++;
            string? F(string name) { var m = Regex.Match(e, @"\*\*" + Regex.Escape(name) + @":\*\*\s*(.+)"); return m.Success ? m.Groups[1].Value.Trim() : null; }
            var probs = new List<string>();
            foreach (var req in new[] { "Class", "Changed", "Why", "Verified", "Reviewed by" }) if (F(req) is null) probs.Add($"missing **{req}:**");
            var klass = F("Class") ?? "";
            var incident = F("Incident");
            var why = F("Why") ?? "";
            bool isIncident = incident is not null || Regex.IsMatch(why, @"\bincident\b|\bagent (did|wrote|proposed|edited)", RegexOptions.IgnoreCase);
            var reg = F("Regression task");
            if (isIncident)
            {
                if (reg is null || Regex.IsMatch(reg, @"^(none|n/?a|-|tbd)", RegexOptions.IgnoreCase)) probs.Add("incident without a regression task (mistake -> task -> fix -> gate)");
                else
                {
                    var ids = Regex.Matches(reg, @"\bT\d{2,3}\b").Select(m => m.Value).ToList();
                    if (ids.Count == 0) probs.Add("regression task field names no task id");
                    foreach (var id in ids) if (!taskIds.Contains(id)) probs.Add($"regression task {id} is not in the task set");
                    if (!Regex.IsMatch(reg + " " + (F("Verified") ?? ""), @"fail", RegexOptions.IgnoreCase)) probs.Add("no evidence the regression task failed before the fix");
                }
            }
            var verified = F("Verified") ?? "";
            if (Regex.IsMatch(verified, @"^not verified|^n/?a|^-$", RegexOptions.IgnoreCase)) probs.Add("not verified");
            else if (verified.Length > 0 && !Regex.IsMatch(verified, @"\d+/\d+|GATE PASSED|gate pass|\d+\s*x\s*\d+|\bpts\b", RegexOptions.IgnoreCase)) probs.Add("Verified: cites no eval numbers or gate result");
            if (Regex.IsMatch(F("Reviewed by") ?? "", @"bypass|self|none|pushed by", RegexOptions.IgnoreCase)) probs.Add("no owner review (bypass or self-merge): record it as an override and review it afterwards");
            if (Regex.IsMatch(klass, @"^C\b") && !Regex.IsMatch(F("Reviewed by") ?? "", "security", RegexOptions.IgnoreCase)) probs.Add("class C (permissions, MCP, workflows, secrets) needs a security reviewer");
            if (Regex.IsMatch(klass, @"^D\b") && !Regex.IsMatch(verified, @"baseline", RegexOptions.IgnoreCase)) probs.Add("class D (agent or model version) must refresh the baseline");

            if (probs.Count == 0) Console.WriteLine($"ok    {head}");
            else { errors += probs.Count; Console.WriteLine($"FAIL  {head}"); foreach (var p in probs) Console.WriteLine($"      - {p}"); }
        }
        Console.WriteLine($"{checkedN} entr{(checkedN == 1 ? "y" : "ies")} checked, {errors} problem(s)");
        return errors == 0 ? 0 : 1;
    }
}
