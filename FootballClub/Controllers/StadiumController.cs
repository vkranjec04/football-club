using FootballClub.Data;
using FootballClub.Models;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Controllers
{
    [Route("stadiums")]
    public class StadiumController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StadiumController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /stadiums/list
        [Route("list")]
        public IActionResult Index()
        {
            ViewData["ActivePage"] = "Stadium";
            ViewData["Breadcrumbs"] = new List<(string, string?)>
            {
                ("Dashboard", "/"),
                ("Stadiums", null)
            };

            var stadiums = _context.Stadiums.OrderByDescending(s => s.Capacity).ToList();
            return View(stadiums);
        }
    }
}