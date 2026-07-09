using FootballClub.Data;
using FootballClub.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballClub.Repositories;

public class PlayerMockRepository
{
    private readonly ApplicationDbContext _context;

    public PlayerMockRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Player> GetAll() => _context.Players.Where(p => !p.IsDeleted).ToList();
    public List<Player> GetAllIncludingDeleted() => _context.Players.ToList();
    public Player? GetById(int id) => _context.Players.Include(p => p.Club).FirstOrDefault(p => p.Id == id && !p.IsDeleted);
    public List<Player> GetByClub(int clubId) => _context.Players.Where(p => p.ClubId == clubId && !p.IsDeleted).ToList();
    public List<Player> GetByClubOrdered(int clubId) => _context.Players
        .Where(p => p.ClubId == clubId && !p.IsDeleted)
        .OrderBy(p => p.Position)
        .ThenBy(p => p.LastName)
        .ToList();
}
