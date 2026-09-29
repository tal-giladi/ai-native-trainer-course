// SkillCheck - Module 6 lab helper: static and output checks for skills and sub-agents.
//   lint      <path>...                         SKILL.md / agent files: spec fields, course contract, links, versioning
//   inventory <repo> [--overlap 0.35]           every skill and agent in the repo: invocation, size, version, overlaps
//   contract  <file>... --rules <rules-file>    does a skill's output file satisfy the skill's output contract?
//   triggers  <runs.tsv> [--min-recall 0.9] [--max-false 0.1]   trigger recall, false-trigger rate, precision
// No dependencies. It never modifies anything.
using System.Globalization;
using System.Text.RegularExpressions;

Console.OutputEncoding = System.Text.Encoding.UTF8;
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
if (args.Length == 0) return Usage();
var named = new Dictionary<string, string>();
var pos = new List<string>();
for (var i = 1; i < args.Length; i++)
{
    if (args[i].StartsWith("--") && i + 1 < args.Length) named[args[i]] = args[++i];
    else pos.Add(args[i]);
}
try
{
    return args[0] switch
    {
        "lint" when pos.Count > 0 => Lint.Run(pos),
        "inventory" when pos.Count == 1 => Inventory.Run(pos[0], Num(named, "--overlap", 0.35)),
        "contract" when pos.Count > 0 && named.ContainsKey("--rules") => Contract.Run(pos, named["--rules"]),
        "triggers" when pos.Count == 1 => Triggers.Run(pos[0], Num(named, "--min-recall", 0.9), Num(named, "--max-false", 0.1)),
        _ => Usage()
    };
}
catch (Exception e) when (e is ArgumentException or IOException or FormatException)
{
    Console.Error.WriteLine("error: " + e.Message);
    return 2;
}

static double Num(Dictionary<string, string> d, string k, double def) =>
    d.TryGetValue(k, out var v) ? double.Parse(v, CultureInfo.InvariantCulture) : def;

static int Usage()
{
    Console.Error.WriteLine("""
        usage: SkillCheck lint <skill-dir | SKILL.md | agent.md | folder>...
               SkillCheck inventory <repo> [--overlap 0.35]
               SkillCheck contract <file>... --rules <contract.rules>
               SkillCheck triggers <runs.tsv> [--min-recall 0.9] [--max-false 0.1]
        """);
    return 2;
}

/// <summary>A markdown file with YAML front-matter (the small subset skills and agents use).</summary>
sealed class Doc
{
    public string Path = "";
    public bool HasFrontMatter;
    public Dictionary<string, string> Fields = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> Metadata = new(StringComparer.OrdinalIgnoreCase);
    public List<string> Body = new();

    public static Doc Load(string path)
    {
        var d = new Doc { Path = path };
        var lines = File.ReadAllLines(path);
        if (lines.Length == 0 || lines[0].Trim() != "---") { d.Body = lines.ToList(); return d; }
        var end = Array.FindIndex(lines, 1, l => l.Trim() == "---");
        if (end < 0) { d.Body = lines.ToList(); return d; }
        d.HasFrontMatter = true;
        string? key = null;
        var block = new List<string>();
        void Flush()
        {
            if (key is null) return;
            if (block.Count > 0)
            {
                if (key.Equals("metadata", StringComparison.OrdinalIgnoreCase))
                    foreach (var b in block)
                    {
                        var m = Regex.Match(b, @"^\s+([\w.-]+):\s*(.*)$");
                        if (m.Success) d.Metadata[m.Groups[1].Value] = Unquote(m.Groups[2].Value);
                    }
                else if (block.All(b => b.TrimStart().StartsWith("- ")))
                    d.Fields[key] = string.Join(", ", block.Select(b => Unquote(b.TrimStart()[2..])));
                else d.Fields[key] = string.Join(" ", block.Select(b => b.Trim()));
            }
            key = null; block.Clear();
        }
        for (var i = 1; i < end; i++)
        {
            var line = lines[i];
            if (line.Trim().Length == 0 || line.TrimStart().StartsWith('#')) continue;
            var top = Regex.Match(line, @"^([A-Za-z][\w-]*):\s*(.*)$");
            if (top.Success)
            {
                Flush();
                var v = top.Groups[2].Value.Trim();
                if (v is "" or ">" or ">-" or "|" or "|-") key = top.Groups[1].Value;
                else d.Fields[top.Groups[1].Value] = Unquote(v);
            }
            else if (key is not null) block.Add(line);
        }
        Flush();
        d.Body = lines.Skip(end + 1).ToList();
        return d;
    }

