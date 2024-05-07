using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShowsTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class V9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MyAnimeListId",
                table: "Show",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MyAnimeListId",
                table: "Show");
        }
    }
}
