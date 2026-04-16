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
            },
            new()
            {
                Id = 4,
                Name = "O2 Arena",
                City = "London",
                CountryCode = "GB",
                Capacity = 20000,
                IsIndoor = true,
                SurfaceType = "Arena Floor"
            },
            new()
            {
                Id = 5,
                Name = "Spodek Small Stage",
                City = "Katowice",
                CountryCode = "PL",
                Capacity = 2500,
                IsIndoor = true,
                SurfaceType = "Stage Floor"
            }
        };
        data.EventVenues.AddRange(venues);

        var teams = CreateTopTeams();
        data.Teams.AddRange(teams);

        var allPlayers = teams.SelectMany(t => t.Players).ToList();
        data.Players.AddRange(allPlayers);

        var tournaments = new List<Event>
        {
            new()
            {
                Id = 1,
                Name = "IEM Katowice 2026",
                Organizer = "ESL",
                Tier = EventTier.S,
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
                Tier = EventTier.S,
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
                Tier = EventTier.S,
                PrizePoolUsd = 1000000m,
                StartDateUtc = new DateTime(2026, 12, 10, 12, 0, 0, DateTimeKind.Utc),
                EndDateUtc = new DateTime(2026, 12, 15, 20, 0, 0, DateTimeKind.Utc),
                IsLan = true,
                EventVenueId = venues[2].Id,
                EventVenue = venues[2],
                AdminUserId = admin.Id,
                AdminUser = admin,
                Teams = teams
            },
            new()
            {
                Id = 4,
                Name = "PGL Masters Bucharest 2026",
                Organizer = "PGL",
                Tier = EventTier.A,
                PrizePoolUsd = 350000m,
                StartDateUtc = new DateTime(2026, 3, 14, 12, 0, 0, DateTimeKind.Utc),
                EndDateUtc = new DateTime(2026, 3, 23, 20, 0, 0, DateTimeKind.Utc),
                IsLan = true,
                EventVenueId = venues[3].Id,
                EventVenue = venues[3],
                AdminUserId = admin.Id,
                AdminUser = admin,
                Teams = new List<Team> { teams[1], teams[2], teams[4], teams[5] }
            },
            new()
            {
                Id = 5,
                Name = "ESL Challenger London 2026",
                Organizer = "ESL",
                Tier = EventTier.B,
                PrizePoolUsd = 100000m,
                StartDateUtc = new DateTime(2026, 4, 8, 12, 0, 0, DateTimeKind.Utc),
                EndDateUtc = new DateTime(2026, 4, 12, 20, 0, 0, DateTimeKind.Utc),
                IsLan = true,
                EventVenueId = venues[3].Id,
                EventVenue = venues[3],
                AdminUserId = admin.Id,
                AdminUser = admin,
                Teams = new List<Team> { teams[0], teams[3], teams[5] }
            },
            new()
            {
                Id = 6,
                Name = "Thunderpick Open 2026",
                Organizer = "Thunderpick",
                Tier = EventTier.C,
                PrizePoolUsd = 50000m,
                StartDateUtc = new DateTime(2026, 6, 2, 12, 0, 0, DateTimeKind.Utc),
                EndDateUtc = new DateTime(2026, 6, 6, 20, 0, 0, DateTimeKind.Utc),
                IsLan = false,
                EventVenueId = venues[4].Id,
                EventVenue = venues[4],
                AdminUserId = admin.Id,
                AdminUser = admin,
                Teams = new List<Team> { teams[2], teams[4] }
            },
            new()
            {
                Id = 7,
                Name = "CCT Season 2 Europe",
                Organizer = "CCT",
                Tier = EventTier.A,
                PrizePoolUsd = 200000m,
                StartDateUtc = new DateTime(2026, 9, 18, 12, 0, 0, DateTimeKind.Utc),
                EndDateUtc = new DateTime(2026, 9, 27, 20, 0, 0, DateTimeKind.Utc),
                IsLan = false,
                EventVenueId = venues[4].Id,
                EventVenue = venues[4],
                AdminUserId = admin.Id,
                AdminUser = admin,
                Teams = new List<Team> { teams[0], teams[1], teams[3] }
            }
        };

        foreach (var tournament in tournaments)
        {
            foreach (var team in tournament.Teams)
            {
                team.Tournaments.Add(tournament);
            }

            tournament.EventVenue?.Tournaments.Add(tournament);
            admin.ManagedTournaments.Add(tournament);
        }

        data.Tournaments.AddRange(tournaments);

        var forumUsers = CreateForumUsers();
        data.ForumUsers.AddRange(forumUsers);

        var forums = CreateForums(forumUsers, tournaments);
        foreach (var forum in forums)
        {
            forum.Author?.Threads.Add(forum);
            forum.Event?.ForumThreads.Add(forum);
        }

        data.Forums.AddRange(forums);

        var comments = CreateForumComments(forumUsers, forums);
        foreach (var comment in comments)
        {
            comment.Author?.Comments.Add(comment);
            comment.Forum?.Comments.Add(comment);
        }

        data.ForumComments.AddRange(comments);

        return data;
    }

    private static List<ForumUser> CreateForumUsers()
    {
        return new List<ForumUser>
        {
            new()
            {
                Id = 1,
                Username = "stratwizard",
                DisplayName = "Strat Wizard",
                Email = "stratwizard@cs2scope.local",
                CountryCode = "HR",
                Password = "password123",
                RegisteredAtUtc = new DateTime(2024, 5, 10, 10, 0, 0, DateTimeKind.Utc),
                LastActiveAtUtc = new DateTime(2026, 4, 12, 18, 30, 0, DateTimeKind.Utc),
                IsPremiumMember = true
            },
            new()
            {
                Id = 2,
                Username = "entryfragger",
                DisplayName = "Entry Fragger",
                Email = "entry@cs2scope.local",
                CountryCode = "DE",
                Password = "password123",
                RegisteredAtUtc = new DateTime(2024, 9, 1, 12, 0, 0, DateTimeKind.Utc),
                LastActiveAtUtc = new DateTime(2026, 4, 11, 15, 0, 0, DateTimeKind.Utc),
                IsPremiumMember = false
            },
            new()
            {
                Id = 3,
                Username = "smokecaller",
                DisplayName = "Smoke Caller",
                Email = "smoke@cs2scope.local",
                CountryCode = "PL",
                Password = "password123",
                RegisteredAtUtc = new DateTime(2025, 2, 20, 8, 0, 0, DateTimeKind.Utc),
                LastActiveAtUtc = new DateTime(2026, 4, 13, 9, 0, 0, DateTimeKind.Utc),
                IsPremiumMember = true
            }
        };
    }

    private static List<Forum> CreateForums(IReadOnlyList<ForumUser> forumUsers, IReadOnlyList<Event> tournaments)
    {
        return new List<Forum>
        {
            new()
            {
                Id = 1,
                Title = "Who was the MVP of IEM Katowice 2026?",
                Content = "Curious what everyone thinks. For me donk was unreal in key rounds.",
                Category = ForumCategory.MatchDiscussion,
                CreatedAtUtc = new DateTime(2026, 2, 16, 11, 0, 0, DateTimeKind.Utc),
                LastUpdatedAtUtc = new DateTime(2026, 2, 16, 18, 15, 0, DateTimeKind.Utc),
                ViewCount = 1142,
                IsPinned = true,
                IsLocked = false,
                AuthorId = forumUsers[0].Id,
                Author = forumUsers[0],
                TournamentId = tournaments[0].Id,
                Event = tournaments[0]
            },
            new()
            {
                Id = 2,
                Title = "Best map pool changes before Cologne",
                Content = "Which maps should top teams prioritize for Cologne prep?",
                Category = ForumCategory.TournamentTalk,
                CreatedAtUtc = new DateTime(2026, 5, 28, 10, 25, 0, DateTimeKind.Utc),
                LastUpdatedAtUtc = new DateTime(2026, 5, 29, 7, 45, 0, DateTimeKind.Utc),
                ViewCount = 694,
                IsPinned = false,
                IsLocked = false,
                AuthorId = forumUsers[1].Id,
                Author = forumUsers[1],
                TournamentId = tournaments[1].Id,
                Event = tournaments[1]
            },
            new()
            {
                Id = 3,
                Title = "Rumored offseason roster swaps",
                Content = "Drop only confirmed or strongly sourced info in this thread.",
                Category = ForumCategory.RosterMoves,
                CreatedAtUtc = new DateTime(2026, 8, 2, 14, 0, 0, DateTimeKind.Utc),
                LastUpdatedAtUtc = new DateTime(2026, 8, 2, 20, 30, 0, DateTimeKind.Utc),
                ViewCount = 2380,
                IsPinned = false,
                IsLocked = false,
                AuthorId = forumUsers[2].Id,
                Author = forumUsers[2]
            },
            new()
            {
                Id = 4,
                Title = "World Final bracket predictions",
                Content = "Post your semifinals and finals predictions before the event starts.",
                Category = ForumCategory.General,
                CreatedAtUtc = new DateTime(2026, 12, 5, 9, 0, 0, DateTimeKind.Utc),
                LastUpdatedAtUtc = new DateTime(2026, 12, 5, 9, 0, 0, DateTimeKind.Utc),
                ViewCount = 320,
                IsPinned = false,
                IsLocked = false,
                AuthorId = forumUsers[0].Id,
                Author = forumUsers[0],
                TournamentId = tournaments[2].Id,
                Event = tournaments[2]
            }
        };
    }

    private static List<ForumComment> CreateForumComments(IReadOnlyList<ForumUser> forumUsers, IReadOnlyList<Forum> forums)
    {
        return new List<ForumComment>
        {
            new()
            {
                Id = 1,
                ForumId = forums[0].Id,
                Forum = forums[0],
                AuthorId = forumUsers[1].Id,
                Author = forumUsers[1],
                Content = "donk for sure, but sh1ro's late-round impact was huge too.",
                CreatedAtUtc = new DateTime(2026, 2, 16, 11, 45, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = 2,
                ForumId = forums[0].Id,
                Forum = forums[0],
                AuthorId = forumUsers[2].Id,
                Author = forumUsers[2],
                Content = "I'd still vote donk. His entries swung multiple map points.",
                CreatedAtUtc = new DateTime(2026, 2, 16, 13, 5, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = 3,
                ForumId = forums[2].Id,
                Forum = forums[2],
                AuthorId = forumUsers[0].Id,
                Author = forumUsers[0],
                Content = "Only HLTV Confirmed-level sources please, no random screenshots.",
                CreatedAtUtc = new DateTime(2026, 8, 2, 16, 10, 0, DateTimeKind.Utc)
            }
        };
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
                Rating2 = GetRating(definition.Nickname, definition.Role),
                TotalMapsPlayed = 400,
                JoinedTeamAtUtc = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                TeamId = team.Id,
                Team = team
            };

            team.Players.Add(player);
        }
    }

    private static decimal GetRating(string nickname, PlayerRole role)
    {
        if (role == PlayerRole.InGameLeader)
        {
            return 0.98m;
        }

        return nickname switch
        {
            "donk" => 1.28m,
            "ZywOo" => 1.26m,
            "m0NESY" => 1.22m,
            "NiKo" => 1.18m,
            "sh1ro" => 1.16m,
            "frozen" => 1.14m,
            "w0nderful" => 1.13m,
            "b1t" => 1.12m,
            "Spinx" => 1.10m,
            "rain" => 1.08m,
            "xertioN" => 1.07m,
            _ => 1.02m
        };
    }
}

