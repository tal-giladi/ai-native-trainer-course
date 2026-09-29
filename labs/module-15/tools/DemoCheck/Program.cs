// DemoCheck — checks for the Module 15 demo repository and demo craft. Read-only, deterministic, no packages.
//
//   credibility <repo> [--config demo/demo.json] [--deny <file outside the repo>]
//        15.1  history, two eras, stale docs, tests, tickets, PRD (credibility: warnings)
//              secrets, e-mail addresses, internal hosts/IPs, deny terms in ALL history (confidentiality: errors)
//   before <repo> [--config demo/demo.json]
//        15.2  empty AI layer at the "before" tag, layer-only diff to the "after" tag, demo ticket still open
//   timeline <file.tsv> [--mode recording|stranger] [--min 15:00] [--max 20:00] [--limit 30:00]
//            [--max-gap 45] [--file-duration mm:ss]
//        15.3  an annotated recording or stranger-test log: cuts, silent gaps, outcome, help given, stuck points
//   runsheet <runsheet.md> [--repo <demo repo>]
//        15.4  every live segment has a fallback and a line to say; budgets fit the slot; P(clean run)
//   drill <runsheet.md> [--seed N]
//        15.4  picks a live segment and a failure to induce during a rehearsal
//
// Exit code: 0 clean, 1 errors found, 2 usage problem.

using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
Console.OutputEncoding = Encoding.UTF8;

try
{
    if (args.Length == 0) throw new UsageException("missing command");
    var opts = Options.Parse(args.Skip(1).ToArray());
    return args[0] switch
    {
        "credibility" => Credibility.Run(opts),
        "before" => Before.Run(opts),
        "timeline" => Timeline.Run(opts),
        "runsheet" => RunSheet.Run(opts),
        "drill" => Drill.Run(opts),
        _ => throw new UsageException($"unknown command '{args[0]}'"),
    };
}
catch (UsageException e)
{
    Console.Error.WriteLine($"DemoCheck: {e.Message}");
    Console.Error.WriteLine("usage: DemoCheck credibility|before <repo> [--config f] [--deny f]");
    Console.Error.WriteLine("       DemoCheck timeline <file.tsv> [--mode recording|stranger] [--file-duration mm:ss]");
    Console.Error.WriteLine("       DemoCheck runsheet|drill <runsheet.md> [--repo dir] [--seed N]");
    return 2;
}

sealed class UsageException(string m) : Exception(m);

sealed class Options
{
    public List<string> Positional { get; } = [];
    public Dictionary<string, string> Named { get; } = new(StringComparer.OrdinalIgnoreCase);

    public static Options Parse(string[] a)
    {
        var o = new Options();
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i].StartsWith("--"))
            {
                if (i + 1 >= a.Length) throw new UsageException($"{a[i]} needs a value");
                o.Named[a[i][2..]] = a[++i];
            }
            else o.Positional.Add(a[i]);
        }
        return o;
    }

    public string Arg(int i, string what) =>
        i < Positional.Count ? Positional[i] : throw new UsageException($"missing {what}");

    public string? Get(string k) => Named.TryGetValue(k, out var v) ? v : null;
}

/// <summary>Prints aligned PASS/WARN/FAIL rows and counts them.</summary>
sealed class Report
{
    public int Errors, Warnings;

    public void Row(string label, string status, string detail)
    {
        if (status == "FAIL") Errors++;
        if (status == "WARN") Warnings++;
        Console.WriteLine($"{label,-13}{status,-6}{detail}");
    }

    public void Detail(string line) => Console.WriteLine($"{"",-19}{line}");

    public int Finish(string okWord, string badWord)
    {
        Console.WriteLine();
        Console.WriteLine($"Result: {(Errors == 0 ? okWord : badWord)} ({Errors} errors, {Warnings} warnings)");
        return Errors == 0 ? 0 : 1;
    }
}

static class Git
{
    public static string Run(string repo, params string[] args)
    {
        var psi = new ProcessStartInfo("git")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            StandardOutputEncoding = Encoding.UTF8,
        };
        psi.ArgumentList.Add("-C");
        psi.ArgumentList.Add(repo);
        psi.ArgumentList.Add("-c");
        psi.ArgumentList.Add("core.quotepath=off");
        foreach (var a in args) psi.ArgumentList.Add(a);
        using var p = Process.Start(psi) ?? throw new UsageException("cannot start git");
        var stdout = p.StandardOutput.ReadToEndAsync();
        var stderr = p.StandardError.ReadToEndAsync();
        p.WaitForExit();
        if (p.ExitCode != 0) throw new GitException(stderr.Result.Trim());
        return stdout.Result;
    }

    public static bool TryRun(string repo, out string output, params string[] args)
    {
        try { output = Run(repo, args); return true; }
        catch (GitException) { output = ""; return false; }
    }

    public static List<string> Lines(string s) =>
        s.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(l => l.TrimEnd('\r')).ToList();

    public static void RequireRepo(string repo)
    {
        if (!Directory.Exists(repo)) throw new UsageException($"no such directory: {repo}");
        if (!TryRun(repo, out _, "rev-parse", "--git-dir")) throw new UsageException($"{repo} is not a git repository");
    }
}

sealed class GitException(string m) : Exception(m);

/// <summary>demo/demo.json in the demo repository.</summary>
sealed class DemoConfig
{
    public string DemoTicket { get; set; } = "";
    public string BeforeTag { get; set; } = "before-ai-layer";
    public string AfterTag { get; set; } = "ai-layer-v1";
    public string TicketsDir { get; set; } = "tickets";
    public List<string> LayerPaths { get; set; } = [];
    public List<string> Placeholders { get; set; } = [".claude/.gitkeep"];
    public OpenCheck? OpenCheck { get; set; }
    public List<string> AllowedEmailDomains { get; set; } = ["example.com", "example.org", "example.net", "users.noreply.github.com"];

