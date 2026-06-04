using FootballClub.Web.Dto;
using FootballClub.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Controllers.Api;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _userService.RegisterAsync(request.Username, request.Email, request.Password, cancellationToken);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Auth);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _userService.AuthenticateAsync(request.UsernameOrEmail, request.Password, cancellationToken);
        if (result == null)
        {
            return Unauthorized("Invalid credentials or inactive account.");
        }

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
            return Unauthorized("Unable to create or resolve a local user for this Google account.");
        }

        // The site authenticates via a JWT held in localStorage (see wwwroot/js/auth.js).
        // Since the Google flow is a full-page redirect rather than a fetch, hand the token
        // to a small MVC bridge page that stores the session client-side, then lands the user.
        return Redirect($"/Account/ExternalLogin?token={Uri.EscapeDataString(result.Token)}"
            + $"&username={Uri.EscapeDataString(result.Username)}"
            + $"&role={Uri.EscapeDataString(result.Role)}"
            + $"&returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}");
    }
}