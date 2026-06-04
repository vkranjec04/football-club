using FootballClub.Data;
using FootballClub.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballClub.Repositories;

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
