using System.ComponentModel.DataAnnotations;
using FootballClub.Models.Enums;

namespace FootballClub.Web.Features.Players;

public class PlayerDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public PlayerPosition Position { get; set; }
    public int JerseyNumber { get; set; }
    public decimal MarketValue { get; set; }
    public DateTime ContractUntil { get; set; }
    public bool IsInjured { get; set; }
    public bool IsDeleted { get; set; }
    public int ClubId { get; set; }
    public string? ClubName { get; set; }
    public int? TrainingSessionId { get; set; }
    public string? TrainingSessionTitle { get; set; }
    public int Age { get; set; }
}

public class PlayerCreateDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [MaxLength(100)]
    public string Nationality { get; set; } = string.Empty;

    [Required]
    public PlayerPosition Position { get; set; }

    public int JerseyNumber { get; set; }

    [Required]
    public decimal MarketValue { get; set; }

    [Required]
    public DateTime ContractUntil { get; set; }

    public bool IsInjured { get; set; }

    [Required]
    public int ClubId { get; set; }

    public int? TrainingSessionId { get; set; }
}

public class PlayerUpdateDto : PlayerCreateDto
{
    [Required]
    public int Id { get; set; }
}