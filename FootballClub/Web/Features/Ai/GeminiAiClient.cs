using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace FootballClub.Web.Features.Ai;

/// <summary>
/// Calls the Google Gemini API (generateContent). For extraction it forces a JSON
/// response schema; for chat it returns free text grounded in a club-data snapshot.
/// Registered as IAiClient only when Ai:ApiKey is configured; otherwise NullAiClient.
/// </summary>
public class GeminiAiClient : IAiClient
{
    private readonly HttpClient _http;
    private readonly AiOptions _options;
    private readonly ILogger<GeminiAiClient> _logger;

    public GeminiAiClient(HttpClient http, IOptions<AiOptions> options, ILogger<GeminiAiClient> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AiExtractionResult> ExtractAsync(string text, AiEntitySchema schema, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            return AiExtractionResult.Failed("AI is not configured (missing Ai:ApiKey).");
        }

        var systemPrompt = schema.Instructions
            + " Fill only the fields you can confidently infer from the text; leave the rest null."
            + " Return dates in ISO 8601 (yyyy-MM-dd, or yyyy-MM-ddTHH:mm:ss when a time is given).";

        var body = new
        {
            systemInstruction = new { parts = new[] { new { text = systemPrompt } } },
            contents = new[] { new { role = "user", parts = new[] { new { text = Trim(text) } } } },
            generationConfig = new
            {
                temperature = 0,
                maxOutputTokens = _options.MaxOutputTokens,
                responseMimeType = "application/json",
                thinkingConfig = new { thinkingBudget = 0 },
                responseSchema = BuildSchema(schema.Fields)
            }
        };

        var (ok, payload, status) = await CallAsync(body, cancellationToken);
        if (!ok)
        {
            _logger.LogWarning("Gemini extract returned {Status}: {Body}", status, payload);
            return AiExtractionResult.Failed($"AI request failed (HTTP {status}).");
        }

        var json = ExtractFirstText(payload);
        if (string.IsNullOrWhiteSpace(json))
        {
            return AiExtractionResult.Failed("AI returned no data.");
        }

        try
        {
            var fields = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? new();
            return new AiExtractionResult { Success = true, Fields = fields };
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse Gemini extraction JSON.");
            return AiExtractionResult.Failed("AI response could not be parsed.");
        }
    }

    public async Task<AiChatResult> ChatAsync(IReadOnlyList<AiChatMessage> messages, string clubContext, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            return AiChatResult.Failed("AI is not configured (missing Ai:ApiKey).");
        }

        if (messages.Count == 0)
        {
            return AiChatResult.Failed("No message provided.");
        }

        var systemPrompt =
            "You are the AI assistant inside a football club's management app. "
            + "Answer questions about THIS club using the DATA below, and give practical football advice "
            + "(tactics, line-ups, training drills) using your expertise. Be concise and well-structured. "
            + "Reply in the same language as the user's last message (Croatian or English). "
            + "If the data does not contain something, say so briefly and give your best general advice.\n\n"
            + "=== CLUB DATA ===\n" + clubContext;

        var contents = messages.Select(m => new
        {
            role = string.Equals(m.Role, "model", StringComparison.OrdinalIgnoreCase) ? "model" : "user",
            parts = new[] { new { text = Trim(m.Text) } }
        }).ToArray();

        var body = new
        {
            systemInstruction = new { parts = new[] { new { text = systemPrompt } } },
            contents,
            generationConfig = new
            {
                temperature = 0.4,
                maxOutputTokens = _options.MaxOutputTokens,
                thinkingConfig = new { thinkingBudget = 0 }
            }
        };

        var (ok, payload, status) = await CallAsync(body, cancellationToken);
        if (!ok)
        {
            _logger.LogWarning("Gemini chat returned {Status}: {Body}", status, payload);
            return AiChatResult.Failed($"AI request failed (HTTP {status}).");
        }

        var reply = ExtractFirstText(payload);
        return string.IsNullOrWhiteSpace(reply)
            ? AiChatResult.Failed("The assistant could not produce a reply.")
            : new AiChatResult { Success = true, Reply = reply.Trim() };
    }

    private string Trim(string text)
        => text.Length > _options.MaxInputChars ? text[.._options.MaxInputChars] : text;

    private async Task<(bool ok, string payload, int status)> CallAsync(object body, CancellationToken ct)
    {
        var url = $"{_options.BaseUrl.TrimEnd('/')}/models/{_options.Model}:generateContent";
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = JsonContent.Create(body) };
            request.Headers.Add("x-goog-api-key", _options.ApiKey);

            using var response = await _http.SendAsync(request, ct);
            var payload = await response.Content.ReadAsStringAsync(ct);
            return (response.IsSuccessStatusCode, payload, (int)response.StatusCode);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Gemini call failed.");
            return (false, ex.Message, 0);
        }
    }

    private static object BuildSchema(IReadOnlyList<AiFieldSpec> fields)
    {
        var properties = new Dictionary<string, object>();
        foreach (var field in fields)
        {
            var prop = new Dictionary<string, object?> { ["nullable"] = true };
            if (!string.IsNullOrWhiteSpace(field.Description))
            {
                prop["description"] = field.Description;
            }

            switch (field.Type)
            {
                case AiFieldType.Integer:
                    prop["type"] = "INTEGER";
                    break;
                case AiFieldType.Number:
                    prop["type"] = "NUMBER";
                    break;
                case AiFieldType.Boolean:
                    prop["type"] = "BOOLEAN";
                    break;
                case AiFieldType.Enum:
                    prop["type"] = "STRING";
                    prop["enum"] = field.EnumValues ?? (IReadOnlyList<string>)Array.Empty<string>();
                    break;
                default:
                    prop["type"] = "STRING";
                    break;
            }

            properties[field.Name] = prop;
        }

        return new { type = "OBJECT", properties };
    }

    private static string? ExtractFirstText(string payload)
    {
        using var doc = JsonDocument.Parse(payload);
        if (!doc.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
        {
            return null;
        }

        var first = candidates[0];
        if (!first.TryGetProperty("content", out var content) ||
            !content.TryGetProperty("parts", out var parts) ||
            parts.GetArrayLength() == 0)
        {
            return null;
        }

        return parts[0].TryGetProperty("text", out var textProp) ? textProp.GetString() : null;
    }
}
