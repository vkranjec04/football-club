using System.ComponentModel.DataAnnotations;

namespace FootballClub.Web.Features.Players;

public class PlayerStatDto
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public int MatchId { get; set; }
    public string? MatchLabel { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public int MinutesPlayed { get; set; }
    public int YellowCards { get; set; }
    public bool RedCard { get; set; }
    public double Rating { get; set; }
}

public class PlayerStatCreateDto
{
    [Required]
    public int PlayerId { get; set; }

    [Required]
    public int MatchId { get; set; }

    public int Goals { get; set; }

    public int Assists { get; set; }

    public int MinutesPlayed { get; set; }

    public int YellowCards { get; set; }

    public bool RedCard { get; set; }

    public double Rating { get; set; }
}

public class PlayerStatUpdateDto : PlayerStatCreateDto
{
    [Required]
    public int Id { get; set; }
}