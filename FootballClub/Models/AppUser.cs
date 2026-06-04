using Microsoft.AspNetCore.Identity;

namespace FootballClub.Models;

public class AppUser : IdentityUser<int>
{
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }
}