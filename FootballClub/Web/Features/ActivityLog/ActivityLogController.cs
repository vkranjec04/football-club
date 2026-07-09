using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Features.ActivityLog;

/// <summary>
/// Renders the audit-log viewer shell. The page is admin-gated on the client (see the view)
/// and pulls its data from the admin-only <see cref="Api.ActivityLogsApiController"/> using
/// the caller's bearer token — consistent with how the rest of the data-driven pages load.
/// </summary>
public class ActivityLogController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["ActivePage"] = "ActivityLog";
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Activity Log", null)
        };

        return View();
    }
}
