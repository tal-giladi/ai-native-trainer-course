// LearnCheck — the Module 16 tool. Dependency-free, read-only, deterministic.
//
//   align    <session.md>
//            Objectives, assessment items and activities in one lesson plan: measurable verbs,
//            every objective assessed at its level and practised, show/do/reflect balance,
//            passive stretches, new terms per step, time budget (16.2, 16.3).
//   gain     <responses.csv> [--plan <session.md>] [--seed 16]
//            Pre/post scores: matched learners, class normalized gain <g> (Hake) with a bootstrap
//            interval, mean normalized change c (Marx & Cummings), Cohen's d, per-item and
//            per-objective gains, ceiling items, repeated forms (16.1, 16.5).
//   feedback <feedback.csv> --responses <responses.csv>
//            Reaction (ratings) next to learning (gain): means, confidence vs actual score,
//            rating vs gain, comments (16.1, 16.5).
//   followup <followup.csv>
//            Behaviour follow-up: share of learners who used it at work, with a Wilson interval (16.5).
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
            var (pos, opt) = Parse(args.Skip(1).ToArray());
            switch (args[0])
            {
                case "align": Align(Need(pos, 0)); break;
                case "gain": Gain(Need(pos, 0), opt.GetValueOrDefault("plan"), int.Parse(opt.GetValueOrDefault("seed", "16"), Inv)); break;
                case "feedback": Feedback(Need(pos, 0), Opt(opt, "responses")); break;
                case "followup": Followup(Need(pos, 0)); break;
                default: Console.Error.WriteLine($"unknown command '{args[0]}'"); Usage(); return 2;
            }
        }
        catch (UsageException e) { Console.Error.WriteLine(e.Message); return 2; }
        catch (IOException e) { Console.Error.WriteLine(e.Message); return 2; }
        Console.WriteLine();
        Console.WriteLine($"{errors} error(s), {warnings} warning(s)");
        return errors > 0 ? 1 : 0;
    }

    // ---------------------------------------------------------------- align

    static readonly string[] Levels = ["remember", "understand", "apply", "analyze", "evaluate", "create"];
    static readonly Regex Vague = new(@"\b(understand|know|learn|appreciate|grasp|realize|realise|be aware|be familiar|get a feel)\b", RegexOptions.IgnoreCase);

    sealed record Objective(string Id, string Text, int Level);
    sealed record Item(string Id, string Objective, int Level);
    sealed record Step(string Name, double Minutes, string Mode, string[] Objectives, int NewTerms);
    sealed record Plan(List<Objective> Objectives, List<Item> Items, List<Step> Steps, double? Budget);

    static int LevelOf(string s, string where)
    {
        var t = s.Trim().ToLowerInvariant().Replace("analyse", "analyze");
        int i = Array.IndexOf(Levels, t);
        if (i < 0) { Error($"{where}: level '{s}' is not one of {string.Join(", ", Levels)}"); return 0; }
        return i + 1;
    }

    static Plan ParsePlan(string path)
    {
        var lines = File.ReadAllLines(path);
        var objs = new List<Objective>(); var items = new List<Item>(); var steps = new List<Step>();
        double? budget = null;
        var b = Regex.Match(string.Join("\n", lines), @"^[-*\s]*Budget:\s*(\d+)", RegexOptions.Multiline);
        if (b.Success) budget = double.Parse(b.Groups[1].Value, Inv);
        for (int i = 0; i < lines.Length; i++)
        {
            if (!lines[i].TrimStart().StartsWith('|')) continue;
            var head = Cells(lines[i]).Select(c => c.ToLowerInvariant()).ToList();
            int j = i + 2; // skip header and separator
            var rows = new List<List<string>>();
            while (j < lines.Length && lines[j].TrimStart().StartsWith('|')) rows.Add(Cells(lines[j++]));
            int Col(string name) => head.FindIndex(h => h.StartsWith(name));
            string Get(List<string> r, int c) => c >= 0 && c < r.Count ? r[c] : "";
            if (Col("minutes") >= 0 && Col("mode") >= 0)
                foreach (var r in rows)
                {
                    double.TryParse(Get(r, Col("minutes")), NumberStyles.Float, Inv, out var min);
                    int.TryParse(Get(r, Col("new")), out var nt);
                    var os = Get(r, Col("objective")).Split([',', ' ', ';'], StringSplitOptions.RemoveEmptyEntries)
                        .Where(o => Regex.IsMatch(o, @"^O\d+$")).ToArray();
                    steps.Add(new Step(Get(r, Col("step")), min, Get(r, Col("mode")).ToLowerInvariant(), os, nt));
                }
            else if (Col("item") >= 0 && Col("objective") >= 0 && Col("level") >= 0)
                foreach (var r in rows)
                    items.Add(new Item(Get(r, Col("item")), Get(r, Col("objective")), LevelOf(Get(r, Col("level")), $"item {Get(r, Col("item"))}")));
            else if (Col("id") >= 0 && Col("objective") >= 0 && Col("level") >= 0)
                foreach (var r in rows)
                    objs.Add(new Objective(Get(r, Col("id")), Get(r, Col("objective")), LevelOf(Get(r, Col("level")), $"objective {Get(r, Col("id"))}")));
            i = j - 1;
        }
        return new Plan(objs, items, steps, budget);
    }

    static void Align(string path)
    {
        var p = ParsePlan(path);
        Console.WriteLine($"{Path.GetFileName(path)}: {p.Objectives.Count} objectives, {p.Items.Count} assessment items, {p.Steps.Count} steps");
        if (p.Objectives.Count == 0) { Error("no objectives table (| ID | Objective | Level |)"); return; }

        Console.WriteLine();
        Console.WriteLine("Objectives");
        foreach (var o in p.Objectives)
        {
            var its = p.Items.Where(i => i.Objective == o.Id).ToList();
            var dos = p.Steps.Where(s => s.Mode == "do" && s.Objectives.Contains(o.Id)).ToList();
            int top = its.Count == 0 ? 0 : its.Max(i => i.Level);
            Console.WriteLine($"  {o.Id} [{Levels[Math.Max(o.Level, 1) - 1]}] items: {its.Count} (highest {(top == 0 ? "-" : Levels[top - 1])}), practice steps: {dos.Count}");
            var v = Vague.Match(o.Text);
            if (v.Success) Error($"{o.Id}: '{v.Value}' is not observable; say what the learner will do (decide, find, write, explain why)");
            if (its.Count == 0) Error($"{o.Id}: no assessment item measures this objective");
            else
            {
                if (top < o.Level) Error($"{o.Id}: assessed only up to '{Levels[top - 1]}' but the objective asks for '{Levels[o.Level - 1]}'");
                if (its.Count == 1) Warn($"{o.Id}: one item only; a single item cannot tell a slip from a misconception");
            }
            if (dos.Count == 0) Error($"{o.Id}: assessed but never practised (no 'do' step)");
        }
        foreach (var i in p.Items.Where(i => p.Objectives.All(o => o.Id != i.Objective)))
            Error($"item {i.Id}: maps to '{i.Objective}', which is not an objective");
        if (p.Objectives.Count > 3 && (p.Budget ?? 999) <= 45)
            Warn($"{p.Objectives.Count} objectives in {p.Budget} minutes; two or three is the usual ceiling for a short session");

        if (p.Steps.Count == 0) { Warn("no activities table (| Step | Minutes | Mode | Objective |)"); return; }
        Console.WriteLine();
        Console.WriteLine("Time");
        double total = p.Steps.Sum(s => s.Minutes);
        double teach = p.Steps.Where(s => s.Mode != "assess").Sum(s => s.Minutes);
        double M(string mode) => p.Steps.Where(s => s.Mode == mode).Sum(s => s.Minutes);
        double active = M("do") + M("reflect");
        Console.WriteLine($"  total {total:0} min{(p.Budget is { } bb ? $" (budget {bb:0})" : "")}; assess {M("assess"):0}, show {M("show"):0}, do {M("do"):0}, reflect {M("reflect"):0}");
        Console.WriteLine($"  learners active (do + reflect): {Pct(active, teach)} of teaching time");
        if (p.Budget is { } budget && total > budget) Error($"plan takes {total:0} minutes, budget is {budget:0}");
        if (teach > 0 && active / teach < 0.4) Error($"learners are active for {Pct(active, teach)} of teaching time (minimum 40%)");
        if (M("reflect") == 0) Warn("no reflect step: nothing makes learners retrieve and say what they learned");
        if (p.Steps.Count > 0 && p.Steps[0].Mode != "assess") Warn("first step is not a pre-assessment");
        if (p.Steps.Count > 0 && p.Steps[^1].Mode != "assess") Warn("last step is not a post-assessment");

        double run = 0; string runStart = "";
        foreach (var s in p.Steps.Append(new Step("", 0, "end", [], 0)))
        {
            if (s.Mode == "show") { if (run == 0) runStart = s.Name; run += s.Minutes; continue; }
            if (run > 10) Error($"{run:0} minutes of passive watching from '{Clip(runStart, 40)}' (limit 10) before learners do anything");
            run = 0;
        }
        int terms = p.Steps.Sum(s => s.NewTerms);
        foreach (var s in p.Steps.Where(s => s.NewTerms > 3))
            Warn($"'{Clip(s.Name, 40)}' introduces {s.NewTerms} new terms in one step (limit 3): extraneous load for anyone without the schema");
        if (terms > 0) Console.WriteLine($"  new terms introduced: {terms}");
    }

    // ---------------------------------------------------------------- gain

    sealed record Sheet(string Learner, string Phase, string Form, double[] Scores);
    sealed record Pair(string Learner, double Pre, double Post, string PreForm, string PostForm);

    static (string[] items, List<Sheet> rows) ReadResponses(string path)
    {
        var lines = File.ReadAllLines(path).Where(l => l.Trim().Length > 0 && !l.StartsWith('#')).ToArray();
        var head = lines[0].Split(',').Select(h => h.Trim()).ToArray();
        int li = Array.FindIndex(head, h => h.Equals("learner", StringComparison.OrdinalIgnoreCase));
        int pi = Array.FindIndex(head, h => h.Equals("phase", StringComparison.OrdinalIgnoreCase));
        int fi = Array.FindIndex(head, h => h.Equals("form", StringComparison.OrdinalIgnoreCase));
        if (li < 0 || pi < 0) throw new UsageException($"{path}: header needs 'learner' and 'phase' columns");
        var itemCols = Enumerable.Range(0, head.Length).Where(i => i != li && i != pi && i != fi).ToArray();
        var rows = new List<Sheet>();
        var seen = new HashSet<string>();
        foreach (var l in lines.Skip(1))
        {
            var c = l.Split(',').Select(x => x.Trim()).ToArray();
            var phase = c[pi].ToLowerInvariant();
            if (phase is not ("pre" or "post")) { Error($"{c[li]}: phase '{c[pi]}' must be pre or post"); continue; }
            if (!seen.Add(c[li] + "|" + phase)) Error($"{c[li]}: more than one {phase} row");
            var s = itemCols.Select(i => double.Parse(c[i], Inv)).ToArray();
            if (s.Any(x => x < 0 || x > 1)) Error($"{c[li]} {phase}: item scores must be between 0 and 1");
            rows.Add(new Sheet(c[li], phase, fi >= 0 ? c[fi] : "", s));
        }
        return (itemCols.Select(i => head[i]).ToArray(), rows);
    }

    static List<Pair> Match(List<Sheet> rows, bool report)
    {
        var pre = rows.Where(r => r.Phase == "pre").ToDictionary(r => r.Learner);
        var post = rows.Where(r => r.Phase == "post").ToDictionary(r => r.Learner);
        if (report)
        {
            foreach (var l in pre.Keys.Except(post.Keys)) Warn($"{l}: pre-test only, excluded (left early?)");
            foreach (var l in post.Keys.Except(pre.Keys)) Warn($"{l}: post-test only, excluded (joined late?); counting it inflates the post average");
        }
        return pre.Keys.Intersect(post.Keys).OrderBy(k => k, StringComparer.Ordinal)
            .Select(k => new Pair(k, 100 * pre[k].Scores.Average(), 100 * post[k].Scores.Average(), pre[k].Form, post[k].Form)).ToList();
    }

    static double ClassG(IList<Pair> ps)
    {
        double pre = ps.Average(p => p.Pre), post = ps.Average(p => p.Post);
        return pre >= 100 ? double.NaN : (post - pre) / (100 - pre);
    }

    static double? IndividualC(Pair p)
    {
        if (p.Pre == p.Post) return (p.Pre is 0 or 100) ? null : 0;
        return p.Post > p.Pre ? (p.Post - p.Pre) / (100 - p.Pre) : (p.Post - p.Pre) / p.Pre;
    }

    static void Gain(string path, string? planPath, int seed)
    {
        var (items, rows) = ReadResponses(path);
        var ps = Match(rows, report: true);
        Console.WriteLine($"{Path.GetFileName(path)}: {items.Length} items, {ps.Count} matched learners");
        if (ps.Count == 0) { Error("no learner has both a pre and a post row"); return; }

        var same = ps.Where(p => p.PreForm != "" && p.PreForm == p.PostForm).Select(p => p.Learner).ToList();
        if (same.Count > 0) Error($"{same.Count} of {ps.Count} learners took the same form before and after ({string.Join(", ", same)}): the post-test measures memory of the pre-test");

        double pre = ps.Average(p => p.Pre), post = ps.Average(p => p.Post);
        double g = ClassG(ps);
        var rnd = new Random(seed);
        var boot = new List<double>();
        for (int b = 0; b < 10000; b++)
        {
            var sample = Enumerable.Range(0, ps.Count).Select(_ => ps[rnd.Next(ps.Count)]).ToList();
            var x = ClassG(sample);
            if (!double.IsNaN(x)) boot.Add(x);
        }
        boot.Sort();
        var cs = ps.Select(IndividualC).ToList();
        var valid = cs.Where(c => c.HasValue).Select(c => c!.Value).ToList();
        double sdPre = Sd(ps.Select(p => p.Pre)), sdPost = Sd(ps.Select(p => p.Post));
        double pooled = Math.Sqrt((sdPre * sdPre + sdPost * sdPost) / 2);

        Console.WriteLine();
        Console.WriteLine("Learners");
        foreach (var (p, c) in ps.Zip(cs))
            Console.WriteLine($"  {p.Learner,-6} pre {p.Pre,5:0.0}%  post {p.Post,5:0.0}%  c {(c.HasValue ? c.Value.ToString("0.00", Inv) : "n/a (pre = post at 0 or 100)")}");
        Console.WriteLine();
        Console.WriteLine("Class");
        Console.WriteLine($"  pre {pre:0.0}%  post {post:0.0}%  (raw gain {post - pre:0.0} points)");
        Console.WriteLine($"  normalized gain <g> = (post - pre) / (100 - pre) = {g:0.00}   95% bootstrap interval {Q(boot, 0.025):0.00} to {Q(boot, 0.975):0.00} (learners resampled, seed {seed})");
        Console.WriteLine($"  band (Hake 1998): {(g >= 0.7 ? "high" : g >= 0.3 ? "medium" : "low")}");
        Console.WriteLine($"  mean normalized change c (Marx & Cummings 2007) = {(valid.Count > 0 ? valid.Average().ToString("0.00", Inv) : "n/a")} over {valid.Count} learners{(valid.Count < ps.Count ? $" ({ps.Count - valid.Count} dropped)" : "")}");
        Console.WriteLine($"  Cohen's d (pooled SD of pre and post) = {(pooled > 0 ? ((post - pre) / pooled).ToString("0.00", Inv) : "n/a")}");
        Console.WriteLine($"  relative change (post - pre) / pre = {(pre > 0 ? (post - pre) / pre : 0).ToString("0%", Inv)}  <- not a learning measure; do not report it");

        if (ps.Count < 10) Warn($"{ps.Count} learners: the interval is wide; report it, and treat the gain as one observation, not a result");
        if (pre > 70) Warn($"class pre-test average {pre:0}%: ceiling; the test is too easy for this group to show learning");
        if (g < 0.3) Warn($"<g> = {g:0.00} is in Hake's low band: learners gained less than 30% of what they could have");

        Console.WriteLine();
        Console.WriteLine("Items");
        Console.WriteLine("  item   pre    post   g");
        var itemPre = new double[items.Length]; var itemPost = new double[items.Length];
        var pre2 = rows.Where(r => r.Phase == "pre" && ps.Any(p => p.Learner == r.Learner)).ToList();
        var post2 = rows.Where(r => r.Phase == "post" && ps.Any(p => p.Learner == r.Learner)).ToList();
        for (int i = 0; i < items.Length; i++)
        {
            itemPre[i] = 100 * pre2.Average(r => r.Scores[i]);
            itemPost[i] = 100 * post2.Average(r => r.Scores[i]);
            var ig = itemPre[i] >= 100 ? double.NaN : (itemPost[i] - itemPre[i]) / (100 - itemPre[i]);
            Console.WriteLine($"  {items[i],-5} {itemPre[i],4:0}%  {itemPost[i],4:0}%  {(double.IsNaN(ig) ? " n/a" : ig.ToString("0.00", Inv))}");
            if (itemPre[i] >= 80) Warn($"item {items[i]}: {itemPre[i]:0}% correct before teaching; it cannot show learning (ceiling)");
            if (itemPost[i] < itemPre[i]) Warn($"item {items[i]}: fewer correct after than before ({itemPre[i]:0}% -> {itemPost[i]:0}%); check the item and the teaching");
        }

        if (planPath is null) return;
        var plan = ParsePlan(planPath);
        Console.WriteLine();
        Console.WriteLine("Objectives");
        foreach (var o in plan.Objectives)
        {
            var idx = plan.Items.Where(it => it.Objective == o.Id).Select(it => Array.IndexOf(items, it.Id)).Where(x => x >= 0).ToList();
            if (idx.Count == 0) { Warn($"{o.Id}: none of its items are in {Path.GetFileName(path)}"); continue; }
            double op = idx.Average(x => itemPre[x]), oq = idx.Average(x => itemPost[x]);
            double og = op >= 100 ? double.NaN : (oq - op) / (100 - op);
            Console.WriteLine($"  {o.Id} ({idx.Count} items)  pre {op,4:0}%  post {oq,4:0}%  g {(double.IsNaN(og) ? "n/a" : og.ToString("0.00", Inv))}  {Clip(o.Text, 60)}");
            if (og < 0.3) Warn($"{o.Id}: low gain ({og:0.00}); look at the step that was supposed to teach it");
        }
    }

    // ---------------------------------------------------------------- feedback

    static void Feedback(string path, string responsesPath)
    {
        var lines = File.ReadAllLines(path).Where(l => l.Trim().Length > 0 && !l.StartsWith('#')).ToArray();
        var head = lines[0].Split(',').Select(h => h.Trim().ToLowerInvariant()).ToArray();
        int C(string n) => Array.IndexOf(head, n);
        if (C("learner") < 0 || C("rating") < 0) throw new UsageException($"{path}: header needs learner and rating");
        var fb = lines.Skip(1).Select(l => SplitCsv(l)).ToList();
        var (_, rows) = ReadResponses(responsesPath);
        var ps = Match(rows, report: false).ToDictionary(p => p.Learner);
        double Num(string[] r, string col) => C(col) >= 0 && C(col) < r.Length && double.TryParse(r[C(col)], NumberStyles.Float, Inv, out var v) ? v : double.NaN;

        var ratings = fb.Select(r => Num(r, "rating")).Where(v => !double.IsNaN(v)).ToList();
        var relevance = fb.Select(r => Num(r, "relevance")).Where(v => !double.IsNaN(v)).ToList();
        Console.WriteLine($"{Path.GetFileName(path)}: {fb.Count} forms, {ps.Count} learners with pre and post");
        Console.WriteLine();
        Console.WriteLine("Reaction (Kirkpatrick level 1)");
        Console.WriteLine($"  rating mean {ratings.Average():0.00} / 5, top-two-box {Pct(ratings.Count(v => v >= 4), ratings.Count)}");
        if (relevance.Count > 0) Console.WriteLine($"  relevance mean {relevance.Average():0.00} / 5");

        double g = ps.Count > 0 ? ClassG(ps.Values.ToList()) : double.NaN;
        Console.WriteLine();
        Console.WriteLine("Learning (Kirkpatrick level 2)");
        Console.WriteLine($"  <g> = {g:0.00}");

        var joined = fb.Where(r => ps.ContainsKey(r[C("learner")])).ToList();
        if (C("confidence") >= 0 && joined.Count > 0)
        {
            double conf = joined.Average(r => (Num(r, "confidence") - 1) / 4 * 100);
            double actual = joined.Average(r => ps[r[C("learner")]].Post);
            Console.WriteLine($"  self-rated confidence {conf:0}% vs actual post-test {actual:0}% (gap {conf - actual:+0;-0} points)");
            if (conf - actual > 20) Warn($"learners feel {conf - actual:0} points more able than the post-test shows: a fluency illusion");
        }
        if (joined.Count >= 3)
        {
            var xs = joined.Select(r => Num(r, "rating")).ToArray();
            var ys = joined.Select(r => IndividualC(ps[r[C("learner")]]) ?? 0).ToArray();
            Console.WriteLine($"  Spearman rho(rating, individual c) = {Spearman(xs, ys):0.00} over {joined.Count} learners{(joined.Count < 10 ? " (too few to interpret)" : "")}");
        }
        foreach (var l in ps.Keys.Except(joined.Select(r => r[C("learner")]))) Warn($"{l}: no feedback form");

        if (ratings.Average() >= 4.5 && g < 0.3)
            Error($"reaction without learning: rating {ratings.Average():0.0}/5 but <g> = {g:0.00}. The session was enjoyed, not learned from");

        if (C("comment") >= 0)
        {
            Console.WriteLine();
            Console.WriteLine("Comments");
            foreach (var r in fb.Where(r => C("comment") < r.Length && r[C("comment")].Length > 0))
                Console.WriteLine($"  [{r[C("rating")]}] {r[C("learner")]}: {r[C("comment")]}");
        }
    }

    // ---------------------------------------------------------------- followup

    static void Followup(string path)
    {
        var lines = File.ReadAllLines(path).Where(l => l.Trim().Length > 0 && !l.StartsWith('#')).ToArray();
        var head = lines[0].Split(',').Select(h => h.Trim().ToLowerInvariant()).ToArray();
        int u = Array.IndexOf(head, "used"), e = Array.IndexOf(head, "evidence"), l = Array.IndexOf(head, "learner");
        if (u < 0 || l < 0) throw new UsageException($"{path}: header needs learner and used");
        var rows = lines.Skip(1).Select(SplitCsv).ToList();
        int n = rows.Count, k = 0;
        foreach (var r in rows)
        {
            bool used = r[u].Trim().ToLowerInvariant() is "yes" or "y" or "1" or "true";
            string ev = e >= 0 && e < r.Length ? r[e].Trim() : "";
            if (used && ev.Length == 0) { Warn($"{r[l]}: 'used' with no evidence (a PR, a ticket, a brief); counted as not used"); used = false; }
            if (used) k++;
        }
        var (lo, hi) = Wilson(k, n);
        Console.WriteLine($"{Path.GetFileName(path)}: {k} of {n} learners used it at work with evidence ({Pct(k, n)}, 95% Wilson interval {lo.ToString("0%", Inv)} to {hi.ToString("0%", Inv)})");
        if (n < 10) Warn($"{n} learners: read the interval, not the percentage");
    }

    // ---------------------------------------------------------------- helpers

    static (double, double) Wilson(int k, int n)
    {
        if (n == 0) return (0, 1);
        double z = 1.96, p = (double)k / n, d = 1 + z * z / n;
        double c = (p + z * z / (2 * n)) / d, h = z * Math.Sqrt(p * (1 - p) / n + z * z / (4.0 * n * n)) / d;
        return (Math.Max(0, c - h), Math.Min(1, c + h));
    }

    static double Spearman(double[] x, double[] y)
    {
        double[] R(double[] v) => v.Select(a => v.Count(b => b < a) + (v.Count(b => b == a) + 1) / 2.0).ToArray();
        var rx = R(x); var ry = R(y);
        double mx = rx.Average(), my = ry.Average();
        double num = rx.Zip(ry).Sum(t => (t.First - mx) * (t.Second - my));
        double den = Math.Sqrt(rx.Sum(a => (a - mx) * (a - mx)) * ry.Sum(b => (b - my) * (b - my)));
        return den == 0 ? 0 : num / den;
    }

    static double Sd(IEnumerable<double> v)
    {
        var a = v.ToList(); if (a.Count < 2) return 0;
        double m = a.Average();
        return Math.Sqrt(a.Sum(x => (x - m) * (x - m)) / (a.Count - 1));
    }

    static double Q(List<double> sorted, double q) =>
        sorted.Count == 0 ? double.NaN : sorted[Math.Clamp((int)Math.Floor(q * (sorted.Count - 1)), 0, sorted.Count - 1)];

    static string[] SplitCsv(string line)
    {
        var res = new List<string>(); var cur = new System.Text.StringBuilder(); bool q = false;
        foreach (var ch in line)
        {
            if (ch == '"') q = !q;
            else if (ch == ',' && !q) { res.Add(cur.ToString().Trim()); cur.Clear(); }
            else cur.Append(ch);
        }
        res.Add(cur.ToString().Trim());
        return res.ToArray();
    }

    static List<string> Cells(string line) =>
        line.Trim().Trim('|').Split('|').Select(c => c.Trim().Replace("**", "").Replace("`", "")).ToList();

    static string Pct(double a, double b) => b == 0 ? "n/a" : (a / b).ToString("0%", Inv);
    static string Clip(string s, int n) => s.Length <= n ? s : s[..(n - 1)] + "…";
    static void Error(string m) { errors++; Console.WriteLine($"ERROR {m}"); }
    static void Warn(string m) { warnings++; Console.WriteLine($"WARN  {m}"); }

    static string Need(List<string> pos, int i) =>
        i < pos.Count ? pos[i] : throw new UsageException("missing file argument (see --help)");
    static string Opt(Dictionary<string, string> opt, string key) =>
        opt.TryGetValue(key, out var v) ? v : throw new UsageException($"missing --{key}");

    static (List<string>, Dictionary<string, string>) Parse(string[] args)
    {
        var pos = new List<string>();
        var opt = new Dictionary<string, string>();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("--"))
            {
                if (i + 1 >= args.Length) throw new UsageException($"{args[i]} needs a value");
                opt[args[i][2..]] = args[++i];
            }
            else pos.Add(args[i]);
        }
        foreach (var f in pos.Concat(opt.Where(o => o.Key is "plan" or "responses").Select(o => o.Value)))
            if (!File.Exists(f)) throw new UsageException($"file not found: {f}");
        return (pos, opt);
    }

    static void Usage() => Console.WriteLine("""
        LearnCheck — Module 16 checks for a teaching session
          align    <session.md>
          gain     <responses.csv> [--plan <session.md>] [--seed 16]
          feedback <feedback.csv> --responses <responses.csv>
          followup <followup.csv>
        Exit code 0 = clean, 1 = errors, 2 = usage problem.
        """);

    sealed class UsageException(string m) : Exception(m);
}
