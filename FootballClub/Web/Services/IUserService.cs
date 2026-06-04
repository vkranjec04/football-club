using FootballClub.Web.Dto;

namespace FootballClub.Web.Services;

public interface IUserService
{
    Task<RegisterResult> RegisterAsync(string username, string email, string password, CancellationToken cancellationToken = default);

    Task<AuthResponseDto?> AuthenticateAsync(string usernameOrEmail, string password, CancellationToken cancellationToken = default);

    Task<AuthResponseDto?> AuthenticateExternalAsync(System.Security.Claims.ClaimsPrincipal externalPrincipal, CancellationToken cancellationToken = default);
}