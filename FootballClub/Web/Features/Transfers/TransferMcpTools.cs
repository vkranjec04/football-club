using System.ComponentModel;
using FootballClub.Data;
using FootballClub.Models.Mapping;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;

namespace FootballClub.Web.Features.Transfers;

/// <summary>MCP tool exposing transfer history; mirrors <see cref="TransfersApiController"/>.</summary>
[McpServerToolType]
public static class TransferMcpTools
{
    [McpServerTool(Name = "list_transfers")]
    [Description("Lists player transfer history, optionally filtered by player name, newest first.")]
    public static List<TransferDto> ListTransfers(
        ApplicationDbContext context,
        [Description("Optional case-insensitive player name filter.")] string? playerName = null)
    {
        var query = context.Transfers
            .Include(transfer => transfer.Player)
            .Include(transfer => transfer.FromClub)
            .Include(transfer => transfer.ToClub)
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(playerName))
        {
            query = query.Where(transfer => transfer.Player.FullName.Contains(playerName, StringComparison.OrdinalIgnoreCase));
        }

        return query.OrderByDescending(transfer => transfer.TransferDate).Select(transfer => transfer.ToDto()).ToList();
    }
}
