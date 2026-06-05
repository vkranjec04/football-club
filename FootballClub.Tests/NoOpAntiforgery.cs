using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace FootballClub.Tests;

/// <summary>
/// Test double that bypasses antiforgery validation so tests can POST to the MVC
/// extraction endpoints (which use [ValidateAntiForgeryToken]) without a token.
/// </summary>
internal sealed class NoOpAntiforgery : IAntiforgery
{
    public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext) => Tokens();

    public AntiforgeryTokenSet GetTokens(HttpContext httpContext) => Tokens();

    public Task<bool> IsRequestValidAsync(HttpContext httpContext) => Task.FromResult(true);

    public Task ValidateRequestAsync(HttpContext httpContext) => Task.CompletedTask;

    public void SetCookieTokenAndHeader(HttpContext httpContext)
    {
    }

    private static AntiforgeryTokenSet Tokens()
        => new("request-token", "cookie-token", "__RequestVerificationToken", "X-CSRF-TOKEN");
}
