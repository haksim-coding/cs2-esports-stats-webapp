using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cs2_esports.Data.Migrations
{
    /// <inheritdoc />
    public partial class SplitAdminUsersToTpt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForumComments_Users_AuthorId",
                table: "ForumComments");

            migrationBuilder.DropForeignKey(
                name: "FK_Forums_Users_AuthorId",
                table: "Forums");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumUserFavoritePlayers_Users_ForumUserId",
                table: "ForumUserFavoritePlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumUserFavoriteTeams_Users_ForumUserId",
                table: "ForumUserFavoriteTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_Users_AdminUserId",
                table: "Tournaments");

            migrationBuilder.CreateTable(
                name: "AdminUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    HiredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModerationActionAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PermissionGroup = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminUsers_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForumUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    LastActiveAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPremiumMember = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForumUsers_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("""
INSERT INTO [Users] ([Username], [DisplayName], [Email], [CountryCode], [RegisteredAtUtc], [IsSuspended], [Password], [HiredAtUtc], [LastModerationActionAtUtc], [PermissionGroup], [LastActiveAtUtc], [IsPremiumMember], [UserType], [Bio])
SELECT N'blast_admin', N'Blast Admin', N'blast_admin@cs2scope.local', N'US', SYSUTCDATETIME(), 0, N'password123', SYSUTCDATETIME(), NULL, N'TournamentAdmin', NULL, NULL, N'AdminUser', N''
WHERE NOT EXISTS (SELECT 1 FROM [Users] WHERE [Username] = N'blast_admin');

INSERT INTO [Users] ([Username], [DisplayName], [Email], [CountryCode], [RegisteredAtUtc], [IsSuspended], [Password], [HiredAtUtc], [LastModerationActionAtUtc], [PermissionGroup], [LastActiveAtUtc], [IsPremiumMember], [UserType], [Bio])
SELECT N'esl_admin', N'ESL Admin', N'esl_admin@cs2scope.local', N'US', SYSUTCDATETIME(), 0, N'password123', SYSUTCDATETIME(), NULL, N'TournamentAdmin', NULL, NULL, N'AdminUser', N''
WHERE NOT EXISTS (SELECT 1 FROM [Users] WHERE [Username] = N'esl_admin');

INSERT INTO [Users] ([Username], [DisplayName], [Email], [CountryCode], [RegisteredAtUtc], [IsSuspended], [Password], [HiredAtUtc], [LastModerationActionAtUtc], [PermissionGroup], [LastActiveAtUtc], [IsPremiumMember], [UserType], [Bio])
SELECT N'admin_maksim', N'Maksim', N'admin_maksim@cs2scope.local', N'US', SYSUTCDATETIME(), 0, N'password123', SYSUTCDATETIME(), NULL, N'SuperAdmin', NULL, NULL, N'AdminUser', N''
WHERE NOT EXISTS (SELECT 1 FROM [Users] WHERE [Username] = N'admin_maksim');

INSERT INTO [AdminUsers] ([Id], [HiredAtUtc], [LastModerationActionAtUtc], [PermissionGroup])
SELECT
    [Id],
    COALESCE([HiredAtUtc], SYSUTCDATETIME()),
    [LastModerationActionAtUtc],
    CASE WHEN [Username] = N'admin_maksim' THEN N'SuperAdmin' ELSE COALESCE([PermissionGroup], N'TournamentAdmin') END
FROM [Users]
WHERE [Username] IN (N'admin_maksim', N'blast_admin', N'esl_admin')
  AND NOT EXISTS (SELECT 1 FROM [AdminUsers] WHERE [AdminUsers].[Id] = [Users].[Id]);

INSERT INTO [ForumUsers] ([Id], [LastActiveAtUtc], [IsPremiumMember])
SELECT
    [Id],
    COALESCE([LastActiveAtUtc], SYSUTCDATETIME()),
    COALESCE([IsPremiumMember], 0)
FROM [Users]
WHERE [UserType] = N'ForumUser'
  AND NOT EXISTS (SELECT 1 FROM [ForumUsers] WHERE [ForumUsers].[Id] = [Users].[Id]);
""");

            migrationBuilder.DropColumn(
                name: "HiredAtUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsPremiumMember",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastActiveAtUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModerationActionAtUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PermissionGroup",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumComments_ForumUsers_AuthorId",
                table: "ForumComments",
                column: "AuthorId",
                principalTable: "ForumUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Forums_ForumUsers_AuthorId",
                table: "Forums",
                column: "AuthorId",
                principalTable: "ForumUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ForumUserFavoritePlayers_ForumUsers_ForumUserId",
                table: "ForumUserFavoritePlayers",
                column: "ForumUserId",
                principalTable: "ForumUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ForumUserFavoriteTeams_ForumUsers_ForumUserId",
                table: "ForumUserFavoriteTeams",
                column: "ForumUserId",
                principalTable: "ForumUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_AdminUsers_AdminUserId",
                table: "Tournaments",
                column: "AdminUserId",
                principalTable: "AdminUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ForumComments_ForumUsers_AuthorId",
                table: "ForumComments");

            migrationBuilder.DropForeignKey(
                name: "FK_Forums_ForumUsers_AuthorId",
                table: "Forums");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumUserFavoritePlayers_ForumUsers_ForumUserId",
                table: "ForumUserFavoritePlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_ForumUserFavoriteTeams_ForumUsers_ForumUserId",
                table: "ForumUserFavoriteTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_AdminUsers_AdminUserId",
                table: "Tournaments");

            migrationBuilder.AddColumn<DateTime>(
                name: "HiredAtUtc",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPremiumMember",
                table: "Users",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActiveAtUtc",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModerationActionAtUtc",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermissionGroup",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "Users",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
UPDATE [Users]
SET
    [HiredAtUtc] = admin.[HiredAtUtc],
    [LastModerationActionAtUtc] = admin.[LastModerationActionAtUtc],
    [PermissionGroup] = admin.[PermissionGroup],
    [UserType] = N'AdminUser'
FROM [Users]
INNER JOIN [AdminUsers] AS admin ON admin.[Id] = [Users].[Id];

UPDATE [Users]
SET
    [LastActiveAtUtc] = forum.[LastActiveAtUtc],
    [IsPremiumMember] = forum.[IsPremiumMember],
    [UserType] = N'ForumUser'
FROM [Users]
INNER JOIN [ForumUsers] AS forum ON forum.[Id] = [Users].[Id];
""");

            migrationBuilder.DropTable(
                name: "AdminUsers");

            migrationBuilder.DropTable(
                name: "ForumUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "HiredAtUtc",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPremiumMember",
                table: "Users",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActiveAtUtc",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModerationActionAtUtc",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermissionGroup",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "Users",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ForumComments_Users_AuthorId",
                table: "ForumComments",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Forums_Users_AuthorId",
                table: "Forums",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ForumUserFavoritePlayers_Users_ForumUserId",
                table: "ForumUserFavoritePlayers",
                column: "ForumUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ForumUserFavoriteTeams_Users_ForumUserId",
                table: "ForumUserFavoriteTeams",
                column: "ForumUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_Users_AdminUserId",
                table: "Tournaments",
                column: "AdminUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