    static string Unquote(string v)
    {
        v = v.Trim();
        return v.Length >= 2 && (v[0] == '"' && v[^1] == '"' || v[0] == '\'' && v[^1] == '\'') ? v[1..^1] : v;
    }

    public string Get(string k) => Fields.TryGetValue(k, out var v) ? v : "";
    public bool IsTrue(string k) => Get(k).ToLowerInvariant() is "true" or "yes" or "on" or "1";
    public bool HasHeading(string text) =>
        Body.Any(l => Regex.IsMatch(l, @"^#{2,3}\s+" + Regex.Escape(text) + @"\s*$", RegexOptions.IgnoreCase));
}

static class Lint
{
    static readonly Regex NameRx = new(@"^[a-z0-9]+(-[a-z0-9]+)*$");
    // A description must say when to use the skill, not only what it does (course rule; the spec recommends it).
    static readonly Regex WhenRx = new(@"\b(use|invoke|run)\s+(it\s+|this\s+)?(when|for|after|before|to|whenever|on)\b|\bwhen (the user|a user|you|a |an |the )", RegexOptions.IgnoreCase);
    static readonly Regex SideEffectRx = new(@"git (push|commit|merge)|\bdeploy\b|DROP (TABLE|DATABASE)|dotnet ef database update", RegexOptions.IgnoreCase);

