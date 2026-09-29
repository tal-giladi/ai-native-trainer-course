// EngageCheck — the Module 21 tool. Dependency-free, read-only, deterministic.
//
//   kickoff   <engagement-record.md>
//             Stakeholder map (sponsor, owner, developers, security or IT, the business people who live with
//             the output), high-influence people interviewed and planned for, an acceptor per SOW deliverable
//             who is not the consultant, dated access, data handling, a premortem with owners, a success
//             measure that is a measurement and not a promise (21.1).
//   baseline  <engagement-record.md>
//             Audit findings with evidence rungs (confirmed = two sources, one from rungs 1-3), and a baseline
//             captured before the build started: definitions, sources, a long enough window, a spread, a
//             guardrail metric, no per-person metrics, the smallest detectable effect (21.2).
//   build     <engagement-record.md> --log <prs.csv>
//             With the team, not for them: client-authored share of AI-layer changes by thirds of the build,
//             client review of every consultant change, client truck factor over the AI-layer files,
//             decisions taken by client people (21.3).
//   handover  <engagement-record.md>
//             Owners who are employees and have already run each rhythm without the consultant, results with
//             intervals and a status that matches them, "what this does not show", follow-ups at 30/60/90
//             days, access revoked and data deleted (21.4).
//   casestudy <case-study.md> --record <engagement-record.md> [--deny <terms.txt>]
//             Anonymized (no client or stakeholder names, e-mails, tracker keys; bands instead of exact
//             counts), approved in writing, every number traceable to the record's results, intervals next
//             to effects, no claims the record does not support, limitations stated (21.5).
//
// Exit code: 0 = no errors, 1 = errors found, 2 = usage or input problem.
using System.Globalization;
using System.Text.RegularExpressions;

static class Program
{
    static int errors, warnings;
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    const RegexOptions I = RegexOptions.IgnoreCase;

    static int Main(string[] args)
    {
        CultureInfo.CurrentCulture = Inv;
        if (args.Length == 0 || args[0] is "-h" or "--help") { Usage(); return 0; }
        try
        {
            var rest = args.Skip(1).ToArray();
            if (rest.Length == 0) throw new UsageException("missing file argument (see --help)");
            switch (args[0])
            {
                case "kickoff": Kickoff(Rec.Load(NeedFile(rest[0]))); break;
                case "baseline": Baseline(Rec.Load(NeedFile(rest[0]))); break;
                case "build": Build(Rec.Load(NeedFile(rest[0])), NeedFile(Opt(rest, "--log") ?? throw new UsageException("build needs --log <prs.csv>"))); break;
                case "handover": Handover(Rec.Load(NeedFile(rest[0]))); break;
                case "casestudy":
                    {
                        var rec = Opt(rest, "--record") ?? throw new UsageException("casestudy needs --record <engagement-record.md>");
                        var deny = Opt(rest, "--deny");
                        CaseStudy(NeedFile(rest[0]), Rec.Load(NeedFile(rec)), deny is null ? null : NeedFile(deny));
                        break;
                    }
                default: Console.Error.WriteLine($"unknown command '{args[0]}'"); Usage(); return 2;
            }
        }
        catch (UsageException e) { Console.Error.WriteLine(e.Message); return 2; }
        catch (IOException e) { Console.Error.WriteLine(e.Message); return 2; }
        Console.WriteLine();
        Console.WriteLine($"{errors} error(s), {warnings} warning(s)");
        return errors > 0 ? 1 : 0;
    }

    static void Usage() => Console.WriteLine(
        "EngageCheck <command> <file>\n" +
        "  kickoff   <engagement-record.md>                       stakeholders, acceptors, access, premortem (21.1)\n" +
        "  baseline  <engagement-record.md>                       audit findings and baseline (21.2)\n" +
        "  build     <engagement-record.md> --log <prs.csv>       with the team, not for them (21.3)\n" +
        "  handover  <engagement-record.md>                       owners, results, follow-ups (21.4)\n" +
        "  casestudy <case-study.md> --record <record.md> [--deny <terms.txt>]   anonymized and traceable (21.5)");

    static string? Opt(string[] a, string name)
    {
        for (int i = 1; i < a.Length; i++)
            if (a[i] == name) return i + 1 < a.Length ? a[i + 1] : throw new UsageException($"{name} needs a value");
        return null;
    }

