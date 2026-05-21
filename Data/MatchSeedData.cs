using cs2_esports.Models;

namespace cs2_esports.Data;

public static class MatchSeedData
{
    public static Team[] GetTeams()
    {
        return new[]
        {
            new Team { Id = 8, Name = "The MongolZ", Tag = "MGLZ", CountryCode = "MN", WorldRanking = 8, FoundedYear = 2013, PrizeMoneyUsd = 1850000m, LastRosterUpdateUtc = new DateTime(2026, 1, 18, 12, 0, 0, DateTimeKind.Utc) },
            new Team { Id = 9, Name = "Team Liquid", Tag = "TL", CountryCode = "US", WorldRanking = 9, FoundedYear = 2000, PrizeMoneyUsd = 1760000m, LastRosterUpdateUtc = new DateTime(2026, 1, 22, 12, 0, 0, DateTimeKind.Utc) },
            new Team { Id = 10, Name = "Aurora", Tag = "AUR", CountryCode = "TR", WorldRanking = 10, FoundedYear = 2020, PrizeMoneyUsd = 1420000m, LastRosterUpdateUtc = new DateTime(2026, 2, 2, 12, 0, 0, DateTimeKind.Utc) },
            new Team { Id = 11, Name = "HEROIC", Tag = "HERO", CountryCode = "DK", WorldRanking = 11, FoundedYear = 2016, PrizeMoneyUsd = 1390000m, LastRosterUpdateUtc = new DateTime(2026, 2, 8, 12, 0, 0, DateTimeKind.Utc) },
            new Team { Id = 12, Name = "FURIA", Tag = "FUR", CountryCode = "BR", WorldRanking = 12, FoundedYear = 2017, PrizeMoneyUsd = 1680000m, LastRosterUpdateUtc = new DateTime(2026, 2, 14, 12, 0, 0, DateTimeKind.Utc) },
            new Team { Id = 13, Name = "Virtus.pro", Tag = "VP", CountryCode = "RU", WorldRanking = 13, FoundedYear = 2003, PrizeMoneyUsd = 1540000m, LastRosterUpdateUtc = new DateTime(2026, 2, 18, 12, 0, 0, DateTimeKind.Utc) },
            new Team { Id = 14, Name = "Complexity", Tag = "COL", CountryCode = "US", WorldRanking = 14, FoundedYear = 2003, PrizeMoneyUsd = 1180000m, LastRosterUpdateUtc = new DateTime(2026, 2, 25, 12, 0, 0, DateTimeKind.Utc) },
            new Team { Id = 15, Name = "MIBR", Tag = "MIBR", CountryCode = "BR", WorldRanking = 15, FoundedYear = 2003, PrizeMoneyUsd = 1120000m, LastRosterUpdateUtc = new DateTime(2026, 3, 3, 12, 0, 0, DateTimeKind.Utc) }
        };
    }

