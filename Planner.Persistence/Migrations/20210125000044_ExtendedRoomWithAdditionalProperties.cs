using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedRoomWithAdditionalProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "floor_section_name",
                table: "rooms",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "floor_sub_section_name",
                table: "rooms",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "floor_section_name",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "floor_sub_section_name",
                table: "rooms");
        }
    }
}
