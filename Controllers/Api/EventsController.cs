using cs2_esports.Dtos.Events;
using cs2_esports.Dtos.Matches;
using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using cs2_esports.Dtos.Teams;
using cs2_esports.Data;
using cs2_esports.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace cs2_esports.Controllers.Api;

[Route("api/events")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly IEventRepository _eventRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly Cs2ScopeDbContext _dbContext;

    public EventsController(IEventRepository eventRepository, ITeamRepository teamRepository, Cs2ScopeDbContext dbContext)
    {
        _eventRepository = eventRepository;
        _teamRepository = teamRepository;
        _dbContext = dbContext;
    }

    [HttpGet]
    public ActionResult<IEnumerable<EventSummaryDto>> GetAll([FromQuery] string? query = null)
    {
        var eventsData = _eventRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalizedQuery = query.Trim();
            if (normalizedQuery.Length < 2)
            {
                return Ok(Array.Empty<EventSummaryDto>());
            }

            eventsData = eventsData
                .Where(eventItem =>
                    eventItem.Name.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                    eventItem.Organizer.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return Ok(eventsData.Select(MapToSummaryDto));
    }

    [HttpGet("{id:int}")]
    public ActionResult<EventDetailsDto> GetById(int id)
    {
        var eventItem = _eventRepository.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        return Ok(MapToDetailsDto(eventItem));
    }

    [HttpPost]
    [Authorize(Roles = EventRoleHelper.EventAdminRoles)]
    public ActionResult<EventDetailsDto> Create([FromBody] EventUpsertDto model)
    {
        if (!EventRoleHelper.CanManageOrganizer(User, model.Organizer))
        {
            return Forbid();
        }

        ValidateEvent(model);
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var selectedTeams = _teamRepository.GetByIds(NormalizeIds(model.SelectedTeamIds)).ToList();
        var eventItem = new Event
        {
            Name = model.Name.Trim(),
            Organizer = model.Organizer.Trim(),
            Tier = model.Tier,
            PrizePoolUsd = model.PrizePoolUsd,
            StartDateUtc = model.StartDateUtc,
            EndDateUtc = model.EndDateUtc,
            IsLan = model.IsLan,
            BannerImagePath = NormalizeBannerImagePath(model.BannerImagePath),
            EventVenueId = model.EventVenueId,
            Teams = selectedTeams
        };

        _eventRepository.Add(eventItem);
        return CreatedAtAction(nameof(GetById), new { id = eventItem.Id }, MapToDetailsDto(eventItem));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = EventRoleHelper.EventAdminRoles)]
    public ActionResult<EventDetailsDto> Update(int id, [FromBody] EventUpsertDto model)
    {
        var eventItem = _eventRepository.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        if (!EventRoleHelper.CanManageOrganizer(User, eventItem.Organizer) || !EventRoleHelper.CanManageOrganizer(User, model.Organizer))
        {
            return Forbid();
        }

        ValidateEvent(model, id);
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var selectedTeams = _teamRepository.GetByIds(NormalizeIds(model.SelectedTeamIds)).ToList();
        eventItem.Name = model.Name.Trim();
        eventItem.Organizer = model.Organizer.Trim();
        eventItem.Tier = model.Tier;
        eventItem.PrizePoolUsd = model.PrizePoolUsd;
        eventItem.StartDateUtc = model.StartDateUtc;
        eventItem.EndDateUtc = model.EndDateUtc;
        eventItem.IsLan = model.IsLan;
        eventItem.BannerImagePath = NormalizeBannerImagePath(model.BannerImagePath) ?? eventItem.BannerImagePath;
        eventItem.EventVenueId = model.EventVenueId;
        eventItem.Teams = selectedTeams;

        _eventRepository.Update(eventItem);
        return Ok(MapToDetailsDto(eventItem));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = EventRoleHelper.EventAdminRoles)]
    public IActionResult Delete(int id)
    {
        var eventItem = _eventRepository.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        if (!EventRoleHelper.CanManageOrganizer(User, eventItem.Organizer))
        {
            return Forbid();
        }

        if ((eventItem.Teams?.Any() ?? false) || (eventItem.Matches?.Any() ?? false) || (eventItem.ForumThreads?.Any() ?? false))
        {
            return Conflict(new { message = "This event cannot be deleted because it still has teams, matches, or forum threads attached." });
        }

        _eventRepository.Delete(id);
        return NoContent();
    }

    private void ValidateEvent(EventUpsertDto model, int? currentEventId = null)
    {
        var normalizedName = model.Name.Trim();
        var eventsData = _eventRepository.GetAll();

        if (eventsData.Any(eventItem => eventItem.Id != currentEventId && eventItem.Name.Equals(normalizedName, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(model.Name), "An event with this name already exists.");
        }

        if (model.EndDateUtc < model.StartDateUtc)
        {
            ModelState.AddModelError(nameof(model.EndDateUtc), "End date must be on or after the start date.");
        }

        if (_dbContext.EventVenues.FirstOrDefault(venue => venue.Id == model.EventVenueId) is null)
        {
            ModelState.AddModelError(nameof(model.EventVenueId), "The selected venue could not be found.");
        }

        var normalizedTeamIds = NormalizeIds(model.SelectedTeamIds);
        if (normalizedTeamIds.Length > 0)
        {
            var selectedTeams = _teamRepository.GetByIds(normalizedTeamIds);
            if (selectedTeams.Count != normalizedTeamIds.Length)
            {
                ModelState.AddModelError(nameof(model.SelectedTeamIds), "One or more selected teams could not be found.");
            }
        }
    }

    private static int[] NormalizeIds(IEnumerable<int> ids)
    {
        return ids
            .Where(id => id > 0)
            .Distinct()
            .ToArray();
    }

    private EventSummaryDto MapToSummaryDto(Event eventItem)
    {
        return new EventSummaryDto
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
        };
    }

    private EventDetailsDto MapToDetailsDto(Event eventItem)
    {
        var teams = eventItem.Teams
            .OrderBy(team => team.WorldRanking)
            .Select(MapTeam)
            .ToList();

        var matches = eventItem.Matches
            .OrderBy(match => match.ScheduledAtUtc)
            .Select(MapMatchSummary)
            .ToList();

        return new EventDetailsDto
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
            TeamCount = teams.Count,
            MatchCount = matches.Count,
            Venue = eventItem.EventVenue is null ? null : new EventVenueDto
            {
                Id = eventItem.EventVenue.Id,
                Name = eventItem.EventVenue.Name,
                City = eventItem.EventVenue.City,
                CountryCode = eventItem.EventVenue.CountryCode,
                Capacity = eventItem.EventVenue.Capacity,
                IsIndoor = eventItem.EventVenue.IsIndoor,
                SurfaceType = eventItem.EventVenue.SurfaceType
            },
            Teams = teams,
            Matches = matches
        };
    }

    private static TeamListItemDto MapTeam(Team team)
    {
        return new TeamListItemDto
        {
            Id = team.Id,
            Name = team.Name,
            Tag = team.Tag,
            CountryCode = team.CountryCode,
            WorldRanking = team.WorldRanking,
            FoundedYear = team.FoundedYear,
            PrizeMoneyUsd = team.PrizeMoneyUsd,
            LastRosterUpdateUtc = team.LastRosterUpdateUtc,
            PlayerCount = team.Players.Count
        };
    }

    private static MatchSummaryDto MapMatchSummary(Match match)
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

    private static string? NormalizeBannerImagePath(string? bannerImagePath)
    {
        if (string.IsNullOrWhiteSpace(bannerImagePath))
        {
            return null;
        }

        var trimmedPath = bannerImagePath.Trim();
        return trimmedPath.StartsWith("/images/events/banners/", StringComparison.OrdinalIgnoreCase)
            ? trimmedPath
            : null;
    }
}
