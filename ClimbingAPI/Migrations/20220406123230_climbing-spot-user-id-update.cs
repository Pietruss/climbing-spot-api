using Microsoft.EntityFrameworkCore.Migrations;

namespace ClimbingAPI.Migrations
{
    public partial class climbingspotuseridupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "I",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "ClimbingSpot",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClimbingSpot_CreatedById",
                table: "ClimbingSpot",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ClimbingSpot_User_CreatedById",
                table: "ClimbingSpot",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClimbingSpot_User_CreatedById",
                table: "ClimbingSpot");

            migrationBuilder.DropIndex(
                name: "IX_ClimbingSpot_CreatedById",
                table: "ClimbingSpot");

            migrationBuilder.DropColumn(
                name: "I",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ClimbingSpot");
        }
    }
}
