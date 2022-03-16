using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ChangeCategoryTableNameToLostAndFoundCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable( name: "categorys", newName: "lost_and_found_categories");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable( name: "lost_and_found_categories", newName: "categorys");
        }
    }
}
