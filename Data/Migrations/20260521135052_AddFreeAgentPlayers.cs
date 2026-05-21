using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace cs2_esports.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFreeAgentPlayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 70,
                column: "FullName",
                value: "Andre Pereira");

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "Id", "CountryCode", "DateOfBirth", "FullName", "JoinedTeamAtUtc", "Nickname", "Rating2", "Role", "TeamId", "TotalMapsPlayed" },
                values: new object[,]
                {
                    { 71, "SE", new DateTime(1990, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Christopher Alesund", new DateTime(2010, 1, 15, 12, 0, 0, 0, DateTimeKind.Utc), "GeT_RiGhT", 0.97m, 1, null, 2450 },
                    { 72, "SE", new DateTime(1988, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Patrik Lindberg", new DateTime(2009, 8, 1, 12, 0, 0, 0, DateTimeKind.Utc), "f0rest", 1.01m, 1, null, 2920 },
                    { 73, "SE", new DateTime(1991, 10, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Adam Friberg", new DateTime(2013, 5, 1, 12, 0, 0, 0, DateTimeKind.Utc), "friberg", 0.96m, 4, null, 2180 },
                    { 74, "SE", new DateTime(1991, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Richard Landstrom", new DateTime(2012, 3, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Xizt", 0.93m, 3, null, 1880 },
                    { 75, "FI", new DateTime(1992, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Aleksi Jalli", new DateTime(2014, 6, 1, 12, 0, 0, 0, DateTimeKind.Utc), "allu", 1.02m, 2, null, 2310 },
                    { 76, "DK", new DateTime(1994, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Philip Aistrup", new DateTime(2015, 2, 1, 12, 0, 0, 0, DateTimeKind.Utc), "aizy", 0.95m, 1, null, 1650 },
                    { 77, "DK", new DateTime(1994, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mathias Lauridsen", new DateTime(2015, 5, 1, 12, 0, 0, 0, DateTimeKind.Utc), "MSL", 0.92m, 3, null, 1725 },
                    { 78, "DK", new DateTime(1990, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Finn Andersen", new DateTime(2010, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "karrigan", 0.98m, 3, null, 2650 },
                    { 79, "DK", new DateTime(1993, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Peter Rasmussen", new DateTime(2013, 8, 1, 12, 0, 0, 0, DateTimeKind.Utc), "dupreeh", 1.03m, 4, null, 2730 },
                    { 80, "DK", new DateTime(1998, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Emil Reif", new DateTime(2017, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "magisk", 1.04m, 1, null, 1680 },
                    { 81, "DK", new DateTime(1995, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nicolai Reedtz", new DateTime(2014, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "device", 1.08m, 2, null, 2785 },
                    { 82, "DK", new DateTime(1990, 2, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Marco Pfeiffer", new DateTime(2016, 3, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Snappi", 0.94m, 3, null, 2105 },
                    { 83, "DK", new DateTime(1997, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Benjamin Bremer", new DateTime(2017, 4, 1, 12, 0, 0, 0, DateTimeKind.Utc), "blameF", 1.07m, 1, null, 1940 },
                    { 84, "DK", new DateTime(2002, 11, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Martin Lund", new DateTime(2019, 7, 1, 12, 0, 0, 0, DateTimeKind.Utc), "stavn", 1.09m, 1, null, 1390 },
                    { 85, "DK", new DateTime(1995, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Casper Moller", new DateTime(2015, 9, 1, 12, 0, 0, 0, DateTimeKind.Utc), "cadiaN", 0.99m, 3, null, 2200 },
                    { 86, "FR", new DateTime(1992, 5, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Richard Papillon", new DateTime(2014, 2, 1, 12, 0, 0, 0, DateTimeKind.Utc), "shox", 0.97m, 1, null, 2405 },
                    { 87, "FR", new DateTime(1993, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dan Madesclaire", new DateTime(2012, 7, 1, 12, 0, 0, 0, DateTimeKind.Utc), "apEX", 0.98m, 3, null, 2575 },
                    { 88, "FR", new DateTime(1994, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nathan Schmitt", new DateTime(2013, 3, 1, 12, 0, 0, 0, DateTimeKind.Utc), "NBK-", 0.95m, 5, null, 2220 },
                    { 89, "FR", new DateTime(1995, 5, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kenny Schrub", new DateTime(2014, 4, 1, 12, 0, 0, 0, DateTimeKind.Utc), "kennyS", 1.03m, 2, null, 2260 },
                    { 90, "FR", new DateTime(1997, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cedric Guipouy", new DateTime(2016, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), "bodyy", 0.94m, 1, null, 1420 },
                    { 91, "FR", new DateTime(1991, 1, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dan Grzesiak", new DateTime(2013, 6, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Happy", 0.91m, 3, null, 1950 },
                    { 92, "SK", new DateTime(1991, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ladislav Kovacs", new DateTime(2013, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "GuardiaN", 1.01m, 2, null, 2495 },
                    { 93, "SE", new DateTime(1992, 1, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Olof Kajbjer", new DateTime(2014, 9, 1, 12, 0, 0, 0, DateTimeKind.Utc), "olofmeister", 0.99m, 1, null, 2560 },
                    { 94, "SE", new DateTime(1993, 8, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Robin Ronnquist", new DateTime(2013, 9, 1, 12, 0, 0, 0, DateTimeKind.Utc), "flusha", 0.98m, 1, null, 2480 },
                    { 95, "SE", new DateTime(1990, 5, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Markus Wallsten", new DateTime(2013, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), "pronax", 0.90m, 3, null, 1640 },
                    { 96, "ID", new DateTime(1995, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hansel Ferdinand", new DateTime(2016, 6, 1, 12, 0, 0, 0, DateTimeKind.Utc), "BnTeT", 1.00m, 1, null, 1695 },
                    { 97, "NL", new DateTime(1990, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chris de Jong", new DateTime(2014, 2, 1, 12, 0, 0, 0, DateTimeKind.Utc), "chrisJ", 0.95m, 1, null, 2300 },
                    { 98, "DK", new DateTime(1991, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jacob Winneche", new DateTime(2015, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Pimp", 0.89m, 5, null, 1120 },
                    { 99, "GB", new DateTime(1999, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Owen Butterfield", new DateTime(2018, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "smooya", 1.02m, 2, null, 1560 },
                    { 100, "ME", new DateTime(1998, 12, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Martin Petrov", new DateTime(2020, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "maden", 1.00m, 4, null, 1480 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 70,
                column: "FullName",
                value: "André Pereira");
        }
    }
}
