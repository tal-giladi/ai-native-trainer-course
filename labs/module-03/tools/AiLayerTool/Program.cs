// AiLayerTool - Module 3 lab helper. Two commands:
//   scan <repo>                  brownfield audit facts an agent cannot reliably infer
//   lint <rules-file> --repo <repo>   checks that every rule line is grounded in the repo
// Deliberately small (no dependencies). It reads files only; it never modifies the repo.
using System.Text.RegularExpressions;

static int Usage()
{
    Console.Error.WriteLine("usage: AiLayerTool scan <repo> | AiLayerTool lint <rules-file> --repo <repo>");
    return 2;
}

Console.OutputEncoding = System.Text.Encoding.UTF8;
if (args.Length < 2) return Usage();
return args[0] switch
{
    "scan" => Scanner.Run(Path.GetFullPath(args[1])),
    "lint" when args.Length >= 4 && args[2] == "--repo" => Linter.Run(Path.GetFullPath(args[1]), Path.GetFullPath(args[3])),
    _ => Usage()
};

/// <summary>Index of the repository: files, code text without comments/strings, declared and obsolete types.</summary>
sealed class RepoIndex
{
    static readonly string[] Skip = { "bin", "obj", ".git", ".vs", "node_modules" };
    static readonly string[] CodeExt = { ".cs", ".csproj", ".sln", ".slnx", ".sql", ".json", ".props" };

    public string Root { get; }
    public List<string> Files { get; }
    public string Code { get; }
    public HashSet<string> Declared { get; } = new();
    public Dictionary<string, (string File, string Message)> Obsolete { get; } = new();

    public RepoIndex(string root)
    {
        Root = root;
        Files = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Where(f => !Rel(f).Split('/').Any(part => Skip.Contains(part)))
            .ToList();
        var code = new System.Text.StringBuilder();
        foreach (var f in Files.Where(f => CodeExt.Contains(Path.GetExtension(f).ToLowerInvariant())))
        {
            var raw = File.ReadAllText(f);
            if (f.EndsWith(".cs"))
            {
                foreach (Match m in Regex.Matches(raw, @"\b(?:class|record|interface|struct|enum)\s+(\w+)"))
                    Declared.Add(m.Groups[1].Value);
                foreach (Match m in Regex.Matches(raw,
                    @"\[Obsolete(?:\(""(?<msg>[^""]*)""\))?\]\s*(?:(?:public|internal|static|sealed|abstract|partial)\s+)*(?:class|record|interface|struct|enum)\s+(?<name>\w+)"))
                    Obsolete[m.Groups["name"].Value] = (Rel(f), m.Groups["msg"].Value);
                raw = StripCommentsAndStrings(raw);
            }
            else if (f.EndsWith(".sql"))
            {
                raw = Regex.Replace(raw, @"--.*$", "", RegexOptions.Multiline);
            }
            code.AppendLine(raw);
        }
        Code = code.ToString();
    }

    public string Rel(string path) => Path.GetRelativePath(Root, path).Replace('\\', '/');

    public static string StripCommentsAndStrings(string s)
    {
        s = Regex.Replace(s, @"/\*.*?\*/", "", RegexOptions.Singleline);
        s = Regex.Replace(s, @"@""(?:[^""]|"""")*""", "\"\"");
        s = Regex.Replace(s, @"""(?:[^""\\\n]|\\.)*""", "\"\"");
        return Regex.Replace(s, @"//.*$", "", RegexOptions.Multiline);
    }

    public bool PathExists(string token)
    {
        var t = token.TrimEnd('/');
        var full = Path.GetFullPath(Path.Combine(Root, t));
        return File.Exists(full) || Directory.Exists(full)
            || Files.Any(f => Path.GetFileName(f) == t);
    }

    public int Uses(string token) => Regex.Matches(Code, $@"(?<![\w.]){Regex.Escape(token)}(?!\w)").Count;

