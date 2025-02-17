﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class RemovedRoomBedSingularTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reservations_room_bed_room_bed_id",
                table: "reservations");

            migrationBuilder.DropTable(
                name: "room_bed");

            migrationBuilder.DropIndex(
                name: "ix_reservations_room_bed_id",
                table: "reservations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "room_bed",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_id = table.Column<string>(type: "text", nullable: false),
                    is_autogenerated_from_reservation_sync = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    room_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_bed", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_bed_rooms_room_id",
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
                name: "ix_room_bed_room_id",
                table: "room_bed",
                column: "room_id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservations_room_bed_room_bed_id",
                table: "reservations",
                column: "room_bed_id",
                principalTable: "room_bed",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
