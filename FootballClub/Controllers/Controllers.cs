using FootballClub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Controllers;

public class HomeController : Controller
{
    private readonly ClubMockRepository _clubs;
    private readonly PlayerMockRepository _players;
    private readonly MatchMockRepository _matches;

    public HomeController(ClubMockRepository clubs, PlayerMockRepository players, MatchMockRepository matches)
    {
        _clubs = clubs; _players = players; _matches = matches;
    }

    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Home";
        ViewBag.TotalClubs = _clubs.GetAll().Count;
        ViewBag.TotalPlayers = _players.GetAll().Count;
        ViewBag.TotalMatches = _matches.GetAll().Count;
        ViewBag.UpcomingMatch = _matches.GetUpcoming().FirstOrDefault();
        ViewBag.TopScorer = _players.GetAll()
            .Where(p => p.Stats.Any())
            .OrderByDescending(p => p.Stats.Sum(s => s.Goals))
            .FirstOrDefault();
        ViewBag.RecentMatches = _matches.GetFinished().Take(3).ToList();
        ViewBag.InjuredPlayers = _players.GetAll().Where(p => p.IsInjured).ToList();
        return View();
    }
}

public class ClubController : Controller
{
    private readonly ClubMockRepository _repo;
    public ClubController(ClubMockRepository repo) => _repo = repo;

    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Club";
        var clubs = _repo.GetAll();
        return View(clubs);
    }

    public IActionResult Details(int id)
    {
        ViewData["ActivePage"] = "Club";
        var club = _repo.GetById(id);
        if (club == null) return NotFound();
        return View(club);
    }
}

public class PlayerController : Controller
{
    private readonly PlayerMockRepository _repo;
    public PlayerController(PlayerMockRepository repo) => _repo = repo;

    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Player";
        var players = _repo.GetAll();
        return View(players);
    }

    public IActionResult Details(int id)
    {
        ViewData["ActivePage"] = "Player";
        var player = _repo.GetById(id);
        if (player == null) return NotFound();
        return View(player);
    }
}

public class MatchController : Controller
{
    private readonly MatchMockRepository _repo;
    public MatchController(MatchMockRepository repo) => _repo = repo;

    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Match";
        var matches = _repo.GetAll();
        return View(matches);
    }

    public IActionResult Details(int id)
    {
        ViewData["ActivePage"] = "Match";
        var match = _repo.GetById(id);
        if (match == null) return NotFound();
        return View(match);
    }
}

public class CoachController : Controller
{
    private readonly CoachMockRepository _repo;
    public CoachController(CoachMockRepository repo) => _repo = repo;

    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Coach";
        var coaches = _repo.GetAll();
        return View(coaches);
    }

    public IActionResult Details(int id)
    {
        ViewData["ActivePage"] = "Coach";
        var coach = _repo.GetById(id);
        if (coach == null) return NotFound();
        return View(coach);
    }
}