using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class RemovedAvailabilityFlagsFromAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_available_to_housekeeping",
                table: "assets");

            migrationBuilder.DropColumn(
                name: "is_available_to_maintenance",
                table: "assets");

            migrationBuilder.DropColumn(
                name: "is_available_to_housekeeping",
                table: "asset_models");

            migrationBuilder.DropColumn(
                name: "is_available_to_maintenance",
                table: "asset_models");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_available_to_housekeeping",
                table: "assets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_available_to_maintenance",
                table: "assets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_available_to_housekeeping",
                table: "asset_models",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_available_to_maintenance",
                table: "asset_models",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
