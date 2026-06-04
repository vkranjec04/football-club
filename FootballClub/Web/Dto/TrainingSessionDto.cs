using System.ComponentModel.DataAnnotations;
using FootballClub.Models.Enums;

namespace FootballClub.Web.Dto;

public class TrainingSessionDto
{
    public int Id { get; set; }
    public int ClubId { get; set; }
    public string? ClubName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FocusArea { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public TrainingIntensity Intensity { get; set; }
    public int? LeadStaffId { get; set; }
    public string? LeadStaffName { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}

public class TrainingSessionCreateDto
{
    [Required]
    public int ClubId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string FocusArea { get; set; } = string.Empty;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    [Required]
    public TrainingIntensity Intensity { get; set; }

    public int? LeadStaffId { get; set; }

    [MaxLength(1000)]
    public string Notes { get; set; } = string.Empty;
}

public class TrainingSessionUpdateDto : TrainingSessionCreateDto
{
    [Required]
    public int Id { get; set; }
}