    public static DemoConfig Load(string repo, string? path, bool required)
    {
        var p = path ?? Path.Combine(repo, "demo", "demo.json");
        if (!File.Exists(p))
        {
            if (required) throw new UsageException($"config not found: {p}");
            return new DemoConfig();
        }
        return JsonSerializer.Deserialize<DemoConfig>(File.ReadAllText(p),
                   new JsonSerializerOptions { PropertyNameCaseInsensitive = true, ReadCommentHandling = JsonCommentHandling.Skip })
               ?? throw new UsageException($"cannot read {p}");
    }

    public bool IsLayer(string path) =>
        LayerPaths.Any(l => l.EndsWith('/') ? path.StartsWith(l, StringComparison.Ordinal) : path == l);
}

sealed class OpenCheck
{
    public string Path { get; set; } = "";
    public string Contains { get; set; } = "";
}

static class Credibility
{
    sealed record Commit(string Hash, string Author, string Email, DateTime Date, string Subject, string CommitterEmail);

    public static int Run(Options o)
    {
        var repo = o.Arg(0, "repository path");
        Git.RequireRepo(repo);
        var cfg = DemoConfig.Load(repo, o.Get("config"), required: false);
        var r = new Report();
        Console.WriteLine($"DemoCheck credibility  {repo}");
        Console.WriteLine();

        var commits = Git.Lines(Git.Run(repo, "log", "--all", "--date=short", "--format=%H%x1f%an%x1f%ae%x1f%ad%x1f%s%x1f%ce"))
            .Select(l => l.Split('\x1f'))
            .Select(p => new Commit(p[0], p[1], p[2], DateTime.ParseExact(p[3], "yyyy-MM-dd", CultureInfo.InvariantCulture), p[4], p[5]))
            .ToList();
        var files = Git.Lines(Git.Run(repo, "ls-files"));
        string Read(string f) { var p = Path.Combine(repo, f); return File.Exists(p) ? File.ReadAllText(p) : ""; }

        // --- Credibility (warnings) ---
        Console.WriteLine("Credibility");
        var authors = commits.Select(c => c.Author).Distinct().Count();
        var span = commits.Count == 0 ? 0 : (commits.Max(c => c.Date) - commits.Min(c => c.Date)).TotalDays / 365.25;
        var histOk = commits.Count >= 12 && span >= 2 && authors >= 3;
        r.Row("History", histOk ? "PASS" : "WARN",
            commits.Count == 0 ? "no commits" :
            $"{commits.Count} commits, {commits.Min(c => c.Date):yyyy-MM-dd} to {commits.Max(c => c.Date):yyyy-MM-dd} ({span:0.0} years), {authors} authors" +
            (histOk ? "" : " -- looks like a toy (want >= 12 commits, >= 2 years, >= 3 authors)"));

        var legacy = files.Where(f => f.EndsWith(".cs") && (f.Contains("/Legacy/") || Read(f).Contains("[Obsolete"))).ToList();
        var adrs = files.Where(f => f.Contains("/adr/", StringComparison.OrdinalIgnoreCase) && f.EndsWith(".md")).ToList();
        r.Row("Two eras", legacy.Count > 0 && adrs.Count > 0 ? "PASS" : "WARN",
            legacy.Count > 0 && adrs.Count > 0
                ? $"legacy code: {legacy[0]}{(legacy.Count > 1 ? $" (+{legacy.Count - 1})" : "")}; decision record: {adrs[0]}"
                : "no legacy code next to a recorded decision -- nothing for an agent to get wrong");

        var names = new HashSet<string>(files.Select(Path.GetFileName)!, StringComparer.OrdinalIgnoreCase);
        var staleRefs = new List<string>();
        foreach (var d in files.Where(f => f.StartsWith("docs/") && f.EndsWith(".md") && !f.StartsWith("docs/ai/")))
            foreach (Match m in Regex.Matches(Read(d), @"\b[\w.-]+\.(sln|csproj)\b"))
                if (!names.Contains(m.Value)) staleRefs.Add($"{d} names {m.Value}");
        r.Row("Stale docs", staleRefs.Count > 0 ? "PASS" : "INFO",
            staleRefs.Count > 0 ? $"{staleRefs.Distinct().First()} (not in the repo) -- real mess"
                                : "no doc contradicts the code; real repositories usually have one");

        var tests = files.Where(f => f.StartsWith("tests/") && f.EndsWith(".cs")).ToList();
        var conv = tests.Where(f => Regex.IsMatch(Path.GetFileName(f), "Convention|Architecture", RegexOptions.IgnoreCase)).ToList();
        r.Row("Tests", tests.Count > 0 && conv.Count > 0 ? "PASS" : "WARN",
            $"{tests.Count} test files, {conv.Count} convention/architecture test file(s)" +
            (conv.Count == 0 ? " -- the house rules are nowhere executable" : ""));

        var tdir = cfg.TicketsDir.TrimEnd('/') + "/";
        var tickets = files.Where(f => f.StartsWith(tdir) && f.EndsWith(".md")).ToList();
        var withAc = tickets.Count(t => Regex.IsMatch(Read(t), @"(?im)^#+\s*acceptance criteria"));
        var ticketDetail = $"{tickets.Count} tickets, {withAc} with acceptance criteria";
        var ticketStatus = tickets.Count >= 5 && withAc == tickets.Count ? "PASS" : "WARN";
        if (cfg.DemoTicket != "")
        {
            // Only the current branch: a pre-baked solution branch is allowed to mention the ticket.
            var mentioned = Git.Lines(Git.Run(repo, "log", "--format=%h %s", "HEAD"))
                .Where(l => Regex.IsMatch(l, $@"\b{Regex.Escape(cfg.DemoTicket)}\b")).ToList();
            ticketDetail += mentioned.Count == 0 ? $"; demo ticket {cfg.DemoTicket} open on this branch" : $"; demo ticket {cfg.DemoTicket} already in history ({mentioned[0]})";
            if (mentioned.Count > 0) ticketStatus = "WARN";
        }
        r.Row("Tickets", ticketStatus, ticketDetail);

        var prds = files.Where(f => (f.StartsWith("prd/") || Path.GetFileName(f).Contains("PRD")) && f.EndsWith(".md")).ToList();
        r.Row("PRD", prds.Count > 0 ? "PASS" : "WARN", prds.Count > 0 ? $"{prds.Count}: {string.Join(", ", prds)}" : "no PRD -- tickets float without a why");

        // --- Confidentiality (errors), over the whole history ---
        Console.WriteLine();
        Console.WriteLine("Confidentiality (every commit, not only HEAD)");
        var added = AddedLines(repo);
        var paths = Git.Lines(Git.Run(repo, "log", "--all", "--name-only", "--format=")).Distinct().ToList();

        var secrets = new List<string>();
        var secretPatterns = new (string Name, Regex Rx)[]
        {
            ("private key", new(@"-----BEGIN [A-Z ]*PRIVATE KEY-----")),
            ("AWS key id", new(@"\bAKIA[0-9A-Z]{16}\b")),
            ("GitHub token", new(@"\b(gh[pousr]_[A-Za-z0-9]{30,}|github_pat_[A-Za-z0-9_]{20,})")),
            ("Anthropic key", new(@"\bsk-ant-[A-Za-z0-9_-]{10,}")),
            ("password", new(@"(?i)\b(password|pwd)\s*[=:]\s*(?!\$\{|<|Lab_only)[^;""'\s,}]{4,}")),
            ("API key", new(@"(?i)\b(api[_-]?key|secret|token)\s*[=:]\s*[""'][A-Za-z0-9_\-]{16,}[""']")),
        };
        foreach (var a in added)
            foreach (var (name, rx) in secretPatterns)
                if (rx.Match(a.Text) is { Success: true } m)
                    secrets.Add($"{a.Commit} {a.Path}:{a.Line}  {name}: {Mask(m.Value)}");
        foreach (var f in paths.Where(p => Regex.IsMatch(Path.GetFileName(p), @"^\.env(\..+)?$|\.pem$|\.pfx$")))
            secrets.Add($"{f}  secret-shaped file was committed");
        Emit(r, "Secrets", secrets, $"0 findings in {commits.Count} commits");

        var emails = new List<string>();
        var emailRx = new Regex(@"\b[A-Za-z0-9._%+-]+@([A-Za-z0-9-]+\.)+[A-Za-z]{2,}\b");
        bool Allowed(string email)
        {
            var domain = email[(email.IndexOf('@') + 1)..].ToLowerInvariant();
            // Reserved names (RFC 2606) are safe to publish, but only the ones you chose are allowed:
            // anything else is a real person's address until proven otherwise.
            return cfg.AllowedEmailDomains.Any(d => domain == d.ToLowerInvariant() || domain.EndsWith("." + d.ToLowerInvariant()));
        }
        foreach (var c in commits)
        {
            if (!Allowed(c.Email)) emails.Add($"{c.Hash[..7]} author {c.Author} <{c.Email}>");
            if (!Allowed(c.CommitterEmail)) emails.Add($"{c.Hash[..7]} committer <{c.CommitterEmail}>");
        }
        foreach (var a in added)
            foreach (Match m in emailRx.Matches(a.Text))
                if (!Allowed(m.Value)) emails.Add($"{a.Commit} {a.Path}:{a.Line}  {m.Value}");
        Emit(r, "E-mail", emails, $"only allowed domains ({string.Join(", ", cfg.AllowedEmailDomains)})");

        var hosts = new List<string>();
        var hostRx = new Regex(@"(?i)\b[a-z0-9-]+(\.[a-z0-9-]+)*\.(corp|internal|intranet|lan|local)\b(?![.\w-])");
        var ipRx = new Regex(@"\b(10\.\d{1,3}\.\d{1,3}\.\d{1,3}|192\.168\.\d{1,3}\.\d{1,3}|172\.(1[6-9]|2\d|3[01])\.\d{1,3}\.\d{1,3})\b");
        foreach (var a in added)
        {
            if (hostRx.Match(a.Text) is { Success: true } h) hosts.Add($"{a.Commit} {a.Path}:{a.Line}  {h.Value}");
            if (ipRx.Match(a.Text) is { Success: true } ip) hosts.Add($"{a.Commit} {a.Path}:{a.Line}  {ip.Value}");
        }
        Emit(r, "Hosts/IPs", hosts, "no internal host names or private addresses");

        var denyFile = o.Get("deny");
        if (denyFile is null)
            r.Row("Deny terms", "WARN", "no --deny list: employer, client and colleague names were not checked");
        else
        {
            if (!File.Exists(denyFile)) throw new UsageException($"deny list not found: {denyFile}");
            var full = Path.GetFullPath(denyFile);
            var repoFull = Path.GetFullPath(repo).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            if (full.StartsWith(repoFull, StringComparison.OrdinalIgnoreCase))
                r.Row("Deny list", "FAIL", "the deny list is inside the demo repository: publishing it publishes the names it protects");
            var terms = File.ReadAllLines(denyFile).Select(t => t.Trim()).Where(t => t.Length > 0 && !t.StartsWith('#')).ToList();
            var hits = new List<string>();
            foreach (var t in terms)
            {
                foreach (var a in added.Where(a => a.Text.Contains(t, StringComparison.OrdinalIgnoreCase)))
                    hits.Add($"{a.Commit} {a.Path}:{a.Line}  \"{t}\"");
                foreach (var p in paths.Where(p => p.Contains(t, StringComparison.OrdinalIgnoreCase)))
                    hits.Add($"path {p}  \"{t}\"");
                foreach (var c in commits.Where(c => (c.Subject + " " + c.Author + " " + c.Email).Contains(t, StringComparison.OrdinalIgnoreCase)))
                    hits.Add($"{c.Hash[..7]} commit metadata  \"{t}\"");
            }
            Emit(r, "Deny terms", hits, $"0 of {terms.Count} terms found");
        }

        return r.Finish("safe to publish", "do NOT publish");
    }