    static void Header(string s) { Console.WriteLine(); Console.WriteLine($"== {s}"); }
    static void Err(string m) { errors++; Console.WriteLine($"ERROR {m}"); }
    static void Warn(string m) { warnings++; Console.WriteLine($"WARN  {m}"); }
    static void Info(string m) => Console.WriteLine($"      {m}");

    static string NeedFile(string p) => File.Exists(p) ? p : throw new UsageException($"file not found: {p}");

    // ---------------------------------------------------------------- record parsing

    sealed class Table
    {
        public List<string> Head = [];
        public List<List<string>> Rows = [];
        public int Col(string prefix) => Head.FindIndex(h => h.StartsWith(prefix));
        public string Get(List<string> r, string prefix) { var i = Col(prefix); return i >= 0 && i < r.Count ? r[i] : ""; }
    }

    sealed class Rec
    {
        public Dictionary<string, string> Facts = new(StringComparer.OrdinalIgnoreCase);
        public List<Table> Tables = [];
        public string[] Lines = [];

        public static Rec Load(string path)
        {
            var lines = File.ReadAllLines(path);
            var p = new Rec { Lines = lines };
            foreach (var l in lines)
            {
                var m = Regex.Match(l, @"^\s*[-*]\s+\**([A-Za-z][A-Za-z /()-]+?)\**:\s*(.+?)\s*$");
                if (m.Success && !l.TrimStart().StartsWith("|")) p.Facts.TryAdd(m.Groups[1].Value.Trim(), m.Groups[2].Value.Trim());
            }
            for (int i = 0; i + 1 < lines.Length; i++)
            {
                if (!lines[i].TrimStart().StartsWith("|") || !Regex.IsMatch(lines[i + 1], @"^\s*\|[\s:|-]+\|\s*$")) continue;
                var t = new Table { Head = Cells(lines[i]).Select(c => c.ToLowerInvariant()).ToList() };
                int j = i + 2;
                for (; j < lines.Length && lines[j].TrimStart().StartsWith("|"); j++) t.Rows.Add(Cells(lines[j]));
                p.Tables.Add(t);
                i = j - 1;
            }
            return p;
        }

        static List<string> Cells(string line)
        {
            var s = line.Trim();
            if (s.StartsWith("|")) s = s[1..];
            if (s.EndsWith("|")) s = s[..^1];
            return s.Split('|').Select(c => c.Trim().Replace("**", "").Replace("`", "")).ToList();
        }

        public Table? Find(params string[] cols) => Tables.FirstOrDefault(t => cols.All(c => t.Col(c) >= 0));
        public string Fact(string k) => Facts.TryGetValue(k, out var v) ? v : "";
    }

    static bool Blank(string s) => string.IsNullOrWhiteSpace(s) || s.Trim() is "-" or "—" or "?" || Regex.IsMatch(s, @"^\s*(tbd|tba|n/?a|none|todo)\b|^\s*no\.?\s*$", I);

    static DateOnly? Date(string s)
    {
        var m = Regex.Match(s, @"\b(\d{4}-\d{2}-\d{2})\b");
        return m.Success && DateOnly.TryParseExact(m.Groups[1].Value, "yyyy-MM-dd", Inv, DateTimeStyles.None, out var d) ? d : null;
    }

    static string ConsultantName(Rec p) => Regex.Replace(p.Fact("Consultant"), @"\(.*?\)", "").Trim();

    static bool IsConsultant(Rec p, string cell)
    {
        if (Regex.IsMatch(cell, @"\b(consultant|supplier|trainer)\b", I)) return true;
        var c = ConsultantName(p);
        if (c.Length > 0 && Regex.IsMatch(cell, $@"\b{Regex.Escape(c)}\b", I)) return true;
        var h = Regex.Match(p.Fact("Consultant"), @"\((.+?)\)");
        return h.Success && Regex.IsMatch(cell, $@"(^|[\s,(]){Regex.Escape(h.Groups[1].Value)}($|[\s,)])");
    }

    static readonly Regex Hype = new(
        @"\b(guarantee[sd]?|10x|100\s?%|zero risk|risk-free|completely safe|never (fails?|breaks?)|always (works|right)|will (be|get|become|make \w+) \d+\s?% (faster|better|more)|at least \d+\s?% (faster|better|fewer|more))",
        I);