    /// <summary>Resolves a backticked token. Returns null when fine, otherwise a problem description.</summary>
    public (string Level, string Text)? Check(string token, bool negated)
    {
        if (token.Contains('#') || token.Contains('*') || token.Contains('…')) return null; // patterns, not names
        if (token.StartsWith("dotnet "))
        {
            foreach (var arg in token.Split(' ').Where(a => Regex.IsMatch(a, @"\.(sln|slnx|csproj)$")))
                if (!PathExists(arg)) return ("ERROR", $"command `{token}` refers to `{arg}`, which does not exist");
            return null;
        }
        if (token.Contains(' ')) return null;
        if (token.Contains('/') || Regex.IsMatch(token, @"\.(md|cs|sql|sln|slnx|csproj|json|txt|yml|yaml|mdc)$"))
            return PathExists(token) ? null : ("ERROR", $"path `{token}` does not exist");
        if (!Regex.IsMatch(token, @"^[A-Z][A-Za-z0-9_]*(\.[A-Za-z_][A-Za-z0-9_]*)*(\(\))?$")) return null;
        var name = token.TrimEnd('(', ')');
        if (PathExists(name)) return null;
        var first = name.Split('.')[0];
        if (!negated && Obsolete.TryGetValue(first, out var obs))
            return ("ERROR", $"`{token}` is [Obsolete] in {obs.File}: \"{obs.Message}\"");
        if (negated) return null;
        var last = name.Split('.').Last();
        bool found = Uses(name) > 0 || (Declared.Contains(first) && Uses(last) > 0);
        return found ? null : ("ERROR", $"`{token}` is not used anywhere in code (stale or invented?)");
    }
}

static class Scanner
{
    public static int Run(string root)
    {
        if (!Directory.Exists(root)) { Console.Error.WriteLine($"not a directory: {root}"); return 2; }
        var ix = new RepoIndex(root);
        Console.WriteLine($"# Brownfield scan: {Path.GetFileName(root)}\n");

        Console.WriteLine("## Solutions and projects");
        foreach (var sln in ix.Files.Where(f => f.EndsWith(".sln") || f.EndsWith(".slnx")))
            Console.WriteLine($"- solution `{ix.Rel(sln)}` -> build: `dotnet build {ix.Rel(sln)}`, test: `dotnet test {ix.Rel(sln)}`");
        foreach (var proj in ix.Files.Where(f => f.EndsWith(".csproj")))
        {
            var xml = File.ReadAllText(proj);
            var tfm = Regex.Match(xml, @"<TargetFrameworks?>([^<]+)<").Groups[1].Value;
            var pkgs = Regex.Matches(xml, @"PackageReference Include=""([^""]+)"" Version=""([^""]+)""")
                .Select(m => $"{m.Groups[1].Value} {m.Groups[2].Value}").ToList();
            var isTest = pkgs.Any(p => p.StartsWith("xunit") || p.StartsWith("NUnit") || p.StartsWith("MSTest"));
            Console.WriteLine($"- `{ix.Rel(proj)}` | {tfm}{(isTest ? " | TEST PROJECT" : "")} | {(pkgs.Count == 0 ? "no packages" : string.Join(", ", pkgs))}");
        }

        Console.WriteLine("\n## SQL migrations (V### needs a matching U### undo script?)");
        var migrations = ix.Files.Select(f => Path.GetFileName(f)).Where(n => Regex.IsMatch(n, @"^[VU]\d{3}__.*\.sql$")).ToHashSet();
        foreach (var v in migrations.Where(n => n.StartsWith('V')).OrderBy(n => n))
            Console.WriteLine($"- {v}: {(migrations.Contains("U" + v[1..]) ? "undo present" : "NO undo script")}");

        Console.WriteLine("\n## Obsolete types and their remaining callers");
        foreach (var (type, (file, msg)) in ix.Obsolete)
        {
            var callers = ix.Files.Where(f => f.EndsWith(".cs") && ix.Rel(f) != file)
                .Where(f => Regex.IsMatch(RepoIndex.StripCommentsAndStrings(File.ReadAllText(f)),$@"\b{type}\.")).Select(ix.Rel).ToList();
            Console.WriteLine($"- `{type}` ({file}) | \"{msg}\" | callers: {(callers.Count == 0 ? "none" : string.Join(", ", callers))}");
        }

        Console.WriteLine("\n## Local wall-clock reads (DateTime.Now / DateTimeOffset.Now) in code");
        var clock = ix.Uses("DateTime.Now") + ix.Uses("DateTimeOffset.Now");
        Console.WriteLine(clock == 0 ? "- none" : $"- {clock} occurrence(s)");

        Console.WriteLine("\n## Existing AI-layer files");
        string[] layer = { "AGENTS.md", "CLAUDE.md", "CLAUDE.local.md", ".claude", ".cursor/rules", ".cursorrules",
                           ".github/copilot-instructions.md", ".github/instructions", ".mcp.json", "CODEOWNERS", ".github/CODEOWNERS" };
        foreach (var l in layer)
            Console.WriteLine($"- {l}: {(File.Exists(Path.Combine(root, l)) || Directory.Exists(Path.Combine(root, l)) ? "present" : "missing")}");

        Console.WriteLine("\n## Docs that name things the code no longer has");
        foreach (var doc in ix.Files.Where(f => f.EndsWith(".md") && ix.Rel(f).StartsWith("docs/")))
        {
            var text = File.ReadAllText(doc);
            var edited = Regex.Match(text, @"Last edited:\s*([0-9-]+)").Groups[1].Value;
            // A doc that itself records a deprecation (an ADR) may name the obsolete type.
            var recordsDeprecation = Regex.IsMatch(text, @"\[Obsolete\]|deprecat|replace", RegexOptions.IgnoreCase);
            var problems = Regex.Matches(text, "`([^`]+)`").Select(m => m.Groups[1].Value).Distinct()
                .Select(t => ix.Check(t, negated: false)).Where(p => p != null).Select(p => p!.Value.Text)
                .Where(t => !(recordsDeprecation && t.Contains("[Obsolete]"))).ToList();
            if (problems.Count > 0)
                Console.WriteLine($"- `{ix.Rel(doc)}`{(edited.Length > 0 ? $" (last edited {edited})" : "")}:\n  - " + string.Join("\n  - ", problems));
        }
        return 0;
    }
}

