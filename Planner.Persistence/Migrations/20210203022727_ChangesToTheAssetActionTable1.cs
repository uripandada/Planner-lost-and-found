using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ChangesToTheAssetActionTable1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_blocking",
                table: "asset_actions");

            migrationBuilder.DropColumn(
                name: "is_high_priority",
                table: "asset_actions");

            migrationBuilder.DropColumn(
                name: "is_mandatory",
                table: "asset_actions");

            migrationBuilder.AddColumn<Guid>(
                name: "default_assigned_to_user_group_id",
                table: "asset_actions",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "default_assigned_to_user_sub_group_id",
                table: "asset_actions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_asset_actions_default_assigned_to_user_group_id",
                table: "asset_actions",
                column: "default_assigned_to_user_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_actions_default_assigned_to_user_sub_group_id",
                table: "asset_actions",
                column: "default_assigned_to_user_sub_group_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asset_actions_user_groups_default_assigned_to_user_group_id",
                table: "asset_actions",
                column: "default_assigned_to_user_group_id",
                principalTable: "user_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_asset_actions_user_sub_groups_default_assigned_to_user_sub_gr~",
                table: "asset_actions",
                column: "default_assigned_to_user_sub_group_id",
                principalTable: "user_sub_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asset_actions_user_groups_default_assigned_to_user_group_id",
                table: "asset_actions");

            migrationBuilder.DropForeignKey(
                name: "fk_asset_actions_user_sub_groups_default_assigned_to_user_sub_gr~",
                table: "asset_actions");

            migrationBuilder.DropIndex(
                name: "ix_asset_actions_default_assigned_to_user_group_id",
                table: "asset_actions");

            migrationBuilder.DropIndex(
                name: "ix_asset_actions_default_assigned_to_user_sub_group_id",
                table: "asset_actions");

            migrationBuilder.DropColumn(
                name: "default_assigned_to_user_group_id",
                table: "asset_actions");

            migrationBuilder.DropColumn(
                name: "default_assigned_to_user_sub_group_id",
                table: "asset_actions");

            migrationBuilder.AddColumn<bool>(
                name: "is_blocking",
                table: "asset_actions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_high_priority",
                table: "asset_actions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_mandatory",
                table: "asset_actions",
                type: "boolean",
                nullable: true);
        }
    }
}
