using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtensionsToTheCpsatConfiguration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "min_credits_for_multiple_cleaners_cleaning",
                table: "settings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql($@"ALTER TABLE settings ALTER COLUMN min_credits_for_multiple_cleaners_cleaning DROP DEFAULT;");

            migrationBuilder.AddColumn<int>(
                name: "minutes_per_credit",
                table: "settings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql($@"ALTER TABLE settings ALTER COLUMN minutes_per_credit DROP DEFAULT;");

            migrationBuilder.AddColumn<int>(
                name: "weight_credits",
                table: "settings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql($@"ALTER TABLE settings ALTER COLUMN weight_credits DROP DEFAULT;");

            migrationBuilder.AddColumn<int>(
                name: "weight_level_change",
                table: "settings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql($@"ALTER TABLE settings ALTER COLUMN weight_level_change DROP DEFAULT;");

            migrationBuilder.AddColumn<int>(
                name: "max_departures_equivalent_credits",
                table: "cleaning_plan_cpsat_configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql($@"ALTER TABLE cleaning_plan_cpsat_configurations ALTER COLUMN max_departures_equivalent_credits DROP DEFAULT;");

            migrationBuilder.AddColumn<bool>(
                name: "max_departures_reduces_credits",
                table: "cleaning_plan_cpsat_configurations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql($@"ALTER TABLE cleaning_plan_cpsat_configurations ALTER COLUMN max_departures_reduces_credits DROP DEFAULT;");

            migrationBuilder.AddColumn<int>(
                name: "max_departures_reduction_threshold",
                table: "cleaning_plan_cpsat_configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql($@"ALTER TABLE cleaning_plan_cpsat_configurations ALTER COLUMN max_departures_reduction_threshold DROP DEFAULT;");

            migrationBuilder.AddColumn<int>(
                name: "max_stays_equivalent_credits",
                table: "cleaning_plan_cpsat_configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql($@"ALTER TABLE cleaning_plan_cpsat_configurations ALTER COLUMN max_stays_equivalent_credits DROP DEFAULT;");

            migrationBuilder.AddColumn<int>(
                name: "max_stays_increase_threshold",
                table: "cleaning_plan_cpsat_configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql($@"ALTER TABLE cleaning_plan_cpsat_configurations ALTER COLUMN max_stays_increase_threshold DROP DEFAULT;");

            migrationBuilder.AddColumn<bool>(
                name: "max_stays_increases_credits",
                table: "cleaning_plan_cpsat_configurations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql($@"ALTER TABLE cleaning_plan_cpsat_configurations ALTER COLUMN max_stays_increases_credits DROP DEFAULT;");

            migrationBuilder.AddColumn<int>(
                name: "min_credits_for_multiple_cleaners_cleaning",
                table: "cleaning_plan_cpsat_configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql($@"ALTER TABLE cleaning_plan_cpsat_configurations ALTER COLUMN min_credits_for_multiple_cleaners_cleaning DROP DEFAULT;");

            migrationBuilder.AddColumn<int>(
                name: "minutes_per_credit",
                table: "cleaning_plan_cpsat_configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql($@"ALTER TABLE cleaning_plan_cpsat_configurations ALTER COLUMN minutes_per_credit DROP DEFAULT;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "min_credits_for_multiple_cleaners_cleaning",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "minutes_per_credit",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "weight_credits",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "weight_level_change",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "max_departures_equivalent_credits",
                table: "cleaning_plan_cpsat_configurations");

            migrationBuilder.DropColumn(
                name: "max_departures_reduces_credits",
                table: "cleaning_plan_cpsat_configurations");

            migrationBuilder.DropColumn(
                name: "max_departures_reduction_threshold",
                table: "cleaning_plan_cpsat_configurations");

            migrationBuilder.DropColumn(
                name: "max_stays_equivalent_credits",
                table: "cleaning_plan_cpsat_configurations");

            migrationBuilder.DropColumn(
                name: "max_stays_increase_threshold",
                table: "cleaning_plan_cpsat_configurations");

            migrationBuilder.DropColumn(
                name: "max_stays_increases_credits",
                table: "cleaning_plan_cpsat_configurations");

            migrationBuilder.DropColumn(
                name: "min_credits_for_multiple_cleaners_cleaning",
                table: "cleaning_plan_cpsat_configurations");

            migrationBuilder.DropColumn(
                name: "minutes_per_credit",
                table: "cleaning_plan_cpsat_configurations");
        }
    }
}
