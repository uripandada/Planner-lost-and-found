using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedCleaningsWithPlannedFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_planned",
                table: "cleanings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql($@"UPDATE cleanings SET is_planned = true;");
            migrationBuilder.Sql($@"ALTER TABLE cleanings ALTER COLUMN is_planned DROP DEFAULT;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_planned",
                table: "cleanings");
        }
    }
}
