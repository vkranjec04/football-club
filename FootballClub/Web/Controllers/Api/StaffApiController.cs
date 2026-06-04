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
[Route("api/staff")]
public class StaffApiController : ApiControllerBase
{
    private readonly StaffMockRepository _staff;
    private readonly ClubMockRepository _clubs;
    private readonly ApplicationDbContext _context;

    public StaffApiController(StaffMockRepository staff, ClubMockRepository clubs, ApplicationDbContext context)
    {
        _staff = staff;
        _clubs = clubs;
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetAll([FromQuery] string? search = null, [FromQuery, Range(1, int.MaxValue)] int page = 1, [FromQuery, Range(1, 100)] int pageSize = 10)
    {
        var q = (search ?? string.Empty).Trim();
        var query = _staff.GetAll().Where(staff => !staff.IsDeleted).AsEnumerable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(staff =>
                staff.FirstName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                staff.LastName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                $"{staff.FirstName} {staff.LastName}".Contains(q, StringComparison.OrdinalIgnoreCase) ||
                staff.Role.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = query.Count();
        var (normalizedPage, normalizedPageSize) = NormalizePaging(page, pageSize);
        var items = query
            .OrderBy(staff => staff.LastName)
            .ThenBy(staff => staff.FirstName)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(staff => new StaffDto
            {
                Id = staff.Id,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                FullName = staff.FullName,
                Nationality = staff.Nationality,
                DateOfBirth = staff.DateOfBirth,
                ContractUntil = staff.ContractUntil,
                Role = staff.Role,
                IsDeleted = staff.IsDeleted,
                ClubId = staff.ClubId,
                ClubName = staff.Club?.Name
            })
            .ToList();

        return Ok(CreatePagedResult(items, normalizedPage, normalizedPageSize, totalCount));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var staff = _context.StaffMembers.Include(staffMember => staffMember.Club).FirstOrDefault(staffMember => staffMember.Id == id && !staffMember.IsDeleted);
        if (staff == null)
        {
            return NotFound();
        }

        return Ok(new StaffDto
        {
            Id = staff.Id,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            FullName = staff.FullName,
            Nationality = staff.Nationality,
            DateOfBirth = staff.DateOfBirth,
            ContractUntil = staff.ContractUntil,
            Role = staff.Role,
            IsDeleted = staff.IsDeleted,
            ClubId = staff.ClubId,
            ClubName = staff.Club?.Name
        });
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost]
    public IActionResult Create([FromBody] StaffCreateDto dto)
    {
        if (!_clubs.GetAll().Any(club => club.Id == dto.ClubId))
        {
            return NotFound($"Club {dto.ClubId} was not found.");
        }

        var staff = new Staff
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Nationality = dto.Nationality,
            DateOfBirth = dto.DateOfBirth,
            ContractUntil = dto.ContractUntil,
            Role = dto.Role,
            ClubId = dto.ClubId,
            IsDeleted = false
        };

        _context.StaffMembers.Add(staff);
        _context.SaveChanges();

        var result = new StaffDto
        {
            Id = staff.Id,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            FullName = staff.FullName,
            Nationality = staff.Nationality,
            DateOfBirth = staff.DateOfBirth,
            ContractUntil = staff.ContractUntil,
            Role = staff.Role,
            IsDeleted = staff.IsDeleted,
            ClubId = staff.ClubId,
            ClubName = _clubs.GetById(staff.ClubId)?.Name
        };

        return CreatedAtAction(nameof(GetById), new { id = staff.Id }, result);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] StaffUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var staff = _context.StaffMembers.FirstOrDefault(staffMember => staffMember.Id == id && !staffMember.IsDeleted);
        if (staff == null)
        {
            return NotFound();
        }

        if (!_clubs.GetAll().Any(club => club.Id == dto.ClubId))
        {
            return NotFound($"Club {dto.ClubId} was not found.");
        }

        staff.FirstName = dto.FirstName;
        staff.LastName = dto.LastName;
        staff.Nationality = dto.Nationality;
        staff.DateOfBirth = dto.DateOfBirth;
        staff.ContractUntil = dto.ContractUntil;
        staff.Role = dto.Role;
        staff.ClubId = dto.ClubId;

        _context.StaffMembers.Update(staff);
        _context.SaveChanges();

        return Ok(new StaffDto
        {
            Id = staff.Id,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            FullName = staff.FullName,
            Nationality = staff.Nationality,
            DateOfBirth = staff.DateOfBirth,
            ContractUntil = staff.ContractUntil,
            Role = staff.Role,
            IsDeleted = staff.IsDeleted,
            ClubId = staff.ClubId,
            ClubName = _clubs.GetById(staff.ClubId)?.Name
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var staff = _context.StaffMembers.FirstOrDefault(staffMember => staffMember.Id == id && !staffMember.IsDeleted);
        if (staff == null)
        {
            return NotFound();
        }

        staff.IsDeleted = true;
        _context.StaffMembers.Update(staff);
        _context.SaveChanges();
        return NoContent();
    }
}