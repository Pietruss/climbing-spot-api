using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClimbingAPI.Migrations
{
    public partial class whoColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "Boulder");

            migrationBuilder.RenameColumn(
                name: "ModificationTime",
                table: "ClimbingSpot",
                newName: "ModificationDateTime");

            migrationBuilder.RenameColumn(
                name: "Author",
                table: "ClimbingSpot",
                newName: "ModifiedUserId");

            migrationBuilder.RenameColumn(
                name: "ModificationTime",
                table: "Boulder",
                newName: "ModificationDateTime");

            migrationBuilder.RenameColumn(
                name: "Author",
                table: "Boulder",
                newName: "ModifiedUserId");

            migrationBuilder.RenameColumn(
                name: "ModificationTime",
                table: "Addresses",
                newName: "ModificationDateTime");

            migrationBuilder.RenameColumn(
                name: "Author",
                table: "Addresses",
                newName: "ModifiedUserId");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "UserClimbingSpotLinks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDateTime",
                table: "UserClimbingSpotLinks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDateTime",
                table: "UserClimbingSpotLinks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedUserId",
                table: "UserClimbingSpotLinks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDateTime",
                table: "User",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDateTime",
                table: "User",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedUserId",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Role",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDateTime",
                table: "Role",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDateTime",
                table: "Role",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedUserId",
                table: "Role",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "ClimbingSpot",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDateTime",
                table: "ClimbingSpot",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Boulder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDateTime",
                table: "Boulder",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDateTime",
                table: "Addresses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "UserClimbingSpotLinks");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "UserClimbingSpotLinks");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "UserClimbingSpotLinks");

            migrationBuilder.DropColumn(
                name: "ModifiedUserId",
                table: "UserClimbingSpotLinks");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ModifiedUserId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ModifiedUserId",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ClimbingSpot");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "ClimbingSpot");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Boulder");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "Boulder");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "ModifiedUserId",
                table: "ClimbingSpot",
                newName: "Author");

            migrationBuilder.RenameColumn(
                name: "ModificationDateTime",
                table: "ClimbingSpot",
                newName: "ModificationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedUserId",
                table: "Boulder",
                newName: "Author");

            migrationBuilder.RenameColumn(
                name: "ModificationDateTime",
                table: "Boulder",
                newName: "ModificationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedUserId",
                table: "Addresses",
                newName: "Author");

            migrationBuilder.RenameColumn(
                name: "ModificationDateTime",
                table: "Addresses",
                newName: "ModificationTime");

            migrationBuilder.AddColumn<int>(
                name: "ModifiedByUserId",
                table: "Boulder",
                type: "int",
                nullable: true);
        }
    }
}
