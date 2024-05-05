using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class ChangedAdminToIpAddresInTwoFactorCodesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TwoFactorAuths_Admins_AdminId",
                table: "TwoFactorAuths");

            migrationBuilder.DropIndex(
                name: "IX_TwoFactorAuths_AdminId",
                table: "TwoFactorAuths");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "TwoFactorAuths");

            migrationBuilder.AddColumn<string>(
                name: "IP",
                table: "TwoFactorAuths",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IP",
                table: "TwoFactorAuths");

            migrationBuilder.AddColumn<long>(
                name: "AdminId",
                table: "TwoFactorAuths",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_TwoFactorAuths_AdminId",
                table: "TwoFactorAuths",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_TwoFactorAuths_Admins_AdminId",
                table: "TwoFactorAuths",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
