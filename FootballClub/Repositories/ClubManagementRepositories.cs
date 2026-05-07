using FootballClub.Models;
using FootballClub.Models.Enums;

namespace FootballClub.Repositories;

public class TrainingMockRepository
{
    private readonly ClubMockRepository _clubs;
    private readonly PlayerMockRepository _players;
    private readonly CoachMockRepository _coaches;

    public TrainingMockRepository(
        ClubMockRepository clubs,
        PlayerMockRepository players,
        CoachMockRepository coaches)
    {
        _clubs = clubs;
        _players = players;
        _coaches = coaches;
    }

    public List<TrainingSession> GetByClub(int clubId)
    {
        var club = _clubs.GetById(clubId);
        if (club == null)
        {
            return new List<TrainingSession>();
        }

        var leadCoach = _coaches.GetCurrentCoachByClub(clubId);
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
                LeadCoach = leadCoach,
                Participants = coreSquad,
                Notes = "Dynamic stretching, prehab circuits and low-load technical drills."
            },
            new()
            {
                Id = clubId * 100 + 2,
                Club = club,
                Title = "Pressing Triggers: Tactical Block",
                FocusArea = "Mid-block coordination and pressing reactions",
                StartTime = today.AddDays(1).AddHours(16),
                EndTime = today.AddDays(1).AddHours(17).AddMinutes(45),
                Location = club.HomeStadium?.Name ?? "Main Pitch",
                Intensity = TrainingIntensity.High,
                LeadCoach = leadCoach,
                Participants = coreSquad,
                Notes = "11v11 scenario work with 6-second pressure rule after losing possession."
            },
            new()
            {
                Id = clubId * 100 + 3,
                Club = club,
                Title = "Set Piece Lab",
                FocusArea = "Attacking and defensive set plays",
                StartTime = today.AddDays(2).AddHours(11),
                EndTime = today.AddDays(2).AddHours(12).AddMinutes(15),
                Location = "Pitch B",
                Intensity = TrainingIntensity.Moderate,
                LeadCoach = leadCoach,
                Participants = coreSquad,
                Notes = "Corner variations, second-ball occupation and goalkeeper communication."
            },
            new()
            {
                Id = clubId * 100 + 4,
                Club = club,
                Title = "Power Endurance Gym Circuit",
                FocusArea = "Strength and conditioning",
                StartTime = today.AddDays(2).AddHours(15),
                EndTime = today.AddDays(2).AddHours(16).AddMinutes(20),
                Location = "Club Gym",
                Intensity = TrainingIntensity.Peak,
                LeadCoach = leadCoach,
                Participants = squad.Where(p => !p.IsInjured).Take(Math.Min(10, squad.Count)).ToList(),
                Notes = "Split into lower-body force and upper-body stability stations."
            },
            new()
            {
                Id = clubId * 100 + 5,
                Club = club,
                Title = "Matchday -1 Session",
                FocusArea = "Walkthrough and confidence rehearsal",
                StartTime = today.AddDays(3).AddHours(18),
                EndTime = today.AddDays(3).AddHours(19),
                Location = "Main Pitch",
                Intensity = TrainingIntensity.Recovery,
                LeadCoach = leadCoach,
                Participants = coreSquad,
                Notes = "Short tactical refresh with penalties and free-kick rehearsal."
            }
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

        var schedule = new List<PlayerScheduleItem>
        {
            Create(player, 1, baseDate.AddHours(8), 75, ScheduleResponsibilityType.Gym, "Explosive Strength Block", "Club Gym", "S&C Coach", "Single-leg force and posterior chain focus."),
            Create(player, 2, baseDate.AddHours(10), 45, ScheduleResponsibilityType.MediaTraining, "Media Training", "Media Room", "Communications Team", "Interview prep and sponsor storytelling."),
            Create(player, 3, baseDate.AddHours(12), 40, ScheduleResponsibilityType.NutritionConsultation, "Nutrition Review", "Performance Lab", "Nutritionist", "Hydration and matchday carb plan."),
            Create(player, 4, baseDate.AddHours(14), 60, ScheduleResponsibilityType.PhysicalTherapy, "Preventive Therapy", "Medical Center", "Head Physio", "Mobility screening and manual therapy."),
            Create(player, 5, baseDate.AddHours(17), 35, ScheduleResponsibilityType.Massage, "Recovery Massage", "Recovery Zone", "Therapy Team", "Soft tissue release after field session."),
            Create(player, 6, baseDate.AddDays(1).AddHours(11), 40, ScheduleResponsibilityType.TacticalVideo, "Video Analysis", "Tactical Theater", "Analyst", "Reviewing zone-14 entries and transition habits."),
            Create(player, 7, baseDate.AddDays(2).AddHours(9), 90, ScheduleResponsibilityType.CommunityEvent, "Community Academy Visit", "Club Academy", "Club Operations", "Q&A session with U-15 players."),
            Create(player, 8, baseDate.AddDays(3).AddHours(9), 60, ScheduleResponsibilityType.PerformanceReview, "Performance Review", "Coach Office", "Head Coach", "Set KPIs for next 3 matches."),
            Create(player, 9, baseDate.AddDays(4).AddHours(10), 30, ScheduleResponsibilityType.Rest, "Active Rest", "Recovery Lounge", "Medical Staff", "Low-load breathing and mobility."),
        };

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
                AssignedBy = session.LeadCoach is null
                    ? "Coaching Staff"
                    : $"{session.LeadCoach.FirstName} {session.LeadCoach.LastName}",
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

    private static PlayerScheduleItem Create(
        Player player,
        int idOffset,
        DateTime start,
        int durationMinutes,
        ScheduleResponsibilityType responsibility,
        string title,
        string location,
        string assignedBy,
        string notes)
    {
        return new PlayerScheduleItem
        {
            Id = player.Id * 1000 + idOffset,
            Player = player,
            StartTime = start,
            EndTime = start.AddMinutes(durationMinutes),
            ResponsibilityType = responsibility,
            Title = title,
            Location = location,
            AssignedBy = assignedBy,
            Notes = notes
        };
    }
}
