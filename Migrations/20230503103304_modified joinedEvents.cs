using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace College.Migrations
{
    public partial class modifiedjoinedEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_joinedEvents_events_event_idEventId",
                table: "joinedEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_joinedEvents_students_reg_noRegNo",
                table: "joinedEvents");

            migrationBuilder.DropIndex(
                name: "IX_joinedEvents_event_idEventId",
                table: "joinedEvents");

            migrationBuilder.DropIndex(
                name: "IX_joinedEvents_reg_noRegNo",
                table: "joinedEvents");

            migrationBuilder.DropColumn(
                name: "event_idEventId",
                table: "joinedEvents");

            migrationBuilder.RenameColumn(
                name: "reg_noRegNo",
                table: "joinedEvents",
                newName: "event_id");

            migrationBuilder.AddColumn<string>(
                name: "reg_no",
                table: "joinedEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reg_no",
                table: "joinedEvents");

            migrationBuilder.RenameColumn(
                name: "event_id",
                table: "joinedEvents",
                newName: "reg_noRegNo");

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

            migrationBuilder.CreateIndex(
                name: "IX_joinedEvents_reg_noRegNo",
                table: "joinedEvents",
                column: "reg_noRegNo");

            migrationBuilder.AddForeignKey(
                name: "FK_joinedEvents_events_event_idEventId",
                table: "joinedEvents",
                column: "event_idEventId",
                principalTable: "events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_joinedEvents_students_reg_noRegNo",
                table: "joinedEvents",
                column: "reg_noRegNo",
                principalTable: "students",
                principalColumn: "RegNo",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
