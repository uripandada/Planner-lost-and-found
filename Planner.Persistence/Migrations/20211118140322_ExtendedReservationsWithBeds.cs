using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedReservationsWithBeds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "external_id",
                table: "room_bed",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "bed_name",
                table: "reservations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pmsbed_name",
                table: "reservations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "room_bed_id",
                table: "reservations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_reservations_room_bed_id",
                table: "reservations",
                column: "room_bed_id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservations_room_bed_room_bed_id",
                table: "reservations",
                column: "room_bed_id",
                principalTable: "room_bed",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reservations_room_bed_room_bed_id",
                table: "reservations");

            migrationBuilder.DropIndex(
                name: "ix_reservations_room_bed_id",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "external_id",
                table: "room_bed");

            migrationBuilder.DropColumn(
                name: "bed_name",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "pmsbed_name",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "room_bed_id",
                table: "reservations");
        }
    }
}
