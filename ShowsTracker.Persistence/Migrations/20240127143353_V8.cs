using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShowsTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class V8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DayOfWeek",
                table: "Show",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Show",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Popularity",
                table: "Show",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Show",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Show",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSeason",
                table: "Show",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "Show");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Show");

            migrationBuilder.DropColumn(
                name: "Popularity",
                table: "Show");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Show");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Show");

            migrationBuilder.DropColumn(
                name: "StartSeason",
                table: "Show");
        }
    }
}
