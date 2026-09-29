// AgentLoop (STARTER) — lesson 02.4. A complete tool-calling agent loop in one file.
// It works on the happy path. It is deliberately naive in four places marked BREAK-IT; lesson 02.4 breaks each one.
//   dotnet run -- --fake                  scripted model, no API key (set AGENT_BREAK=malformed|toolfail|loop|escape)
//   dotnet run                            real model via the Anthropic Messages API (ANTHROPIC_API_KEY, AGENT_MODEL)
using System.Text;
using System.Text.Json.Nodes;

Console.OutputEncoding = Encoding.UTF8;
var fake = args.Contains("--fake");
var breakMode = Environment.GetEnvironmentVariable("AGENT_BREAK") ?? "none";
var model = Environment.GetEnvironmentVariable("AGENT_MODEL") ?? "claude-sonnet-5"; // as of 2026-09; set your own
var workspace = Path.GetFullPath(Environment.GetEnvironmentVariable("AGENT_WORKSPACE") ?? "../workspace");
var question = args.FirstOrDefault(a => !a.StartsWith("--")) ??
    "Which stored procedure does OrderService use to load a customer's orders, and which columns does it return? Cite the files you read.";

const string SystemPrompt =
    "You answer questions about the C# / SQL Server codebase in the workspace. " +
    "Use the tools to read files; never guess file contents. Cite every file you relied on.";

// 1. TOOL DEFINITIONS: name + description + JSON Schema. This is all the model ever knows about a tool.
var tools = JsonNode.Parse("""
[
  { "name": "list_files",
    "description": "List files under a workspace directory. Input: a path relative to the workspace root, e.g. 'src'. Returns one relative path per line.",
    "input_schema": { "type": "object", "properties": { "dir": { "type": "string" } }, "required": ["dir"] } },
  { "name": "read_file",
    "description": "Read a UTF-8 text file from the workspace. Input: a path relative to the workspace root, e.g. 'src/OrderService.cs'.",
    "input_schema": { "type": "object", "properties": { "path": { "type": "string" } }, "required": ["path"] } }
]
""")!.AsArray();

// 2. CONVERSATION STATE: the whole history is re-sent on every turn. The model has no other memory.
var messages = new JsonArray(new JsonObject { ["role"] = "user", ["content"] = question });
Console.WriteLine($"workspace: {workspace}\nmode: {(fake ? $"fake ({breakMode})" : model)}\n> {question}\n");

// 3. THE LOOP: call the model, run any tools it asked for, append results, repeat until it stops asking.
for (var turn = 1; ; turn++)                                            // BREAK-IT #3: no turn cap, no repeat detection
{
    var response = fake ? FakeModel.Next(messages, breakMode) : await CallModel(messages);
    var content = response["content"]!.AsArray();
    messages.Add(new JsonObject { ["role"] = "assistant", ["content"] = content.DeepClone() });
    Console.WriteLine($"[turn {turn}] stop_reason={response["stop_reason"]} " +
                      $"in={response["usage"]?["input_tokens"]} out={response["usage"]?["output_tokens"]}");
    foreach (var block in content.Where(b => (string?)b!["type"] == "text"))
        Console.WriteLine($"  model: {block!["text"]}");

    if ((string?)response["stop_reason"] != "tool_use") break;          // end_turn, max_tokens, refusal...

    var results = new JsonArray();
    foreach (var call in content.Where(b => (string?)b!["type"] == "tool_use"))
    {
        var name = (string)call!["name"]!;
        var input = call["input"]!;
        Console.WriteLine($"  tool: {name} {input.ToJsonString()}");
        var output = name switch                                          // BREAK-IT #2: a tool exception kills the whole run
        {
            "list_files" => ListFiles((string)input["dir"]!),             // BREAK-IT #1: trusts the model's arguments blindly
            "read_file" => ReadFile((string)input["path"]!),
            _ => throw new InvalidOperationException($"unknown tool {name}")
        };
        Console.WriteLine($"  result: {output.Length} chars");
        results.Add(new JsonObject
        {
            ["type"] = "tool_result", ["tool_use_id"] = (string)call["id"]!, ["content"] = output
        });
    }
    messages.Add(new JsonObject { ["role"] = "user", ["content"] = results });
    File.WriteAllText("trace.json", messages.ToJsonString(new() { WriteIndented = true })); // everything the provider saw
}

// 4. TOOLS: plain C#. The model never runs these; this process does, with this process's permissions.
string ListFiles(string dir) =>
    string.Join("\n", Directory.EnumerateFiles(Path.Combine(workspace, dir), "*", SearchOption.AllDirectories)
        .Select(f => Path.GetRelativePath(workspace, f).Replace('\\', '/')));

string ReadFile(string path)                                             // BREAK-IT #4: no check that path stays inside the workspace
{
    if (breakMode == "toolfail" && path.EndsWith(".sql"))
        throw new IOException("The process cannot access the file because it is being used by another process.");
    return File.ReadAllText(Path.Combine(workspace, path));
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
    req.Headers.Add("x-api-key", Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY"));
    req.Headers.Add("anthropic-version", "2023-06-01");
    req.Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");
    var res = await http.SendAsync(req);
    var raw = await res.Content.ReadAsStringAsync();
    if (!res.IsSuccessStatusCode) throw new HttpRequestException($"{(int)res.StatusCode}: {raw}");
    return JsonNode.Parse(raw)!;
}
