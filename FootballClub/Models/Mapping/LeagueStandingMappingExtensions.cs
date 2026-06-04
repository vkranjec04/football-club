using FootballClub.Web.Dto;

namespace FootballClub.Models.Mapping;

public static class LeagueStandingMappingExtensions
{
    public static LeagueStandingDto ToDto(this LeagueStanding leagueStanding)
    {
        return new LeagueStandingDto
        {
            Id = leagueStanding.Id,
            ClubId = leagueStanding.ClubId,
            ClubName = leagueStanding.Club?.Name,
            Played = leagueStanding.Played,
            Wins = leagueStanding.Wins,
            Draws = leagueStanding.Draws,
            Losses = leagueStanding.Losses,
            GoalsFor = leagueStanding.GoalsFor,
            GoalsAgainst = leagueStanding.GoalsAgainst,
            GoalDiff = leagueStanding.GoalDiff,
            Points = leagueStanding.Points
        };
    }
}