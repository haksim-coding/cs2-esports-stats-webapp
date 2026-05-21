using cs2_esports.Data;
using cs2_esports.Helpers;
using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace cs2_esports.Controllers;

public class MatchesController : Controller
{
    private readonly IMatchRepository _matchRepository;
    private readonly IEventRepository _eventRepository;
    private readonly ITeamRepository _teamRepository;

    public MatchesController(IMatchRepository matchRepository, IEventRepository eventRepository, ITeamRepository teamRepository)
    {
        _matchRepository = matchRepository;
        _eventRepository = eventRepository;
        _teamRepository = teamRepository;
    }

    public IActionResult Index()
    {
        var matchesData = _matchRepository.GetAll();
        return View(matchesData);
    }

    public IActionResult Details(int id)
    {
        var match = _matchRepository.GetById(id);
        if (match is null)
        {
            return NotFound();
        }

        return View(match);
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        var model = new MatchCreateModel
        {
            ScheduledAtUtc = DateTime.UtcNow.AddDays(1)
        };

        PopulateLookups(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(MatchCreateModel model)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        ValidateMatch(model);

        if (!ModelState.IsValid)
        {
            PopulateLookups(model);
            return View(model);
        }

        var match = MapToMatch(model);
        _matchRepository.Add(match);
        return RedirectToAction(nameof(Details), new { id = match.Id });
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        var match = _matchRepository.GetById(id);
        if (match is null)
        {
            return NotFound();
        }

        var model = MapToEditModel(match);
        PopulateLookups(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(MatchEditModel model)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        var match = _matchRepository.GetById(model.Id);
        if (match is null)
        {
            return NotFound();
        }

        ValidateMatch(model);

        if (!ModelState.IsValid)
        {
            PopulateLookups(model);
            return View(model);
        }

        match.ScheduledAtUtc = model.ScheduledAtUtc;
        match.IsFinished = model.IsFinished;
        match.Format = model.Format;
        match.TeamAScore = model.TeamAScore;
        match.TeamBScore = model.TeamBScore;
        match.FinishedAtUtc = model.IsFinished ? model.FinishedAtUtc : null;
        match.EventId = model.EventId;
        match.TeamAId = model.TeamAId;
        match.TeamBId = model.TeamBId;

        _matchRepository.Update(match);
        return RedirectToAction(nameof(Details), new { id = match.Id });
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        var match = _matchRepository.GetById(id);
        if (match is null)
        {
            return NotFound();
        }

        return View(match);
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

        var match = _matchRepository.GetById(id);
        if (match is null)
        {
            return NotFound();
        }

        if (match.Maps.Any())
        {
            ModelState.AddModelError(string.Empty, "This match cannot be deleted because it already has maps attached.");
            return View("Delete", match);
        }

        _matchRepository.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    private void PopulateLookups(MatchCreateModel model)
    {
        ViewBag.Events = new SelectList(_eventRepository.GetAll().OrderBy(eventItem => eventItem.StartDateUtc).ToList(), nameof(Event.Id), nameof(Event.Name), model.EventId);
        ViewBag.TeamOptions = new SelectList(_teamRepository.GetAll().OrderBy(team => team.WorldRanking).ToList(), nameof(Team.Id), nameof(Team.Name));
    }

    private static Match MapToMatch(MatchCreateModel model)
    {
        return new Match
        {
            ScheduledAtUtc = model.ScheduledAtUtc,
            IsFinished = model.IsFinished,
            Format = model.Format,
            TeamAScore = model.TeamAScore,
            TeamBScore = model.TeamBScore,
            FinishedAtUtc = model.IsFinished ? model.FinishedAtUtc : null,
            EventId = model.EventId,
            TeamAId = model.TeamAId,
            TeamBId = model.TeamBId
        };
    }

    private static MatchEditModel MapToEditModel(Match match)
    {
        return new MatchEditModel
        {
            Id = match.Id,
            ScheduledAtUtc = match.ScheduledAtUtc,
            IsFinished = match.IsFinished,
            Format = match.Format,
            TeamAScore = match.TeamAScore,
            TeamBScore = match.TeamBScore,
            FinishedAtUtc = match.FinishedAtUtc,
            EventId = match.EventId,
            TeamAId = match.TeamAId,
            TeamBId = match.TeamBId
        };
    }

    private void ValidateMatch(MatchCreateModel model)
    {
        if (model.TeamAId == model.TeamBId)
        {
            ModelState.AddModelError(nameof(model.TeamBId), "Team A and Team B must be different teams.");
        }

        if (model.IsFinished && !model.FinishedAtUtc.HasValue)
        {
            ModelState.AddModelError(nameof(model.FinishedAtUtc), "Finished date is required when the match is marked finished.");
        }
    }

    private bool IsAdminUser()
    {
        return string.Equals(HttpContext.Session.GetString(AuthSessionKeys.UserType), AuthSessionKeys.AdminUserType, StringComparison.Ordinal)
            && HttpContext.Session.GetInt32(AuthSessionKeys.AdminUserId).HasValue;
    }
}