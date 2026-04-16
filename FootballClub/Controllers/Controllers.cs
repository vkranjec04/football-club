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
        var dinamo = _clubs.GetDinamo();
        
        ViewBag.DinamoClub = dinamo;
        ViewBag.TotalPlayers = _players.GetByClub(dinamo?.Id ?? 1).Count;
        ViewBag.UpcomingMatches = _matches.GetUpcomingByClub(dinamo?.Id ?? 1).Count;
        ViewBag.RecentMatches = _matches.GetFinishedByClub(dinamo?.Id ?? 1).Take(3).ToList();
        ViewBag.TopScorer = _players.GetByClub(dinamo?.Id ?? 1)
            .Where(p => p.Stats.Any())
            .OrderByDescending(p => p.Stats.Sum(s => s.Goals))
            .FirstOrDefault();
        ViewBag.InjuredPlayers = _players.GetByClub(dinamo?.Id ?? 1).Where(p => p.IsInjured).ToList();
        
        var upcomingMatch = _matches.GetUpcomingByClub(dinamo?.Id ?? 1).FirstOrDefault();
        ViewBag.UpcomingMatch = upcomingMatch;
        
        return View();
    }
}

public class LeagueController : Controller
{
    private readonly ClubMockRepository _repo;
    public LeagueController(ClubMockRepository repo) => _repo = repo;

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

    public IActionResult Details(int id)
    {
        ViewData["ActivePage"] = "League";
        var club = _repo.GetById(id);
        if (club == null) return NotFound();
        
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("League", "/League"),
            (club.Name, null)
        };
        
        return View(club);
    }
}

public class PlayerController : Controller
{
    private readonly PlayerMockRepository _playerRepo;
    private readonly ClubMockRepository _clubRepo;
    
    public PlayerController(PlayerMockRepository playerRepo, ClubMockRepository clubRepo)
    {
        _playerRepo = playerRepo;
        _clubRepo = clubRepo;
    }

    public IActionResult Index(int? clubId)
    {
        ViewData["ActivePage"] = "Player";
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Players", null)
        };
        
        // Default to Dinamo
        if (!clubId.HasValue)
        {
            var dinamo = _clubRepo.GetDinamo();
            clubId = dinamo?.Id ?? 1;
        }
        
        var selectedClub = _clubRepo.GetById(clubId.Value);
        ViewBag.SelectedClub = selectedClub;
        ViewBag.AllClubs = _clubRepo.GetAll();
        
        var players = _playerRepo.GetByClubOrdered(clubId.Value);
        return View(players);
    }

    public IActionResult Details(int id)
    {
        ViewData["ActivePage"] = "Player";
        var player = _playerRepo.GetById(id);
        if (player == null) return NotFound();
        
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Players", "/Player"),
            ($"{player.FirstName} {player.LastName}", null)
        };
        
        return View(player);
    }
}

public class MatchController : Controller
{
    private readonly MatchMockRepository _matchRepo;
    private readonly ClubMockRepository _clubRepo;
    
    public MatchController(MatchMockRepository matchRepo, ClubMockRepository clubRepo)
    {
        _matchRepo = matchRepo;
        _clubRepo = clubRepo;
    }

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
        if (match == null) return NotFound();
        
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Matches", "/Match"),
            ($"{match.HomeClub.Name} vs {match.AwayClub.Name}", null)
        };
        
        return View(match);
    }
}

public class CoachController : Controller
{
    private readonly CoachMockRepository _coachRepo;
    private readonly ClubMockRepository _clubRepo;
    
    public CoachController(CoachMockRepository coachRepo, ClubMockRepository clubRepo)
    {
        _coachRepo = coachRepo;
        _clubRepo = clubRepo;
    }

    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Coach";
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Coaches", null)
        };
        
        var coaches = _coachRepo.GetCoachesForClub("Dinamo");
        return View(coaches);
    }

    public IActionResult Details(int id)
    {
        ViewData["ActivePage"] = "Coach";
        var coach = _coachRepo.GetById(id);
        if (coach == null) return NotFound();
        
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Coaches", "/Coach"),
            ($"{coach.FirstName} {coach.LastName}", null)
        };
        
        return View(coach);
    }
}