using Microsoft.EntityFrameworkCore.Migrations;

namespace ClimbingAPI.Migrations
{
    public partial class updatemodifiedbyuserid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Boulder");

            migrationBuilder.AddColumn<int>(
                name: "ModifiedByUserId",
                table: "Boulder",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "Boulder");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Boulder",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
