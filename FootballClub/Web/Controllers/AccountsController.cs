using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Controllers;

/// <summary>
/// Renders the account-management shell (distinct from <see cref="AccountController"/>, which
/// serves the login/register pages). The page is admin-gated on the client (see the view) and
/// pulls its data from the admin-only <see cref="Api.UsersApiController"/> using the caller's
/// bearer token — consistent with how the rest of the data-driven pages load.
/// </summary>
public class AccountsController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Accounts";
        ViewData["Breadcrumbs"] = new List<(string, string?)>
        {
            ("Dashboard", "/"),
            ("Accounts", null)
        };

        return View();
    }
}
