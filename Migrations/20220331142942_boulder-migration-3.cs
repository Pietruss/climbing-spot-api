using Microsoft.EntityFrameworkCore.Migrations;

namespace ClimbingAPI.Migrations
{
    public partial class bouldermigration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClimbingSpot_Boulder_BoulderId1",
                table: "ClimbingSpot");

            migrationBuilder.DropIndex(
                name: "IX_ClimbingSpot_BoulderId1",
                table: "ClimbingSpot");

            migrationBuilder.DropColumn(
                name: "BoulderId",
                table: "ClimbingSpot");

            migrationBuilder.DropColumn(
                name: "BoulderId1",
                table: "ClimbingSpot");

            migrationBuilder.DropColumn(
                name: "ClimbingSpot",
                table: "Boulder");

            migrationBuilder.CreateIndex(
                name: "IX_Boulder_ClimbingSpotId",
                table: "Boulder",
                column: "ClimbingSpotId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Boulder_ClimbingSpot_ClimbingSpotId",
                table: "Boulder",
                column: "ClimbingSpotId",
                principalTable: "ClimbingSpot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boulder_ClimbingSpot_ClimbingSpotId",
                table: "Boulder");

            migrationBuilder.DropIndex(
                name: "IX_Boulder_ClimbingSpotId",
                table: "Boulder");

            migrationBuilder.AddColumn<int>(
                name: "BoulderId",
                table: "ClimbingSpot",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BoulderId1",
                table: "ClimbingSpot",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClimbingSpot",
                table: "Boulder",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ClimbingSpot_BoulderId1",
                table: "ClimbingSpot",
                column: "BoulderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ClimbingSpot_Boulder_BoulderId1",
                table: "ClimbingSpot",
                column: "BoulderId1",
                principalTable: "Boulder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
