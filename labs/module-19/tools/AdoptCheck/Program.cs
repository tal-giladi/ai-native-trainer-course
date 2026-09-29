// AdoptCheck — the Module 19 tool. Dependency-free, read-only, deterministic.
//
//   stakeholders <adoption-plan.md>
//             Stakeholder map: required groups present (developers, managers, security, legal or privacy,
//             employee representation where it exists), every objection has an answer with a checkable
//             evidence reference, no promises nobody can keep, influential skeptics have an owner (19.1).
//   champions    <adoption-plan.md>
//             Champion network and tool strategy: one sanctioned champion per team who is not the
//             consultant, champion load, tool concentration (effective number of tools), exceptions
//             backed by a decision, portable rules files (19.2).
//   enablement   <adoption-plan.md>
//             Training paths per audience measured beyond attendance, the recurring rhythms (office hours,
//             feedback triage, AI-layer review, metrics review) with owners who stay, and the feedback
//             queue arithmetic (Little's law) (19.3).
//   plan         <adoption-plan.md>
//             All of the above plus adoption metrics (reach, habit, outcome; team-level aggregation) and
//             anti-regression mechanisms (ownership, succession, onboarding, alert, follow-ups) (19.4).
//   usage        <usage.csv> [--baseline W09-W12] [--events events.csv]
//             Weekly adoption by team and cohort: weekly active and engaged shares, a p-chart lower
//             control limit per team from the baseline weeks, first week below it, step vs decay, and the
//             events of those weeks (19.4).
//
// Exit code: 0 = no errors, 1 = errors found, 2 = usage or input problem.
using System.Globalization;
using System.Text.RegularExpressions;

