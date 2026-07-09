namespace FootballClub.Web.Features.Ai;

/// <summary>
/// Fallback used when no AI provider is configured (no Ai:ApiKey). Lets the app and
/// tests run without an API key; mirrors LocalFileStorage as the no-config fallback.
/// </summary>
public class NullAiClient : IAiClient
{
    private const string NotConfigured = "AI is not configured (missing Ai:ApiKey).";

    public Task<AiExtractionResult> ExtractAsync(string text, AiEntitySchema schema, CancellationToken cancellationToken = default)
        => Task.FromResult(AiExtractionResult.Failed(NotConfigured));

    public Task<AiChatResult> ChatAsync(IReadOnlyList<AiChatMessage> messages, string clubContext, CancellationToken cancellationToken = default)
        => Task.FromResult(AiChatResult.Failed(NotConfigured));
}
