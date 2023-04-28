using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace College.Migrations
{
    public partial class adddedeventsandjoinedEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    venue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "joinedEvents",
                columns: table => new
                {
                    JoinedEventsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    reg_noRegNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_joinedEvents", x => x.JoinedEventsId);
                    table.ForeignKey(
                        name: "FK_joinedEvents_students_reg_noRegNo",
                        column: x => x.reg_noRegNo,
                        principalTable: "students",
                        principalColumn: "RegNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_joinedEvents_reg_noRegNo",
                table: "joinedEvents",
                column: "reg_noRegNo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "joinedEvents");
        }
    }
}
