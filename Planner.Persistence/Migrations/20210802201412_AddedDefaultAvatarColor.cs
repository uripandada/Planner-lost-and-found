using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedDefaultAvatarColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultAvatarColorHex",
                table: "asp_net_users",
                type: "text",
                nullable: false,
                defaultValue: "#EEEEEE");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultAvatarColorHex",
                table: "asp_net_users");
        }
    }
}
