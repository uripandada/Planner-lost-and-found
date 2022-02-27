using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedTimeZoneIdToTheHotelTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "iana_time_zone_id",
                table: "hotels",
                type: "text",
                nullable: false,
                defaultValue: "Etc/GMT");

            migrationBuilder.AddColumn<string>(
                name: "windows_time_zone_id",
                table: "hotels",
                type: "text",
                nullable: false,
                defaultValue: "GMT Standard Time");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "iana_time_zone_id",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "windows_time_zone_id",
                table: "hotels");
        }
    }
}
