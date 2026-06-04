using System.ComponentModel.DataAnnotations;

namespace FootballClub.Web.Dto;

public class LoginRequestDto
{
    [Required]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}