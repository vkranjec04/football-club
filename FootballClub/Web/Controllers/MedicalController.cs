using FootballClub.Models;
using FootballClub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Controllers;

public class MedicalController : Controller
{
    private readonly PlayerMockRepository _playerRepo;

    public MedicalController(PlayerMockRepository playerRepo)
    {
        _playerRepo = playerRepo;
    }

    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Medical";
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Medical Center", null)
        };

        var injuredPlayers = _playerRepo.GetAll().Where(p => p.IsInjured && p.Club?.Id == 1).ToList();
        return View(injuredPlayers);
    }
}
