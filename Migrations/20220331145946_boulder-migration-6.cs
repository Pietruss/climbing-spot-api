using Microsoft.EntityFrameworkCore.Migrations;

namespace ClimbingAPI.Migrations
{
    public partial class bouldermigration6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boulder_ClimbingSpot_ClimbingSpotId",
                table: "Boulder");

            migrationBuilder.AlterColumn<int>(
                name: "ClimbingSpotId",
                table: "Boulder",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Boulder_ClimbingSpot_ClimbingSpotId",
                table: "Boulder",
                column: "ClimbingSpotId",
                principalTable: "ClimbingSpot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boulder_ClimbingSpot_ClimbingSpotId",
                table: "Boulder");

            migrationBuilder.AlterColumn<int>(
                name: "ClimbingSpotId",
                table: "Boulder",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Boulder_ClimbingSpot_ClimbingSpotId",
                table: "Boulder",
                column: "ClimbingSpotId",
                principalTable: "ClimbingSpot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
