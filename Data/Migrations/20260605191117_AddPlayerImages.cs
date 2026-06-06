using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cs2_esports.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Players",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Players");
        }
    }
}
