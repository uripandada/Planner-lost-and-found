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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
