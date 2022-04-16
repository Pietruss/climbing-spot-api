using Microsoft.EntityFrameworkCore.Migrations;

namespace ClimbingAPI.Migrations
{
    public partial class boulderupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastAuthor",
                table: "Boulder",
                newName: "Level");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Boulder",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Boulder");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "Boulder",
                newName: "LastAuthor");
        }
    }
}
