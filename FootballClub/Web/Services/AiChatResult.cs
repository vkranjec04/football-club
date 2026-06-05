namespace FootballClub.Web.Services;

public class AiChatResult
{
    public bool Success { get; set; }

    public string? Error { get; set; }

    public string? Reply { get; set; }

    public static AiChatResult Failed(string error) => new() { Success = false, Error = error };
}
