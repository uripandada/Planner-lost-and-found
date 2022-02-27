using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ModifiedRoomsTable1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_rooms_reservations_occupied_by_reservation_id",
                table: "rooms");

            migrationBuilder.DropIndex(
                name: "ix_rooms_occupied_by_reservation_id",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "occupied_by_reservation_id",
                table: "rooms");

            migrationBuilder.AlterColumn<Guid>(
                name: "room_id",
                table: "reservations",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "occupied_by_reservation_id",
                table: "rooms",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "room_id",
                table: "reservations",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_rooms_occupied_by_reservation_id",
                table: "rooms",
                column: "occupied_by_reservation_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_rooms_reservations_occupied_by_reservation_id",
                table: "rooms",
                column: "occupied_by_reservation_id",
                principalTable: "reservations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
