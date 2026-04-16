using cs2_esports.Repositories.Interfaces;
using cs2_esports.Models;
using Microsoft.AspNetCore.Mvc;

namespace cs2_esports.Controllers;

public class AuthController : Controller
{
    private const string ForumUserSessionKey = "ForumUserId";
    private readonly IForumRepository _forumRepository;

    public AuthController(IForumRepository forumRepository)
    {
        _forumRepository = forumRepository;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginInputModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginInputModel input, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(input);
        }

        var forumUser = _forumRepository.GetForumUserByUsernameOrEmail(input.Username);
        if (forumUser is null || !string.Equals(forumUser.Password, input.Password, StringComparison.Ordinal))
        {
            ModelState.AddModelError(string.Empty, "Invalid login details.");
            ViewData["ReturnUrl"] = returnUrl;
            return View(input);
        }

        HttpContext.Session.SetInt32(ForumUserSessionKey, forumUser.Id);
        forumUser.LastActiveAtUtc = DateTime.UtcNow;

        TempData["LoginMessage"] = $"Welcome back, {forumUser.DisplayName}.";
        return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new ForumRegisterInputModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(ForumRegisterInputModel input, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(input);
        }

        var createdUser = _forumRepository.RegisterForumUser(input);
        if (createdUser is null)
        {
            ModelState.AddModelError(string.Empty, "Username or email is already in use.");
            ViewData["ReturnUrl"] = returnUrl;
            return View(input);
        }

        HttpContext.Session.SetInt32(ForumUserSessionKey, createdUser.Id);
        TempData["LoginMessage"] = $"Welcome, {createdUser.DisplayName}. Your account is ready.";

        return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove(ForumUserSessionKey);
        TempData["LoginMessage"] = "You have been logged out.";
        return RedirectToAction("Index", "Home");
    }
}
