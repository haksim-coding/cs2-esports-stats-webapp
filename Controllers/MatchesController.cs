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
            ScheduledAtUtc = DateTime.UtcNow.AddDays(1),
            Format = MatchFormat.BestOf3
        };

        PopulateLookups(model);
        PrepareMapRows(model);
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
            PrepareMapRows(model);
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
        PrepareMapRows(model);
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
            PrepareMapRows(model);
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
        match.Maps = MapMaps(model);

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

    private void PrepareMapRows(MatchCreateModel model)
    {
        var mapCount = GetMapCount(model.Format);
        var rows = Enumerable.Range(1, mapCount)
            .Select(sequence => new MatchMapInputModel { MapSequence = sequence })
            .ToList();

        var sourceRows = model.Maps ?? [];
        foreach (var map in sourceRows.Where(IsMapRowFilled))
        {
            if (map.MapSequence < 1 || map.MapSequence > mapCount)
            {
                continue;
            }

            rows[map.MapSequence - 1] = map;
        }

        model.Maps = rows;
        UpdateSeriesScores(model);
    }

    private static int GetMapCount(MatchFormat format)
    {
        return format switch
        {
            MatchFormat.BestOf1 => 1,
            MatchFormat.BestOf3 => 3,
            MatchFormat.BestOf5 => 5,
            _ => 3
        };
    }

    private static int GetMinimumMapCount(MatchFormat format)
    {
        return format switch
        {
            MatchFormat.BestOf1 => 1,
            MatchFormat.BestOf3 => 2,
            MatchFormat.BestOf5 => 3,
            _ => 2
        };
    }

    private static bool IsMapRowFilled(MatchMapInputModel map)
    {
        return map.Map.HasValue || map.TeamAScore.HasValue || map.TeamBScore.HasValue || map.WentToOvertime;
    }

    private static void UpdateSeriesScores(MatchCreateModel model)
    {
        model.TeamAScore = model.Maps.Count(map => map.Map.HasValue && map.TeamAScore.HasValue && map.TeamBScore.HasValue && map.TeamAScore > map.TeamBScore);
        model.TeamBScore = model.Maps.Count(map => map.Map.HasValue && map.TeamAScore.HasValue && map.TeamBScore.HasValue && map.TeamBScore > map.TeamAScore);
    }

    private void ValidateMatch(MatchCreateModel model)
    {
        var mapRows = model.Maps.Where(IsMapRowFilled).ToList();
        var minimumMapCount = GetMinimumMapCount(model.Format);

        if (model.TeamAId == model.TeamBId)
        {
            ModelState.AddModelError(nameof(model.TeamBId), "Team A and Team B must be different teams.");
        }

        if (model.IsFinished && !model.FinishedAtUtc.HasValue)
        {
            ModelState.AddModelError(nameof(model.FinishedAtUtc), "Finished date is required when the match is marked finished.");
        }

        if (mapRows.Count < minimumMapCount)
        {
            ModelState.AddModelError(nameof(model.Maps), $"At least {minimumMapCount} map result(s) are required for {MatchDisplayHelper.GetFormatLabel(model.Format)}.");
        }

        var usedMapSequences = new HashSet<int>();
        foreach (var mapRow in mapRows)
        {
            if (!mapRow.Map.HasValue)
            {
                ModelState.AddModelError(nameof(model.Maps), $"Choose a map for map {mapRow.MapSequence}.");
            }

            if (!mapRow.TeamAScore.HasValue || !mapRow.TeamBScore.HasValue)
            {
                ModelState.AddModelError(nameof(model.Maps), $"Enter both scores for map {mapRow.MapSequence}.");
            }

            if (!usedMapSequences.Add(mapRow.MapSequence))
            {
                ModelState.AddModelError(nameof(model.Maps), $"Map {mapRow.MapSequence} is duplicated.");
            }
        }
    }

    private static List<MatchMap> MapMaps(MatchCreateModel model)
    {
        return model.Maps
            .Where(IsMapRowFilled)
            .Where(map => map.Map.HasValue && map.TeamAScore.HasValue && map.TeamBScore.HasValue)
            .Select(map => new MatchMap
            {
                MapSequence = map.MapSequence,
                Map = map.Map.GetValueOrDefault(),
                TeamAScore = map.TeamAScore.GetValueOrDefault(),
                TeamBScore = map.TeamBScore.GetValueOrDefault(),
                WentToOvertime = map.WentToOvertime
            })
            .OrderBy(map => map.MapSequence)
            .ToList();
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
            TeamBId = model.TeamBId,
            Maps = MapMaps(model)
        };
    }

    private static MatchEditModel MapToEditModel(Match match)
    {
        var maxMapCount = GetMapCount(match.Format);
        var maps = Enumerable.Range(1, maxMapCount)
            .Select(sequence => new MatchMapInputModel { MapSequence = sequence })
            .ToList();

        foreach (var matchMap in match.Maps.OrderBy(item => item.MapSequence))
        {
            if (matchMap.MapSequence < 1 || matchMap.MapSequence > maxMapCount)
            {
                continue;
            }

            maps[matchMap.MapSequence - 1] = new MatchMapInputModel
            {
                MapSequence = matchMap.MapSequence,
                Map = matchMap.Map,
                TeamAScore = matchMap.TeamAScore,
                TeamBScore = matchMap.TeamBScore,
                WentToOvertime = matchMap.WentToOvertime
            };
        }

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
            TeamBId = match.TeamBId,
            Maps = maps
        };
    }

    private bool IsAdminUser()
    {
        return string.Equals(HttpContext.Session.GetString(AuthSessionKeys.UserType), AuthSessionKeys.AdminUserType, StringComparison.Ordinal)
            && HttpContext.Session.GetInt32(AuthSessionKeys.AdminUserId).HasValue;
    }
}