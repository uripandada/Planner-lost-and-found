using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ChangedRoleHotelAccessType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "has_access_to_multiple_hotels",
                table: "asp_net_roles");

            //migrationBuilder.AddColumn<string>(
            //    name: "hotel_access_type_key",
            //    table: "asp_net_roles",
            //    type: "text",
            //    nullable: true);


            migrationBuilder.AddColumn<bool>(name: "hotel_access_type_key", table: "asp_net_roles", nullable: true);

            migrationBuilder.Sql(@"
                UPDATE public.asp_net_roles 
                SET hotel_access_type_key = 'ALL'
                WHERE normalized_name = 'ADMIN'
            ;");
            migrationBuilder.Sql(@"
                UPDATE public.asp_net_roles 
                SET hotel_access_type_key = 'SINGLE'
                WHERE normalized_name = 'CLEANER'
            ;");
            migrationBuilder.Sql(@"
                UPDATE public.asp_net_roles 
                SET hotel_access_type_key = 'MULTIPLE'
                WHERE normalized_name != 'CLEANER' AND normalized_name != 'ADMIN'
            ;");

            migrationBuilder.AlterColumn<bool>(name: "hotel_access_type_key", table: "asp_net_roles", nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hotel_access_type_key",
                table: "asp_net_roles");

            migrationBuilder.AddColumn<bool>(
                name: "has_access_to_multiple_hotels",
                table: "asp_net_roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
