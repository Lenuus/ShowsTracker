using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShowsTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class V15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VoteSeason",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsFinished = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteSeason", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserVote",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoteSeasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoteDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserVote_Show_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Show",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserVote_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserVote_VoteSeason_VoteSeasonId",
                        column: x => x.VoteSeasonId,
                        principalTable: "VoteSeason",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VoteShow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoteSeasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsWinner = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteShow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoteShow_Show_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Show",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VoteShow_VoteSeason_VoteSeasonId",
                        column: x => x.VoteSeasonId,
                        principalTable: "VoteSeason",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserVote_ShowId",
                table: "UserVote",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVote_UserId",
                table: "UserVote",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVote_VoteSeasonId",
                table: "UserVote",
                column: "VoteSeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteShow_ShowId",
                table: "VoteShow",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteShow_VoteSeasonId",
                table: "VoteShow",
                column: "VoteSeasonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserVote");

            migrationBuilder.DropTable(
                name: "VoteShow");

            migrationBuilder.DropTable(
                name: "VoteSeason");
        }
    }
}
