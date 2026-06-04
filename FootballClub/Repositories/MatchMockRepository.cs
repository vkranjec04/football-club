using FootballClub.Data;
using FootballClub.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballClub.Repositories;

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