    public static int Run(List<string> paths)
    {
        var files = new List<(string File, bool Skill)>();
        foreach (var p in paths)
        {
            if (File.Exists(p)) files.Add((p, System.IO.Path.GetFileName(p) == "SKILL.md"));
            else if (File.Exists(System.IO.Path.Combine(p, "SKILL.md"))) files.Add((System.IO.Path.Combine(p, "SKILL.md"), true));
            else if (Directory.Exists(p))
            {
                files.AddRange(Directory.EnumerateFiles(p, "SKILL.md", SearchOption.AllDirectories).Select(f => (f, true)));
                files.AddRange(Directory.EnumerateFiles(p, "*.md", SearchOption.AllDirectories)
                    .Where(f => System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(f)!) == "agents").Select(f => (f, false)));
            }
            else throw new ArgumentException($"not found: {p}");
        }
        if (files.Count == 0) throw new ArgumentException("no SKILL.md or agents/*.md found");
        var failed = 0;
        foreach (var (f, skill) in files.OrderBy(x => x.File))
        {
            var d = Doc.Load(f);
            var (errors, warnings) = skill ? Skill(d) : Agent(d);
            var label = skill ? $"skill {System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(f))}" : $"agent {System.IO.Path.GetFileNameWithoutExtension(f)}";
            foreach (var w in warnings) Console.WriteLine($"  WARN [{label}] {w}");
            foreach (var e in errors) Console.WriteLine($"  FAIL [{label}] {e}");
            Console.WriteLine(errors.Count == 0 ? $"PASS {label}" : $"FAIL {label}: {errors.Count} problem(s)");
            if (errors.Count > 0) failed++;
        }
        return failed == 0 ? 0 : 1;
    }

    static (List<string>, List<string>) Skill(Doc d)
    {
        var e = new List<string>(); var w = new List<string>();
        var dir = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(d.Path))!;
        if (!d.HasFrontMatter) { e.Add("[spec] no YAML front-matter"); return (e, w); }
        var name = d.Get("name");
        if (name == "") e.Add("[spec] name is missing");
        else
        {
            if (name.Length > 64 || !NameRx.IsMatch(name)) e.Add($"[spec] name '{name}' must be 1-64 chars of a-z, 0-9 and single hyphens");
            if (name != System.IO.Path.GetFileName(dir)) e.Add($"[spec] name '{name}' does not match its folder '{System.IO.Path.GetFileName(dir)}'");
            if (name.Contains("claude") || name.Contains("anthropic")) w.Add("[platform] names containing 'claude' or 'anthropic' are reserved on some platforms");
        }
        Description(d, e, w);
        var body = d.Body;
        if (body.Count > 500) e.Add($"[spec] body is {body.Count} lines; keep SKILL.md under 500 and move reference material to files");
        else if (body.Count > 200) w.Add($"[course] body is {body.Count} lines; every line is re-sent on every later turn once loaded");
        foreach (var h in new[] { "Inputs", "Output contract", "Steps" })
            if (!d.HasHeading(h)) e.Add($"[course] no '## {h}' section");
        var text = string.Join("\n", body);
        foreach (Match m in Regex.Matches(text, @"\]\(([^)\s#]+)(#[^)]*)?\)"))
        {
            var target = m.Groups[1].Value;
            if (target.Contains("://") || target.StartsWith("mailto:")) continue;
            if (!File.Exists(System.IO.Path.Combine(dir, target)) && !Directory.Exists(System.IO.Path.Combine(dir, target)))
                e.Add($"[spec] link to '{target}', which does not exist next to SKILL.md");
        }
        var mentioned = Regex.Matches(text, @"(?<![\w/.$}])((?:scripts|references|assets)/[\w./-]+)").Select(m => m.Groups[1].Value)
            .Concat(Regex.Matches(text, @"\$\{CLAUDE_SKILL_DIR\}/([\w./-]+)").Select(m => m.Groups[1].Value));
        foreach (var rel in mentioned.Select(x => x.TrimEnd('.')).Distinct())
            if (!File.Exists(System.IO.Path.Combine(dir, rel)))
                e.Add($"[spec] mentions '{rel}', which does not exist in the skill folder");
        if (Regex.IsMatch(text, @"(scripts|references|assets)\\")) e.Add("[spec] Windows-style path; use forward slashes");
        if (SideEffectRx.IsMatch(text) && !d.IsTrue("disable-model-invocation"))
            w.Add("[course] the steps have side effects (commit/push/deploy/DROP) but the model may invoke the skill on its own; consider disable-model-invocation: true");
        // Versioning (course rule): metadata.version is SemVer and matches the newest CHANGELOG entry.
        var version = d.Metadata.TryGetValue("version", out var v) ? v : "";
        var changelog = System.IO.Path.Combine(dir, "CHANGELOG.md");
        if (!Regex.IsMatch(version, @"^\d+\.\d+\.\d+$")) e.Add("[course] metadata.version is missing or not SemVer (x.y.z)");
        if (!File.Exists(changelog)) e.Add("[course] no CHANGELOG.md next to SKILL.md");
        else
        {
            var top = File.ReadLines(changelog).Select(l => Regex.Match(l, @"^##\s+\[?(\d+\.\d+\.\d+)\]?")).FirstOrDefault(m => m.Success);
            if (top is null) e.Add("[course] CHANGELOG.md has no '## [x.y.z]' entry");
            else if (version != "" && top.Groups[1].Value != version)
                e.Add($"[course] metadata.version {version} but the newest CHANGELOG entry is {top.Groups[1].Value}");
        }
        return (e, w);
    }

    static (List<string>, List<string>) Agent(Doc d)
    {
        var e = new List<string>(); var w = new List<string>();
        if (!d.HasFrontMatter) { e.Add("[spec] no YAML front-matter"); return (e, w); }
        var name = d.Get("name");
        if (name == "") e.Add("[spec] name is missing");
        else if (!NameRx.IsMatch(name)) e.Add($"[spec] name '{name}' must use a-z, 0-9 and hyphens");
        Description(d, e, w);
        var tools = d.Get("tools");
        if (tools == "") e.Add("[course] no 'tools' allowlist; the agent inherits every tool, including edits and MCP");
        var readOnlyClaim = Regex.IsMatch(d.Get("description") + " " + string.Join(" ", d.Body), @"read-only|never edits?|does not edit", RegexOptions.IgnoreCase);
        var writers = Regex.Matches(tools, @"\b(Write|Edit|MultiEdit|NotebookEdit|Bash|PowerShell)\b").Select(m => m.Value).Distinct().ToList();
        if (readOnlyClaim && writers.Count > 0) e.Add($"[course] claims to be read-only but its tools include {string.Join(", ", writers)}");
        if (!d.HasHeading("Output")) e.Add("[course] no '## Output' section; the caller cannot rely on what comes back");
        return (e, w);
    }

    static void Description(Doc d, List<string> e, List<string> w)
    {
        var desc = d.Get("description");
        if (desc == "") { e.Add("[spec] description is missing"); return; }
        if (desc.Length > 1024) e.Add($"[spec] description is {desc.Length} chars (max 1024)");
        if (!WhenRx.IsMatch(desc)) e.Add("[course] description says what, not when: add 'Use when ...'");
        if (Regex.IsMatch(desc, @"^(I|You|We)\b|\bI can\b")) e.Add("[spec] write the description in the third person");
        if (desc.Length < 60) w.Add($"[course] description is only {desc.Length} chars; the model has little to match on");
    }
}

