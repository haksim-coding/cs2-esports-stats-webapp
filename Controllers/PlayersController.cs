using cs2_esports.Models;
using cs2_esports.Helpers;
using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace cs2_esports.Controllers;

public class PlayersController : Controller
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IForumRepository _forumRepository;
    private readonly ITeamRepository _teamRepository;
    private const string ForumUserSessionKey = "ForumUserId";

    public PlayersController(IPlayerRepository playerRepository, IForumRepository forumRepository, ITeamRepository teamRepository)
    {
        _playerRepository = playerRepository;
        _forumRepository = forumRepository;
        _teamRepository = teamRepository;
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

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        var model = new PlayerCreateModel
        {
            DateOfBirth = new DateTime(2000, 1, 1)
        };

        PopulateTeams(model.TeamId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(PlayerCreateModel model)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        ValidatePlayer(model);

        if (!ModelState.IsValid)
        {
            PopulateTeams(model.TeamId);
            return View(model);
        }

        var player = MapToPlayer(model);
        _playerRepository.Add(player);
        return RedirectToAction(nameof(DetailsBySlug), new { slug = RouteSlugHelper.ToRouteSegment(player.Nickname) });
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        var player = _playerRepository.GetById(id);
        if (player is null)
        {
            return NotFound();
        }

        var model = MapToEditModel(player);
        PopulateTeams(model.TeamId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(PlayerEditModel model)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        var player = _playerRepository.GetById(model.Id);
        if (player is null)
        {
            return NotFound();
        }

        ValidatePlayer(model, model.Id);

        if (!ModelState.IsValid)
        {
            PopulateTeams(model.TeamId);
            return View(model);
        }

        player.Nickname = model.Nickname.Trim();
        player.FullName = model.FullName.Trim();
        player.CountryCode = model.CountryCode.Trim().ToUpperInvariant();
        player.DateOfBirth = model.DateOfBirth;
        player.Role = model.Role;
        player.Rating2 = model.Rating2;
        player.TotalMapsPlayed = model.TotalMapsPlayed;
        player.TeamId = model.TeamId;

        _playerRepository.Update(player);
        return RedirectToAction(nameof(DetailsBySlug), new { slug = RouteSlugHelper.ToRouteSegment(player.Nickname) });
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        var player = _playerRepository.GetById(id);
        if (player is null)
        {
            return NotFound();
        }

        return View(player);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        var player = _playerRepository.GetById(id);
        if (player is null)
        {
            return NotFound();
        }

        _playerRepository.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/Player/Search")]
    public IActionResult Search(string query, int? currentTeamId = null)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
        {
            return Json(Array.Empty<object>());
        }

        var players = _playerRepository.SearchAvailableByNickname(query, currentTeamId, 8);
        return Json(players.Select(player => new { id = player.Id, text = player.Nickname }));
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

    private void PopulateTeams(int? selectedTeamId)
    {
        ViewBag.Teams = new SelectList(_teamRepository.GetAll().OrderBy(team => team.WorldRanking).ToList(), nameof(Team.Id), nameof(Team.Name), selectedTeamId);
    }

    private static Player MapToPlayer(PlayerCreateModel model)
    {
        return new Player
        {
            Nickname = model.Nickname.Trim(),
            FullName = model.FullName.Trim(),
            CountryCode = model.CountryCode.Trim().ToUpperInvariant(),
            DateOfBirth = model.DateOfBirth,
            Role = model.Role,
            Rating2 = model.Rating2,
            TotalMapsPlayed = model.TotalMapsPlayed,
            TeamId = model.TeamId,
            JoinedTeamAtUtc = DateTime.UtcNow
        };
    }

    private static PlayerEditModel MapToEditModel(Player player)
    {
        return new PlayerEditModel
        {
            Id = player.Id,
            Nickname = player.Nickname,
            FullName = player.FullName,
            CountryCode = player.CountryCode,
            DateOfBirth = player.DateOfBirth,
            Role = player.Role,
            Rating2 = player.Rating2,
            TotalMapsPlayed = player.TotalMapsPlayed,
            TeamId = player.TeamId
        };
    }

    private void ValidatePlayer(PlayerCreateModel model, int? currentPlayerId = null)
    {
        var normalizedNickname = model.Nickname.Trim();
        var players = _playerRepository.GetAllAlphabetical();

        if (players.Any(player => player.Id != currentPlayerId && player.Nickname.Equals(normalizedNickname, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(model.Nickname), "A player with this nickname already exists.");
        }

        if (model.TeamId.HasValue && !_teamRepository.GetAll().Any(team => team.Id == model.TeamId.Value))
        {
            ModelState.AddModelError(nameof(model.TeamId), "The selected team could not be found.");
        }
    }

    private bool IsAdminUser()
    {
        return string.Equals(HttpContext.Session.GetString(AuthSessionKeys.UserType), AuthSessionKeys.AdminUserType, StringComparison.Ordinal)
            && HttpContext.Session.GetInt32(AuthSessionKeys.AdminUserId).HasValue;
    }
}