using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cs2_esports.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilesAndFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ForumUserFavoritePlayers",
                columns: table => new
                {
                    ForumUserId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumUserFavoritePlayers", x => new { x.ForumUserId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_ForumUserFavoritePlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForumUserFavoritePlayers_Users_ForumUserId",
                        column: x => x.ForumUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForumUserFavoriteTeams",
                columns: table => new
                {
                    ForumUserId = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumUserFavoriteTeams", x => new { x.ForumUserId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_ForumUserFavoriteTeams_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForumUserFavoriteTeams_Users_ForumUserId",
                        column: x => x.ForumUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForumUserFavoritePlayers_PlayerId",
                table: "ForumUserFavoritePlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumUserFavoriteTeams_TeamId",
                table: "ForumUserFavoriteTeams",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForumUserFavoritePlayers");

            migrationBuilder.DropTable(
                name: "ForumUserFavoriteTeams");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Users");
        }
    }
}