static class Inventory
{
    static readonly HashSet<string> Stop = new(StringComparer.OrdinalIgnoreCase)
    {
        "the", "and", "for", "use", "when", "with", "this", "that", "from", "into", "user", "asks", "skill", "agent",
        "does", "not", "only", "your", "about", "any", "are", "one", "all", "its", "use", "uses", "used", "after", "before",
        "contoso", "billing", "file", "files", "code", "then", "them", "what", "which", "will", "also", "each"
    };

    public static int Run(string repo, double threshold)
    {
        string[] skip = { "bin", "obj", ".git", "node_modules" };
        bool Ok(string f) => !f.Replace('\\', '/').Split('/').Any(skip.Contains);
        var skills = Directory.EnumerateFiles(repo, "SKILL.md", SearchOption.AllDirectories).Where(Ok)
            .Where(f => f.Replace('\\', '/').Contains("/skills/")).Select(Doc.Load).ToList();
        var agents = Directory.EnumerateFiles(repo, "*.md", SearchOption.AllDirectories).Where(Ok)
            .Where(f => f.Replace('\\', '/').Contains("/agents/") && System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(f)) == "agents")
            .Select(Doc.Load).ToList();
        Console.WriteLine($"{"kind",-6} {"name",-22} {"invocation",-10} {"desc",5} {"body",5} {"files",5} {"version",-8} location");
        var always = 0;
        foreach (var d in skills)
        {
            var inv = d.IsTrue("disable-model-invocation") ? "user" : d.Get("user-invocable").ToLowerInvariant() == "false" ? "model" : "both";
            if (inv != "user") always += d.Get("description").Length;
            var dir = System.IO.Path.GetDirectoryName(d.Path)!;
            var extra = Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories).Count() - 1;
            Console.WriteLine($"{"skill",-6} {d.Get("name"),-22} {inv,-10} {d.Get("description").Length,5} {d.Body.Count,5} {extra,5} {Ver(d),-8} {Rel(repo, dir)}");
        }
        foreach (var d in agents)
        {
            always += d.Get("description").Length;
            Console.WriteLine($"{"agent",-6} {d.Get("name"),-22} {"delegate",-10} {d.Get("description").Length,5} {d.Body.Count,5} {"-",5} {Ver(d),-8} {Rel(repo, d.Path)}");
        }
        Console.WriteLine($"\n{skills.Count} skill(s), {agents.Count} agent(s). Descriptions always in context: {always} chars (~{always / 4} tokens).");
        var all = skills.Select(s => ("skill", s)).Concat(agents.Select(a => ("agent", a))).ToList();
        var problems = 0;
        foreach (var g in all.GroupBy(x => x.Item2.Get("name")).Where(g => g.Count() > 1))
        {
            problems++;
            Console.WriteLine($"  DUPLICATE name '{g.Key}': {string.Join(", ", g.Select(x => Rel(repo, x.Item2.Path)))}");
        }
        for (var i = 0; i < all.Count; i++)
            for (var j = i + 1; j < all.Count; j++)
            {
                var a = Words(all[i].Item2.Get("description")); var b = Words(all[j].Item2.Get("description"));
                if (a.Count == 0 || b.Count == 0) continue;
                var jac = (double)a.Intersect(b).Count() / a.Union(b).Count();
                if (jac >= threshold)
                {
                    problems++;
                    Console.WriteLine($"  OVERLAP {jac:0.00}: '{all[i].Item2.Get("name")}' and '{all[j].Item2.Get("name")}' share: {string.Join(", ", a.Intersect(b).Take(8))}");
                }
            }
        foreach (var d in skills.Where(s => s.Get("description").Length < 60))
        {
            problems++;
            Console.WriteLine($"  VAGUE '{d.Get("name")}': description has {d.Get("description").Length} chars");
        }
        Console.WriteLine(problems == 0 ? "no duplicates, overlaps or vague descriptions found" : $"{problems} finding(s) to review");
        return 0;
    }

    static string Ver(Doc d) => d.Metadata.TryGetValue("version", out var v) ? v : "-";
    static string Rel(string root, string p) => System.IO.Path.GetRelativePath(root, p).Replace('\\', '/');
    static HashSet<string> Words(string s) =>
        Regex.Matches(s.ToLowerInvariant(), @"[a-z][a-z0-9-]{3,}").Select(m => m.Value.TrimEnd('s')).Where(x => !Stop.Contains(x)).ToHashSet();
}

