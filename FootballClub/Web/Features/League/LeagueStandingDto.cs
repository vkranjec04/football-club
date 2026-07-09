namespace FootballClub.Web.Features.League;

public class LeagueStandingDto
{
    public int Id { get; set; }
    public int ClubId { get; set; }
    public string? ClubName { get; set; }
    public int Played { get; set; }
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDiff { get; set; }
    public int Points { get; set; }
}