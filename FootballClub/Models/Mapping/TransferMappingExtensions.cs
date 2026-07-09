
namespace FootballClub.Models.Mapping;

public static class TransferMappingExtensions
{
    public static TransferDto ToDto(this Transfer transfer)
    {
        return new TransferDto
        {
            Id = transfer.Id,
            PlayerId = transfer.PlayerId,
            PlayerName = transfer.Player?.FullName,
            FromClubId = transfer.FromClubId,
            FromClubName = transfer.FromClub?.Name,
            ToClubId = transfer.ToClubId,
            ToClubName = transfer.ToClub?.Name,
            TransferDate = transfer.TransferDate,
            Fee = transfer.Fee
        };
    }

    public static void ApplyUpdate(this Transfer transfer, TransferUpdateDto dto)
    {
        transfer.PlayerId = dto.PlayerId;
        transfer.FromClubId = dto.FromClubId;
        transfer.ToClubId = dto.ToClubId;
        transfer.TransferDate = dto.TransferDate;
        transfer.Fee = dto.Fee;
    }
}