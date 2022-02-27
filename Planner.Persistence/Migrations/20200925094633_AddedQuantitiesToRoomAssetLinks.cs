using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedQuantitiesToRoomAssetLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "quantity",
                table: "room_assets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "quantity",
                table: "room_asset_models",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "available_quantity",
                table: "asset_models",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quantity",
                table: "room_assets");

            migrationBuilder.DropColumn(
                name: "quantity",
                table: "room_asset_models");

            migrationBuilder.DropColumn(
                name: "available_quantity",
                table: "asset_models");
        }
    }
}
