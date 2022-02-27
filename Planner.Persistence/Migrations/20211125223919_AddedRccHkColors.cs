using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedRccHkColors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rcc_housekeeping_status_colors",
                columns: table => new
                {
                    rcc_code = table.Column<string>(type: "text", nullable: false),
                    color_hex = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rcc_housekeeping_status_colors", x => x.rcc_code);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rcc_housekeeping_status_colors");
        }
    }
}
