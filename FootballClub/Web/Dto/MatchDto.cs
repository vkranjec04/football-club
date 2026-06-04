using System.ComponentModel.DataAnnotations;
using FootballClub.Models.Enums;

namespace FootballClub.Web.Dto;

public class MatchDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int HomeClubId { get; set; }
    public string? HomeClubName { get; set; }
    public int AwayClubId { get; set; }
    public string? AwayClubName { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public int StadiumId { get; set; }
    public string? StadiumName { get; set; }
    public MatchStatus Status { get; set; }
    public int Attendance { get; set; }
    public string Referee { get; set; } = string.Empty;
    public string Round { get; set; } = string.Empty;
}

public class MatchCreateDto
{
    [Required]
    public DateTime Date { get; set; }

    [Required]
    public int HomeClubId { get; set; }

    [Required]
    public int AwayClubId { get; set; }

    public int HomeScore { get; set; }

    public int AwayScore { get; set; }

    [Required]
    public int StadiumId { get; set; }

    [Required]
    public MatchStatus Status { get; set; }

    public int Attendance { get; set; }

    [MaxLength(100)]
    public string Referee { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Round { get; set; } = string.Empty;
}

public class MatchUpdateDto : MatchCreateDto
{
    [Required]
    public int Id { get; set; }
}