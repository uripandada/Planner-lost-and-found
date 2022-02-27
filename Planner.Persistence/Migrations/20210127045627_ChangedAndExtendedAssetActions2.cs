using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ChangedAndExtendedAssetActions2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_quick",
                table: "asset_actions");

            migrationBuilder.AddColumn<string>(
                name: "quick_or_timed_key",
                table: "asset_actions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_asset_actions_default_assigned_to_user_id",
                table: "asset_actions",
                column: "default_assigned_to_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asset_actions_asp_net_users_default_assigned_to_user_id",
                table: "asset_actions",
                column: "default_assigned_to_user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asset_actions_asp_net_users_default_assigned_to_user_id",
                table: "asset_actions");

            migrationBuilder.DropIndex(
                name: "ix_asset_actions_default_assigned_to_user_id",
                table: "asset_actions");

            migrationBuilder.DropColumn(
                name: "quick_or_timed_key",
                table: "asset_actions");

            migrationBuilder.AddColumn<bool>(
                name: "is_quick",
                table: "asset_actions",
                type: "boolean",
                nullable: true);
        }
    }
}
