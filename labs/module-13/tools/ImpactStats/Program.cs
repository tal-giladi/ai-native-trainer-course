// ImpactStats — the Module 13 analysis tool. Dependency-free, read-only, deterministic (fixed seeds).
//
//   describe  <csv> [--metric m] [--arm col] [--by col] [--where col=value]...
//   balance   <csv> [--arm col] [--by col]
//   compare   <csv> --metric m [--arm col --a A --b B] [--strata col] [--boot N] [--perm N] [--seed S]
//                   [--binary] [--mean] [--where col=value]... [--exclude col=value]...
//   did       <csv> --treated TEAM --pre Q --post Q [--metric col] [--unit col] [--time col]
//   assign    <csv> --block col[,col] [--arms A,B] [--seed S]
//   power     --sd s --effect e [--alpha a] [--power p]
//   import-gh <prs.json>   (output of: gh pr list --state merged --limit 500 --json number,title,createdAt,mergedAt,additions,deletions,labels)
//
// Continuous metrics (hours) are compared on the log scale: the estimate is a ratio of geometric
// means B/A, stratified when --strata is given, with a percentile bootstrap interval (resampling
// within stratum x arm) and a permutation test (shuffling arm labels within strata).
using System.Globalization;
using System.Text;
using System.Text.Json;

