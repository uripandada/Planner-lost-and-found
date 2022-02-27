using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Planner.Persistence.Migrations
{
    public partial class RelinkedAssetActionsToAssetGroups1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asset_actions_assets_asset_id",
                table: "asset_actions");

            //migrationBuilder.RenameColumn(
            //    name: "asset_id",
            //    table: "asset_actions",
            //    newName: "asset_group_id");

            //migrationBuilder.DropColumn(
            //    name: "asset_id",
            //    table: "asset_actions");
            
            migrationBuilder.AddColumn<Guid>(
                name: "asset_group_id",
                table: "asset_actions",
                nullable: true
                );

            //migrationBuilder.RenameIndex(
            //    name: "ix_asset_actions_asset_id",
            //    table: "asset_actions",
            //    newName: "ix_asset_actions_asset_group_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asset_actions_asset_groups_asset_group_id",
                table: "asset_actions",
                column: "asset_group_id",
                principalTable: "asset_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DropForeignKey(
				name: "fk_asset_actions_asset_groups_asset_group_id",
				table: "asset_actions");

			//migrationBuilder.RenameColumn(
			//    name: "asset_group_id",
			//    table: "asset_actions",
			//    newName: "asset_id");
			migrationBuilder.DropColumn(
				name: "asset_group_id",
				table: "asset_actions");

			//migrationBuilder.RenameIndex(
   //             name: "ix_asset_actions_asset_group_id",
   //             table: "asset_actions",
   //             newName: "ix_asset_actions_asset_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asset_actions_assets_asset_id",
                table: "asset_actions",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
