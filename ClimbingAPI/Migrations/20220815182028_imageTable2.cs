using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClimbingAPI.Migrations
{
    public partial class imageTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Images",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDateTime",
                table: "Images",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDateTime",
                table: "Images",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByUserId",
                table: "Images",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "Images");
        }
    }
}
