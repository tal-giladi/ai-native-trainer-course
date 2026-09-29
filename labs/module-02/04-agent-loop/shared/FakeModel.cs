// A scripted stand-in for the model, so the loop can be run, broken and fixed with no API key and zero cost.
// It returns responses shaped exactly like the Anthropic Messages API: content blocks + stop_reason.
// Break modes (env AGENT_BREAK): none | malformed | toolfail | loop | escape
using System.Text.Json.Nodes;

public static class FakeModel
{
    static readonly (string Name, string Json)[] Plan =
    {
        ("list_files", """{"dir":"src"}"""),
        ("read_file", """{"path":"src/OrderService.cs"}"""),
        ("read_file", """{"path":"sql/usp_GetOrdersByCustomer.sql"}"""),
    };

    public static JsonNode Next(JsonArray messages, string mode)
    {
        var results = messages.Where(m => (string?)m!["role"] == "user" && m["content"] is JsonArray)
            .SelectMany(m => m!["content"]!.AsArray())
            .Where(b => (string?)b!["type"] == "tool_result").ToList();
        int ok = results.Count(r => r!["is_error"]?.GetValue<bool>() != true);
        int errors = results.Count - ok;
        var last = results.LastOrDefault();
        int turn = messages.Count(m => (string?)m!["role"] == "assistant") + 1;

        if (mode == "loop")
            return turn > 50
                ? Text("(fake model) 50 identical list_files calls. A real model can do this too, and you pay for every one.")
                : ToolUse(turn, "list_files", """{"dir":"src"}""", "Let me list the files again to be sure.");

        if (last?["is_error"]?.GetValue<bool>() == true && mode == "toolfail")
            return Text("OrderService.GetOrdersByCustomer calls dbo.usp_GetOrdersByCustomer (src/OrderService.cs). " +
                        "I could not read sql/usp_GetOrdersByCustomer.sql (tool error: " + (string?)last["content"] +
                        "), so I cannot confirm the returned columns and I am not going to guess them.");

        if (ok >= Plan.Length)
            return Text("OrderService.GetOrdersByCustomer (src/OrderService.cs) calls dbo.usp_GetOrdersByCustomer " +
                        "(sql/usp_GetOrdersByCustomer.sql). It returns OrderId, OrderNumber, CreatedUtc, StatusCode and " +
                        "TotalAmount (SUM of Quantity * UnitPrice), filtered by tenant, customer, IsDeleted = 0 and an optional FromUtc.");

        var (name, json) = Plan[ok];
        if (mode == "malformed" && ok == 1 && errors == 0)
            json = """{"file":"src/OrderService.cs"}""";                  // wrong key: schema says "path"
        if (mode == "escape" && ok == 1)
            json = """{"path":"../secrets/appsettings.Production.json"}"""; // tries to leave the workspace
        return ToolUse(turn, name, json, ok == 0 ? "I'll start by looking at the source folder." : null);
    }

    static JsonNode ToolUse(int turn, string name, string inputJson, string? thought)
    {
        var content = new JsonArray();
        if (thought is not null) content.Add(new JsonObject { ["type"] = "text", ["text"] = thought });
        content.Add(new JsonObject
        {
            ["type"] = "tool_use", ["id"] = $"toolu_fake_{turn:D2}", ["name"] = name, ["input"] = JsonNode.Parse(inputJson)
        });
        return new JsonObject { ["content"] = content, ["stop_reason"] = "tool_use", ["usage"] = Usage() };
    }

    static JsonNode Text(string text) => new JsonObject
    {
        ["content"] = new JsonArray(new JsonObject { ["type"] = "text", ["text"] = text }),
        ["stop_reason"] = "end_turn", ["usage"] = Usage()
    };

    static JsonNode Usage() => new JsonObject { ["input_tokens"] = 0, ["output_tokens"] = 0 };
}
