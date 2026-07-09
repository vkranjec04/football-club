
namespace FootballClub.Models.Mapping;

public static class StadiumMappingExtensions
{
    public static StadiumDto ToDto(this Stadium stadium)
    {
        return new StadiumDto
        {
            Id = stadium.Id,
            Name = stadium.Name,
            City = stadium.City,
            Capacity = stadium.Capacity,
            YearBuilt = stadium.YearBuilt
        };
    }

    public static void ApplyUpdate(this Stadium stadium, StadiumUpdateDto dto)
    {
        stadium.Name = dto.Name;
        stadium.City = dto.City;
        stadium.Capacity = dto.Capacity;
        stadium.YearBuilt = dto.YearBuilt;
    }
}