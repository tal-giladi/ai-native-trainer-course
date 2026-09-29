// MethodCheck — the Module 14 tool. Dependency-free, read-only, deterministic.
//
//   tooldep  <method.md> --tools <tool-terms.txt> [--max 0.2]
//            Share of sentences that only make sense for one tool (14.1, the tool-change test).
//   trace    <concepts.md>
//            Every concept card has its fields, a boundary, a dated evidence trail whose links resolve,
//            enough evidence for its status, and no number without an interval (14.2).
//   diff     <old-method.md> <new-method.md>
//            Classifies the changes between two method versions and checks the version bump,
//            the changelog entry and name reuse (14.3).
//   borrowed <draft.md> --terms <terms.csv> [--audit <attribution-audit.md>]
//            Finds terms borrowed from public frameworks and checks each has a decision:
//            attribute, replace or justify (14.4).
//
// Exit code: 0 = no errors, 1 = errors found, 2 = usage or input problem.
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

static class Program
{
    static int errors, warnings;

    static int Main(string[] args)
    {
        if (args.Length == 0 || args[0] is "-h" or "--help") { Usage(); return 0; }
        try
        {
            var (pos, opt) = Parse(args.Skip(1).ToArray());
            switch (args[0])
            {
                case "tooldep": ToolDep(Need(pos, 0), Opt(opt, "tools"), double.Parse(opt.GetValueOrDefault("max", "0.2"), CultureInfo.InvariantCulture)); break;
                case "trace": Trace(Need(pos, 0)); break;
                case "diff": Diff(Need(pos, 0), Need(pos, 1)); break;
                case "borrowed": Borrowed(Need(pos, 0), Opt(opt, "terms"), opt.GetValueOrDefault("audit")); break;
                default: Console.Error.WriteLine($"unknown command '{args[0]}'"); Usage(); return 2;
            }
        }
        catch (UsageException e) { Console.Error.WriteLine(e.Message); return 2; }
        catch (IOException e) { Console.Error.WriteLine(e.Message); return 2; }
        Console.WriteLine();
        Console.WriteLine($"{errors} error(s), {warnings} warning(s)");
        return errors > 0 ? 1 : 0;
    }

    // ---------------------------------------------------------------- tooldep

    static void ToolDep(string path, string toolsPath, double max)
    {
        var terms = File.ReadAllLines(toolsPath).Select(l => l.Trim())
            .Where(l => l.Length > 0 && !l.StartsWith('#')).ToList();
        var sentences = Sentences(ProseLines(File.ReadAllLines(path)));
        var hits = new List<(string s, string term)>();
        foreach (var s in sentences)
        {
            var t = terms.FirstOrDefault(term => ContainsTerm(s, term));
            if (t != null) hits.Add((s, t));
        }
        double share = sentences.Count == 0 ? 0 : (double)hits.Count / sentences.Count;
        Console.WriteLine($"{Path.GetFileName(path)}: {sentences.Count} sentences, {hits.Count} tool-dependent ({share:P0}), limit {max:P0}");
        Console.WriteLine();
        foreach (var (s, term) in hits)
            Console.WriteLine($"  [{term}] {Clip(s, 90)}");
        if (share > max) Error($"{share:P0} of sentences depend on a specific tool (limit {max:P0}): the method will not survive a tool change");
    }

    static bool ContainsTerm(string text, string term)
    {
        bool wordish = Regex.IsMatch(term, @"^[\w ]+$");
        return wordish
            ? Regex.IsMatch(text, @"\b" + Regex.Escape(term) + @"\b", RegexOptions.IgnoreCase)
            : text.Contains(term, StringComparison.OrdinalIgnoreCase);
    }

