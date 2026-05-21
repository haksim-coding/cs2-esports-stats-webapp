using cs2_esports.Data;
using cs2_esports.Helpers;
using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace cs2_esports.Controllers;

public class EventsController : Controller
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

    public IActionResult Index()
    {
        var eventsData = _eventRepository.GetAll();
        return View(eventsData);
    }

    public IActionResult Details(int id)
    {
        var eventItem = _eventRepository.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        return View(eventItem);
    }

    [HttpGet("/event/{slug}")]
    public IActionResult DetailsBySlug(string slug)
    {
        var eventSummary = _eventRepository.GetAll().FirstOrDefault(tournament =>
            RouteSlugHelper.MatchesRouteSegment(tournament.Name, slug));

        if (eventSummary is null)
        {
            return NotFound();
        }

        var eventItem = _eventRepository.GetById(eventSummary.Id);
        if (eventItem is null)
        {
            return NotFound();
        }

        return View("Details", eventItem);
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        var model = new EventCreateModel
        {
            StartDateUtc = DateTime.UtcNow,
            EndDateUtc = DateTime.UtcNow.AddDays(3)
        };

        PopulateViewData(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(EventCreateModel model)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        NormalizeSelectedTeams(model);
        ValidateEventDates(model);
        ValidateEventUniqueness(model);

        if (!ModelState.IsValid)
        {
            PopulateViewData(model);
            return View(model);
        }

        var eventItem = new Event
        {
            Name = model.Name.Trim(),
            Organizer = model.Organizer.Trim(),
            Tier = model.Tier,
            PrizePoolUsd = model.PrizePoolUsd,
            StartDateUtc = model.StartDateUtc,
            EndDateUtc = model.EndDateUtc,
            IsLan = model.IsLan,
            EventVenueId = model.EventVenueId,
            AdminUserId = GetCurrentAdminUserId(),
            Teams = _teamRepository.GetByIds(model.SelectedTeamIds).ToList()
        };

        _eventRepository.Add(eventItem);
        return RedirectToAction(nameof(DetailsBySlug), new { slug = RouteSlugHelper.ToRouteSegment(eventItem.Name) });
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        var eventItem = _eventRepository.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        var model = MapToEditModel(eventItem);
        PopulateViewData(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(EventEditModel model)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        var eventItem = _eventRepository.GetById(model.Id);
        if (eventItem is null)
        {
            return NotFound();
        }

        NormalizeSelectedTeams(model);
        ValidateEventDates(model);
        ValidateEventUniqueness(model, model.Id);

        if (!ModelState.IsValid)
        {
            PopulateViewData(model);
            return View(model);
        }

        eventItem.Name = model.Name.Trim();
        eventItem.Organizer = model.Organizer.Trim();
        eventItem.Tier = model.Tier;
        eventItem.PrizePoolUsd = model.PrizePoolUsd;
        eventItem.StartDateUtc = model.StartDateUtc;
        eventItem.EndDateUtc = model.EndDateUtc;
        eventItem.IsLan = model.IsLan;
        eventItem.EventVenueId = model.EventVenueId;
        eventItem.AdminUserId = GetCurrentAdminUserId();
        eventItem.Teams = _teamRepository.GetByIds(model.SelectedTeamIds).ToList();

        _eventRepository.Update(eventItem);
        return RedirectToAction(nameof(DetailsBySlug), new { slug = RouteSlugHelper.ToRouteSegment(eventItem.Name) });
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        if (!IsAdminUser())
        {
            return NotFound();
        }

        var eventItem = _eventRepository.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        return View(eventItem);
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

        var eventItem = _eventRepository.GetById(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        if ((eventItem.Teams?.Any() ?? false) || (eventItem.Matches?.Any() ?? false) || (eventItem.ForumThreads?.Any() ?? false))
        {
            ModelState.AddModelError(string.Empty, "This event cannot be deleted because it still has teams, matches, or forum threads attached.");
            return View("Delete", eventItem);
        }

        _eventRepository.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/Event/Search")]
    public IActionResult Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
        {
            return Json(Array.Empty<object>());
        }

        var eventsData = _eventRepository.GetAll()
            .Where(eventItem => eventItem.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(eventItem => eventItem.StartDateUtc)
            .Take(8)
            .Select(eventItem => new { id = eventItem.Id, text = eventItem.Name });

        return Json(eventsData);
    }

    private void PopulateViewData(EventCreateModel model)
    {
        ViewBag.EventVenues = new SelectList(
            _dbContext.EventVenues.OrderBy(venue => venue.Name).ToList(),
            nameof(EventVenue.Id),
            nameof(EventVenue.Name),
            model.EventVenueId);
    }

    private void ValidateEventUniqueness(EventCreateModel model, int? currentEventId = null)
    {
        var normalizedName = model.Name.Trim();
        var eventsData = _eventRepository.GetAll();

        if (eventsData.Any(eventItem => eventItem.Id != currentEventId && eventItem.Name.Equals(normalizedName, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(nameof(model.Name), "An event with this name already exists.");
        }
    }

    private void ValidateEventDates(EventCreateModel model)
    {
        if (model.EndDateUtc < model.StartDateUtc)
        {
            ModelState.AddModelError(nameof(model.EndDateUtc), "End date must be on or after the start date.");
        }
    }

    private void NormalizeSelectedTeams(EventCreateModel model)
    {
        model.SelectedTeamIds = model.SelectedTeamIds
            .Where(teamId => teamId > 0)
            .Distinct()
            .ToList();

        var selectedTeams = _teamRepository.GetByIds(model.SelectedTeamIds);
        if (selectedTeams.Count != model.SelectedTeamIds.Count)
        {
            ModelState.AddModelError(nameof(model.SelectedTeamIds), "One or more selected teams could not be found.");
        }

        model.SelectedTeams = selectedTeams
            .Select(team => new TeamAutocompleteItemModel
            {
                Id = team.Id,
                Text = team.Name,
                    LogoPath = TeamLogoResolver.GetLogoPath(team.Name, team.Tag) ?? string.Empty
            })
            .ToList();
    }

    private EventEditModel MapToEditModel(Event eventItem)
    {
        return new EventEditModel
        {
            Id = eventItem.Id,
            Name = eventItem.Name,
            Organizer = eventItem.Organizer,
            Tier = eventItem.Tier,
            PrizePoolUsd = eventItem.PrizePoolUsd,
            StartDateUtc = eventItem.StartDateUtc,
            EndDateUtc = eventItem.EndDateUtc,
            IsLan = eventItem.IsLan,
            EventVenueId = eventItem.EventVenueId,
            SelectedTeamIds = eventItem.Teams.OrderBy(team => team.WorldRanking).Select(team => team.Id).ToList(),
            SelectedTeams = eventItem.Teams.OrderBy(team => team.WorldRanking).Select(team => new TeamAutocompleteItemModel
            {
                Id = team.Id,
                Text = team.Name,
                    LogoPath = TeamLogoResolver.GetLogoPath(team.Name, team.Tag) ?? string.Empty
            }).ToList()
        };
    }

    private bool IsAdminUser()
    {
        return string.Equals(HttpContext.Session.GetString(AuthSessionKeys.UserType), AuthSessionKeys.AdminUserType, StringComparison.Ordinal)
            && HttpContext.Session.GetInt32(AuthSessionKeys.AdminUserId).HasValue;
    }

    private int? GetCurrentAdminUserId()
    {
        return HttpContext.Session.GetInt32(AuthSessionKeys.AdminUserId);
    }
}