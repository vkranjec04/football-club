using FootballClub.Data;
using FootballClub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballClub.Web.Controllers;

public class TacticsController : Controller
{
    private readonly ApplicationDbContext _context;

    public TacticsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["ActivePage"] = "Tactics";
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Tactics Board", null)
        };

        // Get Dinamo Zagreb (ID = 1) players
        var players = await _context.Players
            .Where(p => p.ClubId == 1)
            .OrderBy(p => p.Position)
            .ToListAsync();
        
        return View(players);
    }
}
