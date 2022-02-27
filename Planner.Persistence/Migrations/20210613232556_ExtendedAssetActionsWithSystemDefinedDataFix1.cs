using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedAssetActionsWithSystemDefinedDataFix1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "system_asset_action_move_type_key",
                table: "asset_actions",
                newName: "system_action_type_key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "system_action_type_key",
                table: "asset_actions",
                newName: "system_asset_action_move_type_key");
        }
    }
}
