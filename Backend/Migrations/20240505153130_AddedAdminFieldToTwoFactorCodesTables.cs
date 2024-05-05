using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    public partial class AddedAdminFieldToTwoFactorCodesTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add the AdminId column first
            migrationBuilder.AddColumn<long>(
                name: "AdminId",
                table: "TwoFactorAuths",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            // Then clean up invalid AdminId entries
            migrationBuilder.Sql(
                @"DELETE FROM ""TwoFactorAuths"" 
                  WHERE NOT EXISTS (
                      SELECT 1 FROM ""Admins"" WHERE ""Id"" = ""TwoFactorAuths"".""AdminId""
                  );"
            );

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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}