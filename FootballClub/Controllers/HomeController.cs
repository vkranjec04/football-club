using FootballClub.Models;
using FootballClub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Controllers;

public class HomeController : Controller
{
    private readonly ClubMockRepository _clubs;
    private readonly PlayerMockRepository _players;
    private readonly MatchMockRepository _matches;
    private readonly TrainingMockRepository _training;
    private readonly PlayerScheduleMockRepository _schedules;

    public HomeController(
        ClubMockRepository clubs,
        PlayerMockRepository players,
        MatchMockRepository matches,
        TrainingMockRepository training,
        PlayerScheduleMockRepository schedules)
    {
        _clubs = clubs;
        _players = players;
        _matches = matches;
        _training = training;
        _schedules = schedules;
    }

    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Home";
        var club = _clubs.GetDinamo();
        if (club == null)
        {
            return View();
        }

        var clubPlayers = _players.GetByClub(club.Id);
        var upcomingMatches = _matches.GetUpcomingByClub(club.Id);
        var recentMatches = _matches.GetFinishedByClub(club.Id).Take(3).ToList();
        var upcomingTraining = _training.GetByClub(club.Id).Where(s => s.IsUpcoming).ToList();
        var schedules = _schedules.GetWeeklyScheduleByClub(club.Id);

        ViewBag.ManagedClub = club;
        ViewBag.TotalPlayers = clubPlayers.Count;
        ViewBag.UpcomingMatches = upcomingMatches.Count;
        ViewBag.InjuredPlayers = clubPlayers.Where(p => p.IsInjured).ToList();
        ViewBag.UpcomingTraining = upcomingTraining;
        ViewBag.TodayTrainingCount = upcomingTraining.Count(s => s.IsToday);
        ViewBag.NextTraining = upcomingTraining.FirstOrDefault();
        ViewBag.RecentMatches = recentMatches;
        ViewBag.TotalWeeklyResponsibilities = schedules.Sum(x => x.Value.Count);
        ViewBag.MediaResponsibilities = schedules.Sum(x => x.Value.Count(i => i.ResponsibilityType.ToString().Contains("Media")));

        return View();
    }
}
