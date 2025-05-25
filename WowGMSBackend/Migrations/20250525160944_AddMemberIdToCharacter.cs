using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WowGMSBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberIdToCharacter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Members_OwnerId",
                table: "Characters");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Characters",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_OwnerId",
                table: "Characters",
                newName: "IX_Characters_MemberId");

            migrationBuilder.AlterColumn<int>(
                name: "RealmName",
                table: "Characters",
                type: "int",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Members_MemberId",
                table: "Characters",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Members_MemberId",
                table: "Characters");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "Characters",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_MemberId",
                table: "Characters",
                newName: "IX_Characters_OwnerId");

            migrationBuilder.AlterColumn<string>(
                name: "RealmName",
                table: "Characters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Members_OwnerId",
                table: "Characters",
                column: "OwnerId",
                principalTable: "Members",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
