namespace FootballClub.Web.Features.Ai;

/// <summary>One turn in an assistant conversation. Role is "user" or "model".</summary>
public class AiChatMessage
{
    public string Role { get; set; } = "user";

    public string Text { get; set; } = string.Empty;

    public AiChatMessage()
    {
    }

    public AiChatMessage(string role, string text)
    {
        Role = role;
        Text = text;
    }
}
