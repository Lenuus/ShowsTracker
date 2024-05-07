using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShowsTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class V13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultLink",
                table: "ShowLink",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ShowLink",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ShowLink_UserId",
                table: "ShowLink",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShowLink_User_UserId",
                table: "ShowLink",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShowLink_User_UserId",
                table: "ShowLink");

            migrationBuilder.DropIndex(
                name: "IX_ShowLink_UserId",
                table: "ShowLink");

            migrationBuilder.DropColumn(
                name: "IsDefaultLink",
                table: "ShowLink");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ShowLink");
        }
    }
}
