using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cs2_esports.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventBannerImagesAndMajorTier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BannerImagePath",
                table: "Tournaments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql("UPDATE [Tournaments] SET [Tier] = 0 WHERE [Name] LIKE '%Major%'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [Tournaments] SET [Tier] = 1 WHERE [Tier] = 0");

            migrationBuilder.DropColumn(
                name: "BannerImagePath",
                table: "Tournaments");
        }
    }
}
