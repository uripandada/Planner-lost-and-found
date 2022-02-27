using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedCleaningPlanCpsatConfigurationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_cleaning_plans_hotel_id",
                table: "cleaning_plans");

            migrationBuilder.AlterColumn<Guid>(
                name: "postponed_from_plannable_cleaning_plan_item_id",
                table: "plannable_cleaning_plan_items",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "cleaning_plan_cpsat_configurations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    planning_strategy_type_key = table.Column<string>(type: "text", nullable: false),
                    balance_by_rooms_min_rooms = table.Column<int>(type: "integer", nullable: false),
                    balance_by_rooms_max_rooms = table.Column<int>(type: "integer", nullable: false),
                    balance_by_credits_strict_min_credits = table.Column<int>(type: "integer", nullable: false),
                    balance_by_credits_strict_max_credits = table.Column<int>(type: "integer", nullable: false),
                    balance_by_credits_with_affinities_min_credits = table.Column<int>(type: "integer", nullable: false),
                    balance_by_credits_with_affinities_max_credits = table.Column<int>(type: "integer", nullable: false),
                    target_by_rooms_value = table.Column<string>(type: "text", nullable: true),
                    target_by_credits_value = table.Column<string>(type: "text", nullable: true),
                    do_balance_stays_and_departures = table.Column<bool>(type: "boolean", nullable: false),
                    weight_epsilon_stay_departure = table.Column<int>(type: "integer", nullable: false),
                    max_stay = table.Column<int>(type: "integer", nullable: false),
                    max_departure = table.Column<int>(type: "integer", nullable: false),
                    max_travel_time = table.Column<int>(type: "integer", nullable: false),
                    max_building_travel_time = table.Column<int>(type: "integer", nullable: false),
                    max_building_count_per_attendant = table.Column<int>(type: "integer", nullable: false),
                    max_level_change_count_per_attendant = table.Column<int>(type: "integer", nullable: false),
                    room_award = table.Column<int>(type: "integer", nullable: false),
                    level_award = table.Column<int>(type: "integer", nullable: false),
                    building_award = table.Column<int>(type: "integer", nullable: false),
                    travel_time_weight = table.Column<int>(type: "integer", nullable: false),
                    cleaning_time_weight = table.Column<int>(type: "integer", nullable: false),
                    solver_run_time = table.Column<int>(type: "integer", nullable: false),
                    does_level_movement_reduce_credits = table.Column<bool>(type: "boolean", nullable: false),
                    apply_level_movement_credit_reduction_after_number_of_levels = table.Column<int>(type: "integer", nullable: false),
                    level_movement_credits_reduction = table.Column<int>(type: "integer", nullable: false),
                    do_use_pre_plan = table.Column<bool>(type: "boolean", nullable: false),
                    do_use_pre_affinity = table.Column<bool>(type: "boolean", nullable: false),
                    do_complete_proposed_plan_on_use_preplan = table.Column<bool>(type: "boolean", nullable: false),
                    does_building_movement_reduce_credits = table.Column<bool>(type: "boolean", nullable: false),
                    building_movement_credits_reduction = table.Column<int>(type: "integer", nullable: false),
                    are_preferred_levels_exclusive = table.Column<bool>(type: "boolean", nullable: false),
                    cleaning_priority_key = table.Column<string>(type: "text", nullable: false),
                    buildings_distance_matrix = table.Column<string>(type: "text", nullable: true),
                    levels_distance_matrix = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cleaning_plan_cpsat_configurations", x => x.id);
                    table.ForeignKey(
                        name: "fk_cleaning_plan_cpsat_configurations_cleaning_plans_id",
                        column: x => x.id,
                        principalTable: "cleaning_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plans_hotel_id_date",
                table: "cleaning_plans",
                columns: new[] { "hotel_id", "date" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cleaning_plan_cpsat_configurations");

            migrationBuilder.DropIndex(
                name: "ix_cleaning_plans_hotel_id_date",
                table: "cleaning_plans");

            migrationBuilder.AlterColumn<Guid>(
                name: "postponed_from_plannable_cleaning_plan_item_id",
                table: "plannable_cleaning_plan_items",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plans_hotel_id",
                table: "cleaning_plans",
                column: "hotel_id");
        }
    }
}
