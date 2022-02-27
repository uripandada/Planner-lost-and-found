using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedSupportForMultipleActionsPerTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "action_name",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "asset_id",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "asset_model_id",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "asset_name",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "asset_quantity",
                table: "system_tasks");

            migrationBuilder.CreateTable(
                name: "system_task_actions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    action_name = table.Column<string>(type: "text", nullable: false),
                    asset_name = table.Column<string>(type: "text", nullable: false),
                    asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_model_id = table.Column<Guid>(type: "uuid", nullable: true),
                    asset_quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    system_task_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_task_actions", x => x.id);
                    table.ForeignKey(
                        name: "fk_system_task_actions_system_tasks_system_task_id",
                        column: x => x.system_task_id,
                        principalTable: "system_tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_system_task_actions_system_task_id",
                table: "system_task_actions",
                column: "system_task_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "system_task_actions");

            migrationBuilder.AddColumn<string>(
                name: "action_name",
                table: "system_tasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "asset_id",
                table: "system_tasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "asset_model_id",
                table: "system_tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "asset_name",
                table: "system_tasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "asset_quantity",
                table: "system_tasks",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }
    }
}
