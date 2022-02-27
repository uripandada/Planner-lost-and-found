using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedRoomAndBedWithIsGuestCurrentlyIn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_guest_currently_in",
                table: "rooms",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql($@"ALTER TABLE rooms ALTER COLUMN is_guest_currently_in DROP DEFAULT;");

            migrationBuilder.AddColumn<bool>(
                name: "is_guest_currently_in",
                table: "room_beds",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql($@"ALTER TABLE room_beds ALTER COLUMN is_guest_currently_in DROP DEFAULT;");

            migrationBuilder.AddColumn<bool>(
                name: "is_inspected",
                table: "room_beds",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql($@"ALTER TABLE room_beds ALTER COLUMN is_inspected DROP DEFAULT;");

            migrationBuilder.AddColumn<bool>(
                name: "is_out_of_service",
                table: "room_beds",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql($@"ALTER TABLE room_beds ALTER COLUMN is_out_of_service DROP DEFAULT;");

            migrationBuilder.AddColumn<string>(
                name: "rcc_housekeeping_status",
                table: "room_beds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rcc_room_status",
                table: "room_beds",
                type: "text",
                nullable: true);

            migrationBuilder.Sql($@"UPDATE rooms SET rcc_housekeeping_status = '{Common.Enums.RccHousekeepingStatusCode.HC.ToString()}';");
            migrationBuilder.Sql($@"UPDATE rooms SET rcc_room_status = '{Common.Enums.RccRoomStatusCode.VAC.ToString()}';");
            migrationBuilder.Sql($@"UPDATE room_beds SET rcc_housekeeping_status = '{Common.Enums.RccHousekeepingStatusCode.HC.ToString()}';");
            migrationBuilder.Sql($@"UPDATE room_beds SET rcc_room_status = '{Common.Enums.RccRoomStatusCode.VAC.ToString()}';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_guest_currently_in",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "is_guest_currently_in",
                table: "room_beds");

            migrationBuilder.DropColumn(
                name: "is_inspected",
                table: "room_beds");

            migrationBuilder.DropColumn(
                name: "is_out_of_service",
                table: "room_beds");

            migrationBuilder.DropColumn(
                name: "rcc_housekeeping_status",
                table: "room_beds");

            migrationBuilder.DropColumn(
                name: "rcc_room_status",
                table: "room_beds");
        }
    }
}
