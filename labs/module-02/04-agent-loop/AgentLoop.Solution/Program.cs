// AgentLoop (REFERENCE SOLUTION) — lesson 02.4. Same loop as the starter, with the four BREAK-IT spots fixed:
//   FIX #1 validate arguments against the schema before running a tool; tell the model what was wrong
//   FIX #2 a failing tool becomes an is_error tool_result the model can reason about, not a crash
//   FIX #3 turn cap + repeated-identical-call detection + a token budget
//   FIX #4 authorization lives in the harness: paths are confined to the workspace, output is size-capped
//   dotnet run -- --fake        (AGENT_BREAK=malformed|toolfail|loop|escape)      dotnet run   (real model)
using System.Text;
using System.Text.Json.Nodes;

Console.OutputEncoding = Encoding.UTF8;
var fake = args.Contains("--fake");
var breakMode = Environment.GetEnvironmentVariable("AGENT_BREAK") ?? "none";
var model = Environment.GetEnvironmentVariable("AGENT_MODEL") ?? "claude-sonnet-5"; // as of 2026-09; set your own
var workspace = Path.GetFullPath(Environment.GetEnvironmentVariable("AGENT_WORKSPACE") ?? "../workspace");
var question = args.FirstOrDefault(a => !a.StartsWith("--")) ??
    "Which stored procedure does OrderService use to load a customer's orders, and which columns does it return? Cite the files you read.";
const int MaxTurns = 12, MaxRepeats = 2, MaxToolOutputChars = 20_000, InputTokenBudget = 200_000;

const string SystemPrompt =
    "You answer questions about the C# / SQL Server codebase in the workspace. " +
    "Use the tools to read files; never guess file contents. Cite every file you relied on. " +
    "If a tool returns an error, either correct your call or say plainly what you could not verify.";

var tools = JsonNode.Parse("""
[
  { "name": "list_files",
    "description": "List files under a workspace directory. Input: a path relative to the workspace root, e.g. 'src'. Returns one relative path per line.",
    "input_schema": { "type": "object", "properties": { "dir": { "type": "string" } }, "required": ["dir"], "additionalProperties": false } },
  { "name": "read_file",
    "description": "Read a UTF-8 text file from the workspace. Input: a path relative to the workspace root, e.g. 'src/OrderService.cs'. Paths outside the workspace are refused.",
    "input_schema": { "type": "object", "properties": { "path": { "type": "string" } }, "required": ["path"], "additionalProperties": false } }
]
""")!.AsArray();

var messages = new JsonArray(new JsonObject { ["role"] = "user", ["content"] = question });
var seenCalls = new Dictionary<string, int>();
var inputTokensUsed = 0;
Console.WriteLine($"workspace: {workspace}\nmode: {(fake ? $"fake ({breakMode})" : model)}\n> {question}\n");

for (var turn = 1; ; turn++)
{
    if (turn > MaxTurns) { Console.WriteLine($"STOP: turn cap ({MaxTurns}) reached without an answer."); break; }     // FIX #3
    if (inputTokensUsed > InputTokenBudget) { Console.WriteLine($"STOP: input-token budget spent ({inputTokensUsed:N0})."); break; }

    var response = fake ? FakeModel.Next(messages, breakMode) : await CallModel(messages);
    var content = response["content"]!.AsArray();
    inputTokensUsed += (int?)response["usage"]?["input_tokens"] ?? 0;
    messages.Add(new JsonObject { ["role"] = "assistant", ["content"] = content.DeepClone() });
    Console.WriteLine($"[turn {turn}] stop_reason={response["stop_reason"]} " +
                      $"in={response["usage"]?["input_tokens"]} out={response["usage"]?["output_tokens"]}");
    foreach (var block in content.Where(b => (string?)b!["type"] == "text"))
        Console.WriteLine($"  model: {block!["text"]}");

    var stop = (string?)response["stop_reason"];
    if (stop == "max_tokens") Console.WriteLine("WARN: answer truncated by max_tokens; do not treat it as complete.");
    if (stop != "tool_use") break;

    var results = new JsonArray();
    var repeated = false;
    foreach (var call in content.Where(b => (string?)b!["type"] == "tool_use"))
    {
        var name = (string?)call!["name"] ?? "";
        var input = call["input"] as JsonObject ?? new JsonObject();
        var key = name + input.ToJsonString();
        seenCalls[key] = seenCalls.GetValueOrDefault(key) + 1;
        if (seenCalls[key] > MaxRepeats) repeated = true;                                                               // FIX #3

        var (output, isError) = Execute(name, input);
        Console.WriteLine($"  tool: {name} {input.ToJsonString()} -> {(isError ? "ERROR " + output : output.Length + " chars")}");
        results.Add(new JsonObject
        {
            ["type"] = "tool_result", ["tool_use_id"] = (string)call["id"]!, ["content"] = output, ["is_error"] = isError
        });
    }
    messages.Add(new JsonObject { ["role"] = "user", ["content"] = results });
    File.WriteAllText("trace.json", messages.ToJsonString(new() { WriteIndented = true }));
    if (repeated) { Console.WriteLine($"STOP: the same tool call was made more than {MaxRepeats} times; the agent is looping."); break; }
}

