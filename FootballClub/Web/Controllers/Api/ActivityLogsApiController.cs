using System.ComponentModel.DataAnnotations;
using FootballClub.Data;
using FootballClub.Web.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballClub.Web.Controllers.Api;

/// <summary>
/// Read-only access to the audit trail. Admin-only: this is the genuine server-side gate for
/// the activity log. The MVC viewer page is only a shell that fetches from here with the
/// caller's bearer token, consistent with how the rest of the SPA-style pages load data.
/// </summary>
[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/activity-logs")]
public class ActivityLogsApiController : ApiControllerBase
{
    private readonly ApplicationDbContext _context;

    public ActivityLogsApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? user = null,
        [FromQuery] string? action = null,
        [FromQuery, Range(1, int.MaxValue)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ActivityLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(user))
        {
            var term = user.Trim();
            query = query.Where(log => log.UserName.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            var term = action.Trim();
            query = query.Where(log => log.Action.Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var (normalizedPage, normalizedPageSize) = NormalizePaging(page, pageSize);

        var items = await query
            .OrderByDescending(log => log.TimestampUtc)
            .ThenByDescending(log => log.Id)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(log => new ActivityLogDto
            {
                Id = log.Id,
                TimestampUtc = log.TimestampUtc,
                UserName = log.UserName,
                Action = log.Action,
                EntityType = log.EntityType,
                EntityId = log.EntityId,
                Description = log.Description,
                IpAddress = log.IpAddress,
                Success = log.Success
            })
            .ToListAsync(cancellationToken);

        return Ok(CreatePagedResult(items, normalizedPage, normalizedPageSize, totalCount));
    }
}
