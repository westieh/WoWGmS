using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WowGMSBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddBossSlugToBossRosters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BossSlug",
                table: "BossRosters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BossSlug",
                table: "BossRosters");
        }
    }
}
