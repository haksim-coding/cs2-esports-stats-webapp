using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cs2_esports.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminUser_Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql("UPDATE [Users] SET [AdminUser_Password] = N'password123' WHERE [UserType] = N'AdminUser';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminUser_Password",
                table: "Users");
        }
    }
}
