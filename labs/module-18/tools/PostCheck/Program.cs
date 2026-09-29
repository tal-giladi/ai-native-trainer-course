// PostCheck — the Module 18 tool. Dependency-free, read-only, deterministic.
//
//   calendar  <content-calendar.md> [--concepts <concepts.md>]
//             A content calendar as a funnel: every teaching piece carries one concept and its
//             evidence, few pieces ask for anything, the cadence holds, published pieces have links,
//             and the funnel is measured by the signal (questions from outside your network), not by
//             reach alone. Estimates how many pieces it takes to hear from a stranger (18.1).
//   claims    <piece.md> [--evidence <file> ...] [--deny <terms.txt>]
//             Every number has a source a reader can follow; effects from an experiment carry their
//             interval; no hype, no "studies show" without the study, no generalizing beyond the
//             population; a limits line; a disclosure line; no employer or client names (18.2).
//   format    <piece.md>
//             Format rules for posts, articles, videos, talks, live demos and case-study pieces:
//             length or runtime against the target, one concept per short piece, reviewed captions,
//             a pinned demo repository, a fallback for live demos, consent and limits for case
//             studies (18.3).
//   questions <questions.csv> [--pieces <content-calendar.md>]
//             The questions log: which themes keep coming back and were only answered in private,
//             how long it takes to turn a question into a piece, anonymized askers, and the exit
//             signal: an unprompted question from outside your network (18.4).
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
            var rest = args.Skip(1).ToList();
            var pos = rest.TakeWhile(a => !a.StartsWith("--")).ToList();
            if (pos.Count == 0) throw new UsageException("missing file argument (see --help)");
            switch (args[0])
            {
                case "calendar": Calendar(NeedFile(pos[0]), Opt(rest, "--concepts").FirstOrDefault()); break;
                case "claims": Claims(NeedFile(pos[0]), Opt(rest, "--evidence"), Opt(rest, "--deny").FirstOrDefault()); break;
                case "format": Format(NeedFile(pos[0])); break;
                case "questions": Questions(NeedFile(pos[0]), Opt(rest, "--pieces").FirstOrDefault()); break;
                default: Console.Error.WriteLine($"unknown command '{args[0]}'"); Usage(); return 2;
            }
        }
        catch (UsageException e) { Console.Error.WriteLine(e.Message); return 2; }
        catch (IOException e) { Console.Error.WriteLine(e.Message); return 2; }
        Console.WriteLine();
        Console.WriteLine($"{errors} error(s), {warnings} warning(s)");
        return errors > 0 ? 1 : 0;
    }

    static List<string> Opt(List<string> a, string name)
    {
        int i = a.IndexOf(name);
        if (i < 0) return [];
        var v = a.Skip(i + 1).TakeWhile(x => !x.StartsWith("--")).ToList();
        if (v.Count == 0) throw new UsageException($"{name} needs a value");
        return v.Select(NeedFile).ToList();
    }

    // ---------------------------------------------------------------- calendar

    static readonly string[] Stages = ["discover", "trust", "act"];
    static readonly string[] Out = ["published", "delivered"];

    sealed record Item(string Id, DateTime? Date, string Piece, string Format, string Stage, string Concept, string Evidence,
        string FromQ, string Status, string Link, double? Reach, double? Replies, double? OutsideQ, double? Conversations);

    static List<Item> ReadCalendar(string path, out Dictionary<string, string> meta, out List<string> head)
    {
        var lines = File.ReadAllLines(path);
        meta = Header(lines);
        head = [];
        foreach (var (h, rows) in Tables(lines))
        {
            int C(string n) => h.FindIndex(x => x.StartsWith(n));
            if (C("date") < 0 || C("piece") < 0 || C("status") < 0) continue;
            head = h;
            var list = new List<Item>();
            foreach (var r in rows)
            {
                string G(string n) { int i = C(n); return i >= 0 && i < r.Count ? r[i].Trim() : ""; }
                double? N(string n) { var s = G(n).Replace(",", ""); return double.TryParse(s, NumberStyles.Float, Inv, out var v) ? v : null; }
                DateTime? d = DateTime.TryParseExact(G("date"), "yyyy-MM-dd", Inv, DateTimeStyles.None, out var dt) ? dt : null;
                var piece = G("piece");
                var id = Regex.Match(piece, @"^P-\d+").Value;
                list.Add(new Item(id.Length > 0 ? id : Clip(piece, 14), d, piece, G("format").ToLowerInvariant(), G("stage").ToLowerInvariant(),
                    Dash(G("concept")), Dash(G("evidence")), Dash(G("from question")), G("status").ToLowerInvariant(), Dash(G("link")),
                    N("reach"), N("replies"), N("outside"), N("conversations")));
            }
            return list;
        }
        return [];
    }

    static void Calendar(string path, string? conceptsPath)
    {
        var items = ReadCalendar(path, out var meta, out var head);
        Console.WriteLine($"{Path.GetFileName(path)}: {items.Count} pieces");
        if (items.Count == 0) { Error("no calendar table (| Date | Piece | Format | Stage | Concept | Evidence | From question | Status | Link | ... |)"); return; }
        int cadence = meta.TryGetValue("cadence", out var cs) && int.TryParse(Regex.Match(cs, @"\d+").Value, out var cd) ? cd : 14;
        if (!meta.ContainsKey("cadence")) Warn("no 'Cadence: <days>' line; assuming 14 days between pieces");
        var concepts = conceptsPath is null ? null : ReadConcepts(conceptsPath);

        var outItems = items.Where(i => Out.Contains(i.Status)).ToList();
        var published = items.Count(i => i.Status == "published");
        Console.WriteLine($"  out: {outItems.Count} ({published} published, {outItems.Count - published} delivered in a room); planned or drafted: {items.Count - outItems.Count}");

        Console.WriteLine();
        Console.WriteLine("Pieces");
        foreach (var i in items)
        {
            if (i.Date is null) Error($"{i.Id}: date '{i.Piece}' is not yyyy-mm-dd");
            if (!Stages.Contains(i.Stage)) Warn($"{i.Id}: stage '{i.Stage}' is not one of discover, trust, act");
            if (i.Stage != "act")
            {
                if (i.Concept.Length == 0) Error($"{i.Id}: teaches no concept; name the card from concepts.md it teaches");
                else if (Regex.Matches(i.Concept, @"C\d+").Count > 1 && i.Format is "post" or "video") Warn($"{i.Id}: a {i.Format} with {Regex.Matches(i.Concept, @"C\d+").Count} concepts; short pieces carry one");
                if (i.Evidence.Length == 0) Error($"{i.Id}: no evidence; point to the incident, experiment or source the piece rests on");
            }
            if (i.Status == "published" && i.Link.Length == 0) Error($"{i.Id}: published without a link; nobody can check it, including you in a year");
            if (concepts is not null && i.Concept.Length > 0)
                foreach (Match m in Regex.Matches(i.Concept, @"C\d+"))
                {
                    if (!concepts.TryGetValue(m.Value, out var st)) Error($"{i.Id}: {m.Value} is not in {Path.GetFileName(conceptsPath)}");
                    else if (st == "retired") Error($"{i.Id}: {m.Value} is retired; teach what replaced it");
                    else if (st == "hypothesis") Warn($"{i.Id}: {m.Value} is a hypothesis; say so in the piece");
                }
        }

        Console.WriteLine();
        Console.WriteLine("Mix");
        var stages = Stages.Select(s => $"{s} {outItems.Count(i => i.Stage == s)}");
        Console.WriteLine($"  stages (out): {string.Join(", ", stages)}");
        var formats = outItems.Select(i => i.Format).Where(f => f.Length > 0).Distinct().ToList();
        Console.WriteLine($"  formats (out): {string.Join(", ", formats)}");
        int act = outItems.Count(i => i.Stage == "act");
        if (outItems.Count > 0 && (double)act / outItems.Count > 0.2)
            Error($"{act} of {outItems.Count} pieces out ({Pct(act, outItems.Count)}) ask for something; keep asks to one piece in five or fewer");
        if (formats.Count < 2 && outItems.Count >= 3) Warn("one format only; try a second (18.3)");
        int fromQ = outItems.Count(i => i.FromQ.Length > 0);
        Console.WriteLine($"  from a question: {fromQ} of {outItems.Count}");
        if (outItems.Count >= 4 && fromQ == 0) Warn("no piece answers a question someone asked; start the questions log (18.4)");
        if (published < 4) Warn($"{published} published piece(s); the module artifact needs at least 4 with links");

        Console.WriteLine();
        Console.WriteLine($"Cadence (target: a piece every {cadence} days or less)");
        var dated = outItems.Where(i => i.Date is not null).OrderBy(i => i.Date).ToList();
        for (int k = 1; k < dated.Count; k++)
        {
            var gap = (dated[k].Date!.Value - dated[k - 1].Date!.Value).TotalDays;
            if (gap > 2 * cadence) Error($"{gap:0} days of silence between {dated[k - 1].Id} and {dated[k].Id}");
            else if (gap > cadence) Warn($"{gap:0} days between {dated[k - 1].Id} and {dated[k].Id}");
        }
        for (int k = 2; k < dated.Count; k++)
            if ((dated[k].Date!.Value - dated[k - 2].Date!.Value).TotalDays <= 7)
            { Warn($"three pieces within a week ({dated[k - 2].Id} to {dated[k].Id}): a burst usually precedes a silence"); break; }
        if (dated.Count >= 2)
        {
            var span = (dated[^1].Date!.Value - dated[0].Date!.Value).TotalDays;
            Console.WriteLine($"  {dated.Count} pieces over {span:0} days, mean gap {span / (dated.Count - 1):0.0} days");
        }
        var lastOut = dated.LastOrDefault()?.Date;
        var nextPlanned = items.Where(i => !Out.Contains(i.Status) && i.Date is not null && (lastOut is null || i.Date > lastOut)).OrderBy(i => i.Date).FirstOrDefault();
        if (nextPlanned is null) Warn("nothing planned after the last piece; the funnel needs a next one");

        Console.WriteLine();
        Console.WriteLine("Funnel");
        bool hasReach = head.Any(h => h.StartsWith("reach")), hasOutside = head.Any(h => h.StartsWith("outside"));
        if (hasReach && !hasOutside) { Error("you track reach but not questions from outside your network; reach is attention, the question is the signal"); return; }
        if (!hasOutside) { Warn("no 'Outside questions' column; the exit signal cannot be tracked"); return; }
        var measured = outItems.Where(i => i.OutsideQ is not null).ToList();
        double reach = measured.Sum(i => i.Reach ?? 0), replies = measured.Sum(i => i.Replies ?? 0), oq = measured.Sum(i => i.OutsideQ ?? 0), conv = measured.Sum(i => i.Conversations ?? 0);
        Console.WriteLine($"  {measured.Count} measured pieces: reach {reach:0}, replies {replies:0}, outside questions {oq:0}, conversations {conv:0}");
        if (reach > 0) Console.WriteLine($"  replies per 1,000 reach: {1000 * replies / reach:0.0}");
        int kq = measured.Count(i => i.OutsideQ > 0);
        if (measured.Count > 0)
        {
            var (lo, hi) = Wilson(kq, measured.Count);
            double p = (double)kq / measured.Count;
            Console.WriteLine($"  pieces with at least one outside question: {kq} of {measured.Count} ({Pct(kq, measured.Count)}, 95% CI {lo:0%} to {hi:0%})");
            Console.WriteLine($"  P(at least one in the next 6 pieces) = 1 - (1 - p)^6: {1 - Math.Pow(1 - p, 6):0%} at p = {p:0.00}; {1 - Math.Pow(1 - lo, 6):0%} at the lower bound");
            if (measured.Count < 6) Warn($"{measured.Count} measured pieces; the interval is too wide to plan on");
        }
        var first = measured.Where(i => i.OutsideQ > 0 && i.Date is not null).OrderBy(i => i.Date).FirstOrDefault();
        Console.WriteLine(first is null ? "  exit signal: not yet (no question from outside your network)" : $"  exit signal: seen, first on {first.Date:yyyy-MM-dd} ({first.Id}); confirm it in the questions log");
    }

    static Dictionary<string, string> ReadConcepts(string path)
    {
        var d = new Dictionary<string, string>();
        var text = File.ReadAllText(path).Replace("\r\n", "\n");
        foreach (Match m in Regex.Matches(text, @"^##\s+(C\d+)\s*·[^\n]*\n(?:(?!##)[^\n]*\n)*?[-*]\s*Status:\s*(\w+)", RegexOptions.Multiline))
            d[m.Groups[1].Value] = m.Groups[2].Value.ToLowerInvariant();
        return d;
    }

    // ---------------------------------------------------------------- claims

    static readonly Regex Hype = new(@"\b(guarantee[ds]?|10x|100x|game[- ]chang\w*|revolutioni[sz]\w*|zero risk|no risk|completely safe|proven to|always works|never fails|magic(al)?|effortless(ly)?|mind[- ]blowing|insane(ly)?|skyrocket\w*|the end of (coding|programming|developers))", RegexOptions.IgnoreCase);
    static readonly Regex Absolute = new(@"\b(always|never|everyone|nobody|definitely|certainly|obviously)\b", RegexOptions.IgnoreCase);
    static readonly Regex Effect = new(@"[−+-]?\d+(\.\d+)?\s*(%|x\b|×|percent\b|times (faster|more|fewer|less|cheaper)\b)|[$€£]\s?\d[\d,.]*(\s*(k|m|million|billion)\b)?", RegexOptions.IgnoreCase);
    static readonly Regex Source = new(@"\b(EXP|INC|FAQ)-\d+\b|\[link\]", RegexOptions.IgnoreCase);
    static readonly Regex Interval = new(@"\bCI\b|interval|\d+\s*%?\s*(to|–)\s*[−+-]?\d+\s*%", RegexOptions.IgnoreCase);
    static readonly Regex StudiesShow = new(@"\b(studies|research|data|science|experts?|surveys?)\s+(show|shows|prove|proves|say|says|agree)\b", RegexOptions.IgnoreCase);
    static readonly Regex General = new(@"\b(every|all|any|most)\s+(teams?|developers?|companies|engineers|orgs?|organi[sz]ations)\b|\bteams like yours\b|\byou will (see|get)\b", RegexOptions.IgnoreCase);
    static readonly Regex Limits = new(@"does not (show|apply)|doesn.t apply|doesn't show|do not know|don't know|not (yet )?(tested|measured)|\blimits?\b|caveat|boundary|one team|one repository", RegexOptions.IgnoreCase);

    sealed record Piece(Dictionary<string, string> Meta, string Body, List<string> Sentences, string Raw);

    static Piece ReadPiece(string path)
    {
        var raw = File.ReadAllText(path).Replace("\r\n", "\n");
        var lines = raw.Split('\n');
        int sep = Array.FindIndex(lines, l => l.Trim() == "---");
        var meta = Header(sep > 0 ? lines[..sep] : lines);
        if (sep < 0) Warn("no '---' line between the header and the body; the whole file is read as the body");
        var bodyLines = (sep >= 0 ? lines[(sep + 1)..] : lines).ToList();
        var kept = new List<string>();
        bool fence = false;
        foreach (var l in bodyLines)
        {
            if (l.TrimStart().StartsWith("```")) { fence = !fence; continue; }
            if (!fence) kept.Add(l);
        }
        var body = string.Join("\n", kept);
        var linked = Regex.Replace(body, @"\[([^\]]*)\]\(([^)]*)\)", "$1 [link]");
        var sentences = new List<string>();
        foreach (var para in Regex.Split(linked, @"\n\s*\n|\n(?=\s*(?:[-*]|\d+\.|#|>|\[)\s)"))
        {
            if (string.IsNullOrWhiteSpace(para)) continue;
            foreach (var s in Regex.Split(para.Replace('\n', ' '), @"(?<=[.!?][""”’)]?)\s+(?=[A-Z0-9""“(\[])"))
            {
                var t = Regex.Replace(s, @"^\s*([-*]|\d+\.|#+|>)\s*", "").Trim();
                if (t.Length > 0) sentences.Add(t);
            }
        }
        return new Piece(meta, body, sentences, raw);
    }

    static void Claims(string path, List<string> evidence, string? deny)
    {
        var p = ReadPiece(path);
        Console.WriteLine($"{Path.GetFileName(path)}: {p.Meta.GetValueOrDefault("format", "?")}, concept {p.Meta.GetValueOrDefault("concept", "?")}, {p.Sentences.Count} sentences");

        var known = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var concepts = new Dictionary<string, string>();
        foreach (var f in evidence)
        {
            foreach (Match m in Regex.Matches(File.ReadAllText(f), @"\b(EXP|INC|FAQ)-\d+\b")) known.Add(m.Value);
            foreach (var kv in ReadConcepts(f)) concepts[kv.Key] = kv.Value;
        }
        if (evidence.Count == 0) Warn("no --evidence files; cited IDs are not checked against your notes");

        Console.WriteLine();
        Console.WriteLine("Sentences with a number or a strong claim");
        int effects = 0, sourced = 0, intervals = 0;
        foreach (var s in p.Sentences)
        {
            var flags = new List<string>();
            var e = Effect.Match(s);
            bool src = Source.IsMatch(s);
            if (e.Success)
            {
                effects++;
                if (src) sourced++;
                if (!src) { Error($"'{e.Value.Trim()}' has no source a reader can follow: \"{Clip(s, 90)}\""); flags.Add("no source"); }
                else if (Regex.IsMatch(s, @"EXP-\d+") && !Interval.IsMatch(s)) { Error($"'{e.Value.Trim()}' comes from an experiment but has no interval: \"{Clip(s, 90)}\""); flags.Add("no interval"); }
                if (Interval.IsMatch(s)) intervals++;
            }
            var h = Hype.Match(s);
            if (h.Success) { Error($"'{h.Value}' is a claim no evidence you have supports: \"{Clip(s, 90)}\""); flags.Add("hype"); }
            var st = StudiesShow.Match(s);
            if (st.Success && !Regex.IsMatch(s, @"\[link\]")) { Error($"'{st.Value}' without the study linked in the same sentence: \"{Clip(s, 90)}\""); flags.Add("unnamed study"); }
            var g = General.Match(s);
            if (g.Success && (e.Success || Regex.IsMatch(s, @"\b(faster|slower|fewer|better|save[sd]?|cut|productiv\w*)\b", RegexOptions.IgnoreCase)))
            { Warn($"'{g.Value}' generalizes an effect beyond the people you measured: \"{Clip(s, 90)}\""); flags.Add("rung 4"); }
            var ab = Absolute.Match(s);
            if (ab.Success) { Warn($"'{ab.Value}' is an absolute; name the boundary instead: \"{Clip(s, 70)}\""); flags.Add("absolute"); }
            foreach (Match id in Regex.Matches(s, @"\b(EXP|INC|FAQ)-\d+\b"))
                if (evidence.Count > 0 && !known.Contains(id.Value)) { Error($"cites {id.Value}, which is not in the evidence files"); flags.Add("unknown id"); }
            if (flags.Count > 0 || e.Success) Console.WriteLine($"  {(flags.Count == 0 ? "ok  " : "FIX ")} {Clip(s, 72)}{(flags.Count > 0 ? $"  [{string.Join(", ", flags)}]" : "")}");
        }
        Console.WriteLine();
        Console.WriteLine($"Numbers: {effects} effect claim(s), {sourced} sourced, {intervals} with an interval");

        var concept = p.Meta.GetValueOrDefault("concept", "");
        foreach (Match m in Regex.Matches(concept, @"C\d+"))
            if (concepts.TryGetValue(m.Value, out var status))
            {
                if (status == "retired") Error($"{m.Value} is retired in concepts.md; teach what replaced it");
                if (status == "hypothesis" && !Regex.IsMatch(p.Body, @"hypothes", RegexOptions.IgnoreCase)) Error($"{m.Value} is a hypothesis in concepts.md, but the piece never says so");
            }
            else if (concepts.Count > 0) Error($"{m.Value} is not a card in the evidence files");

        if (!Limits.IsMatch(p.Body))
        {
            if (effects > 0) Error("numbers but no limits line; say what this does not show (one team, one repository, the interval)");
            else Warn("no limits line; one sentence on where this does not apply makes the rest believable");
        }
        if (!p.Meta.ContainsKey("disclosure")) Warn("no 'Disclosure:' line; state any material connection (employer, vendor, free access), or 'none'");

        if (deny is not null)
        {
            var terms = File.ReadAllLines(deny).Select(t => t.Trim()).Where(t => t.Length > 0 && !t.StartsWith('#')).ToList();
            foreach (var t in terms)
            {
                int n = Regex.Matches(p.Raw, Regex.Escape(t), RegexOptions.IgnoreCase).Count;
                if (n > 0) Error($"deny-list term '{t}' appears {n} time(s)");
            }
        }
        else Warn("no --deny list; employer and client names are not checked");

        var words = p.Sentences.Select(s => Regex.Matches(s, @"[\w'’%-]+").Count).ToList();
        if (words.Count > 0)
        {
            Console.WriteLine($"Readability: mean sentence {words.Average():0.0} words, longest {words.Max()}");
            if (words.Average() > 22) Warn($"mean sentence length {words.Average():0} words; aim for under 20");
            foreach (var s in p.Sentences.Where(s => Regex.Matches(s, @"[\w'’%-]+").Count > 35).Take(3))
                Warn($"long sentence ({Regex.Matches(s, @"[\w'’%-]+").Count} words): \"{Clip(s, 60)}\"");
        }
    }

    // ---------------------------------------------------------------- format

    static void Format(string path)
    {
        var p = ReadPiece(path);
        var fmt = p.Meta.GetValueOrDefault("format", "").ToLowerInvariant().Split(' ', '(')[0];
        var concepts = Regex.Matches(p.Meta.GetValueOrDefault("concept", ""), @"C\d+").Count;
        var spoken = string.Join("\n", p.Body.Split('\n').Where(l => !Regex.IsMatch(l, @"^\s*\[")));
        int words = Regex.Matches(spoken, @"[\w'’%-]+").Count;
        double target = double.TryParse(Regex.Match(p.Meta.GetValueOrDefault("target", ""), @"\d+(\.\d+)?").Value, NumberStyles.Float, Inv, out var t) ? t : 0;
        Console.WriteLine($"{Path.GetFileName(path)}: format '{fmt}', {words} words, {concepts} concept(s){(target > 0 ? $", target {target:0}" : "")}");
        if (!p.Meta.ContainsKey("evidence")) Warn("no 'Evidence:' line");
        if (!p.Meta.ContainsKey("audience")) Warn("no 'Audience:' line; a piece for everyone is for no one");

        switch (fmt)
        {
            case "post":
                if (words > 600) Error($"{words} words is an article, not a post; cut to one idea or change the format");
                else if (words > 300) Warn($"{words} words; posts that teach one idea usually fit in 300");
                if (concepts > 1) Error($"{concepts} concepts in a post; one piece, one concept");
                var firstS = p.Sentences.FirstOrDefault() ?? "";
                if (Regex.IsMatch(firstS, @"^(I'?m |I am |We'?re )?(so )?(excited|thrilled|happy|proud|delighted)\b", RegexOptions.IgnoreCase))
                    Warn("the first line is about you; open with the reader's problem");
                if (Regex.Matches(firstS, @"[\w'’%-]+").Count > 25) Warn("the first sentence is over 25 words; it is the only one most people read");
                var lastPara = Regex.Split(p.Body.Trim(), @"\n\s*\n").LastOrDefault() ?? "";
                if (!lastPara.Contains('?')) Warn("no question at the end; ask one to invite the replies you want");
                break;
            case "article":
                if (words < 400) Warn($"{words} words; short for an article, maybe a post");
                if (words > 2500) Warn($"{words} words; split into two pieces");
                if (Regex.Matches(p.Body, @"^##\s", RegexOptions.Multiline).Count < 2) Warn("fewer than two ## headings; readers scan first");
                if (concepts > 2) Warn($"{concepts} concepts; an article carries one or two");
                break;
            case "video":
                if (target <= 0) { Error("no 'Target: <minutes>' line"); break; }
                var est = words / 150.0;
                Console.WriteLine($"  estimated speaking time at 150 words per minute: {est:0.0} min, plus screen-only time (target {target:0})");
                if (est > target * 1.1) Error($"the script runs about {est:0.0} minutes; the target is {target:0}");
                if (target > 6) Warn($"target {target:0} minutes; in the largest study of lecture videos, median watch time stayed near 6 minutes whatever the length");
                if (concepts > 1) Error($"{concepts} concepts in a video; one video, one concept");
                var cap = p.Meta.GetValueOrDefault("captions", "").ToLowerInvariant();
                if (cap.Length == 0 || cap.StartsWith("none")) Error("no captions; prerecorded video needs them (WCAG 1.2.2, level A)");
                else if (cap.StartsWith("auto")) Warn("auto-generated captions; review them, especially numbers and code names");
                RepoRule(p, true);
                var opening = string.Join(" ", Regex.Matches(spoken, @"[\w'’%-]+").Take(40).Select(m => m.Value));
                if (Regex.IsMatch(opening, @"\b(hi|hello|welcome|my name)\b", RegexOptions.IgnoreCase) && !opening.Contains('?'))
                    Warn("the first 15 seconds are an introduction; open with the problem, introduce yourself later");
                if (!p.Meta.ContainsKey("hook")) Warn("no 'Hook:' line; write the problem the first 15 seconds state");
                break;
            case "talk":
            case "lunch-and-learn":
                if (target <= 0) { Error("no 'Target: <minutes>' line (the slot)"); break; }
                double qa = double.TryParse(Regex.Match(p.Meta.GetValueOrDefault("q&a", ""), @"\d+").Value, NumberStyles.Float, Inv, out var q) ? q : 0;
                if (qa == 0) Warn("no 'Q&A: <minutes>' line; the questions are the point (18.4)");
                var talkEst = words / 130.0;
                Console.WriteLine($"  estimated speaking time at 130 words per minute: {talkEst:0.0} min; slot {target:0} min, Q&A {qa:0} min");
                if (talkEst > target - qa) Error($"speaking time {talkEst:0.0} min leaves no room for the {qa:0} min of Q&A in a {target:0}-minute slot");
                if (concepts > 3) Warn($"{concepts} concepts in one talk; three is already a lot");
                if (!p.Meta.ContainsKey("feedback")) Warn("no 'Feedback:' line; name the form and the one question you will read first");
                break;
            case "demo":
                RepoRule(p, true);
                if (!p.Meta.ContainsKey("fallback")) Error("a live demo without a 'Fallback:' line (15.4)");
                if (concepts > 1) Warn($"{concepts} concepts in one demo");
                break;
            case "case-study":
                foreach (var sec in new[] { "Before", "Intervention", "After" })
                    if (!Regex.IsMatch(p.Body, $@"^##\s*{sec}\b", RegexOptions.Multiline | RegexOptions.IgnoreCase)) Error($"no '## {sec}' section");
                if (!Regex.IsMatch(p.Body, @"^##\s*(Limits|What this does not show)", RegexOptions.Multiline | RegexOptions.IgnoreCase)) Error("no '## Limits' or '## What this does not show' section");
                var consent = p.Meta.GetValueOrDefault("consent", "");
                if (!Regex.IsMatch(consent, @"^yes\b.*\d{4}-\d{2}-\d{2}", RegexOptions.IgnoreCase)) Error("no 'Consent: yes, <who>, <yyyy-mm-dd>' line; publish nothing about a client or pilot without written consent");
                if (Regex.IsMatch(p.Body, "[\"“][^\"”]{3,}[\"”]") && !p.Meta.ContainsKey("quotes")) Warn("the piece quotes people; add a 'Quotes:' line saying who approved which quote");
                break;
            default:
                Error($"unknown format '{fmt}' (post, article, video, talk, lunch-and-learn, demo, case-study)");
                break;
        }
    }

    static void RepoRule(Piece p, bool required)
    {
        var repo = p.Meta.GetValueOrDefault("repo", "");
        if (repo.Length == 0) { if (required) Error("no 'Repo:' line; show the public demo repository, pinned, never employer code"); return; }
        if (Regex.IsMatch(repo, @"\b(at work|work|employer|client|customer|internal|private|company)\b", RegexOptions.IgnoreCase))
            Error($"'{repo}' looks like employer or client code; film only the public demo repository (15.1)");
        if (!Regex.IsMatch(repo, @"@|#|\btag\b|/tree/|/commit/|\bv\d", RegexOptions.IgnoreCase)) Warn("the repository is not pinned to a tag or commit; the piece will stop matching the code");
    }

    // ---------------------------------------------------------------- questions

    static void Questions(string path, string? piecesPath)
    {
        var rows = Csv(path);
        Console.WriteLine($"{Path.GetFileName(path)}: {rows.Count} questions");
        if (rows.Count == 0) { Error("no rows (date,channel,network,prompted,asker,question,theme,answered_in)"); return; }
        var dates = new Dictionary<string, DateTime>();
        var planned = new HashSet<string>();
        if (piecesPath is not null)
            foreach (var i in ReadCalendar(piecesPath, out _, out _))
            {
                if (i.Date is not null && i.Id.StartsWith("P-")) dates[i.Id] = i.Date.Value;
                if (i.Id.StartsWith("P-") && !Out.Contains(i.Status)) planned.Add(i.Id);
            }

        string G(Dictionary<string, string> r, string k) => r.GetValueOrDefault(k, "").Trim();
        foreach (var r in rows)
        {
            var who = G(r, "asker");
            if (who.Contains('@') || Regex.IsMatch(who, @"\b[A-Z][a-z]+ [A-Z][a-z]+\b"))
                Error($"{G(r, "date")}: asker '{who}' looks like a name or address; use an anonymous id or a role");
            if (G(r, "network") is not ("inside" or "outside")) Warn($"{G(r, "date")}: network '{G(r, "network")}' is not inside or outside");
            if (G(r, "prompted") is not ("yes" or "no")) Warn($"{G(r, "date")}: prompted '{G(r, "prompted")}' is not yes or no");
        }

        Console.WriteLine();
        Console.WriteLine("Where questions come from");
        foreach (var g in rows.GroupBy(r => G(r, "channel")).OrderByDescending(g => g.Count()))
            Console.WriteLine($"  {g.Key,-18} {g.Count(),3}");
        int outside = rows.Count(r => G(r, "network") == "outside"), unprompted = rows.Count(r => G(r, "prompted") == "no");
        Console.WriteLine($"  outside your network: {outside} of {rows.Count}; unprompted: {unprompted} of {rows.Count}");
        if (outside == 0) Warn("every question came from people who already know you; publish where strangers can reply");

        static bool IsPublic(string a) => Regex.IsMatch(a, @"^(P-\d+|FAQ-\d+|https?://)", RegexOptions.IgnoreCase);
        Console.WriteLine();
        Console.WriteLine("  theme                         asks  outside  answered in public");
        var turnaround = new List<double>();
        foreach (var g in rows.GroupBy(r => G(r, "theme").ToLowerInvariant()).OrderByDescending(g => g.Count()).ThenBy(g => g.Key, StringComparer.Ordinal))
        {
            var answers = g.Select(r => Regex.Match(G(r, "answered_in"), @"^\S+").Value).Where(IsPublic).Distinct().ToList();
            var pub = answers.Where(a => !planned.Contains(a)).ToList();
            var plan = answers.Where(a => planned.Contains(a)).ToList();
            Console.WriteLine($"  {Clip(g.Key, 30),-30} {g.Count(),4}  {g.Count(r => G(r, "network") == "outside"),7}  {(pub.Count > 0 ? string.Join(", ", pub) : plan.Count > 0 ? $"planned: {string.Join(", ", plan)}" : "no")}");
            if (g.Key.Length == 0) { Warn($"{g.Count()} question(s) without a theme"); continue; }
            if (pub.Count == 0 && plan.Count > 0) Warn($"'{g.Key}' asked {g.Count()} times; the answer is planned in {string.Join(", ", plan)}, not out yet");
            else if (pub.Count == 0 && g.Count() >= 3) Error($"'{g.Key}' asked {g.Count()} times and answered only in private; that is your next piece");
            else if (pub.Count == 0 && g.Count() == 2) Warn($"'{g.Key}' asked twice; one more and it is a piece");
            var firstAsk = g.Select(r => DateTime.TryParseExact(G(r, "date"), "yyyy-MM-dd", Inv, DateTimeStyles.None, out var d) ? d : (DateTime?)null).Where(d => d is not null).Min();
            foreach (var pid in pub.Where(x => x.StartsWith("P-")))
                if (dates.TryGetValue(pid, out var pd) && firstAsk is not null && pd > firstAsk.Value) turnaround.Add((pd - firstAsk.Value).TotalDays);
        }
        int privateOnly = rows.Count(r => { var a = Regex.Match(G(r, "answered_in"), @"^\S+").Value; return !IsPublic(a) || planned.Contains(a); });
        Console.WriteLine();
        Console.WriteLine($"Answered only in private or not yet: {privateOnly} of {rows.Count}");
        if (turnaround.Count > 0)
        {
            var s = turnaround.OrderBy(x => x).ToList();
            var med = s.Count % 2 == 1 ? s[s.Count / 2] : (s[s.Count / 2 - 1] + s[s.Count / 2]) / 2;
            Console.WriteLine($"Question to piece: median {med:0} days over {s.Count} piece(s)");
            if (med > 30) Warn($"median {med:0} days from first ask to piece; answer while the question is warm");
        }
        else if (piecesPath is null) Console.WriteLine("Question to piece: pass --pieces <content-calendar.md> to measure it");

        Console.WriteLine();
        var exit = rows.Where(r => G(r, "network") == "outside" && G(r, "prompted") == "no").OrderBy(r => G(r, "date"), StringComparer.Ordinal).FirstOrDefault();
        if (exit is null) { Console.WriteLine("Exit signal: NOT YET (no unprompted question from outside your network)"); Warn("exit signal not yet seen"); }
        else Console.WriteLine($"Exit signal: SEEN on {G(exit, "date")} via {G(exit, "channel")}: \"{Clip(G(exit, "question"), 70)}\"");
    }

    // ---------------------------------------------------------------- helpers

    static (double lo, double hi) Wilson(int k, int n, double z = 1.96)
    {
        if (n == 0) return (0, 1);
        double p = (double)k / n, d = 1 + z * z / n, c = (p + z * z / (2 * n)) / d;
        double h = z * Math.Sqrt(p * (1 - p) / n + z * z / (4.0 * n * n)) / d;
        return (Math.Max(0, c - h), Math.Min(1, c + h));
    }

    static Dictionary<string, string> Header(string[] lines)
    {
        var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        bool fence = false;
        foreach (var l in lines)
        {
            if (l.TrimStart().StartsWith("```")) { fence = !fence; continue; }
            if (fence) continue;
            var m = Regex.Match(l, @"^[-*]\s*\**([A-Za-z][\w &/-]*?)\**\s*:\s*(.*)$");
            if (m.Success && !d.ContainsKey(m.Groups[1].Value.Trim())) d[m.Groups[1].Value.Trim()] = m.Groups[2].Value.Trim();
        }
        return d;
    }

    static List<Dictionary<string, string>> Csv(string path)
    {
        var lines = File.ReadAllLines(path).Where(l => l.Trim().Length > 0 && !l.StartsWith('#')).ToList();
        if (lines.Count == 0) return [];
        var head = Split(lines[0]).Select(h => h.Trim().ToLowerInvariant()).ToList();
        return lines.Skip(1).Select(l =>
        {
            var c = Split(l);
            var d = new Dictionary<string, string>();
            for (int i = 0; i < head.Count; i++) d[head[i]] = i < c.Count ? c[i] : "";
            return d;
        }).ToList();
    }

    static List<string> Split(string line)
    {
        var res = new List<string>(); var cur = new System.Text.StringBuilder(); bool q = false;
        for (int i = 0; i < line.Length; i++)
        {
            char ch = line[i];
            if (q) { if (ch == '"' && i + 1 < line.Length && line[i + 1] == '"') { cur.Append('"'); i++; } else if (ch == '"') q = false; else cur.Append(ch); }
            else if (ch == '"') q = true;
            else if (ch == ',') { res.Add(cur.ToString()); cur.Clear(); }
            else cur.Append(ch);
        }
        res.Add(cur.ToString());
        return res;
    }

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

    static string Dash(string s) => s is "-" or "—" or "–" ? "" : s;
    static string Pct(double a, double b) => b == 0 ? "n/a" : (a / b).ToString("0%", Inv);
    static string Clip(string s, int n) => s.Length <= n ? s : s[..(n - 3)] + "...";
    static void Error(string m) { errors++; Console.WriteLine($"ERROR {m}"); }
    static void Warn(string m) { warnings++; Console.WriteLine($"WARN  {m}"); }
    static string NeedFile(string f) => File.Exists(f) ? f : throw new UsageException($"file not found: {f}");

    static void Usage() => Console.WriteLine("""
        PostCheck — Module 18 checks for teaching in public
          calendar  <content-calendar.md> [--concepts <concepts.md>]
          claims    <piece.md> [--evidence <file> ...] [--deny <terms.txt>]
          format    <piece.md>
          questions <questions.csv> [--pieces <content-calendar.md>]
        Exit code 0 = clean, 1 = errors, 2 = usage problem.
        """);

    sealed class UsageException(string m) : Exception(m);
}
