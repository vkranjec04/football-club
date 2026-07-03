using System.Security.Claims;
using FootballClub.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Controllers.Api;

/// <summary>
/// Account management for administrators: list users, promote a user to Admin, and delete an
/// account. Admin-only — this is the genuine server-side gate. The MVC /Accounts page is only a
/// shell that calls these endpoints with the caller's bearer token (like the activity log).
/// </summary>
[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/users")]
public class UsersApiController : ApiControllerBase
{
    private readonly IUserService _userService;

    public UsersApiController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userService.GetUsersAsync(cancellationToken);
        return Ok(users);
    }

    [HttpPost("{id:int}/promote")]
    public async Task<IActionResult> Promote(int id, CancellationToken cancellationToken)
    {
        var promoted = await _userService.PromoteToAdminAsync(id, cancellationToken);
        if (!promoted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        // Guard against self-lockout: an admin must not delete the account they are signed in as.
        if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var currentUserId) && currentUserId == id)
        {
            return BadRequest(new { error = "You cannot delete your own account." });
        }

        var deleted = await _userService.DeleteUserAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
