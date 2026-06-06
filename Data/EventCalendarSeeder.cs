using cs2_esports.Models;
using Microsoft.EntityFrameworkCore;

namespace cs2_esports.Data;

public static class EventCalendarSeeder
{
    private static readonly DateTime SeededAtUtc = new(2026, 6, 5, 12, 0, 0, DateTimeKind.Utc);

    public static async Task SeedAsync(Cs2ScopeDbContext dbContext)
    {
        var teams = await EnsureTeamsAsync(dbContext);
        var venues = await EnsureVenuesAsync(dbContext);
        await EnsureEventsAsync(dbContext, teams, venues);
    }

    private static async Task<Dictionary<string, Team>> EnsureTeamsAsync(Cs2ScopeDbContext dbContext)
    {
        var seeds = new[]
        {
            new TeamSeed("Vitality", "VIT", "FR", 1, 2013, 5200000m),
            new TeamSeed("Team Spirit", "SPIRIT", "RU", 2, 2015, 4300000m),
            new TeamSeed("NAVI", "NAVI", "UA", 3, 2009, 7100000m),
            new TeamSeed("MOUZ", "MOUZ", "DE", 4, 2002, 3900000m),
            new TeamSeed("FaZe", "FAZE", "US", 5, 2010, 6800000m),
            new TeamSeed("G2", "G2", "DE", 6, 2013, 6100000m),
            new TeamSeed("Falcons", "FLCN", "SA", 7, 2017, 2100000m),
            new TeamSeed("The MongolZ", "MGLZ", "MN", 8, 2013, 1850000m),
            new TeamSeed("Team Liquid", "TL", "US", 9, 2000, 1760000m),
            new TeamSeed("Aurora", "AUR", "TR", 10, 2020, 1420000m),
            new TeamSeed("HEROIC", "HERO", "DK", 11, 2016, 1390000m),
            new TeamSeed("FURIA", "FUR", "BR", 12, 2017, 1680000m),
            new TeamSeed("Virtus.pro", "VP", "RU", 13, 2003, 1540000m),
            new TeamSeed("Complexity", "COL", "US", 14, 2003, 1180000m),
            new TeamSeed("MIBR", "MIBR", "BR", 15, 2003, 1120000m),
            new TeamSeed("paiN", "PAIN", "BR", 16, 2010, 980000m)
        };

        var existingTeams = await dbContext.Teams.ToListAsync();
        foreach (var seed in seeds)
        {
            var aliases = GetTeamAliases(seed.Name);
            var team = existingTeams.FirstOrDefault(item =>
                aliases.Contains(item.Name, StringComparer.OrdinalIgnoreCase) ||
                item.Tag.Equals(seed.Tag, StringComparison.OrdinalIgnoreCase));

            if (team is null)
            {
                team = new Team { Name = seed.Name };
                dbContext.Teams.Add(team);
                existingTeams.Add(team);
            }

            team.Name = seed.Name;
            team.Tag = seed.Tag;
            team.CountryCode = seed.CountryCode;
            team.WorldRanking = seed.WorldRanking;
            team.FoundedYear = seed.FoundedYear;
            team.PrizeMoneyUsd = seed.PrizeMoneyUsd;
            team.LastRosterUpdateUtc = SeededAtUtc.AddDays(-seed.WorldRanking);
        }

        await dbContext.SaveChangesAsync();
        await MergeDuplicateTeamAsync(dbContext, canonicalName: "Team Spirit", duplicateNames: ["Spirit"]);
        await NormalizeUnseededTeamRankingsAsync(dbContext, seeds.Select(seed => seed.Name).ToHashSet(StringComparer.OrdinalIgnoreCase));

        existingTeams = await dbContext.Teams.ToListAsync();

        return existingTeams
            .GroupBy(team => team.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
    }

    private static string[] GetTeamAliases(string canonicalName)
    {
        return canonicalName switch
        {
            "Team Spirit" => ["Team Spirit", "Spirit"],
            _ => [canonicalName]
        };
    }

    private static async Task MergeDuplicateTeamAsync(Cs2ScopeDbContext dbContext, string canonicalName, string[] duplicateNames)
    {
        var canonicalTeam = await dbContext.Teams.FirstOrDefaultAsync(team => team.Name == canonicalName);
        if (canonicalTeam is null)
        {
            return;
        }

        var duplicateTeams = await dbContext.Teams
            .Where(team => team.Id != canonicalTeam.Id && duplicateNames.Contains(team.Name))
            .ToListAsync();

        foreach (var duplicateTeam in duplicateTeams)
        {
            await dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE [Players] SET [TeamId] = {canonicalTeam.Id} WHERE [TeamId] = {duplicateTeam.Id};
                UPDATE [Matches] SET [TeamAId] = {canonicalTeam.Id} WHERE [TeamAId] = {duplicateTeam.Id};
                UPDATE [Matches] SET [TeamBId] = {canonicalTeam.Id} WHERE [TeamBId] = {duplicateTeam.Id};
                UPDATE [ForumUserFavoriteTeams] SET [TeamId] = {canonicalTeam.Id} WHERE [TeamId] = {duplicateTeam.Id};
                UPDATE [EventTeams] SET [TeamsId] = {canonicalTeam.Id}
                WHERE [TeamsId] = {duplicateTeam.Id}
                  AND NOT EXISTS (
                      SELECT 1 FROM [EventTeams] existing
                      WHERE existing.[TeamsId] = {canonicalTeam.Id}
                        AND existing.[TournamentsId] = [EventTeams].[TournamentsId]
                  );
                DELETE FROM [EventTeams] WHERE [TeamsId] = {duplicateTeam.Id};
                DELETE FROM [Teams] WHERE [Id] = {duplicateTeam.Id};");
        }
    }

    private static async Task NormalizeUnseededTeamRankingsAsync(Cs2ScopeDbContext dbContext, HashSet<string> seededTeamNames)
    {
        var teams = await dbContext.Teams.OrderBy(team => team.WorldRanking).ThenBy(team => team.Name).ToListAsync();
        var nextRanking = teams
            .Where(team => seededTeamNames.Contains(team.Name))
            .Select(team => team.WorldRanking)
            .DefaultIfEmpty(0)
            .Max() + 1;

        foreach (var team in teams.Where(team => !seededTeamNames.Contains(team.Name)))
        {
            if (team.WorldRanking < nextRanking || teams.Any(other => other.Id != team.Id && other.WorldRanking == team.WorldRanking))
            {
                team.WorldRanking = nextRanking;
            }

            nextRanking = Math.Max(nextRanking, team.WorldRanking + 1);
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task<Dictionary<string, EventVenue>> EnsureVenuesAsync(Cs2ScopeDbContext dbContext)
    {
        var seeds = new[]
        {
            new VenueSeed("Lanxess Arena", "Cologne", "DE", 20000, true, "Arena"),
            new VenueSeed("OVO Arena Wembley", "London", "GB", 12500, true, "Arena"),
            new VenueSeed("Qudos Bank Arena", "Sydney", "AU", 21000, true, "Arena"),
            new VenueSeed("Riyadh Boulevard Arena", "Riyadh", "SA", 22000, true, "Arena"),
            new VenueSeed("Spodek Arena", "Katowice", "PL", 11500, true, "Arena"),
            new VenueSeed("MVM Dome", "Budapest", "HU", 20000, true, "Arena"),
            new VenueSeed("Singapore Indoor Stadium", "Singapore", "SG", 12000, true, "Arena"),
            new VenueSeed("Mercedes-Benz Arena", "Shanghai", "CN", 18000, true, "Arena")
        };

        var existingVenues = await dbContext.EventVenues.ToListAsync();
        foreach (var seed in seeds)
        {
            var venue = existingVenues.FirstOrDefault(item => item.Name.Equals(seed.Name, StringComparison.OrdinalIgnoreCase));
            if (venue is null)
            {
                venue = new EventVenue { Name = seed.Name };
                dbContext.EventVenues.Add(venue);
                existingVenues.Add(venue);
            }

            venue.City = seed.City;
            venue.CountryCode = seed.CountryCode;
            venue.Capacity = seed.Capacity;
            venue.IsIndoor = seed.IsIndoor;
            venue.SurfaceType = seed.SurfaceType;
        }

        await dbContext.SaveChangesAsync();

        return existingVenues.ToDictionary(venue => venue.Name, StringComparer.OrdinalIgnoreCase);
    }

    private static async Task EnsureEventsAsync(
        Cs2ScopeDbContext dbContext,
        IReadOnlyDictionary<string, Team> teams,
        IReadOnlyDictionary<string, EventVenue> venues)
    {
        var eventSeeds = BuildEventSeeds();
        var seededEventNames = eventSeeds.Select(seed => seed.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var existingEvents = await dbContext.Tournaments
            .Include(eventItem => eventItem.Teams)
            .Include(eventItem => eventItem.Matches)
            .Where(eventItem => seededEventNames.Contains(eventItem.Name))
            .ToListAsync();

        foreach (var seed in eventSeeds)
        {
            var eventItem = existingEvents.FirstOrDefault(item => item.Name.Equals(seed.Name, StringComparison.OrdinalIgnoreCase));
            if (eventItem is null)
            {
                eventItem = new Event { Name = seed.Name };
                dbContext.Tournaments.Add(eventItem);
                existingEvents.Add(eventItem);
            }

            eventItem.Organizer = seed.Organizer;
            eventItem.Tier = seed.Tier;
            eventItem.PrizePoolUsd = seed.PrizePoolUsd;
            eventItem.StartDateUtc = seed.StartDateUtc;
            eventItem.EndDateUtc = seed.EndDateUtc;
            eventItem.IsLan = seed.IsLan;
            eventItem.BannerImagePath = seed.BannerImagePath;
            eventItem.EventVenueId = venues[seed.VenueName].Id;

            eventItem.Teams.Clear();
            foreach (var teamName in seed.TeamNames)
            {
                eventItem.Teams.Add(teams[teamName]);
            }

            dbContext.Matches.RemoveRange(eventItem.Matches);
            eventItem.Matches.Clear();
            foreach (var matchSeed in seed.Matches)
            {
                eventItem.Matches.Add(new Match
                {
                    ScheduledAtUtc = matchSeed.ScheduledAtUtc,
                    FinishedAtUtc = matchSeed.FinishedAtUtc,
                    Format = matchSeed.Format,
                    IsFinished = matchSeed.IsFinished,
                    TeamAId = teams[matchSeed.TeamAName].Id,
                    TeamBId = teams[matchSeed.TeamBName].Id,
                    TeamAScore = matchSeed.TeamAScore,
                    TeamBScore = matchSeed.TeamBScore
                });
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static EventSeed[] BuildEventSeeds()
    {
        var topEight = new[] { "Vitality", "Team Spirit", "NAVI", "MOUZ", "FaZe", "G2", "Falcons", "The MongolZ" };
        var americas = new[] { "Team Liquid", "FURIA", "Complexity", "MIBR", "paiN", "HEROIC", "Aurora", "Virtus.pro" };
        var mixedEight = new[] { "Vitality", "Team Spirit", "MOUZ", "G2", "The MongolZ", "Team Liquid", "FURIA", "Aurora" };
        var challengerEight = new[] { "Falcons", "NAVI", "FaZe", "HEROIC", "Virtus.pro", "Complexity", "MIBR", "paiN" };

        return new[]
        {
            new EventSeed(
                "IEM Cologne Major 2026",
                "ESL",
                EventTier.Major,
                1250000m,
                Utc(2026, 6, 2),
                Utc(2026, 6, 21, 22),
                true,
                "Lanxess Arena",
                "/images/events/banners/iem-cologne-2026.png",
                topEight.Concat(americas).ToArray(),
                new[]
                {
                    Match("Vitality", "paiN", 2026, 6, 5, 13),
                    Match("Team Spirit", "MIBR", 2026, 6, 5, 16),
                    Match("NAVI", "Complexity", 2026, 6, 6, 13),
                    Match("MOUZ", "Virtus.pro", 2026, 6, 6, 16),
                    Match("FaZe", "Aurora", 2026, 6, 7, 13),
                    Match("G2", "HEROIC", 2026, 6, 7, 16),
                    Match("Falcons", "FURIA", 2026, 6, 8, 13),
                    Match("The MongolZ", "Team Liquid", 2026, 6, 8, 16)
                }),
            BracketEvent("BLAST Rivals Summer 2026", "BLAST", EventTier.S, 350000m, Utc(2026, 6, 25), Utc(2026, 6, 28, 21), "OVO Arena Wembley", "/images/events/banners/blast-rivals-summer-2026.png", mixedEight),
            BracketEvent("Esports World Cup 2026", "Esports World Cup Foundation", EventTier.S, 1000000m, Utc(2026, 7, 9), Utc(2026, 7, 19, 21), "Riyadh Boulevard Arena", "/images/events/banners/esports-world-cup-2026.png", topEight),
            BracketEvent("BLAST Bounty Summer 2026", "BLAST", EventTier.A, 500000m, Utc(2026, 7, 30), Utc(2026, 8, 9, 21), "OVO Arena Wembley", "/images/events/banners/blast-bounty-summer-2026.png", challengerEight),
            BracketEvent("PGL Astana 2026", "PGL", EventTier.S, 625000m, Utc(2026, 8, 6), Utc(2026, 8, 16, 21), "Qudos Bank Arena", "/images/events/banners/pgl-astana-2026.png", mixedEight),
            BracketEvent("BLAST Open Fall 2026", "BLAST", EventTier.S, 400000m, Utc(2026, 8, 31), Utc(2026, 9, 12, 21), "MVM Dome", "/images/events/banners/blast-open-fall-2026.png", topEight),
            BracketEvent("StarLadder StarSeries Fall 2026", "StarLadder", EventTier.A, 500000m, Utc(2026, 9, 14), Utc(2026, 9, 20, 21), "Singapore Indoor Stadium", "/images/events/banners/starladder-starseries-fall-2026.png", challengerEight),
            BracketEvent("BLAST Open October 2026", "BLAST", EventTier.S, 400000m, Utc(2026, 10, 6), Utc(2026, 10, 17, 21), "MVM Dome", "/images/events/banners/blast-open-october-2026.png", mixedEight),
            BracketEvent("IEM China 2026", "ESL", EventTier.S, 300000m, Utc(2026, 11, 3), Utc(2026, 11, 15, 21), "Mercedes-Benz Arena", "/images/events/banners/iem-china-2026.png", topEight),
            BracketEvent("Perfect World Shanghai Masters 2026", "Perfect World", EventTier.A, 350000m, Utc(2026, 12, 5), Utc(2026, 12, 13, 21), "Mercedes-Benz Arena", "/images/events/banners/perfect-world-shanghai-masters-2026.png", americas)
        };
    }

    private static EventSeed BracketEvent(
        string name,
        string organizer,
        EventTier tier,
        decimal prizePoolUsd,
        DateTime startDateUtc,
        DateTime endDateUtc,
        string venueName,
        string bannerImagePath,
        string[] teamNames)
    {
        return new EventSeed(
            name,
            organizer,
            tier,
            prizePoolUsd,
            startDateUtc,
            endDateUtc,
            true,
            venueName,
            bannerImagePath,
            teamNames,
            new[]
            {
                Match(teamNames[0], teamNames[7], startDateUtc.AddHours(12)),
                Match(teamNames[1], teamNames[6], startDateUtc.AddHours(15)),
                Match(teamNames[2], teamNames[5], startDateUtc.AddDays(1).AddHours(12)),
                Match(teamNames[3], teamNames[4], startDateUtc.AddDays(1).AddHours(15)),
                Match(teamNames[0], teamNames[3], endDateUtc.Date.AddDays(-1).AddHours(13)),
                Match(teamNames[1], teamNames[2], endDateUtc.Date.AddDays(-1).AddHours(17)),
                Match(teamNames[0], teamNames[1], endDateUtc.Date.AddHours(17), MatchFormat.BestOf5)
            });
    }

    private static MatchSeed Match(
        string teamAName,
        string teamBName,
        DateTime scheduledAtUtc,
        MatchFormat format = MatchFormat.BestOf3)
    {
        return new MatchSeed(teamAName, teamBName, scheduledAtUtc, format, false, 0, 0, null);
    }

    private static MatchSeed Match(
        string teamAName,
        string teamBName,
        int year,
        int month,
        int day,
        int hour,
        MatchFormat format = MatchFormat.BestOf3)
    {
        return new MatchSeed(teamAName, teamBName, Utc(year, month, day, hour), format, false, 0, 0, null);
    }

    private static DateTime Utc(int year, int month, int day, int hour = 0)
        => new(year, month, day, hour, 0, 0, DateTimeKind.Utc);

    private sealed record TeamSeed(string Name, string Tag, string CountryCode, int WorldRanking, int FoundedYear, decimal PrizeMoneyUsd);
    private sealed record VenueSeed(string Name, string City, string CountryCode, int Capacity, bool IsIndoor, string SurfaceType);
    private sealed record MatchSeed(string TeamAName, string TeamBName, DateTime ScheduledAtUtc, MatchFormat Format, bool IsFinished, int TeamAScore, int TeamBScore, DateTime? FinishedAtUtc);
    private sealed record EventSeed(string Name, string Organizer, EventTier Tier, decimal PrizePoolUsd, DateTime StartDateUtc, DateTime EndDateUtc, bool IsLan, string VenueName, string BannerImagePath, string[] TeamNames, MatchSeed[] Matches);
}