static class Contract
{
    // Rules file: `kind[@Section] | a | b | message`, `#` starts a comment. Fields are separated by a pipe
    // with spaces around it, so regex alternations are written without spaces: (yes|no).
    //   section    | <heading>           |          a heading with that text (or starting with it) exists and has content
    //   require    | <regex>             |          the regex matches somewhere
    //   forbid     | <regex>             |          the regex matches nowhere (reported per line)
    //   pair       | <regex with (group)>| <text $1>  every match needs the templated text somewhere
    //   when       | <regex>             | <regex>  if the first matches, the second must match too
    //   max-count / min-count | <regex>  | <n>      number of matching lines
    // With @Section, the rule only looks at that section's text.
    public static int Run(List<string> files, string rulesPath)
    {
        var rules = File.ReadAllLines(rulesPath).Select(r => r.Trim())
            .Where(r => r.Length > 0 && !r.StartsWith('#')).Select(r => Regex.Split(r, @"\s\|(?=\s)").Select(x => x.Trim()).ToArray()).ToList();
        foreach (var r in rules)
            if (r.Length != 4) throw new ArgumentException($"bad rule (need 4 fields): {string.Join(" | ", r)}");
        var failed = 0;
        foreach (var f in files)
        {
            if (!File.Exists(f)) { Console.WriteLine($"  FAIL [{f}] output file does not exist"); Console.WriteLine($"FAIL {f}"); failed++; continue; }
            var lines = File.ReadAllLines(f);
            var p = new List<string>();
            foreach (var r in rules)
            {
                var kindParts = r[0].Split('@', 2);
                var (kind, section) = (kindParts[0], kindParts.Length > 1 ? kindParts[1] : null);
                var scope = section is null ? lines : Section(lines, section) ?? Array.Empty<string>();
                var text = string.Join("\n", scope);
                var msg = r[3];
                switch (kind)
                {
                    case "section":
                        var s = Section(lines, r[1]);
                        if (s is null) p.Add($"missing section '{r[1]}' ({msg})");
                        else if (s.All(string.IsNullOrWhiteSpace)) p.Add($"section '{r[1]}' is empty ({msg})");
                        break;
                    case "require":
                        if (!Regex.IsMatch(text, r[1], RegexOptions.Multiline)) p.Add(msg);
                        break;
                    case "forbid":
                        for (var i = 0; i < scope.Length; i++)
                            if (Regex.IsMatch(scope[i], r[1])) p.Add($"{msg}: \"{scope[i].Trim()}\"");
                        break;
                    case "pair":
                        foreach (Match m in Regex.Matches(text, r[1], RegexOptions.Multiline))
                        {
                            var need = Regex.Replace(r[2], @"\$(\d)", g => m.Groups[int.Parse(g.Groups[1].Value)].Value);
                            if (!text.Contains(need, StringComparison.Ordinal)) p.Add($"{msg}: found '{m.Value}' but not '{need}'");
                        }
                        break;
                    case "when":
                        if (Regex.IsMatch(text, r[1], RegexOptions.Multiline) && !Regex.IsMatch(text, r[2], RegexOptions.Multiline)) p.Add(msg);
                        break;
                    case "max-count":
                    case "min-count":
                        var n = scope.Count(l => Regex.IsMatch(l, r[1]));
                        var limit = int.Parse(r[2]);
                        if (kind == "max-count" ? n > limit : n < limit) p.Add($"{msg} ({n} found, {(kind == "max-count" ? "max" : "min")} {limit})");
                        break;
                    default:
                        throw new ArgumentException($"unknown rule kind '{kind}'");
                }
            }
            foreach (var x in p.Distinct()) Console.WriteLine($"  FAIL [{System.IO.Path.GetFileName(f)}] {x}");
            Console.WriteLine(p.Count == 0 ? $"PASS {f}" : $"FAIL {f}: {p.Distinct().Count()} problem(s)");
            if (p.Count > 0) failed++;
        }
        return failed == 0 ? 0 : 1;
    }

