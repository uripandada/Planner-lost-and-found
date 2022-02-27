using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedCompositeUniqueConstraintsToTheRoomsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_rooms_hotel_id",
                table: "rooms");

            migrationBuilder.AddColumn<Guid>(
                name: "room_id",
                table: "reservations",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "ix_rooms_hotel_id_external_id",
                table: "rooms",
                columns: new[] { "hotel_id", "external_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_rooms_hotel_id_name",
                table: "rooms",
                columns: new[] { "hotel_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reservations_room_id",
                table: "reservations",
                column: "room_id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservations_rooms_room_id",
                table: "reservations",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reservations_rooms_room_id",
                table: "reservations");

            migrationBuilder.DropIndex(
                name: "ix_rooms_hotel_id_external_id",
                table: "rooms");

            migrationBuilder.DropIndex(
                name: "ix_rooms_hotel_id_name",
                table: "rooms");

            migrationBuilder.DropIndex(
                name: "ix_reservations_room_id",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "room_id",
                table: "reservations");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_hotel_id",
                table: "rooms",
                column: "hotel_id");
        }
    }
}
