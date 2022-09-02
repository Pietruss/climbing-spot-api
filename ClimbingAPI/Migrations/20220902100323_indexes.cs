using Microsoft.EntityFrameworkCore.Migrations;

namespace ClimbingAPI.Migrations
{
    public partial class indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UserClimbingSpotLinks_ClimbingSpotId_UserId",
                table: "UserClimbingSpotLinks",
                columns: new[] { "ClimbingSpotId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserClimbingSpotLinks_ClimbingSpotId_UserId_RoleId",
                table: "UserClimbingSpotLinks",
                columns: new[] { "ClimbingSpotId", "UserId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserClimbingSpotLinks_ClimbingSpotId_UserId",
                table: "UserClimbingSpotLinks");

            migrationBuilder.DropIndex(
                name: "IX_UserClimbingSpotLinks_ClimbingSpotId_UserId_RoleId",
                table: "UserClimbingSpotLinks");

            migrationBuilder.DropIndex(
                name: "IX_User_Email",
                table: "User");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
