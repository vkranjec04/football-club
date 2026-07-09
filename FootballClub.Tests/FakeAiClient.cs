using System.Text.Json;

namespace FootballClub.Tests;

/// <summary>
/// Deterministic <see cref="IAiClient"/> for endpoint tests. Configure <see cref="NextExtraction"/>
/// and <see cref="NextChat"/> per test; the last call's inputs are captured for assertions.
/// </summary>
internal sealed class FakeAiClient : IAiClient
{
    public AiExtractionResult NextExtraction { get; set; } = new() { Success = true };

    public AiChatResult NextChat { get; set; } = new() { Success = true, Reply = "FAKE_REPLY" };

    public string? LastExtractText { get; private set; }

    public IReadOnlyList<AiChatMessage>? LastMessages { get; private set; }

    public string? LastClubContext { get; private set; }

    public Task<AiExtractionResult> ExtractAsync(string text, AiEntitySchema schema, CancellationToken cancellationToken = default)
    {
        LastExtractText = text;
        return Task.FromResult(NextExtraction);
    }

    public Task<AiChatResult> ChatAsync(IReadOnlyList<AiChatMessage> messages, string clubContext, CancellationToken cancellationToken = default)
    {
        LastMessages = messages;
        LastClubContext = clubContext;
        return Task.FromResult(NextChat);
    }

    /// <summary>Builds a successful extraction result from an anonymous object of field values.</summary>
    public static AiExtractionResult Extraction(object fields)
    {
        var json = JsonSerializer.Serialize(fields);
        var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? new();
        return new AiExtractionResult { Success = true, Fields = dict };
    }
}
