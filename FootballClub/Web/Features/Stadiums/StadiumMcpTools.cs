using System.ComponentModel;
using FootballClub.Data;
using FootballClub.Models.Mapping;
using ModelContextProtocol.Server;

namespace FootballClub.Web.Features.Stadiums;

/// <summary>MCP tool exposing stadium data; mirrors <see cref="StadiumsApiController"/>.</summary>
[McpServerToolType]
public static class StadiumMcpTools
{
    [McpServerTool(Name = "list_stadiums")]
    [Description("Lists stadiums, optionally filtered by name or city.")]
    public static List<StadiumDto> ListStadiums(
        ApplicationDbContext context,
        [Description("Optional case-insensitive filter matched against name or city.")] string? search = null)
    {
        var q = (search ?? string.Empty).Trim();
        var query = context.Stadiums.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(stadium =>
                stadium.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                stadium.City.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        return query.OrderBy(stadium => stadium.Name).Select(stadium => stadium.ToDto()).ToList();
    }
}
