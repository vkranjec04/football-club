
namespace FootballClub.Models.Mapping;

public static class PlayerMappingExtensions
{
    public static PlayerDto ToDto(this Player player)
    {
        return new PlayerDto
        {
            Id = player.Id,
            FirstName = player.FirstName,
            LastName = player.LastName,
            FullName = player.FullName,
            DateOfBirth = player.DateOfBirth,
            Nationality = player.Nationality,
            Position = player.Position,
            JerseyNumber = player.JerseyNumber,
            MarketValue = player.MarketValue,
            ContractUntil = player.ContractUntil,
            IsInjured = player.IsInjured,
            IsDeleted = player.IsDeleted,
            ClubId = player.ClubId,
            ClubName = player.Club?.Name,
            TrainingSessionId = player.TrainingSessionId,
            TrainingSessionTitle = player.TrainingSession?.Title,
            Age = player.Age
        };
    }

    public static void ApplyUpdate(this Player player, PlayerUpdateDto dto)
    {
        player.FirstName = dto.FirstName;
        player.LastName = dto.LastName;
        player.DateOfBirth = dto.DateOfBirth;
        player.Nationality = dto.Nationality;
        player.Position = dto.Position;
        player.JerseyNumber = dto.JerseyNumber;
        player.MarketValue = dto.MarketValue;
        player.ContractUntil = dto.ContractUntil;
        player.IsInjured = dto.IsInjured;
        player.ClubId = dto.ClubId;
        player.TrainingSessionId = dto.TrainingSessionId;
    }
}