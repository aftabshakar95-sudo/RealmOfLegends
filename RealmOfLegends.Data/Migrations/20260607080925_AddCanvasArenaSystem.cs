using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealmOfLegends.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCanvasArenaSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArenaProgressNews",
                columns: table => new
                {
                    ArenaProgressNewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    HighestLevelCleared = table.Column<int>(type: "int", nullable: false),
                    TotalWins = table.Column<int>(type: "int", nullable: false),
                    TotalLosses = table.Column<int>(type: "int", nullable: false),
                    LastFightAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaProgressNews", x => x.ArenaProgressNewId);
                    table.ForeignKey(
                        name: "FK_ArenaProgressNews_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enemies",
                columns: table => new
                {
                    EnemyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    IsBoss = table.Column<bool>(type: "bit", nullable: false),
                    BaseHP = table.Column<int>(type: "int", nullable: false),
                    BaseATK = table.Column<int>(type: "int", nullable: false),
                    BaseDEF = table.Column<int>(type: "int", nullable: false),
                    EvasionChance = table.Column<int>(type: "int", nullable: false),
                    SpriteSheetPath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FrameWidth = table.Column<int>(type: "int", nullable: false),
                    FrameHeight = table.Column<int>(type: "int", nullable: false),
                    AnimationFPS = table.Column<int>(type: "int", nullable: false),
                    IdleFrames = table.Column<int>(type: "int", nullable: false),
                    WalkFrames = table.Column<int>(type: "int", nullable: false),
                    Attack1Frames = table.Column<int>(type: "int", nullable: false),
                    Attack2Frames = table.Column<int>(type: "int", nullable: false),
                    HeavyFrames = table.Column<int>(type: "int", nullable: false),
                    SpecialFrames = table.Column<int>(type: "int", nullable: false),
                    HitFrames = table.Column<int>(type: "int", nullable: false),
                    StaggerFrames = table.Column<int>(type: "int", nullable: false),
                    KnockbackFrames = table.Column<int>(type: "int", nullable: false),
                    DeathFrames = table.Column<int>(type: "int", nullable: false),
                    RenderScale = table.Column<float>(type: "real", nullable: false),
                    GroundShadowWidth = table.Column<int>(type: "int", nullable: false),
                    SpecialAbilityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SpecialAbilityDamageMultiplier = table.Column<float>(type: "real", nullable: false),
                    DropGoldMin = table.Column<int>(type: "int", nullable: false),
                    DropGoldMax = table.Column<int>(type: "int", nullable: false),
                    DropXP = table.Column<int>(type: "int", nullable: false),
                    DropItemChance = table.Column<int>(type: "int", nullable: false),
                    LightingTint = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ParticleTheme = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enemies", x => x.EnemyId);
                });

            migrationBuilder.CreateTable(
                name: "FightLogNews",
                columns: table => new
                {
                    FightLogNewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    EnemyId = table.Column<int>(type: "int", nullable: false),
                    ArenaLevel = table.Column<int>(type: "int", nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TurnsPlayed = table.Column<int>(type: "int", nullable: false),
                    XPEarned = table.Column<int>(type: "int", nullable: false),
                    GoldEarned = table.Column<int>(type: "int", nullable: false),
                    DroppedItemId = table.Column<int>(type: "int", nullable: true),
                    FoughtAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FightLogNews", x => x.FightLogNewId);
                    table.ForeignKey(
                        name: "FK_FightLogNews_Enemies_EnemyId",
                        column: x => x.EnemyId,
                        principalTable: "Enemies",
                        principalColumn: "EnemyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FightLogNews_Items_DroppedItemId",
                        column: x => x.DroppedItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FightLogNews_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FightSessions",
                columns: table => new
                {
                    FightSessionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    EnemyId = table.Column<int>(type: "int", nullable: false),
                    ArenaLevel = table.Column<int>(type: "int", nullable: false),
                    PlayerCurrentHP = table.Column<int>(type: "int", nullable: false),
                    PlayerMaxHP = table.Column<int>(type: "int", nullable: false),
                    EnemyCurrentHP = table.Column<int>(type: "int", nullable: false),
                    EnemyMaxHP = table.Column<int>(type: "int", nullable: false),
                    TurnNumber = table.Column<int>(type: "int", nullable: false),
                    SessionStartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EnemyATKModifier = table.Column<int>(type: "int", nullable: false),
                    BleedTurnsRemaining = table.Column<int>(type: "int", nullable: false),
                    BleedDamagePerTurn = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FightSessions", x => x.FightSessionId);
                    table.ForeignKey(
                        name: "FK_FightSessions_Enemies_EnemyId",
                        column: x => x.EnemyId,
                        principalTable: "Enemies",
                        principalColumn: "EnemyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FightSessions_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArenaProgressNews_PlayerId",
                table: "ArenaProgressNews",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FightLogNews_DroppedItemId",
                table: "FightLogNews",
                column: "DroppedItemId");

            migrationBuilder.CreateIndex(
                name: "IX_FightLogNews_EnemyId",
                table: "FightLogNews",
                column: "EnemyId");

            migrationBuilder.CreateIndex(
                name: "IX_FightLogNews_PlayerId",
                table: "FightLogNews",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_FightSessions_EnemyId",
                table: "FightSessions",
                column: "EnemyId");

            migrationBuilder.CreateIndex(
                name: "IX_FightSessions_PlayerId",
                table: "FightSessions",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArenaProgressNews");

            migrationBuilder.DropTable(
                name: "FightLogNews");

            migrationBuilder.DropTable(
                name: "FightSessions");

            migrationBuilder.DropTable(
                name: "Enemies");
        }
    }
}
