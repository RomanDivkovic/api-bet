using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiBet.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups");

            migrationBuilder.DropIndex(
                name: "IX_UserGroups_GroupId",
                table: "UserGroups");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "GroupInvitations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "GroupInvitations",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups",
                columns: new[] { "GroupId", "UserId" });

            migrationBuilder.CreateTable(
                name: "BlacklistedTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlacklistedTokens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_UserId",
                table: "UserGroups",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupInvitations_UserId",
                table: "GroupInvitations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupInvitations_Users_UserId",
                table: "GroupInvitations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupInvitations_Users_UserId",
                table: "GroupInvitations");

            migrationBuilder.DropTable(
                name: "BlacklistedTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups");

            migrationBuilder.DropIndex(
                name: "IX_UserGroups_UserId",
                table: "UserGroups");

            migrationBuilder.DropIndex(
                name: "IX_GroupInvitations_UserId",
                table: "GroupInvitations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "GroupInvitations");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "GroupInvitations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups",
                columns: new[] { "UserId", "GroupId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_GroupId",
                table: "UserGroups",
                column: "GroupId");
        }
    }
}
