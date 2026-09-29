// ArchCheck - Module 12 lab tool. Offline checks for an enterprise AI-agent architecture.
//   hosting   <options.json> [--by price]      eliminate hosting options on hard constraints, then cost per success
//   simulate  <gateway.json>                   token-bucket simulation of per-key quotas at the model gateway
//   chargeback <usage.csv> <prices.json>       cost per team and the share of spend nobody can be charged for
//   lint      <architecture.json> [--rules IDN,SEC] identity, secrets, egress, residency, logging and retention rules
//   redact    <log.jsonl> [--out <file>]       find secrets and personal data in logged prompts; optionally write a redacted copy
//   adr       <dir>                            ADR structure, status, supersession and conflicting accepted decisions
//   review    <questionnaire.json> <answers.md> every security-review question answered, with evidence that exists
// Heuristics and models, not proof. All prices, pass rates and loads in the lab files are illustrative.
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
if (args.Length < 2) return Usage();
string? Opt(string name) => args.SkipWhile(a => a != name).Skip(1).FirstOrDefault();
try
{
    return args[0] switch
    {
        "hosting" => Hosting(args[1], Opt("--by") == "price"),
        "simulate" => Simulate(args[1]),
        "chargeback" when args.Length >= 3 => Chargeback(args[1], args[2]),
        "lint" => Lint(args[1], Opt("--rules")),
        "redact" => Redact(args[1], Opt("--out")),
        "adr" => Adr(args[1]),
        "review" when args.Length >= 3 => Review(args[1], args[2]),
        _ => Usage()
    };
}
catch (Exception e) when (e is IOException or JsonException or KeyNotFoundException or FormatException or InvalidOperationException)
{
    Console.Error.WriteLine($"error: {e.Message}");
    return 2;
}

static int Usage()
{
    Console.Error.WriteLine("usage: ArchCheck hosting <options.json> [--by price] | simulate <gateway.json> | chargeback <usage.csv> <prices.json>");
    Console.Error.WriteLine("                 | lint <architecture.json> [--rules IDN,RES,...] | redact <log.jsonl> [--out <file>] | adr <dir> | review <questionnaire.json> <answers.md>");
    return 2;
}

