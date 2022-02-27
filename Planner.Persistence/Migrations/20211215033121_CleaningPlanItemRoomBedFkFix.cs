using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class CleaningPlanItemRoomBedFkFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plan_items_rooms_room_bed_id",
                table: "cleaning_plan_items");

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plan_items_room_beds_room_bed_id",
                table: "cleaning_plan_items",
                column: "room_bed_id",
                principalTable: "room_beds",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plan_items_room_beds_room_bed_id",
                table: "cleaning_plan_items");

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plan_items_rooms_room_bed_id",
                table: "cleaning_plan_items",
                column: "room_bed_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
