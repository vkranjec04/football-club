
namespace FootballClub.Models.Mapping;

public static class TrainingSessionMappingExtensions
{
    public static TrainingSessionDto ToDto(this TrainingSession trainingSession)
    {
        return new TrainingSessionDto
        {
            Id = trainingSession.Id,
            ClubId = trainingSession.ClubId,
            ClubName = trainingSession.Club?.Name,
            Title = trainingSession.Title,
            FocusArea = trainingSession.FocusArea,
            StartTime = trainingSession.StartTime,
            EndTime = trainingSession.EndTime,
            Location = trainingSession.Location,
            Intensity = trainingSession.Intensity,
            LeadStaffId = trainingSession.LeadStaffId,
            LeadStaffName = trainingSession.LeadStaff?.FullName,
            Notes = trainingSession.Notes,
            IsDeleted = trainingSession.IsDeleted
        };
    }

    public static void ApplyUpdate(this TrainingSession trainingSession, TrainingSessionUpdateDto dto)
    {
        trainingSession.ClubId = dto.ClubId;
        trainingSession.Title = dto.Title;
        trainingSession.FocusArea = dto.FocusArea;
        trainingSession.StartTime = dto.StartTime;
        trainingSession.EndTime = dto.EndTime;
        trainingSession.Location = dto.Location;
        trainingSession.Intensity = dto.Intensity;
        trainingSession.LeadStaffId = dto.LeadStaffId;
        trainingSession.Notes = dto.Notes;
    }
}