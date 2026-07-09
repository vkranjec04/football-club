namespace FootballClub.Web.Features.Ai;

public class AssistantMessageDto
{
    public string Role { get; set; } = "user";

    public string Text { get; set; } = string.Empty;
}