    static string Short(string s) => s.Length > 60 ? s[..57] + "..." : s;

    // ---------------------------------------------------------------- kickoff (21.1)

    static readonly (string Group, string Pattern, string Why)[] Groups =
    [
        ("sponsor", @"sponsor|head of|vp|cto|director", "someone who owns the outcome, signs acceptance and can remove obstacles"),
        ("technical owner", @"lead|owner|architect", "the person who will own the AI layer after you leave"),
        ("developer", @"develop|engineer", "the people whose daily work changes"),
        ("security or IT", @"secur|\bit\b|ciso|infra", "access, data handling and agent permissions; a late veto stops the build"),
        ("business user of the output", @"product|finance|business|operations|support|controller|collections|customer", "people who live with the software's output: they can veto a change the engineers think is safe"),
    ];

    static void Kickoff(Rec p)
    {
        foreach (var f in new[] { "Client code", "Sponsor", "SOW" })
            if (Blank(p.Fact(f))) Err($"no '{f}:' line");
        var kick = Date(p.Fact("Kickoff date"));
        if (kick is null) Err("no 'Kickoff date:' (YYYY-MM-DD)");

        var t = p.Find("stakeholder", "role", "influence");
        if (t is null) Err("no stakeholder table (columns Stakeholder | Role | Influence | Interest | Wants | Worries | Success in their words | Interviewed | Plan)");
        else
        {
            var roles = t.Rows.Select(r => t.Get(r, "role")).ToList();
            foreach (var (g, pat, why) in Groups)
                if (!roles.Any(x => Regex.IsMatch(x, pat, I))) Err($"no {g} in the stakeholder map: {why}");
            if (t.Rows.Count < 5) Warn($"only {t.Rows.Count} stakeholders: most engagements have at least five people who can stop or sink them");
            foreach (var r in t.Rows)
            {
                var who = t.Get(r, "stakeholder");
                bool high = Regex.IsMatch(t.Get(r, "influence"), @"^\s*(h|high)", I);
                if (high && Blank(t.Get(r, "plan"))) Err($"{who}: high influence and no engagement plan");
                if (high && t.Col("interviewed") >= 0)
                {
                    var d = Date(t.Get(r, "interviewed"));
                    if (d is null) Err($"{who}: high influence and not interviewed");
                    else if (kick is not null && d > kick) Warn($"{who}: interviewed after kickoff ({d:yyyy-MM-dd}); their worries could not shape the kickoff");
                }
                var h = Hype.Match(t.Get(r, "success"));
                if (h.Success) Err($"{who}: success written as a promise (\"{h.Value}\"); write what they would see, not what you guarantee");
                if (Blank(t.Get(r, "worries")) && Blank(t.Get(r, "wants"))) Warn($"{who}: neither wants nor worries recorded");
            }
            Info($"{t.Rows.Count} stakeholders");
        }

        var a = p.Find("deliverable", "accepted by");
        if (a is null) Err("no acceptance table (Deliverable | Acceptance criterion | Accepted by | Due): who signs each SOW deliverable?");
        else foreach (var r in a.Rows)
            {
                var d = a.Get(r, "deliverable"); var by = a.Get(r, "accepted by");
                if (Blank(by)) Err($"{Short(d)}: nobody accepts it");
                else if (IsConsultant(p, by)) Err($"{Short(d)}: accepted by the consultant; acceptance belongs to the client");
                else if (Regex.IsMatch(by, @"^\s*(the )?(client|team|everyone|all|stakeholders)\s*$", I)) Err($"{Short(d)}: accepted by \"{by}\"; name one role");
                if (a.Col("acceptance") >= 0 && Blank(a.Get(r, "acceptance"))) Err($"{Short(d)}: no acceptance criterion");
            }

        if (Date(p.Fact("Access")) is null) Err("'Access:' has no date: undated access is the most common reason week 1 is lost");
        if (Blank(p.Fact("Data handling"))) Err("no 'Data handling:' line: where client code, tickets and exports may live, and when they are deleted");
        if (Blank(p.Fact("Cadence"))) Warn("no 'Cadence:' line (weekly check-in, steering, demo)");
        if (Blank(p.Fact("Escalation"))) Warn("no 'Escalation:' line: what happens when a client responsibility slips");

        var pm = p.Find("risk", "owner");
        if (pm is null) Err("no premortem table (Risk | Early signal | Owner | Mitigation)");
        else
        {
            if (pm.Rows.Count < 3) Err($"premortem has {pm.Rows.Count} risks; imagine the engagement failed and write at least three reasons");
            foreach (var r in pm.Rows)
            {
                if (Blank(pm.Get(r, "owner"))) Err($"risk \"{Short(pm.Get(r, "risk"))}\" has no owner");
                if (pm.Col("early") >= 0 && Blank(pm.Get(r, "early"))) Warn($"risk \"{Short(pm.Get(r, "risk"))}\" has no early signal: how would you notice it in week 2?");
            }
        }

        var sm = p.Fact("Success measure");
        if (Blank(sm)) Err("no 'Success measure:' line (restate the SOW's measurement)");
        else if (Hype.IsMatch(sm)) Err($"success measure is a promise (\"{Hype.Match(sm).Value}\"): the SOW measures, it does not guarantee");
        if (Blank(p.Fact("Detectable effect"))) Warn("no 'Detectable effect:' line: tell the sponsor at kickoff how small an effect this engagement can see (21.2)");
    }

