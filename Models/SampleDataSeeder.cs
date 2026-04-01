namespace cs2_esports.Models;

public static class SampleDataSeeder
{
    public static InMemoryAppData Create()
    {
        var data = new InMemoryAppData();

        var admin = new AdminUser
        {
            Id = 1,
            Username = "admin_maksim",
            DisplayName = "Maksim Admin",
            Email = "admin@cs2esports.local",
            CountryCode = "HR",
            RegisteredAtUtc = new DateTime(2025, 9, 1, 10, 0, 0, DateTimeKind.Utc),
            HiredAtUtc = new DateTime(2025, 10, 1, 10, 0, 0, DateTimeKind.Utc),
            PermissionGroup = "SuperAdmin"
        };
        data.AdminUsers.Add(admin);

        var venues = new List<EventVenue>
        {
            new()
            {
                Id = 1,
                Name = "Spodek Arena",
                City = "Katowice",
                CountryCode = "PL",
                Capacity = 11500,
                IsIndoor = true,
                SurfaceType = "Arena Floor"
            },
            new()
            {
                Id = 2,
                Name = "Lanxess Arena",
                City = "Cologne",
                CountryCode = "DE",
                Capacity = 18000,
                IsIndoor = true,
                SurfaceType = "Arena Floor"
            },
            new()
            {
                Id = 3,
                Name = "Royal Arena",
                City = "Copenhagen",
                CountryCode = "DK",
                Capacity = 16000,
                IsIndoor = true,
                SurfaceType = "Arena Floor"
            }
        };
        data.EventVenues.AddRange(venues);

        var teams = CreateTopTeams();
        data.Teams.AddRange(teams);

        var allPlayers = teams.SelectMany(t => t.Players).ToList();
        data.Players.AddRange(allPlayers);

        var tournaments = new List<Tournament>
        {
            new()
            {
                Id = 1,
                Name = "IEM Katowice 2026",
                Organizer = "ESL",
                Tier = TournamentTier.S,
                PrizePoolUsd = 1000000m,
                StartDateUtc = new DateTime(2026, 2, 5, 12, 0, 0, DateTimeKind.Utc),
                EndDateUtc = new DateTime(2026, 2, 15, 20, 0, 0, DateTimeKind.Utc),
                IsLan = true,
                EventVenueId = venues[0].Id,
                EventVenue = venues[0],
                AdminUserId = admin.Id,
                AdminUser = admin,
                Teams = teams
            },
            new()
            {
                Id = 2,
                Name = "IEM Cologne 2026",
                Organizer = "ESL",
                Tier = TournamentTier.S,
                PrizePoolUsd = 1000000m,
                StartDateUtc = new DateTime(2026, 7, 20, 12, 0, 0, DateTimeKind.Utc),
                EndDateUtc = new DateTime(2026, 7, 30, 20, 0, 0, DateTimeKind.Utc),
                IsLan = true,
                EventVenueId = venues[1].Id,
                EventVenue = venues[1],
                AdminUserId = admin.Id,
                AdminUser = admin,
                Teams = teams
            },
            new()
            {
                Id = 3,
                Name = "BLAST Premier World Final 2026",
                Organizer = "BLAST",
                Tier = TournamentTier.S,
                PrizePoolUsd = 1000000m,
                StartDateUtc = new DateTime(2026, 12, 10, 12, 0, 0, DateTimeKind.Utc),
                EndDateUtc = new DateTime(2026, 12, 15, 20, 0, 0, DateTimeKind.Utc),
                IsLan = true,
                EventVenueId = venues[2].Id,
                EventVenue = venues[2],
                AdminUserId = admin.Id,
                AdminUser = admin,
                Teams = teams
            }
        };

        foreach (var tournament in tournaments)
        {
            foreach (var team in teams)
            {
                team.Tournaments.Add(tournament);
            }

            tournament.EventVenue?.Tournaments.Add(tournament);
            admin.ManagedTournaments.Add(tournament);
        }

        data.Tournaments.AddRange(tournaments);

        return data;
    }

