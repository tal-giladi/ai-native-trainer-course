// ContractCheck — simulates a multi-service rollout and checks every message contract at every step.
// Usage:
//   dotnet run --project tools/ContractCheck -- rollout <services-dir> <plan.json>
//   dotnet run --project tools/ContractCheck -- owners  <services-dir> <plan.json>
// A service folder holds versions/<version>/contract.json:
//   { "owner": "Team", "emits": { "<message>": { "<field>": "<type>" } },
//                      "reads": { "<message>": { "<field>": "<type>" } } }
// A plan is { "change": "...", "start": { "<service>": "<version>" },
//             "steps": [ { "service": "...", "to": "<version>", "pr": "..." } ] }
// During a step the service runs old and new versions side by side (rolling deploy), so every
// reader version that is live must be satisfied by every emitter version that is live.
using System.Text.Json;

static int Fail(string msg) { Console.Error.WriteLine(msg); return 2; }

if (args.Length != 3 || (args[0] != "rollout" && args[0] != "owners"))
    return Fail("usage: ContractCheck rollout|owners <services-dir> <plan.json>");

var servicesDir = args[1];
JsonDocument plan;
try { plan = JsonDocument.Parse(File.ReadAllText(args[2])); }
catch (Exception e) { return Fail($"cannot read plan: {e.Message}"); }

var contracts = new Dictionary<(string svc, string ver), Contract>();
Contract Load(string svc, string ver)
{
    if (contracts.TryGetValue((svc, ver), out var c)) return c;
    var path = Path.Combine(servicesDir, svc, "versions", ver, "contract.json");
    if (!File.Exists(path)) throw new FileNotFoundException($"no contract for {svc} {ver}: {path}");
    c = Contract.Parse(JsonDocument.Parse(File.ReadAllText(path)).RootElement);
    contracts[(svc, ver)] = c;
    return c;
}

var root = plan.RootElement;
var state = new SortedDictionary<string, string>();
foreach (var p in root.GetProperty("start").EnumerateObject()) state[p.Name] = p.Value.GetString()!;
var steps = root.TryGetProperty("steps", out var s) ? s.EnumerateArray().ToList() : new();

try
{
    if (args[0] == "owners")
    {
        // Who must approve: every team whose service changes, plus every team that reads a message
        // whose emitted shape changes.
        Console.WriteLine($"change: {root.GetProperty("change").GetString()}");
        var cur = new Dictionary<string, string>(state);
        foreach (var st in steps)
        {
            var svc = st.GetProperty("service").GetString()!;
            var to = st.GetProperty("to").GetString()!;
            var before = Load(svc, cur[svc]); var after = Load(svc, to);
            var changed = before.Emits.Keys.Union(after.Emits.Keys)
                .Where(m => !SameShape(before.Emits.GetValueOrDefault(m), after.Emits.GetValueOrDefault(m))).ToList();
            var readers = cur.Where(kv => kv.Key != svc)
                .Where(kv => changed.Any(m => Load(kv.Key, kv.Value).Reads.ContainsKey(m)))
                .Select(kv => $"{kv.Key} ({Load(kv.Key, kv.Value).Owner})").ToList();
            Console.WriteLine($"{Pr(st)} {svc} {cur[svc]} -> {to}: approve {after.Owner}" +
                (readers.Count > 0 ? $"; notify/approve readers of [{string.Join(", ", changed)}]: {string.Join(", ", readers)}" : ""));
            cur[svc] = to;
        }
        return 0;
    }

    var broken = 0;
    Console.WriteLine($"change: {root.GetProperty("change").GetString()}");
    broken += Report("start", state.ToDictionary(kv => kv.Key, kv => new[] { kv.Value }));
    foreach (var st in steps)
    {
        var svc = st.GetProperty("service").GetString()!;
        var to = st.GetProperty("to").GetString()!;
        if (!state.ContainsKey(svc)) return Fail($"step names unknown service {svc}");
        var live = state.ToDictionary(kv => kv.Key, kv => new[] { kv.Value });
        live[svc] = new[] { state[svc], to };
        broken += Report($"{Pr(st)} deploying {svc} {state[svc]} -> {to} (both live)", live);
        state[svc] = to;
        broken += Report($"{Pr(st)} {svc} at {to}", state.ToDictionary(kv => kv.Key, kv => new[] { kv.Value }));
    }
    Console.WriteLine(broken == 0
        ? "RESULT: every step is safe to deploy"
        : $"RESULT: {broken} broken reader(s) across the rollout — do not ship this plan");
    return broken == 0 ? 0 : 1;
}
catch (Exception e) when (e is FileNotFoundException or KeyNotFoundException or InvalidOperationException)
{
    return Fail(e.Message);
}

int Report(string label, Dictionary<string, string[]> live)
{
    var problems = new List<string>();
    foreach (var (reader, rvers) in live)
    foreach (var rv in rvers)
    foreach (var (msg, need) in Load(reader, rv).Reads)
    {
        var emitters = live.Where(kv => kv.Key != reader)
            .SelectMany(kv => kv.Value.Select(v => (svc: kv.Key, ver: v)))
            .Where(e => Load(e.svc, e.ver).Emits.ContainsKey(msg)).ToList();
        if (emitters.Count == 0) { problems.Add($"{reader} {rv} reads '{msg}' but no live service emits it"); continue; }
        foreach (var e in emitters)
        {
            var have = Load(e.svc, e.ver).Emits[msg];
            foreach (var (field, type) in need)
            {
                if (!have.TryGetValue(field, out var t))
                    problems.Add($"{reader} {rv} needs '{msg}.{field}' ({type}); {e.svc} {e.ver} does not send it");
                else if (t != type)
                    problems.Add($"{reader} {rv} needs '{msg}.{field}' as {type}; {e.svc} {e.ver} sends {t}");
            }
        }
    }
    Console.WriteLine($"{(problems.Count == 0 ? "OK   " : "BREAK")} {label}");
    foreach (var p in problems) Console.WriteLine($"        {p}");
    return problems.Count;
}

static string Pr(JsonElement st) => st.TryGetProperty("pr", out var p) ? $"[{p.GetString()}]" : "[step]";

static bool SameShape(Dictionary<string, string>? a, Dictionary<string, string>? b) =>
    a is not null && b is not null && a.Count == b.Count && a.All(kv => b.TryGetValue(kv.Key, out var t) && t == kv.Value);

record Contract(string Owner, Dictionary<string, Dictionary<string, string>> Emits, Dictionary<string, Dictionary<string, string>> Reads)
{
    public static Contract Parse(JsonElement e) => new(
        e.TryGetProperty("owner", out var o) ? o.GetString()! : "(no owner)",
        Section(e, "emits"), Section(e, "reads"));

    static Dictionary<string, Dictionary<string, string>> Section(JsonElement e, string name) =>
        e.TryGetProperty(name, out var sec)
            ? sec.EnumerateObject().ToDictionary(m => m.Name,
                m => m.Value.EnumerateObject().ToDictionary(f => f.Name, f => f.Value.GetString()!))
            : new();
}
