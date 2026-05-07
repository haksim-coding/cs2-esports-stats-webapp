using cs2_esports.Models;
using cs2_esports.Helpers;
using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cs2_esports.Controllers;

public class TeamsController : Controller
{
    private readonly ITeamRepository _teamRepository;
    private readonly IForumRepository _forumRepository;
    private const string ForumUserSessionKey = "ForumUserId";

    public TeamsController(ITeamRepository teamRepository, IForumRepository forumRepository)
    {
        _teamRepository = teamRepository;
        _forumRepository = forumRepository;
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
}