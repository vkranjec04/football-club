using FootballClub.Models;
using FootballClub.Repositories;
using FootballClub.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FootballClub.Data;
namespace FootballClub.Web.Controllers;

public class TrainingController : Controller
{
    private readonly TrainingMockRepository _training;
    private readonly ClubMockRepository _clubs;
    private readonly PlayerScheduleMockRepository _schedules;
    private readonly ApplicationDbContext _context;
    private readonly StaffMockRepository _staffRepo;

    public TrainingController(
        TrainingMockRepository training,
        ClubMockRepository clubs,
        PlayerScheduleMockRepository schedules,
        ApplicationDbContext context,
        StaffMockRepository staffRepo)
    {
        _training = training;
        _clubs = clubs;
        _schedules = schedules;
        _context = context;
        _staffRepo = staffRepo;
    }

    public IActionResult Index(FootballClub.Models.Enums.PlayerPosition? position)
    {
        ViewData["ActivePage"] = "Training";
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Training", null)
        };

        // For now, hardcode IsAdmin=true for testing. Will be replaced with auth check later.
        ViewBag.IsAdmin = true;

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

        var sessions = _context.TrainingSessions.Where(ts => ts.ClubId == selectedClub.Id).ToList();
        return View(sessions);
    }

    [HttpGet]
    public IActionResult Filter(string term)
    {
        ViewBag.IsAdmin = true;
        var q = (term ?? string.Empty).Trim();
        var selectedClub = _clubs.GetDinamo();
        var allSessions = _context.TrainingSessions.Where(ts => ts.ClubId == selectedClub.Id).ToList(); // Materialize first
        
        var sessions = string.IsNullOrWhiteSpace(q)
            ? allSessions
            : allSessions.Where(ts => ts.Title.Contains(q, StringComparison.OrdinalIgnoreCase) || 
                ts.FocusArea.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();

        return PartialView("_TrainingRows", sessions);
    }

    [HttpGet("api/training/search")]
    public IActionResult Search(string term)
    {
        var query = (term ?? string.Empty).Trim();
        if (query.Length < 2)
        {
            return Json(Array.Empty<object>());
        }

        var matches = _context.TrainingSessions.Where(ts => !ts.IsDeleted)
            .Where(session =>
                session.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                session.FocusArea.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(session => session.StartTime)
            .Take(10)
            .Select(session => new
            {
                id = session.Id,
                text = session.Title,
                subtitle = $"{session.StartTime:dd.MM.yyyy HH:mm} • {session.FocusArea}",
                url = Url.Action(nameof(Details), new { id = session.Id })
            })
            .ToList();

        return Json(matches);
    }

    public IActionResult Details(int id)
    {
        ViewData["ActivePage"] = "Training";
        var session = _context.TrainingSessions
            .Include(ts => ts.Club)
            .Include(ts => ts.LeadStaff)
            .Include(ts => ts.Participants)
            .FirstOrDefault(ts => ts.Id == id);
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

    [HttpGet]
    public IActionResult Create()
    {
        var selectedClub = _clubs.GetDinamo();
        if (selectedClub == null)
        {
            return NotFound();
        }

        ViewBag.Clubs = new List<Club> { selectedClub };
        ViewBag.Staff = _context.StaffMembers.Where(s => !s.IsDeleted && s.ClubId == selectedClub.Id).ToList();
        ViewData["ActivePage"] = "Training";
        
        return View(new TrainingSessionCreateModel 
        { 
            ClubId = selectedClub.Id,
            StartTime = DateTime.Now.AddDays(1).Date.AddHours(14),
            EndTime = DateTime.Now.AddDays(1).Date.AddHours(15)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(TrainingSessionCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            var selectedClub = _clubs.GetDinamo();
            ViewBag.Clubs = selectedClub == null ? new List<Club>() : new List<Club> { selectedClub };
            ViewBag.Staff = _context.StaffMembers.Where(s => !s.IsDeleted && s.ClubId == model.ClubId).ToList();
            return View(model);
        }

        var session = new TrainingSession
        {
            ClubId = model.ClubId,
            Title = model.Title,
            FocusArea = model.FocusArea,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            Location = model.Location,
            Intensity = model.Intensity,
            LeadStaffId = model.LeadStaffId > 0 ? model.LeadStaffId : null,
            Notes = model.Notes
        };

        _context.TrainingSessions.Add(session);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var session = _context.TrainingSessions.FirstOrDefault(ts => ts.Id == id);
        if (session == null) return NotFound();

        var model = new TrainingSessionEditModel
        {
            Id = session.Id,
            Title = session.Title,
            FocusArea = session.FocusArea,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            Location = session.Location,
            Intensity = session.Intensity,
            LeadStaffId = session.LeadStaffId,
            ClubId = session.ClubId,
            Notes = session.Notes
        };

        var selectedClub = _clubs.GetDinamo();
        ViewBag.Clubs = selectedClub == null ? new List<Club>() : new List<Club> { selectedClub };
        ViewBag.Staff = _context.StaffMembers.Where(s => !s.IsDeleted && s.ClubId == session.ClubId).ToList();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, TrainingSessionEditModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            var selectedClub = _clubs.GetDinamo();
            ViewBag.Clubs = selectedClub == null ? new List<Club>() : new List<Club> { selectedClub };
            ViewBag.Staff = _context.StaffMembers.Where(s => !s.IsDeleted && s.ClubId == model.ClubId).ToList();
            return View(model);
        }

        var session = _context.TrainingSessions.FirstOrDefault(ts => ts.Id == id);
        if (session == null) return NotFound();

        session.Title = model.Title;
        session.FocusArea = model.FocusArea;
        session.StartTime = model.StartTime;
        session.EndTime = model.EndTime;
        session.Location = model.Location;
        session.Intensity = model.Intensity;
        session.LeadStaffId = model.LeadStaffId > 0 ? model.LeadStaffId : null;
        session.Notes = model.Notes;

        _context.TrainingSessions.Update(session);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var session = _context.TrainingSessions.FirstOrDefault(ts => ts.Id == id);
        if (session == null) return NotFound();

        session.IsDeleted = true;
        _context.TrainingSessions.Update(session);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Restore(int id)
    {
        var session = _context.TrainingSessions.FirstOrDefault(ts => ts.Id == id);
        if (session == null) return NotFound();

        session.IsDeleted = false;
        _context.TrainingSessions.Update(session);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
