using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace cs2_esports.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchesAndMoreTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsFinished = table.Column<bool>(type: "bit", nullable: false),
                    Format = table.Column<int>(type: "int", nullable: false),
                    TeamAScore = table.Column<int>(type: "int", nullable: false),
                    TeamBScore = table.Column<int>(type: "int", nullable: false),
                    FinishedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    TeamAId = table.Column<int>(type: "int", nullable: false),
                    TeamBId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Teams_TeamAId",
                        column: x => x.TeamAId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Teams_TeamBId",
                        column: x => x.TeamBId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Tournaments_EventId",
                        column: x => x.EventId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchMaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MapSequence = table.Column<int>(type: "int", nullable: false),
                    Map = table.Column<int>(type: "int", nullable: false),
                    TeamAScore = table.Column<int>(type: "int", nullable: false),
                    TeamBScore = table.Column<int>(type: "int", nullable: false),
                    WentToOvertime = table.Column<bool>(type: "bit", nullable: false),
                    MatchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchMaps_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Matches",
                columns: new[] { "Id", "EventId", "FinishedAtUtc", "Format", "IsFinished", "ScheduledAtUtc", "TeamAId", "TeamAScore", "TeamBId", "TeamBScore" },
                values: new object[,]
                {
                    { 5, 7, null, 5, false, new DateTime(2026, 9, 22, 13, 0, 0, 0, DateTimeKind.Utc), 1, 0, 2, 0 },
                    { 6, 1, new DateTime(2026, 2, 10, 21, 10, 0, 0, DateTimeKind.Utc), 3, true, new DateTime(2026, 2, 10, 18, 30, 0, 0, DateTimeKind.Utc), 1, 2, 2, 1 },
                    { 7, 4, new DateTime(2026, 3, 16, 19, 20, 0, 0, DateTimeKind.Utc), 3, true, new DateTime(2026, 3, 16, 17, 0, 0, 0, DateTimeKind.Utc), 3, 2, 6, 0 },
                    { 8, 5, new DateTime(2026, 4, 10, 23, 5, 0, 0, DateTimeKind.Utc), 5, true, new DateTime(2026, 4, 10, 19, 0, 0, 0, DateTimeKind.Utc), 5, 3, 4, 2 }
                });

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "CountryCode", "FoundedYear", "LastRosterUpdateUtc", "Name", "PrizeMoneyUsd", "Tag", "WorldRanking" },
                values: new object[,]
                {
                    { 8, "MN", 2013, new DateTime(2026, 1, 18, 12, 0, 0, 0, DateTimeKind.Utc), "The MongolZ", 1850000m, "MGLZ", 8 },
                    { 9, "US", 2000, new DateTime(2026, 1, 22, 12, 0, 0, 0, DateTimeKind.Utc), "Team Liquid", 1760000m, "TL", 9 },
                    { 10, "TR", 2020, new DateTime(2026, 2, 2, 12, 0, 0, 0, DateTimeKind.Utc), "Aurora", 1420000m, "AUR", 10 },
                    { 11, "DK", 2016, new DateTime(2026, 2, 8, 12, 0, 0, 0, DateTimeKind.Utc), "HEROIC", 1390000m, "HERO", 11 },
                    { 12, "BR", 2017, new DateTime(2026, 2, 14, 12, 0, 0, 0, DateTimeKind.Utc), "FURIA", 1680000m, "FUR", 12 },
                    { 13, "RU", 2003, new DateTime(2026, 2, 18, 12, 0, 0, 0, DateTimeKind.Utc), "Virtus.pro", 1540000m, "VP", 13 },
                    { 14, "US", 2003, new DateTime(2026, 2, 25, 12, 0, 0, 0, DateTimeKind.Utc), "Complexity", 1180000m, "COL", 14 },
                    { 15, "BR", 2003, new DateTime(2026, 3, 3, 12, 0, 0, 0, DateTimeKind.Utc), "MIBR", 1120000m, "MIBR", 15 }
                });

            migrationBuilder.InsertData(
                table: "MatchMaps",
                columns: new[] { "Id", "Map", "MapSequence", "MatchId", "TeamAScore", "TeamBScore", "WentToOvertime" },
                values: new object[,]
                {
                    { 1, 1, 1, 6, 13, 12, true },
                    { 2, 2, 2, 6, 10, 13, false },
                    { 3, 5, 3, 6, 13, 9, false },
                    { 4, 3, 1, 7, 13, 8, false },
                    { 5, 4, 2, 7, 13, 11, false },
                    { 6, 2, 1, 8, 13, 11, false },
                    { 7, 1, 2, 8, 8, 13, false },
                    { 8, 3, 3, 8, 13, 9, false },
                    { 9, 5, 4, 8, 9, 13, false },
                    { 10, 6, 5, 8, 13, 10, false }
                });

            migrationBuilder.InsertData(
                table: "Matches",
                columns: new[] { "Id", "EventId", "FinishedAtUtc", "Format", "IsFinished", "ScheduledAtUtc", "TeamAId", "TeamAScore", "TeamBId", "TeamBScore" },
                values: new object[,]
                {
                    { 1, 6, null, 3, false, new DateTime(2026, 6, 3, 14, 0, 0, 0, DateTimeKind.Utc), 8, 0, 9, 0 },
                    { 2, 6, null, 1, false, new DateTime(2026, 6, 4, 12, 30, 0, 0, DateTimeKind.Utc), 10, 0, 12, 0 },
                    { 3, 2, null, 3, false, new DateTime(2026, 7, 21, 16, 0, 0, 0, DateTimeKind.Utc), 11, 0, 13, 0 },
                    { 4, 2, null, 3, false, new DateTime(2026, 7, 22, 18, 0, 0, 0, DateTimeKind.Utc), 14, 0, 15, 0 }
                });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "Id", "CountryCode", "DateOfBirth", "FullName", "JoinedTeamAtUtc", "Nickname", "Rating2", "Role", "TeamId", "TotalMapsPlayed" },
                values: new object[,]
                {
                    { 31, "MN", new DateTime(1999, 10, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Byambasuren Garidmagnai", new DateTime(2024, 11, 15, 12, 0, 0, 0, DateTimeKind.Utc), "bLitz", 1.06m, 3, 8, 1140 },
                    { 32, "MN", new DateTime(2002, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ayush Batbold", new DateTime(2024, 11, 15, 12, 0, 0, 0, DateTimeKind.Utc), "910", 1.14m, 4, 8, 980 },
                    { 33, "MN", new DateTime(2005, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sodbayar Munkhbold", new DateTime(2024, 11, 15, 12, 0, 0, 0, DateTimeKind.Utc), "mzinho", 1.10m, 1, 8, 760 },
                    { 34, "MN", new DateTime(2003, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Munkhbold Enkhbat", new DateTime(2024, 11, 15, 12, 0, 0, 0, DateTimeKind.Utc), "Techno4K", 1.09m, 1, 8, 830 },
                    { 35, "MN", new DateTime(2005, 1, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Garidmagnai Baatarkhuu", new DateTime(2024, 11, 15, 12, 0, 0, 0, DateTimeKind.Utc), "Senzu", 1.13m, 2, 8, 790 },
                    { 36, "CA", new DateTime(1997, 11, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Keith Markovic", new DateTime(2025, 1, 12, 12, 0, 0, 0, DateTimeKind.Utc), "NAF", 1.12m, 1, 9, 1800 },
                    { 37, "CA", new DateTime(1999, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Russel Van Dulken", new DateTime(2025, 1, 12, 12, 0, 0, 0, DateTimeKind.Utc), "Twistzz", 1.15m, 1, 9, 1720 },
                    { 38, "IL", new DateTime(1999, 9, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Guy Iluz", new DateTime(2025, 1, 12, 12, 0, 0, 0, DateTimeKind.Utc), "Nertz", 1.11m, 1, 9, 1110 },
                    { 39, "PL", new DateTime(2003, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Roland Tomkowiak", new DateTime(2025, 1, 12, 12, 0, 0, 0, DateTimeKind.Utc), "ultimate", 1.07m, 2, 9, 940 },
                    { 40, "AU", new DateTime(1995, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Justin Savage", new DateTime(2025, 1, 12, 12, 0, 0, 0, DateTimeKind.Utc), "jks", 1.08m, 5, 9, 1920 },
                    { 41, "TR", new DateTime(1995, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ismailcan Dörtkardeş", new DateTime(2025, 2, 4, 12, 0, 0, 0, DateTimeKind.Utc), "XANTARES", 1.19m, 1, 10, 1950 },
                    { 42, "TR", new DateTime(1998, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Özgür Eker", new DateTime(2025, 2, 4, 12, 0, 0, 0, DateTimeKind.Utc), "woxic", 1.12m, 2, 10, 1885 },
                    { 43, "TR", new DateTime(1990, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Engin Küpeli", new DateTime(2025, 2, 4, 12, 0, 0, 0, DateTimeKind.Utc), "MAJ3R", 1.00m, 3, 10, 1680 },
                    { 44, "TR", new DateTime(2005, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ali Haydar Yalçın", new DateTime(2025, 2, 4, 12, 0, 0, 0, DateTimeKind.Utc), "Wicadia", 1.09m, 4, 10, 860 },
                    { 45, "TR", new DateTime(2003, 8, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jotaro Atmaca", new DateTime(2025, 2, 4, 12, 0, 0, 0, DateTimeKind.Utc), "jottAAA", 1.05m, 5, 10, 920 },
                    { 46, "BY", new DateTime(2003, 7, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Andrey Tatarinovich", new DateTime(2025, 3, 1, 12, 0, 0, 0, DateTimeKind.Utc), "tN1R", 1.10m, 1, 11, 820 },
                    { 47, "DK", new DateTime(1999, 9, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rasmus Beck", new DateTime(2025, 3, 1, 12, 0, 0, 0, DateTimeKind.Utc), "sjuush", 1.02m, 5, 11, 1380 },
                    { 48, "DK", new DateTime(2000, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nico Tamjidi", new DateTime(2025, 3, 1, 12, 0, 0, 0, DateTimeKind.Utc), "nicoodoz", 1.08m, 2, 11, 1280 },
                    { 49, "RU", new DateTime(2004, 9, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Maksim Lukin", new DateTime(2025, 3, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Alkaren", 1.04m, 4, 11, 640 },
                    { 50, "FI", new DateTime(2004, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nilo Ojala", new DateTime(2025, 3, 1, 12, 0, 0, 0, DateTimeKind.Utc), "nilo", 1.03m, 5, 11, 680 },
                    { 51, "BR", new DateTime(1991, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gabriel Toledo", new DateTime(2025, 3, 10, 12, 0, 0, 0, DateTimeKind.Utc), "FalleN", 1.04m, 3, 12, 2140 },
                    { 52, "BR", new DateTime(1999, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kaike Cerato", new DateTime(2025, 3, 10, 12, 0, 0, 0, DateTimeKind.Utc), "KSCERATO", 1.18m, 1, 12, 2050 },
                    { 53, "BR", new DateTime(1999, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Yuri Santos", new DateTime(2025, 3, 10, 12, 0, 0, 0, DateTimeKind.Utc), "yuurih", 1.14m, 1, 12, 1985 },
                    { 54, "KZ", new DateTime(2006, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Danil Golubenko", new DateTime(2025, 3, 10, 12, 0, 0, 0, DateTimeKind.Utc), "molodoy", 1.06m, 4, 12, 420 },
                    { 55, "BR", new DateTime(2000, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gabriel Wilhelm", new DateTime(2025, 3, 10, 12, 0, 0, 0, DateTimeKind.Utc), "yel", 1.01m, 5, 12, 760 },
                    { 56, "RU", new DateTime(1998, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Denis Sharipov", new DateTime(2025, 3, 15, 12, 0, 0, 0, DateTimeKind.Utc), "electroNic", 1.05m, 3, 13, 2055 },
                    { 57, "RU", new DateTime(2000, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Evgenii Lebedev", new DateTime(2025, 3, 15, 12, 0, 0, 0, DateTimeKind.Utc), "FL1T", 1.07m, 1, 13, 1400 },
                    { 58, "RU", new DateTime(2002, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Petr Bolyshev", new DateTime(2025, 3, 15, 12, 0, 0, 0, DateTimeKind.Utc), "fame", 1.03m, 4, 13, 1160 },
                    { 59, "RU", new DateTime(2005, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ilya Ospennikov", new DateTime(2025, 3, 15, 12, 0, 0, 0, DateTimeKind.Utc), "ICY", 1.08m, 2, 13, 580 },
                    { 60, "UA", new DateTime(2004, 12, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Yegor Dovbnya", new DateTime(2025, 3, 15, 12, 0, 0, 0, DateTimeKind.Utc), "nota", 1.02m, 5, 13, 600 },
                    { 61, "US", new DateTime(1997, 7, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jonathan Jablonowski", new DateTime(2025, 4, 1, 12, 0, 0, 0, DateTimeKind.Utc), "EliGE", 1.12m, 1, 14, 2280 },
                    { 62, "US", new DateTime(2000, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Michael Wince", new DateTime(2025, 4, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Grim", 1.06m, 1, 14, 1510 },
                    { 63, "NO", new DateTime(2000, 1, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Håkon Fjærli", new DateTime(2025, 4, 1, 12, 0, 0, 0, DateTimeKind.Utc), "hallzerk", 1.03m, 2, 14, 1395 },
                    { 64, "US", new DateTime(1999, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ricky Kemery", new DateTime(2025, 4, 1, 12, 0, 0, 0, DateTimeKind.Utc), "floppy", 1.00m, 5, 14, 1430 },
                    { 65, "US", new DateTime(2004, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nicholas Lee", new DateTime(2025, 4, 1, 12, 0, 0, 0, DateTimeKind.Utc), "nicx", 1.04m, 4, 14, 520 },
                    { 66, "BR", new DateTime(2003, 7, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lucas Dias", new DateTime(2025, 4, 12, 12, 0, 0, 0, DateTimeKind.Utc), "insani", 1.11m, 4, 15, 780 },
                    { 67, "BR", new DateTime(1998, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Raphael Costa", new DateTime(2025, 4, 12, 12, 0, 0, 0, DateTimeKind.Utc), "saffee", 1.07m, 2, 15, 1680 },
                    { 68, "BR", new DateTime(2003, 4, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Breno Poletto", new DateTime(2025, 4, 12, 12, 0, 0, 0, DateTimeKind.Utc), "brnz4n", 1.05m, 1, 15, 910 },
                    { 69, "BR", new DateTime(2004, 12, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bruno Ribeiro", new DateTime(2025, 4, 12, 12, 0, 0, 0, DateTimeKind.Utc), "drop", 1.02m, 5, 15, 860 },
                    { 70, "BR", new DateTime(1997, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "André Pereira", new DateTime(2025, 4, 12, 12, 0, 0, 0, DateTimeKind.Utc), "exit", 1.00m, 3, 15, 1740 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_EventId_ScheduledAtUtc",
                table: "Matches",
                columns: new[] { "EventId", "ScheduledAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_TeamAId",
                table: "Matches",
                column: "TeamAId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_TeamBId",
                table: "Matches",
                column: "TeamBId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchMaps_MatchId_MapSequence",
                table: "MatchMaps",
                columns: new[] { "MatchId", "MapSequence" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchMaps");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 15);
        }
    }
}
