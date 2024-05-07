using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShowsTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class V6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Shows",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BannerImageUrl",
                table: "Shows",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "Shows",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Shows",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReleaseGap",
                table: "Shows",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReleaseType",
                table: "Shows",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShowLink",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Link = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    ShowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShowLink_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShowLink_ShowId",
                table: "ShowLink",
                column: "ShowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShowLink");

            migrationBuilder.DropColumn(
                name: "BannerImageUrl",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "ReleaseGap",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "ReleaseType",
                table: "Shows");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Shows",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);
        }
    }
}
