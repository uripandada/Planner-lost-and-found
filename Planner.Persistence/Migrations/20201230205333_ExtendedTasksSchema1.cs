using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedTasksSchema1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_system_task_history_asp_net_users_created_by_id",
                table: "system_task_history");

            migrationBuilder.DropForeignKey(
                name: "fk_system_task_history_system_tasks_system_task_id",
                table: "system_task_history");

            migrationBuilder.DropPrimaryKey(
                name: "pk_system_task_history",
                table: "system_task_history");

            migrationBuilder.RenameTable(
                name: "system_task_history",
                newName: "system_task_historys");

            migrationBuilder.RenameIndex(
                name: "ix_system_task_history_system_task_id",
                table: "system_task_historys",
                newName: "ix_system_task_historys_system_task_id");

            migrationBuilder.RenameIndex(
                name: "ix_system_task_history_created_by_id",
                table: "system_task_historys",
                newName: "ix_system_task_historys_created_by_id");

            migrationBuilder.AddColumn<bool>(
                name: "must_be_finished_by_all_whos",
                table: "system_tasks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "recurring_type_key",
                table: "system_tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "repeats_for_key",
                table: "system_tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "type_key",
                table: "system_tasks",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "pk_system_task_historys",
                table: "system_task_historys",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_system_task_historys_asp_net_users_created_by_id",
                table: "system_task_historys",
                column: "created_by_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_system_task_historys_system_tasks_system_task_id",
                table: "system_task_historys",
                column: "system_task_id",
                principalTable: "system_tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_system_task_historys_asp_net_users_created_by_id",
                table: "system_task_historys");

            migrationBuilder.DropForeignKey(
                name: "fk_system_task_historys_system_tasks_system_task_id",
                table: "system_task_historys");

            migrationBuilder.DropPrimaryKey(
                name: "pk_system_task_historys",
                table: "system_task_historys");

            migrationBuilder.DropColumn(
                name: "must_be_finished_by_all_whos",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "recurring_type_key",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "repeats_for_key",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "type_key",
                table: "system_tasks");

            migrationBuilder.RenameTable(
                name: "system_task_historys",
                newName: "system_task_history");

            migrationBuilder.RenameIndex(
                name: "ix_system_task_historys_system_task_id",
                table: "system_task_history",
                newName: "ix_system_task_history_system_task_id");

            migrationBuilder.RenameIndex(
                name: "ix_system_task_historys_created_by_id",
                table: "system_task_history",
                newName: "ix_system_task_history_created_by_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_system_task_history",
                table: "system_task_history",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_system_task_history_asp_net_users_created_by_id",
                table: "system_task_history",
                column: "created_by_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_system_task_history_system_tasks_system_task_id",
                table: "system_task_history",
                column: "system_task_id",
                principalTable: "system_tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
