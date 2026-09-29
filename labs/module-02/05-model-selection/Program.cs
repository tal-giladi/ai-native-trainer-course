// ModelBench — lesson 02.5. Runs every task in tasks/tasks.json against every model in models.json,
// runsPerTask times each, and reports pass rate, latency, tokens, cost per attempt and cost per successful task.
//   dotnet run                  real calls (keys in env vars named by models.json)
//   dotnet run -- --fake        pipeline check with canned answers, no keys, no cost
using System.Globalization;
using System.Text;
using System.Text.Json;

Console.OutputEncoding = Encoding.UTF8;
var inv = CultureInfo.InvariantCulture;
var fake = args.Contains("--fake");
var jsonOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var cfg = JsonSerializer.Deserialize<Config>(File.ReadAllText("models.json"), jsonOpts)!;
var tasks = JsonSerializer.Deserialize<List<BenchTask>>(File.ReadAllText("tasks/tasks.json"), jsonOpts)!;
var rows = new List<Row>();
Console.WriteLine($"{cfg.Models.Count} models x {tasks.Count} tasks x {cfg.RunsPerTask} runs; prices as of {cfg.PricesAsOf}\n");

foreach (var m in cfg.Models)
foreach (var t in tasks)
{
    var prompt = File.ReadAllText(Path.Combine("tasks", t.PromptFile));
    for (var run = 1; run <= cfg.RunsPerTask; run++)
    {
        LlmResult res;
        try
        {
            res = fake ? Fake(m.Label, t.Id, run)
                       : await Llm.CompleteAsync(m.Provider, m.Model, prompt, maxTokens: 1500, baseUrl: m.BaseUrl, apiKeyEnv: m.ApiKeyEnv);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{m.Label,-16} {t.Id,-16} run {run}: ERROR {ex.Message.Split('\n')[0]}");
            rows.Add(new Row(m.Label, t.Id, run, false, 0, 0, 0, 0, "error"));
            continue;
        }
        var pass = t.MustContain.All(s => res.Text.Contains(s, StringComparison.OrdinalIgnoreCase))
                && !t.MustNotContain.Any(s => res.Text.Contains(s, StringComparison.OrdinalIgnoreCase));
        var cost = res.InputTokens / 1e6 * m.InputPerMTok + res.OutputTokens / 1e6 * m.OutputPerMTok;
        rows.Add(new Row(m.Label, t.Id, run, pass, res.Seconds, res.InputTokens, res.OutputTokens, cost, "ok"));
        Console.WriteLine($"{m.Label,-16} {t.Id,-16} run {run}: {(pass ? "PASS" : "fail")} {res.Seconds,6:F1}s in={res.InputTokens} out={res.OutputTokens}");
    }
}

var csv = new StringBuilder("model,task,run,pass,seconds,input_tokens,output_tokens,cost_usd,status\n");
foreach (var r in rows)
    csv.AppendLine(string.Join(",", r.Model, r.Task, r.Run, r.Pass ? 1 : 0, r.Seconds.ToString("F2", inv),
        r.InputTokens, r.OutputTokens, r.Cost.ToString("F6", inv), r.Status));
File.WriteAllText("results.csv", csv.ToString());

Console.WriteLine($"\n{"model",-16} {"task",-16} {"pass",7} {"p50 s",7} {"$/attempt",10} {"$/success",10}");
foreach (var g in rows.GroupBy(r => (r.Model, r.Task)))
{
    int n = g.Count(), k = g.Count(r => r.Pass);
    var secs = g.Where(r => r.Status == "ok").Select(r => r.Seconds).OrderBy(x => x).ToList();
    var p50 = secs.Count == 0 ? 0 : secs[secs.Count / 2];
    var perAttempt = g.Average(r => r.Cost);
    var perSuccess = k == 0 ? "never" : (g.Sum(r => r.Cost) / k).ToString("F4", inv);
    Console.WriteLine($"{g.Key.Model,-16} {g.Key.Task,-16} {k,3}/{n,-3} {p50,7:F1} {perAttempt,10:F4} {perSuccess,10}");
}
Console.WriteLine("\nwrote results.csv — copy the numbers into your model-selection matrix.");

static LlmResult Fake(string label, string task, int run)
{
    // Deterministic canned answers so you can check the pipeline and the arithmetic before spending money.
    var weak = label.Contains("local") && run % 2 == 0;
    var text = task switch
    {
        "sql-risk" => weak ? "The GROUP BY line." : "FROM dbo.Orders AS o WITH (NOLOCK) — dirty reads can show uncommitted or double-counted rows.",
        "csharp-deadlock" => weak ? "var orders = _orderApiClient.GetOrdersAsync(customerId).Result;"
                                  : "public async Task<ActionResult> CustomerOrders(int customerId) { var orders = await _orderApiClient.GetOrdersAsync(customerId); return View(orders); }",
        _ => """{"component": "Orders", "severity": "high", "reason": "Cross-tenant data exposure on the orders screen."}"""
    };
    return new LlmResult(text, 400 + run, 60 + run, label.Contains("local") ? 3.0 + run : 1.0 + 0.1 * run);
}

record Config(string PricesAsOf, int RunsPerTask, List<ModelCfg> Models);
record ModelCfg(string Label, string Provider, string Model, string? BaseUrl, string? ApiKeyEnv, double InputPerMTok, double OutputPerMTok);
record BenchTask(string Id, string PromptFile, List<string> MustContain, List<string> MustNotContain);
record Row(string Model, string Task, int Run, bool Pass, double Seconds, int InputTokens, int OutputTokens, double Cost, string Status);
