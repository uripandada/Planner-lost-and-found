using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedNewStatusFound : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "lost_and_founds");

            migrationBuilder.AddColumn<string>(
                name: "found_status",
                table: "lost_and_founds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "guest_status",
                table: "lost_and_founds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "delivery_status",
                table: "lost_and_founds",
                type: "int",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.AddColumn<string>(
                name: "other_status",
                table: "lost_and_founds",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "found_status",
                table: "lost_and_founds");
            migrationBuilder.DropColumn(
                name: "guest_status",
                table: "lost_and_founds");
            migrationBuilder.DropColumn(
                name: "delivery_status",
                table: "lost_and_founds");
            migrationBuilder.DropColumn(
                name: "other_status",
                table: "lost_and_founds");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "lost_and_founds",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
