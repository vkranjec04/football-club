using FootballClub.Data;
using FootballClub.Models;
using FootballClub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FootballClub.Web.Features.Players;

[ApiController]
[Route("api/players")]
public class PlayersApiController : ApiControllerBase
{
    private readonly PlayerMockRepository _players;
    private readonly ClubMockRepository _clubs;
    private readonly ApplicationDbContext _context;

    public PlayersApiController(PlayerMockRepository players, ClubMockRepository clubs, ApplicationDbContext context)
    {
        _players = players;
        _clubs = clubs;
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? search = null, [FromQuery, Range(1, int.MaxValue)] int page = 1, [FromQuery, Range(1, 100)] int pageSize = 10)
    {
        var q = (search ?? string.Empty).Trim();
        var query = _players.GetAll().AsEnumerable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(player =>
                player.FirstName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                player.LastName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                $"{player.FirstName} {player.LastName}".Contains(q, StringComparison.OrdinalIgnoreCase) ||
                player.Nationality.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                player.Position.ToString().Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = query.Count();
        var (normalizedPage, normalizedPageSize) = NormalizePaging(page, pageSize);
        var items = query
            .OrderBy(player => player.LastName)
            .ThenBy(player => player.FirstName)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(player => new PlayerDto
            {
                Id = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                FullName = player.FullName,
                DateOfBirth = player.DateOfBirth,
                Nationality = player.Nationality,
                Position = player.Position,
                JerseyNumber = player.JerseyNumber,
                MarketValue = player.MarketValue,
                ContractUntil = player.ContractUntil,
                IsInjured = player.IsInjured,
                IsDeleted = player.IsDeleted,
                ClubId = player.ClubId,
                ClubName = player.Club?.Name,
                TrainingSessionId = player.TrainingSessionId,
                TrainingSessionTitle = player.TrainingSession?.Title,
                Age = player.Age
            })
            .ToList();

        return Ok(CreatePagedResult(items, normalizedPage, normalizedPageSize, totalCount));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var player = _context.Players
            .Include(p => p.Club)
            .Include(p => p.TrainingSession)
            .FirstOrDefault(p => p.Id == id && !p.IsDeleted);

        if (player == null)
        {
            return NotFound();
        }

        return Ok(new PlayerDto
        {
            Id = player.Id,
            FirstName = player.FirstName,
            LastName = player.LastName,
            FullName = player.FullName,
            DateOfBirth = player.DateOfBirth,
            Nationality = player.Nationality,
            Position = player.Position,
            JerseyNumber = player.JerseyNumber,
            MarketValue = player.MarketValue,
            ContractUntil = player.ContractUntil,
            IsInjured = player.IsInjured,
            IsDeleted = player.IsDeleted,
            ClubId = player.ClubId,
            ClubName = player.Club?.Name,
            TrainingSessionId = player.TrainingSessionId,
            TrainingSessionTitle = player.TrainingSession?.Title,
            Age = player.Age
        });
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost]
    public IActionResult Create([FromBody] PlayerCreateDto dto)
    {
        if (!_clubs.GetAll().Any(club => club.Id == dto.ClubId))
        {
            return NotFound($"Club {dto.ClubId} was not found.");
        }

        if (dto.TrainingSessionId.HasValue && !_context.TrainingSessions.Any(session => session.Id == dto.TrainingSessionId.Value))
        {
            return NotFound($"Training session {dto.TrainingSessionId.Value} was not found.");
        }

        var player = new Player
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            Nationality = dto.Nationality,
            Position = dto.Position,
            JerseyNumber = dto.JerseyNumber,
            MarketValue = dto.MarketValue,
            ContractUntil = dto.ContractUntil,
            IsInjured = dto.IsInjured,
            ClubId = dto.ClubId,
            TrainingSessionId = dto.TrainingSessionId,
            IsDeleted = false
        };

        _context.Players.Add(player);
        _context.SaveChanges();

        var result = new PlayerDto
        {
            Id = player.Id,
            FirstName = player.FirstName,
            LastName = player.LastName,
            FullName = player.FullName,
            DateOfBirth = player.DateOfBirth,
            Nationality = player.Nationality,
            Position = player.Position,
            JerseyNumber = player.JerseyNumber,
            MarketValue = player.MarketValue,
            ContractUntil = player.ContractUntil,
            IsInjured = player.IsInjured,
            IsDeleted = player.IsDeleted,
            ClubId = player.ClubId,
            ClubName = _clubs.GetById(player.ClubId)?.Name,
            TrainingSessionId = player.TrainingSessionId,
            Age = player.Age
        };

        return CreatedAtAction(nameof(GetById), new { id = player.Id }, result);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] PlayerUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var player = _context.Players.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        if (player == null)
        {
            return NotFound();
        }

        if (!_clubs.GetAll().Any(club => club.Id == dto.ClubId))
        {
            return NotFound($"Club {dto.ClubId} was not found.");
        }

        if (dto.TrainingSessionId.HasValue && !_context.TrainingSessions.Any(session => session.Id == dto.TrainingSessionId.Value))
        {
            return NotFound($"Training session {dto.TrainingSessionId.Value} was not found.");
        }

        player.FirstName = dto.FirstName;
        player.LastName = dto.LastName;
        player.DateOfBirth = dto.DateOfBirth;
        player.Nationality = dto.Nationality;
        player.Position = dto.Position;
        player.JerseyNumber = dto.JerseyNumber;
        player.MarketValue = dto.MarketValue;
        player.ContractUntil = dto.ContractUntil;
        player.IsInjured = dto.IsInjured;
        player.ClubId = dto.ClubId;
        player.TrainingSessionId = dto.TrainingSessionId;

        _context.Players.Update(player);
        _context.SaveChanges();

        return Ok(GetById(player.Id) is ObjectResult objectResult && objectResult.Value is PlayerDto mapped ? mapped : new PlayerDto { Id = player.Id });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var player = _context.Players.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        if (player == null)
        {
            return NotFound();
        }

        player.IsDeleted = true;
        _context.Players.Update(player);
        _context.SaveChanges();
        return NoContent();
    }
}