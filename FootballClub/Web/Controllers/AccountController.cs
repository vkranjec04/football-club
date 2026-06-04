using FootballClub.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        ViewData["ActivePage"] = "Account";
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        ViewData["ActivePage"] = "Account";
        return View();
    }

    [HttpGet]
    public IActionResult ExternalLogin(string token, string username, string role, string? returnUrl = null)
    {
        ViewData["ActivePage"] = "Account";
        return View(new ExternalLoginViewModel
        {
            Token = token,
            Username = username,
            Role = role,
            ReturnUrl = returnUrl ?? "/"
        });
    }
}
