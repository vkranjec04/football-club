using System.ComponentModel.DataAnnotations;

namespace FootballClub.Web.Dto;

public class StadiumDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int YearBuilt { get; set; }
}

public class StadiumCreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public int YearBuilt { get; set; }
}

public class StadiumUpdateDto : StadiumCreateDto
{
    [Required]
    public int Id { get; set; }
}