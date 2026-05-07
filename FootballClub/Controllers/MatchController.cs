using FootballClub.Models;
using FootballClub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Controllers;

public class MatchController : Controller
{
    private readonly MatchMockRepository _matchRepo;
    private readonly ClubMockRepository _clubRepo;

    public MatchController(MatchMockRepository matchRepo, ClubMockRepository clubRepo)
    {
        _matchRepo = matchRepo;
        _clubRepo = clubRepo;
    }

    [Route("fixtures-and-results")]
    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Match";
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Matches", null)
        };

        var dinamo = _clubRepo.GetDinamo();
        var matches = _matchRepo.GetByClub(dinamo?.Id ?? 1).OrderByDescending(m => m.Date).ToList();
        return View(matches);
    }

    public IActionResult Details(int id)
    {
        ViewData["ActivePage"] = "Match";
        var match = _matchRepo.GetById(id);
        if (match == null)
        {
            return NotFound();
        }

        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Matches", "/Match"),
            ($"{match.HomeClub.Name} vs {match.AwayClub.Name}", null)
        };

        return View(match);
    }
}
