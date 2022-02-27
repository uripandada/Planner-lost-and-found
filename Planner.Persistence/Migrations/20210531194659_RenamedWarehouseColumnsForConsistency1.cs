using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class RenamedWarehouseColumnsForConsistency1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_central_storage",
                table: "warehouses",
                newName: "is_central");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_central",
                table: "warehouses",
                newName: "is_central_storage");
        }
    }
}
