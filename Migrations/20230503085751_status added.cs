using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace College.Migrations
{
    public partial class statusadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "joinedEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "event_idEventId",
                table: "joinedEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_joinedEvents_event_idEventId",
                table: "joinedEvents",
                column: "event_idEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_joinedEvents_events_event_idEventId",
                table: "joinedEvents",
                column: "event_idEventId",
                principalTable: "events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_joinedEvents_events_event_idEventId",
                table: "joinedEvents");

            migrationBuilder.DropIndex(
                name: "IX_joinedEvents_event_idEventId",
                table: "joinedEvents");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "joinedEvents");

            migrationBuilder.DropColumn(
                name: "event_idEventId",
                table: "joinedEvents");
        }
    }
}
