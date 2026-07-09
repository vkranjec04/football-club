namespace FootballClub.Web.Features.Ai;

public class AiOptions
{
    public string ApiKey { get; set; } = string.Empty;

    public string Model { get; set; } = "gemini-2.5-flash";

    public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta";

    public int MaxOutputTokens { get; set; } = 2048;

    public int MaxInputChars { get; set; } = 2000;
}
