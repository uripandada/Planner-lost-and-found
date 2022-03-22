using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedStatusToExperienceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "experience_ticket_status",
                table: "experiences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "experience_client_relation_status",
                table: "experiences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "experience_resolution_status",
                table: "experiences",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "experience_ticket_status",
                table: "experiences");
            migrationBuilder.DropColumn(
                name: "experience_client_relation_status",
                table: "experiences");
            migrationBuilder.DropColumn(
                name: "experience_resolution_status",
                table: "experiences");
        }
    }
}
