using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealmOfLegends.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRiddleAttempt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RiddleAttempts",
                columns: table => new
                {
                    RiddleAttemptId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    RiddleId = table.Column<int>(type: "int", nullable: false),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    LastAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiddleAttempts", x => x.RiddleAttemptId);
                    table.ForeignKey(
                        name: "FK_RiddleAttempts_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RiddleAttempts_PlayerId",
                table: "RiddleAttempts",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RiddleAttempts");
        }
    }
}
