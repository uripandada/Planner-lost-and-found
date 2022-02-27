using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.MasterMigrations
{
    public partial class AddedDatabaseNameFieldToTheTenantTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "hotel_group_tenants",
                keyColumn: "id",
                keyValue: new Guid("d8cb04b6-b797-4f97-a67d-1702056333a4"));

            migrationBuilder.DeleteData(
                table: "hotel_group_tenants",
                keyColumn: "id",
                keyValue: new Guid("d0ca67f1-e5aa-40b5-8d5d-818030cc6187"));

            migrationBuilder.DeleteData(
                table: "hotel_group_tenants",
                keyColumn: "id",
                keyValue: new Guid("03009625-a8cc-46a1-991a-7b3f5f49d594"));

            migrationBuilder.DeleteData(
                table: "hotel_group_tenants",
                keyColumn: "id",
                keyValue: new Guid("f6290cb3-7b66-4cef-8c3e-5a44254d7f1f"));

            /*migrationBuilder.AddColumn<string>(
                name: "database_name",
                table: "hotel_group_tenants",
                type: "text",
                nullable: false,
                defaultValue: "");*/

            migrationBuilder.AddColumn<bool>(name: "database_name", table: "hotel_group_tenants", nullable: true, type: "text");
            migrationBuilder.Sql("UPDATE public.hotel_group_tenants SET database_name = 'UNKNOWN_DATABASE';");
            migrationBuilder.AlterColumn<bool>(name: "database_name", table: "hotel_group_tenants", nullable: false);

            /*migrationBuilder.InsertData(
                table: "hotel_group_tenants",
                columns: new[] { "id", "connection_string", "database_name", "is_active", "key", "name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_Roomchecking;Pooling=true;", "hgtest_Roomchecking", true, "Roomchecking", "Roomchecking" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_test_group_1;Pooling=true;", "hgtest_test_group_1", true, "test_group_1", "Test hotel group 1" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_test_group_2;Pooling=true;", "hgtest_test_group_2", true, "test_group_2", "Test hotel group 2" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_test_group_3;Pooling=true;", "hgtest_test_group_3", true, "test_group_3", "Test hotel group 3" }
                });*/
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "hotel_group_tenants",
                keyColumn: "id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "hotel_group_tenants",
                keyColumn: "id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "hotel_group_tenants",
                keyColumn: "id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "hotel_group_tenants",
                keyColumn: "id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"));

            migrationBuilder.DropColumn(
                name: "database_name",
                table: "hotel_group_tenants");

            migrationBuilder.InsertData(
                table: "hotel_group_tenants",
                columns: new[] { "id", "connection_string", "is_active", "key", "name" },
                values: new object[,]
                {
                    { new Guid("d8cb04b6-b797-4f97-a67d-1702056333a4"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_Roomchecking;Pooling=true;", true, "Roomchecking", "Roomchecking" },
                    { new Guid("d0ca67f1-e5aa-40b5-8d5d-818030cc6187"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_test_group_1;Pooling=true;", true, "test_group_1", "Test hotel group 1" },
                    { new Guid("03009625-a8cc-46a1-991a-7b3f5f49d594"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_test_group_2;Pooling=true;", true, "test_group_2", "Test hotel group 2" },
                    { new Guid("f6290cb3-7b66-4cef-8c3e-5a44254d7f1f"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_test_group_3;Pooling=true;", true, "test_group_3", "Test hotel group 3" }
                });
        }
    }
}
