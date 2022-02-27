using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ChangedAndExtendedAssetActions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asset_actions_asset_models_asset_model_id",
                table: "asset_actions");

            migrationBuilder.DropIndex(
                name: "ix_asset_actions_asset_model_id",
                table: "asset_actions");

            migrationBuilder.DropColumn(
                name: "asset_model_id",
                table: "asset_actions");

            migrationBuilder.AlterColumn<Guid>(
                name: "asset_id",
                table: "asset_actions",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "asset_id",
                table: "asset_actions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "asset_model_id",
                table: "asset_actions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_asset_actions_asset_model_id",
                table: "asset_actions",
                column: "asset_model_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asset_actions_asset_models_asset_model_id",
                table: "asset_actions",
                column: "asset_model_id",
                principalTable: "asset_models",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
