using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedRoomHistoryEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "room_bed_id",
                table: "room_history_events",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_room_history_events_room_bed_id",
                table: "room_history_events",
                column: "room_bed_id");

            migrationBuilder.AddForeignKey(
                name: "fk_room_history_events_room_beds_room_bed_id",
                table: "room_history_events",
                column: "room_bed_id",
                principalTable: "room_beds",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_room_history_events_room_beds_room_bed_id",
                table: "room_history_events");

            migrationBuilder.DropIndex(
                name: "ix_room_history_events_room_bed_id",
                table: "room_history_events");

            migrationBuilder.DropColumn(
                name: "room_bed_id",
                table: "room_history_events");
        }
    }
}
