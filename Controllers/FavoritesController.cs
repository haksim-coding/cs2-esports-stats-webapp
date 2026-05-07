using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cs2_esports.Controllers;

public class FavoritesController : Controller
{
    private const string ForumUserSessionKey = "ForumUserId";
    private readonly IForumRepository _forumRepository;

    public FavoritesController(IForumRepository forumRepository)
    {
        _forumRepository = forumRepository;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ToggleTeam(int id, string? returnUrl = null)
    {
        var currentUser = GetCurrentForumUser();
        if (currentUser is null)
        {
            return RedirectToAction("Login", "Auth", new { returnUrl });
        }

        _forumRepository.ToggleFavoriteTeam(currentUser.Id, id);
        return LocalRedirect(returnUrl ?? Url.Action("Index", "Teams")!);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult TogglePlayer(int id, string? returnUrl = null)
    {
        var currentUser = GetCurrentForumUser();
        if (currentUser is null)
        {
            return RedirectToAction("Login", "Auth", new { returnUrl });
        }

        _forumRepository.ToggleFavoritePlayer(currentUser.Id, id);
        return LocalRedirect(returnUrl ?? Url.Action("Index", "Players")!);
    }

    private Models.ForumUser? GetCurrentForumUser()
    {
        var userId = HttpContext.Session.GetInt32(ForumUserSessionKey);
        return userId.HasValue ? _forumRepository.GetForumUserById(userId.Value) : null;
    }
}