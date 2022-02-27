using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedReservedQuantityOnTheAssetAvailability1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "reserved_quantity",
                table: "warehouse_asset_availabilities",
                type: "integer",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reserved_quantity",
                table: "warehouse_asset_availabilities");
        }
    }
}
