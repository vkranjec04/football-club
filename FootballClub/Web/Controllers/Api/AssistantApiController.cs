using System.Text;
using FootballClub.Data;
using FootballClub.Web.Dto;
using FootballClub.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballClub.Web.Controllers.Api;

/// <summary>
/// AI assistant chatbot. Requires authentication (JWT bearer, default scheme) so it is
/// unavailable without login; the browser sends the token from localStorage. Answers are
/// grounded in a compact club-data snapshot built from the database (context stuffing).
/// </summary>
[ApiController]
[Route("api/assistant")]
[Authorize]
public class AssistantApiController : ControllerBase
{
    private const int MaxTurns = 12;

    private readonly IAiClient _ai;
    private readonly ApplicationDbContext _context;

    public AssistantApiController(IAiClient ai, ApplicationDbContext context)
    {
        _ai = ai;
        _context = context;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] AssistantChatRequest? request, CancellationToken cancellationToken)
    {
        if (request?.Messages == null || request.Messages.Count == 0)
        {
            return BadRequest(new { error = "No messages provided." });
        }

        var messages = request.Messages
            .Where(m => !string.IsNullOrWhiteSpace(m.Text))
            .TakeLast(MaxTurns)
            .Select(m => new AiChatMessage(string.Equals(m.Role, "model", StringComparison.OrdinalIgnoreCase) ? "model" : "user", m.Text))
            .ToList();

        if (messages.Count == 0 || messages[^1].Role != "user")
        {
            return BadRequest(new { error = "The last message must be from the user." });
        }

        var clubContext = BuildClubContext();
        var result = await _ai.ChatAsync(messages, clubContext, cancellationToken);
        if (!result.Success)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = result.Error });
        }

        return Ok(new { reply = result.Reply });
    }

    /// <summary>Builds a compact, readable snapshot of the club for grounding the assistant.</summary>
    private string BuildClubContext()
    {
        var club = _context.Clubs.Include(c => c.HomeStadium).FirstOrDefault(c => c.Name.Contains("Dinamo"))
                   ?? _context.Clubs.Include(c => c.HomeStadium).FirstOrDefault();
        if (club == null)
        {
            return "No club data is available.";
        }

        var builder = new StringBuilder();
        builder.AppendLine($"Today: {DateTime.Now:yyyy-MM-dd}");
        builder.AppendLine($"Club: {club.Name} | City: {club.City} | League: {club.LeagueName} | Budget: {club.Budget}M EUR | Stadium: {club.HomeStadium?.Name}");

        var standing = _context.LeagueStandings.FirstOrDefault(s => s.ClubId == club.Id);
        if (standing != null)
        {
            builder.AppendLine($"League record: Played {standing.Played}, W{standing.Wins} D{standing.Draws} L{standing.Losses}, GF {standing.GoalsFor} GA {standing.GoalsAgainst}, Points {standing.Points}");
        }

        var players = _context.Players
            .Where(p => p.ClubId == club.Id && !p.IsDeleted)
            .OrderBy(p => p.Position)
            .ThenByDescending(p => p.MarketValue)
            .ToList();

        builder.AppendLine();
        builder.AppendLine($"Squad ({players.Count} players) [name | position | #jersey | age | nationality | market value (M EUR) | status]:");
        foreach (var p in players)
        {
            builder.AppendLine($"- {p.FirstName} {p.LastName} | {p.Position} | #{p.JerseyNumber} | {p.Age}y | {p.Nationality} | {p.MarketValue}M | {(p.IsInjured ? "INJURED" : "fit")}");
        }

        var staff = _context.StaffMembers.Where(s => !s.IsDeleted && s.ClubId == club.Id).ToList();
        if (staff.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine($"Staff ({staff.Count}):");
            foreach (var s in staff)
            {
                builder.AppendLine($"- {s.FirstName} {s.LastName} | {s.Role}");
            }
        }

        var now = DateTime.Now;

        var upcoming = _context.Matches.Include(m => m.HomeClub).Include(m => m.AwayClub)
            .Where(m => (m.HomeClubId == club.Id || m.AwayClubId == club.Id) && m.Date >= now)
            .OrderBy(m => m.Date).Take(5).ToList();
        if (upcoming.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("Upcoming matches:");
            foreach (var m in upcoming)
            {
                builder.AppendLine($"- {m.Date:yyyy-MM-dd} | {m.HomeClub.Name} vs {m.AwayClub.Name}");
            }
        }

        var recent = _context.Matches.Include(m => m.HomeClub).Include(m => m.AwayClub)
            .Where(m => (m.HomeClubId == club.Id || m.AwayClubId == club.Id) && m.Date < now)
            .OrderByDescending(m => m.Date).Take(5).ToList();
        if (recent.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("Recent results:");
            foreach (var m in recent)
            {
                builder.AppendLine($"- {m.Date:yyyy-MM-dd} | {m.HomeClub.Name} {m.HomeScore}-{m.AwayScore} {m.AwayClub.Name}");
            }
        }

        var training = _context.TrainingSessions
            .Where(t => !t.IsDeleted && t.ClubId == club.Id && t.StartTime >= now)
            .OrderBy(t => t.StartTime).Take(8).ToList();
        if (training.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("Upcoming training sessions:");
            foreach (var t in training)
            {
                builder.AppendLine($"- {t.StartTime:yyyy-MM-dd HH:mm} | {t.Title} | focus: {t.FocusArea} | {t.Intensity}");
            }
        }

        return builder.ToString();
    }
}
