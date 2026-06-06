using cs2_esports.Dtos.Events;
using cs2_esports.Dtos.Matches;
using cs2_esports.Dtos.Teams;
using cs2_esports.Helpers;
using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cs2_esports.Controllers.Api;

[Route("api/matches")]
[ApiController]
public class MatchesController : ControllerBase
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

    [HttpGet]
    public ActionResult<IEnumerable<MatchSummaryDto>> GetAll([FromQuery] string? query = null)
    {
        var matches = _matchRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalizedQuery = query.Trim();
            if (normalizedQuery.Length < 2)
            {
                return Ok(Array.Empty<MatchSummaryDto>());
            }

            matches = matches.Where(match =>
                match.Event?.Name.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) == true ||
                match.TeamA?.Name.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) == true ||
                match.TeamA?.Tag.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) == true ||
                match.TeamB?.Name.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) == true ||
                match.TeamB?.Tag.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }

        return Ok(matches.Select(MapToSummaryDto));
    }

    [HttpGet("{id:int}")]
    public ActionResult<MatchDetailsDto> GetById(int id)
    {
        var match = _matchRepository.GetById(id);
        if (match is null)
        {
            return NotFound();
        }

        return Ok(MapToDetailsDto(match));
    }

    [HttpPost]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public ActionResult<MatchDetailsDto> Create([FromBody] MatchUpsertDto model)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return Forbid();
        }

        ValidateMatch(model);
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var match = MapToMatch(model);
        _matchRepository.Add(match);
        return CreatedAtAction(nameof(GetById), new { id = match.Id }, MapToDetailsDto(match));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public ActionResult<MatchDetailsDto> Update(int id, [FromBody] MatchUpsertDto model)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return Forbid();
        }

        var existingMatch = _matchRepository.GetById(id);
        if (existingMatch is null)
        {
            return NotFound();
        }

        ValidateMatch(model, id);
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var match = MapToMatch(model);
        match.Id = id;
        _matchRepository.Update(match);
        var updatedMatch = _matchRepository.GetById(id);
        return Ok(MapToDetailsDto(updatedMatch ?? match));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = EventRoleHelper.SuperAdminOnlyRoles)]
    public IActionResult Delete(int id)
    {
        if (!EventRoleHelper.CanManageRosterContent(User))
        {
            return Forbid();
        }

        var match = _matchRepository.GetById(id);
        if (match is null)
        {
            return NotFound();
        }

        if (match.Maps.Any())
        {
            return Conflict(new { message = "This match cannot be deleted because it already has maps attached." });
        }

        _matchRepository.Delete(id);
        return NoContent();
    }

    private void ValidateMatch(MatchUpsertDto model, int? currentMatchId = null)
    {
        if (model.TeamAId == model.TeamBId)
        {
            ModelState.AddModelError(nameof(model.TeamBId), "Team A and Team B must be different teams.");
        }

        if (_eventRepository.GetById(model.EventId) is null)
        {
            ModelState.AddModelError(nameof(model.EventId), "The selected event could not be found.");
        }

        if (_teamRepository.GetById(model.TeamAId) is null)
        {
            ModelState.AddModelError(nameof(model.TeamAId), "The selected Team A could not be found.");
        }

        if (_teamRepository.GetById(model.TeamBId) is null)
        {
            ModelState.AddModelError(nameof(model.TeamBId), "The selected Team B could not be found.");
        }

        var mapRows = NormalizeMaps(model.Maps);
        var minimumMapCount = GetMinimumMapCount(model.Format);
        var requiredWins = GetRequiredWins(model.Format);

        if (model.IsFinished && !model.FinishedAtUtc.HasValue)
        {
            ModelState.AddModelError(nameof(model.FinishedAtUtc), "Finished date is required when the match is marked finished.");
        }

        if (mapRows.Count < minimumMapCount)
        {
            ModelState.AddModelError(nameof(model.Maps), $"At least {minimumMapCount} map result(s) are required for {model.Format}.");
        }

        var seriesScoreA = mapRows.Count(map => map.TeamAScore > map.TeamBScore);
        var seriesScoreB = mapRows.Count(map => map.TeamBScore > map.TeamAScore);

        if (model.FinishedAtUtc.HasValue)
        {
            if (seriesScoreA == seriesScoreB)
            {
                ModelState.AddModelError(nameof(model.Maps), "A finished match must have a winner.");
            }

            if (seriesScoreA > requiredWins || seriesScoreB > requiredWins)
            {
                ModelState.AddModelError(nameof(model.Maps), $"A {model.Format} match cannot end with more than {requiredWins} map wins for either team.");
            }

            if (seriesScoreA != requiredWins && seriesScoreB != requiredWins)
            {
                ModelState.AddModelError(nameof(model.Maps), $"A finished {model.Format} match must end when one team reaches {requiredWins} map wins.");
            }

            if (model.FinishedAtUtc.Value < model.ScheduledAtUtc)
            {
                ModelState.AddModelError(nameof(model.FinishedAtUtc), "Finished date must be on or after the scheduled date.");
            }
        }

        if (mapRows.GroupBy(map => map.MapSequence).Any(group => group.Count() > 1))
        {
            ModelState.AddModelError(nameof(model.Maps), "Map sequences must be unique.");
        }

        if (model.Maps.Any(map => map.MapSequence < 1))
        {
            ModelState.AddModelError(nameof(model.Maps), "Map sequence numbers must start at 1.");
        }
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

    private static List<MatchMap> NormalizeMaps(IEnumerable<MatchMapDto> maps)
    {
        return maps
            .Where(map => map.MapSequence > 0)
            .Where(map => map.Map != 0 || map.TeamAScore != 0 || map.TeamBScore != 0 || map.WentToOvertime)
            .Select(map => new MatchMap
            {
                MapSequence = map.MapSequence,
                Map = map.Map,
                TeamAScore = map.TeamAScore,
                TeamBScore = map.TeamBScore,
                WentToOvertime = map.WentToOvertime
            })
            .OrderBy(map => map.MapSequence)
            .ToList();
    }

    private Match MapToMatch(MatchUpsertDto model)
    {
        var maps = NormalizeMaps(model.Maps);
        var seriesScoreA = maps.Count(map => map.TeamAScore > map.TeamBScore);
        var seriesScoreB = maps.Count(map => map.TeamBScore > map.TeamAScore);

        return new Match
        {
            ScheduledAtUtc = model.ScheduledAtUtc,
            IsFinished = model.FinishedAtUtc.HasValue,
            Format = model.Format,
            TeamAScore = seriesScoreA,
            TeamBScore = seriesScoreB,
            FinishedAtUtc = model.FinishedAtUtc,
            EventId = model.EventId,
            TeamAId = model.TeamAId,
            TeamBId = model.TeamBId,
            Maps = maps
        };
    }

    private MatchSummaryDto MapToSummaryDto(Match match)
    {
        return new MatchSummaryDto
        {
            Id = match.Id,
            ScheduledAtUtc = match.ScheduledAtUtc,
            IsFinished = match.IsFinished,
            Format = match.Format,
            TeamAScore = match.TeamAScore,
            TeamBScore = match.TeamBScore,
            FinishedAtUtc = match.FinishedAtUtc,
            EventId = match.EventId,
            EventName = match.Event?.Name ?? string.Empty,
            TeamAId = match.TeamAId,
            TeamAName = match.TeamA?.Name ?? string.Empty,
            TeamATag = match.TeamA?.Tag ?? string.Empty,
            TeamBId = match.TeamBId,
            TeamBName = match.TeamB?.Name ?? string.Empty,
            TeamBTag = match.TeamB?.Tag ?? string.Empty,
            MapCount = match.Maps.Count
        };
    }

    private MatchDetailsDto MapToDetailsDto(Match match)
    {
        var eventItem = match.Event;
        var teamA = match.TeamA;
        var teamB = match.TeamB;

        return new MatchDetailsDto
        {
            Id = match.Id,
            ScheduledAtUtc = match.ScheduledAtUtc,
            IsFinished = match.IsFinished,
            Format = match.Format,
            TeamAScore = match.TeamAScore,
            TeamBScore = match.TeamBScore,
            FinishedAtUtc = match.FinishedAtUtc,
            EventId = match.EventId,
            EventName = eventItem?.Name ?? string.Empty,
            TeamAId = match.TeamAId,
            TeamAName = teamA?.Name ?? string.Empty,
            TeamATag = teamA?.Tag ?? string.Empty,
            TeamBId = match.TeamBId,
            TeamBName = teamB?.Name ?? string.Empty,
            TeamBTag = teamB?.Tag ?? string.Empty,
            MapCount = match.Maps.Count,
            Event = eventItem is null ? null : new EventSummaryDto
            {
                Id = eventItem.Id,
                Name = eventItem.Name,
                Organizer = eventItem.Organizer,
                Tier = eventItem.Tier,
                PrizePoolUsd = eventItem.PrizePoolUsd,
                StartDateUtc = eventItem.StartDateUtc,
                EndDateUtc = eventItem.EndDateUtc,
                IsLan = eventItem.IsLan,
                BannerImagePath = eventItem.BannerImagePath,
                EventVenueId = eventItem.EventVenueId,
                VenueName = eventItem.EventVenue?.Name ?? string.Empty,
                VenueCity = eventItem.EventVenue?.City ?? string.Empty,
                VenueCountryCode = eventItem.EventVenue?.CountryCode ?? string.Empty,
                TeamCount = eventItem.Teams.Count,
                MatchCount = eventItem.Matches.Count
            },
            TeamA = teamA is null ? null : new TeamListItemDto
            {
                Id = teamA.Id,
                Name = teamA.Name,
                Tag = teamA.Tag,
                CountryCode = teamA.CountryCode,
                WorldRanking = teamA.WorldRanking,
                FoundedYear = teamA.FoundedYear,
                PrizeMoneyUsd = teamA.PrizeMoneyUsd,
                LastRosterUpdateUtc = teamA.LastRosterUpdateUtc,
                PlayerCount = teamA.Players.Count
            },
            TeamB = teamB is null ? null : new TeamListItemDto
            {
                Id = teamB.Id,
                Name = teamB.Name,
                Tag = teamB.Tag,
                CountryCode = teamB.CountryCode,
                WorldRanking = teamB.WorldRanking,
                FoundedYear = teamB.FoundedYear,
                PrizeMoneyUsd = teamB.PrizeMoneyUsd,
                LastRosterUpdateUtc = teamB.LastRosterUpdateUtc,
                PlayerCount = teamB.Players.Count
            },
            Maps = match.Maps
                .OrderBy(map => map.MapSequence)
                .Select(map => new MatchMapDto
                {
                    Id = map.Id,
                    MapSequence = map.MapSequence,
                    Map = map.Map,
                    TeamAScore = map.TeamAScore,
                    TeamBScore = map.TeamBScore,
                    WentToOvertime = map.WentToOvertime
                })
                .ToList()
        };
    }
}
