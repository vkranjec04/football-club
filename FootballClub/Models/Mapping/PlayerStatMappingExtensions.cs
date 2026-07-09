
namespace FootballClub.Models.Mapping;

public static class PlayerStatMappingExtensions
{
    public static PlayerStatDto ToDto(this PlayerStat playerStat)
    {
        return new PlayerStatDto
        {
            Id = playerStat.Id,
            PlayerId = playerStat.PlayerId,
            PlayerName = playerStat.Player?.FullName,
            MatchId = playerStat.MatchId,
            MatchLabel = playerStat.Match is null
                ? null
                : $"{playerStat.Match.HomeClub?.Name ?? playerStat.Match.HomeClubId.ToString()} vs {playerStat.Match.AwayClub?.Name ?? playerStat.Match.AwayClubId.ToString()}",
            Goals = playerStat.Goals,
            Assists = playerStat.Assists,
            MinutesPlayed = playerStat.MinutesPlayed,
            YellowCards = playerStat.YellowCards,
            RedCard = playerStat.RedCard,
            Rating = playerStat.Rating
        };
    }

    public static void ApplyUpdate(this PlayerStat playerStat, PlayerStatUpdateDto dto)
    {
        playerStat.PlayerId = dto.PlayerId;
        playerStat.MatchId = dto.MatchId;
        playerStat.Goals = dto.Goals;
        playerStat.Assists = dto.Assists;
        playerStat.MinutesPlayed = dto.MinutesPlayed;
        playerStat.YellowCards = dto.YellowCards;
        playerStat.RedCard = dto.RedCard;
        playerStat.Rating = dto.Rating;
    }
}