    // ---------------------------------------------------------------- baseline (21.2)

    static void Baseline(Rec p)
    {
        var build = Date(p.Fact("Build start"));
        if (build is null) Err("no 'Build start:' date: the baseline must be dated before it");

        var f = p.Find("finding", "evidence");
        if (f is null) Err("no audit findings table (Finding | Evidence (rungs) | Confirmed | Severity | Owner)");
        else
        {
            bool anyExecutable = false;
            foreach (var r in f.Rows)
            {
                var name = Short(f.Get(r, "finding")); var ev = f.Get(r, "evidence"); var conf = f.Get(r, "confirmed");
                var rungs = Regex.Matches(ev, @"\((\d)\)").Select(m => int.Parse(m.Groups[1].Value, Inv)).ToList();
                if (rungs.Contains(1)) anyExecutable = true;
                bool ok = rungs.Count >= 2 && rungs.Min() <= 3;
                bool claimed = Regex.IsMatch(conf, @"^\s*(yes|confirmed)", I);
                if (rungs.Count == 0) Err($"{name}: no evidence rungs, e.g. \"ConventionTests (1), ADR 0007 (3)\"");
                else if (claimed && !ok) Err($"{name}: marked confirmed on {(rungs.Count < 2 ? "one source" : "rungs " + string.Join(",", rungs))}; confirmed needs two sources, one from rungs 1-3");
                else if (!claimed && !ok && !Regex.IsMatch(conf, "hypothes", I)) Warn($"{name}: not confirmed; label it a hypothesis and say how you will test it");
                if (f.Col("owner") >= 0 && Blank(f.Get(r, "owner"))) Warn($"{name}: no client owner");
            }
            if (!anyExecutable) Warn("no finding rests on executable evidence (rung 1): run the build and tests yourself");
            if (f.Rows.Count < 4) Warn($"only {f.Rows.Count} findings");
            Info($"{f.Rows.Count} findings");
        }

        var b = p.Find("metric", "definition", "captured");
        if (b is null) { Err("no baseline table (Metric | Definition | Source | Window | n | Value | Spread | Captured)"); return; }
        bool guard = false, eval = false;
        foreach (var r in b.Rows)
        {
            var m = b.Get(r, "metric"); var def = b.Get(r, "definition"); var src = b.Get(r, "source");
            bool isEval = Regex.IsMatch(m + " " + src, @"eval|pass rate|trial", I);
            eval |= isEval;
            guard |= Regex.IsMatch(m + " " + def, @"defect|fail|revert|rework|incident", I);
            var cap = Date(b.Get(r, "captured"));
            if (cap is null) Err($"{m}: no capture date");
            else if (build is not null && cap >= build) Err($"{m}: captured {cap:yyyy-MM-dd}, on or after the build started ({build:yyyy-MM-dd}): it measures the intervention, not the before");
            if (Blank(def)) Err($"{m}: no definition (start and stop events, unit, inclusion rule)");
            if (Blank(src)) Err($"{m}: no source");
            if (Regex.IsMatch(m + " " + def, @"per (developer|person|engineer)|individual|\bby developer\b", I)) Err($"{m}: a per-person metric; measure tickets and teams, never rank people");
            if (Blank(b.Get(r, "spread"))) Err($"{m}: no spread or interval; a single number cannot tell a later change from noise");
            if (!isEval)
            {
                var w = Regex.Match(b.Get(r, "window"), @"(\d+)\s*weeks?", I);
                if (!w.Success) Warn($"{m}: window not stated in weeks");
                else if (int.Parse(w.Groups[1].Value, Inv) < 6) Err($"{m}: {w.Groups[1].Value}-week window; take at least 6 (ideally 12) weeks so one odd sprint does not become the baseline");
                if (int.TryParse(b.Get(r, "n"), NumberStyles.Integer, Inv, out var n) && n < 20) Warn($"{m}: n = {n}; with fewer than 20 observations the spread is itself very uncertain");
            }
        }
        if (!guard) Err("no guardrail metric (escaped defects, change failure, reverts, rework): speed without a quality guardrail is not a baseline");
        if (!eval) Warn("no eval baseline: run the task set on the before state (Module 7) so the AI layer's effect is measured twice");
        if (Blank(p.Fact("Detectable effect"))) Warn("no 'Detectable effect:' line: compute it from the baseline spread and the tickets the SOW window allows");
        Info($"{b.Rows.Count} baseline metrics");
    }

