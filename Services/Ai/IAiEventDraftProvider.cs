using cs2_esports.Dtos.Events;

namespace cs2_esports.Services.Ai;

public interface IAiEventDraftProvider
{
    Task<AiEventDraft> CreateDraftAsync(
        string prompt,
        AiEventDraftContext context,
        CancellationToken cancellationToken = default);
}

public sealed record AiEventDraftContext(
    DateTime TodayUtc,
    string? RequiredOrganizer,
    IReadOnlyList<AiEventVenueOption> Venues,
    IReadOnlyList<AiEventTeamOption>? Teams = null);

public sealed record AiEventVenueOption(int Id, string Name, string City, string CountryCode);

public sealed record AiEventTeamOption(
    int Id,
    string Name,
    string Tag,
    int WorldRanking,
    string CountryCode);
