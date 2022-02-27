using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.MasterMigrations
{
    public partial class TenantDataSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "hotel_group_tenants",
                columns: new[] { "id", "connection_string", "is_active", "key", "name" },
                values: new object[,]
                {
                    { new Guid("f3702e2c-a97d-4b3a-ae41-f6a491d71647"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_Roomchecking;Pooling=true;", true, "Roomchecking", "Roomchecking" },
                    { new Guid("5ae67a86-d4b2-4416-83f5-38e7e16c9835"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_test_group_1;Pooling=true;", true, "test_group_1", "Test hotel group 1" },
                    { new Guid("48e3affc-0d98-424c-8845-84dac1f9e175"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_test_group_2;Pooling=true;", true, "test_group_2", "Test hotel group 2" },
                    { new Guid("e9bb11d1-166c-4094-9580-c0fb83b3267d"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_test_group_3;Pooling=true;", true, "test_group_3", "Test hotel group 3" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "hotel_group_tenants",
                keyColumn: "id",
                keyValue: new Guid("48e3affc-0d98-424c-8845-84dac1f9e175"));

            migrationBuilder.DeleteData(
                table: "hotel_group_tenants",
                keyColumn: "id",
                keyValue: new Guid("5ae67a86-d4b2-4416-83f5-38e7e16c9835"));

            migrationBuilder.DeleteData(
                table: "hotel_group_tenants",
                keyColumn: "id",
                keyValue: new Guid("e9bb11d1-166c-4094-9580-c0fb83b3267d"));

            migrationBuilder.DeleteData(
                table: "hotel_group_tenants",
                keyColumn: "id",
                keyValue: new Guid("f3702e2c-a97d-4b3a-ae41-f6a491d71647"));
        }
    }
}
