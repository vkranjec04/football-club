using System.ComponentModel;
using FootballClub.Data;
using FootballClub.Models.Mapping;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace FootballClub.Web.Features.Clubs;

/// <summary>
/// MCP tools exposing club data to agentic IDEs (Claude Code, VS Code, Cursor, ...) connected
/// to the app's MCP server (see Program.cs, route "/mcp"). Mirrors the anonymous read surface
/// of <see cref="ClubsApiController"/>; the DbContext parameter is resolved from DI per call,
/// not from the tool's JSON schema (see remarks on <see cref="McpServerToolAttribute"/>).
/// </summary>
[McpServerToolType]
public static class ClubMcpTools
{
    [McpServerTool(Name = "list_clubs")]
    [Description("Lists football clubs, optionally filtered by name, city or league name.")]
    public static List<ClubDto> ListClubs(
        ApplicationDbContext context,
        [Description("Optional case-insensitive filter matched against name, city or league name.")] string? search = null)
    {
        var q = (search ?? string.Empty).Trim();
        var query = context.Clubs.Include(club => club.HomeStadium).AsEnumerable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(club =>
                club.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                club.City.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                club.LeagueName.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        return query.OrderBy(club => club.Name).Select(club => club.ToDto()).ToList();
    }

    [McpServerTool(Name = "get_club")]
    [Description("Gets a single club by id, including its home stadium name.")]
    public static ClubDto GetClub(ApplicationDbContext context, [Description("Club id.")] int id)
    {
        var club = context.Clubs.Include(c => c.HomeStadium).FirstOrDefault(c => c.Id == id);
        return club?.ToDto() ?? throw new McpException($"Club {id} was not found.");
    }
}