    static void Emit(Report r, string label, List<string> findings, string okText)
    {
        var distinct = findings.Distinct().ToList();
        r.Row(label, distinct.Count == 0 ? "PASS" : "FAIL", distinct.Count == 0 ? okText : $"{distinct.Count} finding(s)");
        foreach (var f in distinct.Take(8)) r.Detail(f);
        if (distinct.Count > 8) r.Detail($"... and {distinct.Count - 8} more");
    }

    static string Mask(string s) => s.Length <= 8 ? new string('*', s.Length) : s[..6] + new string('*', Math.Min(12, s.Length - 6));

    sealed record Added(string Commit, string Path, int Line, string Text);

    /// <summary>Every line ever added, in every commit on every ref: a secret deleted at HEAD is still published.</summary>
    static List<Added> AddedLines(string repo)
    {
        var result = new List<Added>();
        string commit = "", path = "";
        int line = 0;
        var hunk = new Regex(@"^@@ -\d+(,\d+)? \+(\d+)(,\d+)? @@");
        foreach (var raw in Git.Run(repo, "log", "-p", "--all", "--no-color", "--no-ext-diff", "--format=@@commit %h").Split('\n'))
        {
            var l = raw.TrimEnd('\r');
            if (l.StartsWith("@@commit ")) { commit = l[9..]; continue; }
            if (l.StartsWith("+++ ")) { path = l.StartsWith("+++ b/") ? l[6..] : l[4..]; continue; }
            if (l.StartsWith("--- ")) continue;
            var h = hunk.Match(l);
            if (h.Success) { line = int.Parse(h.Groups[2].Value); continue; }
            if (l.StartsWith('+')) { result.Add(new(commit, path, line, l[1..])); line++; }
            else if (l.StartsWith(' ')) line++;
        }
        return result;
    }
}