    // ---------------------------------------------------------------- build (21.3)

    sealed record Pr(string Id, int Week, string Author, bool ClientAuthor, string Reviewer, bool ClientReviewer, string Area, string Mode, string[] Files);

    static void Build(Rec p, string logPath)
    {
        if (Blank(p.Fact("Consultant"))) Err("no 'Consultant:' line in the record (name and handle, e.g. Student (S))");
        var lines = File.ReadAllLines(logPath).Where(l => l.Trim().Length > 0).ToList();
        const string head = "pr,week,author,author_side,reviewer,reviewer_side,area,mode,files";
        if (lines.Count < 2 || !lines[0].StartsWith(head)) throw new UsageException($"{logPath} must start with: {head}");
        var prs = lines.Skip(1).Select(l => l.Split(',')).Select(c => new Pr(c[0], int.Parse(c[1].TrimStart('W'), Inv), c[2], c[3] == "client",
            c[4], c[5] == "client", c[6], c[7], c[8].Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))).ToList();
        var layer = prs.Where(x => x.Area == "ai-layer").ToList();
        if (layer.Count == 0) { Err("no AI-layer changes in the log (area = ai-layer)"); return; }

        foreach (var x in layer.Where(x => !x.ClientAuthor && !x.ClientReviewer))
            Err($"{x.Id} (W{x.Week:00}, {x.Author}): consultant change to the AI layer merged without a client reviewer");

        int w0 = prs.Min(x => x.Week), w1 = prs.Max(x => x.Week);
        double span = (w1 - w0 + 1) / 3.0;
        Console.WriteLine("AI-layer changes by third of the build (client-authored share should rise; pair = pair or mob)");
        Console.WriteLine($"  {"third",-12} {"changes",7} {"client",7} {"share",6} {"pair",5}");
        double lastShare = 0;
        for (int k = 0; k < 3; k++)
        {
            int a = w0 + (int)Math.Round(k * span), z = w0 + (int)Math.Round((k + 1) * span) - 1;
            var part = layer.Where(x => x.Week >= a && x.Week <= z).ToList();
            int c = part.Count(x => x.ClientAuthor);
            double s = part.Count == 0 ? 0 : (double)c / part.Count;
            if (k == 2) lastShare = part.Count == 0 ? 0 : s;
            Console.WriteLine($"  W{a:00}-W{z:00}    {part.Count,7} {c,7} {s * 100,5:0}% {part.Count(x => x.Mode is "pair" or "mob"),5}");
        }
        if (lastShare < 0.7) Err($"client-authored share of AI-layer changes in the last third is {lastShare * 100:0}% (target 70% or more): the team will not own what it did not write");

        var clientAuthors = layer.Where(x => x.ClientAuthor).Select(x => x.Author).Distinct().ToList();
        Info($"client authors of AI-layer changes: {(clientAuthors.Count == 0 ? "none" : string.Join(", ", clientAuthors))}");
        if (clientAuthors.Count < 2) Err($"{clientAuthors.Count} client author(s) of the AI layer: one person is not a team");

