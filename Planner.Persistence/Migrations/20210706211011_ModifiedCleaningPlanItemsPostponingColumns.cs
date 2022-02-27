using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ModifiedCleaningPlanItemsPostponingColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "postponed_to_date",
                table: "plannable_cleaning_plan_items");

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_planned",
            //    table: "plannable_cleaning_plan_items",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_planned", table: "plannable_cleaning_plan_items", nullable: true);
            migrationBuilder.Sql("UPDATE public.plannable_cleaning_plan_items SET is_planned = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_planned", table: "plannable_cleaning_plan_items", nullable: false);

            migrationBuilder.AddColumn<Guid>(
                name: "postponed_from_plannable_cleaning_plan_item_id",
                table: "plannable_cleaning_plan_items",
                type: "uuid",
                nullable: true);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_active",
            //    table: "cleaning_plan_items",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_active", table: "cleaning_plan_items", nullable: true);
            migrationBuilder.Sql("UPDATE public.cleaning_plan_items SET is_active = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_active", table: "cleaning_plan_items", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_custom",
            //    table: "cleaning_plan_items",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_custom", table: "cleaning_plan_items", nullable: true);
            migrationBuilder.Sql("UPDATE public.cleaning_plan_items SET is_custom = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_custom", table: "cleaning_plan_items", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_postponed",
            //    table: "cleaning_plan_items",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_postponed", table: "cleaning_plan_items", nullable: true);
            migrationBuilder.Sql("UPDATE public.cleaning_plan_items SET is_postponed = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_postponed", table: "cleaning_plan_items", nullable: false);

            migrationBuilder.CreateIndex(
                name: "ix_plannable_cleaning_plan_items_postponed_from_plannable_clea~",
                table: "plannable_cleaning_plan_items",
                column: "postponed_from_plannable_cleaning_plan_item_id");

            migrationBuilder.AddForeignKey(
                name: "fk_plannable_cleaning_plan_items_plannable_cleaning_plan_items~",
                table: "plannable_cleaning_plan_items",
                column: "postponed_from_plannable_cleaning_plan_item_id",
                principalTable: "plannable_cleaning_plan_items",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_plannable_cleaning_plan_items_plannable_cleaning_plan_items~",
                table: "plannable_cleaning_plan_items");

            migrationBuilder.DropIndex(
                name: "ix_plannable_cleaning_plan_items_postponed_from_plannable_clea~",
                table: "plannable_cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "is_planned",
                table: "plannable_cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "postponed_from_plannable_cleaning_plan_item_id",
                table: "plannable_cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "is_custom",
                table: "cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "is_postponed",
                table: "cleaning_plan_items");

            migrationBuilder.AddColumn<DateTime>(
                name: "postponed_to_date",
                table: "plannable_cleaning_plan_items",
                type: "timestamp without time zone",
                nullable: true);
        }
    }
}
