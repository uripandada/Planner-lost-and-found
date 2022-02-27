using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddRecordTypeColumnInLostAndFound : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "record_type",
                table: "lost_and_founds",
                type: "text",
                nullable: false,
                defaultValue: "Lost");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "record_type",
                table: "lost_and_founds");
        }
    }
}
