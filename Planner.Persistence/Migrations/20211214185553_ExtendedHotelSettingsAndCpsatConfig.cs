using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedHotelSettingsAndCpsatConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("travel_time_weight", "cleaning_plan_cpsat_configurations");

            migrationBuilder.AddColumn<int>(
                name: "weight_travel_time",
                table: "cleaning_plan_cpsat_configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            
            migrationBuilder.DropColumn("max_level_change_count_per_attendant", "cleaning_plan_cpsat_configurations");

            migrationBuilder.AddColumn<int>(
                name: "weight_rooms_cleaned",
                table: "cleaning_plan_cpsat_configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            
            migrationBuilder.DropColumn("max_building_count_per_attendant", "cleaning_plan_cpsat_configurations");

            migrationBuilder.AddColumn<int>(
                name: "weight_level_change",
                table: "cleaning_plan_cpsat_configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            
            migrationBuilder.DropColumn("cleaning_time_weight", "cleaning_plan_cpsat_configurations");

            migrationBuilder.AddColumn<int>(
                name: "weight_credits",
                table: "cleaning_plan_cpsat_configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            //migrationBuilder.RenameColumn(
            //    name: "travel_time_weight",
            //    table: "cleaning_plan_cpsat_configurations",
            //    newName: "weight_travel_time");

            //migrationBuilder.RenameColumn(
            //    name: "max_level_change_count_per_attendant",
            //    table: "cleaning_plan_cpsat_configurations",
            //    newName: "weight_rooms_cleaned");

            //migrationBuilder.RenameColumn(
            //    name: "max_building_count_per_attendant",
            //    table: "cleaning_plan_cpsat_configurations",
            //    newName: "weight_level_change");

            //migrationBuilder.RenameColumn(
            //    name: "cleaning_time_weight",
            //    table: "cleaning_plan_cpsat_configurations",
            //    newName: "weight_credits");

            migrationBuilder.AddColumn<int>(
                name: "building_award",
                table: "settings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "buildings_distance_matrix",
                table: "settings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "cleaning_time",
                table: "settings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "level_award",
                table: "settings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "level_time",
                table: "settings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "levels_distance_matrix",
                table: "settings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "room_award",
                table: "settings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "limit_attendants_per_level",
                table: "cleaning_plan_cpsat_configurations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql($@"ALTER TABLE cleaning_plan_cpsat_configurations ALTER COLUMN limit_attendants_per_level DROP DEFAULT;");

            migrationBuilder.AddColumn<int>(
                name: "max_number_of_buildings_per_attendant",
                table: "cleaning_plan_cpsat_configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "max_number_of_levels_per_attendant",
                table: "cleaning_plan_cpsat_configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "building_award",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "buildings_distance_matrix",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "cleaning_time",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "level_award",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "level_time",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "levels_distance_matrix",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "room_award",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "limit_attendants_per_level",
                table: "cleaning_plan_cpsat_configurations");

            migrationBuilder.DropColumn(
                name: "max_number_of_buildings_per_attendant",
                table: "cleaning_plan_cpsat_configurations");

            migrationBuilder.DropColumn(
                name: "max_number_of_levels_per_attendant",
                table: "cleaning_plan_cpsat_configurations");

            migrationBuilder.DropColumn("weight_travel_time", "cleaning_plan_cpsat_configurations");

            migrationBuilder.AddColumn<int>(
                name: "travel_time_weight",
                table: "cleaning_plan_cpsat_configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropColumn("weight_rooms_cleaned", "cleaning_plan_cpsat_configurations");

            migrationBuilder.AddColumn<int>(
                name: "max_level_change_count_per_attendant",
                table: "cleaning_plan_cpsat_configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropColumn("weight_level_change", "cleaning_plan_cpsat_configurations");

            migrationBuilder.AddColumn<int>(
                name: "max_building_count_per_attendant",
                table: "cleaning_plan_cpsat_configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropColumn("weight_credits", "cleaning_plan_cpsat_configurations");

            migrationBuilder.AddColumn<int>(
                name: "cleaning_time_weight",
                table: "cleaning_plan_cpsat_configurations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