(string Output, bool IsError) Execute(string name, JsonObject input)
{
    try                                                                                                                 // FIX #2
    {
        return name switch
        {
            "list_files" => (ListFiles(RequireString(input, "dir")), false),                                            // FIX #1
            "read_file" => (ReadFile(RequireString(input, "path")), false),
            _ => ($"Unknown tool '{name}'. Available tools: list_files, read_file.", true)
        };
    }
    catch (ArgumentException ex) { return (ex.Message, true); }            // bad arguments: tell the model how to fix the call
    catch (UnauthorizedAccessException ex) { return (ex.Message, true); }  // policy refusal: the model must not retry around it
    catch (IOException ex) { return ($"I/O error: {ex.Message} You may retry once, or report what you could not read.", true); }
}

static string RequireString(JsonObject input, string field) =>
    input[field] is JsonValue v && v.TryGetValue<string>(out var s) && !string.IsNullOrWhiteSpace(s)
        ? s
        : throw new ArgumentException(
            $"Invalid arguments: required string field '{field}' is missing. You sent {input.ToJsonString()}. " +
            $"Call the tool again with {{\"{field}\": \"<relative path>\"}}.");

string Resolve(string relative)                                                                                         // FIX #4
{
    var full = Path.GetFullPath(Path.Combine(workspace, relative));
    var root = workspace.EndsWith(Path.DirectorySeparatorChar) ? workspace : workspace + Path.DirectorySeparatorChar;
    if (!full.StartsWith(root, StringComparison.OrdinalIgnoreCase) && full != workspace)
        throw new UnauthorizedAccessException($"Refused: '{relative}' resolves outside the workspace. This is a policy decision; do not retry.");
    return full;
}

string ListFiles(string dir) =>
    string.Join("\n", Directory.EnumerateFiles(Resolve(dir), "*", SearchOption.AllDirectories)
        .Select(f => Path.GetRelativePath(workspace, f).Replace('\\', '/')));

string ReadFile(string path)
{
    if (breakMode == "toolfail" && path.EndsWith(".sql"))
        throw new IOException("The process cannot access the file because it is being used by another process.");
    var full = Resolve(path);
    if (!File.Exists(full)) throw new ArgumentException($"No such file '{path}'. Use list_files to see what exists.");
    var text = File.ReadAllText(full);
    return text.Length <= MaxToolOutputChars ? text
        : text[..MaxToolOutputChars] + $"\n[truncated: file has {text.Length:N0} chars, showing the first {MaxToolOutputChars:N0}]";
}

async Task<JsonNode> CallModel(JsonArray history)
{
    var body = new JsonObject
    {
        ["model"] = model, ["max_tokens"] = 2048, ["system"] = SystemPrompt,
        ["tools"] = tools.DeepClone(), ["messages"] = history.DeepClone()
    };
    using var http = new HttpClient();
    using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
    req.Headers.Add("x-api-key", Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")
        ?? throw new InvalidOperationException("Set ANTHROPIC_API_KEY, or run with --fake."));
    req.Headers.Add("anthropic-version", "2023-06-01");
    req.Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");
    var res = await http.SendAsync(req);
    var raw = await res.Content.ReadAsStringAsync();
    if (!res.IsSuccessStatusCode) throw new HttpRequestException($"{(int)res.StatusCode}: {raw}");
    return JsonNode.Parse(raw)!;
}
