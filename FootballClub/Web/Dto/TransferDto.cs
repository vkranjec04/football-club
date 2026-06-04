using System.ComponentModel.DataAnnotations;

namespace FootballClub.Web.Dto;

public class TransferDto
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public int FromClubId { get; set; }
    public string? FromClubName { get; set; }
    public int ToClubId { get; set; }
    public string? ToClubName { get; set; }
    public DateTime TransferDate { get; set; }
    public decimal Fee { get; set; }
}

public class TransferCreateDto
{
    [Required]
    public int PlayerId { get; set; }

    [Required]
    public int FromClubId { get; set; }

    [Required]
    public int ToClubId { get; set; }

    [Required]
    public DateTime TransferDate { get; set; }

    [Required]
    public decimal Fee { get; set; }
}

public class TransferUpdateDto : TransferCreateDto
{
    [Required]
    public int Id { get; set; }
}