static class Before
{
    public static int Run(Options o)
    {
        var repo = o.Arg(0, "repository path");
        Git.RequireRepo(repo);
        var cfg = DemoConfig.Load(repo, o.Get("config"), required: true);
        if (cfg.LayerPaths.Count == 0) throw new UsageException("config has no layerPaths");
        var r = new Report();
        Console.WriteLine($"DemoCheck before  {repo}  ({cfg.BeforeTag} -> {cfg.AfterTag}, demo ticket {cfg.DemoTicket})");
        Console.WriteLine();

        string? TagDate(string tag) =>
            Git.TryRun(repo, out var d, "log", "-1", "--date=short", "--format=%ad", $"refs/tags/{tag}") ? d.Trim() : null;
        var bDate = TagDate(cfg.BeforeTag);
        var aDate = TagDate(cfg.AfterTag);
        r.Row("Tags", bDate is not null && aDate is not null ? "PASS" : "FAIL",
            $"{cfg.BeforeTag}: {bDate ?? "missing"}; {cfg.AfterTag}: {aDate ?? "missing"}");
        if (bDate is null || aDate is null) return r.Finish("", "not a before/after pair");

        if (!Git.TryRun(repo, out _, "merge-base", "--is-ancestor", cfg.BeforeTag, cfg.AfterTag))
            r.Row("Order", "FAIL", $"{cfg.BeforeTag} is not an ancestor of {cfg.AfterTag}");

        var beforeFiles = Git.Lines(Git.Run(repo, "ls-tree", "-r", "--name-only", cfg.BeforeTag));
        var layerAtBefore = beforeFiles.Where(cfg.IsLayer).Where(f => !cfg.Placeholders.Contains(f)).ToList();
        r.Row("Empty layer", layerAtBefore.Count == 0 ? "PASS" : "FAIL",
            layerAtBefore.Count == 0
                ? $"at {cfg.BeforeTag}: no AI-layer files" + (beforeFiles.Any(cfg.Placeholders.Contains) ? $" (placeholder: {string.Join(", ", beforeFiles.Where(cfg.Placeholders.Contains))})" : "")
                : $"at {cfg.BeforeTag}: {layerAtBefore.Count} AI-layer file(s) already present -- the 'before' is not before");
        foreach (var f in layerAtBefore.Take(6)) r.Detail(f);

        var diff = Git.Lines(Git.Run(repo, "diff", "--name-only", cfg.BeforeTag, cfg.AfterTag));
        var outside = diff.Where(f => !cfg.IsLayer(f)).ToList();
        r.Row("Layer-only", outside.Count == 0 ? "PASS" : "FAIL",
            outside.Count == 0
                ? $"{diff.Count} files changed between the tags, all under layer paths"
                : $"{outside.Count} of {diff.Count} changed files are app code or company material -- the before/after is confounded");
        foreach (var f in outside.Take(6)) r.Detail(f);
        var afterLayer = Git.Lines(Git.Run(repo, "ls-tree", "-r", "--name-only", cfg.AfterTag)).Count(cfg.IsLayer);
        if (afterLayer <= cfg.Placeholders.Count)
            r.Row("Layer", "FAIL", $"{cfg.AfterTag} has no AI layer");

        if (cfg.DemoTicket != "")
        {
            var rx = new Regex($@"\b{Regex.Escape(cfg.DemoTicket)}\b");
            var hist = Git.Lines(Git.Run(repo, "log", "--format=%h %s", cfg.AfterTag)).Where(l => rx.IsMatch(l)).ToList();
            r.Row("Ticket open", hist.Count == 0 ? "PASS" : "FAIL",
                hist.Count == 0 ? $"{cfg.DemoTicket} is not in any commit message up to {cfg.AfterTag}"
                                : $"{cfg.DemoTicket} already has commits -- the audience would be watching a rerun");
            foreach (var h in hist.Take(4)) r.Detail(h);

            var ticketPath = $"{cfg.TicketsDir.TrimEnd('/')}/{cfg.DemoTicket}.md";
            var hasTicket = Git.TryRun(repo, out var ticket, "show", $"{cfg.BeforeTag}:{ticketPath}");
            r.Row("Ticket file", hasTicket && Regex.IsMatch(ticket, @"(?im)^#+\s*acceptance criteria") ? "PASS" : "FAIL",
                hasTicket ? $"{ticketPath} at {cfg.BeforeTag}" + (Regex.IsMatch(ticket, @"(?im)^#+\s*acceptance criteria") ? " with acceptance criteria" : " has no acceptance criteria")
                          : $"{ticketPath} missing at {cfg.BeforeTag}");
        }

        if (cfg.OpenCheck is { } oc && oc.Path != "")
        {
            foreach (var tag in new[] { cfg.BeforeTag, cfg.AfterTag })
            {
                var ok = Git.TryRun(repo, out var content, "show", $"{tag}:{oc.Path}") && content.Contains(oc.Contains);
                r.Row("Still open", ok ? "PASS" : "FAIL",
                    ok ? $"{tag}: {oc.Path} still contains \"{oc.Contains}\""
                       : $"{tag}: {oc.Path} no longer contains \"{oc.Contains}\" -- the demo ticket is already (partly) done");
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Run both sides:  git -C {repo} worktree add ../demo-before {cfg.BeforeTag}");
        Console.WriteLine($"                 git -C {repo} worktree add ../demo-after  {cfg.AfterTag}");
        return r.Finish("a fair before/after", "fix before you demo");
    }
}

static class Time
{
    public static int Parse(string s)
    {
        var p = s.Trim().Split(':');
        try
        {
            return p.Length switch
            {
                1 => int.Parse(p[0]) * 60,
                2 => int.Parse(p[0]) * 60 + int.Parse(p[1]),
                3 => int.Parse(p[0]) * 3600 + int.Parse(p[1]) * 60 + int.Parse(p[2]),
                _ => throw new FormatException(),
            };
        }
        catch (FormatException) { throw new UsageException($"bad time '{s}' (use mm:ss)"); }
    }

    public static string Fmt(int sec) => $"{sec / 60:00}:{sec % 60:00}";
}

static class Timeline
{
    sealed record Ev(int T, string Kind, string Note, int LineNo);

    static readonly HashSet<string> Kinds =
        ["start", "say", "prompt", "wait", "output", "stuck", "help", "error", "recover", "fallback", "pr", "green", "cut", "end"];

    public static int Run(Options o)
    {
        var file = o.Arg(0, "timeline file");
        if (!File.Exists(file)) throw new UsageException($"not found: {file}");
        var mode = o.Get("mode") ?? "recording";
        if (mode is not ("recording" or "stranger")) throw new UsageException("--mode is recording or stranger");
        var r = new Report();
        var evs = new List<Ev>();
        var lines = File.ReadAllLines(file);
        for (int i = 0; i < lines.Length; i++)
        {
            var l = lines[i];
            if (l.Trim().Length == 0 || l.TrimStart().StartsWith('#')) continue;
            var p = l.Split('\t');
            if (p.Length < 2) throw new UsageException($"{file}:{i + 1}: expected 'mm:ss<TAB>kind<TAB>note'");
            var kind = p[1].Trim().ToLowerInvariant();
            if (!Kinds.Contains(kind)) throw new UsageException($"{file}:{i + 1}: unknown kind '{kind}'");
            evs.Add(new(Time.Parse(p[0]), kind, p.Length > 2 ? p[2].Trim() : "", i + 1));
        }
        if (evs.Count == 0) throw new UsageException("empty timeline");
        Console.WriteLine($"DemoCheck timeline  {file}  ({mode})");
        Console.WriteLine();

        var backwards = evs.Zip(evs.Skip(1)).Where(x => x.Second.T < x.First.T).ToList();
        var cuts = evs.Where(e => e.Kind == "cut").ToList();
        var endT = evs.LastOrDefault(e => e.Kind == "end")?.T ?? evs[^1].T;
        var startT = evs.FirstOrDefault(e => e.Kind == "start")?.T ?? evs[0].T;
        int? pr = evs.FirstOrDefault(e => e.Kind == "pr")?.T;
        int? green = evs.FirstOrDefault(e => e.Kind == "green")?.T;

        if (mode == "recording")
        {
            int min = Time.Parse(o.Get("min") ?? "15:00"), max = Time.Parse(o.Get("max") ?? "20:00");
            var dur = endT - startT;
            r.Row("Duration", dur >= min && dur <= max ? "PASS" : "WARN",
                $"{Time.Fmt(dur)} (target {Time.Fmt(min)}-{Time.Fmt(max)})");

            var editProblems = new List<string>();
            foreach (var (a, b) in backwards) editProblems.Add($"line {b.LineNo}: time goes back from {Time.Fmt(a.T)} to {Time.Fmt(b.T)}");
            foreach (var c in cuts) editProblems.Add($"line {c.LineNo}: cut at {Time.Fmt(c.T)} {c.Note}");
            if (o.Get("file-duration") is { } fd && Math.Abs(Time.Parse(fd) - endT) > 5)
                editProblems.Add($"the file lasts {fd} but the timeline ends at {Time.Fmt(endT)} -- trimmed or spliced");
            r.Row("Unedited", editProblems.Count == 0 ? "PASS" : "FAIL",
                editProblems.Count == 0 ? "no cuts, time only moves forward" + (o.Get("file-duration") is { } f2 ? $", covers the whole file ({f2})" : "")
                                        : $"{editProblems.Count} sign(s) of editing");
            foreach (var e in editProblems) r.Detail(e);

            r.Row("Outcome", pr is not null && green is not null ? "PASS" : "FAIL",
                $"tests green: {(green is { } gt ? Time.Fmt(gt) : "never")}; PR: {(pr is { } pt ? Time.Fmt(pt) : "never")}");

            Recoveries(r, evs, endT);

            int maxGap = int.Parse(o.Get("max-gap") ?? "45");
            var gaps = new List<(int At, int Len)>();
            for (int i = 0; i + 1 < evs.Count; i++)
            {
                var len = evs[i + 1].T - evs[i].T;
                if (evs[i].Kind is "end" or "cut") continue;
                var limit = evs[i].Kind == "say" ? 3 * maxGap : maxGap;
                if (len > limit) gaps.Add((evs[i].T, len));
            }
            var longest = evs.Zip(evs.Skip(1)).Where(x => x.First.Kind != "say").Select(x => (At: x.First.T, Len: x.Second.T - x.First.T)).DefaultIfEmpty().MaxBy(x => x.Len);
            r.Row("Silent gaps", gaps.Count == 0 ? "PASS" : "WARN",
                gaps.Count == 0 ? $"none over {maxGap} s; longest unnarrated stretch {longest.Len} s at {Time.Fmt(longest.At)}"
                                : $"{gaps.Count} over {maxGap} s, {gaps.Sum(g => g.Len)} s in total -- say what the agent is doing, or cut to a question");
            foreach (var g in gaps) r.Detail($"{Time.Fmt(g.At)}  {g.Len} s");

            var prompts = evs.Where(e => e.Kind == "prompt").ToList();
            var off = prompts.Where(p => p.Note.Contains("[off-script]")).ToList();
            r.Row("Prompts", off.Count == 0 ? "PASS" : "WARN",
                $"{prompts.Count} typed, {off.Count} off-script" + (off.Count > 0 ? " -- every off-script prompt is a step the run sheet is missing" : ""));
            foreach (var p in off) r.Detail($"{Time.Fmt(p.T)}  {p.Note}");
            return r.Finish("unedited run", "not an unedited run");
        }
        else
        {
            int limit = Time.Parse(o.Get("limit") ?? "30:00");
            var helps = evs.Where(e => e.Kind == "help").ToList();
            r.Row("Unaided", helps.Count == 0 ? "PASS" : "FAIL",
                helps.Count == 0 ? "no help given" : $"{helps.Count} time(s) the owner helped -- the test stops counting at the first one");
            foreach (var h in helps) r.Detail($"{Time.Fmt(h.T)}  {h.Note}");
            var firstHelp = helps.FirstOrDefault()?.T;

            var done = pr is not null && green is not null ? Math.Max(pr.Value, green.Value) - startT : (int?)null;
            var ok = done is not null && done <= limit && (firstHelp is null || Math.Max(pr!.Value, green!.Value) < firstHelp);
            r.Row("Working PR", ok ? "PASS" : "FAIL",
                done is null ? "no PR with green tests" :
                $"PR with green tests at {Time.Fmt(done.Value)} (limit {Time.Fmt(limit)})" +
                (firstHelp is not null && Math.Max(pr!.Value, green!.Value) >= firstHelp ? ", but only after help" : ""));

            Recoveries(r, evs, endT);

            var stuck = evs.Where(e => e.Kind is "stuck" or "help").ToList();
            Console.WriteLine();
            Console.WriteLine($"Weak points ({stuck.Count} stuck/help events, by component)");
            foreach (var grp in stuck.GroupBy(s => Regex.Match(s.Note, @"^\[([^\]]+)\]") is { Success: true } m ? m.Groups[1].Value : "untagged")
                                     .OrderByDescending(g => g.Count()))
            {
                Console.WriteLine($"  {grp.Key,-16}{grp.Count()}");
                foreach (var s in grp) Console.WriteLine($"      {Time.Fmt(s.T)}  {s.Kind,-6} {Regex.Replace(s.Note, @"^\[[^\]]+\]\s*", "")}");
            }
            return r.Finish("stranger test passed", "stranger test failed");
        }
    }

    static void Recoveries(Report r, List<Ev> evs, int endT)
    {
        var errors = evs.Where(e => e.Kind == "error").ToList();
        var bad = new List<string>();
        var times = new List<int>();
        foreach (var e in errors)
        {
            var rec = evs.FirstOrDefault(x => x.T >= e.T && x.LineNo > e.LineNo && x.Kind is "recover" or "fallback");
            if (rec is null) bad.Add($"{Time.Fmt(e.T)}  {e.Note} -- never recovered");
            else times.Add(rec.T - e.T);
        }
        r.Row("Recoveries", bad.Count == 0 ? "PASS" : "FAIL",
            errors.Count == 0 ? "no errors on this run" :
            $"{errors.Count} error(s), {times.Count} recovered" + (times.Count > 0 ? $", slowest in {times.Max()} s" : ""));
        foreach (var b in bad) r.Detail(b);
    }
}

static class Stats
{
    /// <summary>Wilson score interval, z = 1.96.</summary>
    public static (double Lo, double Hi) Wilson(int k, int n)
    {
        if (n == 0) return (0, 1);
        const double z = 1.96;
        double p = (double)k / n, z2 = z * z;
        double center = (p + z2 / (2 * n)) / (1 + z2 / n);
        double half = z * Math.Sqrt(p * (1 - p) / n + z2 / (4.0 * n * n)) / (1 + z2 / n);
        return (Math.Max(0, center - half), Math.Min(1, center + half));
    }
}

sealed record Segment(string No, string Name, string Mode, int BudgetSec, int? K, int? N, string Fallback, int RecoverySec, string Say);

static class RunSheetParser
{
    public static (List<Segment> Segs, int? SlotSec) Parse(string file)
    {
        if (!File.Exists(file)) throw new UsageException($"not found: {file}");
        var lines = File.ReadAllLines(file);
        int? slot = null;
        foreach (var l in lines)
            if (Regex.Match(l, @"(?i)^\s*\**slot\**\s*:\s*\**\s*(\d+)\s*min") is { Success: true } m) slot = int.Parse(m.Groups[1].Value) * 60;

        var segs = new List<Segment>();
        string[]? header = null;
        foreach (var l in lines)
        {
            if (!l.TrimStart().StartsWith('|')) { header = null; continue; }
            var cells = l.Trim().Trim('|').Split('|').Select(c => c.Trim()).ToArray();
            if (cells.All(c => Regex.IsMatch(c, @"^:?-{2,}:?$"))) continue;
            if (header is null)
            {
                if (cells.Any(c => c.Equals("Mode", StringComparison.OrdinalIgnoreCase))) header = cells.Select(c => c.ToLowerInvariant()).ToArray();
                continue;
            }
            string Col(string name) { var i = Array.FindIndex(header, h => h.StartsWith(name)); return i >= 0 && i < cells.Length ? cells[i] : ""; }
            var reh = Regex.Match(Col("rehears"), @"(\d+)\s*/\s*(\d+)");
            var budget = Col("budget").Replace("min", "").Trim();
            var rec = Col("recover").Replace("min", "").Trim();
            segs.Add(new Segment(
                Col("#"), Col("segment"), Col("mode").ToLowerInvariant(),
                Empty(budget) ? 0 : Time.Parse(budget),
                reh.Success ? int.Parse(reh.Groups[1].Value) : null,
                reh.Success ? int.Parse(reh.Groups[2].Value) : null,
                Col("fallback").Replace("`", ""),
                Empty(rec) ? 120 : Time.Parse(rec),
                Col("if it breaks")));
        }
        if (segs.Count == 0) throw new UsageException($"{file}: no run-sheet table (needs a header row with a Mode column)");
        return (segs, slot);
    }

