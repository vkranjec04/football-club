using FootballClub.Data;
using FootballClub.Models;
using FootballClub.Repositories;
using FootballClub.Web.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FootballClub.Web.Controllers.Api;

[ApiController]
[Route("api/matches")]
public class MatchesApiController : ApiControllerBase
{
    private readonly MatchMockRepository _matches;
    private readonly ClubMockRepository _clubs;
    private readonly ApplicationDbContext _context;

    public MatchesApiController(MatchMockRepository matches, ClubMockRepository clubs, ApplicationDbContext context)
    {
        _matches = matches;
        _clubs = clubs;
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? search = null, [FromQuery, Range(1, int.MaxValue)] int page = 1, [FromQuery, Range(1, 100)] int pageSize = 10)
    {
        var q = (search ?? string.Empty).Trim();
        var query = _matches.GetAll().AsEnumerable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(match =>
                match.HomeClub?.Name.Contains(q, StringComparison.OrdinalIgnoreCase) == true ||
                match.AwayClub?.Name.Contains(q, StringComparison.OrdinalIgnoreCase) == true ||
                match.Referee.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                match.Round.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                match.Status.ToString().Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = query.Count();
        var (normalizedPage, normalizedPageSize) = NormalizePaging(page, pageSize);
        var items = query
            .OrderByDescending(match => match.Date)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(match => new MatchDto
            {
                Id = match.Id,
                Date = match.Date,
                HomeClubId = match.HomeClubId,
                HomeClubName = match.HomeClub?.Name,
                AwayClubId = match.AwayClubId,
                AwayClubName = match.AwayClub?.Name,
                HomeScore = match.HomeScore,
                AwayScore = match.AwayScore,
                StadiumId = match.StadiumId,
                StadiumName = match.Stadium?.Name,
                Status = match.Status,
                Attendance = match.Attendance,
                Referee = match.Referee,
                Round = match.Round
            })
            .ToList();

        return Ok(CreatePagedResult(items, normalizedPage, normalizedPageSize, totalCount));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var match = _context.Matches
            .Include(matchItem => matchItem.HomeClub)
            .Include(matchItem => matchItem.AwayClub)
            .Include(matchItem => matchItem.Stadium)
            .FirstOrDefault(matchItem => matchItem.Id == id);

        if (match == null)
        {
            return NotFound();
        }

        return Ok(new MatchDto
        {
            Id = match.Id,
            Date = match.Date,
            HomeClubId = match.HomeClubId,
            HomeClubName = match.HomeClub?.Name,
            AwayClubId = match.AwayClubId,
            AwayClubName = match.AwayClub?.Name,
            HomeScore = match.HomeScore,
            AwayScore = match.AwayScore,
            StadiumId = match.StadiumId,
            StadiumName = match.Stadium?.Name,
            Status = match.Status,
            Attendance = match.Attendance,
            Referee = match.Referee,
            Round = match.Round
        });
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost]
    public IActionResult Create([FromBody] MatchCreateDto dto)
    {
        if (dto.HomeClubId == dto.AwayClubId)
        {
            return BadRequest("Home and away clubs must be different.");
        }

        if (!_clubs.GetAll().Any(club => club.Id == dto.HomeClubId) || !_clubs.GetAll().Any(club => club.Id == dto.AwayClubId))
        {
            return NotFound("One or more clubs were not found.");
        }

        if (!_context.Stadiums.Any(stadium => stadium.Id == dto.StadiumId))
        {
            return NotFound($"Stadium {dto.StadiumId} was not found.");
        }

        var match = new Match
        {
            Date = dto.Date,
            HomeClubId = dto.HomeClubId,
            AwayClubId = dto.AwayClubId,
            HomeScore = dto.HomeScore,
            AwayScore = dto.AwayScore,
            StadiumId = dto.StadiumId,
            Status = dto.Status,
            Attendance = dto.Attendance,
            Referee = dto.Referee,
            Round = dto.Round
        };

        _context.Matches.Add(match);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = match.Id }, new MatchDto
        {
            Id = match.Id,
            Date = match.Date,
            HomeClubId = match.HomeClubId,
            AwayClubId = match.AwayClubId,
            HomeScore = match.HomeScore,
            AwayScore = match.AwayScore,
            StadiumId = match.StadiumId,
            Status = match.Status,
            Attendance = match.Attendance,
            Referee = match.Referee,
            Round = match.Round,
            HomeClubName = _clubs.GetById(match.HomeClubId)?.Name,
            AwayClubName = _clubs.GetById(match.AwayClubId)?.Name,
            StadiumName = _context.Stadiums.FirstOrDefault(stadium => stadium.Id == match.StadiumId)?.Name
        });
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] MatchUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        if (dto.HomeClubId == dto.AwayClubId)
        {
            return BadRequest("Home and away clubs must be different.");
        }

        var match = _context.Matches.FirstOrDefault(matchItem => matchItem.Id == id);
        if (match == null)
        {
            return NotFound();
        }

        if (!_clubs.GetAll().Any(club => club.Id == dto.HomeClubId) || !_clubs.GetAll().Any(club => club.Id == dto.AwayClubId))
        {
            return NotFound("One or more clubs were not found.");
        }

        if (!_context.Stadiums.Any(stadium => stadium.Id == dto.StadiumId))
        {
            return NotFound($"Stadium {dto.StadiumId} was not found.");
        }

        match.Date = dto.Date;
        match.HomeClubId = dto.HomeClubId;
        match.AwayClubId = dto.AwayClubId;
        match.HomeScore = dto.HomeScore;
        match.AwayScore = dto.AwayScore;
        match.StadiumId = dto.StadiumId;
        match.Status = dto.Status;
        match.Attendance = dto.Attendance;
        match.Referee = dto.Referee;
        match.Round = dto.Round;

        _context.Matches.Update(match);
        _context.SaveChanges();

        return Ok(new MatchDto
        {
            Id = match.Id,
            Date = match.Date,
            HomeClubId = match.HomeClubId,
            AwayClubId = match.AwayClubId,
            HomeScore = match.HomeScore,
            AwayScore = match.AwayScore,
            StadiumId = match.StadiumId,
            Status = match.Status,
            Attendance = match.Attendance,
            Referee = match.Referee,
            Round = match.Round,
            HomeClubName = _clubs.GetById(match.HomeClubId)?.Name,
            AwayClubName = _clubs.GetById(match.AwayClubId)?.Name,
            StadiumName = _context.Stadiums.FirstOrDefault(stadium => stadium.Id == match.StadiumId)?.Name
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var match = _context.Matches.FirstOrDefault(matchItem => matchItem.Id == id);
        if (match == null)
        {
            return NotFound();
        }

        _context.Matches.Remove(match);
        _context.SaveChanges();
        return NoContent();
    }
}