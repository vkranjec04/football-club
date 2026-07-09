
namespace FootballClub.Models.Mapping;

public static class StaffMappingExtensions
{
    public static StaffDto ToDto(this Staff staff)
    {
        return new StaffDto
        {
            Id = staff.Id,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            FullName = staff.FullName,
            Nationality = staff.Nationality,
            DateOfBirth = staff.DateOfBirth,
            ContractUntil = staff.ContractUntil,
            Role = staff.Role,
            IsDeleted = staff.IsDeleted,
            ClubId = staff.ClubId,
            ClubName = staff.Club?.Name
        };
    }

    public static void ApplyUpdate(this Staff staff, StaffUpdateDto dto)
    {
        staff.FirstName = dto.FirstName;
        staff.LastName = dto.LastName;
        staff.Nationality = dto.Nationality;
        staff.DateOfBirth = dto.DateOfBirth;
        staff.ContractUntil = dto.ContractUntil;
        staff.Role = dto.Role;
        staff.ClubId = dto.ClubId;
    }
}