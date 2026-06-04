using System.ComponentModel.DataAnnotations;

namespace FootballClub.Web.Dto;

public class ClubDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int FoundedYear { get; set; }
    public decimal Budget { get; set; }
    public string LeagueName { get; set; } = string.Empty;
    public int HomeStadiumId { get; set; }
    public string? HomeStadiumName { get; set; }
}

public class ClubCreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    public int FoundedYear { get; set; }

    [Required]
    public decimal Budget { get; set; }

    [Required]
    [MaxLength(50)]
    public string LeagueName { get; set; } = string.Empty;

    [Required]
    public int HomeStadiumId { get; set; }
}

public class ClubUpdateDto : ClubCreateDto
{
    [Required]
    public int Id { get; set; }
}