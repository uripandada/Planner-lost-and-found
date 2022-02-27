using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedDefaultForSyncFlagToRoomCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_default_for_reservation_sync",
                table: "room_categorys",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_system_default_for_reservation_sync",
                table: "room_categorys",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_default_for_reservation_sync",
                table: "room_categorys");

            migrationBuilder.DropColumn(
                name: "is_system_default_for_reservation_sync",
                table: "room_categorys");
        }
    }
}
