using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class RemovedReferenceToHotelIdFromAssets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asset_models_hotels_hotel_id",
                table: "asset_models");

            migrationBuilder.DropForeignKey(
                name: "fk_assets_hotels_hotel_id",
                table: "assets");

            migrationBuilder.DropIndex(
                name: "ix_assets_hotel_id",
                table: "assets");

            migrationBuilder.DropIndex(
                name: "ix_asset_models_hotel_id",
                table: "asset_models");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "assets");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "asset_models");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "hotel_id",
                table: "assets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "hotel_id",
                table: "asset_models",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_assets_hotel_id",
                table: "assets",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_models_hotel_id",
                table: "asset_models",
                column: "hotel_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asset_models_hotels_hotel_id",
                table: "asset_models",
                column: "hotel_id",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_assets_hotels_hotel_id",
                table: "assets",
                column: "hotel_id",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
