using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShowsTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StatusBool_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Shows",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Shows");
        }
    }
}
