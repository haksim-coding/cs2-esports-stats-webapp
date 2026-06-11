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
