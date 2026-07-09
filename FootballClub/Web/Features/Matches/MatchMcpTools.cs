using System.ComponentModel;
using FootballClub.Data;
using FootballClub.Models.Mapping;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace FootballClub.Web.Features.Matches;

/// <summary>MCP tools exposing match fixtures/results; mirrors <see cref="MatchesApiController"/>.</summary>
[McpServerToolType]
public static class MatchMcpTools
{
    [McpServerTool(Name = "list_matches")]
    [Description("Lists matches, optionally filtered by a club name (home or away) and/or restricted to upcoming fixtures.")]
    public static List<MatchDto> ListMatches(
        ApplicationDbContext context,
        [Description("Optional case-insensitive club name filter (matches home or away club).")] string? clubName = null,
        [Description("When true, only returns matches on or after now.")] bool upcomingOnly = false)
    {
        var query = context.Matches
            .Include(match => match.HomeClub)
            .Include(match => match.AwayClub)
            .Include(match => match.Stadium)
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(clubName))
        {
            query = query.Where(match =>
                match.HomeClub.Name.Contains(clubName, StringComparison.OrdinalIgnoreCase) ||
                match.AwayClub.Name.Contains(clubName, StringComparison.OrdinalIgnoreCase));
        }

        if (upcomingOnly)
        {
            var now = DateTime.Now;
            query = query.Where(match => match.Date >= now);
        }

        return query.OrderBy(match => match.Date).Select(match => match.ToDto()).ToList();
    }

    [McpServerTool(Name = "get_match")]
    [Description("Gets a single match by id.")]
    public static MatchDto GetMatch(ApplicationDbContext context, [Description("Match id.")] int id)
    {
        var match = context.Matches
            .Include(m => m.HomeClub)
            .Include(m => m.AwayClub)
            .Include(m => m.Stadium)
            .FirstOrDefault(m => m.Id == id);

        return match?.ToDto() ?? throw new McpException($"Match {id} was not found.");
    }
}
