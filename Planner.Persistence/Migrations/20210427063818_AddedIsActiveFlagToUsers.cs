using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedIsActiveFlagToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<bool>(
            //    name: "is_active",
            //    table: "asp_net_users",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_active", table: "asp_net_users", nullable: true);
            migrationBuilder.Sql("UPDATE public.asp_net_users SET is_active = true;");
            migrationBuilder.AlterColumn<bool>(name: "is_active", table: "asp_net_users", nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "asp_net_users");
        }
    }
}
