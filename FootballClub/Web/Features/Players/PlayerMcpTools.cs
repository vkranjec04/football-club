using System.ComponentModel;
using FootballClub.Data;
using FootballClub.Models;
using FootballClub.Models.Enums;
using FootballClub.Models.Mapping;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace FootballClub.Web.Features.Players;

/// <summary>
/// MCP tools exposing player data (and, as the one write tool in the MCP surface, player
/// creation) to agentic IDEs. Read tools mirror <see cref="PlayersApiController"/>; the create
/// tool mirrors its validation (club must exist) so an IDE can add a player the same way a
/// human would through the API, without a JWT - see the "unauthenticated by design" note on
/// the MCP registration in Program.cs.
/// </summary>
[McpServerToolType]
public static class PlayerMcpTools
{
    [McpServerTool(Name = "list_players")]
    [Description("Lists players (excludes soft-deleted), optionally filtered by name/nationality and position/club.")]
    public static List<PlayerDto> ListPlayers(
        ApplicationDbContext context,
        [Description("Optional case-insensitive filter matched against first/last/full name or nationality.")] string? search = null,
        [Description("Optional exact position filter.")] PlayerPosition? position = null,
        [Description("Optional case-insensitive club name filter.")] string? clubName = null)
    {
        var q = (search ?? string.Empty).Trim();
        var query = context.Players
            .Include(player => player.Club)
            .Include(player => player.TrainingSession)
            .Where(player => !player.IsDeleted)
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(player =>
                player.FirstName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                player.LastName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                player.FullName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                player.Nationality.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        if (position.HasValue)
        {
            query = query.Where(player => player.Position == position.Value);
        }

        if (!string.IsNullOrWhiteSpace(clubName))
        {
            query = query.Where(player => player.Club.Name.Contains(clubName, StringComparison.OrdinalIgnoreCase));
        }

        return query
            .OrderBy(player => player.LastName).ThenBy(player => player.FirstName)
            .Select(player => player.ToDto())
            .ToList();
    }

    [McpServerTool(Name = "get_player")]
    [Description("Gets a single player by id, including club and current training session.")]
    public static PlayerDto GetPlayer(ApplicationDbContext context, [Description("Player id.")] int id)
    {
        var player = context.Players
            .Include(p => p.Club)
            .Include(p => p.TrainingSession)
            .FirstOrDefault(p => p.Id == id && !p.IsDeleted);

        return player?.ToDto() ?? throw new McpException($"Player {id} was not found.");
    }

    [McpServerTool(Name = "create_player")]
    [Description("Creates a new player on an existing club. Returns the created player; review it in the app afterwards.")]
    public static PlayerDto CreatePlayer(
        ApplicationDbContext context,
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        string nationality,
        PlayerPosition position,
        int jerseyNumber,
        decimal marketValue,
        DateTime contractUntil,
        [Description("Id of an existing club (see list_clubs/get_club).")] int clubId,
        bool isInjured = false)
    {
        if (!context.Clubs.Any(club => club.Id == clubId))
        {
            throw new McpException($"Club {clubId} was not found.");
        }

        var player = new Player
        {
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            Nationality = nationality,
            Position = position,
            JerseyNumber = jerseyNumber,
            MarketValue = marketValue,
            ContractUntil = contractUntil,
            IsInjured = isInjured,
            ClubId = clubId,
            IsDeleted = false
        };

        context.Players.Add(player);
        context.SaveChanges();

        return context.Players.Include(p => p.Club).First(p => p.Id == player.Id).ToDto();
    }
}