        // Client truck factor over AI-layer files (greedy, 50% threshold, after Avelino et al. 2016).
        var knowers = new Dictionary<string, HashSet<string>>();
        foreach (var x in layer)
            foreach (var f in x.Files)
            {
                if (!knowers.TryGetValue(f, out var set)) knowers[f] = set = [];
                if (x.ClientAuthor) set.Add(x.Author);
                if (x.Mode is "pair" or "mob" && x.ClientReviewer) set.Add(x.Reviewer);
            }
        int files = knowers.Count;
        var orphan = knowers.Where(kv => kv.Value.Count == 0).Select(kv => kv.Key).ToList();
        int tf = 0;
        var k2 = knowers.ToDictionary(kv => kv.Key, kv => new HashSet<string>(kv.Value));
        while (k2.Count(kv => kv.Value.Count == 0) * 2 <= files)
        {
            var top = k2.SelectMany(kv => kv.Value).GroupBy(x => x).OrderByDescending(g => g.Count()).ThenBy(g => g.Key).FirstOrDefault();
            if (top is null) break;
            foreach (var kv in k2) kv.Value.Remove(top.Key);
            tf++;
        }
        Info($"AI-layer files: {files}; known only to the consultant: {orphan.Count}; client truck factor: {tf}");
        foreach (var o in orphan.Take(5)) Warn($"{o}: no client person has written or paired on it");
        if (tf < 2) Err($"client truck factor {tf}: if {(tf == 0 ? "the consultant leaves" : "one person leaves")}, more than half of the AI layer has nobody who has worked on it");

        if (!layer.Any(x => x.Mode is "pair" or "mob")) Warn("no pairing or mobbing on the AI layer: review alone transfers less than building together");