static class Linter
{
    static readonly Regex Negation = new(@"\b(never|not|no|don't|do not|forbid\w*|avoid|instead of)\b\W*(\w+\W+){0,3}$", RegexOptions.IgnoreCase);

    public static int Run(string rulesFile, string root)
    {
        if (!File.Exists(rulesFile)) { Console.Error.WriteLine($"no such file: {rulesFile}"); return 2; }
        var ix = new RepoIndex(root);
        var lines = File.ReadAllLines(rulesFile);
        int errors = 0, warnings = 0;
        void Report(string level, int line, string text)
        {
            if (level == "ERROR") errors++; else warnings++;
            Console.WriteLine($"{level} line {line}: {text}");
        }

        if (lines.Length > 300) Report("ERROR", lines.Length, $"{lines.Length} lines; the limit for this course is 300");
        else if (lines.Length > 200) Report("WARN", lines.Length, $"{lines.Length} lines; consider path-scoped rules or skills");

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmed = line.TrimStart();
            if (trimmed.StartsWith('@'))
            {
                var target = trimmed[1..].Trim();
                var full = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(rulesFile)!, target));
                if (!File.Exists(full)) Report("ERROR", i + 1, $"import `@{target}` not found next to the rules file");
                continue;
            }
            bool grounded = false;
            foreach (Match m in Regex.Matches(line, "`([^`]+)`"))
            {
                var token = m.Groups[1].Value;
                var negated = Negation.IsMatch(line[..m.Index]);
                var problem = ix.Check(token, negated);
                if (problem is { } p) Report(p.Level, i + 1, p.Text);
                else if (token.Contains('/') || ix.PathExists(token)) grounded = true;
            }
            if (Regex.IsMatch(trimmed, @"^[-*] ") && !grounded && !line.Contains("(ev:"))
                Report("WARN", i + 1, "rule has no evidence: cite a file with (ev: `path`) or delete it");
        }
        Console.WriteLine($"\n{lines.Length} lines | {errors} error(s) | {warnings} warning(s)");
        return errors > 0 ? 1 : 0;
    }
}
