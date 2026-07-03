using FootballClub.Data;
using FootballClub.Models;
using FootballClub.Web.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FootballClub.Web.Controllers.Api;

[ApiController]
[Route("api/playerstats")]
public class PlayerStatsApiController : ApiControllerBase
{
    private readonly ApplicationDbContext _context;

    public PlayerStatsApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? search = null, [FromQuery, Range(1, int.MaxValue)] int page = 1, [FromQuery, Range(1, 100)] int pageSize = 10)
    {
        var q = (search ?? string.Empty).Trim();
        var query = _context.PlayerStats
            .Include(stat => stat.Player)
            .Include(stat => stat.Match)
                .ThenInclude(match => match.HomeClub)
            .Include(stat => stat.Match)
                .ThenInclude(match => match.AwayClub)
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(stat =>
                stat.Player.FirstName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                stat.Player.LastName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                $"{stat.Player.FirstName} {stat.Player.LastName}".Contains(q, StringComparison.OrdinalIgnoreCase) ||
                stat.Match.HomeClub.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                stat.Match.AwayClub.Name.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = query.Count();
        var (normalizedPage, normalizedPageSize) = NormalizePaging(page, pageSize);
        var items = query
            .OrderByDescending(stat => stat.Match.Date)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(stat => new PlayerStatDto
            {
                Id = stat.Id,
                PlayerId = stat.PlayerId,
                PlayerName = stat.Player?.FullName,
                MatchId = stat.MatchId,
                MatchLabel = stat.Match == null ? null : $"{stat.Match.HomeClub?.Name ?? stat.Match.HomeClubId.ToString()} vs {stat.Match.AwayClub?.Name ?? stat.Match.AwayClubId.ToString()}",
                Goals = stat.Goals,
                Assists = stat.Assists,
                MinutesPlayed = stat.MinutesPlayed,
                YellowCards = stat.YellowCards,
                RedCard = stat.RedCard,
                Rating = stat.Rating
            })
            .ToList();

        return Ok(CreatePagedResult(items, normalizedPage, normalizedPageSize, totalCount));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var stat = _context.PlayerStats
            .Include(item => item.Player)
            .Include(item => item.Match)
                .ThenInclude(match => match.HomeClub)
            .Include(item => item.Match)
                .ThenInclude(match => match.AwayClub)
            .FirstOrDefault(item => item.Id == id);

        if (stat == null)
        {
            return NotFound();
        }

        return Ok(new PlayerStatDto
        {
            Id = stat.Id,
            PlayerId = stat.PlayerId,
            PlayerName = stat.Player?.FullName,
            MatchId = stat.MatchId,
            MatchLabel = stat.Match == null ? null : $"{stat.Match.HomeClub?.Name ?? stat.Match.HomeClubId.ToString()} vs {stat.Match.AwayClub?.Name ?? stat.Match.AwayClubId.ToString()}",
            Goals = stat.Goals,
            Assists = stat.Assists,
            MinutesPlayed = stat.MinutesPlayed,
            YellowCards = stat.YellowCards,
            RedCard = stat.RedCard,
            Rating = stat.Rating
        });
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost]
    public IActionResult Create([FromBody] PlayerStatCreateDto dto)
    {
        if (!_context.Players.Any(item => item.Id == dto.PlayerId && !item.IsDeleted))
        {
            return NotFound($"Player {dto.PlayerId} was not found.");
        }

        if (!_context.Matches.Any(item => item.Id == dto.MatchId))
        {
            return NotFound($"Match {dto.MatchId} was not found.");
        }

        var stat = new PlayerStat
        {
            PlayerId = dto.PlayerId,
            MatchId = dto.MatchId,
            Goals = dto.Goals,
            Assists = dto.Assists,
            MinutesPlayed = dto.MinutesPlayed,
            YellowCards = dto.YellowCards,
            RedCard = dto.RedCard,
            Rating = dto.Rating
        };

        _context.PlayerStats.Add(stat);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = stat.Id }, GetById(stat.Id) is ObjectResult result && result.Value is PlayerStatDto mapped
            ? mapped
            : new PlayerStatDto { Id = stat.Id });
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] PlayerStatUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var stat = _context.PlayerStats.FirstOrDefault(item => item.Id == id);
        if (stat == null)
        {
            return NotFound();
        }

        if (!_context.Players.Any(item => item.Id == dto.PlayerId && !item.IsDeleted))
        {
            return NotFound($"Player {dto.PlayerId} was not found.");
        }

        if (!_context.Matches.Any(item => item.Id == dto.MatchId))
        {
            return NotFound($"Match {dto.MatchId} was not found.");
        }

        stat.PlayerId = dto.PlayerId;
        stat.MatchId = dto.MatchId;
        stat.Goals = dto.Goals;
        stat.Assists = dto.Assists;
        stat.MinutesPlayed = dto.MinutesPlayed;
        stat.YellowCards = dto.YellowCards;
        stat.RedCard = dto.RedCard;
        stat.Rating = dto.Rating;

        _context.PlayerStats.Update(stat);
        _context.SaveChanges();

        return Ok(GetById(stat.Id) is ObjectResult objectResult && objectResult.Value is PlayerStatDto mapped ? mapped : new PlayerStatDto { Id = stat.Id });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var stat = _context.PlayerStats.FirstOrDefault(item => item.Id == id);
        if (stat == null)
        {
            return NotFound();
        }

        _context.PlayerStats.Remove(stat);
        _context.SaveChanges();
        return NoContent();
    }
}