using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedTasksForPlannedAttendant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                table: "system_tasks",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<bool>(
                name: "is_for_planned_attendant",
                table: "system_tasks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql($@"ALTER TABLE system_tasks ALTER COLUMN is_for_planned_attendant DROP DEFAULT;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_for_planned_attendant",
                table: "system_tasks");

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                table: "system_tasks",
                type: "uuid",
                nullable: false,
                //defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