static JsonElement Load(string path) => JsonDocument.Parse(File.ReadAllText(path), new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true }).RootElement;
static string S(JsonElement e, string p, string d = "") => e.TryGetProperty(p, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() ?? d : d;
static double D(JsonElement e, string p, double d = 0) => e.TryGetProperty(p, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetDouble() : d;
static bool B(JsonElement e, string p, bool d = false) => e.TryGetProperty(p, out var v) && (v.ValueKind == JsonValueKind.True || v.ValueKind == JsonValueKind.False) ? v.GetBoolean() : d;
static List<string> L(JsonElement e, string p) => e.TryGetProperty(p, out var v) && v.ValueKind == JsonValueKind.Array ? v.EnumerateArray().Select(x => x.GetString() ?? "").ToList() : new();
static IEnumerable<JsonElement> A(JsonElement e, string p) => e.TryGetProperty(p, out var v) && v.ValueKind == JsonValueKind.Array ? v.EnumerateArray() : Enumerable.Empty<JsonElement>();

static (double lo, double hi) Wilson(int k, int n, double z = 1.96)
{
    if (n == 0) return (0, 1);
    double p = (double)k / n, den = 1 + z * z / n, centre = (p + z * z / (2 * n)) / den;
    double half = z * Math.Sqrt(p * (1 - p) / n + z * z / (4.0 * n * n)) / den;
    return (Math.Max(0, centre - half), Math.Min(1, centre + half));
}

// ---------------------------------------------------------------- hosting
static int Hosting(string path, bool byPrice)
{
    var root = Load(path);
    var c = root.GetProperty("constraints");
    var geo = S(c, "residency");
    var need = L(c, "features");
    var minPass = D(c, "minPassRate");
    var vol = root.GetProperty("volume");
    double n = D(vol, "attemptsPerMonth"), h = D(vol, "failureReviewCost");
    var options = A(root, "options").ToList();

    if (byPrice)
    {
        Console.WriteLine("Ranking by price per attempt only (no constraints, no pass rate):");
        foreach (var o in options.OrderBy(o => D(o, "costPerAttempt") + D(o, "fixedMonthly") / n))
            Console.WriteLine($"  {S(o, "id"),-30} ${D(o, "costPerAttempt") + D(o, "fixedMonthly") / n,7:F3} per attempt (incl. fixed cost spread over {n:N0} attempts)");
        Console.WriteLine("This is the ranking a price sheet gives you. Run without --by price to apply constraints and pass rates.");
        return 0;
    }

    Console.WriteLine($"Step 1 - hard constraints (eliminate, never weigh): residency={geo}; features={string.Join(",", need)}; pass rate >= {minPass:P0} on our tasks");
    var survivors = new List<JsonElement>();
    foreach (var o in options)
    {
        var why = new List<string>();
        var geos = L(o, "geos");
        if (geo.Length > 0 && !geos.Contains(geo)) why.Add($"inference geo {string.Join("/", geos)} does not include '{geo}'");
        var missing = need.Except(L(o, "features")).ToList();
        if (missing.Count > 0) why.Add($"missing {string.Join(", ", missing)}");
        var p = D(o, "passRate"); var trials = (int)D(o, "trials");
        var (lo, hi) = Wilson((int)Math.Round(p * trials), trials);
        if (p < minPass) why.Add($"pass rate {p:P0} (95% CI {lo:P0}-{hi:P0}, n={trials}) below {minPass:P0}");
        Console.WriteLine(why.Count == 0 ? $"  keep {S(o, "id")}" : $"  OUT  {S(o, "id")}: {string.Join("; ", why)}");
        if (why.Count == 0) survivors.Add(o);
    }
    if (survivors.Count == 0) { Console.WriteLine("No option survives. Relax a constraint explicitly (and write down who accepted it), or change the task."); return 1; }

    Console.WriteLine($"\nStep 2 - survivors at {n:N0} attempts/month, ${h} human review per failed attempt");
    Console.WriteLine($"  {"option",-30} {"p",5} {"95% CI",11} {"$/attempt",10} {"$/success",10} {"+review",9} {"$/month",11}");
    foreach (var o in survivors.OrderBy(o => CostPerSuccess(o, n, h)))
    {
        var p = D(o, "passRate"); var trials = (int)D(o, "trials");
        var (lo, hi) = Wilson((int)Math.Round(p * trials), trials);
        var perAttempt = D(o, "costPerAttempt") + D(o, "fixedMonthly") / n;
        var monthly = n * D(o, "costPerAttempt") + D(o, "fixedMonthly");
        Console.WriteLine($"  {S(o, "id"),-30} {p,5:F2} {lo,5:F2}-{hi,4:F2} {perAttempt,10:F3} {perAttempt / p,10:F3} {CostPerSuccess(o, n, h),9:F2} {monthly,11:N0}");
    }
    Console.WriteLine("\n$/success = $/attempt / p;  +review = $/attempt / p + (1/p - 1) x review cost  (cost per successful task, 02.5).");
    Console.WriteLine("Overlapping intervals mean the pass-rate difference is not established; decide on constraints and cost, then measure more.");
    return 0;
}
static double CostPerSuccess(JsonElement o, double n, double h)
{
    var p = D(o, "passRate"); var c = D(o, "costPerAttempt") + D(o, "fixedMonthly") / n;
    return c / p + (1 / p - 1) * h;
}

// ---------------------------------------------------------------- simulate
static int Simulate(string path)
{
    var root = Load(path);
    var rng = new Random((int)D(root, "seed", 12));
    int seconds = (int)D(root, "seconds", 3600);
    double orgTpm = D(root.GetProperty("org"), "tpm");
    var keys = A(root, "keys").ToDictionary(k => S(k, "id"), k => new Bucket(D(k, "tpm"), D(k, "burst", D(k, "tpm"))));
    var org = new Bucket(orgTpm, orgTpm);
    var loads = A(root, "load").Select(l => new Load(S(l, "id"), S(l, "team"), S(l, "key"), D(l, "users"), D(l, "activeShare", 1),
        D(l, "reqPerMin"), D(l, "tokensMean"), (int)D(l, "from", 0), (int)D(l, "to", seconds))).ToList();
    foreach (var l in loads) if (!keys.ContainsKey(l.Key)) throw new KeyNotFoundException($"load '{l.Id}' uses unknown key '{l.Key}'");

    var sumKeys = keys.Values.Sum(b => b.Tpm);
    Console.WriteLine($"Org limit {orgTpm:N0} TPM; {keys.Count} gateway key(s) with {sumKeys:N0} TPM allocated ({sumKeys / orgTpm:P0} of org); {seconds / 60} simulated minutes, token buckets refilled every second.");
    for (int t = 0; t < seconds; t++)
    {
        org.Refill(); foreach (var b in keys.Values) b.Refill();
        var batch = new List<(Load l, double tok)>();
        foreach (var l in loads)
        {
            if (t < l.From || t >= l.To) continue;
            var lambda = l.Users * l.ActiveShare * l.ReqPerMin / 60.0;
            for (int i = Poisson(rng, lambda); i > 0; i--)
                batch.Add((l, Math.Max(500, -l.TokensMean * Math.Log(1 - rng.NextDouble()))));
        }
        foreach (var (l, tok) in batch.OrderBy(_ => rng.Next()))
        {
            l.Requests++; l.Demand += tok;
            var k = keys[l.Key];
            if (k.Level >= tok && org.Level >= tok) { k.Level -= tok; org.Level -= tok; l.Served += tok; }
            else { l.Throttled++; k.Throttled++; }
        }
    }
    Console.WriteLine($"\n  {"load",-22} {"team",-10} {"key",-14} {"requests",9} {"429s",7} {"429 %",7} {"demand TPM",11}");
    foreach (var l in loads)
        Console.WriteLine($"  {l.Id,-22} {l.Team,-10} {l.Key,-14} {l.Requests,9:N0} {l.Throttled,7:N0} {(l.Requests == 0 ? 0 : (double)l.Throttled / l.Requests),7:P1} {l.Demand / (seconds / 60.0),11:N0}");
    var byTeam = loads.GroupBy(l => l.Team).Select(g => (g.Key, r: g.Sum(x => x.Requests), th: g.Sum(x => x.Throttled))).ToList();
    var worst = byTeam.OrderByDescending(x => x.r == 0 ? 0 : (double)x.th / x.r).First();
    Console.WriteLine($"\nWorst-hit team: {worst.Key} ({(worst.r == 0 ? 0 : (double)worst.th / worst.r):P1} of requests throttled).");
    var victims = byTeam.Where(x => x.r > 0 && (double)x.th / x.r > 0.02).Select(x => x.Key).ToList();
    Console.WriteLine(victims.Count > 1
        ? $"{victims.Count} teams lose more than 2% of requests. If only one of them caused the load, the quota boundary is in the wrong place (noisy neighbour)."
        : "At most one team is throttled above 2%: the quota contains the load where it originates.");
    return victims.Count > 1 ? 1 : 0;
}
static int Poisson(Random r, double lambda)
{
    double l = Math.Exp(-lambda), p = 1; int k = 0;
    do { k++; p *= r.NextDouble(); } while (p > l);
    return k - 1;
}

// ---------------------------------------------------------------- chargeback
static int Chargeback(string csvPath, string pricesPath)
{
    var prices = Load(pricesPath);
    var per = prices.GetProperty("perMTok");
    var heads = prices.TryGetProperty("headcount", out var hc) ? hc.EnumerateObject().ToDictionary(x => x.Name, x => x.Value.GetDouble()) : new();
    var lines = File.ReadAllLines(csvPath).Where(x => x.Trim().Length > 0).ToList();
    var head = lines[0].Split(',').Select(x => x.Trim()).ToList();
    int I(string c) => head.IndexOf(c) is var i && i >= 0 ? i : throw new FormatException($"usage.csv needs a '{c}' column");
    int iKey = I("key"), iTeam = I("team"), iModel = I("model"), iIn = I("input"), iCw = I("cache_write"), iCr = I("cache_read"), iOut = I("output");
    var cost = new Dictionary<string, double>(); var keysOf = new Dictionary<string, HashSet<string>>();
    double total = 0;
    foreach (var line in lines.Skip(1))
    {
        var f = line.Split(',');
        var m = per.GetProperty(f[iModel]);
        var usd = (double.Parse(f[iIn]) * D(m, "input") + double.Parse(f[iCw]) * D(m, "cache_write") + double.Parse(f[iCr]) * D(m, "cache_read") + double.Parse(f[iOut]) * D(m, "output")) / 1e6;
        var team = f[iTeam].Trim().Length == 0 ? "(unattributed)" : f[iTeam].Trim();
        cost[team] = cost.GetValueOrDefault(team) + usd; total += usd;
        (keysOf.TryGetValue(team, out var s) ? s : keysOf[team] = new()).Add(f[iKey]);
    }
    Console.WriteLine($"  {"team",-16} {"keys",-28} {"cost",12} {"share",7} {"devs",5} {"per dev",9}");
    foreach (var (team, usd) in cost.OrderByDescending(x => x.Value))
    {
        var devs = heads.GetValueOrDefault(team);
        Console.WriteLine($"  {team,-16} {string.Join(" ", keysOf[team].OrderBy(x => x)),-28} {usd,12:N0} {usd / total,7:P1} {(devs > 0 ? devs.ToString("N0") : "-"),5} {(devs > 0 ? (usd / devs).ToString("N0") : "-"),9}");
    }
    var un = cost.GetValueOrDefault("(unattributed)");
    Console.WriteLine($"\nTotal {total:N0} {S(prices, "currency", "USD")}. Attribution coverage {(1 - un / total):P1}.");
    if (un / total > 0.05)
    {
        Console.WriteLine($"FAIL {un / total:P1} of spend is on keys with no owning team. Nobody can be asked to explain it, and nobody's budget stops it.");
        return 1;
    }
    Console.WriteLine("PASS every key belongs to a team (coverage above 95%).");
    return 0;
}

// ---------------------------------------------------------------- lint
static int Lint(string path, string? only)
{
    var prefixes = only?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? Array.Empty<string>();
    bool On(string rule) => prefixes.Length == 0 || prefixes.Any(p => rule.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    var a = Load(path);
    var pol = a.GetProperty("policy");
    var geos = L(pol, "residency");
    var maxRet = D(pol, "maxRetentionDays", 30);
    var teams = (int)D(pol, "teams", 1);
    int fails = 0, warns = 0;
    void Fail(string rule, string m) { if (!On(rule)) return; fails++; Console.WriteLine($"FAIL {rule,-14} {m}"); }
    void Warn(string rule, string m) { if (!On(rule)) return; warns++; Console.WriteLine($"WARN {rule,-14} {m}"); }

    foreach (var id in A(a, "identities"))
    {
        var name = S(id, "id"); var kind = S(id, "kind"); var ts = L(id, "teams");
        if (kind != "human" && (ts.Contains("*") || ts.Count > 1))
        {
            var n = ts.Contains("*") ? teams : ts.Count;
            Fail("IDN-SHARED", $"{name}: {kind} credential shared by {n} of {teams} teams. One leak or one runaway job reaches all {n}; spend and actions cannot be attributed to a team.");
        }
        var life = D(id, "lifetimeHours");
        if (kind == "workload" && life > 24) Fail("IDN-LIFETIME", $"{name}: CI/workload credential lives {life:N0} h. Use workload identity federation (e.g. GitHub OIDC) so each job gets a token that expires with it.");
        if (kind == "gateway-key" && life > 90 * 24) Warn("IDN-ROTATE", $"{name}: key lives {life / 24:N0} days; rotate at most every 90 days, or issue it from a vault via apiKeyHelper.");
        var store = S(id, "storedIn");
        if (store is "repo" or "env-file" or "plaintext" or "wiki") Fail("SEC-STORAGE", $"{name}: credential stored in '{store}'. Keep it in a vault and hand it out at run time.");
        if (store == "ci-secret" && kind == "workload") Warn("SEC-STATIC", $"{name}: static secret in CI settings; prefer OIDC federation (no stored secret at all).");
    }
    if (B(a.GetProperty("egress"), "directProviderAccess")) Fail("NET-DIRECT", "clients may call model providers directly, around the gateway: no attribution, no quota, no audit for that traffic. Allow provider hosts only from the gateway's egress.");
    var routes = A(a, "routes").ToList();
    foreach (var r in routes)
        if (!geos.Contains(S(r, "geo"))) Fail("RES-ROUTE", $"route '{S(r, "id")}' ({S(r, "role")}, {S(r, "provider")} {S(r, "endpoint")}) runs inference in '{S(r, "geo")}', outside {string.Join("/", geos)}.{(S(r, "role") == "fallback" ? " A fallback is still a route: on the first outage, prompts go there." : "")}");
    if (routes.Count(r => S(r, "role") == "fallback") == 0) Warn("AVL-FALLBACK", "no fallback route: a provider incident stops every agent in the company.");
    var stores = A(a, "stores").ToList();
    if (!stores.Any(s => S(s, "kind") == "audit-log")) Fail("LOG-AUDIT", "no audit-log store: you cannot answer 'which identity called which tool/model, when'.");
    foreach (var s in stores)
    {
        var id = S(s, "id"); var content = S(s, "content"); var kind = S(s, "kind");
        if (!geos.Contains(S(s, "geo")))
        {
            if (content == "metadata") Warn("RES-STORE", $"{id}: metadata stored in '{S(s, "geo")}'. Allowed only if the policy covers metadata; list it in the data inventory.");
            else Fail("RES-STORE", $"{id}: {content} content stored in '{S(s, "geo")}', outside {string.Join("/", geos)}.");
        }
        if (content == "raw" && kind is not ("provider" or "client-cache")) Fail("LOG-RAW", $"{id}: prompts/responses stored unredacted. Whatever a developer pastes (connection strings, tokens, customer data) is now in a log with its own readers and retention.");
        if (kind == "audit-log" && !B(s, "immutable")) Fail("LOG-IMMUTABLE", $"{id}: audit log is writable by the people it audits. Ship it to append-only (WORM) storage.");
        var ret = D(s, "retentionDays");
        if (content != "metadata" && ret > maxRet) Fail("RET-MAX", $"{id}: keeps {content} content {ret:N0} days, policy max is {maxRet:N0}.");
    }
    foreach (var m in A(a, "mcpServers"))
    {
        var who = S(m, "identity"); var acc = S(m, "access");
        if (who == "shared-token" && acc == "write") Fail("MCP-SHARED", $"{S(m, "id")}: write access through one shared token. Every write looks like the same user; use per-user OAuth or a per-team bot.");
        else if (who == "shared-token") Warn("MCP-SHARED", $"{S(m, "id")}: shared read token; reads are not attributable to a person.");
        if (!geos.Contains(S(m, "geo"))) Fail("RES-MCP", $"{S(m, "id")}: MCP server hosted in '{S(m, "geo")}'. Tool results and arguments are prompt data too.");
    }
    Console.WriteLine(fails == 0 ? $"PASS {S(a, "name")}: 0 failures, {warns} warning(s)" : $"FAIL {S(a, "name")}: {fails} failure(s), {warns} warning(s)");
    return fails == 0 ? 0 : 1;
}

// ---------------------------------------------------------------- redact
static int Redact(string path, string? outPath)
{
    var rules = new (string kind, bool secret, Regex rx)[]
    {
        ("github-token", true, new Regex(@"\b(ghp_[A-Za-z0-9]{20,}|github_pat_[A-Za-z0-9_]{20,})")),
        ("aws-key-id", true, new Regex(@"\bAKIA[0-9A-Z]{16}\b")),
        ("api-key", true, new Regex(@"\bsk-[A-Za-z0-9_-]{20,}")),
        ("jwt", true, new Regex(@"\beyJ[A-Za-z0-9_-]{8,}\.[A-Za-z0-9_-]{8,}\.[A-Za-z0-9_-]{8,}")),
        ("bearer", true, new Regex(@"(?i)\bBearer\s+[A-Za-z0-9._~+/-]{16,}")),
        ("password", true, new Regex(@"(?i)\b(password|pwd)\s*=\s*[^;""'\s]+")),
        ("private-key", true, new Regex(@"-----BEGIN [A-Z ]*PRIVATE KEY-----")),
        ("email", false, new Regex(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b")),
        ("iban", false, new Regex(@"\b[A-Z]{2}\d{2}(?:\s?[A-Z0-9]{4}){3,7}\b")),
    };
    int n = 0, withSecret = 0, secrets = 0, pii = 0;
    var output = new List<string>();
    foreach (var line in File.ReadLines(path))
    {
        if (line.Trim().Length == 0) continue;
        n++;
        var obj = System.Text.Json.Nodes.JsonNode.Parse(line)!.AsObject();
        bool hit = false;
        foreach (var field in new[] { "prompt", "response", "tool_input" })
        {
            if (obj[field] is not System.Text.Json.Nodes.JsonValue jv || !jv.TryGetValue<string>(out var text)) continue;
            var original = text;
            foreach (var (kind, secret, rx) in rules)
                foreach (Match m in rx.Matches(text))
                {
                    if (secret) { secrets++; hit = true; } else pii++;
                    Console.WriteLine($"  line {n,3} {field,-10} {(secret ? "SECRET" : "pii   "),-6} {kind,-13} {Mask(m.Value)}");
                }
            foreach (var (kind, _, rx) in rules) text = rx.Replace(text, $"[REDACTED:{kind}]");
            if (text != original)
            {
                obj[field] = text;
                obj[field + "_sha256"] = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(original)))[..16].ToLowerInvariant();
            }
        }
        if (hit) withSecret++;
        output.Add(obj.ToJsonString(new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
    }
    Console.WriteLine($"\n{n} log lines: {withSecret} contain secrets ({secrets} matches), {pii} personal-data matches.");
    if (outPath is not null)
    {
        var outDir = Path.GetDirectoryName(Path.GetFullPath(outPath));
        if (outDir is not null) Directory.CreateDirectory(outDir);
        File.WriteAllLines(outPath, output);
        Console.WriteLine($"Redacted copy written to {outPath} (matches replaced; a short SHA-256 of each original field kept for correlation).");
    }
    if (secrets > 0) { Console.WriteLine("FAIL secrets in stored prompts. Redact at the gateway before the log is written, and rotate every credential listed above."); return 1; }
    Console.WriteLine(pii > 0 ? "PASS no secrets (personal data present: check it is allowed by the data inventory)" : "PASS no secrets or personal data found by these patterns");
    return 0;
}
static string Mask(string s) => s.Length <= 10 ? "****" : s[..6] + "…" + $"({s.Length} chars)";

// ---------------------------------------------------------------- adr
static int Adr(string dir)
{
    var files = Directory.GetFiles(dir, "*.md").Where(f => Regex.IsMatch(Path.GetFileName(f), @"^\d{4}-")).OrderBy(f => f).ToList();
    if (files.Count == 0) { Console.WriteLine($"FAIL no ADR files (NNNN-title.md) in {dir}"); return 1; }
    var recs = new List<(string file, string num, string status, string topic, string supersedes)>();
    int fails = 0, warns = 0;
    foreach (var f in files)
    {
        var name = Path.GetFileName(f); var text = File.ReadAllText(f).Replace("\r\n", "\n");
        var problems = new List<string>(); var notes = new List<string>();
        var num = name[..4];
        var h1 = Regex.Match(text, @"^# ADR-(\d{4}):\s*(.+)$", RegexOptions.Multiline);
        if (!h1.Success) problems.Add("H1 must be '# ADR-NNNN: Title'");
        else if (h1.Groups[1].Value != num) problems.Add($"H1 number {h1.Groups[1].Value} does not match file number {num}");
        string Meta(string k) => Regex.Match(text, $@"^{k}:\s*(.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase) is { Success: true } m ? m.Groups[1].Value.Trim() : "";
        var status = Meta("Status"); var topic = Meta("Topic"); var sup = Meta("Supersedes");
        if (!Regex.IsMatch(status, @"^(Proposed|Accepted|Rejected|Deprecated|Superseded by ADR-\d{4})$")) problems.Add($"Status '{status}' is not Proposed | Accepted | Rejected | Deprecated | Superseded by ADR-NNNN");
        if (topic.Length == 0) problems.Add("no 'Topic:' line (needed to detect two accepted decisions on one question)");
        string Section(string h) { var m = Regex.Match(text, $@"^## {Regex.Escape(h)}\s*\n(.*?)(?=^## |\z)", RegexOptions.Multiline | RegexOptions.Singleline); return m.Success ? m.Groups[1].Value : "\u0000"; }
        foreach (var h in new[] { "Context", "Options considered", "Decision", "Consequences", "Confirmation" })
            if (Section(h) == "\u0000") problems.Add($"missing '## {h}'");
        var opts = Section("Options considered");
        if (opts != "\u0000" && Regex.Matches(opts, @"^### ", RegexOptions.Multiline).Count < 2) problems.Add("fewer than two options: a decision with one option is an announcement, not a decision");
        var cons = Section("Consequences");
        if (cons != "\u0000" && !Regex.IsMatch(cons, @"(?im)^\s*[-*]\s*(Negative|Cost|Risk)\b")) { warns++; notes.Add("no negative consequence listed; every real decision costs something"); }
        var conf = Section("Confirmation");
        if (conf != "\u0000" && conf.Trim().Length < 20) { warns++; notes.Add("Confirmation is empty: say how you will know the decision is being followed (a check, a rule, an eval)"); }
        recs.Add((name, num, status, topic, sup));
        if (problems.Count > 0) fails++;
        Console.WriteLine($"{(problems.Count > 0 ? "FAIL" : "ok  ")} {name,-44} {status}");
        foreach (var p in problems) Console.WriteLine($"       - {p}");
        foreach (var p in notes) Console.WriteLine($"       ~ {p}");
    }
    foreach (var g in recs.GroupBy(r => r.num).Where(g => g.Count() > 1)) { fails++; Console.WriteLine($"FAIL number {g.Key} used by {string.Join(", ", g.Select(x => x.file))}: numbers are never reused"); }
    foreach (var r in recs.Where(r => r.status.StartsWith("Superseded by ")))
    {
        var by = r.status[^4..];
        var target = recs.FirstOrDefault(x => x.num == by);
        if (target.file is null) { fails++; Console.WriteLine($"FAIL {r.file} is superseded by ADR-{by}, which does not exist"); }
        else if (!target.supersedes.Contains($"ADR-{r.num}")) { fails++; Console.WriteLine($"FAIL {target.file} does not say 'Supersedes: ADR-{r.num}' (the link must go both ways)"); }
    }
    foreach (var g in recs.Where(r => r.status == "Accepted" && r.topic.Length > 0).GroupBy(r => r.topic.ToLowerInvariant()).Where(g => g.Count() > 1))
    {
        fails++;
        Console.WriteLine($"FAIL two accepted decisions on topic '{g.Key}': {string.Join(", ", g.Select(x => x.file))}. Supersede the older one instead of leaving both in force.");
    }
    Console.WriteLine(fails == 0 ? $"PASS {files.Count} ADRs, {warns} warning(s)" : $"FAIL {fails} problem(s) in {files.Count} ADRs, {warns} warning(s)");
    return fails == 0 ? 0 : 1;
}

// ---------------------------------------------------------------- review
static int Review(string qPath, string answersPath)
{
    var qs = Load(qPath).EnumerateArray().Select(q => (id: S(q, "id"), cat: S(q, "category"), text: S(q, "question"))).ToList();
    var text = File.ReadAllText(answersPath).Replace("\r\n", "\n");
    var baseDir = Path.GetDirectoryName(Path.GetFullPath(answersPath)) ?? ".";
    var blocks = Regex.Matches(text, @"^### (Q\d{2})\b.*?\n(.*?)(?=^### Q\d{2}|\z)", RegexOptions.Multiline | RegexOptions.Singleline)
        .ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value);
    int fails = 0;
    var perCat = new Dictionary<string, (int ok, int all)>();
    foreach (var q in qs)
    {
        var problems = new List<string>();
        if (!blocks.TryGetValue(q.id, out var b)) problems.Add("not answered");
        else
        {
            var ans = Regex.Match(b, @"^Answer:\s*(.*)$", RegexOptions.Multiline).Groups[1].Value.Trim();
            var ev = Regex.Match(b, @"^Evidence:\s*(.*)$", RegexOptions.Multiline).Groups[1].Value.Trim();
            if (ans.Length == 0 || Regex.IsMatch(ans, @"(?i)^(tbd|todo|n/?a|\?)\.?$")) problems.Add("answer is empty or TBD");
            if (ev.Length == 0 || Regex.IsMatch(ev, @"(?i)^(tbd|todo|none|-)\.?$")) problems.Add("no evidence: a reviewer cannot check a bare 'yes'");
            foreach (Match m in Regex.Matches(ev, @"\]\(([^)\s#]+)(#[^)]*)?\)"))
            {
                var target = m.Groups[1].Value;
                if (target.StartsWith("http")) continue;
                if (!File.Exists(Path.Combine(baseDir, target)) && !Directory.Exists(Path.Combine(baseDir, target))) problems.Add($"evidence link '{target}' does not exist");
            }
        }
        var c = perCat.GetValueOrDefault(q.cat);
        perCat[q.cat] = (c.ok + (problems.Count == 0 ? 1 : 0), c.all + 1);
        if (problems.Count > 0) { fails++; Console.WriteLine($"FAIL {q.id} [{q.cat}] {string.Join("; ", problems)}"); }
    }
    Console.WriteLine();
    foreach (var (cat, v) in perCat) Console.WriteLine($"  {cat,-24} {v.ok,2}/{v.all}");
    Console.WriteLine(fails == 0 ? $"PASS all {qs.Count} questions answered with evidence" : $"FAIL {fails} of {qs.Count} questions lack an answer or checkable evidence");
    return fails == 0 ? 0 : 1;
}

sealed class Bucket
{
    public Bucket(double tpm, double cap) { Tpm = tpm; Cap = cap; Level = cap; }
    public double Tpm, Cap, Level; public long Throttled;
    public void Refill() => Level = Math.Min(Cap, Level + Tpm / 60.0);
}
sealed class Load
{
    public Load(string id, string team, string key, double users, double active, double rpm, double tok, int from, int to)
    { Id = id; Team = team; Key = key; Users = users; ActiveShare = active; ReqPerMin = rpm; TokensMean = tok; From = from; To = to; }
    public string Id, Team, Key; public double Users, ActiveShare, ReqPerMin, TokensMean; public int From, To;
    public long Requests, Throttled; public double Demand, Served;
}
