using FootballClub.Data;
using FootballClub.Models;
using FootballClub.Web.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FootballClub.Web.Controllers.Api;

[ApiController]
[Route("api/transfers")]
public class TransfersApiController : ApiControllerBase
{
    private readonly ApplicationDbContext _context;

    public TransfersApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? search = null, [FromQuery, Range(1, int.MaxValue)] int page = 1, [FromQuery, Range(1, 100)] int pageSize = 10)
    {
        var q = (search ?? string.Empty).Trim();
        var query = _context.Transfers
            .Include(transfer => transfer.Player)
            .Include(transfer => transfer.FromClub)
            .Include(transfer => transfer.ToClub)
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(transfer =>
                transfer.Player.FirstName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                transfer.Player.LastName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                $"{transfer.Player.FirstName} {transfer.Player.LastName}".Contains(q, StringComparison.OrdinalIgnoreCase) ||
                transfer.FromClub.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                transfer.ToClub.Name.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = query.Count();
        var (normalizedPage, normalizedPageSize) = NormalizePaging(page, pageSize);
        var items = query
            .OrderByDescending(transfer => transfer.TransferDate)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(transfer => new TransferDto
            {
                Id = transfer.Id,
                PlayerId = transfer.PlayerId,
                PlayerName = transfer.Player?.FullName,
                FromClubId = transfer.FromClubId,
                FromClubName = transfer.FromClub?.Name,
                ToClubId = transfer.ToClubId,
                ToClubName = transfer.ToClub?.Name,
                TransferDate = transfer.TransferDate,
                Fee = transfer.Fee
            })
            .ToList();

        return Ok(CreatePagedResult(items, normalizedPage, normalizedPageSize, totalCount));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var transfer = _context.Transfers
            .Include(item => item.Player)
            .Include(item => item.FromClub)
            .Include(item => item.ToClub)
            .FirstOrDefault(item => item.Id == id);

        if (transfer == null)
        {
            return NotFound();
        }

        return Ok(new TransferDto
        {
            Id = transfer.Id,
            PlayerId = transfer.PlayerId,
            PlayerName = transfer.Player?.FullName,
            FromClubId = transfer.FromClubId,
            FromClubName = transfer.FromClub?.Name,
            ToClubId = transfer.ToClubId,
            ToClubName = transfer.ToClub?.Name,
            TransferDate = transfer.TransferDate,
            Fee = transfer.Fee
        });
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost]
    public IActionResult Create([FromBody] TransferCreateDto dto)
    {
        if (dto.FromClubId == dto.ToClubId)
        {
            return BadRequest("From and to clubs must be different.");
        }

        var player = _context.Players.FirstOrDefault(item => item.Id == dto.PlayerId && !item.IsDeleted);
        if (player == null)
        {
            return NotFound($"Player {dto.PlayerId} was not found.");
        }

        if (!_context.Clubs.Any(item => item.Id == dto.FromClubId) || !_context.Clubs.Any(item => item.Id == dto.ToClubId))
        {
            return NotFound("One or more clubs were not found.");
        }

        var transfer = new Transfer
        {
            PlayerId = dto.PlayerId,
            FromClubId = dto.FromClubId,
            ToClubId = dto.ToClubId,
            TransferDate = dto.TransferDate,
            Fee = dto.Fee
        };

        _context.Transfers.Add(transfer);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = transfer.Id }, new TransferDto
        {
            Id = transfer.Id,
            PlayerId = transfer.PlayerId,
            PlayerName = player.FullName,
            FromClubId = transfer.FromClubId,
            FromClubName = _context.Clubs.FirstOrDefault(item => item.Id == transfer.FromClubId)?.Name,
            ToClubId = transfer.ToClubId,
            ToClubName = _context.Clubs.FirstOrDefault(item => item.Id == transfer.ToClubId)?.Name,
            TransferDate = transfer.TransferDate,
            Fee = transfer.Fee
        });
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] TransferUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        if (dto.FromClubId == dto.ToClubId)
        {
            return BadRequest("From and to clubs must be different.");
        }

        var transfer = _context.Transfers.FirstOrDefault(item => item.Id == id);
        if (transfer == null)
        {
            return NotFound();
        }

        if (!_context.Players.Any(item => item.Id == dto.PlayerId && !item.IsDeleted))
        {
            return NotFound($"Player {dto.PlayerId} was not found.");
        }

        if (!_context.Clubs.Any(item => item.Id == dto.FromClubId) || !_context.Clubs.Any(item => item.Id == dto.ToClubId))
        {
            return NotFound("One or more clubs were not found.");
        }

        transfer.PlayerId = dto.PlayerId;
        transfer.FromClubId = dto.FromClubId;
        transfer.ToClubId = dto.ToClubId;
        transfer.TransferDate = dto.TransferDate;
        transfer.Fee = dto.Fee;

        _context.Transfers.Update(transfer);
        _context.SaveChanges();

        return Ok(new TransferDto
        {
            Id = transfer.Id,
            PlayerId = transfer.PlayerId,
            PlayerName = _context.Players.FirstOrDefault(item => item.Id == transfer.PlayerId)?.FullName,
            FromClubId = transfer.FromClubId,
            FromClubName = _context.Clubs.FirstOrDefault(item => item.Id == transfer.FromClubId)?.Name,
            ToClubId = transfer.ToClubId,
            ToClubName = _context.Clubs.FirstOrDefault(item => item.Id == transfer.ToClubId)?.Name,
            TransferDate = transfer.TransferDate,
            Fee = transfer.Fee
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var transfer = _context.Transfers.FirstOrDefault(item => item.Id == id);
        if (transfer == null)
        {
            return NotFound();
        }

        _context.Transfers.Remove(transfer);
        _context.SaveChanges();
        return NoContent();
    }
}