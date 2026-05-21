using cs2_esports.Helpers;
using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cs2_esports.Controllers;

public class TeamsController : Controller
{
    private readonly ITeamRepository _teamRepository;
    private readonly IForumRepository _forumRepository;
    private readonly IPlayerRepository _playerRepository;
    private const string ForumUserSessionKey = "ForumUserId";

    public TeamsController(ITeamRepository teamRepository, IForumRepository forumRepository, IPlayerRepository playerRepository)
    {
        _teamRepository = teamRepository;
        _forumRepository = forumRepository;
        _playerRepository = playerRepository;
    }

    public IActionResult Index()
    {
        var teams = _teamRepository.GetAll();
        ApplyFavoriteState(teams);
        return View(teams);
    }

    public IActionResult Details(int id)
    {
        var team = _teamRepository.GetById(id);
        if (team is null)
        {
            return NotFound();
        }

        ApplyFavoriteState(new[] { team });
        return View(team);
    }

    [HttpGet("/team/{slug}")]
    public IActionResult DetailsBySlug(string slug)
    {
        var teamSummary = _teamRepository.GetAll().FirstOrDefault(team =>
            RouteSlugHelper.MatchesRouteSegment(team.Name, slug) || RouteSlugHelper.MatchesRouteSegment(team.Tag, slug));

        if (teamSummary is null)
        {
            return NotFound();
        }

        var team = _teamRepository.GetById(teamSummary.Id);
        if (team is null)
        {
            return NotFound();
        }

        ApplyFavoriteState(new[] { team });
        return View("Details", team);
    }

    [HttpGet("/Team/Search")]
    public IActionResult Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
        {
            return Json(Array.Empty<object>());
        }