    private static List<Team> CreateTopTeams()
    {
        var teamDefinitions = new List<(int Id, string Name, string Tag, string CountryCode, int Ranking, int FoundedYear, decimal PrizeMoneyUsd)>
        {
            (1, "Team Spirit", "TS", "RU", 1, 2015, 15000000m),
            (2, "Vitality", "VIT", "FR", 2, 2013, 22000000m),
            (3, "Natus Vincere", "NAVI", "UA", 3, 2009, 21000000m),
            (4, "MOUZ", "MOUZ", "DE", 4, 2002, 9000000m),
            (5, "FaZe Clan", "FAZE", "EU", 5, 2016, 20000000m),
            (6, "G2 Esports", "G2", "EU", 6, 2013, 17000000m)
        };

        var teams = teamDefinitions
            .Select(def => new Team
            {
                Id = def.Id,
                Name = def.Name,
                Tag = def.Tag,
                CountryCode = def.CountryCode,
                WorldRanking = def.Ranking,
                FoundedYear = def.FoundedYear,
                PrizeMoneyUsd = def.PrizeMoneyUsd,
                LastRosterUpdateUtc = new DateTime(2026, 1, 15, 9, 0, 0, DateTimeKind.Utc)
            })
            .ToList();

        int playerId = 1;

        AddPlayers(teams[0], ref playerId, new[]
        {
            ("donk", "Danil Kryshkovets", "RU", PlayerRole.Rifler),
            ("sh1ro", "Dmitriy Sokolov", "RU", PlayerRole.Awper),
            ("zont1x", "Myroslav Plakhotia", "UA", PlayerRole.Support),
            ("magixx", "Boris Vorobiev", "RU", PlayerRole.EntryFragger),
            ("chopper", "Leonid Vishnyakov", "RU", PlayerRole.InGameLeader)
        });

        AddPlayers(teams[1], ref playerId, new[]
        {
            ("ZywOo", "Mathieu Herbaut", "FR", PlayerRole.Awper),
            ("flameZ", "Shahar Shushan", "IL", PlayerRole.EntryFragger),
            ("Spinx", "Lotan Giladi", "IL", PlayerRole.Rifler),
            ("mezii", "William Merriman", "GB", PlayerRole.Support),
            ("apEX", "Dan Madesclaire", "FR", PlayerRole.InGameLeader)
        });

        AddPlayers(teams[2], ref playerId, new[]
        {
            ("b1t", "Valeriy Vakhovskiy", "UA", PlayerRole.Rifler),
            ("w0nderful", "Ihor Zhdanov", "UA", PlayerRole.Awper),
            ("iM", "Ivan Mihai", "RO", PlayerRole.EntryFragger),
            ("jL", "Justinas Lekavicius", "LT", PlayerRole.Support),
            ("Aleksib", "Aleksi Virolainen", "FI", PlayerRole.InGameLeader)
        });

        AddPlayers(teams[3], ref playerId, new[]
        {
            ("xertioN", "Dorian Berman", "IL", PlayerRole.EntryFragger),
            ("torzsi", "Adam Torzsas", "HU", PlayerRole.Awper),
            ("Brollan", "Ludvig Brolin", "SE", PlayerRole.Rifler),
            ("Jimpphat", "Jimi Salo", "FI", PlayerRole.Support),
            ("siuhy", "Kamil Szkaradek", "PL", PlayerRole.InGameLeader)
        });

        AddPlayers(teams[4], ref playerId, new[]
        {
            ("rain", "Havard Nygaard", "NO", PlayerRole.Rifler),
            ("broky", "Helvijs Saukants", "LV", PlayerRole.Awper),
            ("frozen", "David Cernansky", "SK", PlayerRole.Support),
            ("ropz", "Robin Kool", "EE", PlayerRole.EntryFragger),
            ("karrigan", "Finn Andersen", "DK", PlayerRole.InGameLeader)
        });

        AddPlayers(teams[5], ref playerId, new[]
        {
            ("NiKo", "Nikola Kovac", "BA", PlayerRole.Rifler),
            ("m0NESY", "Ilya Osipov", "RU", PlayerRole.Awper),
            ("huNter-", "Nemanja Kovac", "BA", PlayerRole.Support),
            ("malbsMd", "Mario Samayoa", "GT", PlayerRole.EntryFragger),
            ("Snax", "Janusz Pogorzelski", "PL", PlayerRole.InGameLeader)
        });

        return teams;
    }

    private static void AddPlayers(
        Team team,
        ref int playerId,
        IEnumerable<(string Nickname, string FullName, string CountryCode, PlayerRole Role)> definitions)
    {
        foreach (var definition in definitions)
        {
            var player = new Player
            {
                Id = playerId++,
                Nickname = definition.Nickname,
                FullName = definition.FullName,
                CountryCode = definition.CountryCode,
                DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Role = definition.Role,
                Rating2 = 1.1m,
                TotalMapsPlayed = 400,
                JoinedTeamAtUtc = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                TeamId = team.Id,
                Team = team
            };

            team.Players.Add(player);
        }
    }
}