    public static Player[] GetPlayers()
    {
        return new[]
        {
            new Player { Id = 31, TeamId = 8, Nickname = "bLitz", FullName = "Byambasuren Garidmagnai", CountryCode = "MN", DateOfBirth = new DateTime(1999, 10, 7), Role = PlayerRole.InGameLeader, Rating2 = 1.06m, TotalMapsPlayed = 1140, JoinedTeamAtUtc = new DateTime(2024, 11, 15, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 32, TeamId = 8, Nickname = "910", FullName = "Ayush Batbold", CountryCode = "MN", DateOfBirth = new DateTime(2002, 6, 18), Role = PlayerRole.EntryFragger, Rating2 = 1.14m, TotalMapsPlayed = 980, JoinedTeamAtUtc = new DateTime(2024, 11, 15, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 33, TeamId = 8, Nickname = "mzinho", FullName = "Sodbayar Munkhbold", CountryCode = "MN", DateOfBirth = new DateTime(2005, 5, 1), Role = PlayerRole.Rifler, Rating2 = 1.10m, TotalMapsPlayed = 760, JoinedTeamAtUtc = new DateTime(2024, 11, 15, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 34, TeamId = 8, Nickname = "Techno4K", FullName = "Munkhbold Enkhbat", CountryCode = "MN", DateOfBirth = new DateTime(2003, 4, 30), Role = PlayerRole.Rifler, Rating2 = 1.09m, TotalMapsPlayed = 830, JoinedTeamAtUtc = new DateTime(2024, 11, 15, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 35, TeamId = 8, Nickname = "Senzu", FullName = "Garidmagnai Baatarkhuu", CountryCode = "MN", DateOfBirth = new DateTime(2005, 1, 29), Role = PlayerRole.Awper, Rating2 = 1.13m, TotalMapsPlayed = 790, JoinedTeamAtUtc = new DateTime(2024, 11, 15, 12, 0, 0, DateTimeKind.Utc) },

            new Player { Id = 36, TeamId = 9, Nickname = "NAF", FullName = "Keith Markovic", CountryCode = "CA", DateOfBirth = new DateTime(1997, 11, 24), Role = PlayerRole.Rifler, Rating2 = 1.12m, TotalMapsPlayed = 1800, JoinedTeamAtUtc = new DateTime(2025, 1, 12, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 37, TeamId = 9, Nickname = "Twistzz", FullName = "Russel Van Dulken", CountryCode = "CA", DateOfBirth = new DateTime(1999, 11, 14), Role = PlayerRole.Rifler, Rating2 = 1.15m, TotalMapsPlayed = 1720, JoinedTeamAtUtc = new DateTime(2025, 1, 12, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 38, TeamId = 9, Nickname = "Nertz", FullName = "Guy Iluz", CountryCode = "IL", DateOfBirth = new DateTime(1999, 9, 4), Role = PlayerRole.Rifler, Rating2 = 1.11m, TotalMapsPlayed = 1110, JoinedTeamAtUtc = new DateTime(2025, 1, 12, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 39, TeamId = 9, Nickname = "ultimate", FullName = "Roland Tomkowiak", CountryCode = "PL", DateOfBirth = new DateTime(2003, 11, 5), Role = PlayerRole.Awper, Rating2 = 1.07m, TotalMapsPlayed = 940, JoinedTeamAtUtc = new DateTime(2025, 1, 12, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 40, TeamId = 9, Nickname = "jks", FullName = "Justin Savage", CountryCode = "AU", DateOfBirth = new DateTime(1995, 3, 12), Role = PlayerRole.Support, Rating2 = 1.08m, TotalMapsPlayed = 1920, JoinedTeamAtUtc = new DateTime(2025, 1, 12, 12, 0, 0, DateTimeKind.Utc) },

            new Player { Id = 41, TeamId = 10, Nickname = "XANTARES", FullName = "Ismailcan Dörtkardeş", CountryCode = "TR", DateOfBirth = new DateTime(1995, 8, 7), Role = PlayerRole.Rifler, Rating2 = 1.19m, TotalMapsPlayed = 1950, JoinedTeamAtUtc = new DateTime(2025, 2, 4, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 42, TeamId = 10, Nickname = "woxic", FullName = "Özgür Eker", CountryCode = "TR", DateOfBirth = new DateTime(1998, 9, 2), Role = PlayerRole.Awper, Rating2 = 1.12m, TotalMapsPlayed = 1885, JoinedTeamAtUtc = new DateTime(2025, 2, 4, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 43, TeamId = 10, Nickname = "MAJ3R", FullName = "Engin Küpeli", CountryCode = "TR", DateOfBirth = new DateTime(1990, 1, 25), Role = PlayerRole.InGameLeader, Rating2 = 1.00m, TotalMapsPlayed = 1680, JoinedTeamAtUtc = new DateTime(2025, 2, 4, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 44, TeamId = 10, Nickname = "Wicadia", FullName = "Ali Haydar Yalçın", CountryCode = "TR", DateOfBirth = new DateTime(2005, 2, 12), Role = PlayerRole.EntryFragger, Rating2 = 1.09m, TotalMapsPlayed = 860, JoinedTeamAtUtc = new DateTime(2025, 2, 4, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 45, TeamId = 10, Nickname = "jottAAA", FullName = "Jotaro Atmaca", CountryCode = "TR", DateOfBirth = new DateTime(2003, 8, 19), Role = PlayerRole.Support, Rating2 = 1.05m, TotalMapsPlayed = 920, JoinedTeamAtUtc = new DateTime(2025, 2, 4, 12, 0, 0, DateTimeKind.Utc) },

            new Player { Id = 46, TeamId = 11, Nickname = "tN1R", FullName = "Andrey Tatarinovich", CountryCode = "BY", DateOfBirth = new DateTime(2003, 7, 16), Role = PlayerRole.Rifler, Rating2 = 1.10m, TotalMapsPlayed = 820, JoinedTeamAtUtc = new DateTime(2025, 3, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 47, TeamId = 11, Nickname = "sjuush", FullName = "Rasmus Beck", CountryCode = "DK", DateOfBirth = new DateTime(1999, 9, 13), Role = PlayerRole.Support, Rating2 = 1.02m, TotalMapsPlayed = 1380, JoinedTeamAtUtc = new DateTime(2025, 3, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 48, TeamId = 11, Nickname = "nicoodoz", FullName = "Nico Tamjidi", CountryCode = "DK", DateOfBirth = new DateTime(2000, 1, 11), Role = PlayerRole.Awper, Rating2 = 1.08m, TotalMapsPlayed = 1280, JoinedTeamAtUtc = new DateTime(2025, 3, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 49, TeamId = 11, Nickname = "Alkaren", FullName = "Maksim Lukin", CountryCode = "RU", DateOfBirth = new DateTime(2004, 9, 24), Role = PlayerRole.EntryFragger, Rating2 = 1.04m, TotalMapsPlayed = 640, JoinedTeamAtUtc = new DateTime(2025, 3, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 50, TeamId = 11, Nickname = "nilo", FullName = "Nilo Ojala", CountryCode = "FI", DateOfBirth = new DateTime(2004, 4, 7), Role = PlayerRole.Support, Rating2 = 1.03m, TotalMapsPlayed = 680, JoinedTeamAtUtc = new DateTime(2025, 3, 1, 12, 0, 0, DateTimeKind.Utc) },

            new Player { Id = 51, TeamId = 12, Nickname = "FalleN", FullName = "Gabriel Toledo", CountryCode = "BR", DateOfBirth = new DateTime(1991, 5, 30), Role = PlayerRole.InGameLeader, Rating2 = 1.04m, TotalMapsPlayed = 2140, JoinedTeamAtUtc = new DateTime(2025, 3, 10, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 52, TeamId = 12, Nickname = "KSCERATO", FullName = "Kaike Cerato", CountryCode = "BR", DateOfBirth = new DateTime(1999, 9, 12), Role = PlayerRole.Rifler, Rating2 = 1.18m, TotalMapsPlayed = 2050, JoinedTeamAtUtc = new DateTime(2025, 3, 10, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 53, TeamId = 12, Nickname = "yuurih", FullName = "Yuri Santos", CountryCode = "BR", DateOfBirth = new DateTime(1999, 9, 16), Role = PlayerRole.Rifler, Rating2 = 1.14m, TotalMapsPlayed = 1985, JoinedTeamAtUtc = new DateTime(2025, 3, 10, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 54, TeamId = 12, Nickname = "molodoy", FullName = "Danil Golubenko", CountryCode = "KZ", DateOfBirth = new DateTime(2006, 1, 20), Role = PlayerRole.EntryFragger, Rating2 = 1.06m, TotalMapsPlayed = 420, JoinedTeamAtUtc = new DateTime(2025, 3, 10, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 55, TeamId = 12, Nickname = "yel", FullName = "Gabriel Wilhelm", CountryCode = "BR", DateOfBirth = new DateTime(2000, 11, 2), Role = PlayerRole.Support, Rating2 = 1.01m, TotalMapsPlayed = 760, JoinedTeamAtUtc = new DateTime(2025, 3, 10, 12, 0, 0, DateTimeKind.Utc) },

            new Player { Id = 56, TeamId = 13, Nickname = "electroNic", FullName = "Denis Sharipov", CountryCode = "RU", DateOfBirth = new DateTime(1998, 9, 2), Role = PlayerRole.InGameLeader, Rating2 = 1.05m, TotalMapsPlayed = 2055, JoinedTeamAtUtc = new DateTime(2025, 3, 15, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 57, TeamId = 13, Nickname = "FL1T", FullName = "Evgenii Lebedev", CountryCode = "RU", DateOfBirth = new DateTime(2000, 1, 15), Role = PlayerRole.Rifler, Rating2 = 1.07m, TotalMapsPlayed = 1400, JoinedTeamAtUtc = new DateTime(2025, 3, 15, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 58, TeamId = 13, Nickname = "fame", FullName = "Petr Bolyshev", CountryCode = "RU", DateOfBirth = new DateTime(2002, 3, 26), Role = PlayerRole.EntryFragger, Rating2 = 1.03m, TotalMapsPlayed = 1160, JoinedTeamAtUtc = new DateTime(2025, 3, 15, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 59, TeamId = 13, Nickname = "ICY", FullName = "Ilya Ospennikov", CountryCode = "RU", DateOfBirth = new DateTime(2005, 8, 22), Role = PlayerRole.Awper, Rating2 = 1.08m, TotalMapsPlayed = 580, JoinedTeamAtUtc = new DateTime(2025, 3, 15, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 60, TeamId = 13, Nickname = "nota", FullName = "Yegor Dovbnya", CountryCode = "UA", DateOfBirth = new DateTime(2004, 12, 9), Role = PlayerRole.Support, Rating2 = 1.02m, TotalMapsPlayed = 600, JoinedTeamAtUtc = new DateTime(2025, 3, 15, 12, 0, 0, DateTimeKind.Utc) },

            new Player { Id = 61, TeamId = 14, Nickname = "EliGE", FullName = "Jonathan Jablonowski", CountryCode = "US", DateOfBirth = new DateTime(1997, 7, 16), Role = PlayerRole.Rifler, Rating2 = 1.12m, TotalMapsPlayed = 2280, JoinedTeamAtUtc = new DateTime(2025, 4, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 62, TeamId = 14, Nickname = "Grim", FullName = "Michael Wince", CountryCode = "US", DateOfBirth = new DateTime(2000, 11, 22), Role = PlayerRole.Rifler, Rating2 = 1.06m, TotalMapsPlayed = 1510, JoinedTeamAtUtc = new DateTime(2025, 4, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 63, TeamId = 14, Nickname = "hallzerk", FullName = "Håkon Fjærli", CountryCode = "NO", DateOfBirth = new DateTime(2000, 1, 24), Role = PlayerRole.Awper, Rating2 = 1.03m, TotalMapsPlayed = 1395, JoinedTeamAtUtc = new DateTime(2025, 4, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 64, TeamId = 14, Nickname = "floppy", FullName = "Ricky Kemery", CountryCode = "US", DateOfBirth = new DateTime(1999, 10, 13), Role = PlayerRole.Support, Rating2 = 1.00m, TotalMapsPlayed = 1430, JoinedTeamAtUtc = new DateTime(2025, 4, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 65, TeamId = 14, Nickname = "nicx", FullName = "Nicholas Lee", CountryCode = "US", DateOfBirth = new DateTime(2004, 5, 8), Role = PlayerRole.EntryFragger, Rating2 = 1.04m, TotalMapsPlayed = 520, JoinedTeamAtUtc = new DateTime(2025, 4, 1, 12, 0, 0, DateTimeKind.Utc) },

            new Player { Id = 66, TeamId = 15, Nickname = "insani", FullName = "Lucas Dias", CountryCode = "BR", DateOfBirth = new DateTime(2003, 7, 4), Role = PlayerRole.EntryFragger, Rating2 = 1.11m, TotalMapsPlayed = 780, JoinedTeamAtUtc = new DateTime(2025, 4, 12, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 67, TeamId = 15, Nickname = "saffee", FullName = "Raphael Costa", CountryCode = "BR", DateOfBirth = new DateTime(1998, 2, 1), Role = PlayerRole.Awper, Rating2 = 1.07m, TotalMapsPlayed = 1680, JoinedTeamAtUtc = new DateTime(2025, 4, 12, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 68, TeamId = 15, Nickname = "brnz4n", FullName = "Breno Poletto", CountryCode = "BR", DateOfBirth = new DateTime(2003, 4, 24), Role = PlayerRole.Rifler, Rating2 = 1.05m, TotalMapsPlayed = 910, JoinedTeamAtUtc = new DateTime(2025, 4, 12, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 69, TeamId = 15, Nickname = "drop", FullName = "Bruno Ribeiro", CountryCode = "BR", DateOfBirth = new DateTime(2004, 12, 16), Role = PlayerRole.Support, Rating2 = 1.02m, TotalMapsPlayed = 860, JoinedTeamAtUtc = new DateTime(2025, 4, 12, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 70, TeamId = 15, Nickname = "exit", FullName = "Andre Pereira", CountryCode = "BR", DateOfBirth = new DateTime(1997, 9, 5), Role = PlayerRole.InGameLeader, Rating2 = 1.00m, TotalMapsPlayed = 1740, JoinedTeamAtUtc = new DateTime(2025, 4, 12, 12, 0, 0, DateTimeKind.Utc) },

            new Player { Id = 71, TeamId = null, Nickname = "GeT_RiGhT", FullName = "Christopher Alesund", CountryCode = "SE", DateOfBirth = new DateTime(1990, 5, 29), Role = PlayerRole.Rifler, Rating2 = 0.97m, TotalMapsPlayed = 2450, JoinedTeamAtUtc = new DateTime(2010, 1, 15, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 72, TeamId = null, Nickname = "f0rest", FullName = "Patrik Lindberg", CountryCode = "SE", DateOfBirth = new DateTime(1988, 6, 10), Role = PlayerRole.Rifler, Rating2 = 1.01m, TotalMapsPlayed = 2920, JoinedTeamAtUtc = new DateTime(2009, 8, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 73, TeamId = null, Nickname = "friberg", FullName = "Adam Friberg", CountryCode = "SE", DateOfBirth = new DateTime(1991, 10, 19), Role = PlayerRole.EntryFragger, Rating2 = 0.96m, TotalMapsPlayed = 2180, JoinedTeamAtUtc = new DateTime(2013, 5, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 74, TeamId = null, Nickname = "Xizt", FullName = "Richard Landstrom", CountryCode = "SE", DateOfBirth = new DateTime(1991, 2, 27), Role = PlayerRole.InGameLeader, Rating2 = 0.93m, TotalMapsPlayed = 1880, JoinedTeamAtUtc = new DateTime(2012, 3, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 75, TeamId = null, Nickname = "allu", FullName = "Aleksi Jalli", CountryCode = "FI", DateOfBirth = new DateTime(1992, 5, 15), Role = PlayerRole.Awper, Rating2 = 1.02m, TotalMapsPlayed = 2310, JoinedTeamAtUtc = new DateTime(2014, 6, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 76, TeamId = null, Nickname = "aizy", FullName = "Philip Aistrup", CountryCode = "DK", DateOfBirth = new DateTime(1994, 7, 30), Role = PlayerRole.Rifler, Rating2 = 0.95m, TotalMapsPlayed = 1650, JoinedTeamAtUtc = new DateTime(2015, 2, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 77, TeamId = null, Nickname = "MSL", FullName = "Mathias Lauridsen", CountryCode = "DK", DateOfBirth = new DateTime(1994, 2, 23), Role = PlayerRole.InGameLeader, Rating2 = 0.92m, TotalMapsPlayed = 1725, JoinedTeamAtUtc = new DateTime(2015, 5, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 78, TeamId = null, Nickname = "karrigan", FullName = "Finn Andersen", CountryCode = "DK", DateOfBirth = new DateTime(1990, 4, 14), Role = PlayerRole.InGameLeader, Rating2 = 0.98m, TotalMapsPlayed = 2650, JoinedTeamAtUtc = new DateTime(2010, 1, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 79, TeamId = null, Nickname = "dupreeh", FullName = "Peter Rasmussen", CountryCode = "DK", DateOfBirth = new DateTime(1993, 3, 26), Role = PlayerRole.EntryFragger, Rating2 = 1.03m, TotalMapsPlayed = 2730, JoinedTeamAtUtc = new DateTime(2013, 8, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 80, TeamId = null, Nickname = "magisk", FullName = "Emil Reif", CountryCode = "DK", DateOfBirth = new DateTime(1998, 3, 5), Role = PlayerRole.Rifler, Rating2 = 1.04m, TotalMapsPlayed = 1680, JoinedTeamAtUtc = new DateTime(2017, 1, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 81, TeamId = null, Nickname = "device", FullName = "Nicolai Reedtz", CountryCode = "DK", DateOfBirth = new DateTime(1995, 9, 1), Role = PlayerRole.Awper, Rating2 = 1.08m, TotalMapsPlayed = 2785, JoinedTeamAtUtc = new DateTime(2014, 1, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 82, TeamId = null, Nickname = "Snappi", FullName = "Marco Pfeiffer", CountryCode = "DK", DateOfBirth = new DateTime(1990, 2, 9), Role = PlayerRole.InGameLeader, Rating2 = 0.94m, TotalMapsPlayed = 2105, JoinedTeamAtUtc = new DateTime(2016, 3, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 83, TeamId = null, Nickname = "blameF", FullName = "Benjamin Bremer", CountryCode = "DK", DateOfBirth = new DateTime(1997, 6, 10), Role = PlayerRole.Rifler, Rating2 = 1.07m, TotalMapsPlayed = 1940, JoinedTeamAtUtc = new DateTime(2017, 4, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 84, TeamId = null, Nickname = "stavn", FullName = "Martin Lund", CountryCode = "DK", DateOfBirth = new DateTime(2002, 11, 8), Role = PlayerRole.Rifler, Rating2 = 1.09m, TotalMapsPlayed = 1390, JoinedTeamAtUtc = new DateTime(2019, 7, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 85, TeamId = null, Nickname = "cadiaN", FullName = "Casper Moller", CountryCode = "DK", DateOfBirth = new DateTime(1995, 6, 26), Role = PlayerRole.InGameLeader, Rating2 = 0.99m, TotalMapsPlayed = 2200, JoinedTeamAtUtc = new DateTime(2015, 9, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 86, TeamId = null, Nickname = "shox", FullName = "Richard Papillon", CountryCode = "FR", DateOfBirth = new DateTime(1992, 5, 27), Role = PlayerRole.Rifler, Rating2 = 0.97m, TotalMapsPlayed = 2405, JoinedTeamAtUtc = new DateTime(2014, 2, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 87, TeamId = null, Nickname = "apEX", FullName = "Dan Madesclaire", CountryCode = "FR", DateOfBirth = new DateTime(1993, 2, 22), Role = PlayerRole.InGameLeader, Rating2 = 0.98m, TotalMapsPlayed = 2575, JoinedTeamAtUtc = new DateTime(2012, 7, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 88, TeamId = null, Nickname = "NBK-", FullName = "Nathan Schmitt", CountryCode = "FR", DateOfBirth = new DateTime(1994, 6, 5), Role = PlayerRole.Support, Rating2 = 0.95m, TotalMapsPlayed = 2220, JoinedTeamAtUtc = new DateTime(2013, 3, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 89, TeamId = null, Nickname = "kennyS", FullName = "Kenny Schrub", CountryCode = "FR", DateOfBirth = new DateTime(1995, 5, 19), Role = PlayerRole.Awper, Rating2 = 1.03m, TotalMapsPlayed = 2260, JoinedTeamAtUtc = new DateTime(2014, 4, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 90, TeamId = null, Nickname = "bodyy", FullName = "Cedric Guipouy", CountryCode = "FR", DateOfBirth = new DateTime(1997, 2, 24), Role = PlayerRole.Rifler, Rating2 = 0.94m, TotalMapsPlayed = 1420, JoinedTeamAtUtc = new DateTime(2016, 10, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 91, TeamId = null, Nickname = "Happy", FullName = "Dan Grzesiak", CountryCode = "FR", DateOfBirth = new DateTime(1991, 1, 23), Role = PlayerRole.InGameLeader, Rating2 = 0.91m, TotalMapsPlayed = 1950, JoinedTeamAtUtc = new DateTime(2013, 6, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 92, TeamId = null, Nickname = "GuardiaN", FullName = "Ladislav Kovacs", CountryCode = "SK", DateOfBirth = new DateTime(1991, 7, 9), Role = PlayerRole.Awper, Rating2 = 1.01m, TotalMapsPlayed = 2495, JoinedTeamAtUtc = new DateTime(2013, 1, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 93, TeamId = null, Nickname = "olofmeister", FullName = "Olof Kajbjer", CountryCode = "SE", DateOfBirth = new DateTime(1992, 1, 21), Role = PlayerRole.Rifler, Rating2 = 0.99m, TotalMapsPlayed = 2560, JoinedTeamAtUtc = new DateTime(2014, 9, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 94, TeamId = null, Nickname = "flusha", FullName = "Robin Ronnquist", CountryCode = "SE", DateOfBirth = new DateTime(1993, 8, 14), Role = PlayerRole.Rifler, Rating2 = 0.98m, TotalMapsPlayed = 2480, JoinedTeamAtUtc = new DateTime(2013, 9, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 95, TeamId = null, Nickname = "pronax", FullName = "Markus Wallsten", CountryCode = "SE", DateOfBirth = new DateTime(1990, 5, 24), Role = PlayerRole.InGameLeader, Rating2 = 0.90m, TotalMapsPlayed = 1640, JoinedTeamAtUtc = new DateTime(2013, 10, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 96, TeamId = null, Nickname = "BnTeT", FullName = "Hansel Ferdinand", CountryCode = "ID", DateOfBirth = new DateTime(1995, 1, 28), Role = PlayerRole.Rifler, Rating2 = 1.00m, TotalMapsPlayed = 1695, JoinedTeamAtUtc = new DateTime(2016, 6, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 97, TeamId = null, Nickname = "chrisJ", FullName = "Chris de Jong", CountryCode = "NL", DateOfBirth = new DateTime(1990, 5, 5), Role = PlayerRole.Rifler, Rating2 = 0.95m, TotalMapsPlayed = 2300, JoinedTeamAtUtc = new DateTime(2014, 2, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 98, TeamId = null, Nickname = "Pimp", FullName = "Jacob Winneche", CountryCode = "DK", DateOfBirth = new DateTime(1991, 2, 23), Role = PlayerRole.Support, Rating2 = 0.89m, TotalMapsPlayed = 1120, JoinedTeamAtUtc = new DateTime(2015, 1, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 99, TeamId = null, Nickname = "smooya", FullName = "Owen Butterfield", CountryCode = "GB", DateOfBirth = new DateTime(1999, 10, 16), Role = PlayerRole.Awper, Rating2 = 1.02m, TotalMapsPlayed = 1560, JoinedTeamAtUtc = new DateTime(2018, 1, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Player { Id = 100, TeamId = null, Nickname = "maden", FullName = "Martin Petrov", CountryCode = "ME", DateOfBirth = new DateTime(1998, 12, 8), Role = PlayerRole.EntryFragger, Rating2 = 1.00m, TotalMapsPlayed = 1480, JoinedTeamAtUtc = new DateTime(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc) }
        };
    }

    public static Match[] GetMatches()
    {
        return new[]
        {
            new Match { Id = 1, EventId = 6, TeamAId = 8, TeamBId = 9, ScheduledAtUtc = new DateTime(2026, 6, 3, 14, 0, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = false, TeamAScore = 0, TeamBScore = 0 },
            new Match { Id = 2, EventId = 6, TeamAId = 10, TeamBId = 12, ScheduledAtUtc = new DateTime(2026, 6, 4, 12, 30, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf1, IsFinished = false, TeamAScore = 0, TeamBScore = 0 },
            new Match { Id = 3, EventId = 2, TeamAId = 11, TeamBId = 13, ScheduledAtUtc = new DateTime(2026, 7, 21, 16, 0, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = false, TeamAScore = 0, TeamBScore = 0 },
            new Match { Id = 4, EventId = 2, TeamAId = 14, TeamBId = 15, ScheduledAtUtc = new DateTime(2026, 7, 22, 18, 0, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = false, TeamAScore = 0, TeamBScore = 0 },
            new Match { Id = 5, EventId = 7, TeamAId = 1, TeamBId = 2, ScheduledAtUtc = new DateTime(2026, 9, 22, 13, 0, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf5, IsFinished = false, TeamAScore = 0, TeamBScore = 0 },

            new Match { Id = 6, EventId = 1, TeamAId = 1, TeamBId = 2, ScheduledAtUtc = new DateTime(2026, 2, 10, 18, 30, 0, DateTimeKind.Utc), FinishedAtUtc = new DateTime(2026, 2, 10, 21, 10, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = true, TeamAScore = 2, TeamBScore = 1 },
            new Match { Id = 7, EventId = 4, TeamAId = 3, TeamBId = 6, ScheduledAtUtc = new DateTime(2026, 3, 16, 17, 0, 0, DateTimeKind.Utc), FinishedAtUtc = new DateTime(2026, 3, 16, 19, 20, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = true, TeamAScore = 2, TeamBScore = 0 },
            new Match { Id = 8, EventId = 5, TeamAId = 5, TeamBId = 4, ScheduledAtUtc = new DateTime(2026, 4, 10, 19, 0, 0, DateTimeKind.Utc), FinishedAtUtc = new DateTime(2026, 4, 10, 23, 5, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf5, IsFinished = true, TeamAScore = 3, TeamBScore = 2 },
            new Match { Id = 9, EventId = 1, TeamAId = 8, TeamBId = 9, ScheduledAtUtc = new DateTime(2026, 2, 11, 13, 0, 0, DateTimeKind.Utc), FinishedAtUtc = new DateTime(2026, 2, 11, 15, 25, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = true, TeamAScore = 2, TeamBScore = 1 },
            new Match { Id = 10, EventId = 1, TeamAId = 10, TeamBId = 11, ScheduledAtUtc = new DateTime(2026, 2, 12, 14, 30, 0, DateTimeKind.Utc), FinishedAtUtc = new DateTime(2026, 2, 12, 16, 10, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = true, TeamAScore = 2, TeamBScore = 0 },
            new Match { Id = 11, EventId = 1, TeamAId = 12, TeamBId = 1, ScheduledAtUtc = new DateTime(2026, 2, 13, 17, 0, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = false, TeamAScore = 0, TeamBScore = 0 },
            new Match { Id = 12, EventId = 1, TeamAId = 2, TeamBId = 3, ScheduledAtUtc = new DateTime(2026, 2, 14, 18, 0, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = false, TeamAScore = 0, TeamBScore = 0 },
            new Match { Id = 13, EventId = 1, TeamAId = 4, TeamBId = 5, ScheduledAtUtc = new DateTime(2026, 2, 14, 20, 0, 0, DateTimeKind.Utc), FinishedAtUtc = new DateTime(2026, 2, 14, 22, 50, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf5, IsFinished = true, TeamAScore = 3, TeamBScore = 2 },
            new Match { Id = 14, EventId = 1, TeamAId = 6, TeamBId = 7, ScheduledAtUtc = new DateTime(2026, 2, 15, 15, 0, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf1, IsFinished = false, TeamAScore = 0, TeamBScore = 0 },

            new Match { Id = 15, EventId = 2, TeamAId = 3, TeamBId = 4, ScheduledAtUtc = new DateTime(2026, 7, 18, 14, 0, 0, DateTimeKind.Utc), FinishedAtUtc = new DateTime(2026, 7, 18, 16, 20, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = true, TeamAScore = 2, TeamBScore = 0 },
            new Match { Id = 16, EventId = 2, TeamAId = 5, TeamBId = 6, ScheduledAtUtc = new DateTime(2026, 7, 19, 12, 0, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = false, TeamAScore = 0, TeamBScore = 0 },
            new Match { Id = 17, EventId = 2, TeamAId = 8, TeamBId = 10, ScheduledAtUtc = new DateTime(2026, 7, 19, 18, 0, 0, DateTimeKind.Utc), FinishedAtUtc = new DateTime(2026, 7, 19, 20, 15, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = true, TeamAScore = 2, TeamBScore = 1 },
            new Match { Id = 18, EventId = 2, TeamAId = 9, TeamBId = 11, ScheduledAtUtc = new DateTime(2026, 7, 20, 15, 30, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = false, TeamAScore = 0, TeamBScore = 0 },

            new Match { Id = 19, EventId = 3, TeamAId = 1, TeamBId = 8, ScheduledAtUtc = new DateTime(2026, 12, 11, 13, 0, 0, DateTimeKind.Utc), FinishedAtUtc = new DateTime(2026, 12, 11, 15, 35, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = true, TeamAScore = 2, TeamBScore = 1 },
            new Match { Id = 20, EventId = 3, TeamAId = 2, TeamBId = 9, ScheduledAtUtc = new DateTime(2026, 12, 12, 16, 0, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = false, TeamAScore = 0, TeamBScore = 0 },
            new Match { Id = 21, EventId = 3, TeamAId = 3, TeamBId = 10, ScheduledAtUtc = new DateTime(2026, 12, 12, 18, 0, 0, DateTimeKind.Utc), FinishedAtUtc = new DateTime(2026, 12, 12, 19, 40, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = true, TeamAScore = 2, TeamBScore = 0 },
            new Match { Id = 22, EventId = 3, TeamAId = 4, TeamBId = 12, ScheduledAtUtc = new DateTime(2026, 12, 13, 14, 30, 0, DateTimeKind.Utc), FinishedAtUtc = new DateTime(2026, 12, 13, 17, 10, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = true, TeamAScore = 2, TeamBScore = 1 },

            new Match { Id = 23, EventId = 4, TeamAId = 1, TeamBId = 2, ScheduledAtUtc = new DateTime(2026, 3, 14, 13, 0, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = false, TeamAScore = 0, TeamBScore = 0 },
            new Match { Id = 24, EventId = 4, TeamAId = 7, TeamBId = 8, ScheduledAtUtc = new DateTime(2026, 3, 15, 16, 0, 0, DateTimeKind.Utc), FinishedAtUtc = new DateTime(2026, 3, 15, 18, 45, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = true, TeamAScore = 2, TeamBScore = 1 },

            new Match { Id = 25, EventId = 5, TeamAId = 4, TeamBId = 5, ScheduledAtUtc = new DateTime(2026, 4, 9, 14, 30, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = false, TeamAScore = 0, TeamBScore = 0 },
            new Match { Id = 26, EventId = 5, TeamAId = 6, TeamBId = 8, ScheduledAtUtc = new DateTime(2026, 4, 10, 16, 0, 0, DateTimeKind.Utc), FinishedAtUtc = new DateTime(2026, 4, 10, 18, 10, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = true, TeamAScore = 2, TeamBScore = 0 },

            new Match { Id = 27, EventId = 6, TeamAId = 1, TeamBId = 8, ScheduledAtUtc = new DateTime(2026, 6, 5, 15, 0, 0, DateTimeKind.Utc), FinishedAtUtc = new DateTime(2026, 6, 5, 17, 20, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = true, TeamAScore = 2, TeamBScore = 1 },
            new Match { Id = 28, EventId = 6, TeamAId = 2, TeamBId = 9, ScheduledAtUtc = new DateTime(2026, 6, 5, 18, 0, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = false, TeamAScore = 0, TeamBScore = 0 },

            new Match { Id = 29, EventId = 7, TeamAId = 3, TeamBId = 5, ScheduledAtUtc = new DateTime(2026, 9, 20, 13, 0, 0, DateTimeKind.Utc), FinishedAtUtc = new DateTime(2026, 9, 20, 15, 30, 0, DateTimeKind.Utc), Format = MatchFormat.BestOf3, IsFinished = true, TeamAScore = 2, TeamBScore = 0 }
        };
    }

    public static MatchMap[] GetMatchMaps()
    {
        return new[]
        {
            new MatchMap { Id = 1, MatchId = 6, MapSequence = 1, Map = MapPool.Ancient, TeamAScore = 13, TeamBScore = 12, WentToOvertime = true },
            new MatchMap { Id = 2, MatchId = 6, MapSequence = 2, Map = MapPool.Mirage, TeamAScore = 10, TeamBScore = 13, WentToOvertime = false },
            new MatchMap { Id = 3, MatchId = 6, MapSequence = 3, Map = MapPool.Nuke, TeamAScore = 13, TeamBScore = 9, WentToOvertime = false },

            new MatchMap { Id = 4, MatchId = 7, MapSequence = 1, Map = MapPool.Inferno, TeamAScore = 13, TeamBScore = 8, WentToOvertime = false },
            new MatchMap { Id = 5, MatchId = 7, MapSequence = 2, Map = MapPool.Anubis, TeamAScore = 13, TeamBScore = 11, WentToOvertime = false },

            new MatchMap { Id = 6, MatchId = 8, MapSequence = 1, Map = MapPool.Mirage, TeamAScore = 13, TeamBScore = 11, WentToOvertime = false },
            new MatchMap { Id = 7, MatchId = 8, MapSequence = 2, Map = MapPool.Ancient, TeamAScore = 8, TeamBScore = 13, WentToOvertime = false },
            new MatchMap { Id = 8, MatchId = 8, MapSequence = 3, Map = MapPool.Inferno, TeamAScore = 13, TeamBScore = 9, WentToOvertime = false },
            new MatchMap { Id = 9, MatchId = 8, MapSequence = 4, Map = MapPool.Nuke, TeamAScore = 9, TeamBScore = 13, WentToOvertime = false },
            new MatchMap { Id = 10, MatchId = 8, MapSequence = 5, Map = MapPool.Dust2, TeamAScore = 13, TeamBScore = 10, WentToOvertime = false },

            new MatchMap { Id = 11, MatchId = 9, MapSequence = 1, Map = MapPool.Ancient, TeamAScore = 13, TeamBScore = 10, WentToOvertime = false },
            new MatchMap { Id = 12, MatchId = 9, MapSequence = 2, Map = MapPool.Mirage, TeamAScore = 11, TeamBScore = 13, WentToOvertime = false },
            new MatchMap { Id = 13, MatchId = 9, MapSequence = 3, Map = MapPool.Inferno, TeamAScore = 13, TeamBScore = 8, WentToOvertime = false },

            new MatchMap { Id = 14, MatchId = 10, MapSequence = 1, Map = MapPool.Anubis, TeamAScore = 13, TeamBScore = 6, WentToOvertime = false },
            new MatchMap { Id = 15, MatchId = 10, MapSequence = 2, Map = MapPool.Nuke, TeamAScore = 13, TeamBScore = 11, WentToOvertime = false },

            new MatchMap { Id = 16, MatchId = 13, MapSequence = 1, Map = MapPool.Mirage, TeamAScore = 13, TeamBScore = 9, WentToOvertime = false },
            new MatchMap { Id = 17, MatchId = 13, MapSequence = 2, Map = MapPool.Ancient, TeamAScore = 9, TeamBScore = 13, WentToOvertime = false },
            new MatchMap { Id = 18, MatchId = 13, MapSequence = 3, Map = MapPool.Nuke, TeamAScore = 13, TeamBScore = 11, WentToOvertime = false },
            new MatchMap { Id = 19, MatchId = 13, MapSequence = 4, Map = MapPool.Dust2, TeamAScore = 10, TeamBScore = 13, WentToOvertime = false },
            new MatchMap { Id = 20, MatchId = 13, MapSequence = 5, Map = MapPool.Anubis, TeamAScore = 13, TeamBScore = 7, WentToOvertime = false },

            new MatchMap { Id = 21, MatchId = 15, MapSequence = 1, Map = MapPool.Inferno, TeamAScore = 13, TeamBScore = 8, WentToOvertime = false },
            new MatchMap { Id = 22, MatchId = 15, MapSequence = 2, Map = MapPool.Mirage, TeamAScore = 13, TeamBScore = 10, WentToOvertime = false },

            new MatchMap { Id = 23, MatchId = 17, MapSequence = 1, Map = MapPool.Ancient, TeamAScore = 11, TeamBScore = 13, WentToOvertime = false },
            new MatchMap { Id = 24, MatchId = 17, MapSequence = 2, Map = MapPool.Nuke, TeamAScore = 13, TeamBScore = 7, WentToOvertime = false },
            new MatchMap { Id = 25, MatchId = 17, MapSequence = 3, Map = MapPool.Anubis, TeamAScore = 13, TeamBScore = 9, WentToOvertime = false },

            new MatchMap { Id = 26, MatchId = 19, MapSequence = 1, Map = MapPool.Mirage, TeamAScore = 13, TeamBScore = 5, WentToOvertime = false },
            new MatchMap { Id = 27, MatchId = 19, MapSequence = 2, Map = MapPool.Ancient, TeamAScore = 10, TeamBScore = 13, WentToOvertime = false },
            new MatchMap { Id = 28, MatchId = 19, MapSequence = 3, Map = MapPool.Dust2, TeamAScore = 13, TeamBScore = 11, WentToOvertime = false },

            new MatchMap { Id = 29, MatchId = 21, MapSequence = 1, Map = MapPool.Inferno, TeamAScore = 13, TeamBScore = 9, WentToOvertime = false },
            new MatchMap { Id = 30, MatchId = 21, MapSequence = 2, Map = MapPool.Anubis, TeamAScore = 13, TeamBScore = 11, WentToOvertime = false },

            new MatchMap { Id = 31, MatchId = 22, MapSequence = 1, Map = MapPool.Nuke, TeamAScore = 9, TeamBScore = 13, WentToOvertime = false },
            new MatchMap { Id = 32, MatchId = 22, MapSequence = 2, Map = MapPool.Mirage, TeamAScore = 13, TeamBScore = 8, WentToOvertime = false },
            new MatchMap { Id = 33, MatchId = 22, MapSequence = 3, Map = MapPool.Inferno, TeamAScore = 13, TeamBScore = 7, WentToOvertime = false },

            new MatchMap { Id = 34, MatchId = 24, MapSequence = 1, Map = MapPool.Ancient, TeamAScore = 13, TeamBScore = 11, WentToOvertime = false },
            new MatchMap { Id = 35, MatchId = 24, MapSequence = 2, Map = MapPool.Mirage, TeamAScore = 10, TeamBScore = 13, WentToOvertime = false },
            new MatchMap { Id = 36, MatchId = 24, MapSequence = 3, Map = MapPool.Anubis, TeamAScore = 13, TeamBScore = 9, WentToOvertime = false },

            new MatchMap { Id = 37, MatchId = 26, MapSequence = 1, Map = MapPool.Nuke, TeamAScore = 13, TeamBScore = 7, WentToOvertime = false },
            new MatchMap { Id = 38, MatchId = 26, MapSequence = 2, Map = MapPool.Dust2, TeamAScore = 13, TeamBScore = 12, WentToOvertime = false },

            new MatchMap { Id = 39, MatchId = 27, MapSequence = 1, Map = MapPool.Anubis, TeamAScore = 13, TeamBScore = 9, WentToOvertime = false },
            new MatchMap { Id = 40, MatchId = 27, MapSequence = 2, Map = MapPool.Mirage, TeamAScore = 11, TeamBScore = 13, WentToOvertime = false },
            new MatchMap { Id = 41, MatchId = 27, MapSequence = 3, Map = MapPool.Inferno, TeamAScore = 13, TeamBScore = 10, WentToOvertime = false },

            new MatchMap { Id = 42, MatchId = 29, MapSequence = 1, Map = MapPool.Ancient, TeamAScore = 13, TeamBScore = 8, WentToOvertime = false },
            new MatchMap { Id = 43, MatchId = 29, MapSequence = 2, Map = MapPool.Nuke, TeamAScore = 13, TeamBScore = 6, WentToOvertime = false }
        };
    }
}