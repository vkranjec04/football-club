using System.ComponentModel.DataAnnotations;

namespace FootballClub.Web.Features.Auth;

public class LoginRequestDto
{
    [Required]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}