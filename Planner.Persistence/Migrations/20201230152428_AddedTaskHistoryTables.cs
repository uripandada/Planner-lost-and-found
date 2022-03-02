using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Planner.Domain.Entities;

namespace Planner.Persistence.Migrations
{
    public partial class AddedTaskHistoryTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_manually_modified",
                table: "system_tasks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "system_task_history",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    system_task_id = table.Column<Guid>(nullable: false),
                    created_by_id = table.Column<Guid>(nullable: true),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    changed_by_key = table.Column<string>(nullable: false),
                    message = table.Column<string>(nullable: false),
                    old_data = table.Column<SystemTaskHistoryData>(type: "jsonb", nullable: false),
                    new_data = table.Column<SystemTaskHistoryData>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_task_history", x => x.id);
                    table.ForeignKey(
                        name: "fk_system_task_history_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_system_task_history_system_tasks_system_task_id",
                        column: x => x.system_task_id,
                        principalTable: "system_tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_system_task_history_created_by_id",
                table: "system_task_history",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_task_history_system_task_id",
                table: "system_task_history",
                column: "system_task_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "system_task_history");

            migrationBuilder.DropColumn(
                name: "is_manually_modified",
                table: "system_tasks");
        }
    }
}
