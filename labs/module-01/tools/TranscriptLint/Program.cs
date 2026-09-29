// TranscriptLint — the Module 1 interview linter. Dependency-free, read-only, deterministic.
//
//   dotnet run --project tools/TranscriptLint -- <transcript.md> [--legend]
//
// Input: a markdown transcript with one turn per line, in either format:
//   **Q7 · Interviewer:** question text        **Participant:** answer text
//   I: question text                           P: answer text
//
// It flags interviewer turns with regex heuristics (leading, hypothetical, generic, pitch,
// double-barrelled, compliment-fishing, anchoring), marks past-specific asks, and classifies
// participant turns as past evidence, opinion/future, or neutral. It is a first pass for a
// human reviewer: it misses presupposing questions and cannot see a dropped thread.
using System.Text.RegularExpressions;

static class Program
{
    const RegexOptions Opt = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

    // Interviewer defect heuristics. Order = print order.
    static readonly (string Code, string Name, Regex Rx)[] Rules =
    {
        ("L", "leading", new Regex(@"\b(don'?t you|doesn'?t it|isn'?t it|wouldn'?t (you|it|that)|would you (say|agree)|surely|so you('d| would))\b|\bright\?", Opt)),
        ("H", "hypothetical/future", new Regex(@"\bwould you(?! (say|agree))\b|\bwill you\b|\bif (you|we) had\b|\bhow much would\b|\bcould you see yourself\b", Opt)),
        ("G", "generic", new Regex(@"\b(usually|ever|in general|generally|typically|normally|always|scale of)\b", Opt)),
        ("P", "pitch", new Regex(@"\b(my|our) (workshop|course|product|tool|service|module)\b|\bI'?m building\b|\bwe'?re building\b|\bI'?ll add\b", Opt)),
        ("D", "double-barrelled", new Regex(@"\band (how|why|what|would|because)\b|\bor for\b", Opt)),
        ("C", "compliment-fishing", new Regex(@"\bdo you (like|love)\b|\bsounds? (good|great)\b|\blove the idea\b", Opt)),
        ("A", "anchoring", new Regex(@"\b\d+\s?%|\$\s?\d|\bthousand\b", Opt)),
    };

    static readonly Regex PastAsk = new(@"\b(tell me about the last|the last time|last time you|walk me through|what happened|when did|(how|what|who) did|who else (felt|was)|afterwards)\b", Opt);

    static readonly Regex Evidence = new(
        @"\b(last (week|month|sprint|quarter|year|time)|yesterday|in the (spring|summer|autumn|fall|winter)|on the (monday|tuesday|wednesday|thursday|friday)|on (a |the )?(monday|tuesday|wednesday|thursday|friday)|(a|one|two|three) days?|in the retro|\d+ (hours?|days?|weeks?|minutes?)|(two|three|four|five|six) (hours?|days?|weeks?|of us)|we tried|turned it off|took)\b", Opt);

    static readonly Regex Opinion = new(@"\b(I guess|probably|maybe|I think|sounds|would help|would be|I'?d love|all the time)\b", Opt);

    static readonly Regex MdTurn = new(@"^\*\*(?:Q(?<n>\d+)\s*·\s*)?(?<who>Interviewer|Participant):\*\*\s*(?<text>.*)$", Opt);
    static readonly Regex PlainTurn = new(@"^(?<who>I|P):\s*(?<text>.*)$");

    static int Main(string[] args)
    {
        if (args.Length == 0 || args[0] is "-h" or "--help")
        {
            Console.WriteLine("usage: TranscriptLint <transcript.md> [--legend]");
            return 0;
        }
        var path = args[0];
        if (!File.Exists(path)) { Console.Error.WriteLine($"not found: {path}"); return 2; }

        var turns = new List<(bool Interviewer, int Q, string Text)>();
        int q = 0;
        foreach (var raw in File.ReadLines(path))
        {
            var line = raw.Trim();
            var m = MdTurn.Match(line);
            bool? interviewer = null; string text = "";
            if (m.Success) { interviewer = m.Groups["who"].Value.Equals("Interviewer", StringComparison.OrdinalIgnoreCase); text = m.Groups["text"].Value; }
            else if ((m = PlainTurn.Match(line)).Success) { interviewer = m.Groups["who"].Value == "I"; text = m.Groups["text"].Value; }
            if (interviewer is null) continue;
            if (interviewer.Value) q++;
            turns.Add((interviewer.Value, q, text));
        }
        if (q == 0) { Console.Error.WriteLine("no interviewer turns found (expected '**Qn · Interviewer:**' or 'I:' lines)"); return 2; }

        Console.WriteLine($"TranscriptLint — {Path.GetFileName(path)}");
        Console.WriteLine();
        Console.WriteLine($"{"Q",-4} {"flags",-8} {"past?",-6} interviewer");
        int flagged = 0, pastAsks = 0, iWords = 0, pWords = 0;
        var codeCounts = Rules.ToDictionary(r => r.Code, _ => 0);
        foreach (var t in turns.Where(t => t.Interviewer))
        {
            var codes = Rules.Where(r => r.Rx.IsMatch(t.Text)).Select(r => r.Code).ToList();
            foreach (var c in codes) codeCounts[c]++;
            bool past = PastAsk.IsMatch(t.Text);
            if (codes.Count > 0) flagged++;
            if (past) pastAsks++;
            var shown = t.Text.Length > 64 ? t.Text[..61] + "..." : t.Text;
            Console.WriteLine($"Q{t.Q,-3} {(codes.Count == 0 ? "-" : string.Join(",", codes)),-8} {(past ? "yes" : ""),-6} {shown}");
        }

        int ev = 0, op = 0, neu = 0;
        var evidenceAt = new List<string>();
        foreach (var t in turns)
        {
            int w = Regex.Matches(t.Text, @"[\p{L}\p{N}']+").Count;
            if (t.Interviewer) { iWords += w; continue; }
            pWords += w;
            if (Evidence.IsMatch(t.Text)) { ev++; evidenceAt.Add($"Q{t.Q}"); }
            else if (Opinion.IsMatch(t.Text)) op++;
            else neu++;
        }

        int pTurns = ev + op + neu;
        Console.WriteLine();
        Console.WriteLine("Summary");
        Console.WriteLine($"  interviewer turns     {q}");
        Console.WriteLine($"  flagged               {flagged} ({100.0 * flagged / q:F0}%)");
        Console.WriteLine($"  by code               {string.Join("  ", Rules.Select(r => $"{r.Code}={codeCounts[r.Code]}"))}");
        Console.WriteLine($"  past-specific asks    {pastAsks}");
        Console.WriteLine($"  talk share            interviewer {iWords} words, participant {pWords} words -> interviewer {100.0 * iWords / Math.Max(1, iWords + pWords):F0}%");
        Console.WriteLine($"  participant turns     {pTurns}: past evidence {ev}, opinion/future {op}, neutral {neu}");
        Console.WriteLine($"  evidence after        {(evidenceAt.Count == 0 ? "(none)" : string.Join(", ", evidenceAt))}");
        Console.WriteLine();
        Console.WriteLine("Heuristic only. Read every 'evidence' answer: did the next question follow it up?");

        if (args.Contains("--legend"))
        {
            Console.WriteLine();
            foreach (var r in Rules) Console.WriteLine($"  {r.Code} = {r.Name}");
            Console.WriteLine("  past? = the question asks about a specific past event");
        }
        return 0;
    }
}
