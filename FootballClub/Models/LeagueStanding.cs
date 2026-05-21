namespace FootballClub.Models;

public class LeagueStanding
{
    public int Id { get; set; }
    public int ClubId { get; set; }
    public virtual Club Club { get; set; } = null!;
    public int Played { get; set; }
    public int Wins { get; set; }
    public int Draws { get; set; }
    public int Losses { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDiff => GoalsFor - GoalsAgainst;
    public int Points { get; set; }
}