        var teams = _teamRepository.SearchByNameOrTag(query, 8);
        return Json(teams.Select(team => new
        {
            id = team.Id,
            text = team.Name,
            logoPath = TeamLogoResolver.GetLogoPath(team.Name, team.Tag),
            badgeText = TeamLogoResolver.GetBadgeText(team.Name, team.Tag)
        }));
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        return View(new TeamCreateModel
        {
            FoundedYear = DateTime.UtcNow.Year
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(TeamCreateModel model)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        NormalizeSelectedPlayers(model);
        ValidateTeamUniqueness(model);

        if (!ModelState.IsValid)
        {
            PopulateSelectedPlayers(model);
            return View(model);
        }

        var selectedPlayers = _playerRepository.GetByIds(model.SelectedPlayerIds);
        var team = CreateTeamEntity(model, selectedPlayers);
        _teamRepository.Add(team);
        return RedirectToAction(nameof(Details), new { id = team.Id });
    }

    [HttpGet("/team/{slug}/edit")]
    public IActionResult Edit(string slug)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        var teamSummary = _teamRepository.GetAll().FirstOrDefault(team =>
            RouteSlugHelper.MatchesRouteSegment(team.Name, slug) || RouteSlugHelper.MatchesRouteSegment(team.Tag, slug));

        if (teamSummary is null)
        {
            return NotFound();
        }

        var team = _teamRepository.GetById(teamSummary.Id);
        if (team is null)
        {
            return NotFound();
        }

        return View(MapToEditModel(team));
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/team/{slug}/edit")]
    public IActionResult Edit(string slug, TeamEditModel model)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        model.RouteSlug = slug;

        var existingTeam = _teamRepository.GetById(model.Id);
        if (existingTeam is null)
        {
            return NotFound();
        }

        NormalizeSelectedPlayers(model);
        ValidateTeamUniqueness(model, model.Id);

        if (!ModelState.IsValid)
        {
            PopulateSelectedPlayers(model);
            return View(model);
        }

        var selectedPlayers = _playerRepository.GetByIds(model.SelectedPlayerIds);
        ApplyModelToTeam(existingTeam, model, selectedPlayers);
        _teamRepository.Update(existingTeam);
        return RedirectToAction(nameof(DetailsBySlug), new { slug = RouteSlugHelper.ToRouteSegment(existingTeam.Name) });
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        var team = _teamRepository.GetById(id);
        if (team is null)
        {
            return NotFound();
        }

        return View(team);
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

        var team = _teamRepository.GetById(id);
        if (team is null)
        {
            return NotFound();
        }

        if ((team.Players?.Any() ?? false) || (team.Tournaments?.Any() ?? false) || (team.HomeMatches?.Any() ?? false) || (team.AwayMatches?.Any() ?? false))
        {
            ModelState.AddModelError(string.Empty, "This team cannot be deleted because it is still used by players, events, or matches.");
            return View("Delete", team);
        }

        _teamRepository.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    private void ApplyFavoriteState(IEnumerable<Team> teams)
    {
        var currentUser = GetCurrentForumUser();
        if (currentUser is null)
        {
            return;
        }

        var favoriteTeamIds = _forumRepository.GetFavoriteTeams(currentUser.Id).Select(team => team.Id).ToHashSet();
        foreach (var team in teams)
        {
            team.IsFavorite = favoriteTeamIds.Contains(team.Id);
        }
    }

    private ForumUser? GetCurrentForumUser()
    {
        var userId = HttpContext.Session.GetInt32(ForumUserSessionKey);
        return userId.HasValue ? _forumRepository.GetForumUserById(userId.Value) : null;
    }

    private bool IsAdminUser()
    {
        return string.Equals(HttpContext.Session.GetString(AuthSessionKeys.UserType), AuthSessionKeys.AdminUserType, StringComparison.Ordinal)
            && HttpContext.Session.GetInt32(AuthSessionKeys.AdminUserId).HasValue;
    }

    private void ValidateTeamUniqueness(TeamCreateModel model, int? currentTeamId = null)
    {
        var normalizedName = model.Name.Trim();
        var normalizedTag = model.Tag.Trim();
        var teams = _teamRepository.GetAll();

        if (teams.Any(team => team.Id != currentTeamId && team.Name.Equals(normalizedName, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(model.Name), "A team with this name already exists.");
        }

        if (teams.Any(team => team.Id != currentTeamId && team.Tag.Equals(normalizedTag, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(model.Tag), "A team with this tag already exists.");
        }
    }

    private int GetNextWorldRanking()
    {
        return _teamRepository.GetAll().Select(team => team.WorldRanking).DefaultIfEmpty(0).Max() + 1;
    }

    private Team CreateTeamEntity(TeamCreateModel model, IReadOnlyCollection<Player> selectedPlayers)
    {
        return new Team
        {
            Name = model.Name.Trim(),
            Tag = model.Tag.Trim().ToUpperInvariant(),
            CountryCode = model.CountryCode.Trim().ToUpperInvariant(),
            WorldRanking = GetNextWorldRanking(),
            FoundedYear = model.FoundedYear,
            PrizeMoneyUsd = 0,
            LastRosterUpdateUtc = DateTime.UtcNow,
            Players = selectedPlayers.ToList()
        };
    }

    private static void ApplyModelToTeam(Team team, TeamCreateModel model, IReadOnlyCollection<Player> selectedPlayers)
    {
        team.Name = model.Name.Trim();
        team.Tag = model.Tag.Trim().ToUpperInvariant();
        team.CountryCode = model.CountryCode.Trim().ToUpperInvariant();
        team.FoundedYear = model.FoundedYear;
        team.LastRosterUpdateUtc = DateTime.UtcNow;
        team.Players = selectedPlayers.ToList();
    }

    private TeamEditModel MapToEditModel(Team team)
    {
        var model = new TeamEditModel
        {
            Id = team.Id,
            RouteSlug = RouteSlugHelper.ToRouteSegment(team.Name),
            Name = team.Name,
            Tag = team.Tag,
            CountryCode = team.CountryCode,
            FoundedYear = team.FoundedYear,
            SelectedPlayerIds = team.Players
                .OrderBy(player => player.Role)
                .ThenBy(player => player.Nickname)
                .Select(player => player.Id)
                .ToList()
        };

        PopulateSelectedPlayers(model);
        return model;
    }

    private void NormalizeSelectedPlayers(TeamCreateModel model)
    {
        model.SelectedPlayerIds = model.SelectedPlayerIds
            .Where(playerId => playerId > 0)
            .Distinct()
            .ToList();

        if (model.SelectedPlayerIds.Count > 5)
        {
            ModelState.AddModelError(nameof(model.SelectedPlayerIds), "You can select at most 5 players.");
        }

        var selectedPlayers = _playerRepository.GetByIds(model.SelectedPlayerIds);
        if (selectedPlayers.Count != model.SelectedPlayerIds.Count)
        {
            ModelState.AddModelError(nameof(model.SelectedPlayerIds), "One or more selected players could not be found.");
        }

        var currentTeamId = model is TeamEditModel editModel ? editModel.Id : (int?)null;
        var allowedPlayers = selectedPlayers
            .Where(player => player.TeamId is null || player.TeamId == currentTeamId)
            .ToList();

        if (allowedPlayers.Count != selectedPlayers.Count)
        {
            ModelState.AddModelError(nameof(model.SelectedPlayerIds), "Selected players must be free agents or already belong to this team.");
        }

        var playersById = allowedPlayers.ToDictionary(player => player.Id);
        model.SelectedPlayers = model.SelectedPlayerIds
            .Where(playersById.ContainsKey)
            .Select(playerId => new PlayerAutocompleteItemModel
            {
                Id = playerId,
                Text = playersById[playerId].Nickname
            })
            .ToList();
    }

    private void PopulateSelectedPlayers(TeamCreateModel model)
    {
        if (model.SelectedPlayerIds.Count == 0)
        {
            model.SelectedPlayers = [];
            return;
        }

        var players = _playerRepository.GetByIds(model.SelectedPlayerIds);
        var currentTeamId = model is TeamEditModel editModel ? editModel.Id : (int?)null;
        var allowedPlayers = players
            .Where(player => player.TeamId is null || player.TeamId == currentTeamId)
            .ToList();
        var playersById = allowedPlayers.ToDictionary(player => player.Id);

        model.SelectedPlayers = model.SelectedPlayerIds
            .Where(playersById.ContainsKey)
            .Select(playerId => new PlayerAutocompleteItemModel
            {
                Id = playerId,
                Text = playersById[playerId].Nickname
            })
            .ToList();
    }
}