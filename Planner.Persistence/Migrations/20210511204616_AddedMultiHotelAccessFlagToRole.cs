using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedMultiHotelAccessFlagToRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<bool>(
            //    name: "has_access_to_multiple_hotels",
            //    table: "asp_net_roles",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "has_access_to_multiple_hotels", table: "asp_net_roles", nullable: true);

            migrationBuilder.Sql(@"
                UPDATE public.asp_net_roles 
                SET has_access_to_multiple_hotels = true
                WHERE normalized_name != 'CLEANER'
            ;");
            migrationBuilder.Sql(@"
                UPDATE public.asp_net_roles 
                SET has_access_to_multiple_hotels = false
                WHERE normalized_name = 'CLEANER'
            ;");

            migrationBuilder.AlterColumn<bool>(name: "has_access_to_multiple_hotels", table: "asp_net_roles", nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "has_access_to_multiple_hotels",
                table: "asp_net_roles");
        }
    }
}
