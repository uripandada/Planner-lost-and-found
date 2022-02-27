using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ChangedAssetActionTableSchemaAndReferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asset_actions_hotels_hotel_id",
                table: "asset_actions");

            migrationBuilder.DropForeignKey(
                name: "fk_asset_actions_assets_id",
                table: "asset_actions");

            migrationBuilder.DropIndex(
                name: "ix_asset_actions_hotel_id",
                table: "asset_actions");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "asset_actions");

            migrationBuilder.AddColumn<Guid>(
                name: "asset_id",
                table: "asset_actions",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "asset_model_id",
                table: "asset_actions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_asset_actions_asset_id",
                table: "asset_actions",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_actions_asset_model_id",
                table: "asset_actions",
                column: "asset_model_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asset_actions_assets_asset_id",
                table: "asset_actions",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_asset_actions_asset_models_asset_model_id",
                table: "asset_actions",
                column: "asset_model_id",
                principalTable: "asset_models",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asset_actions_assets_asset_id",
                table: "asset_actions");

            migrationBuilder.DropForeignKey(
                name: "fk_asset_actions_asset_models_asset_model_id",
                table: "asset_actions");

            migrationBuilder.DropIndex(
                name: "ix_asset_actions_asset_id",
                table: "asset_actions");

            migrationBuilder.DropIndex(
                name: "ix_asset_actions_asset_model_id",
                table: "asset_actions");

            migrationBuilder.DropColumn(
                name: "asset_id",
                table: "asset_actions");

            migrationBuilder.DropColumn(
                name: "asset_model_id",
                table: "asset_actions");

            migrationBuilder.AddColumn<string>(
                name: "hotel_id",
                table: "asset_actions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_asset_actions_hotel_id",
                table: "asset_actions",
                column: "hotel_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asset_actions_hotels_hotel_id",
                table: "asset_actions",
                column: "hotel_id",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_asset_actions_assets_id",
                table: "asset_actions",
                column: "id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
