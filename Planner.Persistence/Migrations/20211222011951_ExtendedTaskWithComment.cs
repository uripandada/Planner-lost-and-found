using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedTaskWithComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "comment",
                table: "system_tasks",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "comment",
                table: "system_tasks");
        }
    }
}