        var d = p.Find("decision", "decided by");
        if (d is null) Warn("no decisions table (Decision | Decided by | Record)");
        else foreach (var r in d.Rows)
            {
                var by = d.Get(r, "decided by");
                if (Blank(by)) Err($"decision \"{Short(d.Get(r, "decision"))}\" has no decider");
                else if (IsConsultant(p, by) && !Regex.IsMatch(by, "recommend|with", I)) Err($"decision \"{Short(d.Get(r, "decision"))}\" decided by the consultant; recommend, and let a client person decide");
                if (d.Col("record") >= 0 && Blank(d.Get(r, "record"))) Warn($"decision \"{Short(d.Get(r, "decision"))}\" has no record (ADR, PR, changelog)");
            }
    }

    // ---------------------------------------------------------------- handover (21.4)

    static readonly (string What, string Pattern)[] Responsibilities =
    [
        ("the AI layer (rules, skills, changelog)", @"ai.layer|rules"),
        ("the eval regression gate", @"eval|gate|regression"),
        ("office hours or support", @"office hours|support|questions"),
        ("the metrics and usage review", @"metric|measure|usage"),
    ];

    static void Handover(Rec p)
    {
        var ho = Date(p.Fact("Handover date"));
        if (ho is null) Err("no 'Handover date:' (YYYY-MM-DD)");

        var o = p.Find("responsibility", "owner");
        if (o is null) Err("no owners table (Responsibility | Owner | Backup | Ran without consultant)");
        else
        {
            var names = o.Rows.Select(r => o.Get(r, "responsibility")).ToList();
            foreach (var (what, pat) in Responsibilities)
                if (!names.Any(n => Regex.IsMatch(n, pat, I))) Err($"no owner for {what}");
            foreach (var r in o.Rows)
            {
                var what = o.Get(r, "responsibility"); var owner = o.Get(r, "owner");
                if (Blank(owner)) Err($"{what}: no owner");
                else if (IsConsultant(p, owner)) Err($"{what}: owned by the consultant; a handover to yourself is not a handover");
                if (o.Col("backup") >= 0 && Blank(o.Get(r, "backup"))) Warn($"{what}: no backup owner (succession)");
                if (o.Col("ran") >= 0)
                {
                    var d = Date(o.Get(r, "ran"));
                    if (d is null) Err($"{what}: never run without the consultant; rehearse it before you leave");
                    else if (ho is not null && d > ho) Err($"{what}: first run without the consultant is after the handover ({d:yyyy-MM-dd})");
                }
            }
        }

        var res = p.Find("outcome", "result", "interval");
        if (res is null) Err("no results table (Outcome | Baseline | Result | Interval | Source | Status)");
        else foreach (var r in res.Rows)
            {
                var what = res.Get(r, "outcome"); var iv = res.Get(r, "interval"); var st = res.Get(r, "status");
                if (Blank(iv)) { Err($"{what}: no interval; a point estimate from one engagement is mostly noise"); continue; }
                var nums = Regex.Matches(iv, @"[-+−]?\d+(\.\d+)?").Select(m => double.Parse(m.Value.Replace("−", "-"), Inv)).ToList();
                if (nums.Count >= 2)
                {
                    bool crossesZero = nums[0] <= 0 && nums[1] >= 0;
                    if (crossesZero && Regex.IsMatch(st, @"improv|better|faster|reduc|worse|increas", I))
                        Err($"{what}: interval {iv} includes zero but status says \"{st}\"; call it inconclusive or no detectable change");
                    if (!crossesZero && Regex.IsMatch(st, @"inconclusive|no detectable", I))
                        Warn($"{what}: interval {iv} excludes zero but status says \"{st}\"");
                }
                if (Blank(st)) Err($"{what}: no status");
                if (res.Col("source") >= 0 && Blank(res.Get(r, "source"))) Warn($"{what}: no source (report, command, data file)");
            }

        var fu = p.Find("follow-up", "date");
        if (fu is null) Err("no follow-ups table (Follow-up | Date | Checks | Owner | Result)");
        else if (ho is not null)
        {
            var days = fu.Rows.Select(r => Date(fu.Get(r, "date"))).Where(d => d is not null).Select(d => d!.Value.DayNumber - ho.Value.DayNumber).ToList();
            foreach (var (target, tol) in new[] { (30, 7), (60, 7), (90, 10) })
                if (!days.Any(x => Math.Abs(x - target) <= tol)) Err($"no follow-up about {target} days after the handover");
        }

        if (!p.Lines.Any(l => Regex.IsMatch(l, @"does not show|did not show|not shown", I)))
            Err("no \"What this does not show\" section: say what the results cannot support before someone else says it for you");
        if (Date(p.Fact("Access revoked")) is null) Warn("no 'Access revoked:' date");
        if (Date(p.Fact("Data deleted")) is null) Warn("no 'Data deleted:' date (the SOW's data clause)");
    }

    // ---------------------------------------------------------------- case study (21.5)

    static void CaseStudy(string path, Rec rec, string? denyPath)
    {
        var cs = Rec.Load(path);
        var text = cs.Lines;

        // Terms that identify the client: the client's name, every stakeholder's name, plus the private deny list.
        var deny = new List<string>();
        foreach (var k in new[] { "Client", "Client name" }) if (!Blank(rec.Fact(k))) deny.Add(Regex.Replace(rec.Fact(k), @"\(.*?\)", "").Trim());
        var st = rec.Find("stakeholder", "role");
        if (st is not null)
            foreach (var r in st.Rows)
            {
                var n = Regex.Replace(st.Get(r, "stakeholder"), @"\(.*?\)", "").Trim();
                if (n.Length > 2) deny.Add(n);
                var parts = n.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 && parts[^1].Length >= 4) deny.Add(parts[^1]);
            }
        if (denyPath is not null) deny.AddRange(File.ReadAllLines(denyPath).Select(l => l.Trim()).Where(l => l.Length > 0 && !l.StartsWith("#")));
        deny = deny.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        Info($"{deny.Count} identifying terms checked ({(denyPath is null ? "from the record only; pass --deny for your private list" : "record + deny list")})");

        for (int i = 0; i < text.Length; i++)
        {
            var l = text[i]; int ln = i + 1;
            foreach (var d in deny)
                if (Regex.IsMatch(l, $@"\b{Regex.Escape(d)}\b", I)) Err($"line {ln}: names \"{d}\"");
            foreach (Match m in Regex.Matches(l, @"[\w.+-]+@[\w-]+(\.[\w-]+)+"))
                if (!m.Value.EndsWith(".example")) Err($"line {ln}: e-mail address {m.Value}");
            foreach (Match m in Regex.Matches(l, @"\b[A-Z][A-Z0-9]{1,9}-\d+\b"))
                if (!Regex.IsMatch(m.Value, @"^(EXP|ADR|INC|FAQ|ISO|CC|RFC)-")) Warn($"line {ln}: tracker key {m.Value} points at the client's project; remove it");
            foreach (Match m in Regex.Matches(l, @"(?<!\d\s?[-–]\s?)\b\d{2,}\s+(developers|engineers|employees|customers|people|staff)\b", I))
                Warn($"line {ln}: exact count \"{m.Value}\" narrows who it is; use a band (e.g. 15-25 developers)");
            var h = Hype.Match(l);
            if (h.Success) Err($"line {ln}: promise language \"{h.Value}\"");
            var up = Regex.Match(l, @"\bup to \d+", I);
            if (up.Success) Err($"line {ln}: \"{up.Value}\" presents the best case as the claim; report the estimate and its interval");
            if (Regex.IsMatch(l, @"results (are )?not typical|results may vary", I)) Warn($"line {ln}: a \"results may vary\" disclaimer does not fix an unrepresentative claim; state what is generally expected");
        }

        if (Date(cs.Fact("Client approval")) is null) Err("no 'Client approval:' date: publish only after the client approves the exact text in writing");

        // Every number must come from the engagement record.
        var recNums = new HashSet<string>();
        foreach (var t in rec.Tables.Where(t => t.Col("outcome") >= 0 || t.Col("metric") >= 0))
            foreach (var r in t.Rows)
                foreach (var c in r)
                    foreach (Match m in Regex.Matches(c, @"\d+(\.\d+)?")) recNums.Add(Norm(m.Value));
        foreach (Match m in Regex.Matches(rec.Fact("Detectable effect"), @"\d+(\.\d+)?")) recNums.Add(Norm(m.Value));
        var resultVals = new List<string>();
        var res = rec.Find("outcome", "result", "interval");
        if (res is not null) foreach (var r in res.Rows) foreach (Match m in Regex.Matches(res.Get(r, "result"), @"\d+(\.\d+)?\s?(%|pts?|points|h)")) resultVals.Add(m.Value);

        for (int i = 0; i < text.Length; i++)
        {
            var l = text[i];
            if (Regex.IsMatch(l, @"^\s*[-*]\s+\**(Client approval|Engagement|Version|Record)", I)) continue;
            foreach (Match m in Regex.Matches(l, @"(\d+(\.\d+)?)\s?(%|pts?\b|points\b|percentage points)(?!\s*(CI|confidence|interval|bootstrap))", I))
                if (!recNums.Contains(Norm(m.Groups[1].Value))) Err($"line {i + 1}: \"{m.Value.Trim()}\" does not appear in the engagement record's baseline or results");
            if (!l.TrimStart().StartsWith("|") && resultVals.Any(v => l.Contains(v)) && !Regex.IsMatch(l, @"\[|\binterval\b|\bCI\b|Wilson|compatible with", I))
                Warn($"line {i + 1}: an effect without its interval on the same line");
        }

        // Claims the record does not support.
        if (res is not null)
            foreach (var r in res.Rows)
            {
                if (!Regex.IsMatch(res.Get(r, "status"), @"inconclusive|no detectable", I)) continue;
                var outcome = res.Get(r, "outcome").ToLowerInvariant();
                var key = outcome.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
                if (key.Length < 4) continue;
                for (int i = 0; i < text.Length; i++)
                {
                    var l = text[i].ToLowerInvariant();
                    if (l.Contains(key) && Regex.IsMatch(l, @"faster|improv|reduc|\bcut\b|better|fewer|drop|fell|lower|to zero") && !Regex.IsMatch(l, @"inconclusive|no detectable|not (show|detect|conclude)|cannot|could not|can't|did not"))
                        Err($"line {i + 1}: claims a change in {outcome} that the record calls {res.Get(r, "status")}");
                }
            }

        if (!text.Any(l => Regex.IsMatch(l, @"^#+\s.*(limitation|does not show|what this cannot)", I)))
            Err("no limitations heading (\"What this does not show\")");
        if (!text.Any(l => Regex.IsMatch(l, @"^#+\s.*(what we did|intervention|approach)", I)))
            Warn("no \"What we did\" section: a result without the intervention cannot be repeated");
    }

    static string Norm(string s) => double.TryParse(s, NumberStyles.Float, Inv, out var d) ? Math.Abs(d).ToString("0.###", Inv) : s;

    sealed class UsageException(string m) : Exception(m);
}