    /// <summary>Lines under a heading with this text, up to the next heading of the same or a higher level.</summary>
    static string[]? Section(string[] lines, string heading)
    {
        for (var i = 0; i < lines.Length; i++)
        {
            var h = Regex.Match(lines[i], @"^(#{1,6})\s+(.+?)\s*$");
            if (!h.Success) continue;
            var t = h.Groups[2].Value;
            if (!t.Equals(heading, StringComparison.OrdinalIgnoreCase) &&
                !t.StartsWith(heading + " ", StringComparison.OrdinalIgnoreCase)) continue;
            var level = h.Groups[1].Value.Length;
            var body = new List<string>();
            for (var j = i + 1; j < lines.Length; j++)
            {
                var n = Regex.Match(lines[j], @"^(#{1,6})\s");
                if (n.Success && n.Groups[1].Value.Length <= level) break;
                body.Add(lines[j]);
            }
            return body.ToArray();
        }
        return null;
    }
}

static class Triggers
{
    // Input: TSV with a header `id  expected  run  fired` (what scripts/run-triggers.sh writes).
    // expected: 1 = the skill should load for this query, 0 = it should not. fired: 1 if it loaded.
    public static int Run(string path, double minRecall, double maxFalse)
    {
        var rows = File.ReadAllLines(path).Skip(1).Where(l => l.Trim().Length > 0)
            .Select(l => l.Split('\t')).Select(c => (Id: c[0], Expected: c[1] == "1", Fired: c[3] == "1")).ToList();
        if (rows.Count == 0) throw new ArgumentException("no runs in file");
        Console.WriteLine($"{"query",-6} {"expect",-7} {"fired",-6} rate");
        foreach (var g in rows.GroupBy(r => r.Id))
        {
            var fired = g.Count(r => r.Fired);
            Console.WriteLine($"{g.Key,-6} {(g.First().Expected ? "load" : "skip"),-7} {fired + "/" + g.Count(),-6} {(double)fired / g.Count():0.00}");
        }
        var pos = rows.Where(r => r.Expected).ToList();
        var neg = rows.Where(r => !r.Expected).ToList();
        int tp = pos.Count(r => r.Fired), fp = neg.Count(r => r.Fired);
        var recall = pos.Count == 0 ? 1 : (double)tp / pos.Count;
        var falseRate = neg.Count == 0 ? 0 : (double)fp / neg.Count;
        Console.WriteLine();
        Console.WriteLine($"should load:     fired on {tp}/{pos.Count} runs  recall {recall:0.00}");
        Console.WriteLine($"should not load: fired on {fp}/{neg.Count} runs  false-trigger rate {falseRate:0.00}");
        Console.WriteLine(tp + fp == 0 ? "precision: n/a (never fired)" : $"precision:       {tp}/{tp + fp} = {(double)tp / (tp + fp):0.00}");
        var unstable = rows.GroupBy(r => r.Id).Where(g => g.Any(r => r.Fired) && g.Any(r => !r.Fired)).Select(g => g.Key).ToList();
        if (unstable.Count > 0) Console.WriteLine($"unstable (fired on some runs, not all): {string.Join(", ", unstable)}");
        Console.WriteLine($"note: {rows.Count} runs is a small sample; Module 7 puts an interval around these rates.");
        var ok = recall >= minRecall && falseRate <= maxFalse;
        Console.WriteLine(ok ? $"PASS triggers (recall >= {minRecall}, false-trigger rate <= {maxFalse})"
                             : $"FAIL triggers (need recall >= {minRecall} and false-trigger rate <= {maxFalse})");
        return ok ? 0 : 1;
    }
}
