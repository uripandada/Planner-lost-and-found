using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class RenamedColumnsInLostAndFound : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "description",
                table: "lost_and_founds",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "item_name",
                table: "lost_and_founds",
                newName: "description");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
               name: "description",
               table: "lost_and_founds",
               newName: "item_name");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "lost_and_founds",
                newName: "description");
        }
    }
}