    static List<string> ProseLines(string[] lines)
    {
        var result = new List<string>();
        bool fence = false;
        foreach (var raw in lines)
        {
            var l = raw.Trim();
            if (l.StartsWith("```") || l.StartsWith("~~~")) { fence = !fence; continue; }
            if (fence || l.Length == 0 || l.StartsWith('#') || l.StartsWith('|') || l.StartsWith('>')) continue;
            result.Add(Regex.Replace(l, @"^([-*]|\d+\.)\s+", ""));
        }
        return result;
    }

    static List<string> Sentences(List<string> lines) =>
        lines.SelectMany(l => Regex.Split(l, @"(?<=[.!?])\s+(?=[A-Z`""(])"))
             .Select(s => s.Trim()).Where(s => Regex.IsMatch(s, @"\w{2,}")).ToList();

    // ---------------------------------------------------------------- trace

    static readonly string[] Required = { "Status", "Named", "Plain words", "Claim", "Boundary", "Evidence" };
    static readonly string[] Jargon = { "token", "context window", "subagent", "sub-agent", "MCP", "LLM", "hook", "prompt", "embedding", "regression task" };
    static readonly Regex Number = new(@"\d+(\.\d+)?\s?%|\b\d+(\.\d+)?\s?[x×]\b|\b\d+(\.\d+)?×", RegexOptions.Compiled);

    record Evidence(DateOnly Date, string Id, string Text, string? Link);
    record Card(string Id, string Name, Dictionary<string, string> Fields, List<Evidence> Evidence, int Line);

