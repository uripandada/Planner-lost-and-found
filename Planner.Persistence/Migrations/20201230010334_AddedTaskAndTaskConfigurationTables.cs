using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Planner.Domain.Entities;

namespace Planner.Persistence.Migrations
{
    public partial class AddedTaskAndTaskConfigurationTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "system_task_configurations",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    data = table.Column<SystemTaskConfigurationData>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_task_configurations", x => x.id);
                    table.ForeignKey(
                        name: "fk_system_task_configurations_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_system_task_configurations_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "system_tasks",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    action_name = table.Column<string>(nullable: false),
                    asset_name = table.Column<string>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    asset_model_id = table.Column<Guid>(nullable: true),
                    who_type_key = table.Column<string>(nullable: false),
                    who_type_description = table.Column<string>(nullable: false),
                    who_reference_id = table.Column<string>(nullable: false),
                    who_reference_name = table.Column<string>(nullable: false),
                    where_type_key = table.Column<string>(nullable: false),
                    where_type_description = table.Column<string>(nullable: false),
                    where_reference_id = table.Column<string>(nullable: false),
                    where_reference_name = table.Column<string>(nullable: false),
                    event_modifier_key = table.Column<string>(nullable: true),
                    event_key = table.Column<string>(nullable: true),
                    event_time_key = table.Column<string>(nullable: true),
                    status_key = table.Column<string>(nullable: false),
                    starts_at = table.Column<DateTime>(nullable: false),
                    system_task_configuration_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_tasks", x => x.id);
                    table.ForeignKey(
                        name: "fk_system_tasks_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_system_tasks_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_system_tasks_system_task_configurations_system_task_configura~",
                        column: x => x.system_task_configuration_id,
                        principalTable: "system_task_configurations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_system_task_configurations_created_by_id",
                table: "system_task_configurations",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_task_configurations_modified_by_id",
                table: "system_task_configurations",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_tasks_created_by_id",
                table: "system_tasks",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_tasks_modified_by_id",
                table: "system_tasks",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_tasks_system_task_configuration_id",
                table: "system_tasks",
                column: "system_task_configuration_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "system_tasks");

            migrationBuilder.DropTable(
                name: "system_task_configurations");
        }
    }
}
