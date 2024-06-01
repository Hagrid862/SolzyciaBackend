using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class updatedOrderModel2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("ALTER TABLE \"OrderProduct\" ALTER COLUMN \"Id\" TYPE bigint USING \"Id\"::bigint;");

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "OrderProduct",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "OrderProduct");

            // Cast the "Id" column back to text
            migrationBuilder.Sql("ALTER TABLE \"OrderProduct\" ALTER COLUMN \"Id\" TYPE text USING \"Id\"::text;");
        }
    }
}
