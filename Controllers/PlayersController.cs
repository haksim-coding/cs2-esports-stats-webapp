using cs2_esports.Models;
using cs2_esports.Helpers;
using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cs2_esports.Controllers;

public class PlayersController : Controller
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IForumRepository _forumRepository;
    private const string ForumUserSessionKey = "ForumUserId";

    public PlayersController(IPlayerRepository playerRepository, IForumRepository forumRepository)
    {
        _playerRepository = playerRepository;
        _forumRepository = forumRepository;
    }

    public IActionResult Index()
    {
        var players = _playerRepository.GetAllAlphabetical();
        ApplyFavoriteState(players);
        return View(players);
    }

    public IActionResult Details(int id)
    {
        var player = _playerRepository.GetById(id);
        if (player is null)
        {
            return NotFound();
        }

        ApplyFavoriteState(new[] { player });
        return View(player);
    }

    [HttpGet("/player/{slug}")]
    public IActionResult DetailsBySlug(string slug)
    {
        var playerSummary = _playerRepository.GetAllAlphabetical().FirstOrDefault(player =>
            RouteSlugHelper.MatchesRouteSegment(player.Nickname, slug));

        if (playerSummary is null)
        {
            return NotFound();
        }

        var player = _playerRepository.GetById(playerSummary.Id);
        if (player is null)
        {
            return NotFound();
        }

        ApplyFavoriteState(new[] { player });
        return View("Details", player);
    }

    private void ApplyFavoriteState(IEnumerable<Player> players)
    {
        var currentUser = GetCurrentForumUser();
        if (currentUser is null)
        {
            return;
        }

        var favoritePlayerIds = _forumRepository.GetFavoritePlayers(currentUser.Id).Select(player => player.Id).ToHashSet();
        foreach (var player in players)
        {
            player.IsFavorite = favoritePlayerIds.Contains(player.Id);
        }
    }

    private ForumUser? GetCurrentForumUser()
    {
        var userId = HttpContext.Session.GetInt32(ForumUserSessionKey);
        return userId.HasValue ? _forumRepository.GetForumUserById(userId.Value) : null;
    }
}