using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedCleaningPlanItem1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "HotelPlugin");

            //migrationBuilder.DropTable(
            //    name: "plugin");

            migrationBuilder.RenameColumn(
                name: "reservation_id",
                table: "cleaning_plan_items",
                newName: "cleaning_plugin_name");

            migrationBuilder.AddColumn<Guid>(
                name: "cleaning_plugin_id",
                table: "cleaning_plan_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "credits",
                table: "cleaning_plan_items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "key",
                table: "cleaning_plan_items",
                type: "text",
                nullable: false,
                defaultValue: "UNKNOWN");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_items_cleaning_plugin_id",
                table: "cleaning_plan_items",
                column: "cleaning_plugin_id");

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plan_items_cleaning_plugins_cleaning_plugin_id",
                table: "cleaning_plan_items",
                column: "cleaning_plugin_id",
                principalTable: "cleaning_plugins",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plan_items_cleaning_plugins_cleaning_plugin_id",
                table: "cleaning_plan_items");

            migrationBuilder.DropIndex(
                name: "ix_cleaning_plan_items_cleaning_plugin_id",
                table: "cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "cleaning_plugin_id",
                table: "cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "credits",
                table: "cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "key",
                table: "cleaning_plan_items");

            migrationBuilder.RenameColumn(
                name: "cleaning_plugin_name",
                table: "cleaning_plan_items",
                newName: "reservation_id");

            //migrationBuilder.CreateTable(
            //    name: "plugin",
            //    columns: table => new
            //    {
            //        id = table.Column<Guid>(type: "uuid", nullable: false),
            //        description = table.Column<string>(type: "text", nullable: true),
            //        fully_qualified_csharp_type_name = table.Column<string>(type: "text", nullable: false),
            //        is_used_by_default = table.Column<bool>(type: "boolean", nullable: false),
            //        name = table.Column<string>(type: "text", nullable: false),
            //        type_key = table.Column<string>(type: "text", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("pk_plugin", x => x.id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "HotelPlugin",
            //    columns: table => new
            //    {
            //        HotelId = table.Column<string>(type: "text", nullable: true),
            //        PluginId = table.Column<Guid>(type: "uuid", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.ForeignKey(
            //            name: "fk_hotel_plugin_hotels_hotel_id",
            //            column: x => x.HotelId,
            //            principalTable: "hotels",
            //            principalColumn: "id",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "fk_hotel_plugin_plugin_plugin_id",
            //            column: x => x.PluginId,
            //            principalTable: "plugin",
            //            principalColumn: "id",
            //            onDelete: ReferentialAction.Restrict);
            //    });
        }
    }
}
