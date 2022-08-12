using Microsoft.EntityFrameworkCore.Migrations;

namespace ClimbingAPI.Migrations
{
    public partial class whoColumns2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModifiedUserId",
                table: "UserClimbingSpotLinks",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedUserId",
                table: "User",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedUserId",
                table: "Role",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedUserId",
                table: "ClimbingSpot",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedUserId",
                table: "Boulder",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedUserId",
                table: "Addresses",
                newName: "ModifiedByUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "UserClimbingSpotLinks",
                newName: "ModifiedUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "User",
                newName: "ModifiedUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "Role",
                newName: "ModifiedUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "ClimbingSpot",
                newName: "ModifiedUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "Boulder",
                newName: "ModifiedUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "Addresses",
                newName: "ModifiedUserId");
        }
    }
}
