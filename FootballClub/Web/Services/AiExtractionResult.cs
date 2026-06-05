using System.Text.Json;

namespace FootballClub.Web.Services;

/// <summary>
/// Raw structured fields extracted by the AI, keyed by field name. Untrusted output —
/// callers re-validate and resolve any names/enums against the database.
/// </summary>
public class AiExtractionResult
{
    public bool Success { get; set; }

    public string? Error { get; set; }

    public List<string> Warnings { get; set; } = new();

    public Dictionary<string, JsonElement> Fields { get; set; } = new();

    public static AiExtractionResult Failed(string error) => new() { Success = false, Error = error };
}
