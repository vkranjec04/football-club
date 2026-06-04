using FootballClub.Models;
using FootballClub.Models.Enums;

namespace FootballClub.Repositories;

public class PlayerScheduleMockRepository
{
    private readonly PlayerMockRepository _players;
    private readonly TrainingMockRepository _training;

    public PlayerScheduleMockRepository(PlayerMockRepository players, TrainingMockRepository training)
    {
        _players = players;
        _training = training;
    }

    public List<PlayerScheduleItem> GetWeeklyScheduleForPlayer(int playerId)
    {
        var player = _players.GetById(playerId);
        if (player?.Club == null)
        {
            return new List<PlayerScheduleItem>();
        }

        var clubSessions = _training.GetByClub(player.Club.Id);
        var baseDate = DateTime.Today;
        var seed = player.Id * 37;

        var schedule = new List<PlayerScheduleItem>();
        // simplified for brevity
        foreach (var session in clubSessions.Where(s => s.Participants.Any(p => p.Id == playerId)))
        {
            schedule.Add(new PlayerScheduleItem
            {
                Id = seed * 100 + session.Id,
                Player = player,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                ResponsibilityType = ScheduleResponsibilityType.RegularTraining,
                Title = session.Title,
                Location = session.Location,
                AssignedBy = session.LeadStaff is null
                    ? "Coaching Staff"
                    : $"{session.LeadStaff.FirstName} {session.LeadStaff.LastName}",
                Notes = session.FocusArea
            });
        }

        return schedule
            .OrderBy(item => item.StartTime)
            .ThenBy(item => item.ResponsibilityType)
            .ToList();
    }

    public Dictionary<Player, List<PlayerScheduleItem>> GetWeeklyScheduleByClub(int clubId)
    {
        var players = _players.GetByClub(clubId);
        return players
            .OrderBy(p => p.LastName)
            .ToDictionary(player => player, player => GetWeeklyScheduleForPlayer(player.Id));
    }
}
