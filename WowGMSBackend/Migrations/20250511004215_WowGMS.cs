using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WowGMSBackend.Migrations
{
    /// <inheritdoc />
    public partial class WowGMS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BossRosters",
                columns: table => new
                {
                    RosterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BossName = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InstanceTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BossRosters", x => x.RosterId);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.MemberId);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    CharacterName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RealmName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Class = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    BossRosterRosterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.CharacterName);
                    table.ForeignKey(
                        name: "FK_Characters_BossRosters_BossRosterRosterId",
                        column: x => x.BossRosterRosterId,
                        principalTable: "BossRosters",
                        principalColumn: "RosterId");
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscordName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Class = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedByMemberId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_Applications_Members_ProcessedByMemberId",
                        column: x => x.ProcessedByMemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ProcessedByMemberId",
                table: "Applications",
                column: "ProcessedByMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_BossRosterRosterId",
                table: "Characters",
                column: "BossRosterRosterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "BossRosters");
        }
    }
}
