using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Planner.Persistence.Migrations
{
    public partial class AddedNewFieldsToLostAndFoundTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "client_name",
                table: "lost_and_founds",
                type: "text",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "founder_name",
                table: "lost_and_founds",
                type: "text",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "founder_email",
                table: "lost_and_founds",
                type: "text",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "founder_phone_number",
                table: "lost_and_founds",
                type: "text",
                nullable: true);
            migrationBuilder.AddColumn<Guid>(
                name: "storage_room_id",
                table: "lost_and_founds",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "client_name",
                table: "lost_and_founds");
            migrationBuilder.DropColumn(
                name: "founder_name",
                table: "lost_and_founds");
            migrationBuilder.DropColumn(
                name: "founder_email",
                table: "lost_and_founds");
            migrationBuilder.DropColumn(
                name: "founder_phone_number",
                table: "lost_and_founds");
            migrationBuilder.DropColumn(
                name: "storage_room_id",
                table: "lost_and_founds");
        }
    }
}
