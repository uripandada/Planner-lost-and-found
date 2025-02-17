﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ReAddedRoomBedTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "room_beds",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    room_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    external_id = table.Column<string>(type: "text", nullable: false),
                    is_autogenerated_from_reservation_sync = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_beds", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_beds_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_reservations_room_bed_id",
                table: "reservations",
                column: "room_bed_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_beds_room_id",
                table: "room_beds",
                column: "room_id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservations_room_beds_room_bed_id",
                table: "reservations",
                column: "room_bed_id",
                principalTable: "room_beds",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reservations_room_beds_room_bed_id",
                table: "reservations");

            migrationBuilder.DropTable(
                name: "room_beds");

            migrationBuilder.DropIndex(
                name: "ix_reservations_room_bed_id",
                table: "reservations");
        }
    }
}
