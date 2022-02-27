using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedReservationsWithCurrentStatuses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active_today",
                table: "reservations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql($@"ALTER TABLE reservations ALTER COLUMN is_active_today DROP DEFAULT;");

            migrationBuilder.AddColumn<string>(
                name: "reservation_status_description",
                table: "reservations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reservation_status_key",
                table: "reservations",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active_today",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "reservation_status_description",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "reservation_status_key",
                table: "reservations");
        }
    }
}
