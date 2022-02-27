using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.MasterMigrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hotel_group_tenants",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    key = table.Column<string>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    connection_string = table.Column<string>(nullable: false),
                    is_active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_hotel_group_tenants", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hotel_group_tenants");
        }
    }
}
