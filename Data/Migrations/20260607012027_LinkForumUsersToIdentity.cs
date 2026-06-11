using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cs2_esports.Data.Migrations
{
    /// <inheritdoc />
    public partial class LinkForumUsersToIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LegacyForumUserId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LegacyAdminUserId",
                table: "AspNetUsers",
                column: "LegacyAdminUserId",
                unique: true,
                filter: "[LegacyAdminUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LegacyForumUserId",
                table: "AspNetUsers",
                column: "LegacyForumUserId",
                unique: true,
                filter: "[LegacyForumUserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LegacyAdminUserId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LegacyForumUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LegacyForumUserId",
                table: "AspNetUsers");
        }
    }
}
