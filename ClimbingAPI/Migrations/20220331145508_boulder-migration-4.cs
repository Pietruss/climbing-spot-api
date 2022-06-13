using Microsoft.EntityFrameworkCore.Migrations;

namespace ClimbingAPI.Migrations
{
    public partial class bouldermigration4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Boulder_ClimbingSpotId",
                table: "Boulder");

            migrationBuilder.CreateIndex(
                name: "IX_Boulder_ClimbingSpotId",
                table: "Boulder",
                column: "ClimbingSpotId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Boulder_ClimbingSpotId",
                table: "Boulder");

            migrationBuilder.CreateIndex(
                name: "IX_Boulder_ClimbingSpotId",
                table: "Boulder",
                column: "ClimbingSpotId",
                unique: true);
        }
    }
}
