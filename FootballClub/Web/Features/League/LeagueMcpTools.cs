using System.ComponentModel;
using FootballClub.Data;
using FootballClub.Models.Mapping;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;

namespace FootballClub.Web.Features.League;

/// <summary>MCP tool exposing the league table.</summary>
[McpServerToolType]
public static class LeagueMcpTools
{
    [McpServerTool(Name = "get_league_standings")]
    [Description("Gets the league table ordered by points, then goal difference, then goals for.")]
    public static List<LeagueStandingDto> GetLeagueStandings(ApplicationDbContext context)
    {
        return context.LeagueStandings
            .Include(standing => standing.Club)
            .AsEnumerable()
            .OrderByDescending(standing => standing.Points)
            .ThenByDescending(standing => standing.GoalDiff)
            .ThenByDescending(standing => standing.GoalsFor)
            .Select(standing => standing.ToDto())
            .ToList();
    }
}
