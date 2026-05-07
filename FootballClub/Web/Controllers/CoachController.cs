using FootballClub.Models;
using FootballClub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Controllers;

public class CoachController : Controller
{
    private readonly CoachMockRepository _coachRepo;

    public CoachController(CoachMockRepository coachRepo)
    {
        _coachRepo = coachRepo;
    }

    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Coach";
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Staff", null)
        };

        var coaches = _coachRepo.GetCoachesForClub("Dinamo");
        return View(coaches);
    }

    public IActionResult Details(int id)
    {
        ViewData["ActivePage"] = "Coach";
        var coach = _coachRepo.GetById(id);
        if (coach == null)
        {
            return NotFound();
        }

        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Staff", "/Coach"),
            ($"{coach.FirstName} {coach.LastName}", null)
        };

        return View(coach);
    }
}
