using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedAssetModelColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_available_to_housekeeping",
                table: "asset_models",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_available_to_maintenance",
                table: "asset_models",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_available_to_housekeeping",
                table: "asset_models");

            migrationBuilder.DropColumn(
                name: "is_available_to_maintenance",
                table: "asset_models");
        }
    }
}