static class Program
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    static int Main(string[] args)
    {
        if (args.Length == 0 || args[0] is "-h" or "--help") { Usage(); return 0; }
        try
        {
            var cmd = args[0];
            var (pos, opt) = Parse(args.Skip(1).ToArray());
            switch (cmd)
            {
                case "describe": Describe(Load(pos, opt), opt); break;
                case "balance": Balance(Load(pos, opt), opt); break;
                case "compare": Compare(Load(pos, opt), opt); break;
                case "did": Did(Load(pos, opt), opt); break;
                case "assign": Assign(Load(pos, opt), opt); break;
                case "power": Power(opt); break;
                case "import-gh": ImportGh(pos); break;
                default: Console.Error.WriteLine($"unknown command '{cmd}'"); Usage(); return 2;
            }
            return 0;
        }
        catch (Exception e) when (e is IOException or FormatException or ArgumentException or InvalidOperationException or JsonException)
        {
            Console.Error.WriteLine("error: " + e.Message);
            return 1;
        }
    }

    static void Usage() => Console.WriteLine(
        "ImpactStats — describe | balance | compare | did | assign | power | import-gh\n" +
        "  describe  data.csv --metric cycle_hours --arm assigned --by size\n" +
        "  balance   data.csv --arm assigned --by size\n" +
        "  compare   data.csv --metric cycle_hours --a manual --b ai --strata size [--mean] [--binary]\n" +
        "  did       teams.csv --treated Forms --pre 2026-Q2 --post 2026-Q3\n" +
        "  assign    tickets.csv --block developer,size --seed 42\n" +
        "  power     --sd 0.35 --effect 0.15\n" +
        "  import-gh prs.json > prs.csv\n" +
        "Common: --where col=value and --exclude col=value (repeatable), --seed (default 13), --boot/--perm (default 10000).");

    // ---------------------------------------------------------------- parsing and loading
    static (List<string> pos, Dictionary<string, List<string>> opt) Parse(string[] a)
    {
        var pos = new List<string>();
        var opt = new Dictionary<string, List<string>>();
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i].StartsWith("--"))
            {
                var key = a[i][2..];
                var val = i + 1 < a.Length && !a[i + 1].StartsWith("--") ? a[++i] : "true";
                if (!opt.TryGetValue(key, out var l)) opt[key] = l = new List<string>();
                l.Add(val);
            }
            else pos.Add(a[i]);
        }
        return (pos, opt);
    }

    static string Opt(Dictionary<string, List<string>> o, string k, string def) => o.TryGetValue(k, out var v) ? v[^1] : def;
    static bool Flag(Dictionary<string, List<string>> o, string k) => o.ContainsKey(k);

    sealed record Table(string[] Cols, List<Dictionary<string, string>> Rows, string Path);

    static Table Load(List<string> pos, Dictionary<string, List<string>> opt)
    {
        if (pos.Count == 0) throw new ArgumentException("missing CSV path");
        var lines = File.ReadAllLines(pos[0]).Where(l => l.Trim().Length > 0).ToList();
        var cols = SplitCsv(lines[0]);
        var rows = new List<Dictionary<string, string>>();
        foreach (var line in lines.Skip(1))
        {
            var f = SplitCsv(line);
            var r = new Dictionary<string, string>();
            for (int i = 0; i < cols.Length; i++) r[cols[i]] = i < f.Length ? f[i] : "";
            rows.Add(r);
        }
        if (opt.TryGetValue("where", out var wh))
            foreach (var w in wh)
            {
                var kv = w.Split('=', 2);
                if (kv.Length != 2 || !cols.Contains(kv[0])) throw new ArgumentException($"bad --where '{w}'");
                rows = rows.Where(r => r[kv[0]] == kv[1]).ToList();
            }
        if (opt.TryGetValue("exclude", out var ex))
            foreach (var w in ex)
            {
                var kv = w.Split('=', 2);
                if (kv.Length != 2 || !cols.Contains(kv[0])) throw new ArgumentException($"bad --exclude '{w}'");
                rows = rows.Where(r => r[kv[0]] != kv[1]).ToList();
            }
        return new Table(cols, rows, pos[0]);
    }

    static string[] SplitCsv(string line)
    {
        var res = new List<string>(); var sb = new StringBuilder(); bool q = false;
        foreach (var ch in line)
        {
            if (ch == '"') q = !q;
            else if (ch == ',' && !q) { res.Add(sb.ToString()); sb.Clear(); }
            else sb.Append(ch);
        }
        res.Add(sb.ToString());
        return res.ToArray();
    }

    static double Num(Dictionary<string, string> r, string col)
    {
        if (!r.TryGetValue(col, out var s)) throw new ArgumentException($"no column '{col}'");
        if (!double.TryParse(s, NumberStyles.Float, Inv, out var d)) throw new FormatException($"'{s}' in column '{col}' is not a number");
        return d;
    }

    static string F(double x, int d = 1) => double.IsNaN(x) ? "n/a" : x.ToString("F" + d, Inv);
    static string Pct(double ratio) => (ratio - 1 >= 0 ? "+" : "") + ((ratio - 1) * 100).ToString("F0", Inv) + "%";

    // ---------------------------------------------------------------- basic statistics
    static double Mean(IReadOnlyList<double> x) => x.Count == 0 ? double.NaN : x.Average();
    static double Var(IReadOnlyList<double> x) { if (x.Count < 2) return double.NaN; var m = x.Average(); return x.Sum(v => (v - m) * (v - m)) / (x.Count - 1); }
    static double Median(IEnumerable<double> xs)
    {
        var s = xs.OrderBy(v => v).ToArray();
        if (s.Length == 0) return double.NaN;
        return s.Length % 2 == 1 ? s[s.Length / 2] : (s[s.Length / 2 - 1] + s[s.Length / 2]) / 2;
    }
    static double Quantile(double[] sorted, double q)
    {
        var h = (sorted.Length - 1) * q; var lo = (int)Math.Floor(h); var hi = (int)Math.Ceiling(h);
        return sorted[lo] + (h - lo) * (sorted[hi] - sorted[lo]);
    }
    static double GeoMean(IReadOnlyList<double> x) => Math.Exp(x.Average(v => Math.Log(v)));

    static (double lo, double hi) Wilson(int k, int n, double z = 1.96)
    {
        if (n == 0) return (double.NaN, double.NaN);
        double p = (double)k / n, den = 1 + z * z / n;
        double c = (p + z * z / (2 * n)) / den, h = z * Math.Sqrt(p * (1 - p) / n + z * z / (4.0 * n * n)) / den;
        return (Math.Max(0, c - h), Math.Min(1, c + h));
    }

    // Student t two-sided p-value via the regularized incomplete beta function.
    static double TwoSidedP(double t, double df) => IncBeta(df / 2, 0.5, df / (df + t * t));
    static double IncBeta(double a, double b, double x)
    {
        if (x <= 0) return 0; if (x >= 1) return 1;
        double lbeta = LogGamma(a + b) - LogGamma(a) - LogGamma(b) + a * Math.Log(x) + b * Math.Log(1 - x);
        if (x < (a + 1) / (a + b + 2)) return Math.Exp(lbeta) * BetaCf(a, b, x) / a;
        return 1 - Math.Exp(lbeta) * BetaCf(b, a, 1 - x) / b;
    }
    static double BetaCf(double a, double b, double x)
    {
        double qab = a + b, qap = a + 1, qam = a - 1, c = 1, d = 1 - qab * x / qap;
        if (Math.Abs(d) < 1e-300) d = 1e-300; d = 1 / d; double h = d;
        for (int m = 1; m <= 300; m++)
        {
            int m2 = 2 * m; double aa = m * (b - m) * x / ((qam + m2) * (a + m2));
            d = 1 + aa * d; if (Math.Abs(d) < 1e-300) d = 1e-300; c = 1 + aa / c; if (Math.Abs(c) < 1e-300) c = 1e-300; d = 1 / d; h *= d * c;
            aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2));
            d = 1 + aa * d; if (Math.Abs(d) < 1e-300) d = 1e-300; c = 1 + aa / c; if (Math.Abs(c) < 1e-300) c = 1e-300; d = 1 / d;
            var del = d * c; h *= del; if (Math.Abs(del - 1) < 1e-12) break;
        }
        return h;
    }
    static double LogGamma(double x)
    {
        double[] c = { 76.18009172947146, -86.50532032941677, 24.01409824083091, -1.231739572450155, 0.1208650973866179e-2, -0.5395239384953e-5 };
        double y = x, tmp = x + 5.5; tmp -= (x + 0.5) * Math.Log(tmp); double ser = 1.000000000190015;
        foreach (var ci in c) ser += ci / ++y;
        return -tmp + Math.Log(2.5066282746310005 * ser / x);
    }
    static double NormInv(double p)
    {
        // Acklam's rational approximation (sufficient for sample-size tables).
        double[] a = { -3.969683028665376e+01, 2.209460984245205e+02, -2.759285104469687e+02, 1.383577518672690e+02, -3.066479806614716e+01, 2.506628277459239e+00 };
        double[] b = { -5.447609879822406e+01, 1.615858368580409e+02, -1.556989798598866e+02, 6.680131188771972e+01, -1.328068155288572e+01 };
        double[] c = { -7.784894002430293e-03, -3.223964580411365e-01, -2.400758277161838e+00, -2.549732539343734e+00, 4.374664141464968e+00, 2.938163982698783e+00 };
        double[] d = { 7.784695709041462e-03, 3.224671290700398e-01, 2.445134137142996e+00, 3.754408661907416e+00 };
        double q, r;
        if (p < 0.02425) { q = Math.Sqrt(-2 * Math.Log(p)); return (((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) / ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1); }
        if (p > 1 - 0.02425) { q = Math.Sqrt(-2 * Math.Log(1 - p)); return -(((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) / ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1); }
        q = p - 0.5; r = q * q;
        return (((((a[0] * r + a[1]) * r + a[2]) * r + a[3]) * r + a[4]) * r + a[5]) * q / (((((b[0] * r + b[1]) * r + b[2]) * r + b[3]) * r + b[4]) * r + 1);
    }

    // ---------------------------------------------------------------- describe / balance
    static void Describe(Table t, Dictionary<string, List<string>> o)
    {
        var metric = Opt(o, "metric", "cycle_hours"); var arm = Opt(o, "arm", "assigned"); var by = Opt(o, "by", "");
        Console.WriteLine($"{Path.GetFileName(t.Path)}: {t.Rows.Count} rows, metric {metric}, arm column {arm}{(by != "" ? ", by " + by : "")}");
        Console.WriteLine($"{"group",-18} {"n",4} {"median",8} {"p25",7} {"p75",7} {"p90",7} {"mean",8} {"geomean",8} {"sd(log)",8} {"max",7}");
        var groups = t.Rows.GroupBy(r => (by == "" ? "" : r[by] + " / ") + r[arm]).OrderBy(g => g.Key, StringComparer.Ordinal);
        foreach (var g in groups)
        {
            var x = g.Select(r => Num(r, metric)).OrderBy(v => v).ToArray();
            var gm = x.All(v => v > 0) ? GeoMean(x) : double.NaN;
            var sdl = x.All(v => v > 0) ? Math.Sqrt(Var(x.Select(v => Math.Log(v)).ToArray())) : double.NaN;
            Console.WriteLine($"{g.Key,-18} {x.Length,4} {F(Median(x)),8} {F(Quantile(x, .25)),7} {F(Quantile(x, .75)),7} {F(Quantile(x, .9)),7} {F(Mean(x)),8} {F(gm),8} {F(sdl, 2),8} {F(x[^1]),7}");
        }
    }

    static void Balance(Table t, Dictionary<string, List<string>> o)
    {
        var arm = Opt(o, "arm", "assigned"); var by = Opt(o, "by", "size");
        var arms = t.Rows.Select(r => r[arm]).Distinct().OrderBy(s => s, StringComparer.Ordinal).ToList();
        var levels = t.Rows.Select(r => r[by]).Distinct().OrderBy(s => s, StringComparer.Ordinal).ToList();
        Console.WriteLine($"Balance of {arm} across {by} ({t.Rows.Count} rows)");
        Console.WriteLine($"{by,-10}" + string.Concat(arms.Select(a => $"{a,10}")) + $"{"share " + arms[^1],14}");
        foreach (var lv in levels)
        {
            var counts = arms.Select(a => t.Rows.Count(r => r[by] == lv && r[arm] == a)).ToList();
            var share = (double)counts[^1] / counts.Sum();
            Console.WriteLine($"{lv,-10}" + string.Concat(counts.Select(c => $"{c,10}")) + $"{(share * 100).ToString("F0", Inv) + "%",14}");
        }
        foreach (var a in arms)
        {
            var mix = levels.Select(lv => $"{lv} {(100.0 * t.Rows.Count(r => r[by] == lv && r[arm] == a) / t.Rows.Count(r => r[arm] == a)).ToString("F0", Inv)}%");
            Console.WriteLine($"mix of {a}: " + string.Join(", ", mix));
        }
        // Chi-square test of independence (approximate).
        double chi = 0; int n = t.Rows.Count;
        foreach (var lv in levels) foreach (var a in arms)
            {
                double obs = t.Rows.Count(r => r[by] == lv && r[arm] == a);
                double exp = (double)t.Rows.Count(r => r[by] == lv) * t.Rows.Count(r => r[arm] == a) / n;
                chi += (obs - exp) * (obs - exp) / exp;
            }
        int df = (levels.Count - 1) * (arms.Count - 1);
        Console.WriteLine($"chi-square {F(chi, 2)} on {df} df, p = {F(ChiSqP(chi, df), 4)} " +
                          (ChiSqP(chi, df) < 0.05 ? "-> arms are NOT balanced on " + by + ": compare within strata or randomize." : "-> no evidence of imbalance on " + by + "."));
    }

    static double ChiSqP(double x, int k) => 1 - LowerGammaReg(k / 2.0, x / 2);
    static double LowerGammaReg(double s, double x)
    {
        if (x <= 0) return 0;
        if (x < s + 1)
        {
            double sum = 1 / s, term = sum;
            for (int n = 1; n < 500; n++) { term *= x / (s + n); sum += term; if (term < sum * 1e-14) break; }
            return sum * Math.Exp(-x + s * Math.Log(x) - LogGamma(s));
        }
        // continued fraction for the upper tail
        double b = x + 1 - s, c = 1e300, d = 1 / b, h = d;
        for (int i = 1; i < 500; i++)
        {
            double an = -i * (i - s); b += 2; d = an * d + b; if (Math.Abs(d) < 1e-300) d = 1e-300; c = b + an / c; if (Math.Abs(c) < 1e-300) c = 1e-300;
            d = 1 / d; var del = d * c; h *= del; if (Math.Abs(del - 1) < 1e-14) break;
        }
        return 1 - Math.Exp(-x + s * Math.Log(x) - LogGamma(s)) * h;
    }

    // ---------------------------------------------------------------- compare
    sealed record Cell(string Stratum, double[] A, double[] B);

    static void Compare(Table t, Dictionary<string, List<string>> o)
    {
        var metric = Opt(o, "metric", "cycle_hours"); var arm = Opt(o, "arm", "assigned");
        var a = Opt(o, "a", "manual"); var b = Opt(o, "b", "ai"); var strata = Opt(o, "strata", "");
        int boot = int.Parse(Opt(o, "boot", "10000"), Inv), perm = int.Parse(Opt(o, "perm", "10000"), Inv), seed = int.Parse(Opt(o, "seed", "13"), Inv);
        bool binary = Flag(o, "binary");
        var rows = t.Rows.Where(r => r[arm] == a || r[arm] == b).ToList();
        if (rows.Count == 0) throw new InvalidOperationException($"no rows with {arm} = {a} or {b}");
        Func<Dictionary<string, string>, double> val = binary ? r => Num(r, metric) > 0 ? 1 : 0 : r => Num(r, metric);
        if (!binary && rows.Any(r => Num(r, metric) <= 0)) throw new InvalidOperationException($"{metric} has values <= 0; use --binary or a positive metric");

        var cells = rows.GroupBy(r => strata == "" ? "(all)" : r[strata]).OrderBy(g => g.Key, StringComparer.Ordinal)
            .Select(g => new Cell(g.Key, g.Where(r => r[arm] == a).Select(val).ToArray(), g.Where(r => r[arm] == b).Select(val).ToArray())).ToList();
        var dropped = cells.Where(c => c.A.Length == 0 || c.B.Length == 0).ToList();
        cells = cells.Where(c => c.A.Length > 0 && c.B.Length > 0).ToList();
        if (cells.Count == 0) throw new InvalidOperationException($"no stratum contains both {a} and {b}; nothing to compare");

        Console.WriteLine($"{Path.GetFileName(t.Path)}: {metric}, {arm} {b} (B) vs {a} (A), {rows.Count} rows" +
                          (strata != "" ? $", stratified by {strata}" : ", unstratified") + (o.ContainsKey("where") ? ", where " + string.Join(" & ", o["where"]) : "") + (o.ContainsKey("exclude") ? ", excluding " + string.Join(" & ", o["exclude"]) : ""));
        foreach (var c in dropped) Console.WriteLine($"  WARNING stratum {c.Stratum} has only one arm ({c.A.Length} A, {c.B.Length} B) and is excluded");

        Func<List<Cell>, double> stat = binary ? StratDiffMean : StratDiffLog;
        double obs = stat(cells);

        // per-stratum table
        if (binary)
        {
            Console.WriteLine($"{"stratum",-8} {"nA",4} {"nB",4} {"A rate",8} {"B rate",8} {"B-A",8}");
            foreach (var c in cells) Console.WriteLine($"{c.Stratum,-8} {c.A.Length,4} {c.B.Length,4} {F(100 * c.A.Average(), 0) + "%",8} {F(100 * c.B.Average(), 0) + "%",8} {F(100 * (c.B.Average() - c.A.Average()), 0) + " pt",8}");
            int ka = (int)cells.Sum(c => c.A.Sum()), na = cells.Sum(c => c.A.Length), kb = (int)cells.Sum(c => c.B.Sum()), nb = cells.Sum(c => c.B.Length);
            var wa = Wilson(ka, na); var wb = Wilson(kb, nb);
            Console.WriteLine($"A {a}: {ka}/{na} = {F(100.0 * ka / na, 0)}%  Wilson 95% [{F(100 * wa.lo, 0)}%, {F(100 * wa.hi, 0)}%]");
            Console.WriteLine($"B {b}: {kb}/{nb} = {F(100.0 * kb / nb, 0)}%  Wilson 95% [{F(100 * wb.lo, 0)}%, {F(100 * wb.hi, 0)}%]");
        }
        else
        {
            Console.WriteLine($"{"stratum",-8} {"nA",4} {"nB",4} {"median A",9} {"median B",9} {"geo A",7} {"geo B",7} {"B/A",7} {"Cliff d",8}");
            foreach (var c in cells)
                Console.WriteLine($"{c.Stratum,-8} {c.A.Length,4} {c.B.Length,4} {F(Median(c.A)),9} {F(Median(c.B)),9} {F(GeoMean(c.A)),7} {F(GeoMean(c.B)),7} {F(GeoMean(c.B) / GeoMean(c.A), 2),7} {F(Cliff(c.A, c.B), 2),8}");
            var allA = cells.SelectMany(c => c.A).ToArray(); var allB = cells.SelectMany(c => c.B).ToArray();
            Console.WriteLine($"pooled medians: A {F(Median(allA))} h, B {F(Median(allB))} h (B/A {F(Median(allB) / Median(allA), 2)}, {Pct(Median(allB) / Median(allA))}) — not adjusted for {(strata == "" ? "anything" : strata)}");
        }

        // bootstrap
        var rng = new Random(seed);
        var bs = new double[boot];
        for (int i = 0; i < boot; i++)
            bs[i] = stat(cells.Select(c => new Cell(c.Stratum, Resample(c.A, rng), Resample(c.B, rng))).ToList());
        Array.Sort(bs);
        double lo = Quantile(bs, 0.025), hi = Quantile(bs, 0.975);

        // permutation within strata
        var prng = new Random(seed + 1); int extreme = 0;
        for (int i = 0; i < perm; i++)
        {
            var shuffled = cells.Select(c =>
            {
                var all = c.A.Concat(c.B).ToArray(); Shuffle(all, prng);
                return new Cell(c.Stratum, all[..c.A.Length], all[c.A.Length..]);
            }).ToList();
            if (Math.Abs(stat(shuffled)) >= Math.Abs(obs) - 1e-12) extreme++;
        }
        double p = (extreme + 1.0) / (perm + 1.0);

        if (binary)
        {
            Console.WriteLine($"{(strata != "" ? "stratified " : "")}difference B-A: {F(100 * obs, 1)} pt, bootstrap 95% CI [{F(100 * lo, 1)}, {F(100 * hi, 1)}] pt ({boot} resamples, seed {seed})");
        }
        else
        {
            Console.WriteLine($"{(strata != "" ? "stratified " : "")}ratio of geometric means B/A: {F(Math.Exp(obs), 3)} ({Pct(Math.Exp(obs))}), " +
                              $"bootstrap 95% CI [{F(Math.Exp(lo), 3)}, {F(Math.Exp(hi), 3)}] = [{Pct(Math.Exp(lo))}, {Pct(Math.Exp(hi))}] ({boot} resamples, seed {seed})");
            double g = Hedges(cells, obs);
            var allA = cells.SelectMany(c => c.A).ToArray(); var allB = cells.SelectMany(c => c.B).ToArray();
            Console.WriteLine($"effect size: Hedges g on log {metric} = {F(g, 2)} (within-{(strata == "" ? "arm" : "stratum")} SD); Cliff's delta (pooled, raw) = {F(Cliff(allA, allB), 2)}");
        }
        Console.WriteLine($"permutation test ({perm} shuffles{(strata != "" ? " within " + strata : "")}, seed {seed + 1}): two-sided p = {F(p, 4)}");

        if (Flag(o, "mean") && !binary)
        {
            var A = cells.SelectMany(c => c.A).ToArray(); var B = cells.SelectMany(c => c.B).ToArray();
            double ma = Mean(A), mb = Mean(B), va = Var(A), vb = Var(B);
            double se = Math.Sqrt(va / A.Length + vb / B.Length), tt = (mb - ma) / se;
            double df = Math.Pow(va / A.Length + vb / B.Length, 2) / (Math.Pow(va / A.Length, 2) / (A.Length - 1) + Math.Pow(vb / B.Length, 2) / (B.Length - 1));
            Console.WriteLine($"[naive] raw means (unstratified): A {F(ma)} h, B {F(mb)} h, B/A {F(mb / ma, 3)} ({Pct(mb / ma)}); Welch t = {F(tt, 2)}, df = {F(df, 1)}, p = {F(TwoSidedP(tt, df), 4)}");
            var top = A.Concat(B).OrderByDescending(v => v).Take(1).First();
            Console.WriteLine($"[naive] largest single value {F(top)} h is {F(top / Median(A.Concat(B)), 1)}x the pooled median");
        }
    }

    static double StratDiffLog(List<Cell> cells)
    {
        double acc = 0, w = 0;
        foreach (var c in cells) { var n = c.A.Length + c.B.Length; acc += n * (c.B.Average(v => Math.Log(v)) - c.A.Average(v => Math.Log(v))); w += n; }
        return acc / w;
    }
    static double StratDiffMean(List<Cell> cells)
    {
        double acc = 0, w = 0;
        foreach (var c in cells) { var n = c.A.Length + c.B.Length; acc += n * (c.B.Average() - c.A.Average()); w += n; }
        return acc / w;
    }
    static double Hedges(List<Cell> cells, double diff)
    {
        double ss = 0; int df = 0;
        foreach (var c in cells)
            foreach (var arr in new[] { c.A, c.B })
            {
                var l = arr.Select(v => Math.Log(v)).ToArray(); var m = l.Average();
                ss += l.Sum(v => (v - m) * (v - m)); df += l.Length - 1;
            }
        if (df <= 1) return double.NaN;
        var sd = Math.Sqrt(ss / df); var j = 1 - 3.0 / (4 * df - 1);
        return diff / sd * j;
    }
    static double Cliff(double[] a, double[] b)
    {
        long gt = 0, lt = 0;
        foreach (var y in b) foreach (var x in a) { if (y > x) gt++; else if (y < x) lt++; }
        return (double)(gt - lt) / ((long)a.Length * b.Length);
    }
    static double[] Resample(double[] x, Random r) { var o = new double[x.Length]; for (int i = 0; i < x.Length; i++) o[i] = x[r.Next(x.Length)]; return o; }
    static void Shuffle(double[] x, Random r) { for (int i = x.Length - 1; i > 0; i--) { int j = r.Next(i + 1); (x[i], x[j]) = (x[j], x[i]); } }

    // ---------------------------------------------------------------- difference in differences
    static void Did(Table t, Dictionary<string, List<string>> o)
    {
        var unit = Opt(o, "unit", "team"); var time = Opt(o, "time", "quarter"); var metric = Opt(o, "metric", "median_cycle_hours");
        var treated = Opt(o, "treated", ""); var pre = Opt(o, "pre", ""); var post = Opt(o, "post", "");
        if (treated == "" || pre == "" || post == "") throw new ArgumentException("did needs --treated, --pre and --post");
        var units = t.Rows.Select(r => r[unit]).Distinct().OrderBy(s => s, StringComparer.Ordinal).ToList();
        double Get(string u, string q) => Num(t.Rows.Single(r => r[unit] == u && r[time] == q), metric);
        double Change(string u) => Math.Log(Get(u, post) / Get(u, pre));
        var others = units.Where(u => u != treated).ToList();
        double tc = Change(treated), oc = others.Average(Change), did = tc - oc;
        Console.WriteLine($"{metric}: {treated} {pre} -> {post}: {F(Get(treated, pre))} -> {F(Get(treated, post))} h ({Pct(Math.Exp(tc))})");
        Console.WriteLine($"other {others.Count} {unit}s, mean change on the log scale: {Pct(Math.Exp(oc))}");
        Console.WriteLine($"difference-in-differences (ratio): {F(Math.Exp(did), 3)} ({Pct(Math.Exp(did))})");
        // Placebo: pretend each other unit was treated.
        var placebo = others.Select(u => Change(u) - units.Where(v => v != u).Average(Change)).OrderBy(v => v).ToList();
        int asExtreme = placebo.Count(v => Math.Abs(v) >= Math.Abs(did) - 1e-12);
        Console.WriteLine($"placebo DiDs for the other {unit}s: {string.Join(", ", placebo.Select(v => Pct(Math.Exp(v))))}");
        Console.WriteLine($"{asExtreme} of {placebo.Count} placebo {unit}s are at least as extreme (placebo p = {F((asExtreme + 1.0) / (placebo.Count + 1), 2)})");
        // Selection check: was the treated unit extreme in the pre period?
        var preVals = units.Select(u => (u, v: Get(u, pre))).OrderByDescending(x => x.v).ToList();
        int rank = preVals.FindIndex(x => x.u == treated) + 1;
        Console.WriteLine($"selection check: {treated} ranked {rank} of {units.Count} on {metric} in {pre}" +
                          (rank == 1 || rank == units.Count ? " — the most extreme value; expect regression to the mean." : "."));
    }

    // ---------------------------------------------------------------- assignment
    static void Assign(Table t, Dictionary<string, List<string>> o)
    {
        var blocks = Opt(o, "block", "size").Split(',', StringSplitOptions.RemoveEmptyEntries);
        var arms = Opt(o, "arms", "ai,manual").Split(',');
        var rng = new Random(int.Parse(Opt(o, "seed", "13"), Inv));
        var assigned = new Dictionary<Dictionary<string, string>, string>();
        foreach (var g in t.Rows.GroupBy(r => string.Join("|", blocks.Select(b => r[b]))).OrderBy(g => g.Key, StringComparer.Ordinal))
        {
            var items = g.ToList();
            // Full rounds of every arm, then a random subset of arms for the remainder, so odd-sized
            // blocks do not always favour the first arm.
            var extra = arms.OrderBy(_ => rng.Next()).Take(items.Count % arms.Length);
            var labels = Enumerable.Range(0, items.Count / arms.Length).SelectMany(_ => arms).Concat(extra).ToArray();
            for (int i = labels.Length - 1; i > 0; i--) { int j = rng.Next(i + 1); (labels[i], labels[j]) = (labels[j], labels[i]); }
            for (int i = 0; i < items.Count; i++) assigned[items[i]] = labels[i];
        }
        Console.WriteLine(string.Join(",", t.Cols) + ",arm");
        foreach (var r in t.Rows) Console.WriteLine(string.Join(",", t.Cols.Select(c => r[c])) + "," + assigned[r]);
    }

    // ---------------------------------------------------------------- power
    static void Power(Dictionary<string, List<string>> o)
    {
        double sd = double.Parse(Opt(o, "sd", "0.5"), Inv), alpha = double.Parse(Opt(o, "alpha", "0.05"), Inv), pw = double.Parse(Opt(o, "power", "0.8"), Inv);
        double za = NormInv(1 - alpha / 2), zb = NormInv(pw);
        Console.WriteLine($"Tickets per arm to detect a reduction in the geometric mean, two arms, sd of log(hours) = {F(sd, 2)}, alpha {F(alpha, 2)}, power {F(pw, 2)}");
        Console.WriteLine("n per arm = 2 (z_a/2 + z_b)^2 sd^2 / delta^2, delta = -ln(1 - reduction)");
        var effects = o.ContainsKey("effect") ? new[] { double.Parse(Opt(o, "effect", "0.15"), Inv) } : new[] { 0.05, 0.10, 0.15, 0.20, 0.30, 0.40 };
        foreach (var e in effects)
        {
            double delta = -Math.Log(1 - e);
            double n = 2 * Math.Pow(za + zb, 2) * sd * sd / (delta * delta);
            Console.WriteLine($"  reduction {F(100 * e, 0),3}%  -> {Math.Ceiling(n),6} tickets per arm ({Math.Ceiling(2 * n)} total)");
        }
    }

    // ---------------------------------------------------------------- import from gh
    static void ImportGh(List<string> pos)
    {
        if (pos.Count == 0) throw new ArgumentException("missing prs.json");
        using var doc = JsonDocument.Parse(File.ReadAllText(pos[0]));
        Console.WriteLine("pr,created,merged,open_to_merge_hours,lines_changed,labels");
        foreach (var pr in doc.RootElement.EnumerateArray())
        {
            if (!pr.TryGetProperty("mergedAt", out var m) || m.ValueKind != JsonValueKind.String) continue;
            var created = DateTimeOffset.Parse(pr.GetProperty("createdAt").GetString()!, Inv);
            var merged = DateTimeOffset.Parse(m.GetString()!, Inv);
            int lines = (pr.TryGetProperty("additions", out var ad) ? ad.GetInt32() : 0) + (pr.TryGetProperty("deletions", out var de) ? de.GetInt32() : 0);
            var labels = pr.TryGetProperty("labels", out var lb) ? string.Join(";", lb.EnumerateArray().Select(l => l.GetProperty("name").GetString()?.Replace(',', ' '))) : "";
            Console.WriteLine($"{pr.GetProperty("number").GetInt32()},{created:O},{merged:O},{F((merged - created).TotalHours)},{lines},{labels}");
        }
    }
}
