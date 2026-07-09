
namespace FootballClub.Web.Features.Auth;

public class RegisterResult
{
    public bool Succeeded { get; init; }

    public AuthResponseDto? Auth { get; init; }

    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    public static RegisterResult Success(AuthResponseDto auth) =>
        new() { Succeeded = true, Auth = auth };

    public static RegisterResult Failure(IEnumerable<string> errors) =>
        new() { Succeeded = false, Errors = errors.ToList() };
}
