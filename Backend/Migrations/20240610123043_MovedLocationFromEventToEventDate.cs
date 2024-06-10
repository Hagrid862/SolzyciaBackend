using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class MovedLocationFromEventToEventDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventLocation_LocationId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_LocationId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Events");

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "EventDates",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventDates_LocationId",
                table: "EventDates",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventDates_EventLocation_LocationId",
                table: "EventDates",
                column: "LocationId",
                principalTable: "EventLocation",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventDates_EventLocation_LocationId",
                table: "EventDates");

            migrationBuilder.DropIndex(
                name: "IX_EventDates_LocationId",
                table: "EventDates");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "EventDates");

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "Events",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_LocationId",
                table: "Events",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventLocation_LocationId",
                table: "Events",
                column: "LocationId",
                principalTable: "EventLocation",
                principalColumn: "Id");
        }
    }
}
