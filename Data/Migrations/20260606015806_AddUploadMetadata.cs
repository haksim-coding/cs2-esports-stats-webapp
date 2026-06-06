using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cs2_esports.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUploadMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BannerContentType",
                table: "Tournaments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BannerCreatedAtUtc",
                table: "Tournaments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BannerFileSize",
                table: "Tournaments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "Players",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ImageCreatedAtUtc",
                table: "Players",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ImageFileSize",
                table: "Players",
                type: "bigint",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 75,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 76,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 77,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 78,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 82,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 83,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 84,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 85,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 86,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 87,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 88,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 89,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 91,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 92,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 93,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 94,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 95,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 96,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 97,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 98,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "ImageContentType", "ImageCreatedAtUtc", "ImageFileSize" },
                values: new object[] { null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerContentType",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "BannerCreatedAtUtc",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "BannerFileSize",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "ImageContentType",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ImageCreatedAtUtc",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ImageFileSize",
                table: "Players");
        }
    }
}
