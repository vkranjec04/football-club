using FootballClub.Models;
using FootballClub.Repositories;
using FootballClub.Data;
using FootballClub.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballClub.Web.Controllers;

public class PlayerController : Controller
{
    private readonly PlayerMockRepository _playerRepo;
    private readonly ClubMockRepository _clubRepo;
    private readonly PlayerScheduleMockRepository _scheduleRepo;
    private readonly ApplicationDbContext _context;

    public PlayerController(
        PlayerMockRepository playerRepo,
        ClubMockRepository clubRepo,
        PlayerScheduleMockRepository scheduleRepo,
        ApplicationDbContext context)
    {
        _playerRepo = playerRepo;
        _clubRepo = clubRepo;
        _scheduleRepo = scheduleRepo;
        _context = context;
    }

    [HttpGet("api/clubs/search")]
    public IActionResult ClubSearch(string term)
    {
        var q = (term ?? string.Empty).Trim();
        if (q.Length < 1) return Json(Array.Empty<object>());

        var matches = _clubRepo.GetAll()
            .Where(c => c.Name.Contains(q, StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.Name)
            .Take(10)
            .Select(c => new { id = c.Id, text = c.Name })
            .ToList();

        return Json(matches);
    }

    [HttpGet]
    public IActionResult Filter(string term)
    {
        ViewBag.IsAdmin = true;
        var q = (term ?? string.Empty).Trim();
        var players = _context.Players
            .Include(p => p.Club)
            .ToList();

        if (!string.IsNullOrWhiteSpace(q))
        {
            players = players
                .Where(p => (p.FirstName + " " + p.LastName).Contains(q, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return PartialView("_PlayerRows", players);
    }

    [HttpGet("api/players/search")]
    public IActionResult Search(string term)
    {
        var query = (term ?? string.Empty).Trim();
        if (query.Length < 2)
        {
            return Json(Array.Empty<object>());
        }

        var matches = _playerRepo.GetAll()
            .Where(player =>
                player.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                player.LastName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                ($"{player.FirstName} {player.LastName}").Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(player => player.LastName)
            .ThenBy(player => player.FirstName)
            .Take(10)
            .Select(player => new
            {
                id = player.Id,
                text = $"{player.FirstName} {player.LastName}",
                subtitle = $"{player.Position} • {player.Club?.Name}",
                url = Url.Action(nameof(Details), new { id = player.Id })
            })
            .ToList();

        return Json(matches);
    }

    [Route("team-roster")]
    public IActionResult Index(FootballClub.Models.Enums.PlayerPosition? position)
    {
        ViewData["ActivePage"] = "Player";
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Players", null)
        };

        // For now, hardcode IsAdmin=true for testing. Will be replaced with auth check later.
        ViewBag.IsAdmin = true;

        var selectedClub = _clubRepo.GetDinamo();
        ViewBag.SelectedClub = selectedClub;
        ViewBag.SelectedPosition = position;

        // Include deleted players for admin view
        var players = selectedClub != null
            ? _context.Players.Include(p => p.Club).Where(p => p.ClubId == selectedClub.Id).ToList()
            : _context.Players.Include(p => p.Club).ToList();
        
        if (position.HasValue)
        {
            players = players.Where(p => p.Position == position.Value).ToList();
        }
        
        return View(players);
    }

    [Route("player-profile/{id:int}")]
    public IActionResult Details(int id)
    {
        ViewData["ActivePage"] = "Player";
        var player = _playerRepo.GetById(id);
        if (player == null)
        {
            return NotFound();
        }

        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Players", "/Player"),
            ($"{player.FirstName} {player.LastName}", null)
        };

        ViewBag.WeeklySchedule = _scheduleRepo.GetWeeklyScheduleForPlayer(player.Id);
        return View(player);
    }

    public IActionResult Schedule(FootballClub.Models.Enums.PlayerPosition? position)
    {
        ViewData["ActivePage"] = "Schedules";
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Player Schedules", null)
        };

        var selectedClub = _clubRepo.GetDinamo();
        if (selectedClub == null)
        {
            return NotFound();
        }

        ViewBag.SelectedClub = selectedClub;
        ViewBag.SelectedPosition = position;

        var byPlayer = _scheduleRepo.GetWeeklyScheduleByClub(selectedClub.Id);
        if (position.HasValue)
        {
            var filteredByPlayer = new Dictionary<FootballClub.Models.Player, List<FootballClub.Models.PlayerScheduleItem>>();
            foreach(var kvp in byPlayer)
            {
                if (kvp.Key.Position == position.Value)
                {
                    filteredByPlayer.Add(kvp.Key, kvp.Value);
                }
            }
            byPlayer = filteredByPlayer;
        }

        return View(byPlayer);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Clubs = _clubRepo.GetAll();
        ViewData["ActivePage"] = "Player";
        return View(new PlayerCreateModel { DateOfBirth = DateTime.Now.AddYears(-18), ContractUntil = DateTime.Now.AddYears(1) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(PlayerCreateModel model)
    {
        if (model.TrainingSessionId.HasValue && !TrainingSessionExists(model.TrainingSessionId.Value))
        {
            ModelState.AddModelError(nameof(model.TrainingSessionId), "Selected training session does not exist.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Clubs = _clubRepo.GetAll();
            return View(model);
        }

        var player = new Player
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            DateOfBirth = model.DateOfBirth,
            Nationality = model.Nationality,
            Position = model.Position,
            JerseyNumber = model.JerseyNumber,
            MarketValue = model.MarketValue,
            ContractUntil = model.ContractUntil,
            IsInjured = model.IsInjured,
            ClubId = model.ClubId,
            TrainingSessionId = model.TrainingSessionId > 0 ? model.TrainingSessionId : null
        };

        _context.Players.Add(player);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var player = _playerRepo.GetById(id);
        if (player == null) return NotFound();

        var model = new PlayerEditModel
        {
            Id = player.Id,
            FirstName = player.FirstName,
            LastName = player.LastName,
            DateOfBirth = player.DateOfBirth,
            Nationality = player.Nationality,
            Position = player.Position,
            JerseyNumber = player.JerseyNumber,
            MarketValue = player.MarketValue,
            ContractUntil = player.ContractUntil,
            IsInjured = player.IsInjured,
            ClubId = player.ClubId,
            TrainingSessionId = player.TrainingSessionId
        };

        ViewBag.Clubs = _clubRepo.GetAll();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, PlayerEditModel model)
    {
        if (id != model.Id) return BadRequest();
        if (model.TrainingSessionId.HasValue && !TrainingSessionExists(model.TrainingSessionId.Value))
        {
            ModelState.AddModelError(nameof(model.TrainingSessionId), "Selected training session does not exist.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Clubs = _clubRepo.GetAll();
            return View(model);
        }

        var player = _context.Players.FirstOrDefault(p => p.Id == id);
        if (player == null) return NotFound();

        // manual mapping to avoid overposting
        player.FirstName = model.FirstName;
        player.LastName = model.LastName;
        player.DateOfBirth = model.DateOfBirth;
        player.Nationality = model.Nationality;
        player.Position = model.Position;
        player.JerseyNumber = model.JerseyNumber;
        player.MarketValue = model.MarketValue;
        player.ContractUntil = model.ContractUntil;
        player.IsInjured = model.IsInjured;
        player.ClubId = model.ClubId;
        player.TrainingSessionId = model.TrainingSessionId > 0 ? model.TrainingSessionId : null;

        _context.Players.Update(player);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var player = _context.Players.FirstOrDefault(p => p.Id == id);
        if (player == null) return NotFound();

        player.IsDeleted = true;
        _context.Players.Update(player);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    private bool TrainingSessionExists(int trainingSessionId)
    {
        return _context.TrainingSessions.Any(session => session.Id == trainingSessionId);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Restore(int id)
    {
        var player = _context.Players.FirstOrDefault(p => p.Id == id && p.IsDeleted);
        if (player == null) return NotFound();

        player.IsDeleted = false;
        _context.Players.Update(player);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
