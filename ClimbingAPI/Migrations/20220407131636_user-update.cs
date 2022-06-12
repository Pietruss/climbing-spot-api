using Microsoft.EntityFrameworkCore.Migrations;

namespace ClimbingAPI.Migrations
{
    public partial class userupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Boulder",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Boulder_CreatedById",
                table: "Boulder",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Boulder_User_CreatedById",
                table: "Boulder",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boulder_User_CreatedById",
                table: "Boulder");

            migrationBuilder.DropIndex(
                name: "IX_Boulder_CreatedById",
                table: "Boulder");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Boulder");
        }
    }
}
