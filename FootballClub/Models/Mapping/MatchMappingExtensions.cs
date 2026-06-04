using FootballClub.Web.Dto;

namespace FootballClub.Models.Mapping;

public static class MatchMappingExtensions
{
    public static MatchDto ToDto(this Match match)
    {
        return new MatchDto
        {
            Id = match.Id,
            Date = match.Date,
            HomeClubId = match.HomeClubId,
            HomeClubName = match.HomeClub?.Name,
            AwayClubId = match.AwayClubId,
            AwayClubName = match.AwayClub?.Name,
            HomeScore = match.HomeScore,
            AwayScore = match.AwayScore,
            StadiumId = match.StadiumId,
            StadiumName = match.Stadium?.Name,
            Status = match.Status,
            Attendance = match.Attendance,
            Referee = match.Referee,
            Round = match.Round
        };
    }

    public static void ApplyUpdate(this Match match, MatchUpdateDto dto)
    {
        match.Date = dto.Date;
        match.HomeClubId = dto.HomeClubId;
        match.AwayClubId = dto.AwayClubId;
        match.HomeScore = dto.HomeScore;
        match.AwayScore = dto.AwayScore;
        match.StadiumId = dto.StadiumId;
        match.Status = dto.Status;
        match.Attendance = dto.Attendance;
        match.Referee = dto.Referee;
        match.Round = dto.Round;
    }
}