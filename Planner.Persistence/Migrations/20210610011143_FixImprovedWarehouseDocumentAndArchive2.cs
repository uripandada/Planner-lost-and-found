using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class FixImprovedWarehouseDocumentAndArchive2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "concurrency_token",
                table: "warehouse_asset_availabilities");

            migrationBuilder.DropColumn(
                name: "concurrency_token",
                table: "room_asset_usages");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "warehouse_asset_availabilities",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "room_asset_usages",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "warehouse_asset_availabilities");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "room_asset_usages");

            migrationBuilder.AddColumn<byte[]>(
                name: "concurrency_token",
                table: "warehouse_asset_availabilities",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "concurrency_token",
                table: "room_asset_usages",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
