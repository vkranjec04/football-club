using FootballClub.Models;
using FootballClub.Models.Enums;

namespace FootballClub.Repositories;

public class TrainingMockRepository
{
    private readonly ClubMockRepository _clubs;
    private readonly PlayerMockRepository _players;
    private readonly StaffMockRepository _staff;

    public TrainingMockRepository(
        ClubMockRepository clubs,
        PlayerMockRepository players,
        StaffMockRepository staff)
    {
        _clubs = clubs;
        _players = players;
        _staff = staff;
    }

    public List<TrainingSession> GetByClub(int clubId)
    {
        var club = _clubs.GetById(clubId);
        if (club == null)
        {
            return new List<TrainingSession>();
        }

        var leadStaff = _staff.GetCurrentStaffByClub(clubId);
        var squad = _players.GetByClub(clubId).OrderBy(p => p.JerseyNumber).ToList();
        var coreSquad = squad.Take(Math.Min(14, squad.Count)).ToList();

        var today = DateTime.Today;
        var sessions = new List<TrainingSession>
        {
            new()
            {
                Id = clubId * 100 + 1,
                Club = club,
                Title = "Morning Activation & Mobility",
                FocusArea = "Recovery and injury prevention",
                StartTime = today.AddDays(1).AddHours(9),
                EndTime = today.AddDays(1).AddHours(10).AddMinutes(30),
                Location = "Indoor Training Hall",
                Intensity = TrainingIntensity.Light,
                LeadStaff = leadStaff,
                Participants = coreSquad,
                Notes = "Dynamic stretching, prehab circuits and low-load technical drills."
            },
            // ... other sessions omitted for brevity
        };

        return sessions.OrderBy(s => s.StartTime).ToList();
    }

    public TrainingSession? GetById(int id)
    {
        var clubIds = _clubs.GetAll().Select(c => c.Id).ToList();
        return clubIds
            .SelectMany(GetByClub)
            .FirstOrDefault(s => s.Id == id);
    }
}
