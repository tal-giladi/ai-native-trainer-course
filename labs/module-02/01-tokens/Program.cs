// TokenLab — lesson 02.1. Three commands:
//   dotnet run -- count [files...] [--show 40]        token counts under two tokenizers (+ Anthropic if configured)
//   dotnet run -- solution <dir> [--price 4.00]       total tokens of a real solution and cost to read it once
//   dotnet run -- needle --provider anthropic|openai --model <id> [--tokens 20000] [--variant single|multi] [--runs 3]
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.ML.Tokenizers;

Console.OutputEncoding = Encoding.UTF8;
var cl100k = TiktokenTokenizer.CreateForEncoding("cl100k_base");
var o200k = TiktokenTokenizer.CreateForEncoding("o200k_base");
var cmd = args.FirstOrDefault() ?? "count";
string Opt(string name, string dflt) { var i = Array.IndexOf(args, name); return i >= 0 && i + 1 < args.Length ? args[i + 1] : dflt; }

switch (cmd)
{
    case "count": await Count(); break;
    case "solution": Solution(); break;
    case "needle": await Needle(); break;
    default: Console.WriteLine("commands: count | solution <dir> | needle"); break;
}

async Task Count()
{
    var files = args.Skip(1).Where(a => !a.StartsWith("--") && File.Exists(a)).ToList();
    if (files.Count == 0) files = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "samples")).OrderBy(f => f).ToList();
    var anthropicModel = Environment.GetEnvironmentVariable("ANTHROPIC_COUNT_MODEL"); // optional third tokenizer
    Console.WriteLine($"{"file",-30} {"chars",6} {"cl100k",7} {"o200k",7} {"ch/tok(o200k)",14} {(anthropicModel is null ? "" : "anthropic")}");
    foreach (var f in files)
    {
        var text = File.ReadAllText(f);
        int a = cl100k.CountTokens(text), b = o200k.CountTokens(text);
        var extra = anthropicModel is null ? "" : (await AnthropicCount(anthropicModel, text)).ToString();
        Console.WriteLine($"{Path.GetFileName(f),-30} {text.Length,6} {a,7} {b,7} {(double)text.Length / b,14:F2} {extra}");
    }
    var show = int.Parse(Opt("--show", "0"));
    if (show > 0)
        foreach (var f in files)
        {
            Console.WriteLine($"\n--- first {show} o200k tokens of {Path.GetFileName(f)} (| marks boundaries) ---");
            var toks = o200k.EncodeToTokens(File.ReadAllText(f), out _).Take(show);
            Console.WriteLine(string.Join("|", toks.Select(t => o200k.Decode(new[] { t.Id }))));
        }
}

void Solution()
{
    var dir = args.Length > 1 ? args[1] : ".";
    var price = double.Parse(Opt("--price", "0"), System.Globalization.CultureInfo.InvariantCulture); // USD per million input tokens, from YOUR provider's page
    var exts = new[] { ".cs", ".sql", ".csproj", ".sln", ".json", ".config", ".md", ".cshtml", ".js", ".ts" };
    var skip = new[] { "\\bin\\", "\\obj\\", "\\.git\\", "/bin/", "/obj/", "/.git/", "node_modules" };
    var rows = Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories)
        .Where(f => exts.Contains(Path.GetExtension(f).ToLowerInvariant()) && !skip.Any(f.Contains))
        .Select(f => (ext: Path.GetExtension(f).ToLowerInvariant(), tokens: o200k.CountTokens(File.ReadAllText(f))))
        .GroupBy(r => r.ext).Select(g => (g.Key, files: g.Count(), tokens: g.Sum(r => r.tokens)))
        .OrderByDescending(r => r.tokens).ToList();
    foreach (var r in rows) Console.WriteLine($"{r.Key,-8} {r.files,6} files {r.tokens,12:N0} tokens");
    var total = rows.Sum(r => r.tokens);
    Console.WriteLine($"TOTAL    {rows.Sum(r => r.files),6} files {total,12:N0} tokens (o200k estimate)");
    if (price > 0) Console.WriteLine($"Reading all of it once as input: ~${total / 1_000_000.0 * price:F2} at ${price}/MTok");
}

