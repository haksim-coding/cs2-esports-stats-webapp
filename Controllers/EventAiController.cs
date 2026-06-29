using System.ComponentModel.DataAnnotations;
using cs2_esports.Data;
using cs2_esports.Dtos.Events;
using cs2_esports.Helpers;
using cs2_esports.Models;
using cs2_esports.Services.Ai;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cs2_esports.Controllers;

[Route("events")]
[Authorize(Roles = EventRoleHelper.EventAdminRoles)]
public sealed class EventAiController : Controller
{
    private readonly IAiEventDraftProvider _draftProvider;
    private readonly Cs2ScopeDbContext _dbContext;
    private readonly ILogger<EventAiController> _logger;

    public EventAiController(
        IAiEventDraftProvider draftProvider,
        Cs2ScopeDbContext dbContext,
        ILogger<EventAiController> logger)
    {
        _draftProvider = draftProvider;
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpPost("ai-draft")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDraft(
        [FromForm] AiEventDraftRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var requiredOrganizer = EventRoleHelper.GetDefaultOrganizerForAdmin(User);
        var venues = await _dbContext.EventVenues
            .AsNoTracking()
            .OrderBy(venue => venue.Name)
            .Select(venue => new AiEventVenueOption(venue.Id, venue.Name, venue.City, venue.CountryCode))
            .ToListAsync(cancellationToken);
        var teams = await _dbContext.Teams
            .AsNoTracking()
            .OrderBy(team => team.WorldRanking <= 0 ? int.MaxValue : team.WorldRanking)
            .ThenBy(team => team.Name)
            .Take(100)
            .Select(team => new AiEventTeamOption(
                team.Id,
                team.Name,
                team.Tag,
                team.WorldRanking,
                team.CountryCode))
            .ToListAsync(cancellationToken);

        AiEventDraft draft;
        try
        {
            draft = await _draftProvider.CreateDraftAsync(
                request.Prompt.Trim(),
                new AiEventDraftContext(DateTime.UtcNow.Date, NullIfEmpty(requiredOrganizer), venues, teams),
                cancellationToken);
        }
        catch (AiProviderException exception)
        {
            _logger.LogWarning(exception, "AI event drafting failed.");
            return Problem(
                title: "AI drafting is unavailable",
                detail: exception.Message,
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        Normalize(draft);
        var errors = ValidateDraft(draft, requiredOrganizer, venues, teams);
        if (errors.Count > 0)
        {
            return BadRequest(new
            {
                message = "The AI response did not produce a valid event draft.",
                errors
            });
        }

        if (!string.IsNullOrWhiteSpace(requiredOrganizer) && string.IsNullOrWhiteSpace(draft.Organizer))
        {
            draft.Organizer = requiredOrganizer;
        }

        var teamsById = teams.ToDictionary(team => team.Id);
        var selectedTeams = (draft.SelectedTeamIds ?? [])
            .Where(teamsById.ContainsKey)
            .Select(teamId => teamsById[teamId])
            .Select(team => new
            {
                id = team.Id,
                text = team.Name,
                worldRanking = team.WorldRanking,
                logoPath = TeamLogoResolver.GetLogoPath(team.Name, team.Tag),
                badgeText = TeamLogoResolver.GetBadgeText(team.Name, team.Tag)
            });

        return Ok(new { draft, selectedTeams });
    }

    private List<string> ValidateDraft(
        AiEventDraft draft,
        string requiredOrganizer,
        IReadOnlyCollection<AiEventVenueOption> venues,
        IReadOnlyCollection<AiEventTeamOption> teams)
    {
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(draft, new ValidationContext(draft), validationResults, true);
        var errors = validationResults
            .Select(result => result.ErrorMessage ?? "The draft contains an invalid value.")
            .ToList();

        if (IsEmpty(draft))
        {
            errors.Add("The prompt did not contain recognizable event details.");
        }

        if (draft.StartDateUtc.HasValue && draft.EndDateUtc.HasValue &&
            draft.EndDateUtc.Value < draft.StartDateUtc.Value)
        {
            errors.Add("End date must be on or after the start date.");
        }

        if (draft.Tier.HasValue && !Enum.IsDefined(draft.Tier.Value))
        {
            errors.Add("The event tier is invalid.");
        }

        if (draft.EventVenueId.HasValue && venues.All(venue => venue.Id != draft.EventVenueId.Value))
        {
            errors.Add("The selected venue does not exist.");
        }

        if (draft.SelectedTeamIds is { Count: > 0 })
        {
            var knownTeamIds = teams.Select(team => team.Id).ToHashSet();
            if (draft.SelectedTeamIds.Any(teamId => !knownTeamIds.Contains(teamId)))
            {
                errors.Add("One or more selected teams do not exist.");
            }
        }

        if (!string.IsNullOrWhiteSpace(draft.Organizer) &&
            (!EventRoleHelper.CanManageOrganizer(User, draft.Organizer) ||
             (!string.IsNullOrWhiteSpace(requiredOrganizer) &&
              !draft.Organizer.Equals(requiredOrganizer, StringComparison.OrdinalIgnoreCase))))
        {
            errors.Add("You are not allowed to create events for this organizer.");
        }

        if (!string.IsNullOrWhiteSpace(draft.Name) &&
            _dbContext.Tournaments
                .AsNoTracking()
                .Select(eventItem => eventItem.Name)
                .AsEnumerable()
                .Any(name => name.Equals(draft.Name, StringComparison.OrdinalIgnoreCase)))
        {
            errors.Add("An event with this name already exists.");
        }

        return errors;
    }

    private static void Normalize(AiEventDraft draft)
    {
        draft.Name = NullIfEmpty(draft.Name);
        draft.Organizer = NullIfEmpty(draft.Organizer);
        draft.StartDateUtc = ToUtc(draft.StartDateUtc);
        draft.EndDateUtc = ToUtc(draft.EndDateUtc);
        draft.SelectedTeamIds = draft.SelectedTeamIds?
            .Where(teamId => teamId > 0)
            .Distinct()
            .ToList();
    }

    private static string? NullIfEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static DateTime? ToUtc(DateTime? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        return value.Value.Kind switch
        {
            DateTimeKind.Utc => value.Value,
            DateTimeKind.Local => value.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
        };
    }

    private static bool IsEmpty(AiEventDraft draft) =>
        draft.Name is null &&
        draft.Organizer is null &&
        draft.Tier is null &&
        draft.PrizePoolUsd is null &&
        draft.StartDateUtc is null &&
        draft.EndDateUtc is null &&
        draft.IsLan is null &&
        draft.EventVenueId is null &&
        (draft.SelectedTeamIds is null || draft.SelectedTeamIds.Count == 0);
}
