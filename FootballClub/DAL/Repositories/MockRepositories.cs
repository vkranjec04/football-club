using FootballClub.Data;
using FootballClub.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballClub.Repositories;

public class ClubMockRepository
{
    private readonly ApplicationDbContext _context;

    public ClubMockRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Club> GetAll() => _context.Clubs.ToList();
    
    public Club? GetById(int id) => _context.Clubs.FirstOrDefault(c => c.Id == id);
    
    public List<Club> GetByLeague(string leagueName) => _context.Clubs.Where(c => c.LeagueName == leagueName).OrderBy(c => c.Name).ToList();
    
    public Club? GetDinamo() => _context.Clubs.FirstOrDefault(c => c.Name.Contains("Dinamo"));

    public List<LeagueStanding> GetLeagueStandings(string leagueName)
    {
        var leagueClubs = GetByLeague(leagueName);
        var finishedMatches = _context.Matches
            .Where(m => m.Status == Models.Enums.MatchStatus.Finished)
            .ToList();

        return leagueClubs
            .Select(club =>
            {
                var clubMatches = finishedMatches
                    .Where(m => m.HomeClub.Id == club.Id || m.AwayClub.Id == club.Id)
                    .ToList();

                var wins = clubMatches.Count(m =>
                    (m.HomeClub.Id == club.Id && m.HomeScore > m.AwayScore) ||
                    (m.AwayClub.Id == club.Id && m.AwayScore > m.HomeScore));

                var draws = clubMatches.Count(m => m.HomeScore == m.AwayScore);
                var losses = clubMatches.Count - wins - draws;
                var goalsFor = clubMatches.Sum(m => m.HomeClub.Id == club.Id ? m.HomeScore : m.AwayScore);
                var goalsAgainst = clubMatches.Sum(m => m.HomeClub.Id == club.Id ? m.AwayScore : m.HomeScore);

                return new LeagueStanding
                {
                    Club = club,
                    Played = clubMatches.Count,
                    Wins = wins,
                    Draws = draws,
                    Losses = losses,
                    GoalsFor = goalsFor,
                    GoalsAgainst = goalsAgainst,
                    Points = wins * 3 + draws
                };
            })
            .OrderByDescending(x => x.Points)
            .ThenByDescending(x => x.GoalDiff)
            .ThenByDescending(x => x.GoalsFor)
            .ThenBy(x => x.Club.Name)
            .ToList();
    }
}

public class PlayerMockRepository
{
    private readonly ApplicationDbContext _context;

    public PlayerMockRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Player> GetAll() => _context.Players.Where(p => !p.IsDeleted).ToList();
    public List<Player> GetAllIncludingDeleted() => _context.Players.ToList();
    public Player? GetById(int id) => _context.Players.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
    public List<Player> GetByClub(int clubId) => _context.Players.Where(p => p.ClubId == clubId && !p.IsDeleted).ToList();
    public List<Player> GetByClubOrdered(int clubId) => _context.Players
        .Where(p => p.ClubId == clubId && !p.IsDeleted)
        .OrderBy(p => p.Position)
        .ThenBy(p => p.LastName)
        .ToList();
}

public class MatchMockRepository
{
    private readonly ApplicationDbContext _context;

    public MatchMockRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Match> GetAll() => _context.Matches.Include(m => m.HomeClub).Include(m => m.AwayClub).ToList();
    public Match? GetById(int id) => _context.Matches.Include(m => m.HomeClub).Include(m => m.AwayClub).FirstOrDefault(m => m.Id == id);
    public List<Match> GetUpcoming() => _context.Matches
        .Include(m => m.HomeClub)
        .Include(m => m.AwayClub)
        .Where(m => m.Status == Models.Enums.MatchStatus.Scheduled)
        .OrderBy(m => m.Date).ToList();
    public List<Match> GetFinished() => _context.Matches
        .Include(m => m.HomeClub)
        .Include(m => m.AwayClub)
        .Where(m => m.Status == Models.Enums.MatchStatus.Finished)
        .OrderByDescending(m => m.Date).ToList();
    
    public List<Match> GetByClub(int clubId) => _context.Matches
        .Include(m => m.HomeClub)
        .Include(m => m.AwayClub)
        .Where(m => m.HomeClub.Id == clubId || m.AwayClub.Id == clubId)
        .OrderByDescending(m => m.Date)
        .ToList();
    
    public List<Match> GetUpcomingByClub(int clubId) => _context.Matches
        .Include(m => m.HomeClub)
        .Include(m => m.AwayClub)
        .Where(m => (m.HomeClub.Id == clubId || m.AwayClub.Id == clubId) && m.Status == Models.Enums.MatchStatus.Scheduled)
        .OrderBy(m => m.Date)
        .ToList();
    
    public List<Match> GetFinishedByClub(int clubId) => _context.Matches
        .Include(m => m.HomeClub)
        .Include(m => m.AwayClub)
        .Where(m => (m.HomeClub.Id == clubId || m.AwayClub.Id == clubId) && m.Status == Models.Enums.MatchStatus.Finished)
        .OrderByDescending(m => m.Date)
        .ToList();
}

public class StaffMockRepository
{
    private readonly ApplicationDbContext _context;

    public StaffMockRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Staff> GetAll() => _context.StaffMembers.ToList();
    public Staff? GetById(int id) => _context.StaffMembers.FirstOrDefault(c => c.Id == id);
    
    public Staff? GetCurrentStaffByClub(int clubId)
    {
        var club = _context.Clubs.Include(c => c.StaffMembers).FirstOrDefault(c => c.Id == clubId);
        if (club == null || club.StaffMembers == null || !club.StaffMembers.Any()) return null;
        var head = club.StaffMembers.FirstOrDefault(c => c.Role != null && c.Role.ToLower().Contains("head"));
        return head ?? club.StaffMembers.OrderByDescending(c => c.ContractUntil).FirstOrDefault();
    }
    
    public List<Staff> GetStaffForClub(string clubName)
    {
        // Resolve club first, then query by ClubId to avoid pitfalls with navigation properties
        var club = _context.Clubs.FirstOrDefault(c => c.Name.Contains(clubName));
        if (club == null) return new List<Staff>();
        return _context.StaffMembers.Where(c => c.ClubId == club.Id).OrderByDescending(c => c.ContractUntil).ToList();
    }
}
