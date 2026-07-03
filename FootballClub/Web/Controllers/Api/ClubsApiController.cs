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
[Route("api/clubs")]
public class ClubsApiController : ApiControllerBase
{
    private readonly ClubMockRepository _clubs;
    private readonly ApplicationDbContext _context;

    public ClubsApiController(ClubMockRepository clubs, ApplicationDbContext context)
    {
        _clubs = clubs;
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? search = null, [FromQuery, Range(1, int.MaxValue)] int page = 1, [FromQuery, Range(1, 100)] int pageSize = 10)
    {
        var q = (search ?? string.Empty).Trim();
        var query = _clubs.GetAll().AsEnumerable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(club =>
                club.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                club.City.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                club.LeagueName.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = query.Count();
        var (normalizedPage, normalizedPageSize) = NormalizePaging(page, pageSize);
        var items = query
            .OrderBy(club => club.Name)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(club => new ClubDto
            {
                Id = club.Id,
                Name = club.Name,
                City = club.City,
                FoundedYear = club.FoundedYear,
                Budget = club.Budget,
                LeagueName = club.LeagueName,
                HomeStadiumId = club.HomeStadiumId,
                HomeStadiumName = club.HomeStadium?.Name
            })
            .ToList();

        return Ok(CreatePagedResult(items, normalizedPage, normalizedPageSize, totalCount));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var club = _context.Clubs.Include(c => c.HomeStadium).FirstOrDefault(c => c.Id == id);
        if (club == null)
        {
            return NotFound();
        }

        return Ok(new ClubDto
        {
            Id = club.Id,
            Name = club.Name,
            City = club.City,
            FoundedYear = club.FoundedYear,
            Budget = club.Budget,
            LeagueName = club.LeagueName,
            HomeStadiumId = club.HomeStadiumId,
            HomeStadiumName = club.HomeStadium?.Name
        });
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost]
    public IActionResult Create([FromBody] ClubCreateDto dto)
    {
        if (!_context.Stadiums.Any(stadium => stadium.Id == dto.HomeStadiumId))
        {
            return NotFound($"Stadium {dto.HomeStadiumId} was not found.");
        }

        var club = new Club
        {
            Name = dto.Name,
            City = dto.City,
            FoundedYear = dto.FoundedYear,
            Budget = dto.Budget,
            LeagueName = dto.LeagueName,
            HomeStadiumId = dto.HomeStadiumId
        };

        _context.Clubs.Add(club);
        _context.SaveChanges();

        var result = new ClubDto
        {
            Id = club.Id,
            Name = club.Name,
            City = club.City,
            FoundedYear = club.FoundedYear,
            Budget = club.Budget,
            LeagueName = club.LeagueName,
            HomeStadiumId = club.HomeStadiumId,
            HomeStadiumName = _context.Stadiums.FirstOrDefault(stadium => stadium.Id == club.HomeStadiumId)?.Name
        };

        return CreatedAtAction(nameof(GetById), new { id = club.Id }, result);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] ClubUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var club = _context.Clubs.Include(c => c.HomeStadium).FirstOrDefault(c => c.Id == id);
        if (club == null)
        {
            return NotFound();
        }

        if (!_context.Stadiums.Any(stadium => stadium.Id == dto.HomeStadiumId))
        {
            return NotFound($"Stadium {dto.HomeStadiumId} was not found.");
        }

        club.Name = dto.Name;
        club.City = dto.City;
        club.FoundedYear = dto.FoundedYear;
        club.Budget = dto.Budget;
        club.LeagueName = dto.LeagueName;
        club.HomeStadiumId = dto.HomeStadiumId;

        _context.Clubs.Update(club);
        _context.SaveChanges();

        return Ok(new ClubDto
        {
            Id = club.Id,
            Name = club.Name,
            City = club.City,
            FoundedYear = club.FoundedYear,
            Budget = club.Budget,
            LeagueName = club.LeagueName,
            HomeStadiumId = club.HomeStadiumId,
            HomeStadiumName = _context.Stadiums.FirstOrDefault(stadium => stadium.Id == club.HomeStadiumId)?.Name
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var club = _context.Clubs.FirstOrDefault(c => c.Id == id);
        if (club == null)
        {
            return NotFound();
        }

        _context.Clubs.Remove(club);
        _context.SaveChanges();
        return NoContent();
    }
}