    public static bool Empty(string s) => s is "" or "-" or "—" or "none" or "n/a" or "TBD" or "tbd";
}

static class RunSheet
{
    public static int Run(Options o)
    {
        var file = o.Arg(0, "run sheet");
        var repo = o.Get("repo");
        var (segs, slot) = RunSheetParser.Parse(file);
        var r = new Report();
        Console.WriteLine($"DemoCheck runsheet  {file}");
        Console.WriteLine();
        Console.WriteLine($"{"#",-4}{"Segment",-34}{"Mode",-10}{"Budget",-8}{"Rehearsed",-11}{"p",-6}{"95% interval",-14}Fallback");
        foreach (var s in segs)
        {
            var p = s.K is { } k && s.N is { } n && n > 0 ? $"{(double)k / n:0.00}" : "";
            var ci = s.K is { } k2 && s.N is { } n2 && n2 > 0 ? Stats.Wilson(k2, n2) is var w ? $"{w.Lo:0.00}-{w.Hi:0.00}" : "" : "";
            Console.WriteLine($"{s.No,-4}{Trunc(s.Name, 33),-34}{s.Mode,-10}{Time.Fmt(s.BudgetSec),-8}{(s.N is null ? "-" : $"{s.K}/{s.N}"),-11}{p,-6}{ci,-14}{Trunc(s.Fallback, 40)}");
        }
        Console.WriteLine();

        var live = segs.Where(s => s.Mode == "live").ToList();
        var noFallback = live.Where(s => RunSheetParser.Empty(s.Fallback)).ToList();
        r.Row("Fallbacks", noFallback.Count == 0 ? "PASS" : "FAIL",
            noFallback.Count == 0 ? $"all {live.Count} live segments have one" : $"{noFallback.Count} live segment(s) without a fallback");
        foreach (var s in noFallback) r.Detail($"#{s.No} {s.Name}");

        var noSay = live.Where(s => RunSheetParser.Empty(s.Say)).ToList();
        r.Row("Say line", noSay.Count == 0 ? "PASS" : "FAIL",
            noSay.Count == 0 ? "every live segment says what you tell the room when it breaks" : $"{noSay.Count} live segment(s) with nothing to say when it breaks");
        foreach (var s in noSay) r.Detail($"#{s.No} {s.Name}");

        var unrehearsed = live.Where(s => s.N is null).ToList();
        var thin = live.Where(s => s.N is > 0 and < 3).ToList();
        r.Row("Rehearsed", unrehearsed.Count > 0 ? "FAIL" : thin.Count > 0 ? "WARN" : "PASS",
            unrehearsed.Count > 0 ? $"{unrehearsed.Count} live segment(s) never rehearsed" :
            thin.Count > 0 ? $"{thin.Count} live segment(s) rehearsed fewer than 3 times" : "every live segment rehearsed at least 3 times");
        foreach (var s in unrehearsed.Concat(thin)) r.Detail($"#{s.No} {s.Name}");

        if (repo is not null)
        {
            Git.RequireRepo(repo);
            var missing = new List<string>();
            foreach (var s in segs)
            {
                foreach (Match m in Regex.Matches(s.Fallback, @"\b(tag|branch):([\w./-]+)"))
                {
                    var refName = m.Groups[1].Value == "tag" ? $"refs/tags/{m.Groups[2].Value}" : $"refs/heads/{m.Groups[2].Value}";
                    if (!Git.TryRun(repo, out _, "rev-parse", "--verify", "--quiet", refName)) missing.Add($"#{s.No} {m.Value} does not exist");
                }
                foreach (Match m in Regex.Matches(s.Fallback, @"\brec:([^\s@]+)(@\d+:\d\d)?"))
                    if (!File.Exists(Path.Combine(repo, m.Groups[1].Value))) missing.Add($"#{s.No} {m.Groups[1].Value} is not in the repo");
            }
            r.Row("Fallback ref", missing.Count == 0 ? "PASS" : "FAIL",
                missing.Count == 0 ? "every tag, branch and recording named exists" : $"{missing.Count} fallback(s) point at nothing");
            foreach (var m in missing) r.Detail(m);
        }

        var total = segs.Sum(s => s.BudgetSec);
        if (slot is { } sl)
        {
            var buffer = sl - total;
            r.Row("Budget", total > sl ? "FAIL" : buffer < sl / 10 ? "WARN" : "PASS",
                $"{Time.Fmt(total)} planned in a {Time.Fmt(sl)} slot, buffer {Time.Fmt(Math.Max(0, buffer))}" +
                (total > sl ? " -- over before anything breaks" : buffer < sl / 10 ? " -- under 10%: one failure eats the close" : ""));
        }
        else r.Row("Budget", "WARN", $"{Time.Fmt(total)} planned; no 'Slot: NN min' line to check it against");

        var rehearsedLive = live.Where(s => s.N is > 0).ToList();
        if (rehearsedLive.Count > 0)
        {
            double pClean = 1, pLo = 1, expectedLoss = 0;
            foreach (var s in rehearsedLive)
            {
                var p = (double)s.K!.Value / s.N!.Value;
                pClean *= p;
                pLo *= Stats.Wilson(s.K.Value, s.N.Value).Lo;
                expectedLoss += (1 - p) * s.RecoverySec;
            }
            r.Row("P(clean)", pClean >= 0.5 ? "PASS" : "WARN",
                $"{pClean:0.00} from rehearsals (pessimistic, product of lower bounds: {pLo:0.00}); expected time lost to recoveries {expectedLoss / 60:0.0} min" +
                (pClean < 0.5 ? " -- more likely to break than not: pre-record or pre-bake a segment" : ""));
        }
        return r.Finish("ready to rehearse", "not ready");
    }

    static string Trunc(string s, int n) => s.Length <= n ? s : s[..(n - 1)] + "…";
}

static class Drill
{
    sealed record Card(string Id, string Name, string Induce, string Symptom);

