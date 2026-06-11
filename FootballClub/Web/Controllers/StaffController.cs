using FootballClub.Models;
using FootballClub.Repositories;
using FootballClub.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FootballClub.Data;
namespace FootballClub.Web.Controllers;

public class StaffController : Controller
{
    private readonly StaffMockRepository _staffRepo;
    private readonly ClubMockRepository _clubRepo;
    private readonly ApplicationDbContext _context;

    public StaffController(
        StaffMockRepository staffRepo,
        ClubMockRepository clubRepo,
        ApplicationDbContext context)
    {
        _staffRepo = staffRepo;
        _clubRepo = clubRepo;
        _context = context;
    }

    [Route("staff-members")]
    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Staff";
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Staff", null)
        };

        // For now, hardcode IsAdmin=true for testing. Will be replaced with auth check later.
        ViewBag.IsAdmin = true;

        var staffMembers = _context.StaffMembers.ToList();
        return View(staffMembers);
    }

    [HttpGet]
    public IActionResult Filter(string term)
    {
        ViewBag.IsAdmin = true;
        var q = (term ?? string.Empty).Trim();
        var allStaff = _context.StaffMembers.ToList(); // Materialize first
        var staff = string.IsNullOrWhiteSpace(q)
            ? allStaff
            : allStaff.Where(s => (s.FirstName + " " + s.LastName).Contains(q, StringComparison.OrdinalIgnoreCase) || s.Role.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();

        return PartialView("_StaffRows", staff);
    }

    [HttpGet("api/staff/search")]
    public IActionResult Search(string term)
    {
        var query = (term ?? string.Empty).Trim();
        if (query.Length < 2)
        {
            return Json(Array.Empty<object>());
        }

        // Filter !IsDeleted in SQL, then materialize: the case-insensitive
        // string.Contains overload and Url.Action below cannot be translated by EF.
        var matches = _context.StaffMembers.Where(s => !s.IsDeleted)
            .AsEnumerable()
            .Where(staff =>
                staff.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                staff.LastName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                ($"{staff.FirstName} {staff.LastName}").Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(staff => staff.LastName)
            .ThenBy(staff => staff.FirstName)
            .Take(10)
            .Select(staff => new
            {
                id = staff.Id,
                text = $"{staff.FirstName} {staff.LastName}",
                subtitle = $"{staff.Role}",
                url = Url.Action(nameof(Details), new { id = staff.Id })
            })
            .ToList();

        return Json(matches);
    }

    [Route("staff-members/{id:int}")]
    public IActionResult Details(int id)
    {
        ViewData["ActivePage"] = "Staff";
        var staffMember = _staffRepo.GetById(id);
        if (staffMember == null)
        {
            return NotFound();
        }

        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Staff", "/staff-members"),
            ($"{staffMember.FirstName} {staffMember.LastName}", null)
        };

        return View(staffMember);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Clubs = _clubRepo.GetAll();
        ViewData["ActivePage"] = "Staff";
        return View(new StaffCreateModel { DateOfBirth = DateTime.Now.AddYears(-25), ContractUntil = DateTime.Now.AddYears(1) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(StaffCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Clubs = _clubRepo.GetAll();
            return View(model);
        }

        var staff = new Staff
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Nationality = model.Nationality,
            DateOfBirth = model.DateOfBirth,
            ContractUntil = model.ContractUntil,
            Role = model.Role,
            ClubId = model.ClubId
        };

        _context.StaffMembers.Add(staff);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var staff = _context.StaffMembers.FirstOrDefault(s => s.Id == id);
        if (staff == null) return NotFound();

        var model = new StaffEditModel
        {
            Id = staff.Id,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Nationality = staff.Nationality,
            DateOfBirth = staff.DateOfBirth,
            ContractUntil = staff.ContractUntil,
            Role = staff.Role,
            ClubId = staff.ClubId
        };

        ViewBag.Clubs = _clubRepo.GetAll();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, StaffEditModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewBag.Clubs = _clubRepo.GetAll();
            return View(model);
        }

        var staff = _context.StaffMembers.FirstOrDefault(s => s.Id == id);
        if (staff == null) return NotFound();

        // manual mapping to avoid overposting
        staff.FirstName = model.FirstName;
        staff.LastName = model.LastName;
        staff.Nationality = model.Nationality;
        staff.DateOfBirth = model.DateOfBirth;
        staff.ContractUntil = model.ContractUntil;
        staff.Role = model.Role;
        staff.ClubId = model.ClubId;

        _context.StaffMembers.Update(staff);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var staff = _context.StaffMembers.FirstOrDefault(s => s.Id == id);
        if (staff == null) return NotFound();

        staff.IsDeleted = true;
        _context.StaffMembers.Update(staff);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Restore(int id)
    {
        var staff = _context.StaffMembers.FirstOrDefault(s => s.Id == id);
        if (staff == null) return NotFound();

        staff.IsDeleted = false;
        _context.StaffMembers.Update(staff);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
