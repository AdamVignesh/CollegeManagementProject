using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace College.Migrations
{
    public partial class updatedstudents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_students_departments_department_idDeptId",
                table: "students");

            migrationBuilder.AlterColumn<int>(
                name: "department_idDeptId",
                table: "students",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_students_departments_department_idDeptId",
                table: "students",
                column: "department_idDeptId",
                principalTable: "departments",
                principalColumn: "DeptId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_students_departments_department_idDeptId",
                table: "students");

            migrationBuilder.AlterColumn<int>(
                name: "department_idDeptId",
                table: "students",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_students_departments_department_idDeptId",
                table: "students",
                column: "department_idDeptId",
                principalTable: "departments",
                principalColumn: "DeptId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