    static readonly Card[] Deck =
    [
        new("api-down", "API unreachable (stands in for a rate limit or an overload)",
            "Turn off Wi-Fi or disable the network adapter for 60 seconds, then turn it back on.",
            "requests fail and retry, then the session reports an error; edits made before the drop stay on disk"),
        new("wrong-plan", "Wrong plan",
            "Before the plan step, rename AGENTS.md to AGENTS.md.off (CLAUDE.md now imports nothing).",
            "the plan follows the stale docs/ARCHITECTURE.md: SqlHelper, DataSet, DateTime.Now"),
        new("mcp-down", "Schema server missing",
            "Rename .claude/mcp/contoso-schema to contoso-schema.off before the research step.",
            "the contoso-schema tools are gone; the agent reads migrations by hand or guesses"),
        new("stale-snapshot", "Stale schema snapshot",
            "Start the session with the schema server's --max-age-hours set to 1 and --on-stale refuse.",
            "every schema answer is refused as stale; the agent must fall back to db/migrations"),
        new("hook-deny", "A guard hook fires",
            "Ask for something the rules forbid, in passing: \"and tidy V004 while you are there\".",
            "the PreToolUse guard denies the edit and the agent reports it"),
        new("slow-run", "The agent takes three times longer",
            "Nothing to induce: pretend the segment is running long and act at the budget.",
            "you are at the segment's budget and the agent is still working"),
    ];

