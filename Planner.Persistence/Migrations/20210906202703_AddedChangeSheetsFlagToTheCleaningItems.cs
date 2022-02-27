using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedChangeSheetsFlagToTheCleaningItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<bool>(
            //    name: "is_change_sheets",
            //    table: "plannable_cleaning_plan_items",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_change_sheets", table: "plannable_cleaning_plan_items", nullable: true);
            migrationBuilder.Sql("UPDATE public.plannable_cleaning_plan_items SET is_change_sheets = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_change_sheets", table: "plannable_cleaning_plan_items", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_change_sheets",
            //    table: "cleaning_plan_items",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_change_sheets", table: "cleaning_plan_items", nullable: true);
            migrationBuilder.Sql("UPDATE public.cleaning_plan_items SET is_change_sheets = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_change_sheets", table: "cleaning_plan_items", nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_change_sheets",
                table: "plannable_cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "is_change_sheets",
                table: "cleaning_plan_items");
        }
    }
}
