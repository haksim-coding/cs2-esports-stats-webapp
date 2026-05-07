using cs2_esports.Repositories.Interfaces;
using cs2_esports.Models;
using cs2_esports.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cs2_esports.Controllers;

public class AuthController : Controller
{
    private const string ForumUserSessionKey = "ForumUserId";
    private readonly IForumRepository _forumRepository;
    private readonly Cs2ScopeDbContext _dbContext;

    public AuthController(IForumRepository forumRepository, Cs2ScopeDbContext dbContext)
    {
        _forumRepository = forumRepository;
        _dbContext = dbContext;
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
        if (forumUser is not null && string.Equals(forumUser.Password, input.Password, StringComparison.Ordinal))
        {
            HttpContext.Session.SetInt32(ForumUserSessionKey, forumUser.Id);
            forumUser.LastActiveAtUtc = DateTime.UtcNow;

            TempData["LoginMessage"] = $"Welcome back, {forumUser.DisplayName}.";
            return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
        }

        var adminUser = _dbContext.AdminUsers.FirstOrDefault(user =>
            string.Equals(user.Username, input.Username, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(user.Email, input.Username, StringComparison.OrdinalIgnoreCase));

        if (adminUser is null || !string.Equals(adminUser.Password, input.Password, StringComparison.Ordinal))
        {
            ModelState.AddModelError(string.Empty, "Invalid login details.");
            ViewData["ReturnUrl"] = returnUrl;
            return View(input);
        }

        HttpContext.Session.SetInt32(ForumUserSessionKey, adminUser.Id);
        TempData["LoginMessage"] = $"Welcome back, {adminUser.DisplayName}.";
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

    [HttpGet]
    public IActionResult Profile()
    {
        var currentUser = GetCurrentForumUser();
        if (currentUser is null)
        {
            return RedirectToAction(nameof(Login), new { returnUrl = Url.Action(nameof(Profile)) });
        }

        var model = new ForumUserProfileViewModel
        {
            User = currentUser,
            FavoriteTeams = _forumRepository.GetFavoriteTeams(currentUser.Id),
            FavoritePlayers = _forumRepository.GetFavoritePlayers(currentUser.Id)
        };

        return View(model);
    }

    private ForumUser? GetCurrentForumUser()
    {
        var userId = HttpContext.Session.GetInt32(ForumUserSessionKey);
        return userId.HasValue ? _forumRepository.GetForumUserById(userId.Value) : null;
    }
}