async Task Needle()
{
    var provider = Opt("--provider", "anthropic");
    var model = Opt("--model", "");
    if (model == "") { Console.WriteLine("--model is required"); return; }
    int target = int.Parse(Opt("--tokens", "20000")), runs = int.Parse(Opt("--runs", "3"));
    var multi = Opt("--variant", "single") == "multi";
    var positions = new[] { 0.0, 0.25, 0.5, 0.75, 1.0 };
    Console.WriteLine($"variant={(multi ? "multi" : "single")} target≈{target:N0} tokens, {runs} runs per position");
    foreach (var pos in positions)
    {
        int hits = 0, inTok = 0;
        for (var r = 0; r < runs; r++)
        {
            var prompt = BuildHaystack(target, pos, multi);
            var res = await Llm.CompleteAsync(provider, model, prompt, maxTokens: 20);
            inTok = res.InputTokens;
            if (res.Text.Trim().StartsWith("7")) hits++;
            else Console.WriteLine($"   miss at {pos:P0}: \"{res.Text.Trim()}\"");
        }
        Console.WriteLine($"needle at {pos,5:P0}: {hits}/{runs} correct (provider counted {inTok:N0} input tokens)");
    }
}

string BuildHaystack(int targetTokens, double needlePos, bool multi)
{
    // Deterministic, boring, plausible filler: the kind of notes a legacy wiki is full of.
    var rng = new Random(42);
    string[] jobs = { "invoice export", "stock sync", "email digest", "audit purge", "price import", "report cache" };
    string[] tables = { "Invoices", "StockLevels", "Customers", "AuditLog", "PriceLists", "ReportCache" };
    var lines = new List<string>();
    var sb = new StringBuilder();
    while (o200k.CountTokens(sb.ToString()) < targetTokens)
    {
        var line = $"Ops note {lines.Count + 1}: the {jobs[rng.Next(jobs.Length)]} job runs every {rng.Next(5, 60)} minutes, " +
                   $"writes to dbo.{tables[rng.Next(tables.Length)]} and alerts on-call after {rng.Next(2, 9)} failures.";
        lines.Add(line); sb.AppendLine(line);
    }
    void Insert(double p, string s) => lines.Insert((int)Math.Round(p * lines.Count), s);
    if (multi)
    {   // Distractors: the same fact with stale or never-deployed values, elsewhere in the context.
        Insert((needlePos + 0.33) % 1.0, "Old note (2019): the Orders API retry count was 3.");
        Insert((needlePos + 0.66) % 1.0, "Draft proposal, never deployed: raise the Orders API retry count to 5.");
    }
    Insert(needlePos, "Current production setting: the Orders API retry count is 7.");
    var question = multi
        ? "What is the CURRENT PRODUCTION Orders API retry count? Answer with the number only."
        : "What is the Orders API retry count? Answer with the number only.";
    return "Below are operations notes.\n\n" + string.Join("\n", lines) + "\n\n" + question;
}

async Task<int> AnthropicCount(string model, string text)
{
    var body = new JsonObject
    {
        ["model"] = model,
        ["messages"] = new JsonArray(new JsonObject { ["role"] = "user", ["content"] = text })
    };
    using var http = new HttpClient();
    using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages/count_tokens");
    req.Headers.Add("x-api-key", Llm.Env("ANTHROPIC_API_KEY"));
    req.Headers.Add("anthropic-version", "2023-06-01");
    req.Content = new StringContent(body.ToJsonString(), Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
    var res = await http.SendAsync(req);
    var json = JsonNode.Parse(await res.Content.ReadAsStringAsync())!;
    // The count includes a few tokens of message framing; compare files relative to each other, not absolutely.
    return (int?)json["input_tokens"] ?? -1;
}
