namespace FootballClub.Web.Services;

/// <summary>
/// Abstracts the AI provider. Implemented by <see cref="GeminiAiClient"/> (Google Gemini,
/// when an API key is configured) and <see cref="NullAiClient"/> (no-op fallback). Mirrors
/// the IFileStorage abstraction so the provider is swappable without touching callers.
/// </summary>
public interface IAiClient
{
    /// <summary>Extracts structured fields from a free-text prompt according to <paramref name="schema"/>.</summary>
    Task<AiExtractionResult> ExtractAsync(string text, AiEntitySchema schema, CancellationToken cancellationToken = default);

    /// <summary>Answers a conversation grounded in <paramref name="clubContext"/> (a compact data snapshot).</summary>
    Task<AiChatResult> ChatAsync(IReadOnlyList<AiChatMessage> messages, string clubContext, CancellationToken cancellationToken = default);
}
