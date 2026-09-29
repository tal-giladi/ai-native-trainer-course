// KitCheck — the Module 17 tool. Dependency-free, read-only, deterministic.
//
//   agenda    <agenda.md>
//             A timestamped workshop agenda: timestamps add up, the arc is complete and in order,
//             every live segment has a fallback and a wait-time plan, every hands-on step has a
//             catch-up tag, buffer, break and close are sized (17.1). The pedagogy of the same file
//             (objectives, items, active share, passive stretches) is LearnCheck align's job (16.2, 16.3).
//   kit       <workshop-kit dir>
//             Everything the agenda needs exists: instructor notes per step, starter pack with a setup
//             check and the catch-up tags, exercise specs, troubleshooting, fallback list, forms,
//             question bank, rehearsal logs (17.2).
//   questions <question-bank.md>
//             Hard-questions bank: categories covered, evidence per answer, no unsourced numbers,
//             no hype words, answers short enough to say in about a minute (17.3).
//   rehearsal <log.md> [<log.md> ...]
//             Rehearsal logs in order: timing drift, notes checks, incidents, observer scores and
//             agreement, and the exit standard on the last one (17.4).
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
        if (args.Length == 0 || args[0] is "-h" or "--help") { Usage(); return 0; }
        try
        {
            var rest = args.Skip(1).ToArray();
            if (rest.Length == 0) throw new UsageException("missing file or folder argument (see --help)");
            switch (args[0])
            {
                case "agenda": Agenda(NeedFile(rest[0])); break;
                case "kit": Kit(NeedDir(rest[0])); break;
                case "questions": Questions(NeedFile(rest[0])); break;
                case "rehearsal": Rehearsals(rest.Select(NeedFile).ToList()); break;
                default: Console.Error.WriteLine($"unknown command '{args[0]}'"); Usage(); return 2;
            }
        }
        catch (UsageException e) { Console.Error.WriteLine(e.Message); return 2; }
        catch (IOException e) { Console.Error.WriteLine(e.Message); return 2; }
        Console.WriteLine();
        Console.WriteLine($"{errors} error(s), {warnings} warning(s)");
        return errors > 0 ? 1 : 0;
    }

    // ---------------------------------------------------------------- agenda

    static readonly string[] Arc = ["credibility", "problem", "demo", "exercise", "qa", "close"];
    static readonly string[] Deliveries = ["talk", "live", "recorded", "hands-on", "paper", "-"];

    sealed record Row(string Start, int StartMin, string Step, double Minutes, string Mode, string Delivery, string Arc, string Fallback);
    sealed record AgendaFile(double? Slot, List<Row> Rows);

    static AgendaFile ParseAgenda(string path)
    {
        var lines = File.ReadAllLines(path);
        double? slot = null;
        var s = Regex.Match(string.Join("\n", lines), @"^[-*\s]*(Slot|Budget):\s*(\d+)", RegexOptions.Multiline);
        if (s.Success) slot = double.Parse(s.Groups[2].Value, Inv);
        foreach (var (head, rows) in Tables(lines))
        {
            int C(string n) => head.FindIndex(h => h.StartsWith(n));
            if (C("start") < 0 || C("minutes") < 0 || C("delivery") < 0) continue;
            var list = new List<Row>();
            foreach (var r in rows)
            {
                string G(string n) => C(n) >= 0 && C(n) < r.Count ? r[C(n)] : "";
                double.TryParse(G("minutes"), NumberStyles.Float, Inv, out var min);
                list.Add(new Row(G("start"), Clock(G("start")), G("step"), min, G("mode").ToLowerInvariant(),
                    G("delivery").ToLowerInvariant(), G("arc").ToLowerInvariant(), G("fallback")));
            }
            return new AgendaFile(slot, list);
        }
        return new AgendaFile(slot, []);
    }

    static int Clock(string t)
    {
        var m = Regex.Match(t.Trim(), @"^(\d{1,2}):(\d{2})$");
        return m.Success ? int.Parse(m.Groups[1].Value, Inv) * 60 + int.Parse(m.Groups[2].Value, Inv) : -1;
    }

    static string Hm(double minutes) => $"{(int)minutes / 60}:{(int)minutes % 60:00}";

    static void Agenda(string path)
    {
        var a = ParseAgenda(path);
        var rows = a.Rows;
        Console.WriteLine($"{Path.GetFileName(path)}: {rows.Count} steps{(a.Slot is { } sl ? $", slot {sl:0} min" : "")}");
        if (rows.Count == 0) { Error("no agenda table (| Start | Step | Minutes | Mode | Delivery | Arc | ... | Fallback |)"); return; }
        if (a.Slot is null) Warn("no 'Slot: <minutes>' line; cannot check the total against the booking");

        // Timestamps
        Console.WriteLine();
        Console.WriteLine("Clock");
        double expected = 0; int bad = 0;
        foreach (var r in rows)
        {
            if (r.StartMin < 0) { Error($"'{Clip(r.Step, 40)}': start '{r.Start}' is not h:mm"); bad++; }
            else if (Math.Abs(r.StartMin - expected) > 0.01)
            { Error($"'{Clip(r.Step, 40)}' starts at {r.Start}, but the previous step ends at {Hm(expected)}"); bad++; }
            if (r.StartMin >= 0) expected = r.StartMin + r.Minutes; else expected += r.Minutes;
            if (!Deliveries.Contains(r.Delivery)) Warn($"'{Clip(r.Step, 40)}': delivery '{r.Delivery}' is not one of {string.Join(", ", Deliveries)}");
        }
        double total = rows.Sum(r => r.Minutes);
        Console.WriteLine($"  {rows.Count} steps, {total:0} minutes, ends at {Hm(expected)}; {(bad == 0 ? "timestamps add up" : $"{bad} timestamp(s) wrong")}");
        if (a.Slot is { } slot)
        {
            if (total > slot) Error($"the agenda takes {total:0} minutes; the slot is {slot:0}");
            else if (expected > slot) Error($"the clock ends at {Hm(expected)}, after the {slot:0}-minute slot, although the steps add up to {total:0} minutes");
            else if (slot - total > 5) Warn($"{slot - total:0} minutes of the slot are unplanned; name them (buffer, Q&A)");
        }

        // Arc
        Console.WriteLine();
        Console.WriteLine("Arc");
        double slotMin = a.Slot ?? total;
        double ArcMin(string x) => rows.Where(r => r.Arc == x).Sum(r => r.Minutes);
        foreach (var x in Arc.Concat(["assess", "reflect", "break", "buffer"]))
            Console.WriteLine($"  {x,-12} {ArcMin(x),4:0} min  {Pct(ArcMin(x), slotMin),4}");
        int last = -1; string lastName = "";
        foreach (var x in Arc)
        {
            int i = rows.FindIndex(r => r.Arc == x);
            if (i < 0) { Error($"no '{x}' step: the arc is credibility -> problem -> demo -> exercise -> qa -> close"); continue; }
            if (i < last) Warn($"first '{x}' step comes before the first '{lastName}' step");
            last = i; lastName = x;
        }
        double handsOn = rows.Where(r => r.Delivery == "hands-on").Sum(r => r.Minutes);
        double demo = ArcMin("demo");
        if (handsOn < 0.25 * slotMin)
            Error($"hands-on is {handsOn:0} of {slotMin:0} minutes ({Pct(handsOn, slotMin)}); below a quarter, learners watch a workshop instead of doing one");
        if (demo > 0.4 * slotMin)
            Error($"demo is {demo:0} of {slotMin:0} minutes ({Pct(demo, slotMin)}); cut it into live segments of at most 10 minutes, each followed by hands-on");
        if (ArcMin("close") > 0.1 * slotMin)
            Warn($"close is {ArcMin("close"):0} minutes; more than a tenth of the slot is a pitch, not a close");
        if (ArcMin("buffer") == 0) Error("no buffer: the first failure you did not plan for comes out of the exercises or the post-test");
        else if (ArcMin("buffer") < 0.05 * slotMin) Warn($"buffer {ArcMin("buffer"):0} minutes is under 5% of the slot");
        if (slotMin >= 90 && ArcMin("break") == 0) Warn($"no break in {slotMin:0} minutes");
        if (!rows.Any(r => r.Mode == "assess")) Warn("no assessment step: there will be no evidence of learning (16.5)");
        var firstHands = rows.FirstOrDefault(r => r.Delivery == "hands-on");
        if (firstHands is not null && firstHands.StartMin > 45)
            Warn($"first hands-on step starts at {firstHands.Start}; setup problems surface after the room has been passive for 45+ minutes");

        // Live segments and hands-on
        Console.WriteLine();
        Console.WriteLine("Live and hands-on");
        foreach (var r in rows.Where(r => r.Delivery is "live" or "hands-on"))
        {
            var fb = r.Fallback.Trim();
            bool hasFb = Regex.IsMatch(fb, @"(rec|branch|tag):\S+");
            Console.WriteLine($"  {r.Start,5} {r.Delivery,-9} {r.Minutes,3:0} min  {Clip(r.Step, 46),-46} {(fb.Length == 0 || fb == "-" ? "(none)" : fb)}");
            if (r.Delivery == "live")
            {
                if (!hasFb) Error($"live '{Clip(r.Step, 40)}' has no fallback (rec:<file>@mm:ss, branch:<name> or tag:<name>)");
                if (r.Minutes > 10) Warn($"live '{Clip(r.Step, 40)}' runs {r.Minutes:0} minutes; the room watches one agent run longer than 10 minutes");
                if (!Regex.IsMatch(r.Step, @"predict|q&a|question", RegexOptions.IgnoreCase))
                    Warn($"live '{Clip(r.Step, 40)}' has no wait-time plan (a prediction question or Q&A while the agent runs)");
            }
            else if (!Regex.IsMatch(fb, @"tag:\S+"))
                Error($"hands-on '{Clip(r.Step, 40)}' has no catch-up tag: a learner who falls behind cannot rejoin the next step");
        }
        Console.WriteLine();
        Console.WriteLine("Pedagogy (objectives, items, active share, passive stretches): run LearnCheck align on the same file.");
    }

    // ---------------------------------------------------------------- kit

    static void Kit(string dir)
    {
        Console.WriteLine($"{Path.GetFileName(Path.GetFullPath(dir).TrimEnd(Path.DirectorySeparatorChar))}/");
        string P(string rel) => Path.Combine(dir, rel);
        var required = new[] { "agenda.md", "instructor-notes.md", "starter-pack/README.md", "troubleshooting.md", "fallback/README.md", "forms.md", "question-bank.md" };
        foreach (var f in required)
        {
            bool ok = File.Exists(P(f));
            Console.WriteLine($"  {(ok ? "ok     " : "MISSING")} {f}");
            if (!ok) Error($"{f} is missing");
        }
        if (!File.Exists(P("agenda.md"))) return;
        var agenda = ParseAgenda(P("agenda.md"));

        // Instructor notes: one section per agenda step, short enough to glance at
        if (File.Exists(P("instructor-notes.md")))
        {
            Console.WriteLine();
            Console.WriteLine("Instructor notes");
            var text = File.ReadAllText(P("instructor-notes.md"));
            var sections = Regex.Split(text, @"^(?=## )", RegexOptions.Multiline).Where(x => x.StartsWith("## ")).ToList();
            int found = 0;
            foreach (var r in agenda.Rows)
            {
                var sec = sections.FirstOrDefault(x => Regex.IsMatch(x, $@"^## {Regex.Escape(r.Start)}\b"));
                if (sec is null) { Error($"no notes section '## {r.Start} ...' for '{Clip(r.Step, 40)}'"); continue; }
                found++;
                int words = Regex.Matches(sec, @"\w+").Count;
                if (words > 150) Warn($"notes for {r.Start} are {words} words; you cannot glance at more than about 150 in front of a room");
                if ((r.Delivery == "live" || r.Delivery == "hands-on") && !Regex.IsMatch(sec, @"if it breaks|if behind|fallback|catch-up", RegexOptions.IgnoreCase))
                    Warn($"notes for {r.Start} ({r.Delivery}) do not say what to do if it breaks or the room falls behind");
            }
            Console.WriteLine($"  {found} of {agenda.Rows.Count} agenda steps have a notes section");
        }

        // Starter pack
        Console.WriteLine();
        Console.WriteLine("Starter pack");
        var spReadme = P("starter-pack/README.md");
        bool check = File.Exists(P("starter-pack/check-setup.ps1")) || File.Exists(P("starter-pack/check-setup.sh"));
        Console.WriteLine($"  setup check script: {(check ? "yes" : "no")}");
        if (!check) Error("starter-pack has no check-setup.ps1 or check-setup.sh: setup problems will surface in the first exercise");
        if (File.Exists(spReadme))
        {
            var sp = File.ReadAllText(spReadme);
            if (!Regex.IsMatch(sp, @"offline|local feed|packages folder|no network", RegexOptions.IgnoreCase))
                Warn("starter-pack README has no offline path; one blocked proxy or package feed stops the room");
            if (!Regex.IsMatch(sp, @"no agent|paper path|without an agent|pair", RegexOptions.IgnoreCase))
                Warn("starter-pack README says nothing about learners without agent access (policy, licence, laptop)");
            foreach (var tag in agenda.Rows.SelectMany(r => Regex.Matches(r.Fallback, @"tag:([\w./-]+)").Select(m => m.Groups[1].Value)).Distinct())
                if (!sp.Contains(tag)) Error($"catch-up tag '{tag}' is in the agenda but not in starter-pack/README.md");
        }

        // Exercises
        Console.WriteLine();
        Console.WriteLine("Exercises");
        var exDir = P("exercises");
        var exFiles = Directory.Exists(exDir) ? Directory.GetFiles(exDir, "*.md").OrderBy(f => f, StringComparer.Ordinal).ToList() : [];
        int handsOnSteps = agenda.Rows.Count(r => r.Delivery == "hands-on");
        Console.WriteLine($"  {exFiles.Count} exercise file(s) for {handsOnSteps} hands-on step(s)");
        if (exFiles.Count < handsOnSteps) Error($"{handsOnSteps} hands-on steps but {exFiles.Count} exercise files in exercises/");
        var fields = new[] { "Start", "Task", "Done when", "Time", "Hints", "Catch-up" };
        foreach (var f in exFiles)
        {
            var t = File.ReadAllText(f);
            var missing = fields.Where(x => !Regex.IsMatch(t, $@"^\s*[-*]?\s*\**{Regex.Escape(x)}\**\s*:", RegexOptions.Multiline | RegexOptions.IgnoreCase)).ToList();
            Console.WriteLine($"  {Path.GetFileName(f),-28} {(missing.Count == 0 ? "complete" : "missing: " + string.Join(", ", missing))}");
            if (missing.Count > 0) Error($"{Path.GetFileName(f)}: missing {string.Join(", ", missing)}");
        }

        // Troubleshooting
        if (File.Exists(P("troubleshooting.md")))
        {
            Console.WriteLine();
            Console.WriteLine("Troubleshooting");
            var tbl = Tables(File.ReadAllLines(P("troubleshooting.md"))).FirstOrDefault(t => t.head.Any(h => h.StartsWith("symptom")));
            if (tbl.head is null) Error("troubleshooting.md has no | ID | Symptom | Cause | Fix | Seconds | Tested | table");
            else
            {
                int C(string n) => tbl.head.FindIndex(h => h.StartsWith(n));
                int untested = 0, slow = 0;
                foreach (var r in tbl.rows)
                {
                    string G(string n) => C(n) >= 0 && C(n) < r.Count ? r[C(n)] : "";
                    var id = G("id").Length > 0 ? G("id") : Clip(G("symptom"), 30);
                    if (G("fix").Length == 0) Error($"{id}: no fix");
                    var tested = G("tested");
                    if (tested.Length == 0 || tested == "-" || tested.Equals("no", StringComparison.OrdinalIgnoreCase)) { untested++; Warn($"{id}: fix never tested; a fix you have not run is a guess"); }
                    if (double.TryParse(G("seconds"), NumberStyles.Float, Inv, out var sec) && sec > 180) { slow++; Warn($"{id}: fix takes {sec:0} s; longer than an exercise can absorb, send them to the catch-up tag or the paper path"); }
                }
                Console.WriteLine($"  {tbl.rows.Count} entries, {untested} untested, {slow} slower than 3 minutes");
                if (tbl.rows.Count < 6) Warn($"{tbl.rows.Count} entries; three rehearsals usually produce at least six");
            }
        }

        // Fallback recordings
        if (File.Exists(P("fallback/README.md")))
        {
            Console.WriteLine();
            Console.WriteLine("Fallback recordings");
            var fbText = File.ReadAllText(P("fallback/README.md"));
            var tbl = Tables(File.ReadAllLines(P("fallback/README.md"))).FirstOrDefault(t => t.head.Any(h => h.StartsWith("file")));
            var recs = agenda.Rows.SelectMany(r => Regex.Matches(r.Fallback, @"rec:([^\s@]+)").Select(m => Path.GetFileName(m.Groups[1].Value))).Distinct().ToList();
            foreach (var rec in recs)
            {
                bool listed = fbText.Contains(rec);
                Console.WriteLine($"  {(listed ? "listed " : "MISSING")} {rec}");
                if (!listed) Error($"agenda falls back to '{rec}', which fallback/README.md does not list");
            }
            if (tbl.head is not null)
            {
                int ci = tbl.head.FindIndex(h => h.StartsWith("checked"));
                foreach (var r in tbl.rows.Where(r => ci < 0 || ci >= r.Count || r[ci].Length == 0 || r[ci] == "-"))
                    Warn($"fallback '{r.ElementAtOrDefault(tbl.head.FindIndex(h => h.StartsWith("file")))}' never played back end to end");
            }
        }

        // Question bank and rehearsals, summarized
        if (File.Exists(P("question-bank.md")))
        {
            Console.WriteLine();
            Console.WriteLine("Question bank (details: KitCheck questions)");
            int e0 = errors, w0 = warnings;
            var saved = Console.Out; Console.SetOut(TextWriter.Null);
            try { Questions(P("question-bank.md")); } finally { Console.SetOut(saved); }
            Console.WriteLine($"  {errors - e0} error(s), {warnings - w0} warning(s)");
        }
        var reh = Directory.Exists(P("rehearsals")) ? Directory.GetFiles(P("rehearsals"), "*.md").Where(f => !Path.GetFileName(f).Equals("README.md", StringComparison.OrdinalIgnoreCase)).OrderBy(f => f, StringComparer.Ordinal).ToList() : [];
        Console.WriteLine();
        Console.WriteLine($"Rehearsals: {reh.Count} log(s) (details and exit standard: KitCheck rehearsal)");
        if (reh.Count < 3) Warn($"{reh.Count} rehearsal log(s); the standard is three before a paid room");
    }

    // ---------------------------------------------------------------- questions

    static readonly string[] Categories = ["replace", "security", "roi", "skeptic", "confidentiality"];
    static readonly Regex Hype = new(@"\b(guarantee[ds]?|10x|100x|zero risk|no risk|completely safe|proven to|always works|never fails)\b", RegexOptions.IgnoreCase);
    static readonly Regex Absolute = new(@"\b(always|never|everyone|nobody|definitely|certainly)\b", RegexOptions.IgnoreCase);
    static readonly Regex Number = new(@"\d+(\.\d+)?\s*(%|x\b|percent|times|hours?|minutes?|days?|weeks?)", RegexOptions.IgnoreCase);

    static void Questions(string path)
    {
        var text = File.ReadAllText(path).Replace("\r\n", "\n");
        var blocks = Regex.Split(text, @"^(?=### )", RegexOptions.Multiline).Where(b => b.StartsWith("### ")).ToList();
        Console.WriteLine($"{Path.GetFileName(path)}: {blocks.Count} questions");
        var cats = new Dictionary<string, int>();
        Console.WriteLine();
        Console.WriteLine("  id    category          words  evidence  not-known");
        foreach (var b in blocks)
        {
            var title = b.Split('\n')[0][4..].Trim();
            var id = Regex.Match(title, @"^[A-Z]+-?\d+").Value is { Length: > 0 } x ? x : Clip(title, 12);
            string F(string name)
            {
                var m = Regex.Match(b, $@"^\s*[-*]\s*\**{name}\**[ \t]*:[ \t]*(.*(?:\n(?!\s*[-*]\s*\**[A-Z][\w' -]*\**\s*:).*)*)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                return m.Success ? m.Groups[1].Value.Trim() : "";
            }
            var cat = F("Category").ToLowerInvariant();
            var answer = F("Answer"); var evidence = F("Evidence"); var unknown = F("Not known"); var bridge = F("Bridge");
            int words = Regex.Matches(answer, @"[\w'’%-]+").Count;
            bool hasEv = evidence.Length > 0 && evidence != "-";
            Console.WriteLine($"  {id,-5} {Clip(cat, 16),-16} {words,6}  {(hasEv ? "yes" : "NO"),-8}  {(unknown.Length > 0 && unknown != "-" ? "yes" : "no")}");
            foreach (var c in cat.Split([',', ' ', '/'], StringSplitOptions.RemoveEmptyEntries)) cats[c] = cats.GetValueOrDefault(c) + 1;
            if (answer.Length == 0) { Error($"{id}: no answer"); continue; }
            if (!hasEv) Error($"{id}: no evidence; cite your notes, an experiment, a source, or write 'opinion' and say so in the answer");
            else if (Regex.IsMatch(evidence, @"^opinion", RegexOptions.IgnoreCase)) Warn($"{id}: opinion only; say so out loud when you answer");
            else if (!Regex.IsMatch(evidence, @"(EXP|INC|FAQ|C|T)-?\d+|https?://|\]\(|report|study|survey|docs|notes|\.md|lesson|module|opinion", RegexOptions.IgnoreCase))
                Warn($"{id}: evidence '{Clip(evidence, 30)}' is not something a listener could check; point to a note, an experiment or a source");
            if (words > 130) Warn($"{id}: answer is {words} words, over a minute spoken; cut to the concession, the evidence and the boundary");
            var h = Hype.Match(answer);
            if (h.Success) Error($"{id}: '{h.Value}' is a claim no workshop can support");
            var ab = Absolute.Match(answer);
            if (ab.Success) Warn($"{id}: '{ab.Value}' is an absolute; name the boundary instead");
            if (Number.IsMatch(answer) && !Regex.IsMatch(evidence, @"(EXP|INC|FAQ|C)-?\d+|https?://|\]\(|report|study|survey|docs|notes", RegexOptions.IgnoreCase))
                Error($"{id}: the answer has a number ('{Number.Match(answer).Value}') but the evidence does not point to where it comes from");
            if (unknown.Length == 0 || unknown == "-") Warn($"{id}: no 'Not known' line; the honest limit is what makes the rest believable");
            if (bridge.Length == 0) Warn($"{id}: no bridge back to the agenda");
        }
        Console.WriteLine();
        Console.WriteLine("Categories: " + string.Join(", ", Categories.Select(c => $"{c} {cats.GetValueOrDefault(c)}")));
        foreach (var c in Categories.Where(c => cats.GetValueOrDefault(c) == 0))
            Error($"no question in category '{c}'; someone in a paid room will ask it");
        if (blocks.Count < 12) Warn($"{blocks.Count} questions; aim for at least 12 before a paid room");
    }

    // ---------------------------------------------------------------- rehearsal

    static readonly string[] Criteria = ["timing", "objectives", "narration", "wait-time", "recovery", "exercise", "questions", "claims"];

    sealed record Log(string Name, string Kind, int Audience, int NotesChecks, double Planned, double Actual,
        int Incidents, int Recovered, Dictionary<string, Dictionary<string, int>> Scores, List<string> Observers);

    static Log ReadLog(string path)
    {
        var lines = File.ReadAllLines(path);
        var all = string.Join("\n", lines);
        string H(string k) { var m = Regex.Match(all, $@"^[-*\s]*{k}:\s*(.*)$", RegexOptions.Multiline | RegexOptions.IgnoreCase); return m.Success ? m.Groups[1].Value.Trim() : ""; }
        int.TryParse(Regex.Match(H("Audience"), @"\d+").Value, out var audience);
        int notes = int.TryParse(Regex.Match(H("Notes checks"), @"\d+").Value, out var n) ? n : -1;
        var kind = H("Kind").Split(' ', '(')[0].ToLowerInvariant();
        double planned = 0, actual = 0; int incidents = 0, recovered = 0;
        var scores = new Dictionary<string, Dictionary<string, int>>();
        var observers = new List<string>();
        foreach (var (head, rows) in Tables(lines))
        {
            int C(string x) => head.FindIndex(h => h.StartsWith(x));
            if (C("planned") >= 0 && C("actual") >= 0)
                foreach (var r in rows)
                {
                    double.TryParse(r.ElementAtOrDefault(C("planned")), NumberStyles.Float, Inv, out var p);
                    double.TryParse(r.ElementAtOrDefault(C("actual")), NumberStyles.Float, Inv, out var q);
                    planned += p; actual += q;
                    if (q - p > 3) Warn($"{Path.GetFileName(path)}: '{Clip(r.ElementAtOrDefault(C("step")) ?? "", 40)}' ran {q - p:0} minutes over");
                }
            else if (C("what happened") >= 0)
                foreach (var r in rows.Where(r => Regex.IsMatch(r.ElementAtOrDefault(C("time")) ?? "", @"\d")))
                {
                    incidents++;
                    var rec = r.ElementAtOrDefault(C("recovered")) ?? "";
                    if (Regex.IsMatch(rec, @"\d")) recovered++;
                }
            else if (C("criterion") >= 0)
            {
                observers = head.Select((h, i) => (h, i)).Where(t => t.i != C("criterion")).Select(t => t.h).ToList();
                foreach (var r in rows)
                {
                    var crit = Criteria.FirstOrDefault(c => r[C("criterion")].ToLowerInvariant().StartsWith(c.Split('-')[0])) ?? r[C("criterion")].ToLowerInvariant();
                    var d = new Dictionary<string, int>();
                    for (int i = 0; i < head.Count; i++)
                        if (i != C("criterion") && int.TryParse(r.ElementAtOrDefault(i), out var v)) d[head[i]] = v;
                    scores[crit] = d;
                }
            }
        }
        return new Log(Path.GetFileName(path), kind, audience, notes, planned, actual, incidents, recovered, scores, observers);
    }

    static void Rehearsals(List<string> paths)
    {
        Console.WriteLine($"{paths.Count} rehearsal log(s)");
        var logs = new List<Log>();
        foreach (var path in paths)
        {
            Console.WriteLine();
            var l = ReadLog(path);
            logs.Add(l);
            var drift = l.Actual - l.Planned;
            Console.WriteLine($"{l.Name}: kind {(l.Kind.Length > 0 ? l.Kind : "?")}, audience {l.Audience}, notes checks {(l.NotesChecks < 0 ? "not counted" : l.NotesChecks.ToString(Inv))}");
            Console.WriteLine($"  timing: planned {l.Planned:0} min, actual {l.Actual:0} min, drift {drift:+0;-0;0} min");
            Console.WriteLine($"  incidents: {l.Incidents}, recovered with a time: {l.Recovered}");
            if (l.NotesChecks < 0) Warn($"{l.Name}: notes checks not counted; the exit standard needs the number");
            var obs = l.Observers.Where(o => !o.Equals("self", StringComparison.OrdinalIgnoreCase)).ToList();
            if (l.Observers.Any(o => o.Equals("self", StringComparison.OrdinalIgnoreCase)))
                Warn($"{l.Name}: self-scores are not observations; they are excluded");
            if (l.Scores.Count == 0) { Console.WriteLine("  observer scores: none"); continue; }
            foreach (var o in obs)
            {
                var v = l.Scores.Values.Where(d => d.ContainsKey(o)).Select(d => d[o]).ToList();
                if (v.Count > 0) Console.WriteLine($"  {o}: mean {v.Average():0.0} of 4 over {v.Count} criteria, lowest {v.Min()}");
            }
            if (obs.Count >= 2)
            {
                var pairs = l.Scores.Where(kv => kv.Value.ContainsKey(obs[0]) && kv.Value.ContainsKey(obs[1]))
                    .Select(kv => (kv.Key, a: kv.Value[obs[0]], b: kv.Value[obs[1]])).ToList();
                int exact = pairs.Count(p => p.a == p.b), within = pairs.Count(p => Math.Abs(p.a - p.b) <= 1);
                Console.WriteLine($"  agreement {obs[0]}/{obs[1]}: exact {exact} of {pairs.Count} ({Pct(exact, pairs.Count)}), within one point {within} of {pairs.Count} ({Pct(within, pairs.Count)})");
                foreach (var p in pairs.Where(p => Math.Abs(p.a - p.b) >= 2))
                    Warn($"{l.Name}: observers differ by {Math.Abs(p.a - p.b)} on '{p.Key}'; agree what a 2 and a 4 look like before the next run");
            }
        }

        // Exit standard, on the last log
        var last = logs[^1];
        var lastObs = last.Observers.Where(o => !o.Equals("self", StringComparison.OrdinalIgnoreCase)).ToList();
        Console.WriteLine();
        Console.WriteLine($"Exit standard (on {last.Name})");
        void Gate(bool ok, string what) { Console.WriteLine($"  {(ok ? "PASS" : "FAIL")}  {what}"); if (!ok) errors++; }
        Gate(logs.Count >= 3, $"three rehearsals or more ({logs.Count})");
        Gate(last.Kind == "dress", $"the last one is a dress rehearsal ({(last.Kind.Length > 0 ? last.Kind : "?")})");
        Gate(last.Audience >= 3, $"in front of at least 3 people who did not build it ({last.Audience})");
        Gate(lastObs.Count >= 2, $"two observers other than you ({lastObs.Count})");
        Gate(last.NotesChecks is >= 0 and <= 2, $"notes checked at most twice ({(last.NotesChecks < 0 ? "not counted" : last.NotesChecks.ToString(Inv))})");
        Gate(Math.Abs(last.Actual - last.Planned) <= 5 && last.Planned > 0, $"total within 5 minutes of plan ({last.Actual - last.Planned:+0;-0;0})");
        var missing = Criteria.Where(c => !last.Scores.ContainsKey(c)).ToList();
        Gate(missing.Count == 0, missing.Count == 0 ? "all eight rubric criteria scored" : $"all eight rubric criteria scored (missing: {string.Join(", ", missing)})");
        var low = last.Scores.SelectMany(kv => kv.Value.Where(o => lastObs.Contains(o.Key) && o.Value < 3).Select(o => $"{kv.Key} ({o.Key}: {o.Value})")).ToList();
        Gate(lastObs.Count > 0 && low.Count == 0, low.Count == 0 ? "no criterion below 3 from either observer" : $"no criterion below 3 from either observer: {string.Join(", ", low)}");
        Gate(logs.Any(l => l.Kind is "friendly" or "dress" && l.Recovered > 0), "a failure recovered in front of people, with the time it took");
        Console.WriteLine();
        Console.WriteLine(errors == 0 ? "READY for a paid room" : "NOT READY");
    }

    // ---------------------------------------------------------------- helpers

    static IEnumerable<(List<string> head, List<List<string>> rows)> Tables(string[] lines)
    {
        bool fence = false;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].TrimStart().StartsWith("```")) { fence = !fence; continue; }
            if (fence || !lines[i].TrimStart().StartsWith('|')) continue;
            var head = Cells(lines[i]).Select(c => c.ToLowerInvariant()).ToList();
            int j = i + 2;
            var rows = new List<List<string>>();
            while (j < lines.Length && lines[j].TrimStart().StartsWith('|')) rows.Add(Cells(lines[j++]));
            i = j - 1;
            yield return (head, rows);
        }
    }

    static List<string> Cells(string line) =>
        line.Trim().Trim('|').Split('|').Select(c => c.Trim().Replace("**", "").Replace("`", "")).ToList();

    static string Pct(double a, double b) => b == 0 ? "n/a" : (a / b).ToString("0%", Inv);
    static string Clip(string s, int n) => s.Length <= n ? s : s[..(n - 1)] + "…";
    static void Error(string m) { errors++; Console.WriteLine($"ERROR {m}"); }
    static void Warn(string m) { warnings++; Console.WriteLine($"WARN  {m}"); }

    static string NeedFile(string f) => File.Exists(f) ? f : throw new UsageException($"file not found: {f}");
    static string NeedDir(string d) => Directory.Exists(d) ? d : throw new UsageException($"folder not found: {d}");

    static void Usage() => Console.WriteLine("""
        KitCheck — Module 17 checks for a workshop kit
          agenda    <agenda.md>
          kit       <workshop-kit folder>
          questions <question-bank.md>
          rehearsal <r1.md> [<r2.md> ...]      (in the order you ran them)
        Exit code 0 = clean, 1 = errors, 2 = usage problem.
        """);

    sealed class UsageException(string m) : Exception(m);
}
