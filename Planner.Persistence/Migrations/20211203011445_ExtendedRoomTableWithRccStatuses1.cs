using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedRoomTableWithRccStatuses1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<bool>(
            //    name: "is_inspected",
            //    table: "rooms",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_inspected", table: "rooms", nullable: true);
            migrationBuilder.Sql("UPDATE public.rooms SET is_inspected = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_inspected", table: "rooms", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_out_of_service",
            //    table: "rooms",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_out_of_service", table: "rooms", nullable: true);
            migrationBuilder.Sql("UPDATE public.rooms SET is_out_of_service = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_out_of_service", table: "rooms", nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "rcc_housekeeping_status",
                table: "rooms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rcc_room_status",
                table: "rooms",
                type: "text",
                nullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "modified_at",
            //    table: "cleanings",
            //    type: "timestamp without time zone",
            //    nullable: false,
            //    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            //migrationBuilder.AddColumn<DateTime>(name: "modified_at", table: "cleanings", nullable: true);
            //migrationBuilder.Sql("UPDATE public.cleanings SET modified_at = now();");
            //migrationBuilder.AlterColumn<DateTime>(name: "modified_at", table: "cleanings", nullable: false);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_inspected",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "is_out_of_service",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "rcc_housekeeping_status",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "rcc_room_status",
                table: "rooms");

            //migrationBuilder.DropColumn(
            //    name: "modified_at",
            //    table: "cleanings");
        }
    }
}
