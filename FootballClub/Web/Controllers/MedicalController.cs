using FootballClub.Data;
using FootballClub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballClub.Web.Controllers;

public class MedicalController : Controller
{
    private readonly ApplicationDbContext _context;

    public MedicalController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["ActivePage"] = "Medical";
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Medical Center", null)
        };

        // Get Dinamo Zagreb (ID = 1) injured players
        var injuredPlayers = await _context.Players
            .Where(p => p.IsInjured && p.ClubId == 1)
            .ToListAsync();
        
        return View(injuredPlayers);
    }
}
