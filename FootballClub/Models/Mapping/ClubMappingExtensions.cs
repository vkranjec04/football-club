using FootballClub.Web.Dto;

namespace FootballClub.Models.Mapping;

public static class ClubMappingExtensions
{
    public static ClubDto ToDto(this Club club)
    {
        return new ClubDto
        {
            Id = club.Id,
            Name = club.Name,
            City = club.City,
            FoundedYear = club.FoundedYear,
            Budget = club.Budget,
            LeagueName = club.LeagueName,
            HomeStadiumId = club.HomeStadiumId,
            HomeStadiumName = club.HomeStadium?.Name
        };
    }

    public static void ApplyUpdate(this Club club, ClubUpdateDto dto)
    {
        club.Name = dto.Name;
        club.City = dto.City;
        club.FoundedYear = dto.FoundedYear;
        club.Budget = dto.Budget;
        club.LeagueName = dto.LeagueName;
        club.HomeStadiumId = dto.HomeStadiumId;
    }
}