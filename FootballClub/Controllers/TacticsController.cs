using FootballClub.Models;
using FootballClub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Controllers;

public class TacticsController : Controller
{
    private readonly PlayerMockRepository _playerRepo;

    public TacticsController(PlayerMockRepository playerRepo)
    {
        _playerRepo = playerRepo;
    }

    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Tactics";
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Tactics Board", null)
        };

        var players = _playerRepo.GetAll().Where(p => p.Club?.Id == 1).ToList();
        return View(players);
    }
}
