using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedRoomsWithCleaningPriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_cleaning_priority",
                table: "rooms",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql($@"ALTER TABLE rooms ALTER COLUMN is_cleaning_priority DROP DEFAULT;");

            migrationBuilder.AddColumn<bool>(
                name: "is_cleaning_priority",
                table: "room_beds",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql($@"ALTER TABLE room_beds ALTER COLUMN is_cleaning_priority DROP DEFAULT;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_cleaning_priority",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "is_cleaning_priority",
                table: "room_beds");
        }
    }
}
