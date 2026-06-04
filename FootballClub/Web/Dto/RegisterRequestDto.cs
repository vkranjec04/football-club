using System.ComponentModel.DataAnnotations;

namespace FootballClub.Web.Dto;

public class RegisterRequestDto
{
    [Required]
    [StringLength(256, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
