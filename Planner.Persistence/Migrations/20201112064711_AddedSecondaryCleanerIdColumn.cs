using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedSecondaryCleanerIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "secondary_cleaner_id",
                table: "cleaning_plan_groups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_groups_secondary_cleaner_id",
                table: "cleaning_plan_groups",
                column: "secondary_cleaner_id");

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plan_groups_asp_net_users_secondary_cleaner_id",
                table: "cleaning_plan_groups",
                column: "secondary_cleaner_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plan_groups_asp_net_users_secondary_cleaner_id",
                table: "cleaning_plan_groups");

            migrationBuilder.DropIndex(
                name: "ix_cleaning_plan_groups_secondary_cleaner_id",
                table: "cleaning_plan_groups");

            migrationBuilder.DropColumn(
                name: "secondary_cleaner_id",
                table: "cleaning_plan_groups");
        }
    }
}
