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
[Route("api/training")]
public class TrainingApiController : ApiControllerBase
{
    private readonly TrainingMockRepository _training;
    private readonly ClubMockRepository _clubs;
    private readonly StaffMockRepository _staff;
    private readonly ApplicationDbContext _context;

    public TrainingApiController(TrainingMockRepository training, ClubMockRepository clubs, StaffMockRepository staff, ApplicationDbContext context)
    {
        _training = training;
        _clubs = clubs;
        _staff = staff;
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetAll([FromQuery] string? search = null, [FromQuery, Range(1, int.MaxValue)] int page = 1, [FromQuery, Range(1, 100)] int pageSize = 10)
    {
        var q = (search ?? string.Empty).Trim();
        var query = _context.TrainingSessions
            .Include(session => session.Club)
            .Include(session => session.LeadStaff)
            .Where(session => !session.IsDeleted)
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(session =>
                session.Title.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                session.FocusArea.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                session.Location.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                session.Club.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                (session.LeadStaff != null && $"{session.LeadStaff.FirstName} {session.LeadStaff.LastName}".Contains(q, StringComparison.OrdinalIgnoreCase)));
        }

        var totalCount = query.Count();
        var (normalizedPage, normalizedPageSize) = NormalizePaging(page, pageSize);
        var items = query
            .OrderBy(session => session.StartTime)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(session => new TrainingSessionDto
            {
                Id = session.Id,
                ClubId = session.ClubId,
                ClubName = session.Club?.Name,
                Title = session.Title,
                FocusArea = session.FocusArea,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                Location = session.Location,
                Intensity = session.Intensity,
                LeadStaffId = session.LeadStaffId,
                LeadStaffName = session.LeadStaff?.FullName,
                Notes = session.Notes,
                IsDeleted = session.IsDeleted
            })
            .ToList();

        return Ok(CreatePagedResult(items, normalizedPage, normalizedPageSize, totalCount));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var session = _context.TrainingSessions
            .Include(trainingSession => trainingSession.Club)
            .Include(trainingSession => trainingSession.LeadStaff)
            .FirstOrDefault(trainingSession => trainingSession.Id == id && !trainingSession.IsDeleted);

        if (session == null)
        {
            return NotFound();
        }

        return Ok(new TrainingSessionDto
        {
            Id = session.Id,
            ClubId = session.ClubId,
            ClubName = session.Club?.Name,
            Title = session.Title,
            FocusArea = session.FocusArea,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            Location = session.Location,
            Intensity = session.Intensity,
            LeadStaffId = session.LeadStaffId,
            LeadStaffName = session.LeadStaff?.FullName,
            Notes = session.Notes,
            IsDeleted = session.IsDeleted
        });
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost]
    public IActionResult Create([FromBody] TrainingSessionCreateDto dto)
    {
        if (!_clubs.GetAll().Any(club => club.Id == dto.ClubId))
        {
            return NotFound($"Club {dto.ClubId} was not found.");
        }

        if (dto.LeadStaffId.HasValue && !_context.StaffMembers.Any(staff => staff.Id == dto.LeadStaffId.Value && !staff.IsDeleted))
        {
            return NotFound($"Staff member {dto.LeadStaffId.Value} was not found.");
        }

        var session = new TrainingSession
        {
            ClubId = dto.ClubId,
            Title = dto.Title,
            FocusArea = dto.FocusArea,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Location = dto.Location,
            Intensity = dto.Intensity,
            LeadStaffId = dto.LeadStaffId,
            Notes = dto.Notes,
            IsDeleted = false
        };

        _context.TrainingSessions.Add(session);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = session.Id }, new TrainingSessionDto
        {
            Id = session.Id,
            ClubId = session.ClubId,
            ClubName = _clubs.GetById(session.ClubId)?.Name,
            Title = session.Title,
            FocusArea = session.FocusArea,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            Location = session.Location,
            Intensity = session.Intensity,
            LeadStaffId = session.LeadStaffId,
            LeadStaffName = session.LeadStaffId.HasValue ? _staff.GetById(session.LeadStaffId.Value)?.FullName : null,
            Notes = session.Notes,
            IsDeleted = session.IsDeleted
        });
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] TrainingSessionUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        var session = _context.TrainingSessions.FirstOrDefault(trainingSession => trainingSession.Id == id && !trainingSession.IsDeleted);
        if (session == null)
        {
            return NotFound();
        }

        if (!_clubs.GetAll().Any(club => club.Id == dto.ClubId))
        {
            return NotFound($"Club {dto.ClubId} was not found.");
        }

        if (dto.LeadStaffId.HasValue && !_context.StaffMembers.Any(staff => staff.Id == dto.LeadStaffId.Value && !staff.IsDeleted))
        {
            return NotFound($"Staff member {dto.LeadStaffId.Value} was not found.");
        }

        session.ClubId = dto.ClubId;
        session.Title = dto.Title;
        session.FocusArea = dto.FocusArea;
        session.StartTime = dto.StartTime;
        session.EndTime = dto.EndTime;
        session.Location = dto.Location;
        session.Intensity = dto.Intensity;
        session.LeadStaffId = dto.LeadStaffId;
        session.Notes = dto.Notes;

        _context.TrainingSessions.Update(session);
        _context.SaveChanges();

        return Ok(new TrainingSessionDto
        {
            Id = session.Id,
            ClubId = session.ClubId,
            ClubName = _clubs.GetById(session.ClubId)?.Name,
            Title = session.Title,
            FocusArea = session.FocusArea,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            Location = session.Location,
            Intensity = session.Intensity,
            LeadStaffId = session.LeadStaffId,
            LeadStaffName = session.LeadStaffId.HasValue ? _staff.GetById(session.LeadStaffId.Value)?.FullName : null,
            Notes = session.Notes,
            IsDeleted = session.IsDeleted
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var session = _context.TrainingSessions.FirstOrDefault(trainingSession => trainingSession.Id == id && !trainingSession.IsDeleted);
        if (session == null)
        {
            return NotFound();
        }

        session.IsDeleted = true;
        _context.TrainingSessions.Update(session);
        _context.SaveChanges();
        return NoContent();
    }
}