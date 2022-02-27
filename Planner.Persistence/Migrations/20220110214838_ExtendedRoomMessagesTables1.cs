using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedRoomMessagesTables1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "room_bed_id",
                table: "room_message_rooms",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_room_message_rooms_room_bed_id",
                table: "room_message_rooms",
                column: "room_bed_id");

            migrationBuilder.AddForeignKey(
                name: "fk_room_message_rooms_rooms_room_bed_id",
                table: "room_message_rooms",
                column: "room_bed_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_room_message_rooms_rooms_room_bed_id",
                table: "room_message_rooms");

            migrationBuilder.DropIndex(
                name: "ix_room_message_rooms_room_bed_id",
                table: "room_message_rooms");

            migrationBuilder.DropColumn(
                name: "room_bed_id",
                table: "room_message_rooms");
        }
    }
}
