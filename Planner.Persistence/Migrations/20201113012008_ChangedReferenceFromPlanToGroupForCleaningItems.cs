using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ChangedReferenceFromPlanToGroupForCleaningItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plan_items_asp_net_users_cleaner_id",
                table: "cleaning_plan_items");

            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plan_items_cleaning_plans_cleaning_plan_id",
                table: "cleaning_plan_items");

            migrationBuilder.DropIndex(
                name: "ix_cleaning_plan_items_cleaner_id",
                table: "cleaning_plan_items");

            migrationBuilder.DropIndex(
                name: "ix_cleaning_plan_items_cleaning_plan_id",
                table: "cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "cleaner_id",
                table: "cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "cleaning_plan_id",
                table: "cleaning_plan_items");

            migrationBuilder.AddColumn<Guid>(
                name: "cleaning_plan_group_id",
                table: "cleaning_plan_items",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_items_cleaning_plan_group_id",
                table: "cleaning_plan_items",
                column: "cleaning_plan_group_id");

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plan_items_cleaning_plan_groups_cleaning_plan_grou~",
                table: "cleaning_plan_items",
                column: "cleaning_plan_group_id",
                principalTable: "cleaning_plan_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plan_items_cleaning_plan_groups_cleaning_plan_grou~",
                table: "cleaning_plan_items");

            migrationBuilder.DropIndex(
                name: "ix_cleaning_plan_items_cleaning_plan_group_id",
                table: "cleaning_plan_items");

            migrationBuilder.DropColumn(
                name: "cleaning_plan_group_id",
                table: "cleaning_plan_items");

            migrationBuilder.AddColumn<Guid>(
                name: "cleaner_id",
                table: "cleaning_plan_items",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "cleaning_plan_id",
                table: "cleaning_plan_items",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_items_cleaner_id",
                table: "cleaning_plan_items",
                column: "cleaner_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_items_cleaning_plan_id",
                table: "cleaning_plan_items",
                column: "cleaning_plan_id");

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plan_items_asp_net_users_cleaner_id",
                table: "cleaning_plan_items",
                column: "cleaner_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plan_items_cleaning_plans_cleaning_plan_id",
                table: "cleaning_plan_items",
                column: "cleaning_plan_id",
                principalTable: "cleaning_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
