using Microsoft.EntityFrameworkCore.Migrations;

namespace ClimbingAPI.Migrations
{
    public partial class userClimbingSpotadded2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClimbingSpot_User_CreatedById",
                table: "ClimbingSpot");

            migrationBuilder.DropIndex(
                name: "IX_ClimbingSpot_CreatedById",
                table: "ClimbingSpot");

            migrationBuilder.AddColumn<int>(
                name: "ClimbingSpotId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById1",
                table: "ClimbingSpot",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClimbingSpot_CreatedById1",
                table: "ClimbingSpot",
                column: "CreatedById1");

            migrationBuilder.AddForeignKey(
                name: "FK_ClimbingSpot_User_CreatedById1",
                table: "ClimbingSpot",
                column: "CreatedById1",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClimbingSpot_User_CreatedById1",
                table: "ClimbingSpot");

            migrationBuilder.DropIndex(
                name: "IX_ClimbingSpot_CreatedById1",
                table: "ClimbingSpot");

            migrationBuilder.DropColumn(
                name: "ClimbingSpotId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedById1",
                table: "ClimbingSpot");

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
    }
}
