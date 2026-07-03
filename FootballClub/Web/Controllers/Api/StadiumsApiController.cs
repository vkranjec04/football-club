using FootballClub.Data;
using FootballClub.Models;
using FootballClub.Web.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FootballClub.Web.Controllers.Api;

[ApiController]
[Route("api/stadiums")]
public class StadiumsApiController : ApiControllerBase
{
    private readonly ApplicationDbContext _context;

    public StadiumsApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? search = null, [FromQuery, Range(1, int.MaxValue)] int page = 1, [FromQuery, Range(1, 100)] int pageSize = 10)
    {
        var q = (search ?? string.Empty).Trim();
        var query = _context.Stadiums.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(stadium =>
                stadium.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                stadium.City.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = query.Count();
        var (normalizedPage, normalizedPageSize) = NormalizePaging(page, pageSize);
        var items = query
            .OrderBy(stadium => stadium.Name)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(stadium => new StadiumDto
            {
                Id = stadium.Id,
                Name = stadium.Name,
                City = stadium.City,
                Capacity = stadium.Capacity,
                YearBuilt = stadium.YearBuilt
            })
            .ToList();

        return Ok(CreatePagedResult(items, normalizedPage, normalizedPageSize, totalCount));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var stadium = _context.Stadiums.FirstOrDefault(item => item.Id == id);
        if (stadium == null)
        {
            return NotFound();
        }

        return Ok(new StadiumDto
        {
            Id = stadium.Id,
            Name = stadium.Name,
            City = stadium.City,
            Capacity = stadium.Capacity,
            YearBuilt = stadium.YearBuilt
        });
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost]
    public IActionResult Create([FromBody] StadiumCreateDto dto)
    {
        var stadium = new Stadium
        {
            Name = dto.Name,
            City = dto.City,
            Capacity = dto.Capacity,
            YearBuilt = dto.YearBuilt
        };

        _context.Stadiums.Add(stadium);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = stadium.Id }, new StadiumDto
        {
            Id = stadium.Id,
            Name = stadium.Name,
            City = stadium.City,
            Capacity = stadium.Capacity,
            YearBuilt = stadium.YearBuilt
        });
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] StadiumUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var stadium = _context.Stadiums.FirstOrDefault(item => item.Id == id);
        if (stadium == null)
        {
            return NotFound();
        }

        stadium.Name = dto.Name;
        stadium.City = dto.City;
        stadium.Capacity = dto.Capacity;
        stadium.YearBuilt = dto.YearBuilt;

        _context.Stadiums.Update(stadium);
        _context.SaveChanges();

        return Ok(new StadiumDto
        {
            Id = stadium.Id,
            Name = stadium.Name,
            City = stadium.City,
            Capacity = stadium.Capacity,
            YearBuilt = stadium.YearBuilt
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var stadium = _context.Stadiums.FirstOrDefault(item => item.Id == id);
        if (stadium == null)
        {
            return NotFound();
        }

        _context.Stadiums.Remove(stadium);
        _context.SaveChanges();
        return NoContent();
    }
}