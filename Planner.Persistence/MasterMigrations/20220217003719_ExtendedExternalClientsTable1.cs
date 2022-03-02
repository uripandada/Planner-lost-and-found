using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.MasterMigrations
{
    public partial class ExtendedExternalClientsTable1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "has_access_to_list_of_hotel_groups",
                table: "external_client_secret_keys",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql($@"ALTER TABLE external_client_secret_keys ALTER COLUMN has_access_to_list_of_hotel_groups DROP DEFAULT;");

            migrationBuilder.AddColumn<bool>(
                name: "has_access_to_list_of_hotels",
                table: "external_client_secret_keys",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql($@"ALTER TABLE external_client_secret_keys ALTER COLUMN has_access_to_list_of_hotels DROP DEFAULT;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "has_access_to_list_of_hotel_groups",
                table: "external_client_secret_keys");

            migrationBuilder.DropColumn(
                name: "has_access_to_list_of_hotels",
                table: "external_client_secret_keys");
        }
    }
}
