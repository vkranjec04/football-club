using FootballClub.Data;
using FootballClub.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballClub.Repositories;

public class TrainingMockRepository
{
    private readonly ApplicationDbContext _context;

    public TrainingMockRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<TrainingSession> GetByClub(int clubId) => _context.TrainingSessions
        .Include(ts => ts.Club)
        .Include(ts => ts.LeadStaff)
        .Include(ts => ts.Participants)
        .Where(ts => ts.ClubId == clubId && !ts.IsDeleted)
        .OrderBy(ts => ts.StartTime)
        .ToList();

    public TrainingSession? GetById(int id) => _context.TrainingSessions
        .Include(ts => ts.Club)
        .Include(ts => ts.LeadStaff)
        .Include(ts => ts.Participants)
        .FirstOrDefault(ts => ts.Id == id && !ts.IsDeleted);
}
