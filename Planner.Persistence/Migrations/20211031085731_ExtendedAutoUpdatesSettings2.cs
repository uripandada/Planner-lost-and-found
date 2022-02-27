using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedAutoUpdatesSettings2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "automatic_housekeeping_update_settingss",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<string>(
                name: "hotel_id",
                table: "automatic_housekeeping_update_settingss",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_automatic_housekeeping_update_settingss_hotel_id",
                table: "automatic_housekeeping_update_settingss",
                column: "hotel_id");

            migrationBuilder.AddForeignKey(
                name: "fk_automatic_housekeeping_update_settingss_hotels_hotel_id",
                table: "automatic_housekeeping_update_settingss",
                column: "hotel_id",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_automatic_housekeeping_update_settingss_hotels_hotel_id",
                table: "automatic_housekeeping_update_settingss");

            migrationBuilder.DropIndex(
                name: "ix_automatic_housekeeping_update_settingss_hotel_id",
                table: "automatic_housekeeping_update_settingss");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "automatic_housekeeping_update_settingss");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "automatic_housekeeping_update_settingss");
        }
    }
}