    static void Trace(string path)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path))!;
        var cards = ParseCards(File.ReadAllLines(path));
        if (cards.Count == 0) { Error("no concept cards found (expected '## C1 · Name' headings)"); return; }
        foreach (var dup in cards.GroupBy(c => c.Name.ToLowerInvariant()).Where(g => g.Count() > 1))
            Error($"name '{dup.First().Name}' is used by {string.Join(", ", dup.Select(c => c.Id))}");
        foreach (var dup in cards.GroupBy(c => c.Id).Where(g => g.Count() > 1))
            Error($"id {dup.Key} is used twice");

        foreach (var c in cards)
        {
            var status = c.Fields.GetValueOrDefault("Status", "").ToLowerInvariant();
            int dates = c.Evidence.Select(e => e.Date).Distinct().Count();
            int exps = c.Evidence.Count(e => e.Id.StartsWith("EXP-"));
            Console.WriteLine($"{c.Id} · {c.Name}  [{(status.Length > 0 ? status : "?")}]  evidence {c.Evidence.Count} (dates {dates}, experiments {exps})");
            string p = $"  {c.Id}: ";

            foreach (var f in Required.Where(f => !c.Fields.ContainsKey(f)))
                Error(p + $"missing field '{f}'");
            if (status is not ("hypothesis" or "supported" or "retired"))
                Error(p + $"status '{status}' must be hypothesis, supported or retired");
            if (status == "retired" && !c.Fields.ContainsKey("Retired"))
                Error(p + "retired concepts need a 'Retired:' line (date and reason)");

            if (c.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length > 4)
                Warn(p + "name longer than four words; it will not survive conversation");

            var plain = c.Fields.GetValueOrDefault("Plain words", "");
            int plainWords = plain.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            if (plainWords > 30) Warn(p + $"plain-words definition has {plainWords} words (aim for 30 or fewer)");
            var jargon = Jargon.Where(j => ContainsTerm(plain, j)).ToList();
            if (plain.Contains('`') || jargon.Count > 0)
                Warn(p + $"plain-words definition uses engineering jargon ({string.Join(", ", jargon.DefaultIfEmpty("code"))}); a non-engineer must be able to paraphrase it");

            var boundary = c.Fields.GetValueOrDefault("Boundary", "").Trim().TrimEnd('.').ToLowerInvariant();
            if (c.Fields.ContainsKey("Boundary") && boundary is "" or "none" or "always" or "n/a" or "-" or "—")
                Error(p + "no boundary: a concept that applies everywhere is a slogan, not a claim");

            foreach (var f in new[] { "Claim", "Plain words" })
            {
                var v = c.Fields.GetValueOrDefault(f, "");
                if (Number.IsMatch(v) && (!Regex.IsMatch(v, @"\bCI\b|interval") || exps == 0))
                    Error(p + $"'{f}' states a number ({Number.Match(v).Value.Trim()}) without an interval from an experiment (EXP-) in the evidence");
            }

            if (status == "hypothesis" && c.Evidence.Count == 0)
                Error(p + "even a hypothesis needs one dated incident");
            if (status == "supported" && !(exps > 0 || (c.Evidence.Count >= 3 && dates >= 2)))
                Error(p + $"'supported' needs 3+ incidents on 2+ dates or an experiment; has {c.Evidence.Count} on {dates} date(s): demote to hypothesis or find evidence");

            if (DateOnly.TryParse(c.Fields.GetValueOrDefault("Named", ""), CultureInfo.InvariantCulture, out var named))
            {
                if (c.Evidence.Count > 0 && c.Evidence.All(e => e.Date > named))
                    Error(p + $"named on {named:yyyy-MM-dd}, before any of its evidence: the name came first and the evidence was collected to fit it");
            }
            else if (c.Fields.ContainsKey("Named")) Error(p + "'Named' must be a date (yyyy-MM-dd)");

            foreach (var e in c.Evidence)
            {
                if (e.Link is null) { Error(p + $"{e.Id} has no link to its source"); continue; }
                var file = Path.GetFullPath(Path.Combine(dir, e.Link.Split('#')[0]));
                if (!File.Exists(file)) { Error(p + $"{e.Id} links to a missing file: {e.Link}"); continue; }
                if (!File.ReadAllText(file).Contains(e.Id, StringComparison.OrdinalIgnoreCase))
                    Error(p + $"{e.Id} does not appear in {e.Link.Split('#')[0]}");
            }
        }
    }

    static List<Card> ParseCards(string[] lines)
    {
        var cards = new List<Card>();
        Card? cur = null;
        bool inEvidence = false;
        var head = new Regex(@"^##\s+(C\d+)\s*·\s*(.+?)\s*$");
        var field = new Regex(@"^-\s+(?:\*\*)?([A-Za-z -]+?):(?:\*\*)?\s*(.*)$");
        var ev = new Regex(@"^\s+[-*]\s+(\d{4}-\d{2}-\d{2})\s*·\s*([A-Z]+-\d+)\s*·\s*(.*)$");
        for (int i = 0; i < lines.Length; i++)
        {
            var l = lines[i];
            var h = head.Match(l);
            if (h.Success) { cur = new Card(h.Groups[1].Value, h.Groups[2].Value, new(), new(), i + 1); cards.Add(cur); inEvidence = false; continue; }
            if (l.StartsWith("## ")) { cur = null; continue; }
            if (cur is null) continue;
            var e = ev.Match(l);
            if (inEvidence && e.Success)
            {
                var link = Regex.Match(e.Groups[3].Value, @"\]\(([^)\s]+)\)");
                cur.Evidence.Add(new Evidence(DateOnly.ParseExact(e.Groups[1].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    e.Groups[2].Value, e.Groups[3].Value, link.Success ? link.Groups[1].Value : null));
                continue;
            }
            var f = field.Match(l);
            if (f.Success)
            {
                cur.Fields[f.Groups[1].Value.Trim()] = f.Groups[2].Value.Trim();
                inEvidence = f.Groups[1].Value.Trim() == "Evidence";
            }
        }
        return cards;
    }

    // ---------------------------------------------------------------- diff

    record Concept(string Id, string Name, string Status, string? Formerly);
    record Method(string Version, List<string> Loop, Dictionary<string, Concept> Concepts, HashSet<string> ChangelogVersions, string Text);

    static Method ParseMethod(string path)
    {
        var lines = File.ReadAllLines(path);
        string version = "", section = "";
        var loop = new List<string>();
        var concepts = new Dictionary<string, Concept>();
        var log = new HashSet<string>();
        foreach (var l in lines)
        {
            var v = Regex.Match(l, @"^Version:\s*(\d+\.\d+\.\d+)");
            if (v.Success) version = v.Groups[1].Value;
            if (l.StartsWith("## ")) { section = l[3..].Trim().ToLowerInvariant(); continue; }
            if (section == "loop")
            {
                var m = Regex.Match(l, @"^\d+\.\s+\*\*(.+?)\*\*");
                if (m.Success) loop.Add(m.Groups[1].Value.Trim());
            }
            else if (section == "concepts")
            {
                var m = Regex.Match(l, @"^-\s+\*\*(C\d+)\s*·\s*(.+?)\*\*\s*\(([^)]*)\)");
                if (m.Success)
                {
                    var inside = m.Groups[3].Value;
                    var status = inside.Split(',')[0].Trim().ToLowerInvariant();
                    var formerly = Regex.Match(inside, @"formerly\s+""([^""]+)""");
                    concepts[m.Groups[1].Value] = new Concept(m.Groups[1].Value, m.Groups[2].Value.Trim(), status, formerly.Success ? formerly.Groups[1].Value : null);
                }
            }
            else if (section == "changelog")
            {
                var m = Regex.Match(l, @"^###\s+(\d+\.\d+\.\d+)");
                if (m.Success) log.Add(m.Groups[1].Value);
            }
        }
        if (version == "") throw new UsageException($"{path}: no 'Version: x.y.z' line");
        return new Method(version, loop, concepts, log, string.Join("\n", lines.Where(x => !x.StartsWith("Version:"))));
    }

    static void Diff(string oldPath, string newPath)
    {
        var a = ParseMethod(oldPath);
        var b = ParseMethod(newPath);
        int required = 0; // 0 none, 1 patch, 2 minor, 3 major
        var changes = new List<(int level, string what)>();
        var reuse = new List<string>();
        void Change(int level, string what) { changes.Add((level, what)); required = Math.Max(required, level); }

        if (!a.Loop.SequenceEqual(b.Loop))
            Change(3, $"loop changed: {string.Join(" → ", a.Loop)}  ⇒  {string.Join(" → ", b.Loop)}");

        foreach (var (id, oc) in a.Concepts)
        {
            if (!b.Concepts.TryGetValue(id, out var nc)) { Change(3, $"{id} '{oc.Name}' removed (retire it instead of deleting it)"); continue; }
            if (!string.Equals(oc.Name, nc.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(nc.Formerly, oc.Name, StringComparison.OrdinalIgnoreCase))
                    Change(2, $"{id} renamed '{oc.Name}' → '{nc.Name}' with the old name kept as an alias");
                else
                    Change(3, $"{id} renamed '{oc.Name}' → '{nc.Name}' with no alias: old slides, posts and notes now point at nothing");
            }
            if (oc.Status != nc.Status)
                Change(nc.Status == "retired" ? 3 : 2, $"{id} status {oc.Status} → {nc.Status}");
        }
        foreach (var (id, nc) in b.Concepts.Where(kv => !a.Concepts.ContainsKey(kv.Key)))
        {
            Change(2, $"{id} '{nc.Name}' added ({nc.Status})");
            var clash = a.Concepts.Values.FirstOrDefault(oc =>
                string.Equals(oc.Name, nc.Name, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(oc.Formerly, nc.Name, StringComparison.OrdinalIgnoreCase));
            if (clash != null)
                reuse.Add($"{id} reuses the name '{nc.Name}', which meant {clash.Id} in {a.Version}: never give an old name a new meaning");
        }
        if (required == 0 && a.Text != b.Text) Change(1, "wording changed");

        Console.WriteLine($"{Path.GetFileName(oldPath)} ({a.Version}) → {Path.GetFileName(newPath)} ({b.Version})");
        Console.WriteLine();
        string[] label = { "none", "PATCH", "MINOR", "MAJOR" };
        foreach (var (level, what) in changes) Console.WriteLine($"  {label[level],-5}  {what}");
        if (changes.Count == 0) Console.WriteLine("  no changes");
        Console.WriteLine();
        foreach (var r in reuse) Error(r);

        int actual = Bump(a.Version, b.Version);
        Console.WriteLine($"required bump: {label[required]}   actual bump: {(actual < 0 ? "backwards" : label[actual])}");
        if (actual < 0) Error($"version went backwards ({a.Version} → {b.Version})");
        else if (actual < required) Error($"changes need a {label[required]} bump but the version is a {label[actual]} bump: anyone teaching {a.Version} will not know their material broke");
        if (required > 0 && !b.ChangelogVersions.Contains(b.Version))
            Error($"no changelog entry '### {b.Version}' in {Path.GetFileName(newPath)}");
    }

    static int Bump(string from, string to)
    {
        var f = from.Split('.').Select(int.Parse).ToArray();
        var t = to.Split('.').Select(int.Parse).ToArray();
        for (int i = 0; i < 3; i++)
        {
            if (t[i] > f[i]) return 3 - i;
            if (t[i] < f[i]) return -1;
        }
        return 0;
    }

    // ---------------------------------------------------------------- borrowed

    record Term(string[] Variants, string Framework, string Source, string Url, string Note);
    record Decision(string Term, string Framework, string Choice, string Note);

    static void Borrowed(string draftPath, string termsPath, string? auditPath)
    {
        var terms = ReadCsv(termsPath).Skip(1).Where(r => r.Length >= 4 && r[0].Trim().Length > 0)
            .Select(r => new Term(r[0].Split('|').Select(s => s.Trim()).ToArray(), r[1].Trim(), r[2].Trim(), r[3].Trim(), r.Length > 4 ? r[4].Trim() : "")).ToList();
        var draftLines = File.ReadAllLines(draftPath);
        var normLines = new List<(int no, string text)>();
        bool fence = false;
        for (int i = 0; i < draftLines.Length; i++)
        {
            var t = draftLines[i].Trim();
            if (t.StartsWith("```") || t.StartsWith("~~~")) { fence = !fence; continue; }
            if (!fence) normLines.Add((i + 1, Norm(draftLines[i])));
        }
        var whole = Norm(string.Join("\n", draftLines));
        var audit = auditPath is null ? new List<Decision>() : ReadAudit(auditPath);

        int found = 0, resolved = 0;
        Console.WriteLine($"{Path.GetFileName(draftPath)} against {terms.Count} terms from {terms.Select(t => t.Framework).Distinct().Count()} frameworks");
        Console.WriteLine();
        foreach (var term in terms)
        {
            var hits = normLines.Where(l => term.Variants.Any(v => Has(l.text, Norm(v)))).Select(l => l.no).ToList();
            var dec = audit.FirstOrDefault(d => term.Variants.Any(v => Norm(v) == Norm(d.Term)));
            if (hits.Count == 0)
            {
                if (dec is { Choice: "replace" }) { Console.WriteLine($"  replaced   {term.Variants[0]}  ({term.Framework})"); }
                continue;
            }
            found++;
            string where = "lines " + string.Join(",", hits.Take(6)) + (hits.Count > 6 ? ",…" : "");
            if (dec is null)
            {
                Console.WriteLine($"  BORROWED   {term.Variants[0]}  ({term.Framework}, {where})");
                Error($"'{term.Variants[0]}' comes from {term.Framework} ({term.Url}) and has no decision: attribute, replace or justify");
                continue;
            }
            switch (dec.Choice)
            {
                case "attribute":
                    bool credited = whole.Contains(Norm(term.Framework)) || whole.Contains(term.Url.ToLowerInvariant()) ||
                                    (term.Source.Length > 0 && whole.Contains(Norm(term.Source)));
                    if (credited) { resolved++; Console.WriteLine($"  attributed {term.Variants[0]}  ({term.Framework})"); }
                    else Error($"'{term.Variants[0]}' is marked 'attribute' but the draft never credits {term.Framework}");
                    break;
                case "replace":
                    Error($"'{term.Variants[0]}' is marked 'replace' but is still used ({where})");
                    break;
                case "justify":
                    if (dec.Note.Length >= 20) { resolved++; Console.WriteLine($"  justified  {term.Variants[0]}  ({term.Framework}): {Clip(dec.Note, 60)}"); }
                    else Error($"'{term.Variants[0]}' is marked 'justify' without a reason (20+ characters)");
                    break;
                default:
                    Error($"'{term.Variants[0]}': decision '{dec.Choice}' must be attribute, replace or justify");
                    break;
            }
        }
        Console.WriteLine();
        Console.WriteLine($"{found} borrowed term(s) found, {resolved} resolved, {found - resolved} open");
    }

    static string Norm(string s)
    {
        s = s.ToLowerInvariant().Replace("→", "->").Replace("—", "-").Replace("–", "-").Replace("*", "").Replace("`", "");
        s = Regex.Replace(s, @"\s*->\s*", " -> ");
        return Regex.Replace(s, @"[ \t]+", " ");
    }

    static bool Has(string text, string term) =>
        Regex.IsMatch(text, @"(?<![\w-])" + Regex.Escape(term) + @"(?![\w-])");

    static List<Decision> ReadAudit(string path)
    {
        var list = new List<Decision>();
        foreach (var l in File.ReadAllLines(path))
        {
            if (!l.TrimStart().StartsWith('|')) continue;
            var cells = l.Trim().Trim('|').Split('|').Select(c => c.Trim()).ToArray();
            if (cells.Length < 4 || cells[0].StartsWith("---") || cells[0].Equals("Term", StringComparison.OrdinalIgnoreCase) || cells[0].Length == 0) continue;
            list.Add(new Decision(cells[0].Trim('`', '*'), cells[1], cells[2].ToLowerInvariant().Trim('*'), cells[3]));
        }
        return list;
    }

    static List<string[]> ReadCsv(string path)
    {
        var rows = new List<string[]>();
        foreach (var line in File.ReadAllLines(path))
        {
            if (line.Trim().Length == 0 || line.TrimStart().StartsWith('#')) continue;
            var cells = new List<string>();
            var sb = new StringBuilder();
            bool q = false;
            for (int i = 0; i < line.Length; i++)
            {
                char ch = line[i];
                if (ch == '"') { if (q && i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; } else q = !q; }
                else if (ch == ',' && !q) { cells.Add(sb.ToString()); sb.Clear(); }
                else sb.Append(ch);
            }
            cells.Add(sb.ToString());
            rows.Add(cells.ToArray());
        }
        return rows;
    }

    // ---------------------------------------------------------------- helpers

    static void Error(string msg) { errors++; Console.WriteLine("ERROR " + msg); }
    static void Warn(string msg) { warnings++; Console.WriteLine("WARN  " + msg); }
    static string Clip(string s, int n) => s.Length <= n ? s : s[..(n - 1)] + "…";

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
        return (pos, opt);
    }

    static void Usage() => Console.WriteLine("""
        MethodCheck — Module 14 checks for method-notes (read-only)

          tooldep  <method.md> --tools <tool-terms.txt> [--max 0.2]
          trace    <concepts.md>
          diff     <old-method.md> <new-method.md>
          borrowed <draft.md> --terms <terms.csv> [--audit <attribution-audit.md>]

        Exit code 0 = clean, 1 = errors, 2 = usage.
        """);

    sealed class UsageException(string m) : Exception(m);
}
