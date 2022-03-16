using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExchangeNameFieldOnFoundAndLostTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "lost_and_founds",
                newName: "name");

            migrationBuilder.DropColumn(
                name: "first_name",
                table: "lost_and_founds");

            migrationBuilder.CreateIndex(
                name: "ix_lost_and_founds_lost_and_found_category_id",
                table: "lost_and_founds",
                column: "lost_and_found_category_id");

            migrationBuilder.AddForeignKey(
                name: "fk_lost_and_founds_categorys_lost_and_found_category_id",
                table: "lost_and_founds",
                column: "lost_and_found_category_id",
                principalTable: "categorys",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_lost_and_founds_categorys_lost_and_found_category_id",
                table: "lost_and_founds");

            migrationBuilder.DropIndex(
                name: "ix_lost_and_founds_lost_and_found_category_id",
                table: "lost_and_founds");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "lost_and_founds",
                newName: "last_name");

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                table: "lost_and_founds",
                type: "text",
                nullable: true);
        }
    }
}
