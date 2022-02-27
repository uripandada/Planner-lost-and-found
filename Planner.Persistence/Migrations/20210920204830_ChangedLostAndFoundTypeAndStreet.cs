using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ChangedLostAndFoundTypeAndStreet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "street",
                table: "lost_and_founds");

            migrationBuilder.RenameColumn(
                name: "record_type",
                table: "lost_and_founds",
                newName: "type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "type",
                table: "lost_and_founds",
                newName: "record_type");

            migrationBuilder.AddColumn<string>(
                name: "street",
                table: "lost_and_founds",
                type: "text",
                nullable: true);
        }
    }
}
