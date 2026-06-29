using cs2_esports.Data;
using cs2_esports.Helpers;
using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace cs2_esports.Controllers;

public class MatchesController : Controller
{
    private readonly IMatchRepository _matchRepository;
    private readonly IEventRepository _eventRepository;

    public MatchesController(IMatchRepository matchRepository, IEventRepository eventRepository)
    {
        _matchRepository = matchRepository;
        _eventRepository = eventRepository;
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
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult EventTeams(int eventId)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return NotFound();
        }

        var eventItem = _eventRepository.GetById(eventId);
        if (eventItem is null)
        {
            return NotFound();
        }

        return Json(eventItem.Teams
            .OrderBy(team => team.WorldRanking <= 0 ? int.MaxValue : team.WorldRanking)
            .ThenBy(team => team.Name)
            .Select(team => new { team.Id, team.Name }));
    }

    [HttpGet]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult Create()
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
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
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult Create(MatchCreateModel model)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
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
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult Edit(int id)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
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
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult Edit(MatchEditModel model)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
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
        match.Format = model.Format;
        match.TeamAScore = model.TeamAScore;
        match.TeamBScore = model.TeamBScore;
        match.IsFinished = IsCompletedMatch(model);
        match.FinishedAtUtc = match.IsFinished ? model.FinishedAtUtc : null;
        match.EventId = model.EventId;
        match.TeamAId = model.TeamAId;
        match.TeamBId = model.TeamBId;
        match.Maps = MapMaps(model);

        _matchRepository.Update(match);
        return RedirectToAction(nameof(Details), new { id = match.Id });
    }

    [HttpGet]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult Delete(int id)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
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
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
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
        var events = _eventRepository.GetAll().OrderBy(eventItem => eventItem.StartDateUtc).ToList();
        var selectedEventTeams = events
            .FirstOrDefault(eventItem => eventItem.Id == model.EventId)?
            .Teams
            .OrderBy(team => team.WorldRanking <= 0 ? int.MaxValue : team.WorldRanking)
            .ThenBy(team => team.Name)
            .ToList() ?? [];

        ViewBag.Events = new SelectList(events, nameof(Event.Id), nameof(Event.Name), model.EventId);
        ViewBag.TeamOptions = new SelectList(selectedEventTeams, nameof(Team.Id), nameof(Team.Name));
    }

    private void PrepareMapRows(MatchCreateModel model)
    {
        var mapCount = GetMapCount(model.Format);
        var rows = Enumerable.Range(1, 5)
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

    private static int GetRequiredWins(MatchFormat format)
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

    private bool IsCompletedMatch(MatchCreateModel model)
    {
        UpdateSeriesScores(model);

        var requiredWins = GetRequiredWins(model.Format);
        var seriesScoreA = model.TeamAScore;
        var seriesScoreB = model.TeamBScore;
        var hasWinner = seriesScoreA == requiredWins || seriesScoreB == requiredWins;
        var hasValidMargin = seriesScoreA != seriesScoreB && seriesScoreA <= requiredWins && seriesScoreB <= requiredWins;

        return model.FinishedAtUtc.HasValue && hasWinner && hasValidMargin;
    }

    private void ValidateMatch(MatchCreateModel model)
    {
        var mapRows = model.Maps.Where(IsMapRowFilled).ToList();
        var minimumMapCount = GetMinimumMapCount(model.Format);
        var requiredWins = GetRequiredWins(model.Format);

        UpdateSeriesScores(model);

        var selectedEvent = _eventRepository.GetById(model.EventId);
        if (selectedEvent is null)
        {
            ModelState.AddModelError(nameof(model.EventId), "The selected event could not be found.");
        }
        else
        {
            var eventTeamIds = selectedEvent.Teams.Select(team => team.Id).ToHashSet();
            if (!eventTeamIds.Contains(model.TeamAId))
            {
                ModelState.AddModelError(nameof(model.TeamAId), "Team A must be a team participating in the selected event.");
            }

            if (!eventTeamIds.Contains(model.TeamBId))
            {
                ModelState.AddModelError(nameof(model.TeamBId), "Team B must be a team participating in the selected event.");
            }
        }

        if (model.TeamAId == model.TeamBId)
        {
            ModelState.AddModelError(nameof(model.TeamBId), "Team A and Team B must be different teams.");
        }

        if (model.IsFinished && !model.FinishedAtUtc.HasValue)
        {
            ModelState.AddModelError(nameof(model.FinishedAtUtc), "Finished date is required when the match is marked finished.");
        }

        if (model.FinishedAtUtc.HasValue)
        {
            var seriesScoreA = model.TeamAScore;
            var seriesScoreB = model.TeamBScore;

            if (mapRows.Count < minimumMapCount)
            {
                ModelState.AddModelError(nameof(model.Maps), $"At least {minimumMapCount} map result(s) are required for a finished {MatchDisplayHelper.GetFormatLabel(model.Format)} match.");
            }

            if (model.FinishedAtUtc.Value < model.ScheduledAtUtc)
            {
                ModelState.AddModelError(nameof(model.FinishedAtUtc), "Finished date and time must be on or after the match start date and time.");
            }

            if (seriesScoreA == seriesScoreB)
            {
                ModelState.AddModelError(nameof(model.Maps), "A finished match must have a winner.");
            }

            if (seriesScoreA > requiredWins || seriesScoreB > requiredWins)
            {
                ModelState.AddModelError(nameof(model.Maps), $"A {MatchDisplayHelper.GetFormatLabel(model.Format)} match cannot end with more than {requiredWins} map wins for either team.");
            }

            if (seriesScoreA != requiredWins && seriesScoreB != requiredWins)
            {
                ModelState.AddModelError(nameof(model.Maps), $"A finished {MatchDisplayHelper.GetFormatLabel(model.Format)} match must end when one team reaches {requiredWins} map wins.");
            }
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
            else
            {
                ValidateMapScore(model, mapRow);
            }

            if (!usedMapSequences.Add(mapRow.MapSequence))
            {
                ModelState.AddModelError(nameof(model.Maps), $"Map {mapRow.MapSequence} is duplicated.");
            }
        }
    }

    private void ValidateMapScore(MatchCreateModel model, MatchMapInputModel mapRow)
    {
        var scoreA = mapRow.TeamAScore.GetValueOrDefault();
        var scoreB = mapRow.TeamBScore.GetValueOrDefault();
        var mapIndex = model.Maps.IndexOf(mapRow);
        var scoreKey = mapIndex >= 0
            ? $"{nameof(model.Maps)}[{mapIndex}].{nameof(mapRow.TeamBScore)}"
            : nameof(model.Maps);

        if (scoreA == scoreB)
        {
            ModelState.AddModelError(scoreKey, $"Map {mapRow.MapSequence} cannot end in a tie.");
            return;
        }

        var winningScore = Math.Max(scoreA, scoreB);
        var losingScore = Math.Min(scoreA, scoreB);

        if (!mapRow.WentToOvertime && (winningScore != 13 || losingScore >= 12))
        {
            ModelState.AddModelError(scoreKey, $"Map {mapRow.MapSequence} has an invalid regulation score. The winner must have 13 rounds and the loser no more than 11.");
            return;
        }

        if (mapRow.WentToOvertime && (winningScore < 16 || losingScore < 12 || winningScore - losingScore < 2))
        {
            ModelState.AddModelError(scoreKey, $"Map {mapRow.MapSequence} has an invalid overtime score. The winner needs at least 16 rounds and a margin of at least 2 rounds.");
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
            IsFinished = model.FinishedAtUtc.HasValue,
            Format = model.Format,
            TeamAScore = model.TeamAScore,
            TeamBScore = model.TeamBScore,
            FinishedAtUtc = model.FinishedAtUtc,
            EventId = model.EventId,
            TeamAId = model.TeamAId,
            TeamBId = model.TeamBId,
            Maps = MapMaps(model)
        };
    }

    private static MatchEditModel MapToEditModel(Match match)
    {
        var maxMapCount = GetMapCount(match.Format);
        var maps = Enumerable.Range(1, 5)
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

}
