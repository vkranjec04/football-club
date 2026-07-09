using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Features.Auth;

// Auth events are audited explicitly below (with the attempted username and outcome), so the
// generic request filter is told to skip this controller to avoid blander duplicate entries.
[SkipActivityLog]
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IActivityLogger _activityLogger;

    public AuthController(IUserService userService, IActivityLogger activityLogger)
    {
        _userService = userService;
        _activityLogger = activityLogger;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _userService.RegisterAsync(request.Username, request.Email, request.Password, cancellationToken);
        if (!result.Succeeded)
        {
            await _activityLogger.LogAsync("Register", "Auth", request.Username, "Registration failed.", success: false, cancellationToken);
            return BadRequest(new { errors = result.Errors });
        }

        await _activityLogger.LogAsync("Register", "Auth", request.Username, "New account registered.", success: true, cancellationToken);
        return Ok(result.Auth);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _userService.AuthenticateAsync(request.UsernameOrEmail, request.Password, cancellationToken);
        if (result == null)
        {
            await _activityLogger.LogAsync("LoginFailed", "Auth", request.UsernameOrEmail, "Invalid credentials or inactive account.", success: false, cancellationToken);
            return Unauthorized("Invalid credentials or inactive account.");
        }

        await _activityLogger.LogAsync("Login", "Auth", result.Username, "Login successful.", success: true, cancellationToken);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("google-login")]
    public IActionResult GoogleLogin([FromQuery] string? returnUrl = null)
    {
        var redirectUri = Url.Action(nameof(GoogleCallback), values: new { returnUrl }) ?? "/";
        var properties = new AuthenticationProperties { RedirectUri = redirectUri };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [AllowAnonymous]
    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        var externalAuth = await HttpContext.AuthenticateAsync("External");
        if (!externalAuth.Succeeded || externalAuth.Principal == null)
        {
            return Unauthorized("Google authentication failed.");
        }

        var result = await _userService.AuthenticateExternalAsync(externalAuth.Principal, cancellationToken);
        await HttpContext.SignOutAsync("External");

        if (result == null)
        {
            await _activityLogger.LogAsync("LoginFailed", "Auth", null, "Google login could not resolve a local user.", success: false, cancellationToken);
            return Unauthorized("Unable to create or resolve a local user for this Google account.");
        }

        await _activityLogger.LogAsync("Login", "Auth", result.Username, "Login successful (Google).", success: true, cancellationToken);

        // The site authenticates via a JWT held in localStorage (see wwwroot/js/auth.js).
        // Since the Google flow is a full-page redirect rather than a fetch, hand the token
        // to a small MVC bridge page that stores the session client-side, then lands the user.
        return Redirect($"/Account/ExternalLogin?token={Uri.EscapeDataString(result.Token)}"
            + $"&username={Uri.EscapeDataString(result.Username)}"
            + $"&role={Uri.EscapeDataString(result.Role)}"
            + $"&returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}");
    }
}