using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedPlanAndCleaningsAddedNightlyCycleConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "sent_by_id",
                table: "cleaning_plans",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cleaning_status",
                table: "cleaning_plan_items",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "inspected_by_id",
                table: "cleaning_plan_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_inspected",
                table: "cleaning_plan_items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_inspection_required",
                table: "cleaning_plan_items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_inspection_success",
                table: "cleaning_plan_items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_ready_for_inspection",
                table: "cleaning_plan_items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_sent",
                table: "cleaning_plan_items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "automatic_housekeeping_update_settingss",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    dirty = table.Column<bool>(type: "boolean", nullable: false),
                    clean = table.Column<bool>(type: "boolean", nullable: false),
                    clean_needs_inspection = table.Column<bool>(type: "boolean", nullable: false),
                    inspected = table.Column<bool>(type: "boolean", nullable: false),
                    vacant = table.Column<bool>(type: "boolean", nullable: false),
                    occupied = table.Column<bool>(type: "boolean", nullable: false),
                    do_not_disturb = table.Column<bool>(type: "boolean", nullable: false),
                    do_disturb = table.Column<bool>(type: "boolean", nullable: false),
                    out_of_service = table.Column<bool>(type: "boolean", nullable: false),
                    in_service = table.Column<bool>(type: "boolean", nullable: false),
                    room_name_regex = table.Column<string>(type: "text", nullable: false),
                    update_status_to = table.Column<string>(type: "text", nullable: false),
                    update_status_when = table.Column<string>(type: "text", nullable: false),
                    update_status_at_time = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_automatic_housekeeping_update_settingss", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cleaning_inspections",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cleaning_plan_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ended_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    is_finished = table.Column<bool>(type: "boolean", nullable: false),
                    is_success = table.Column<bool>(type: "boolean", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cleaning_inspections", x => x.id);
                    table.ForeignKey(
                        name: "fk_cleaning_inspections_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cleaning_inspections_cleaning_plan_items_cleaning_plan_item_id",
                        column: x => x.cleaning_plan_item_id,
                        principalTable: "cleaning_plan_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cleaning_plan_item_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_system_event = table.Column<bool>(type: "boolean", nullable: false),
                    event_type = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    message = table.Column<string>(type: "text", nullable: true),
                    old_state = table.Column<string>(type: "text", nullable: true),
                    new_state = table.Column<string>(type: "text", nullable: true),
                    cleaning_plan_item_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cleaning_plan_item_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_cleaning_plan_item_events_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cleaning_plan_item_events_cleaning_plan_items_cleaning_plan~",
                        column: x => x.cleaning_plan_item_id,
                        principalTable: "cleaning_plan_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cleaning_plan_sending_histories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    sent_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cleaning_plan_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cleaning_plan_sending_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_cleaning_plan_sending_histories_asp_net_users_sent_by_id",
                        column: x => x.sent_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cleaning_plan_sending_histories_cleaning_plans_cleaning_pla~",
                        column: x => x.cleaning_plan_id,
                        principalTable: "cleaning_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "housekeeping_nightly_update_cycles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ended_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    in_progress = table.Column<bool>(type: "boolean", nullable: false),
                    state_changes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_housekeeping_nightly_update_cycles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rcc_products",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    external_name = table.Column<string>(type: "text", nullable: false),
                    service_id = table.Column<string>(type: "text", nullable: false),
                    category_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rcc_products", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plans_sent_by_id",
                table: "cleaning_plans",
                column: "sent_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_items_inspected_by_id",
                table: "cleaning_plan_items",
                column: "inspected_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_inspections_cleaning_plan_item_id",
                table: "cleaning_inspections",
                column: "cleaning_plan_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_inspections_created_by_id",
                table: "cleaning_inspections",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_item_events_cleaning_plan_item_id",
                table: "cleaning_plan_item_events",
                column: "cleaning_plan_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_item_events_created_by_id",
                table: "cleaning_plan_item_events",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_sending_histories_cleaning_plan_id",
                table: "cleaning_plan_sending_histories",
                column: "cleaning_plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_sending_histories_sent_by_id",
                table: "cleaning_plan_sending_histories",
                column: "sent_by_id");

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plan_items_asp_net_users_inspected_by_id",
                table: "cleaning_plan_items",
                column: "inspected_by_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plans_asp_net_users_sent_by_id",
                table: "cleaning_plans",
                column: "sent_by_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plan_items_asp_net_users_inspected_by_id",
                table: "cleaning_plan_items");

            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plans_asp_net_users_sent_by_id",
                table: "cleaning_plans");

            migrationBuilder.DropTable(
                name: "automatic_housekeeping_update_settingss");

            migrationBuilder.DropTable(
                name: "cleaning_inspections");

            migrationBuilder.DropTable(
                name: "cleaning_plan_item_events");

            migrationBuilder.DropTable(
                name: "cleaning_plan_sending_histories");

            migrationBuilder.DropTable(
                name: "housekeeping_nightly_update_cycles");

            migrationBuilder.DropTable(
                name: "rcc_products");

            migrationBuilder.DropIndex(
                name: "ix_cleaning_plans_sent_by_id",
                table: "cleaning_plans");

            migrationBuilder.DropIndex(
                name: "ix_cleaning_plan_items_inspected_by_id",
                table: "cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "sent_by_id",
                table: "cleaning_plans");

            migrationBuilder.DropColumn(
                name: "cleaning_status",
                table: "cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "inspected_by_id",
                table: "cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "is_inspected",
                table: "cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "is_inspection_required",
                table: "cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "is_inspection_success",
                table: "cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "is_ready_for_inspection",
                table: "cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "is_sent",
                table: "cleaning_plan_items");
        }
    }
}
