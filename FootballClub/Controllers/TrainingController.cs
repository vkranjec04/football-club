using FootballClub.Models;
using FootballClub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Controllers;

public class TrainingController : Controller
{
    private readonly TrainingMockRepository _training;
    private readonly ClubMockRepository _clubs;
    private readonly PlayerScheduleMockRepository _schedules;

    public TrainingController(
        TrainingMockRepository training,
        ClubMockRepository clubs,
        PlayerScheduleMockRepository schedules)
    {
        _training = training;
        _clubs = clubs;
        _schedules = schedules;
    }

    public IActionResult Index(FootballClub.Models.Enums.PlayerPosition? position)
    {
        ViewData["ActivePage"] = "Training";
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Training", null)
        };

        var selectedClub = _clubs.GetDinamo();
        if (selectedClub == null)
        {
            return NotFound();
        }

        ViewBag.SelectedClub = selectedClub;
        ViewBag.SelectedPosition = position;

        var schedulesForClub = _schedules.GetWeeklyScheduleByClub(selectedClub.Id);
        if (position.HasValue)
        {
            var filteredSchedules = new Dictionary<FootballClub.Models.Player, List<FootballClub.Models.PlayerScheduleItem>>();
            foreach(var kvp in schedulesForClub)
            {
                if (kvp.Key.Position == position.Value)
                {
                    filteredSchedules.Add(kvp.Key, kvp.Value);
                }
            }
            ViewBag.ScheduleByPlayer = filteredSchedules;
        }
        else
        {
            ViewBag.ScheduleByPlayer = schedulesForClub;
        }

        var sessions = _training.GetByClub(selectedClub.Id);
        return View(sessions);
    }

    public IActionResult Details(int id)
    {
        ViewData["ActivePage"] = "Training";
        var session = _training.GetById(id);
        if (session == null)
        {
            return NotFound();
        }

        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Training", "/Training"),
            (session.Title, null)
        };

        return View(session);
    }
}
