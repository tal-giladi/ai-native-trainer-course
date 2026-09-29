// ScaleCheck — the Module 22 tool. Dependency-free, read-only, deterministic.
//
//   service <service.md>            Service card: fixed price, fixed scope, a standard asset for every
//                                   standard step, custom share, effective day rate against your floor,
//                                   and the learning curve across past deliveries (22.1).
//   catalog <catalog.md>            Workshop, course, community and free products: a licence for every
//                                   artifact, software under a software licence, team-licence terms,
//                                   a third-party register, community rules, revenue per live day and
//                                   the share of revenue not tied to your hours (22.2).
//   kit     <kit-dir> [deny.txt]    Templates, starter pack and assessments as a product: versions,
//                                   licences, verification dates, stranger tests with a Wilson interval,
//                                   a changelog that matches, and no employer or client terms (22.3).
//   items   <responses.csv>         Assessment item analysis: difficulty p and the upper-lower 27%
//                                   discrimination index D for each item (22.3).
//   plan    <productization-plan.md> Capacity cap, allocation, retainers with caps and rollover rules,
//                                   queueing wait at the planned utilization (Kingman), month-12 revenue
//                                   mix, a 12-month roadmap with exit signals, and stop rules (22.4).
//
// Licensing checks are orientation only: the tool checks that a licence was chosen and stated, never
// that it is the right one for your jurisdiction or your contracts.
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
            switch (args[0])
            {
                case "service": Service(NeedFile(args[1])); break;
                case "catalog": Catalog(NeedFile(args[1])); break;
                case "kit": Kit(NeedDir(args[1]), args.Length > 2 ? NeedFile(args[2]) : null); break;
                case "items": Items(NeedFile(args[1])); break;
                case "plan": Plan(NeedFile(args[1])); break;
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
        "ScaleCheck <command> <file>\n" +
        "  service <service.md>              productized service card (22.1)\n" +
        "  catalog <catalog.md>              products, licences and revenue mix (22.2)\n" +
        "  kit     <kit-dir> [deny.txt]      templates, starter pack and assessments as a product (22.3)\n" +
        "  items   <responses.csv>           assessment item difficulty and discrimination (22.3)\n" +
        "  plan    <productization-plan.md>  capacity, retainers, queue, revenue mix, roadmap (22.4)\n" +
        "Exit code 0 = clean, 1 = errors, 2 = usage problem.");

    // ---------------------------------------------------------------- shared text checks

    static readonly Regex Negation = new(@"\b(no|not|never|cannot|can't|don't|doesn't|won't|without|nor|isn't|aren't)\b", I);

    static IEnumerable<string> Sentences(string text) =>
        Regex.Split(StripFences(text), @"(?<=[.!?])\s+|\n").Select(s => s.Trim()).Where(s => s.Length > 0);

    static string StripFences(string text) => Regex.Replace(text, "```.*?```", "", RegexOptions.Singleline);

    static void Promises(string text, string where)
    {
        foreach (var s in Sentences(text))
        {
            if (s.EndsWith('?')) continue;
            if (Regex.IsMatch(s, @"guarante", I) && !Negation.IsMatch(s))
                Error($"{where}: guarantee of an outcome: \"{Clip(s, 90)}\"");
            if (Regex.IsMatch(s, @"\b\d+(\.\d+)?\s?x\b(?!\s*\d)", I) && !Negation.IsMatch(s))
                Error($"{where}: multiplier claim: \"{Clip(s, 90)}\"");
            if (Regex.IsMatch(s, @"\b\d+(\.\d+)?\s?%\s+(faster|more productive|productivity|improvement|increase|fewer|better|shorter)", I)
                && !Regex.IsMatch(s, @"interval|\bCI\b|range|\bto\s+[−-]?\d|–\s?\d|EXP-\d+", I) && !Negation.IsMatch(s))
                Error($"{where}: effect without interval or source: \"{Clip(s, 90)}\"");
            if (Regex.IsMatch(s, @"\bonly \d+ (spots?|seats?|places?)\b|\b(price|prices)\s+(goes|go|will go)\s+up\b|\bact now\b|\blimited[- ]time\b", I))
                Error($"{where}: pressure tactic: \"{Clip(s, 90)}\"");
        }
    }

    // ---------------------------------------------------------------- service (22.1)

    static readonly Regex OpenScope = new(@"\b(bespoke|tailored to (you|your)|whatever you need|unlimited|as many as|as needed|open[- ]ended|anything (you|your))\b", I);

    static void Service(string path)
    {
        var lines = File.ReadAllLines(path);
        var text = string.Join("\n", lines);
        var kv = KeyValues(lines);
        var sec = Sections(lines);
        Console.WriteLine($"{Path.GetFileName(path)}: service card");

        var name = kv.GetValueOrDefault("service", "");
        if (name == "") Error("no 'Service:' line");
        else Console.WriteLine($"  service: {name}");

        double price = double.NaN;
        if (!kv.TryGetValue("price", out var ps)) Error("no 'Price:' line");
        else if (Regex.IsMatch(ps, @"\b(from|starting|contact|tbd|tba|quote|per (day|hour)|day rate|hourly|/\s?h\b)", I))
        { Error($"price is not fixed: \"{ps}\" (a productized service has one price for one scope)"); price = FirstNumber(ps) ?? double.NaN; }
        else if (FirstNumber(ps) is double p) price = p;
        else Error($"price has no number: \"{ps}\"");

        foreach (var k in new[] { "duration", "entry", "version" })
            if (!kv.ContainsKey(k)) Error($"no '{char.ToUpper(k[0]) + k[1..]}:' line");
        double hpd = kv.TryGetValue("hours per day", out var h) && FirstNumber(h) is double hh ? hh : 8;
        double floor = kv.TryGetValue("floor", out var fl) && FirstNumber(fl) is double ff ? ff : double.NaN;
        if (double.IsNaN(floor)) Error("no 'Floor:' day rate (20.2); the card cannot tell whether the price pays you");
        double plannedDays = kv.TryGetValue("planned days", out var pd) && FirstNumber(pd) is double pdd ? pdd : double.NaN;

        var prose = string.Join("\n", lines.Where(l => !l.TrimStart().StartsWith('|')));
        foreach (var s in Sentences(prose))
            if (OpenScope.IsMatch(s) && !Negation.IsMatch(s)) Error($"open-ended scope: \"{Clip(s, 90)}\"");
        Promises(prose, "card");

        // steps
        var steps = Tables(lines).FirstOrDefault(t => t.head.Contains("step") && t.head.Any(c => c.StartsWith("standard asset")));
        double planned = 0, custom = 0;
        if (steps.head == null) Error("no steps table (| # | Step | Owner | Standard asset | Planned hours | Custom |)");
        else
        {
            int iStep = steps.head.IndexOf("step"), iAsset = steps.head.FindIndex(c => c.StartsWith("standard asset")),
                iH = steps.head.FindIndex(c => c.StartsWith("planned hours")), iC = steps.head.IndexOf("custom"), iO = steps.head.IndexOf("owner");
            Console.WriteLine();
            Console.WriteLine("  step                                        hours  custom  asset");
            foreach (var r in steps.rows)
            {
                string step = Get(r, iStep), asset = Dash(Get(r, iAsset)), owner = Dash(Get(r, iO));
                double hrs = FirstNumber(Get(r, iH)) ?? double.NaN;
                bool isCustom = Regex.IsMatch(Get(r, iC), @"^\s*(yes|y|true)\b", I);
                Console.WriteLine($"  {Clip(step, 42),-42}  {Num(hrs),5}  {(isCustom ? "yes" : "no"),-6}  {Clip(asset == "" ? "(none)" : asset, 40)}");
                if (OpenScope.IsMatch(step) || Regex.IsMatch(step, @"\bwhatever\b|as needed", I)) Error($"step '{step}': open-ended step");
                if (double.IsNaN(hrs)) { Error($"step '{step}': no planned hours"); continue; }
                planned += hrs;
                if (isCustom) custom += hrs;
                else if (asset == "") Error($"step '{step}': standard step with no standard asset (it lives in your head, so it cannot repeat)");
                if (owner == "") Warn($"step '{step}': no owner (me, client, tool)");
            }
            double share = planned > 0 ? custom / planned : 0;
            Console.WriteLine();
            Console.WriteLine($"  planned: {Num(planned)} h ({Num(planned / hpd)} days), custom share {Pct(share)}");
            if (share > 0.40) Error($"custom share {Pct(share)}: this is consulting with a fixed price, not a repeatable service (aim for 25% or less)");
            else if (share > 0.25) Warn($"custom share {Pct(share)}: above 25%; which custom step could become a standard asset?");
            if (!double.IsNaN(plannedDays) && Math.Abs(plannedDays * hpd - planned) > 0.1 * planned)
                Warn($"'Planned days: {Num(plannedDays)}' does not match the steps' {Num(planned)} h");
            if (!double.IsNaN(price) && !double.IsNaN(floor) && planned > 0)
            {
                double eff = price / (planned / hpd);
                Console.WriteLine($"  effective day rate at plan: {Money(eff)} (floor {Money(floor)})");
                if (eff < floor) Error($"planned effective day rate {Money(eff)} is below your floor {Money(floor)}: raise the price or cut the hours");
            }
        }

        // not included
        var ni = sec.FirstOrDefault(kvp => kvp.Key.StartsWith("not included")).Value;
        int niCount = ni == null ? 0 : ni.Split('\n').Count(l => l.TrimStart().StartsWith("- "));
        if (niCount < 3) Error($"'## Not included' lists {niCount} item(s); a fixed scope needs at least 3 explicit exclusions");

        // deliveries and the learning curve
        var del = Tables(lines).FirstOrDefault(t => t.head.Contains("delivery") && t.head.Contains("hours"));
        if (del.head == null) { Warn("no deliveries table: the service has never been delivered; label it beta"); return; }
        int iD = del.head.IndexOf("delivery"), iHr = del.head.IndexOf("hours"), iCu = del.head.FindIndex(c => c.StartsWith("custom hours"));
        var hrsList = new List<double>();
        Console.WriteLine();
        Console.WriteLine("  delivery  hours  custom  effective day rate");
        foreach (var r in del.rows)
        {
            var hv = FirstNumber(Get(r, iHr));
            if (hv is not double hd) { Error($"delivery '{Get(r, iD)}': no hours"); continue; }
            double cu = FirstNumber(Get(r, iCu)) ?? double.NaN;
            hrsList.Add(hd);
            double eff = double.IsNaN(price) ? double.NaN : price / (hd / hpd);
            Console.WriteLine($"  {Clip(Get(r, iD), 8),-8}  {Num(hd),5}  {(double.IsNaN(cu) ? "?" : Num(cu)),6}  {Money(eff)}{(!double.IsNaN(floor) && eff < floor ? "  below floor" : "")}");
        }
        if (hrsList.Count < 3) { Warn($"{hrsList.Count} deliveries: too few to call it repeatable; label it beta until three"); return; }

        // Wright's learning curve: T_n = T_1 * n^(-b); fit by least squares on logs.
        var xs = Enumerable.Range(1, hrsList.Count).Select(n => Math.Log(n)).ToArray();
        var ys = hrsList.Select(v => Math.Log(v)).ToArray();
        double mx = xs.Average(), my = ys.Average();
        double sxy = xs.Zip(ys, (x, y) => (x - mx) * (y - my)).Sum(), sxx = xs.Sum(x => (x - mx) * (x - mx));
        double slope = sxy / sxx, b = -slope, rate = Math.Pow(2, -b);
        double next = Math.Exp(my + slope * (Math.Log(hrsList.Count + 1) - mx));
        var last3 = hrsList.TakeLast(5).ToArray();
        double mean3 = last3.Average(), cv = Math.Sqrt(last3.Sum(v => (v - mean3) * (v - mean3)) / (last3.Length - 1)) / mean3;
        Console.WriteLine();
        Console.WriteLine($"  learning curve: b = {b:0.000}, learning rate {Pct(rate)} per doubling of deliveries");
        Console.WriteLine($"  predicted delivery {hrsList.Count + 1}: {Num(Math.Round(next, 0))} h (plan {Num(planned)} h)");
        Console.WriteLine($"  variability of the last {last3.Length} deliveries: CV {cv:0.00}");
        if (b <= 0) Warn("hours are not falling with repetition: the work is not repeating, or the assets are not being reused");
        if (cv > 0.30) Warn($"CV {cv:0.00}: delivery hours vary too much to price one scope at one price");
        if (!double.IsNaN(price) && !double.IsNaN(floor))
        {
            double effNext = price / (next / hpd);
            if (effNext < floor) Warn($"at the predicted {Num(Math.Round(next, 0))} h the effective day rate is {Money(effNext)}, below your floor; the plan's hours are a target, not yet a fact");
        }
        if (planned > 0 && next > planned * 1.15) Warn($"plan ({Num(planned)} h) is more than 15% below the curve's prediction ({Num(Math.Round(next, 0))} h)");
    }

    // ---------------------------------------------------------------- catalog (22.2)

    static void Catalog(string path)
    {
        var lines = File.ReadAllLines(path);
        var text = string.Join("\n", lines);
        var kv = KeyValues(lines);
        var sec = Sections(lines);
        Console.WriteLine($"{Path.GetFileName(path)}: product catalog");

        if (!kv.TryGetValue("method", out var method) || !Regex.IsMatch(method, @"v?\d+\.\d+\.\d+")) Error("no 'Method:' line with a version (14.3): products must say which method version they teach");
        else Console.WriteLine($"  method: {method}");
        double cap = kv.TryGetValue("capacity", out var cs) && FirstNumber(cs) is double c ? c : double.NaN;
        if (double.IsNaN(cap)) Error("no 'Capacity:' line (days a month)");

        Promises(text, "catalog");
        foreach (var s in Sentences(text))
        {
            if (Regex.IsMatch(s, @"24/7|\banytime\b|direct access to me|unlimited (questions|access|support|calls)", I) && !Negation.IsMatch(s))
                Error($"unbounded access promise: \"{Clip(s, 90)}\"");
            if (Regex.IsMatch(s, @"\blifetime\b", I) && !Negation.IsMatch(s))
                Warn($"'lifetime': of what? state a term and what happens if you stop: \"{Clip(s, 80)}\"");
            if (Regex.IsMatch(s, @"recording of (the |a |our )?[\w' -]{0,30}(workshop|session|engagement|call)", I) && !Regex.IsMatch(s, @"rehearsal|public|consent", I))
                Error($"recording of a client delivery sold as a product: \"{Clip(s, 90)}\"");
            if (Regex.IsMatch(s, @"passive income", I))
                Warn("'passive income': products need upkeep days; budget them in the plan (22.4)");
        }
        bool certClaim = Regex.IsMatch(text, @"\bcertif(ied|ication)\b", I);
        if (certClaim && !kv.ContainsKey("cut score"))
            Error("certification claim with no 'Cut score:' line and assessment behind it (22.3); call it a certificate of completion or build the assessment");

        // products
        var prod = Tables(lines).FirstOrDefault(t => t.head.Contains("product") && t.head.Contains("price"));
        double liveLow = 0, liveHigh = 0, revLow = 0, revHigh = 0, freeLow = 0, freeHigh = 0;
        if (prod.head == null) Error("no products table (| ID | Product | Format | Price | Units/month | Live days/unit | Fixed live days/month | Licence | Evidence |)");
        else
        {
            int iId = prod.head.IndexOf("id"), iP = prod.head.IndexOf("product"), iPr = prod.head.IndexOf("price"),
                iU = prod.head.FindIndex(x => x.StartsWith("units")), iL = prod.head.FindIndex(x => x.StartsWith("live days/unit")),
                iF = prod.head.FindIndex(x => x.StartsWith("fixed live")), iLic = prod.head.FindIndex(x => x.StartsWith("licen")),
                iE = prod.head.IndexOf("evidence");
            Console.WriteLine();
            Console.WriteLine("  id   product                        revenue/month        live days/month  licence");
            foreach (var r in prod.rows)
            {
                string id = Get(r, iId), pn = Get(r, iP), lic = Dash(Get(r, iLic)), pr = Get(r, iPr);
                if (lic == "") Error($"{id} {pn}: no licence (who may use it, for what, for how long)");
                if (Dash(Get(r, iE)) == "") Warn($"{id} {pn}: no evidence ID");
                var (uLo, uHi) = Range(Get(r, iU));
                double price = Regex.IsMatch(pr, @"^\s*(free|0)\b", I) ? 0 : FirstNumber(pr) ?? double.NaN;
                if (double.IsNaN(price)) { Error($"{id} {pn}: price \"{pr}\" has no number"); continue; }
                double perUnit = FirstNumber(Get(r, iL)) ?? 0, fixedDays = FirstNumber(Get(r, iF)) ?? 0;
                double rl = price * uLo, rh = price * uHi, dl = perUnit * uLo + fixedDays, dh = perUnit * uHi + fixedDays;
                revLow += rl; revHigh += rh; liveLow += dl; liveHigh += dh;
                if (perUnit == 0) { freeLow += rl; freeHigh += rh; }
                Console.WriteLine($"  {Clip(id, 4),-4} {Clip(pn, 30),-30} {Money(rl) + "–" + Money(rh),-20} {Num(dl) + "–" + Num(dh),-16} {Clip(lic, 38)}");
                if (Regex.IsMatch(pn + " " + Get(r, 2), @"communit", I) && fixedDays == 0)
                    Warn($"{id} {pn}: a community with no fixed live days; office hours and moderation take time");
            }
            Console.WriteLine();
            Console.WriteLine($"  revenue per month: {Money(revLow)} to {Money(revHigh)}; live days: {Num(liveLow)} to {Num(liveHigh)}");
            if (liveHigh > 0) Console.WriteLine($"  revenue per live day: {Money(revLow / Math.Max(liveLow, 0.01))} to {Money(revHigh / liveHigh)}");
            if (revHigh > 0) Console.WriteLine($"  share of revenue that grows without more live days: {Pct(revLow > 0 ? freeLow / revLow : 0)} to {Pct(freeHigh / revHigh)}");
            if (!double.IsNaN(cap) && liveHigh > cap)
                Error($"at high demand the catalog needs {Num(liveHigh)} live days a month; capacity is {Num(cap)}. Say what you decline or waitlist (22.4)");
        }

        // licences
        var lic2 = Tables(lines).FirstOrDefault(t => t.head.Contains("artifact") && t.head.Any(x => x.StartsWith("licen")));
        if (lic2.head == null) Error("no licence table (| Artifact | Audience | Licence | Notes |)");
        else
        {
            int iA = lic2.head.IndexOf("artifact"), iL = lic2.head.FindIndex(x => x.StartsWith("licen")), iN = lic2.head.IndexOf("notes");
            foreach (var r in lic2.rows)
            {
                string a = Get(r, iA), l = Dash(Get(r, iL)), n = Get(r, iN), all = a + " " + l + " " + n;
                if (l == "") { Error($"licence table: '{a}' has no licence"); continue; }
                if (Regex.IsMatch(a, @"\b(code|software|script|scripts|repo|repository|hooks?|tool)\b", I) && Regex.IsMatch(l, @"\bCC[- ]BY|creative commons", I) && !Regex.IsMatch(l, @"CC0", I))
                    Error($"'{a}' is software under '{l}': Creative Commons recommends against CC licences for software; use a software licence (MIT, Apache-2.0)");
                if (Regex.IsMatch(l, @"free to use|do what(ever)? you want|no licen[cs]e|public domain", I) && !Regex.IsMatch(l, @"CC0", I))
                    Error($"'{a}': \"{l}\" is not a licence; name one (MIT, CC BY 4.0) or state your own terms");
                if (Regex.IsMatch(a + " " + l, @"\b(team|company|enterprise|corporate|site)\b", I))
                {
                    if (!Regex.IsMatch(all, @"\bseats?\b|\bup to \d+", I) || Regex.IsMatch(all, @"unlimited (seats|users)|whole company|all employees", I)) Error($"'{a}': team licence with no seat limit");
                    if (!Regex.IsMatch(all, @"\b\d+\s*(months?|years?)\b|term", I)) Error($"'{a}': team licence with no term");
                    if (!Regex.IsMatch(all, @"resal|resell|sublicen", I)) Error($"'{a}': team licence does not say whether it may be resold or sublicensed");
                    if (!Regex.IsMatch(all, @"attribut|credit", I)) Warn($"'{a}': team licence does not require attribution to be kept");
                    if (!Regex.IsMatch(all, @"update|version", I)) Warn($"'{a}': team licence does not say which versions and updates it covers");
                }
                if (Regex.IsMatch(l, @"\b(perpetual|unlimited|irrevocable)\b", I) && !Regex.IsMatch(all, @"internal|seats?", I))
                    Warn($"'{a}': \"{l}\" gives away more than you may intend; state scope and term");
            }
        }

        // third-party register (TASL)
        var tp = Tables(lines).FirstOrDefault(t => t.head.Contains("title") && t.head.Contains("author") && t.head.Contains("source"));
        if (tp.head == null) Error("no third-party register (| Item | Title | Author | Source | Licence |); write 'none' in a row if there is none");
        else
        {
            int iT = tp.head.IndexOf("title"), iAu = tp.head.IndexOf("author"), iS = tp.head.IndexOf("source"), iL = tp.head.FindIndex(x => x.StartsWith("licen"));
            foreach (var r in tp.rows)
            {
                if (Regex.IsMatch(string.Join(" ", r), @"^\s*none\b", I)) continue;
                var missing = new List<string>();
                if (Dash(Get(r, iT)) == "") missing.Add("title");
                if (Dash(Get(r, iAu)) == "" || Regex.IsMatch(Get(r, iAu), @"unknown|internet|various", I)) missing.Add("author");
                if (Dash(Get(r, iS)) == "") missing.Add("source");
                if (Dash(Get(r, iL)) == "" || Regex.IsMatch(Get(r, iL), @"unknown|found online|\?", I)) missing.Add("licence");
                if (missing.Count > 0) Error($"third-party '{Get(r, 0)}': missing {string.Join(", ", missing)} (TASL)");
            }
        }

        // community rules
        if (Regex.IsMatch(text, @"communit", I))
        {
            var cr = sec.FirstOrDefault(k => k.Key.StartsWith("community rules")).Value;
            if (cr == null) Error("a community is offered but there is no '## Community rules' section");
            else
            {
                var ckv = KeyValues(cr.Split('\n'));
                foreach (var k in new[] { "response time", "office hours", "not included", "moderation", "if it closes" })
                    if (!ckv.ContainsKey(k)) Error($"community rules: no '{char.ToUpper(k[0]) + k[1..]}:' line");
            }
        }
    }

    // ---------------------------------------------------------------- kit (22.3)

    static void Kit(string dir, string? denyPath)
    {
        var manifest = Path.Combine(dir, "kit.md");
        if (!File.Exists(manifest)) throw new UsageException($"no kit.md in {dir}");
        var lines = File.ReadAllLines(manifest);
        var kv = KeyValues(lines);
        Console.WriteLine($"{Path.GetFileName(Path.GetFullPath(dir).TrimEnd(Path.DirectorySeparatorChar))}: kit");

        var version = kv.GetValueOrDefault("version", "");
        if (!Semver(version)) Error($"kit 'Version:' \"{version}\" is not MAJOR.MINOR.PATCH");
        else Console.WriteLine($"  version: {version}");
        if (!kv.ContainsKey("method")) Warn("no 'Method:' line: which method version does the kit teach?");
        if (!kv.ContainsKey("support")) Error("no 'Support:' line (what buyers can expect, how fast, for which versions)");
        DateTime? checkedOn = kv.TryGetValue("checked", out var cd) && DateTime.TryParse(cd, Inv, DateTimeStyles.None, out var cdt) ? cdt : null;
        if (checkedOn == null) Error("no 'Checked: yyyy-mm-dd' line");

        var items = Tables(lines).FirstOrDefault(t => t.head.Contains("item") && t.head.Contains("path"));
        if (items.head == null) Error("no items table (| Item | Type | Path | Version | Licence | Last verified | Stranger test |)");
        else
        {
            int iI = items.head.IndexOf("item"), iT = items.head.IndexOf("type"), iP = items.head.IndexOf("path"), iV = items.head.IndexOf("version"),
                iL = items.head.FindIndex(x => x.StartsWith("licen")), iLv = items.head.IndexOf("last verified"), iS = items.head.IndexOf("stranger test");
            Console.WriteLine();
            Console.WriteLine("  item                         type        version  verified    stranger test (Wilson 95%)");
            foreach (var r in items.rows)
            {
                string it = Get(r, iI), type = Get(r, iT), p = Get(r, iP), v = Get(r, iV), l = Dash(Get(r, iL)), lv = Get(r, iLv), st = Get(r, iS);
                string wil = "";
                var m = Regex.Match(st, @"(\d+)\s*/\s*(\d+)");
                if (m.Success)
                {
                    int k = int.Parse(m.Groups[1].Value), n = int.Parse(m.Groups[2].Value);
                    var (lo, hi) = Wilson(k, n);
                    wil = $"{k}/{n} ({Pct(lo)} to {Pct(hi)})";
                    if (n < 5) Warn($"{it}: stranger test with {n} people; the interval is too wide to mean much");
                    else if (lo < 0.5) Warn($"{it}: stranger test lower bound {Pct(lo)}: many buyers may fail without you");
                }
                else { wil = "(none)"; Error($"{it}: no stranger test result (k/n people who succeeded alone, 15.3)"); }
                Console.WriteLine($"  {Clip(it, 28),-28} {Clip(type, 11),-11} {Clip(v, 8),-8} {Clip(lv, 10),-10}  {wil}");
                if (!File.Exists(Path.Combine(dir, p)) && !Directory.Exists(Path.Combine(dir, p))) Error($"{it}: path '{p}' does not exist in the kit");
                if (!Semver(v)) Error($"{it}: version \"{v}\" is not MAJOR.MINOR.PATCH");
                if (l == "") Error($"{it}: no licence");
                else if (Regex.IsMatch(type, @"starter|code|tool", I) && Regex.IsMatch(l, @"\bCC[- ]BY", I))
                    Error($"{it}: code under a Creative Commons licence; use a software licence");
                if (DateTime.TryParse(lv, Inv, DateTimeStyles.None, out var d) && checkedOn is DateTime ck)
                {
                    var age = (ck - d).TotalDays;
                    if (age > 180) Error($"{it}: last verified {lv}, {age:0} days before the check; re-run it against the current agent tools");
                    else if (age > 90) Warn($"{it}: last verified {lv}, {age:0} days ago (tools change quarterly)");
                }
                else Error($"{it}: no valid 'Last verified' date");
                if (Regex.IsMatch(type, @"assessment", I) && File.Exists(Path.Combine(dir, p)))
                {
                    var at = File.ReadAllText(Path.Combine(dir, p));
                    if (!Regex.IsMatch(at, @"item analysis|ScaleCheck items", I)) Warn($"{it}: no item analysis recorded (ScaleCheck items)");
                    if (Regex.IsMatch(at, @"\bcertif(ied|ication)\b", I) && !Regex.IsMatch(at, @"cut score", I))
                        Error($"{it}: certification claim with no cut score and no statement of what a pass means");
                }
            }
        }

        // changelog
        var cl = Path.Combine(dir, "CHANGELOG.md");
        if (!File.Exists(cl)) Error("no CHANGELOG.md (Keep a Changelog: buyers need to know what changed between versions)");
        else
        {
            var ct = File.ReadAllText(cl);
            var vers = Regex.Matches(ct, @"^##\s*\[(\d+\.\d+\.\d+)\]\s*-\s*(\d{4}-\d{2}-\d{2})", RegexOptions.Multiline).Select(x => x.Groups[1].Value).ToList();
            if (vers.Count == 0) Error("CHANGELOG.md has no '## [x.y.z] - yyyy-mm-dd' entries");
            else if (Semver(version) && vers[0] != version) Error($"CHANGELOG.md's latest entry is {vers[0]}, kit.md says {version}");
            else Console.WriteLine($"\n  changelog: {vers.Count} release(s), latest {vers[0]}");
        }

        // licence files
        if (!Directory.EnumerateFiles(dir, "LICENSE*", SearchOption.AllDirectories).Any())
            Error("no LICENSE file anywhere in the kit; a licence named only in kit.md does not travel with a copy");

        // deny list: employer and client terms
        if (denyPath == null) Warn("no deny list given (kit <dir> <deny.txt>): employer and client terms were not checked");
        else
        {
            var terms = File.ReadAllLines(denyPath).Select(t => t.Trim()).Where(t => t.Length > 0 && !t.StartsWith('#')).ToList();
            int hits = 0;
            foreach (var f in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories).Where(f => Regex.IsMatch(f, @"\.(md|txt|cs|sql|json|csv|ps1|sh|ya?ml)$", I)))
            {
                var fl = File.ReadAllLines(f);
                for (int i = 0; i < fl.Length; i++)
                    foreach (var t in terms)
                        if (fl[i].Contains(t, StringComparison.OrdinalIgnoreCase))
                        { Error($"{Path.GetRelativePath(dir, f).Replace('\\', '/')}:{i + 1}: deny-list term '{t}'"); hits++; }
            }
            Console.WriteLine($"  deny list: {terms.Count} term(s), {hits} hit(s)");
        }
    }

    // ---------------------------------------------------------------- items (22.3)

    static void Items(string path)
    {
        var rows = File.ReadAllLines(path).Where(l => l.Trim().Length > 0 && !l.StartsWith('#')).Select(Split).ToList();
        if (rows.Count < 2) throw new UsageException("responses.csv needs a header and at least one row");
        var head = rows[0].Select(h => h.Trim()).ToList();
        var qcols = Enumerable.Range(0, head.Count).Where(i => Regex.IsMatch(head[i], @"^q\d+$", I)).ToList();
        var data = new List<int[]>();
        foreach (var r in rows.Skip(1))
        {
            var v = qcols.Select(i => i < r.Count && r[i].Trim() == "1" ? 1 : 0).ToArray();
            data.Add(v);
        }
        int n = data.Count, g = (int)Math.Round(0.27 * n);
        Console.WriteLine($"{Path.GetFileName(path)}: item analysis, {n} learners, {qcols.Count} items, upper and lower groups of {g} (27%)");
        if (n < 20) Warn($"{n} learners: item statistics are unstable below about 20; treat them as hints");
        if (g < 1) { Error("too few learners for upper and lower groups"); return; }
        var order = data.Select((v, i) => (total: v.Sum(), i)).OrderByDescending(x => x.total).ThenBy(x => x.i).Select(x => x.i).ToList();
        var upper = order.Take(g).ToList(); var lower = order.TakeLast(g).ToList();
        Console.WriteLine();
        Console.WriteLine("  item   p (difficulty)  D (discrimination)  verdict");
        for (int q = 0; q < qcols.Count; q++)
        {
            double p = data.Average(v => v[q]);
            double pu = upper.Average(i => data[i][q]), pl = lower.Average(i => data[i][q]), d = pu - pl;
            string verdict = "ok";
            if (d < 0) { verdict = "negative D: check the key and the wording"; Error($"{head[qcols[q]]}: D = {d:0.00}; stronger learners got it wrong more often (miskeyed or misleading)"); }
            else if (p >= 0.95) { verdict = "too easy: tells you nothing"; Warn($"{head[qcols[q]]}: p = {p:0.00}; almost everyone answers it correctly"); }
            else if (p <= 0.20) { verdict = "very hard: check the key"; Warn($"{head[qcols[q]]}: p = {p:0.00}; very few answer it correctly"); }
            else if (d < 0.20) { verdict = "weak: revise or drop"; Warn($"{head[qcols[q]]}: D = {d:0.00}; does not separate stronger from weaker learners"); }
            Console.WriteLine($"  {head[qcols[q]],-5}  {p,14:0.00}  {d,18:0.00}  {verdict}");
        }
    }

    // ---------------------------------------------------------------- plan (22.4)

    static void Plan(string path)
    {
        var lines = File.ReadAllLines(path);
        var text = string.Join("\n", lines);
        var kv = KeyValues(lines);
        var sec = Sections(lines);
        Console.WriteLine($"{Path.GetFileName(path)}: productization plan");

        double cap = kv.TryGetValue("capacity", out var cs) && FirstNumber(cs) is double c ? c : double.NaN;
        if (double.IsNaN(cap)) { Error("no 'Capacity:' line (days a month you can sell, after your job)"); cap = 0; }
        else Console.WriteLine($"  capacity: {Num(cap)} days a month");
        double floor = kv.TryGetValue("floor", out var fs) && FirstNumber(fs) is double f ? f : double.NaN;
        if (double.IsNaN(floor)) Warn("no 'Floor:' day rate; retainer fees cannot be checked");
        if (!kv.ContainsKey("when full")) Error("no 'When full:' line: what happens to the next request once the cap is reached (waitlist, referral)");
        if (!kv.ContainsKey("employer")) Warn("no 'Employer:' line: does your permission (20.5) cover everything in this plan?");
        Promises(text, "plan");
        foreach (var s in Sentences(text))
        {
            if (Regex.IsMatch(s, @"passive income", I)) Warn("'passive income': products need upkeep days; budget them");
            if (Regex.IsMatch(s, @"\b(on call|24/7|whenever (they|you|needed)|unlimited|asap|same[- ]day)\b", I) && !Negation.IsMatch(s))
                Error($"unbounded commitment: \"{Clip(s, 90)}\"");
        }

        // allocation
        var alloc = Tables(lines).FirstOrDefault(t => t.head.Contains("activity") && t.head.Any(x => x.StartsWith("days")));
        var days = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        if (alloc.head == null) Error("no allocation table (| Activity | Days/month |)");
        else
        {
            int iA = alloc.head.IndexOf("activity"), iD = alloc.head.FindIndex(x => x.StartsWith("days"));
            foreach (var r in alloc.rows) days[Get(r, iA)] = FirstNumber(Get(r, iD)) ?? 0;
            double sum = days.Values.Sum();
            Console.WriteLine();
            foreach (var (a, d) in days) Console.WriteLine($"  {Clip(a, 32),-32} {Num(d),5} d");
            Console.WriteLine($"  {"total",-32} {Num(sum),5} of {Num(cap)}");
            if (sum > cap + 1e-9) Error($"allocation {Num(sum)} days exceeds capacity {Num(cap)}");
            double buffer = days.Where(x => Regex.IsMatch(x.Key, @"buffer|slack", I)).Sum(x => x.Value);
            if (buffer <= 0) Error("no buffer row: every overrun, sick day and urgent client lands on your job or your evenings");
            if (!days.Keys.Any(k => Regex.IsMatch(k, @"product|course|community|kit", I) && days[k] > 0))
                Warn("no days for products: courses, kits and communities need upkeep");
        }
        double Alloc(string pat) => days.Where(x => Regex.IsMatch(x.Key, pat, I)).Sum(x => x.Value);

        // retainers
        var ret = Tables(lines).FirstOrDefault(t => t.head.Contains("retainer"));
        if (ret.head != null)
        {
            int iR = ret.head.IndexOf("retainer"), iC = ret.head.FindIndex(x => x.StartsWith("days")), iF = ret.head.FindIndex(x => x.StartsWith("fee")),
                iRo = ret.head.IndexOf("rollover"), iN = ret.head.IndexOf("notice"), iS = ret.head.IndexOf("scope");
            double total = 0;
            Console.WriteLine();
            foreach (var r in ret.rows)
            {
                string name = Get(r, iR);
                double capd = FirstNumber(Get(r, iC)) ?? double.NaN, fee = FirstNumber(Get(r, iF)) ?? double.NaN;
                if (double.IsNaN(capd)) { Error($"retainer '{name}': no day cap"); continue; }
                total += capd;
                string ro = Dash(Get(r, iRo));
                if (ro == "" || Regex.IsMatch(ro, @"unlimited|indefinite|never expire|carr(y|ies) over forever", I))
                    Error($"retainer '{name}': rollover \"{ro}\"; unused days must expire or be capped, or they become a debt you owe");
                if (Dash(Get(r, iN)) == "") Error($"retainer '{name}': no notice period");
                if (Dash(Get(r, iS)) == "") Error($"retainer '{name}': no scope (what the days are for)");
                string perDay = double.IsNaN(fee) ? "?" : Money(fee / capd);
                Console.WriteLine($"  retainer {Clip(name, 28),-28} cap {Num(capd)} d a month, {perDay} per capped day");
                if (!double.IsNaN(fee) && !double.IsNaN(floor) && fee / capd < floor)
                    Warn($"retainer '{name}': {perDay} per capped day is below your floor {Money(floor)}");
            }
            double ra = Alloc(@"retainer");
            Console.WriteLine($"  retainer days committed: {Num(total)} of {Num(ra)} allocated");
            if (total > ra + 1e-9) Error($"retainers commit {Num(total)} days a month; the allocation has {Num(ra)}");
        }

        // queue: Kingman's approximation for the delivery "server"
        double dAlloc = Alloc(@"^deliver");
        var (lamLo, lamHi) = kv.TryGetValue("demand", out var dm) ? Range(dm) : (double.NaN, double.NaN);
        double job = kv.TryGetValue("job days", out var jd) && FirstNumber(jd) is double j ? j : double.NaN;
        double cA = kv.TryGetValue("arrival variability", out var av) && FirstNumber(av) is double a1 ? a1 : 1.0;
        double cS = kv.TryGetValue("job variability", out var jv) && FirstNumber(jv) is double s1 ? s1 : 1.0;
        if (double.IsNaN(lamLo) || double.IsNaN(job) || dAlloc <= 0)
            Error("queue: needs 'Demand: a–b engagements a month', 'Job days:' and a Delivery row in the allocation");
        else
        {
            double mu = dAlloc / job, tau = 1 / mu;
            Console.WriteLine();
            Console.WriteLine($"  delivery: {Num(dAlloc)} days a month, jobs of {Num(job)} days -> {mu:0.00} jobs a month, {tau:0.0} months each");
            foreach (var (label, lam) in new[] { ("low", lamLo), ("high", lamHi) })
            {
                double rho = lam / mu;
                if (rho >= 1) { Console.WriteLine($"  demand {label} {lam:0.##}/month: utilization {Pct(rho)}: the queue grows without limit"); if (label == "low") Error($"even at low demand utilization is {Pct(rho)}: the delivery allocation cannot keep up; add days, shorten jobs or plan to decline"); else Warn($"at high demand utilization is {Pct(rho)}: only the cap and 'When full' keep the queue finite"); continue; }
                double wq = rho / (1 - rho) * (cA * cA + cS * cS) / 2 * tau;
                Console.WriteLine($"  demand {label} {lam:0.##}/month: utilization {Pct(rho)}, expected wait to start {wq:0.0} months (Kingman, cA {cA:0.00}, cS {cS:0.00})");
                if (label == "low" && rho > 0.85) Warn($"utilization {Pct(rho)} even at low demand; waits get long and there is no room for anything to go wrong");
            }
        }

        // revenue streams at month 12
        var st = Tables(lines).FirstOrDefault(t => t.head.Contains("stream"));
        if (st.head == null) Error("no revenue streams table (| Stream | From month | Low | High | Price | Live days/unit |)");
        else
        {
            int iS = st.head.IndexOf("stream"), iFm = st.head.FindIndex(x => x.StartsWith("from")), iLo = st.head.IndexOf("low"), iHi = st.head.IndexOf("high"),
                iP = st.head.IndexOf("price"), iLd = st.head.FindIndex(x => x.StartsWith("live days"));
            double rl = 0, rh = 0, ll = 0, lh = 0, nl = 0, nh = 0;
            foreach (var r in st.rows)
            {
                double from = FirstNumber(Get(r, iFm)) ?? 1;
                if (from > 12) continue;
                double lo = FirstNumber(Get(r, iLo)) ?? 0, hi = FirstNumber(Get(r, iHi)) ?? 0, pr = FirstNumber(Get(r, iP)) ?? 0, ld = FirstNumber(Get(r, iLd)) ?? 0;
                if (lo > hi) Error($"stream '{Get(r, iS)}': low {lo} above high {hi}");
                if (lo == hi && pr > 0) Warn($"stream '{Get(r, iS)}': low equals high; a forecast is a range");
                rl += lo * pr; rh += hi * pr; ll += lo * ld; lh += hi * ld;
                if (ld == 0) { nl += lo * pr; nh += hi * pr; }
            }
            Console.WriteLine();
            Console.WriteLine($"  month 12 revenue: {Money(rl)} to {Money(rh)} a month; live days needed {Num(ll)} to {Num(lh)}");
            Console.WriteLine($"  not tied to live days: {Money(nl)} to {Money(nh)} ({Pct(rl > 0 ? nl / rl : 0)} to {Pct(rh > 0 ? nh / rh : 0)})");
            double live = Alloc(@"^deliver") + Alloc(@"retainer") + Alloc(@"workshop");
            if (lh > cap) Error($"high scenario needs {Num(lh)} live days a month; capacity is {Num(cap)}. Lower the high case or say what you decline");
            else if (live > 0 && lh > live) Warn($"high scenario needs {Num(lh)} live days; the allocation gives delivery and retainers {Num(live)}");
        }

        // roadmap
        var rm = Tables(lines).FirstOrDefault(t => t.head.Contains("month") && t.head.Contains("milestone"));
        if (rm.head == null) Error("no roadmap table (| Month | Milestone | Exit signal | Evidence |)");
        else
        {
            int iM = rm.head.IndexOf("month"), iMs = rm.head.IndexOf("milestone"), iX = rm.head.IndexOf("exit signal"), iE = rm.head.IndexOf("evidence");
            double maxMonth = 0;
            foreach (var r in rm.rows)
            {
                var nums = Regex.Matches(Get(r, iM), @"\d+").Select(x => double.Parse(x.Value, Inv)).ToList();
                if (nums.Count > 0) maxMonth = Math.Max(maxMonth, nums.Max());
                string x = Dash(Get(r, iX));
                if (x == "") Error($"roadmap month {Get(r, iM)}: '{Clip(Get(r, iMs), 40)}' has no exit signal");
                else if (!Regex.IsMatch(x, @"\d") && !Regex.IsMatch(x, @"clean|passes|signed|published|delivered|approv|written", I))
                    Warn($"roadmap month {Get(r, iM)}: exit signal \"{Clip(x, 50)}\" has no number or check");
                if (iE >= 0 && Dash(Get(r, iE)) == "") Warn($"roadmap month {Get(r, iM)}: no evidence file named");
            }
            Console.WriteLine($"\n  roadmap: {rm.rows.Count} rows, up to month {Num(maxMonth)}");
            if (maxMonth < 12) Error($"roadmap ends at month {Num(maxMonth)}; the plan covers 12 months");
        }

        var stop = sec.FirstOrDefault(k => k.Key.StartsWith("stop rules")).Value;
        if (stop == null || !stop.Split('\n').Any(l => l.TrimStart().StartsWith("- ") && Regex.IsMatch(l, @"\d")))
            Error("no '## Stop rules' with a number: when do you stop a product that is not working?");
    }

    // ---------------------------------------------------------------- helpers

    static (double lo, double hi) Wilson(int k, int n)
    {
        if (n == 0) return (0, 1);
        double z = 1.959964, p = (double)k / n, z2 = z * z;
        double centre = (p + z2 / (2 * n)) / (1 + z2 / n);
        double half = z * Math.Sqrt(p * (1 - p) / n + z2 / (4.0 * n * n)) / (1 + z2 / n);
        return (Math.Max(0, centre - half), Math.Min(1, centre + half));
    }

    static string Pct(double v) => double.IsNaN(v) ? "?" : (v * 100).ToString("0", Inv) + "%";

    static bool Semver(string v) => Regex.IsMatch(v.Trim(), @"^v?\d+\.\d+\.\d+$");

    static (double lo, double hi) Range(string s)
    {
        var m = Regex.Matches(s, @"\d[\d,]*(?:\.\d+)?").Select(x => double.Parse(x.Value.Replace(",", ""), Inv)).ToList();
        if (m.Count == 0) return (0, 0);
        return m.Count == 1 ? (m[0], m[0]) : (m[0], m[1]);
    }

    static string Get(List<string> r, int i) => i >= 0 && i < r.Count ? r[i] : "";

    static Dictionary<string, string> KeyValues(string[] lines)
    {
        var d = new Dictionary<string, string>();
        bool fence = false;
        foreach (var l in lines)
        {
            if (l.TrimStart().StartsWith("```")) { fence = !fence; continue; }
            if (fence || l.TrimStart().StartsWith('|') || l.TrimStart().StartsWith('#') || l.TrimStart().StartsWith('>')) continue;
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

    static string Money(double v) => double.IsNaN(v) || double.IsInfinity(v) ? "?" : (v < 0 ? "-" : "") + Math.Abs(Math.Round(v, 0)).ToString("#,0", Inv);
    static string Num(double v) => double.IsNaN(v) ? "?" : Math.Abs(v) >= 100 ? v.ToString("#,0", Inv) : v.ToString("0.##", Inv);
    static string Dash(string s) => s.Trim() is "-" or "—" or "–" ? "" : s.Trim();
    static string Clip(string s, int n) => s.Length <= n ? s : s[..(n - 3)] + "...";
    static void Error(string m) { errors++; Console.WriteLine($"ERROR {m}"); }
    static void Warn(string m) { warnings++; Console.WriteLine($"WARN  {m}"); }
    static string NeedFile(string f) => File.Exists(f) ? f : throw new UsageException($"file not found: {f}");
    static string NeedDir(string d) => Directory.Exists(d) ? d : throw new UsageException($"directory not found: {d}");
}

sealed class UsageException(string message) : Exception(message);
