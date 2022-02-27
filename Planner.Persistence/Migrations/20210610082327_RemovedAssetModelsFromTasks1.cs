using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class RemovedAssetModelsFromTasks1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "asset_model_id",
                table: "system_task_actions");

            migrationBuilder.AddColumn<Guid>(
                name: "asset_group_id",
                table: "system_task_actions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "asset_group_name",
                table: "system_task_actions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "asset_group_id",
                table: "system_task_actions");

            migrationBuilder.DropColumn(
                name: "asset_group_name",
                table: "system_task_actions");

            migrationBuilder.AddColumn<Guid>(
                name: "asset_model_id",
                table: "system_task_actions",
                type: "uuid",
                nullable: true);
        }
    }
}
