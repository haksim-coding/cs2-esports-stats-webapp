namespace cs2_esports.Models;

public static class SampleLinqQueries
{
    public static List<string> BuildReport(InMemoryAppData data)
    {
        var report = new List<string>();

        var topThreeTeams = data.Teams
            .OrderBy(team => team.WorldRanking)
            .Take(3)
            .Select(team => $"#{team.WorldRanking} {team.Name} ({team.Tag})")
            .ToList();

        report.Add("Top 3 teams by ranking:");
        report.AddRange(topThreeTeams);

        var tournaments2026 = data.Tournaments
            .Where(tournament => tournament.StartDateUtc.Year == 2026)
            .Select(tournament => $"{tournament.Name} in {tournament.EventVenue?.City}")
            .ToList();

        report.Add("Tournaments in 2026:");
        report.AddRange(tournaments2026);

        var awpers = data.Teams
            .SelectMany(team => team.Players)
            .Where(player => player.Role == PlayerRole.Awper)
            .Select(player => $"{player.Nickname} ({player.Team?.Tag})")
            .ToList();

        report.Add("Awper players:");
        report.AddRange(awpers);

        var completeTeams = data.Teams
            .Where(team => team.Players.Count == 5)
            .Select(team => $"{team.Name} has {team.Players.Count} players")
            .ToList();

        report.Add("Teams with exactly 5 players:");
        report.AddRange(completeTeams);

        var blastTournaments = data.Tournaments
            .Where(tournament => tournament.Organizer == "BLAST")
            .Select(tournament => $"{tournament.Name} at {tournament.EventVenue?.Name}")
            .ToList();

        report.Add("Tournaments hosted by BLAST:");
        report.AddRange(blastTournaments);

        return report;
    }
}
