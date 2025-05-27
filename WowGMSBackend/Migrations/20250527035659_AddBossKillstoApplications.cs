using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WowGMSBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddBossKillstoApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationId",
                table: "BossKills",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BossKills_ApplicationId",
                table: "BossKills",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_BossKills_Applications_ApplicationId",
                table: "BossKills",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "ApplicationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BossKills_Applications_ApplicationId",
                table: "BossKills");

            migrationBuilder.DropIndex(
                name: "IX_BossKills_ApplicationId",
                table: "BossKills");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "BossKills");
        }
    }
}
