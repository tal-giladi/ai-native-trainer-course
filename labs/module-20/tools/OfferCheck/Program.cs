// OfferCheck — the Module 20 tool. Dependency-free, read-only, deterministic.
//
//   ladder   <offer-ladder.md>   Positioning line, 3–5 rungs, every rung with problem, deliverables,
//                                duration, a real price, evidence, entry criteria and a next step;
//                                capacity stated; no promises or pressure tactics (20.1).
//   price    <pricing.md>        Pricing worksheet: value range from a measured effect with its
//                                interval (corners and a seeded Monte Carlo), quality costs,
//                                ROI range after the fee, probability of loss, payback (20.2).
//   qualify  <applications.csv>  Application-form answers: fit score, conflicts, expectation
//                                mismatches, personal data the form should not collect (20.3).
//   call     <transcript.md>     Discovery-call transcript: talk share, first pitch, past-event,
//                                decision, budget and timing questions, pressure and promises,
//                                an honest limit said aloud, a dated next step (20.3).
//   proposal <proposal.md>       Situation in the client's words, options with prices, evidence IDs,
//                                what is not promised, a dated next step, no fake urgency (20.4).
//   sow      <sow.md>            Required sections, acceptance criteria per deliverable, exclusions,
//                                change control, payment terms, fee arithmetic, measurement instead
//                                of a guarantee, background IP (20.4).
//   changes  <change-log.md>     Scope requests: nothing outside the SOW done without a written
//                                change request or a logged decision (20.4, 20.5).
//   admin    <admin.md>          Employer-conflict, contract and invoicing register: blockers,
//                                professional review, invoice numbering and due dates (20.5).
//
// Legal and tax checks are orientation only: the tool checks that a question was asked and
// answered in writing, never that the answer is right for your jurisdiction.
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
        if (args.Length == 0 || args[0] is "-h" or "--help") { Usage(); return 0; }
        try
        {
            if (args.Length < 2) throw new UsageException("missing file argument (see --help)");
            var f = NeedFile(args[1]);
            switch (args[0])
            {
                case "ladder": Ladder(f); break;
                case "price": Price(f); break;
                case "qualify": Qualify(f); break;
                case "call": Call(f); break;
                case "proposal": Proposal(f); break;
                case "sow": Sow(f); break;
                case "changes": Changes(f); break;
                case "admin": Admin(f); break;
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
        "OfferCheck <command> <file>\n" +
        "  ladder   <offer-ladder.md>    offer ladder and positioning (20.1)\n" +
        "  price    <pricing.md>         value and ROI ranges from a measured effect (20.2)\n" +
        "  qualify  <applications.csv>   application-form answers (20.3)\n" +
        "  call     <transcript.md>      discovery-call transcript (20.3)\n" +
        "  proposal <proposal.md>        proposal (20.4)\n" +
        "  sow      <sow.md>             statement of work (20.4)\n" +
        "  changes  <change-log.md>      scope requests and change requests (20.4)\n" +
        "  admin    <admin.md>           employer conflicts, contract and invoicing register (20.5)\n" +
        "Exit code 0 = clean, 1 = errors, 2 = usage problem.");

    // ---------------------------------------------------------------- shared text checks

    static readonly Regex Negation = new(@"\b(no|not|never|cannot|can't|don't|doesn't|won't|without|nor|isn't|aren't)\b", I);

    static IEnumerable<string> Sentences(string text) =>
        Regex.Split(StripFences(text), @"(?<=[.!?])\s+|\n").Select(s => s.Trim()).Where(s => s.Length > 0);

    static string StripFences(string text) => Regex.Replace(text, "```.*?```", "", RegexOptions.Singleline);

    // Outcome promises: guarantees, multipliers, percentages with no interval.
    static int Promises(string text, string where)
    {
        int n = 0;
        foreach (var s in Sentences(text))
        {
            // Questions and reported speech ("your VP wants...") are not your claims.
            if (s.EndsWith('?') || Regex.IsMatch(s, @"\b(wants|wanted|asked|asks|mentions|mentioned|you wrote|your form|they said|he said|she said)\b", I)) continue;
            if (Regex.IsMatch(s, @"guarante", I) && !Negation.IsMatch(s))
            { Error($"{where}: guarantee of an outcome: \"{Clip(s, 90)}\""); n++; }
            if (Regex.IsMatch(s, @"\b\d+(\.\d+)?\s?x\b(?!\s*\d)", I) && !Negation.IsMatch(s))
            { Error($"{where}: multiplier claim: \"{Clip(s, 90)}\""); n++; }
            if (Regex.IsMatch(s, @"\b\d+(\.\d+)?\s?%\s+(faster|more productive|productivity|improvement|increase|reduction|fewer|less|higher|lower|better|shorter)", I)
                && !Regex.IsMatch(s, @"interval|\bCI\b|range|\bto\s+[−-]?\d|–\s?\d|EXP-\d+", I) && !Negation.IsMatch(s))
            { Error($"{where}: effect without interval or source: \"{Clip(s, 90)}\""); n++; }
        }
        return n;
    }

    static readonly (string pattern, string name)[] PressurePatterns =
    [
        (@"\bonly \d+ (spots?|slots?|places?|seats?)\b", "scarcity claim"),
        (@"\b(price|prices|rate|rates)\s+(goes|go|will go|is going)\s+up\b", "price-rise threat"),
        (@"\b(today|this week) only\b", "deadline pressure"),
        (@"\bsign (today|now|tonight|by end of day)\b", "deadline pressure"),
        (@"\bexpires? in \d+ (hours?|days?)\b", "exploding offer"),
        (@"\blast chance\b", "deadline pressure"),
        (@"\bdon'?t miss\b", "fear of missing out"),
        (@"\b(everyone|all your competitors)( else)?\b[^.]{0,30}\b(is|are) (already )?(doing|buying|signing|adopting|using)", "bandwagon pressure"),
        (@"\bact now\b", "deadline pressure"),
        (@"\blimited[- ]time\b", "deadline pressure"),
        (@"\bbefore it'?s too late\b", "fear appeal"),
        (@"\byou'?d be (crazy|mad|foolish)\b", "shaming"),
    ];

    static int Pressure(string text, string where)
    {
        int n = 0;
        foreach (var s in Sentences(text))
            foreach (var (p, name) in PressurePatterns)
                if (Regex.IsMatch(s, p, I)) { Error($"{where}: {name}: \"{Clip(s, 90)}\""); n++; break; }
        return n;
    }

    // ---------------------------------------------------------------- ladder (20.1)

    static void Ladder(string path)
    {
        var lines = File.ReadAllLines(path);
        var text = string.Join("\n", lines);
        var kv = KeyValues(lines);
        Console.WriteLine($"{Path.GetFileName(path)}: offer ladder");

        if (!kv.TryGetValue("positioning", out var pos)) Error("no 'Positioning:' line (for whom, which problem, unlike what)");
        else
        {
            Console.WriteLine($"  positioning: {Clip(pos, 110)}");
            var generic = Regex.Matches(pos, @"any (team|company|industry|stack)|everyone|all industries|ai transformation|digital transformation|end-to-end|world-class|cutting-edge", I);
            foreach (Match g in generic) Warn($"positioning: generic phrase '{g.Value}' (name one audience and one problem)");
            if (!Regex.IsMatch(pos, @"\bfor\b", I) || !Regex.IsMatch(pos, @"\bunlike\b|\binstead of\b|\brather than\b", I))
                Warn("positioning: say who it is for and what it is unlike (the alternative the buyer has today)");
        }
        if (!kv.TryGetValue("capacity", out var cap)) Warn("no 'Capacity:' line; any scarcity you mention must be a real, stated cap");
        else Console.WriteLine($"  capacity: {cap}");

        var t = Tables(lines).FirstOrDefault(x => x.head.Any(h => h.StartsWith("rung")) && x.head.Any(h => h.StartsWith("price")));
        if (t.head == null) { Error("no ladder table (columns: Rung | Offer | Problem | Deliverables | Duration | Price | Evidence | Entry | Next)"); return; }
        int C(string n) => t.head.FindIndex(h => h.StartsWith(n));
        string G(List<string> r, string n) => C(n) >= 0 && C(n) < r.Count ? Dash(r[C(n)]) : "";

        var rows = t.rows;
        if (rows.Count < 3) Error($"{rows.Count} rung(s); a ladder needs at least 3 (a first step, a measured step, a bigger commitment)");
        if (rows.Count > 5) Warn($"{rows.Count} rungs; more than 5 and buyers compare options instead of taking the next step");

        Console.WriteLine();
        Console.WriteLine($"  {"rung",-5}{"offer",-34}{"price",-28}{"next",-10}");
        double? prevOnce = null;
        for (int i = 0; i < rows.Count; i++)
        {
            var r = rows[i];
            string rung = G(r, "rung"), offer = G(r, "offer"), price = G(r, "price"), next = G(r, "next");
            Console.WriteLine($"  {rung,-5}{Clip(offer, 32),-34}{Clip(price, 26),-28}{Clip(next, 10),-10}");
            string where = $"rung {(rung == "" ? (i + 1).ToString(Inv) : rung)}";
            foreach (var (col, label) in new[] { ("offer", "offer"), ("problem", "problem"), ("deliverable", "deliverables"), ("duration", "duration"), ("price", "price"), ("evidence", "evidence"), ("entry", "entry criteria") })
                if (C(col) < 0) { if (i == 0) Error($"ladder table has no '{label}' column"); }
                else if (G(r, col) == "") Error($"{where}: missing {label}");
            if (price != "" && (!Regex.IsMatch(price, @"\d") || Regex.IsMatch(price, @"tbd|contact|on request|call us|ask", I)))
                Error($"{where}: price '{price}' is not a price (a fixed fee or a range with a currency)");
            else if (price != "" && !Regex.IsMatch(price, @"[$€£₪]|USD|EUR|GBP|ILS", I))
                Warn($"{where}: price has no currency");
            var ev = G(r, "evidence");
            if (ev != "" && !Regex.IsMatch(ev, @"\b[A-Z]{2,}-[A-Z]?\d+\b|pre/post|gain|follow-up", I))
                Warn($"{where}: evidence '{Clip(ev, 40)}' names no experiment, incident or measured result ID");
            var entry = G(r, "entry");
            if (i > 0 && Regex.IsMatch(entry, @"^(none|anyone|any|open|n/a)$", I))
                Warn($"{where}: no entry criteria; a later rung should follow evidence from an earlier one");
            if (i < rows.Count - 1 && next == "") Warn($"{where}: no next step; what does a buyer who is happy with this rung do next?");
            Promises(G(r, "deliverable") + ". " + G(r, "problem") + ". " + offer, where);
            var once = FirstNumber(price);
            bool recurring = Regex.IsMatch(price, @"/\s?(month|mo|quarter)|per month|monthly", I);
            if (once is double p && !recurring)
            {
                if (prevOnce is double q && p < q) Warn($"{where}: cheaper than the rung below it; is the order right?");
                prevOnce = p;
            }
        }
        Console.WriteLine();
        Promises(string.Join("\n", lines.Where(l => !l.TrimStart().StartsWith('|'))), "ladder text");
        Pressure(text, "ladder");
    }

    // ---------------------------------------------------------------- price (20.2)

    sealed record In(string Name, double Lo, double Hi, string Unit, string Source);

    static readonly string[] Required = ["tickets_per_month", "baseline_median_hours", "ratio", "defect_delta", "hours_per_defect", "loaded_rate", "agent_spend_per_month"];

    static void Price(string path)
    {
        var lines = File.ReadAllLines(path);
        var kv = KeyValues(lines);
        string offer = kv.GetValueOrDefault("offer", "?"), client = kv.GetValueOrDefault("client", "?");
        string cur = kv.GetValueOrDefault("currency", "");
        double fee = kv.TryGetValue("price", out var ps) && FirstNumber(ps) is double pv ? pv : double.NaN;
        int months = kv.TryGetValue("horizon months", out var hs) && FirstNumber(hs) is double hv ? (int)hv : 12;
        int seed = kv.TryGetValue("seed", out var ss) && FirstNumber(ss) is double sv ? (int)sv : 20;
        string scope = kv.GetValueOrDefault("effect scope", "").ToLowerInvariant();
        Console.WriteLine($"{Path.GetFileName(path)}: {offer} for {client}, fee {Money(fee)} {cur}, horizon {months} months");

        if (double.IsNaN(fee)) Error("no 'Price:' line with a number");
        if (cur == "") Warn("no 'Currency:' line");

        var t = Tables(lines).FirstOrDefault(x => x.head.Count > 0 && x.head[0].StartsWith("input") && x.head.Any(h => h == "low"));
        if (t.head == null) { Error("no inputs table (columns: input | low | high | unit | source)"); return; }
        int C(string n) => t.head.FindIndex(h => h.StartsWith(n));
        var ins = new Dictionary<string, In>();
        foreach (var r in t.rows)
        {
            string G(string n) => C(n) >= 0 && C(n) < r.Count ? Dash(r[C(n)]) : "";
            var name = G("input").ToLowerInvariant();
            if (!double.TryParse(G("low").Replace(",", ""), NumberStyles.Float, Inv, out var lo) ||
                !double.TryParse(G("high").Replace(",", ""), NumberStyles.Float, Inv, out var hi))
            { Error($"input '{name}': low/high must be numbers"); continue; }
            ins[name] = new In(name, lo, hi, G("unit"), G("source"));
        }

        Console.WriteLine();
        Console.WriteLine($"  {"input",-24}{"low",10}{"high",10}  source");
        foreach (var x in ins.Values)
            Console.WriteLine($"  {x.Name,-24}{Num(x.Lo),10}{Num(x.Hi),10}  {Clip(x.Source, 60)}");
        Console.WriteLine();

        foreach (var req in Required)
            if (!ins.ContainsKey(req))
                Error(req == "defect_delta" ? "no defect_delta: the quality guardrail's cost is missing (13.6)" : $"missing input '{req}'");
        if (!ins.ContainsKey("adoption_share"))
        {
            Warn("no adoption_share: assumes every ticket uses the method from day one (see Module 19)");
            ins["adoption_share"] = new In("adoption_share", 1, 1, "share", "assumed");
        }
        foreach (var x in ins.Values)
        {
            if (x.Lo > x.Hi) Error($"{x.Name}: low > high");
            if (x.Source == "") Warn($"{x.Name}: no source");
        }
        if (ins.TryGetValue("ratio", out var ratio))
        {
            if (ratio.Lo == ratio.Hi) Error("ratio: a point estimate; use the interval from your experiment report (13.6)");
            if (!Regex.IsMatch(ratio.Source, @"\bEXP-\d+", I)) Error("ratio: source is not one of your own experiments (EXP-nn); ROI may cite only your measured numbers");
            if (ratio.Lo <= 0 || ratio.Hi > 2) Error("ratio: expected a ratio of times (e.g. 0.75 to 0.95), not a percentage");
            if (scope.StartsWith("other") && ratio.Hi < 1)
                Warn("effect measured on another team: this prospect's effect may be zero; plan with ratio high >= 1.0 or sell the measurement first");
        }
        if (ins.TryGetValue("defect_delta", out var dd) && !Regex.IsMatch(dd.Source, @"\bEXP-\d+", I))
            Warn("defect_delta: source is not one of your own experiments");
        if (ins["adoption_share"].Hi > 1 || ins["adoption_share"].Lo < 0) Error("adoption_share must be between 0 and 1");
        if (scope.StartsWith("other") && Regex.IsMatch(offer, @"implementation|retainer|rollout", I))
            Warn("pricing an implementation on another team's effect; the pilot exists to measure it on theirs");

        var prose = string.Join("\n", lines.Where(l => !l.TrimStart().StartsWith('|')));
        Promises(prose, "worksheet text");
        foreach (var s in Sentences(prose))
            if (Regex.IsMatch(s, @"\bROI\b[^.]*?\b\d+(\.\d+)?\s?(%|x)", I) && !Regex.IsMatch(s, @"\bto\b|–|interval|range|between", I))
                Error($"worksheet text: single-number ROI: \"{Clip(s, 90)}\"");
        if (Required.Any(r => !ins.ContainsKey(r))) { Console.WriteLine("  (value range not computed: inputs missing)"); return; }

        var order = new[] { "tickets_per_month", "baseline_median_hours", "ratio", "defect_delta", "hours_per_defect", "loaded_rate", "agent_spend_per_month", "adoption_share" };
        double Net(double[] v) => v[0] * v[7] * v[1] * (1 - v[2]) * v[5] - v[0] * v[7] * v[3] * v[4] * v[5] - v[6];

        // Corners: every input at an end of its range.
        double cmin = double.MaxValue, cmax = double.MinValue;
        for (int mask = 0; mask < 1 << order.Length; mask++)
        {
            var v = order.Select((n, k) => (mask >> k & 1) == 1 ? ins[n].Hi : ins[n].Lo).ToArray();
            var net = Net(v); cmin = Math.Min(cmin, net); cmax = Math.Max(cmax, net);
        }
        // Monte Carlo (JCGM 101 style): interval inputs as their sampling distributions, planning ranges as uniform.
        var rng = new Random(seed); const int N = 20000;
        var draws = new double[N];
        double Normal() { double u1 = 1 - rng.NextDouble(), u2 = rng.NextDouble(); return Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2); }
        for (int d = 0; d < N; d++)
        {
            var v = new double[order.Length];
            for (int k = 0; k < order.Length; k++)
            {
                var x = ins[order[k]];
                if (x.Lo == x.Hi) v[k] = x.Lo;
                else if (order[k] == "ratio") { double a = Math.Log(x.Lo), b = Math.Log(x.Hi); v[k] = Math.Exp((a + b) / 2 + (b - a) / 3.92 * Normal()); }
                else if (order[k] == "defect_delta") v[k] = (x.Lo + x.Hi) / 2 + (x.Hi - x.Lo) / 3.92 * Normal();
                else v[k] = x.Lo + (x.Hi - x.Lo) * rng.NextDouble();
            }
            draws[d] = Net(v);
        }
        Array.Sort(draws);
        double Q(double p) => draws[Math.Clamp((int)Math.Round(p * (N - 1)), 0, N - 1)];
        double pNeg = draws.Count(x => x < 0) / (double)N;

        Console.WriteLine("Monthly net value to the client (before your fee)");
        Console.WriteLine($"  corners, every input at its worst or best:  {Money(cmin)} to {Money(cmax)}");
        Console.WriteLine($"  Monte Carlo 90% interval ({N:N0} draws, seed {seed}): {Money(Q(0.05))} to {Money(Q(0.95))} (median {Money(Q(0.5))})");
        Console.WriteLine($"  P(monthly net < 0): {pNeg:0%}");

        if (!double.IsNaN(fee) && fee > 0)
        {
            var roi = draws.Select(x => (x * months - fee) / fee).ToArray();
            double pLoss = roi.Count(x => x < 0) / (double)N;
            double R(double p) => roi[Math.Clamp((int)Math.Round(p * (N - 1)), 0, N - 1)];
            Console.WriteLine($"Over {months} months, after the fee of {Money(fee)}:");
            Console.WriteLine($"  net 90% interval: {Money(Q(0.05) * months - fee)} to {Money(Q(0.95) * months - fee)} (median {Money(Q(0.5) * months - fee)})");
            Console.WriteLine($"  ROI 90% interval: {R(0.05):+0%;-0%} to {R(0.95):+0%;-0%} (median {R(0.5):+0%;-0%})");
            Console.WriteLine($"  P(the client loses money over the horizon): {pLoss:0%}");
            Console.WriteLine(Q(0.5) > 0 && fee / Q(0.5) <= 10 * months ? $"  payback at the median: {fee / Q(0.5):0.0} months" : "  payback at the median: not within ten horizons (median monthly net near or below 0)");
            if (Q(0.5) * months > fee)
                Console.WriteLine($"  fee as a share of the median {months}-month value: {(fee / (Q(0.5) * months)).ToString("0%", Inv)}");
            if (Q(0.5) * months <= fee && Regex.IsMatch(offer, @"pilot|workshop|measure", I))
                Console.WriteLine("  note: the median value does not cover the fee; this rung is priced as a measurement, and the proposal must say it may not pay back on its own");
            else if (Q(0.5) * months <= fee)
                Warn("the median value does not cover the fee; cut scope or price, or sell this rung as a measurement");
            if (pLoss > 0.4 && !Regex.IsMatch(offer, @"pilot|workshop|measure", I))
                Warn($"{pLoss:0%} chance the client loses money; say so in the proposal or change the offer");
        }

    }

    // ---------------------------------------------------------------- qualify (20.3)

    static readonly string[] PersonalData = ["phone", "mobile", "address", "birth", "dob", "age", "gender", "salary", "personal_email", "photo", "passport", "id_number", "national_id", "marital", "religion"];

    static void Qualify(string path)
    {
        var lines = File.ReadAllLines(path).Where(l => l.Trim().Length > 0 && !l.StartsWith('#')).ToList();
        if (lines.Count < 2) throw new UsageException("applications file has no rows");
        var head = Split(lines[0]).Select(h => h.Trim().ToLowerInvariant()).ToList();
        int C(string n) => head.IndexOf(n);
        foreach (var need in new[] { "id", "problem", "last_incident", "sponsor", "budget", "timing_weeks", "uses_agent", "expectation", "conflict" })
            if (C(need) < 0) Error($"missing column '{need}'");
        foreach (var h in head)
            if (PersonalData.Any(p => p.Contains('_') ? h.Contains(p) : h.Split('_', ' ', '-').Contains(p)))
                Error($"column '{h}': personal data a qualification form does not need (collect only what the decision uses)");
        if (errors > 0 && head.IndexOf("id") < 0) return;

        Console.WriteLine($"{Path.GetFileName(path)}: {lines.Count - 1} applications");
        Console.WriteLine();
        Console.WriteLine($"  {"id",-6}{"score",-7}{"decision",-10}reasons");
        var tally = new Dictionary<string, int>();
        foreach (var line in lines.Skip(1))
        {
            var r = Split(line);
            string G(string n) => C(n) >= 0 && C(n) < r.Count ? r[C(n)].Trim() : "";
            var reasons = new List<string>();
            int score = 0;
            if (G("last_incident").Length >= 15 && !Regex.IsMatch(G("last_incident"), @"^(n/?a|none|-)$", I)) score++; else reasons.Add("no specific past incident");
            if (G("sponsor") != "" && !Regex.IsMatch(G("sponsor"), @"^(unknown|none|n/?a|-|tbd)$", I)) score++; else reasons.Add("no sponsor");
            var budget = G("budget").ToLowerInvariant();
            if (budget is "approved" or "process-known") score++; else reasons.Add($"budget {(budget == "" ? "unknown" : budget)}");
            if (double.TryParse(G("timing_weeks"), NumberStyles.Float, Inv, out var wk) && wk <= 12) score++; else reasons.Add("no start within 12 weeks");
            if (Regex.IsMatch(G("uses_agent"), @"^(yes|y|true|daily|weekly)$", I)) score++; else reasons.Add("team does not use an agent yet");
            bool expectFlag = Regex.IsMatch(G("expectation"), @"guarante|\d+\s?%|\d+\s?x\b|replace|headcount|fewer developers|cut (staff|devs|developers)|double", I);
            if (expectFlag) reasons.Add("expectation needs resetting: " + Clip(G("expectation"), 40));
            bool conflict = Regex.IsMatch(G("conflict"), @"^(yes|y|true)", I);

            string decision = conflict ? "REFER" : score >= 4 ? "CALL" : score >= 2 ? "NURTURE" : "DECLINE";
            if (conflict) reasons.Insert(0, "conflict of interest: " + Clip(G("conflict"), 40));
            tally[decision] = tally.GetValueOrDefault(decision) + 1;
            Console.WriteLine($"  {G("id"),-6}{score + "/5",-7}{decision,-10}{(reasons.Count == 0 ? "fit" : string.Join("; ", reasons))}");
        }
        Console.WriteLine();
        Console.WriteLine("  " + string.Join(", ", tally.OrderBy(k => k.Key).Select(k => $"{k.Key} {k.Value}")));
        if (!tally.ContainsKey("DECLINE") && !tally.ContainsKey("NURTURE") && !tally.ContainsKey("REFER") && lines.Count - 1 >= 4)
            Warn("every application gets a call; a form that turns nobody away is not qualifying anyone");
    }

    // ---------------------------------------------------------------- call (20.3)

    static void Call(string path)
    {
        var turns = new List<(int sec, string who, string text)>();
        foreach (var l in File.ReadAllLines(path))
        {
            var m = Regex.Match(l, @"^\s*\[(\d+):(\d\d)\]\s*(Me|Them)\s*:\s*(.*)$", I);
            if (m.Success) turns.Add((int.Parse(m.Groups[1].Value, Inv) * 60 + int.Parse(m.Groups[2].Value, Inv), m.Groups[3].Value.ToLowerInvariant(), m.Groups[4].Value));
        }
        if (turns.Count == 0) throw new UsageException("no transcript lines like '[05:10] Me: ...' or '[05:30] Them: ...'");
        var me = turns.Where(t => t.who == "me").ToList();
        int W(string s) => Regex.Matches(s, @"\w+").Count;
        int wMe = me.Sum(t => W(t.text)), wAll = turns.Sum(t => W(t.text));
        int end = turns.Max(t => t.sec);
        double share = wAll == 0 ? 0 : wMe / (double)wAll;
        Console.WriteLine($"{Path.GetFileName(path)}: {turns.Count} turns, {end / 60}:{end % 60:00} long, you spoke {share:0%} of the words");
        if (share > 0.60) Error($"you spoke {share:0%} of the words; discovery is their call, aim for under 45%");
        else if (share > 0.45) Warn($"you spoke {share:0%} of the words; aim for under 45%");

        var pitch = me.FirstOrDefault(t => Regex.IsMatch(t.text, @"\b(my|our) (workshop|offer|package|pilot|program|programme|service|engagement)\b|\bpric(e|ing)\b|\bproposal\b|\bcosts? \$", I));
        if (pitch.text != null)
        {
            Console.WriteLine($"  first mention of your offer or price: {Clock(pitch.sec)}");
            if (pitch.sec < 0.6 * end) Warn($"offer or price at {Clock(pitch.sec)}, before the last 40% of the call; understand the problem first");
        }
        else Console.WriteLine("  no mention of your offer or price");

        var topics = new (string name, string pattern)[]
        {
            ("a specific past event", @"last time|walk me through|tell me about the last|what happened (when|with|on|in)|most recent"),
            ("who decides and signs", @"\bdecid|sign(s|ed)? off|\bapprove|\bsign the\b|who else (needs|would|is involved)"),
            ("budget or how money gets spent", @"\bbudget|how (do|does|would) (you|your company) (pay|buy|spend|fund)|purchase order|procurement"),
            ("timing", @"\bby when\b|\bwhen (do|would|does|should)\b|timeline|deadline|what happens if (you|nothing)"),
            ("how success would be measured", @"how (will|would) you know|what would (success|good) look|measure|what number"),
        };
        foreach (var (name, pattern) in topics)
        {
            var hit = me.FirstOrDefault(t => (t.text.Contains('?') || Regex.IsMatch(t.text, @"\b(tell|walk) me\b", I)) && Regex.IsMatch(t.text, pattern, I));
            if (hit.text == null) Warn($"no question about {name}");
            else Console.WriteLine($"  asked about {name} at {Clock(hit.sec)}");
        }
        foreach (var t in me)
        {
            if (Regex.IsMatch(t.text, @"^\s*(don't you|wouldn't you|isn't (it|that)|aren't you|wouldn't it|surely)\b", I))
                Warn($"{Clock(t.sec)} leading question: \"{Clip(t.text, 80)}\"");
            if (Regex.IsMatch(t.text, @"how much would you (pay|spend)|would you (buy|pay|sign)", I))
                Warn($"{Clock(t.sec)} hypothetical question: \"{Clip(t.text, 80)}\"");
            Pressure(t.text, Clock(t.sec));
            Promises(t.text, Clock(t.sec));
        }
        var honest = me.FirstOrDefault(t => Regex.IsMatch(t.text, @"not (a|the right) fit|(can't|cannot|won't|will not) promise|don't know|wouldn't recommend|would not recommend|don't need (me|us|this)|someone else|refer you|isn't (something|a problem) (I|we)|I can't tell you", I));
        if (honest.text == null) Warn("no limit or disqualifier said aloud (what you cannot promise, or when they do not need you)");
        else Console.WriteLine($"  honest limit said at {Clock(honest.sec)}: \"{Clip(honest.text, 70)}\"");
        var nextStep = me.Where(t => t.sec >= 0.8 * end).FirstOrDefault(t =>
            Regex.IsMatch(t.text, @"\d{4}-\d{2}-\d{2}|\b(monday|tuesday|wednesday|thursday|friday)\b|\b\d{1,2} (jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)", I));
        if (nextStep.text == null) Warn("no dated next step in the last 20% of the call");
        else Console.WriteLine($"  next step at {Clock(nextStep.sec)}: \"{Clip(nextStep.text, 70)}\"");
    }

    // ---------------------------------------------------------------- proposal (20.4)

    static void Proposal(string path)
    {
        var lines = File.ReadAllLines(path);
        var text = string.Join("\n", lines);
        var sec = Sections(lines);
        Console.WriteLine($"{Path.GetFileName(path)}: {sec.Count} sections");
        string? Find(params string[] keys) => sec.Keys.FirstOrDefault(k => keys.Any(x => k.Contains(x)));
        var required = new (string label, string[] keys)[]
        {
            ("situation in the client's words", ["situation", "context", "what we heard"]),
            ("objectives", ["objective", "outcome", "goal"]),
            ("options", ["option"]),
            ("evidence", ["evidence"]),
            ("what is not promised", ["not promise", "limits", "what this does not"]),
            ("investment", ["investment", "fee", "price"]),
            ("next steps", ["next step"]),
        };
        foreach (var (label, keys) in required)
            if (Find(keys) is string k) Console.WriteLine($"  ok   {label} ({k})");
            else Error($"missing section: {label}");

        if (Find("situation", "context", "what we heard") is string s && !Regex.IsMatch(sec[s], "[\"“”]"))
            Warn("situation has no quoted words from the discovery call; write it in their words, not yours");
        if (Find("option") is string o)
        {
            var t = Tables(sec[o].Split('\n')).FirstOrDefault();
            if (t.head == null) Warn("options: no table (option | scope | duration | price)");
            else
            {
                if (t.rows.Count < 2 || t.rows.Count > 3) Warn($"options: {t.rows.Count} option(s); give 2 or 3 real ones, including the smallest useful one");
                int pc = t.head.FindIndex(h => h.StartsWith("price") || h.StartsWith("fee") || h.StartsWith("investment"));
                foreach (var r in t.rows)
                    if (pc < 0 || pc >= r.Count || !Regex.IsMatch(r[pc], @"\d")) Error($"option '{Clip(r[0], 30)}' has no price");
            }
        }
        if (Find("evidence") is string e && !Regex.IsMatch(sec[e], @"\b[A-Z]{2,}-[A-Z]?\d+\b"))
            Error("evidence names no experiment or case ID; say which measurement each claim rests on");
        if (Find("not promise", "limits", "what this does not") is string np && sec[np].Split('\n').Count(l => l.TrimStart().StartsWith("- ")) < 2)
            Warn("what is not promised: fewer than 2 items");
        if (Find("next step") is string ns && !Regex.IsMatch(sec[ns], @"\d{4}-\d{2}-\d{2}"))
            Warn("next steps have no date");
        Promises(text, "proposal");
        Pressure(text, "proposal");
        foreach (var x in Sentences(text))
            if (Regex.IsMatch(x, @"\bROI\b[^.]*?\b\d+(\.\d+)?\s?(%|x)", I) && !Regex.IsMatch(x, @"\bto\b|–|interval|range|between", I))
                Error($"proposal: single-number ROI: \"{Clip(x, 90)}\"");
    }

    // ---------------------------------------------------------------- sow (20.4)

    static void Sow(string path)
    {
        var lines = File.ReadAllLines(path);
        var text = string.Join("\n", lines);
        var sec = Sections(lines);
        var kv = KeyValues(lines);
        Console.WriteLine($"{Path.GetFileName(path)}: {sec.Count} sections");
        string? Find(Func<string, bool> f) => sec.Keys.FirstOrDefault(f);
        var required = new (string label, Func<string, bool> match)[]
        {
            ("objectives", k => k.Contains("objective") || k.Contains("background") || k.Contains("purpose")),
            ("scope", k => k.Contains("scope") && !k.Contains("out of") && !k.Contains("exclu")),
            ("deliverables", k => k.Contains("deliverable")),
            ("out of scope", k => k.Contains("out of scope") || k.Contains("exclusion")),
            ("assumptions", k => k.Contains("assumption")),
            ("client responsibilities", k => k.Contains("responsibilit") || k.Contains("dependenc")),
            ("schedule", k => k.Contains("schedule") || k.Contains("milestone") || k.Contains("timeline")),
            ("fees and payment", k => k.Contains("fee") || k.Contains("payment")),
            ("change control", k => k.Contains("change")),
            ("measurement", k => k.Contains("measure")),
            ("acceptance", k => k.Contains("acceptance")),
            ("intellectual property", k => k.Contains("intellectual") || k == "ip" || k.StartsWith("ip ") || k.Contains("ownership")),
            ("confidentiality and data", k => k.Contains("confidential") || k.Contains("data")),
            ("term and termination", k => k.Contains("terminat")),
        };
        var found = new Dictionary<string, string>();
        foreach (var (label, match) in required)
            if (Find(match) is string k) found[label] = k;
            else Error($"missing section: {label}");
        Console.WriteLine($"  sections present: {found.Count} of {required.Length}");

        if (found.TryGetValue("deliverables", out var dk))
        {
            var t = Tables(sec[dk].Split('\n')).FirstOrDefault(x => x.head.Any(h => h.StartsWith("accept")));
            if (t.head == null) Error("deliverables: no table with an acceptance-criterion column");
            else
            {
                int ac = t.head.FindIndex(h => h.StartsWith("accept"));
                Console.WriteLine($"  deliverables: {t.rows.Count}");
                foreach (var r in t.rows)
                {
                    var a = ac < r.Count ? Dash(r[ac]) : "";
                    if (a == "") Error($"deliverable '{Clip(r[0], 40)}': no acceptance criterion");
                    else if (Regex.IsMatch(a, @"satisf|to the client'?s|as agreed|\btbd\b|good quality|high quality|best practice|fit for purpose|^(works|done|complete|delivered).?$|^approved( by (the )?client)?\.?$", I))
                        Error($"deliverable '{Clip(r[0], 40)}': acceptance '{Clip(a, 40)}' cannot be checked by a third person");
                }
            }
        }
        foreach (var label in new[] { "scope", "deliverables" })
            if (found.TryGetValue(label, out var k))
                foreach (Match m in Regex.Matches(sec[k], @"\betc\b|as needed|as required|unlimited|ongoing support|and more\b|any other|including but not limited to|whatever (is|it takes)|full support", I))
                    Warn($"{label}: open-ended wording '{m.Value}'");
        if (found.TryGetValue("out of scope", out var ok))
        {
            int n = sec[ok].Split('\n').Count(l => Regex.IsMatch(l, @"^\s*[-*]\s+\S"));
            Console.WriteLine($"  exclusions: {n}");
            if (n == 0) Error("out of scope: no items");
            else if (n < 3) Warn($"out of scope: only {n} item(s); list the things the client is most likely to assume are included");
        }
        if (found.TryGetValue("change control", out var ck))
        {
            var c = sec[ck];
            if (!Regex.IsMatch(c, @"writ", I)) Warn("change control: does not require changes in writing");
            if (!Regex.IsMatch(c, @"fee|price|cost", I) || !Regex.IsMatch(c, @"schedule|date|timeline", I))
                Warn("change control: a change request should state its effect on fee and schedule");
        }
        if (found.TryGetValue("fees and payment", out var fk))
        {
            var f = sec[fk];
            var terms = Regex.Match(f, @"net\s?(\d+)|within (\d+) (calendar |business )?days", I);
            if (!terms.Success) Error("fees: no payment terms (e.g. 'net 30')");
            else
            {
                int d = int.Parse(terms.Groups[1].Success ? terms.Groups[1].Value : terms.Groups[2].Value, Inv);
                Console.WriteLine($"  payment terms: {d} days");
                if (d > 60) Warn($"payment terms of {d} days; longer than 60 is worth negotiating (and in the EU, B2B terms over 60 days must be expressly agreed)");
            }
            var t = Tables(f.Split('\n')).FirstOrDefault(x => x.head.Any(h => h.StartsWith("amount")));
            var total = Regex.Match(f, @"total[^\d\n]*([\d,]+(\.\d+)?)", I);
            if (t.head != null && total.Success)
            {
                int ac = t.head.FindIndex(h => h.StartsWith("amount"));
                double sum = t.rows.Sum(r => ac < r.Count && FirstNumber(r[ac]) is double v ? v : 0);
                double tot = double.Parse(total.Groups[1].Value.Replace(",", ""), Inv);
                Console.WriteLine($"  payment schedule sums to {Money(sum)}; stated total {Money(tot)}");
                if (Math.Abs(sum - tot) > 0.5) Error($"fees: payment schedule sums to {Money(sum)} but the total says {Money(tot)}");
            }
            else Warn("fees: no payment schedule table with an Amount column and a Total line");
        }
        if (found.TryGetValue("measurement", out var mk))
        {
            var m = sec[mk];
            if (Regex.IsMatch(m, @"%") && !Regex.IsMatch(m, @"interval|\bCI\b|range", I))
                Error("measurement: a percentage with no interval; measure and report, do not promise a number");
            if (!Regex.IsMatch(m, @"baseline", I)) Warn("measurement: no baseline mentioned");
        }
        if (found.TryGetValue("intellectual property", out var ik) && !Regex.IsMatch(sec[ik], @"pre-existing|background|prior", I))
            Warn("IP: says nothing about your pre-existing material (method, templates, tools)");
        if (Regex.IsMatch(text, @"production", I) && !Regex.IsMatch(text, @"no (write |direct )?access to production|no production access|not (have|get|require) (any )?(write )?access to production|never .{0,30}production|production .{0,40}(out of scope|excluded)", I))
            Warn("mentions production without stating that you have no production access");
        Promises(text, "sow");
        Pressure(text, "sow");
        if (!kv.ContainsKey("version") && !Regex.IsMatch(text, @"version\s*[:\d]", I)) Warn("no version line; SOW changes need a version history");
    }

    // ---------------------------------------------------------------- changes (20.4, 20.5)

    static void Changes(string path)
    {
        var lines = File.ReadAllLines(path);
        var t = Tables(lines).FirstOrDefault(x => x.head.Any(h => h.StartsWith("request")) && x.head.Any(h => h.StartsWith("decision")));
        if (t.head == null) { Error("no table with Request and Decision columns (Date | Request | In SOW | Decision | CR | Impact)"); return; }
        int C(string n) => t.head.FindIndex(h => h.StartsWith(n));
        Console.WriteLine($"{Path.GetFileName(path)}: {t.rows.Count} scope requests");
        int inSow = 0, cr = 0, declined = 0, silent = 0;
        foreach (var r in t.rows)
        {
            string G(string n) => C(n) >= 0 && C(n) < r.Count ? Dash(r[C(n)]) : "";
            string req = Clip(G("request"), 45), dec = G("decision").ToLowerInvariant(), crid = G("cr"), inside = G("in sow").ToLowerInvariant();
            if (inside.StartsWith("y")) { inSow++; continue; }
            if (Regex.IsMatch(dec, @"declin|refus|defer|later|backlog|next phase")) { declined++; continue; }
            if (Regex.IsMatch(crid, @"verbal|call|chat|slack", I)) { Error($"'{req}': agreed verbally only; confirm every change in writing"); silent++; continue; }
            if (crid == "" && Regex.IsMatch(dec, @"done|did|yes|absorb|agreed|accepted|delivered"))
            { Error($"'{req}': outside the SOW and done without a change request (silent scope creep)"); silent++; continue; }
            if (crid != "")
            {
                cr++;
                if (!Regex.IsMatch(G("impact"), @"\d|none|no (fee|cost|change)", I)) Warn($"'{req}': {crid} states no effect on fee or schedule");
            }
            else Warn($"'{req}': outside the SOW with no decision recorded");
        }
        Console.WriteLine($"  in SOW {inSow}, change requests {cr}, declined or deferred {declined}, silent {silent}");
    }

    // ---------------------------------------------------------------- admin (20.5)

    static readonly (string id, string label, bool blocker)[] AdminItems =
    [
        ("E1", "employment contract: IP assignment clause read", true),
        ("E2", "employment contract: outside-work, non-compete and confidentiality clauses read", true),
        ("E3", "employer's written permission (or written confirmation none is needed)", true),
        ("E4", "no employer time, equipment, accounts, code or data used", true),
        ("E5", "client is not the employer's customer, competitor or supplier", true),
        ("C1", "signed contract and SOW before any work starts", true),
        ("C2", "IP: background IP retained; deliverables licensed or assigned on payment", true),
        ("C3", "liability capped (e.g. at fees paid)", false),
        ("C4", "confidentiality and data-processing terms agreed", false),
        ("C5", "payment terms agreed and in the SOW", false),
        ("T1", "tax registration and status checked for your jurisdiction", false),
        ("T2", "worker-status / contractor rules checked for the client's jurisdiction", false),
        ("T3", "insurance (professional indemnity) considered", false),
        ("I1", "invoice template has every legally required field for your jurisdiction", false),
    ];

    static void Admin(string path)
    {
        var lines = File.ReadAllLines(path);
        var kv = KeyValues(lines);
        Console.WriteLine($"{Path.GetFileName(path)}: engagement admin register");
        Console.WriteLine("  (orientation only: the tool checks that each question was asked and answered in writing, not that the answer is right)");
        if (!kv.TryGetValue("jurisdiction", out var jur) || jur.Trim() == "") Warn("no 'Jurisdiction:' line; the rules differ by country and state");
        else Console.WriteLine($"  jurisdiction: {jur}");
        if (!kv.TryGetValue("reviewed by", out var rev) || Regex.IsMatch(rev, @"^(none|nobody|-|n/a|me|myself)$", I))
            Warn("no professional review recorded ('Reviewed by:' a lawyer or accountant in your jurisdiction)");

        var t = Tables(lines).FirstOrDefault(x => x.head.Count > 1 && x.head[0].StartsWith("item") && x.head.Any(h => h.StartsWith("status")));
        var status = new Dictionary<string, (string st, string ev)>();
        if (t.head == null) Error("no checklist table (Item | Status | Evidence)");
        else
        {
            int sc = t.head.FindIndex(h => h.StartsWith("status")), ec = t.head.FindIndex(h => h.StartsWith("evidence"));
            foreach (var r in t.rows)
            {
                var m = Regex.Match(r[0], @"^([A-Z]\d)\b");
                if (m.Success) status[m.Groups[1].Value] = (sc < r.Count ? r[sc].Trim().ToLowerInvariant() : "", ec >= 0 && ec < r.Count ? Dash(r[ec]) : "");
            }
        }
        Console.WriteLine();
        foreach (var (id, label, blocker) in AdminItems)
        {
            if (!status.TryGetValue(id, out var s)) { (blocker ? (Action<string>)Error : Warn)($"{id} missing: {label}"); continue; }
            bool yes = s.st.StartsWith("yes") || s.st.StartsWith("done"), na = s.st is "n/a" or "na";
            Console.WriteLine($"  {id} {(yes ? "yes " : na ? "n/a " : "NO  ")} {label}");
            if (!yes && !na) (blocker ? (Action<string>)Error : Warn)($"{id} {(blocker ? "blocker" : "open")}: {label}");
            else if (yes && s.ev == "") Warn($"{id}: marked yes with no evidence (a document, a date, an email)");
        }

        // Invoices
        var inv = Tables(lines).FirstOrDefault(x => x.head.Any(h => h.StartsWith("invoice")) && x.head.Any(h => h.StartsWith("due")));
        if (inv.head != null)
        {
            int C(string n) => inv.head.FindIndex(h => h.StartsWith(n));
            int terms = kv.TryGetValue("payment terms", out var pt) && FirstNumber(pt) is double d ? (int)d : 30;
            Console.WriteLine();
            Console.WriteLine($"  invoices: {inv.rows.Count}, payment terms net {terms}");
            int? prev = null;
            foreach (var r in inv.rows)
            {
                string G(string n) => C(n) >= 0 && C(n) < r.Count ? Dash(r[C(n)]) : "";
                var num = Regex.Match(G("invoice"), @"(\d+)\s*$");
                if (num.Success)
                {
                    int n = int.Parse(num.Groups[1].Value, Inv);
                    if (prev is int p && n != p + 1) Error($"invoice {G("invoice")}: numbering not sequential after {p}");
                    prev = n;
                }
                if (DateTime.TryParse(G("date"), Inv, DateTimeStyles.None, out var dt) && DateTime.TryParse(G("due"), Inv, DateTimeStyles.None, out var due)
                    && (due - dt).TotalDays > terms)
                    Error($"invoice {G("invoice")}: due {(due - dt).TotalDays:0} days after issue; the contract says net {terms}");
                if (C("milestone") >= 0 && G("milestone") == "") Warn($"invoice {G("invoice")}: no SOW milestone referenced");
                if (C("tax") >= 0 && G("tax") == "") Warn($"invoice {G("invoice")}: tax column empty (state the tax or why none applies)");
            }
        }
    }

    // ---------------------------------------------------------------- helpers

    static Dictionary<string, string> KeyValues(string[] lines)
    {
        var d = new Dictionary<string, string>();
        bool fence = false;
        foreach (var l in lines)
        {
            if (l.TrimStart().StartsWith("```")) { fence = !fence; continue; }
            if (fence || l.TrimStart().StartsWith('|') || l.TrimStart().StartsWith('#')) continue;
            var m = Regex.Match(l, @"^\s*[-*]?\s*\**([A-Za-z][A-Za-z /()_-]{1,30}?)\**:\s*(.+)$");
            if (m.Success) { var k = m.Groups[1].Value.Trim().ToLowerInvariant(); if (!d.ContainsKey(k)) d[k] = m.Groups[2].Value.Trim(); }
        }
        return d;
    }

    static Dictionary<string, string> Sections(string[] lines)
    {
        var d = new Dictionary<string, string>();
        string? cur = null; var sb = new System.Text.StringBuilder();
        foreach (var l in lines)
        {
            var m = Regex.Match(l, @"^##\s+(?:\d+[.)]?\s*)?(.+?)\s*$");
            if (m.Success && !l.StartsWith("###"))
            {
                if (cur != null) d[cur] = sb.ToString();
                cur = m.Groups[1].Value.ToLowerInvariant(); sb.Clear();
            }
            else if (cur != null) sb.AppendLine(l);
        }
        if (cur != null) d[cur] = sb.ToString();
        return d;
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

    static double? FirstNumber(string s)
    {
        var m = Regex.Match(s, @"(\d[\d,]*(?:\.\d+)?)\s?(k\b)?", I);
        if (!m.Success) return null;
        var v = double.Parse(m.Groups[1].Value.Replace(",", ""), Inv);
        return m.Groups[2].Success ? v * 1000 : v;
    }

    static string Money(double v) => double.IsNaN(v) ? "?" : (v < 0 ? "-" : "") + Math.Abs(Math.Round(v, 0)).ToString("#,0", Inv);
    static string Num(double v) => Math.Abs(v) >= 100 ? v.ToString("#,0", Inv) : v.ToString("0.###", Inv);
    static string Clock(int sec) => $"{sec / 60:00}:{sec % 60:00}";
    static string Dash(string s) => s.Trim() is "-" or "—" or "–" ? "" : s.Trim();
    static string Clip(string s, int n) => s.Length <= n ? s : s[..(n - 3)] + "...";
    static void Error(string m) { errors++; Console.WriteLine($"ERROR {m}"); }
    static void Warn(string m) { warnings++; Console.WriteLine($"WARN  {m}"); }
    static string NeedFile(string f) => File.Exists(f) ? f : throw new UsageException($"file not found: {f}");
}

sealed class UsageException(string message) : Exception(message);
