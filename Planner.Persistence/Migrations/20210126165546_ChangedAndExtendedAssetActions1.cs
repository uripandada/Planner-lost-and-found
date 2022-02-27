using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ChangedAndExtendedAssetActions1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "credits",
                table: "asset_actions",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "default_assigned_to_user_id",
                table: "asset_actions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_mandatory",
                table: "asset_actions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_quick",
                table: "asset_actions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_system_defined",
                table: "asset_actions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "price",
                table: "asset_actions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "priority_key",
                table: "asset_actions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "credits",
                table: "asset_actions");

            migrationBuilder.DropColumn(
                name: "default_assigned_to_user_id",
                table: "asset_actions");

            migrationBuilder.DropColumn(
                name: "is_mandatory",
                table: "asset_actions");

            migrationBuilder.DropColumn(
                name: "is_quick",
                table: "asset_actions");

            migrationBuilder.DropColumn(
                name: "is_system_defined",
                table: "asset_actions");

            migrationBuilder.DropColumn(
                name: "price",
                table: "asset_actions");

            migrationBuilder.DropColumn(
                name: "priority_key",
                table: "asset_actions");
        }
    }
}
