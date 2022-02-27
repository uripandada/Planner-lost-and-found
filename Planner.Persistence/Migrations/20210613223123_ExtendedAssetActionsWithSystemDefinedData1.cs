using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedAssetActionsWithSystemDefinedData1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<bool>(
            //    name: "is_system_defined",
            //    table: "asset_actions",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false,
            //    oldClrType: typeof(bool),
            //    oldType: "boolean",
            //    oldNullable: true);


            migrationBuilder.Sql("UPDATE public.asset_actions SET is_system_defined = false WHERE is_system_defined IS NULL;");
            migrationBuilder.AlterColumn<bool>(name: "is_system_defined", table: "asset_actions", nullable: false);

            //migrationBuilder.AddColumn<string>(
            //    name: "system_asset_action_move_type_key",
            //    table: "asset_actions",
            //    type: "text",
            //    nullable: false,
            //    defaultValue: "");


            migrationBuilder.AddColumn<string>(name: "system_asset_action_move_type_key", table: "asset_actions", nullable: true);
            migrationBuilder.Sql("UPDATE public.asset_actions SET system_asset_action_move_type_key = 'NONE';");
            migrationBuilder.AlterColumn<string>(name: "system_asset_action_move_type_key", table: "asset_actions", nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "system_asset_action_move_type_key",
                table: "asset_actions");

            migrationBuilder.AlterColumn<bool>(name: "is_system_defined", table: "asset_actions", nullable: true);
        }
    }
}
