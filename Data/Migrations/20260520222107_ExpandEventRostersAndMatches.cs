using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace cs2_esports.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExpandEventRostersAndMatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EventTeams",
                columns: new[] { "TeamsId", "TournamentsId" },
                values: new object[,]
                {
                    { 1, 4 },
                    { 1, 6 },
                    { 2, 5 },
                    { 2, 6 },
                    { 3, 5 },
                    { 3, 7 },
                    { 4, 4 },
                    { 4, 6 },
                    { 5, 5 },
                    { 5, 7 },
                    { 6, 6 },
                    { 6, 7 },
                    { 7, 1 },
                    { 7, 2 },
                    { 7, 3 },
                    { 7, 4 },
                    { 7, 5 },
                    { 7, 6 },
                    { 7, 7 },
                    { 8, 1 },
                    { 8, 2 },
                    { 8, 3 },
                    { 8, 4 },
                    { 8, 5 },
                    { 8, 6 },
                    { 8, 7 },
                    { 9, 1 },
                    { 9, 2 },
                    { 9, 3 },
                    { 10, 1 },
                    { 10, 2 },
                    { 10, 3 },
                    { 11, 1 },
                    { 11, 2 },
                    { 11, 3 },
                    { 12, 1 },
                    { 12, 2 },
                    { 12, 3 }
                });

            migrationBuilder.InsertData(
                table: "Matches",
                columns: new[] { "Id", "EventId", "FinishedAtUtc", "Format", "IsFinished", "ScheduledAtUtc", "TeamAId", "TeamAScore", "TeamBId", "TeamBScore" },
                values: new object[,]
                {
                    { 9, 1, new DateTime(2026, 2, 11, 15, 25, 0, 0, DateTimeKind.Utc), 3, true, new DateTime(2026, 2, 11, 13, 0, 0, 0, DateTimeKind.Utc), 8, 2, 9, 1 },
                    { 10, 1, new DateTime(2026, 2, 12, 16, 10, 0, 0, DateTimeKind.Utc), 3, true, new DateTime(2026, 2, 12, 14, 30, 0, 0, DateTimeKind.Utc), 10, 2, 11, 0 },
                    { 11, 1, null, 3, false, new DateTime(2026, 2, 13, 17, 0, 0, 0, DateTimeKind.Utc), 12, 0, 1, 0 },
                    { 12, 1, null, 3, false, new DateTime(2026, 2, 14, 18, 0, 0, 0, DateTimeKind.Utc), 2, 0, 3, 0 },
                    { 13, 1, new DateTime(2026, 2, 14, 22, 50, 0, 0, DateTimeKind.Utc), 5, true, new DateTime(2026, 2, 14, 20, 0, 0, 0, DateTimeKind.Utc), 4, 3, 5, 2 },
                    { 14, 1, null, 1, false, new DateTime(2026, 2, 15, 15, 0, 0, 0, DateTimeKind.Utc), 6, 0, 7, 0 },
                    { 15, 2, new DateTime(2026, 7, 18, 16, 20, 0, 0, DateTimeKind.Utc), 3, true, new DateTime(2026, 7, 18, 14, 0, 0, 0, DateTimeKind.Utc), 3, 2, 4, 0 },
                    { 16, 2, null, 3, false, new DateTime(2026, 7, 19, 12, 0, 0, 0, DateTimeKind.Utc), 5, 0, 6, 0 },
                    { 17, 2, new DateTime(2026, 7, 19, 20, 15, 0, 0, DateTimeKind.Utc), 3, true, new DateTime(2026, 7, 19, 18, 0, 0, 0, DateTimeKind.Utc), 8, 2, 10, 1 },
                    { 18, 2, null, 3, false, new DateTime(2026, 7, 20, 15, 30, 0, 0, DateTimeKind.Utc), 9, 0, 11, 0 },
                    { 19, 3, new DateTime(2026, 12, 11, 15, 35, 0, 0, DateTimeKind.Utc), 3, true, new DateTime(2026, 12, 11, 13, 0, 0, 0, DateTimeKind.Utc), 1, 2, 8, 1 },
                    { 20, 3, null, 3, false, new DateTime(2026, 12, 12, 16, 0, 0, 0, DateTimeKind.Utc), 2, 0, 9, 0 },
                    { 21, 3, new DateTime(2026, 12, 12, 19, 40, 0, 0, DateTimeKind.Utc), 3, true, new DateTime(2026, 12, 12, 18, 0, 0, 0, DateTimeKind.Utc), 3, 2, 10, 0 },
                    { 22, 3, new DateTime(2026, 12, 13, 17, 10, 0, 0, DateTimeKind.Utc), 3, true, new DateTime(2026, 12, 13, 14, 30, 0, 0, DateTimeKind.Utc), 4, 2, 12, 1 },
                    { 23, 4, null, 3, false, new DateTime(2026, 3, 14, 13, 0, 0, 0, DateTimeKind.Utc), 1, 0, 2, 0 },
                    { 24, 4, new DateTime(2026, 3, 15, 18, 45, 0, 0, DateTimeKind.Utc), 3, true, new DateTime(2026, 3, 15, 16, 0, 0, 0, DateTimeKind.Utc), 7, 2, 8, 1 },
                    { 25, 5, null, 3, false, new DateTime(2026, 4, 9, 14, 30, 0, 0, DateTimeKind.Utc), 4, 0, 5, 0 },
                    { 26, 5, new DateTime(2026, 4, 10, 18, 10, 0, 0, DateTimeKind.Utc), 3, true, new DateTime(2026, 4, 10, 16, 0, 0, 0, DateTimeKind.Utc), 6, 2, 8, 0 },
                    { 27, 6, new DateTime(2026, 6, 5, 17, 20, 0, 0, DateTimeKind.Utc), 3, true, new DateTime(2026, 6, 5, 15, 0, 0, 0, DateTimeKind.Utc), 1, 2, 8, 1 },
                    { 28, 6, null, 3, false, new DateTime(2026, 6, 5, 18, 0, 0, 0, DateTimeKind.Utc), 2, 0, 9, 0 },
                    { 29, 7, new DateTime(2026, 9, 20, 15, 30, 0, 0, DateTimeKind.Utc), 3, true, new DateTime(2026, 9, 20, 13, 0, 0, 0, DateTimeKind.Utc), 3, 2, 5, 0 }
                });

            migrationBuilder.InsertData(
                table: "MatchMaps",
                columns: new[] { "Id", "Map", "MapSequence", "MatchId", "TeamAScore", "TeamBScore", "WentToOvertime" },
                values: new object[,]
                {
                    { 11, 1, 1, 9, 13, 10, false },
                    { 12, 2, 2, 9, 11, 13, false },
                    { 13, 3, 3, 9, 13, 8, false },
                    { 14, 4, 1, 10, 13, 6, false },
                    { 15, 5, 2, 10, 13, 11, false },
                    { 16, 2, 1, 13, 13, 9, false },
                    { 17, 1, 2, 13, 9, 13, false },
                    { 18, 5, 3, 13, 13, 11, false },
                    { 19, 6, 4, 13, 10, 13, false },
                    { 20, 4, 5, 13, 13, 7, false },
                    { 21, 3, 1, 15, 13, 8, false },
                    { 22, 2, 2, 15, 13, 10, false },
                    { 23, 1, 1, 17, 11, 13, false },
                    { 24, 5, 2, 17, 13, 7, false },
                    { 25, 4, 3, 17, 13, 9, false },
                    { 26, 2, 1, 19, 13, 5, false },
                    { 27, 1, 2, 19, 10, 13, false },
                    { 28, 6, 3, 19, 13, 11, false },
                    { 29, 3, 1, 21, 13, 9, false },
                    { 30, 4, 2, 21, 13, 11, false },
                    { 31, 5, 1, 22, 9, 13, false },
                    { 32, 2, 2, 22, 13, 8, false },
                    { 33, 3, 3, 22, 13, 7, false },
                    { 34, 1, 1, 24, 13, 11, false },
                    { 35, 2, 2, 24, 10, 13, false },
                    { 36, 4, 3, 24, 13, 9, false },
                    { 37, 5, 1, 26, 13, 7, false },
                    { 38, 6, 2, 26, 13, 12, false },
                    { 39, 4, 1, 27, 13, 9, false },
                    { 40, 2, 2, 27, 11, 13, false },
                    { 41, 3, 3, 27, 13, 10, false },
                    { 42, 1, 1, 29, 13, 8, false },
                    { 43, 5, 2, 29, 13, 6, false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 1, 6 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 2, 5 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 2, 6 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 3, 5 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 3, 7 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 4, 6 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 5, 5 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 5, 7 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 6, 6 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 6, 7 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 7, 1 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 7, 2 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 7, 3 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 7, 4 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 7, 5 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 7, 6 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 7, 7 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 8, 1 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 8, 2 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 8, 3 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 8, 4 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 8, 5 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 8, 6 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 8, 7 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 9, 1 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 9, 2 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 9, 3 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 10, 1 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 10, 2 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 10, 3 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 11, 1 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 11, 2 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 11, 3 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 12, 1 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 12, 2 });

            migrationBuilder.DeleteData(
                table: "EventTeams",
                keyColumns: new[] { "TeamsId", "TournamentsId" },
                keyValues: new object[] { 12, 3 });

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "MatchMaps",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: 29);
        }
    }
}
