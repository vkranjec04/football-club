using System.ComponentModel.DataAnnotations;

namespace FootballClub.Web.Dto;

public class StaffDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public DateTime ContractUntil { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public int ClubId { get; set; }
    public string? ClubName { get; set; }
}

public class StaffCreateDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Nationality { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public DateTime ContractUntil { get; set; }

    [Required]
    [MaxLength(100)]
    public string Role { get; set; } = string.Empty;

    [Required]
    public int ClubId { get; set; }
}

public class StaffUpdateDto : StaffCreateDto
{
    [Required]
    public int Id { get; set; }
}