    public static int Run(Options o)
    {
        var file = o.Arg(0, "run sheet");
        var (segs, _) = RunSheetParser.Parse(file);
        var live = segs.Where(s => s.Mode == "live").ToList();
        if (live.Count == 0) throw new UsageException("the run sheet has no live segments to drill");
        var seed = int.Parse(o.Get("seed") ?? DateTime.UtcNow.ToString("yyyyMMdd"));
        var rnd = new Random(seed);
        var seg = live[rnd.Next(live.Count)];
        var card = Deck[rnd.Next(Deck.Length)];
        var minute = Math.Max(1, (int)Math.Round(seg.BudgetSec / 60.0 * (0.3 + 0.4 * rnd.NextDouble())));

        Console.WriteLine($"DemoCheck drill  {file}  (seed {seed})");
        Console.WriteLine();
        Console.WriteLine($"Segment   #{seg.No} {seg.Name} (budget {Time.Fmt(seg.BudgetSec)})");
        Console.WriteLine($"When      about {minute} min into the segment");
        Console.WriteLine($"Failure   {card.Name}  [{card.Id}]");
        Console.WriteLine($"Induce    {card.Induce}");
        Console.WriteLine($"Expect    {card.Symptom}");
        Console.WriteLine();
        Console.WriteLine("Your run sheet says");
        Console.WriteLine($"  Say       {(RunSheetParser.Empty(seg.Say) ? "(nothing -- write it now)" : seg.Say)}");
        Console.WriteLine($"  Fallback  {(RunSheetParser.Empty(seg.Fallback) ? "(nothing -- write it now)" : seg.Fallback)}");
        Console.WriteLine($"  Recovery  {Time.Fmt(seg.RecoverySec)} budgeted to switch");
        Console.WriteLine();
        Console.WriteLine("Log it in the timeline as 'error' at the moment it shows, and 'recover' or 'fallback' when you are back.");
        return RunSheetParser.Empty(seg.Fallback) || RunSheetParser.Empty(seg.Say) ? 1 : 0;
    }
}
