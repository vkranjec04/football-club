using System.ComponentModel;
using FootballClub.Data;
using FootballClub.Models.Mapping;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;

namespace FootballClub.Web.Features.Players;

/// <summary>MCP tool exposing per-match player statistics; mirrors <see cref="PlayerStatsApiController"/>.</summary>
[McpServerToolType]
public static class PlayerStatMcpTools
{
    [McpServerTool(Name = "get_player_stats")]
    [Description("Gets per-match statistics (goals, assists, minutes, cards, rating), optionally filtered by player name.")]
    public static List<PlayerStatDto> GetPlayerStats(
        ApplicationDbContext context,
        [Description("Optional case-insensitive player name filter.")] string? playerName = null)
    {
        var query = context.PlayerStats
            .Include(stat => stat.Player)
            .Include(stat => stat.Match).ThenInclude(match => match.HomeClub)
            .Include(stat => stat.Match).ThenInclude(match => match.AwayClub)
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(playerName))
        {
            query = query.Where(stat => stat.Player.FullName.Contains(playerName, StringComparison.OrdinalIgnoreCase));
        }

        return query
            .OrderByDescending(stat => stat.Match.Date)
            .Select(stat => stat.ToDto())
            .ToList();
    }
}