static class Program
{
    static int errors, warnings;
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    static int Main(string[] args)
    {
        var ci = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        ci.NumberFormat.PercentPositivePattern = 1; ci.NumberFormat.PercentNegativePattern = 1;
        CultureInfo.CurrentCulture = ci;
        if (args.Length == 0 || args[0] is "-h" or "--help") { Usage(); return 0; }
        try
        {
            var rest = args.Skip(1).ToArray();
            if (rest.Length == 0) throw new UsageException("missing file argument (see --help)");
            switch (args[0])
            {
                case "stakeholders": { var p = Plan.Load(NeedFile(rest[0])); Stakeholders(p); break; }
                case "champions": { var p = Plan.Load(NeedFile(rest[0])); Champions(p); break; }
                case "enablement": { var p = Plan.Load(NeedFile(rest[0])); Enablement(p); break; }
                case "plan":
                    {
                        var p = Plan.Load(NeedFile(rest[0]));
                        Header("Stakeholders"); Stakeholders(p);
                        Header("Champions and tools"); Champions(p);
                        Header("Enablement"); Enablement(p);
                        Header("Metrics"); Metrics(p);
                        Header("Anti-regression"); AntiRegression(p);
                        break;
                    }
                case "usage": UsageCmd(rest); break;
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
        "AdoptCheck <command> <file>\n" +
        "  stakeholders <adoption-plan.md>   stakeholder map, objections and evidence (19.1)\n" +
        "  champions    <adoption-plan.md>   champion network and tool strategy (19.2)\n" +
        "  enablement   <adoption-plan.md>   training paths, rhythms, feedback queue (19.3)\n" +
        "  plan         <adoption-plan.md>   everything above + metrics + anti-regression (19.4)\n" +
        "  usage        <usage.csv> [--baseline W09-W12] [--events events.csv]   weekly adoption (19.4)");

    static void Header(string s) { Console.WriteLine(); Console.WriteLine($"== {s}"); }
    static void Err(string m) { errors++; Console.WriteLine($"ERROR {m}"); }
    static void Warn(string m) { warnings++; Console.WriteLine($"WARN  {m}"); }
    static void Info(string m) => Console.WriteLine($"      {m}");
    static void Ok(string m) => Console.WriteLine($"ok    {m}");

    static string NeedFile(string p) => File.Exists(p) ? p : throw new UsageException($"file not found: {p}");

    // ---------------------------------------------------------------- plan parsing

    sealed class Table
    {
        public List<string> Head = [];
        public List<List<string>> Rows = [];
        public int Col(string prefix) => Head.FindIndex(h => h.StartsWith(prefix));
        public string Get(List<string> r, string prefix) { var i = Col(prefix); return i >= 0 && i < r.Count ? r[i] : ""; }
    }

    sealed class Plan
    {
        public Dictionary<string, string> Facts = new(StringComparer.OrdinalIgnoreCase);
        public List<Table> Tables = [];

        public static Plan Load(string path)
        {
            var lines = File.ReadAllLines(path);
            var p = new Plan();
            foreach (var l in lines)
            {
                var m = Regex.Match(l, @"^\s*[-*]\s+\**([A-Za-z][A-Za-z /()]+?)\**:\s*(.+?)\s*$");
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

        public Table? Find(params string[] cols) =>
            Tables.FirstOrDefault(t => cols.All(c => t.Col(c) >= 0));

        public string Fact(string k) => Facts.TryGetValue(k, out var v) ? v : "";
    }

    static bool Blank(string s) => string.IsNullOrWhiteSpace(s) || s.Trim() is "-" or "—" or "?" || Regex.IsMatch(s, @"^\s*(tbd|tba|n/?a|none|todo)\b", RegexOptions.IgnoreCase);

    static string ConsultantName(Plan p) => Regex.Replace(p.Fact("Consultant"), @"\(.*?\)", "").Trim();

    static bool IsConsultant(Plan p, string cell)
    {
        if (Regex.IsMatch(cell, @"\bconsultant\b", RegexOptions.IgnoreCase)) return true;
        var c = ConsultantName(p);
        return c.Length > 0 && cell.Contains(c, StringComparison.OrdinalIgnoreCase);
    }

    static readonly Regex EvidenceRef = new(
        @"(\b\d{2}\.\d\b|ADR-\d+|EXP-\d+|INC-[\w-]+|FAQ-\d+|https?://|\.(md|json|csv|yml|yaml)\b|\bR\d{1,2}\b|§)",
        RegexOptions.IgnoreCase);

    static readonly Regex Hype = new(
        @"\b(guarantee[sd]?|10x|100\s?%|zero risk|no risk|risk-free|completely safe|never (fails?|leaks?|makes)|won'?t replace anyone|nobody will (lose|be replaced)|no one will lose|always (works|right|correct))\b",
        RegexOptions.IgnoreCase);

    // ---------------------------------------------------------------- stakeholders (19.1)

    static readonly (string Group, string Pattern, string Why)[] RequiredGroups =
    [
        ("developer", @"develop|engineer", "the people whose daily work changes"),
        ("manager", @"manag|lead|director|cto|vp", "they decide whether practice time and review time exist"),
        ("security", @"secur|ciso", "a late security veto stops a rollout after the licences are bought"),
        ("legal or privacy", @"legal|privacy|dpo|data.protection|counsel", "training terms, IP, personal data in prompts and telemetry"),
    ];

    static void Stakeholders(Plan p)
    {
        if (Blank(p.Fact("Sponsor"))) Err("no 'Sponsor:' line: name the executive who owns the outcome and can remove obstacles");
        var t = p.Find("stakeholder", "group", "objection", "answer", "evidence");
        if (t is null) { Err("no stakeholder table (columns Stakeholder | Group | Influence | Stance | Objection | Answer | Evidence | Changes for them | Owner)"); return; }
        var groups = t.Rows.Select(r => t.Get(r, "group").ToLowerInvariant()).ToList();
        foreach (var (g, pat, why) in RequiredGroups)
            if (!groups.Any(x => Regex.IsMatch(x, pat))) Err($"no {g} stakeholder: {why}");
        var countries = p.Fact("Countries").ToUpperInvariant();
        bool codetermination = Regex.IsMatch(countries, @"\b(DE|GERMANY|NL|NETHERLANDS|AT|AUSTRIA|FR|FRANCE)\b");
        if (codetermination && !groups.Any(x => Regex.IsMatch(x, @"works|council|employee rep|union|betriebsrat|ondernemingsraad")))
            Err($"countries {p.Fact("Countries")} but no works council / employee representation in the map: usage telemetry can be a system able to monitor performance (DE: BetrVG §87(1) no. 6)");

        int rowsOk = 0;
        foreach (var r in t.Rows)
        {
            var who = t.Get(r, "stakeholder");
            var obj = t.Get(r, "objection"); var ans = t.Get(r, "answer"); var ev = t.Get(r, "evidence");
            var stance = t.Get(r, "stance").ToLowerInvariant(); var infl = t.Get(r, "influence").ToLowerInvariant();
            int before = errors;
            if (!Blank(obj) && Blank(ans)) Err($"{who}: objection \"{Short(obj)}\" has no answer");
            if (!Blank(ans) && Blank(ev)) Err($"{who}: answer has no evidence reference (lesson, ADR, EXP, file or URL)");
            else if (!Blank(ev) && !EvidenceRef.IsMatch(ev)) Warn($"{who}: evidence \"{Short(ev)}\" is not a checkable reference");
            var hype = Hype.Match(ans);
            var mandate = Regex.Match(ans, @"\b(mandatory|mandated? by|performance reviews?|every developer'?s review|individual objectives?)\b", RegexOptions.IgnoreCase);
            if (mandate.Success) Err($"{who}: answer ties usage to individual performance (\"{mandate.Value}\"): that buys logins, not habit, and needs the works council");
            if (hype.Success) Err($"{who}: answer promises \"{hype.Value}\": a promise nobody can keep is the fastest way to lose this stakeholder");
            if (Regex.IsMatch(stance, "skeptic|blocker|opposed") && Regex.IsMatch(infl, "high") && Blank(t.Get(r, "owner")))
                Err($"{who}: high-influence {stance} with no owner: someone must meet them before the rollout, not after");
            if (t.Col("changes") >= 0 && Blank(t.Get(r, "changes"))) Warn($"{who}: 'changes for them' is empty: say what is different in their week");
            if (errors == before) rowsOk++;
        }
        Info($"{t.Rows.Count} stakeholders, {rowsOk} without errors");
    }

    static string Short(string s) => s.Length > 50 ? s[..47] + "..." : s;

    // ---------------------------------------------------------------- champions (19.2)

    static void Champions(Plan p)
    {
        var t = p.Find("team", "developers", "champion", "tool");
        if (t is null) { Err("no champions table (columns Team | Developers | Champion | Hours/week | Agreed with | Tool | Rules file)"); return; }
        var byChampion = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        var toolDevs = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        var rulesFormats = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var defaultTool = p.Fact("Default tool");
        if (Blank(defaultTool)) Err("no 'Default tool:' line: without one supported default, every team picks its own and enablement splits");
        double total = 0;
        foreach (var r in t.Rows)
        {
            var team = t.Get(r, "team");
            double.TryParse(t.Get(r, "developers"), NumberStyles.Float, Inv, out var devs);
            total += devs;
            var champ = t.Get(r, "champion");
            if (Blank(champ)) { Err($"{team}: no champion"); }
            else
            {
                if (IsConsultant(p, champ)) Err($"{team}: the champion is the consultant, who leaves; champions must be members of the team");
                var names = Regex.Split(Regex.Replace(champ, @"\(.*?\)", ""), @",|\+|&|/|\band\b").Select(n => n.Trim()).Where(n => n.Length > 0).ToList();
                foreach (var key in names)
                {
                    if (!byChampion.TryGetValue(key, out var l)) byChampion[key] = l = [];
                    l.Add(team);
                }
                if (names.Count > 0 && devs / names.Count > 25)
                    Warn($"{team}: {devs:0} developers for {names.Count} champion(s) (planning heuristic: about one per 25; add a co-champion)");
                var h = t.Get(r, "hours");
                if (!double.TryParse(Regex.Match(h, @"[\d.]+").Value, NumberStyles.Float, Inv, out var hours) || hours <= 0)
                    Err($"{team}: no champion hours per week: unsanctioned champion work is the first thing dropped in a busy sprint");
                if (t.Col("agreed") >= 0 && Blank(t.Get(r, "agreed")))
                    Err($"{team}: champion time not agreed with a manager");
            }
            var tool = t.Get(r, "tool");
            var toolName = Regex.Replace(tool, @"\(.*?\)", "").Trim();
            if (Blank(toolName)) { Err($"{team}: no tool"); continue; }
            toolDevs[toolName] = toolDevs.GetValueOrDefault(toolName) + devs;
            if (!Blank(defaultTool) && !toolName.Equals(defaultTool.Trim(), StringComparison.OrdinalIgnoreCase) && !Regex.IsMatch(tool, @"ADR-\d+", RegexOptions.IgnoreCase))
                Err($"{team}: uses {toolName}, not the default {defaultTool}, with no decision record (ADR) for the exception");
            var rules = t.Get(r, "rules");
            if (t.Col("rules") >= 0)
            {
                if (Blank(rules)) Warn($"{team}: no rules file: the agent starts every session without the team's conventions");
                else
                {
                    rulesFormats.Add(Regex.Replace(rules, @"\s*\(.*?\)|\s*\+.*", "").Trim());
                    if (!rules.Contains("AGENTS.md", StringComparison.OrdinalIgnoreCase))
                        Warn($"{team}: rules in {rules} only: not portable across tools (keep the core in AGENTS.md, 03.2)");
                }
            }
        }
        foreach (var (champ, teams) in byChampion.Where(kv => kv.Value.Count > 1))
            Err($"{champ} is champion for {teams.Count} teams ({string.Join(", ", teams)}): a champion is a peer inside one team");
        if (total > 0)
        {
            double hhi = toolDevs.Values.Sum(d => (d / total) * (d / total));
            Info($"tools by developers: {string.Join(", ", toolDevs.OrderByDescending(kv => kv.Value).Select(kv => $"{kv.Key} {kv.Value / total:P0}"))}");
            Info($"concentration H = sum of squared shares = {hhi:0.00}; effective number of tools 1/H = {1 / hhi:0.0}");
            if (1 / hhi > 2.0) Warn($"effective number of tools {1 / hhi:0.0} > 2: each extra tool multiplies security reviews, gateway work, rules formats and training paths");
        }
        if (rulesFormats.Count > 2) Warn($"{rulesFormats.Count} rules-file formats in use ({string.Join(", ", rulesFormats)})");
        Info($"{t.Rows.Count} teams, {total:0} developers, {byChampion.Count} distinct champions");
    }

    // ---------------------------------------------------------------- enablement (19.3)

    static readonly (string Name, string Pattern)[] Audiences =
    [
        ("developers", @"develop|engineer"),
        ("leads or managers", @"lead|manag"),
        ("security", @"secur"),
        ("new hires (onboarding)", @"new hire|joiner|onboard"),
    ];

    static readonly (string Name, string Pattern, string Why)[] Rhythms =
    [
        ("office hours", @"office hours|clinic", "the low-cost place to bring a stuck task"),
        ("feedback triage", @"feedback|triage|intake", "complaints that go nowhere teach people to stop reporting"),
        ("AI-layer review", @"ai.layer|governance|rules review|evolution", "the 11.5 loop: incidents become regression tasks and fixes"),
        ("metrics review", @"metric|adoption review|dashboard", "someone looks at the numbers on a date, not when it is too late"),
    ];

    static void Enablement(Plan p)
    {
        var tr = p.Find("audience", "format", "measured");
        if (tr is null) Err("no training table (columns Audience | Format | Minutes | When | Measured by | Owner)");
        else
        {
            var aud = tr.Rows.Select(r => tr.Get(r, "audience").ToLowerInvariant()).ToList();
            foreach (var (n, pat) in Audiences)
                if (!aud.Any(a => Regex.IsMatch(a, pat))) Err($"no training path for {n}");
            int deep = 0;
            foreach (var r in tr.Rows)
            {
                var m = tr.Get(r, "measured").ToLowerInvariant();
                var who = tr.Get(r, "audience");
                if (Blank(m)) Warn($"{who}: training not measured");
                else if (Regex.IsMatch(m, @"pre/?post|gain|exercise|follow-?up|observed|task|pr\b|behaviou?r|retention")) deep++;
                else Warn($"{who}: measured by \"{m}\" only: that is reaction (Kirkpatrick level 1), not learning or behaviour (16.5)");
                if (Regex.IsMatch(tr.Get(r, "format"), @"webinar|recording|video", RegexOptions.IgnoreCase) && !Regex.IsMatch(tr.Get(r, "format"), @"hands-on|exercise|lab", RegexOptions.IgnoreCase))
                    Warn($"{who}: {tr.Get(r, "format")} with no hands-on part (16.3: people learn what they do)");
                if (IsConsultant(p, tr.Get(r, "owner"))) Warn($"{who}: training owned by the consultant: fine for delivery, name who runs it after the handover");
            }
            if (deep == 0) Err("no training path is measured beyond attendance or satisfaction");
        }

        var rh = p.Find("mechanism", "cadence", "owner");
        var anti = p.Find("mechanism", "trigger");
        if (rh is null || rh == anti) Err("no rhythms table (columns Mechanism | Cadence | Owner | Input | Output)");
        else
        {
            foreach (var (n, pat, why) in Rhythms)
            {
                var row = rh.Rows.FirstOrDefault(r => Regex.IsMatch(rh.Get(r, "mechanism"), pat, RegexOptions.IgnoreCase));
                if (row is null) { Err($"no {n}: {why}"); continue; }
                var owner = rh.Get(row, "owner");
                if (Blank(owner)) Err($"{n}: no owner");
                else if (IsConsultant(p, owner)) Err($"{n}: owned by the consultant, who leaves: it stops the week after the handover");
                if (Blank(rh.Get(row, "cadence"))) Warn($"{n}: no cadence");
                else if (Regex.IsMatch(rh.Get(row, "cadence"), @"ad.?hoc|as needed|when|if", RegexOptions.IgnoreCase))
                    Warn($"{n}: cadence \"{rh.Get(row, "cadence")}\" is not a date anyone can miss");
            }
            var channel = rh.Rows.Where(r => Regex.IsMatch(rh.Get(r, "mechanism"), "channel|slack|teams", RegexOptions.IgnoreCase)).ToList();
            if (channel.Count > 0 && rh.Rows.Count == channel.Count) Err("the only rhythm is a chat channel: a channel is a place, not a mechanism with an owner and a date");
        }

        // Feedback queue: arrivals vs resolution capacity, Little's law L = lambda * W
        var a = Num(p.Fact("Feedback arrivals per week"));
        var c = Num(p.Fact("Feedback resolved per week"));
        var w = Num(p.Fact("Feedback target weeks"));
        if (a is null || c is null) Warn("no 'Feedback arrivals per week:' / 'Feedback resolved per week:' lines: the feedback loop has no capacity plan");
        else
        {
            Info($"feedback: arrivals {a:0.#}/week, resolved {c:0.#}/week, utilisation {a / c:P0}");
            if (c < a) Err($"feedback backlog grows by {a - c:0.#} items a week ({(a - c) * 12:0} after a quarter): people stop reporting when nothing comes back");
            else if (a / c > 0.9) Warn($"utilisation {a / c:P0}: any bad week creates a backlog that does not drain");
            if (w is not null)
                Info($"Little's law: target time in system {w:0.#} weeks x {a:0.#} arrivals/week = {a * w:0.#} open items on average (L = lambda W)");
        }
    }

    static double? Num(string s) => double.TryParse(Regex.Match(s ?? "", @"[\d.]+").Value, NumberStyles.Float, Inv, out var v) ? v : null;

    // ---------------------------------------------------------------- metrics and anti-regression (19.4)

    static void Metrics(Plan p)
    {
        var t = p.Find("metric", "level", "definition");
        if (t is null) { Err("no metrics table (columns Metric | Level | Definition | Target | Source | Aggregation)"); return; }
        var levels = t.Rows.Select(r => t.Get(r, "level").ToLowerInvariant()).ToList();
        foreach (var lv in new[] { "reach", "habit", "outcome" })
            if (!levels.Any(l => l.Contains(lv))) Err($"no {lv} metric (reach = who has it, habit = who keeps using it, outcome = what changed in delivery, 13.1)");
        foreach (var r in t.Rows)
        {
            var m = t.Get(r, "metric"); var def = t.Get(r, "definition"); var target = t.Get(r, "target");
            var agg = t.Get(r, "aggregation").ToLowerInvariant();
            if (Regex.IsMatch(m, @"seats?|licen[cs]e|prompts?|tokens?|lines of code|loc\b|logins?|acceptance rate", RegexOptions.IgnoreCase) && !Blank(target) && !t.Get(r, "level").ToLowerInvariant().Contains("reach"))
                Warn($"{m}: a volume or activity count with a target invites gaming (Goodhart); use it as a diagnostic, not a goal");
            if (Regex.IsMatch(agg, @"individual|per (developer|person|user)|named|leaderboard"))
                Err($"{m}: aggregated per individual: that turns adoption telemetry into performance monitoring (works council, privacy); report teams of 5 or more");
            if (Blank(target)) Warn($"{m}: no target or threshold");
            if (Blank(t.Get(r, "source"))) Warn($"{m}: no data source");
        }
        Info($"{t.Rows.Count} metrics");
    }

    static readonly (string Name, string Pattern, string Why)[] AntiReg =
    [
        ("ownership transfer", @"owner|handover|hand-over", "a named internal owner for the programme after the consultant leaves"),
        ("champion succession", @"succession|successor|rotation|backup", "champions change teams; the network must survive it"),
        ("onboarding", @"onboard|joiner|new hire", "every new hire otherwise starts at zero"),
        ("usage alert", @"alert|control limit|lcl|threshold", "a drop is noticed in a week, not in a quarter"),
        ("follow-ups", @"follow-?up|30.?60.?90|check-?in", "dated checks after the engagement"),
    ];

    static void AntiRegression(Plan p)
    {
        if (Blank(p.Fact("Handover date"))) Err("no 'Handover date:' line");
        var t = p.Find("mechanism", "trigger", "action");
        if (t is null) { Err("no anti-regression table (columns Mechanism | Owner | Trigger | Action)"); return; }
        foreach (var (n, pat, why) in AntiReg)
        {
            var row = t.Rows.FirstOrDefault(r => Regex.IsMatch(t.Get(r, "mechanism") + " " + t.Get(r, "trigger"), pat, RegexOptions.IgnoreCase));
            if (row is null) { Err($"no {n}: {why}"); continue; }
            var owner = t.Get(row, "owner");
            if (Blank(owner)) Err($"{n}: no owner");
            else if (IsConsultant(p, owner) && !Regex.IsMatch(n, "follow")) Err($"{n}: owned by the consultant, who leaves");
            if (Blank(t.Get(row, "action"))) Warn($"{n}: trigger without an action");
        }
        Info($"{t.Rows.Count} mechanisms");
    }

    // ---------------------------------------------------------------- usage (19.4)

    sealed record U(int Week, string Team, string Cohort, int Seats, int Active, int Engaged);

    static void UsageCmd(string[] rest)
    {
        var path = NeedFile(rest[0]);
        int b0 = 9, b1 = 12; string? eventsPath = null;
        for (int i = 1; i < rest.Length; i++)
        {
            if (rest[i] == "--baseline" && i + 1 < rest.Length)
            {
                var m = Regex.Match(rest[++i], @"^W?(\d+)-W?(\d+)$");
                if (!m.Success) throw new UsageException("--baseline expects e.g. W09-W12");
                b0 = int.Parse(m.Groups[1].Value, Inv); b1 = int.Parse(m.Groups[2].Value, Inv);
            }
            else if (rest[i] == "--events" && i + 1 < rest.Length) eventsPath = NeedFile(rest[++i]);
            else throw new UsageException($"unknown option {rest[i]}");
        }
        var rows = new List<U>();
        var lines = File.ReadAllLines(path);
        if (lines.Length < 2 || !lines[0].StartsWith("week,team,cohort,seats,active,engaged"))
            throw new UsageException("usage.csv must start with: week,team,cohort,seats,active,engaged");
        foreach (var l in lines.Skip(1).Where(l => l.Trim().Length > 0))
        {
            var c = l.Split(',');
            rows.Add(new U(int.Parse(c[0].TrimStart('W'), Inv), c[1], c[2], int.Parse(c[3], Inv), int.Parse(c[4], Inv), int.Parse(c[5], Inv)));
        }
        var events = new Dictionary<int, string>();
        if (eventsPath is not null)
            foreach (var l in File.ReadAllLines(eventsPath).Skip(1).Where(l => l.Contains(',')))
            {
                var i = l.IndexOf(',');
                events[int.Parse(l[..i].TrimStart('W'), Inv)] = l[(i + 1)..].Trim();
            }
        int last = rows.Max(r => r.Week);

        foreach (var small in rows.Where(r => r.Seats > 0 && r.Seats < 5).Select(r => r.Team).Distinct())
            Warn($"{small}: some weeks have fewer than 5 seats; those shares are suppressed below (small groups identify people)");

        // Organisation series
        Console.WriteLine("Organisation, weekly active share (engaged = sessions on 3+ days that week)");
        var weeks = rows.GroupBy(r => r.Week).OrderBy(g => g.Key).ToList();
        foreach (var g in weeks)
        {
            int n = g.Sum(r => r.Seats), a = g.Sum(r => r.Active), e = g.Sum(r => r.Engaged);
            var ev = events.TryGetValue(g.Key, out var s) ? "  <- " + s : "";
            Console.WriteLine($"  W{g.Key:00}  {a,4}/{n,-4} {(double)a / n,5:P0} active  {(double)e / n,5:P0} engaged  {Bar((double)a / n)}{ev}");
        }
        var baseRows = rows.Where(r => r.Week >= b0 && r.Week <= b1 && r.Cohort != "newhire").ToList();
        double pOrg = (double)baseRows.Sum(r => r.Active) / Math.Max(1, baseRows.Sum(r => r.Seats));
        var lastOrg = weeks.Last();
        int nLast = lastOrg.Sum(r => r.Seats), aLast = lastOrg.Sum(r => r.Active);
        double lclOrg = pOrg - 3 * Math.Sqrt(pOrg * (1 - pOrg) / nLast);
        Info($"baseline W{b0:00}-W{b1:00}: p = {pOrg:P1}; W{last:00}: {(double)aLast / nLast:P1} of {nLast}; lower control limit p - 3*sqrt(p(1-p)/n) = {lclOrg:P1}");
        if ((double)aLast / nLast < lclOrg) Err($"organisation W{last:00} is below its lower control limit: a real drop, not noise");

        // Per team p-chart
        Console.WriteLine();
        Console.WriteLine($"By team (baseline W{b0:00}-W{b1:00}; LCL computed with each week's seats)");
        Console.WriteLine($"  {"team",-10} {"base",5} {"LCL",5} {"last",5} {"first<LCL",9}  pattern");
        var teamErrors = new List<string>(); var teamInfo = new List<string>();
        foreach (var tg in rows.Where(r => r.Cohort != "newhire").GroupBy(r => r.Team))
        {
            var br = tg.Where(r => r.Week >= b0 && r.Week <= b1).ToList();
            if (br.Count == 0 || br.Sum(r => r.Seats) == 0) { Warn($"{tg.Key}: no baseline weeks"); continue; }
            double p = (double)br.Sum(r => r.Active) / br.Sum(r => r.Seats);
            var after = tg.Where(r => r.Week > b1).OrderBy(r => r.Week).ToList();
            int? first = null; int below = 0; string pattern = "stable";
            U? prev = br.OrderBy(r => r.Week).Last();
            foreach (var r in after)
            {
                double lcl = p - 3 * Math.Sqrt(p * (1 - p) / r.Seats);
                double share = (double)r.Active / r.Seats;
                if (share < lcl) { first ??= r.Week; below++; }
                if (prev is not null && first == r.Week)
                {
                    double prevShare = (double)prev.Active / prev.Seats;
                    pattern = prevShare - share > p / 2 ? "STEP (something changed that week: tool, access, quota, measurement)" : "decay (habit and support fading)";
                }
                prev = r;
            }
            var lr = tg.OrderBy(r => r.Week).Last();
            double lastShare = (double)lr.Active / lr.Seats;
            double lclLast = p - 3 * Math.Sqrt(p * (1 - p) / lr.Seats);
            bool recovered = first is not null && lastShare >= lclLast;
            if (first is not null && recovered) pattern += $"; recovered by W{last:00}";
            Console.WriteLine($"  {tg.Key,-10} {p,5:P0} {lclLast,5:P0} {lastShare,5:P0} {(first is null ? "-" : $"W{first:00}"),9}  {pattern}");
            if (first is not null && !recovered)
            {
                teamErrors.Add($"{tg.Key}: below its lower control limit since W{first:00} ({below} weeks){(events.TryGetValue(first.Value, out var e) ? $"; events that week: {e}" : "")}");
                if (lr.Seats > 0 && lastShare < 0.1 && pattern.StartsWith("STEP")) teamInfo.Add($"{tg.Key}: near zero after a step: check whether usage moved somewhere the telemetry cannot see before calling it abandonment");
            }
        }

        foreach (var e in teamErrors) Err(e);
        foreach (var i in teamInfo) Info(i);

        // Cohorts
        var nh = rows.Where(r => r.Cohort == "newhire").GroupBy(r => r.Week).OrderBy(g => g.Key).ToList();
        if (nh.Count > 0)
        {
            Console.WriteLine();
            var lastNh = nh.Last();
            int s = lastNh.Sum(r => r.Seats), a = lastNh.Sum(r => r.Active);
            if (s < 5) Info($"new hires W{last:00}: fewer than 5, suppressed");
            else
            {
                double share = (double)a / s;
                Console.WriteLine($"New-hire cohort W{lastNh.Key:00}: {a}/{s} active ({share:P0}) vs rollout baseline {pOrg:P0}");
                if (share < pOrg / 2) Err($"new hires at {share:P0}, under half the rollout baseline: onboarding does not include the agent");
            }
        }

        // Stickiness
        var lastAll = weeks.Last();
        int la = lastAll.Sum(r => r.Active), le = lastAll.Sum(r => r.Engaged);
        if (la > 0)
        {
            var bA = baseRows.Sum(r => r.Active); var bE = baseRows.Sum(r => r.Engaged);
            Info($"engaged / active: baseline {(double)bE / Math.Max(1, bA):P0}, W{last:00} {(double)le / la:P0} (share of users for whom it is a habit)");
        }
    }

    static string Bar(double share) => new string('#', (int)Math.Round(share * 30));

    sealed class UsageException(string m) : Exception(m);
}
