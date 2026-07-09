using FootballClub.Models;
using FootballClub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Features.League;

public class LeagueController : Controller
{
    private readonly ClubMockRepository _repo;
    public LeagueController(ClubMockRepository repo) => _repo = repo;

    [Route("league-standings")]
    [Route("standings/{leagueName?}")]
    public IActionResult Index()
    {
        ViewData["ActivePage"] = "League";
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("League", null)
        };
        
        var standings = _repo.GetLeagueStandings("HNL");
        return View(standings);
    }
}
