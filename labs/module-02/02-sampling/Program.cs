// SamplingLab — lesson 02.2. Three commands:
//   dotnet run -- softmax --logits 2,1,0 [--t 1.0] [--top-p 1.0] [--draws 10000]
//   dotnet run -- chain --p 0.97 --steps 20                 probability that every step / every run succeeds
//   dotnet run -- runs --provider openai|anthropic --model <id> [--t 0] [--n 10] [--prompt-file prompt.txt]
using System.Globalization;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
var inv = CultureInfo.InvariantCulture;
string Opt(string name, string dflt) { var i = Array.IndexOf(args, name); return i >= 0 && i + 1 < args.Length ? args[i + 1] : dflt; }
double D(string name, string dflt) => double.Parse(Opt(name, dflt), inv);

switch (args.FirstOrDefault() ?? "softmax")
{
    case "softmax": Softmax(); break;
    case "chain": Chain(); break;
    case "runs": await Runs(); break;
    default: Console.WriteLine("commands: softmax | chain | runs"); break;
}

void Softmax()
{
    var logits = Opt("--logits", "2,1,0").Split(',').Select(s => double.Parse(s, inv)).ToArray();
    double t = D("--t", "1.0"), topP = D("--top-p", "1.0");
    int draws = int.Parse(Opt("--draws", "10000"));
    var probs = ApplyTemperature(logits, t);
    var kept = TopP(probs, topP);
    Console.WriteLine($"T={t}, top-p={topP}");
    for (var i = 0; i < logits.Length; i++)
        Console.WriteLine($"  token {i}: logit {logits[i],5:F2} -> p {probs[i]:F3} -> after top-p {kept[i]:F3}");
    // Empirical check: sample many times and compare frequencies with the probabilities above.
    var rng = new Random(7);
    var counts = new int[logits.Length];
    for (var d = 0; d < draws; d++) counts[Sample(kept, rng)]++;
    Console.WriteLine($"  {draws:N0} draws: " + string.Join("  ", counts.Select((c, i) => $"token {i}={c / (double)draws:F3}")));
}

static double[] ApplyTemperature(double[] logits, double t)
{
    if (t <= 0) // T -> 0 is argmax (greedy); ties would go to the first index
    {
        var best = Array.IndexOf(logits, logits.Max());
        return logits.Select((_, i) => i == best ? 1.0 : 0.0).ToArray();
    }
    var scaled = logits.Select(l => l / t).ToArray();
    var max = scaled.Max();                      // subtract the max for numerical stability
    var exps = scaled.Select(s => Math.Exp(s - max)).ToArray();
    var sum = exps.Sum();
    return exps.Select(e => e / sum).ToArray();
}

static double[] TopP(double[] probs, double p)
{
    // Keep the smallest set of most-likely tokens whose cumulative probability reaches p, then renormalise.
    var order = probs.Select((v, i) => (v, i)).OrderByDescending(x => x.v).ToList();
    var keep = new HashSet<int>();
    double cum = 0;
    foreach (var (v, i) in order) { keep.Add(i); cum += v; if (cum >= p - 1e-12) break; }
    var total = keep.Sum(i => probs[i]);
    return probs.Select((v, i) => keep.Contains(i) ? v / total : 0.0).ToArray();
}

static int Sample(double[] probs, Random rng)
{
    var r = rng.NextDouble(); double cum = 0;
    for (var i = 0; i < probs.Length; i++) { cum += probs[i]; if (r < cum) return i; }
    return probs.Length - 1;
}

void Chain()
{
    double p = D("--p", "0.97"); int steps = int.Parse(Opt("--steps", "20"));
    Console.WriteLine($"per-step success p={p}");
    foreach (var k in new[] { 1, 2, 3, 5, 10, steps }.Distinct().OrderBy(x => x))
        Console.WriteLine($"  k={k,3}: all succeed p^k = {Math.Pow(p, k):F3}   at least one succeeds 1-(1-p)^k = {1 - Math.Pow(1 - p, k):F3}");
}

async Task Runs()
{
    var provider = Opt("--provider", "openai");
    var model = Opt("--model", "");
    if (model == "") { Console.WriteLine("--model is required"); return; }
    var tArg = Opt("--t", "none");
    double? t = tArg == "none" ? null : double.Parse(tArg, inv);
    int n = int.Parse(Opt("--n", "10"));
    var promptFile = Opt("--prompt-file", Path.Combine(AppContext.BaseDirectory, "prompt.txt"));
    var prompt = File.ReadAllText(promptFile);
    var outputs = new List<string>();
    for (var i = 0; i < n; i++)
    {
        var res = await Llm.CompleteAsync(provider, model, prompt, t, maxTokens: 400);
        outputs.Add(res.Text.Trim());
        Console.WriteLine($"run {i + 1,2}: {res.OutputTokens,4} output tokens, {res.Seconds,5:F1}s, starts \"{Head(res.Text)}\"");
    }
    var groups = outputs.GroupBy(o => o).OrderByDescending(g => g.Count()).ToList();
    Console.WriteLine($"\n{groups.Count} distinct outputs in {n} runs at T={tArg}");
    foreach (var g in groups) Console.WriteLine($"  x{g.Count()}: \"{Head(g.Key)}\"");
    // Where do two outputs first diverge? That is the first token where sampling (or numerics) chose differently.
    if (groups.Count > 1)
    {
        string a = groups[0].Key, b = groups[1].Key;
        var i = 0; while (i < Math.Min(a.Length, b.Length) && a[i] == b[i]) i++;
        Console.WriteLine($"  top two outputs share the first {i} characters, then diverge");
    }
}

static string Head(string s) => s.Replace("\n", " ").Length > 60 ? s.Replace("\n", " ")[..60] + "..." : s.Replace("\n", " ");
