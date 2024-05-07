using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShowsTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class V7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShowLink_Shows_ShowId",
                table: "ShowLink");

            migrationBuilder.DropForeignKey(
                name: "FK_Shows_User_InsertedUserId",
                table: "Shows");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowUsers_Shows_ShowId",
                table: "ShowUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowUsers_User_UserId",
                table: "ShowUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShowUsers",
                table: "ShowUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shows",
                table: "Shows");

            migrationBuilder.RenameTable(
                name: "ShowUsers",
                newName: "ShowUser");

            migrationBuilder.RenameTable(
                name: "Shows",
                newName: "Show");

            migrationBuilder.RenameIndex(
                name: "IX_ShowUsers_UserId",
                table: "ShowUser",
                newName: "IX_ShowUser_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ShowUsers_ShowId",
                table: "ShowUser",
                newName: "IX_ShowUser_ShowId");

            migrationBuilder.RenameIndex(
                name: "IX_Shows_InsertedUserId",
                table: "Show",
                newName: "IX_Show_InsertedUserId");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Show",
                type: "float",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShowUser",
                table: "ShowUser",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Show",
                table: "Show",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Genre",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genre", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShowGenre",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowGenre", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShowGenre_Genre_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genre",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShowGenre_Show_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Show",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShowGenre_GenreId",
                table: "ShowGenre",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowGenre_ShowId",
                table: "ShowGenre",
                column: "ShowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Show_User_InsertedUserId",
                table: "Show",
                column: "InsertedUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShowLink_Show_ShowId",
                table: "ShowLink",
                column: "ShowId",
                principalTable: "Show",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShowUser_Show_ShowId",
                table: "ShowUser",
                column: "ShowId",
                principalTable: "Show",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShowUser_User_UserId",
                table: "ShowUser",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Show_User_InsertedUserId",
                table: "Show");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowLink_Show_ShowId",
                table: "ShowLink");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowUser_Show_ShowId",
                table: "ShowUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ShowUser_User_UserId",
                table: "ShowUser");

            migrationBuilder.DropTable(
                name: "ShowGenre");

            migrationBuilder.DropTable(
                name: "Genre");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShowUser",
                table: "ShowUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Show",
                table: "Show");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Show");

            migrationBuilder.RenameTable(
                name: "ShowUser",
                newName: "ShowUsers");

            migrationBuilder.RenameTable(
                name: "Show",
                newName: "Shows");

            migrationBuilder.RenameIndex(
                name: "IX_ShowUser_UserId",
                table: "ShowUsers",
                newName: "IX_ShowUsers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ShowUser_ShowId",
                table: "ShowUsers",
                newName: "IX_ShowUsers_ShowId");

            migrationBuilder.RenameIndex(
                name: "IX_Show_InsertedUserId",
                table: "Shows",
                newName: "IX_Shows_InsertedUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShowUsers",
                table: "ShowUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shows",
                table: "Shows",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShowLink_Shows_ShowId",
                table: "ShowLink",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shows_User_InsertedUserId",
                table: "Shows",
                column: "InsertedUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShowUsers_Shows_ShowId",
                table: "ShowUsers",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShowUsers_User_UserId",
                table: "ShowUsers",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
