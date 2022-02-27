using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedCleaningPlanPlannableItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "key",
                table: "cleaning_plan_items");

            migrationBuilder.CreateTable(
                name: "plannable_cleaning_plan_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cleaning_plugin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cleaning_plugin_name = table.Column<string>(type: "text", nullable: true),
                    credits = table.Column<int>(type: "integer", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_custom = table.Column<bool>(type: "boolean", nullable: false),
                    is_postponed = table.Column<bool>(type: "boolean", nullable: false),
                    postponed_to_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    room_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cleaning_plan_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plannable_cleaning_plan_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_plannable_cleaning_plan_items_cleaning_plans_cleaning_plan_~",
                        column: x => x.cleaning_plan_id,
                        principalTable: "cleaning_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_plannable_cleaning_plan_items_cleaning_plugins_cleaning_plu~",
                        column: x => x.cleaning_plugin_id,
                        principalTable: "cleaning_plugins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_plannable_cleaning_plan_items_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_plannable_cleaning_plan_items_cleaning_plan_id",
                table: "plannable_cleaning_plan_items",
                column: "cleaning_plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_plannable_cleaning_plan_items_cleaning_plugin_id",
                table: "plannable_cleaning_plan_items",
                column: "cleaning_plugin_id");

            migrationBuilder.CreateIndex(
                name: "ix_plannable_cleaning_plan_items_room_id",
                table: "plannable_cleaning_plan_items",
                column: "room_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "plannable_cleaning_plan_items");

            migrationBuilder.AddColumn<string>(
                name: "key",
                table: "cleaning_plan_items",
                type: "text",
                nullable: false,
                defaultValue: "UNKNOWN");
        }
    }
}
