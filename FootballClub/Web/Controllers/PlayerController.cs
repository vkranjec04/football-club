using FootballClub.Models;
using FootballClub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Controllers;

public class PlayerController : Controller
{
    private readonly PlayerMockRepository _playerRepo;
    private readonly ClubMockRepository _clubRepo;
    private readonly PlayerScheduleMockRepository _scheduleRepo;

    public PlayerController(
        PlayerMockRepository playerRepo,
        ClubMockRepository clubRepo,
        PlayerScheduleMockRepository scheduleRepo)
    {
        _playerRepo = playerRepo;
        _clubRepo = clubRepo;
        _scheduleRepo = scheduleRepo;
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

        var selectedClub = _clubRepo.GetDinamo();
        ViewBag.SelectedClub = selectedClub;
        ViewBag.SelectedPosition = position;

        var players = _playerRepo.GetByClubOrdered(selectedClub.Id);
        if (position.HasValue)
        {
            players = players.Where(p => p.Position == position.Value).ToList();
        }
        
        return View(players);
    }

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
}
