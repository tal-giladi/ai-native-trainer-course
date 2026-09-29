// Minimal, dependency-free client for two API shapes, shared by the module 02 labs.
//   provider "anthropic" -> Anthropic Messages API        (needs ANTHROPIC_API_KEY)
//   provider "openai"    -> any OpenAI-compatible endpoint (OpenAI, Ollama, vLLM, LM Studio...)
//                           base URL from OPENAI_BASE_URL (default https://api.openai.com/v1),
//                           key from OPENAI_API_KEY (may be empty for a local Ollama).
// Deliberately raw HttpClient + System.Text.Json so you can see every field that goes over the wire.
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

public sealed record LlmResult(string Text, int InputTokens, int OutputTokens, double Seconds);

public static class Llm
{
    static readonly HttpClient Http = new() { Timeout = TimeSpan.FromMinutes(5) };

    public static async Task<LlmResult> CompleteAsync(
        string provider, string model, string prompt, double? temperature = null, int maxTokens = 1024,
        string? baseUrl = null, string? apiKeyEnv = null)
    {
        var sw = Stopwatch.StartNew();
        return provider switch
        {
            "anthropic" => await Anthropic(model, prompt, temperature, maxTokens, sw),
            "openai" => await OpenAiCompatible(model, prompt, temperature, sw, baseUrl, apiKeyEnv),
            _ => throw new ArgumentException($"Unknown provider '{provider}' (use anthropic or openai)")
        };
    }

    static async Task<LlmResult> Anthropic(string model, string prompt, double? temperature, int maxTokens, Stopwatch sw)
    {
        var body = new JsonObject
        {
            ["model"] = model,
            ["max_tokens"] = maxTokens,
            ["messages"] = new JsonArray(new JsonObject { ["role"] = "user", ["content"] = prompt })
        };
        // Some newer models reject any temperature other than the default; the 400 you get back is part of the lesson.
        if (temperature is not null) body["temperature"] = temperature;
        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
        req.Headers.Add("x-api-key", Env("ANTHROPIC_API_KEY"));
        req.Headers.Add("anthropic-version", "2023-06-01");
        req.Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");
        var json = await Send(req);
        var text = string.Concat(json["content"]!.AsArray()
            .Where(b => (string?)b!["type"] == "text").Select(b => (string?)b!["text"]));
        return new LlmResult(text, (int)json["usage"]!["input_tokens"]!, (int)json["usage"]!["output_tokens"]!,
            sw.Elapsed.TotalSeconds);
    }

    static async Task<LlmResult> OpenAiCompatible(string model, string prompt, double? temperature, Stopwatch sw,
        string? baseUrl, string? apiKeyEnv)
    {
        baseUrl = (baseUrl ?? Environment.GetEnvironmentVariable("OPENAI_BASE_URL") ?? "https://api.openai.com/v1").TrimEnd('/');
        var body = new JsonObject
        {
            ["model"] = model,
            ["messages"] = new JsonArray(new JsonObject { ["role"] = "user", ["content"] = prompt })
        };
        if (temperature is not null) body["temperature"] = temperature;
        using var req = new HttpRequestMessage(HttpMethod.Post, baseUrl + "/chat/completions");
        var key = Environment.GetEnvironmentVariable(apiKeyEnv ?? "OPENAI_API_KEY");
        if (!string.IsNullOrEmpty(key)) req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);
        req.Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");
        var json = await Send(req);
        var text = (string?)json["choices"]![0]!["message"]!["content"] ?? "";
        var usage = json["usage"];
        return new LlmResult(text, (int?)usage?["prompt_tokens"] ?? -1, (int?)usage?["completion_tokens"] ?? -1,
            sw.Elapsed.TotalSeconds);
    }

    static async Task<JsonNode> Send(HttpRequestMessage req)
    {
        using var res = await Http.SendAsync(req);
        var raw = await res.Content.ReadAsStringAsync();
        if (!res.IsSuccessStatusCode) throw new HttpRequestException($"{(int)res.StatusCode}: {raw}");
        return JsonNode.Parse(raw)!;
    }

    public static string Env(string name) =>
        Environment.GetEnvironmentVariable(name) is { Length: > 0 } v
            ? v : throw new InvalidOperationException($"Set the {name} environment variable first.");
}
