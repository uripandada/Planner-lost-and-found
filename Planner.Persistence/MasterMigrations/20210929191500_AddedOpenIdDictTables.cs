using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.MasterMigrations
{
    public partial class AddedOpenIdDictTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DeleteData(
            //    table: "hotel_group_tenants",
            //    keyColumn: "id",
            //    keyValue: new Guid("15493b3b-0aa7-4fcf-8d09-0f22bd076510"));

            //migrationBuilder.DeleteData(
            //    table: "hotel_group_tenants",
            //    keyColumn: "id",
            //    keyValue: new Guid("307fc9c6-4c8c-4022-9068-6a0ab0ceb6c1"));

            //migrationBuilder.DeleteData(
            //    table: "hotel_group_tenants",
            //    keyColumn: "id",
            //    keyValue: new Guid("6fb8062a-a790-4525-b0fc-dd9a4bb371da"));

            //migrationBuilder.DeleteData(
            //    table: "hotel_group_tenants",
            //    keyColumn: "id",
            //    keyValue: new Guid("92afb035-d29d-44c6-948c-12d40677c282"));

            migrationBuilder.AddColumn<string>(
                name: "hotel_access_type_key",
                table: "asp_net_roles",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "open_iddict_applications",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    client_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    client_secret = table.Column<string>(type: "text", nullable: true),
                    concurrency_token = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    consent_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    display_names = table.Column<string>(type: "text", nullable: true),
                    permissions = table.Column<string>(type: "text", nullable: true),
                    post_logout_redirect_uris = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redirect_uris = table.Column<string>(type: "text", nullable: true),
                    requirements = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_open_iddict_applications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "open_iddict_scopes",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    concurrency_token = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    descriptions = table.Column<string>(type: "text", nullable: true),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    display_names = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    resources = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_open_iddict_scopes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "open_iddict_authorizations",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    application_id = table.Column<string>(type: "text", nullable: true),
                    concurrency_token = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    creation_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    scopes = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_open_iddict_authorizations", x => x.id);
                    table.ForeignKey(
                        name: "fk_open_iddict_authorizations_open_iddict_applications_applica~",
                        column: x => x.application_id,
                        principalTable: "open_iddict_applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "open_iddict_tokens",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    application_id = table.Column<string>(type: "text", nullable: true),
                    authorization_id = table.Column<string>(type: "text", nullable: true),
                    concurrency_token = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    creation_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    expiration_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    payload = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redemption_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    reference_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_open_iddict_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_open_iddict_tokens_open_iddict_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "open_iddict_applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_open_iddict_tokens_open_iddict_authorizations_authorization~",
                        column: x => x.authorization_id,
                        principalTable: "open_iddict_authorizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            //migrationBuilder.InsertData(
            //    table: "hotel_group_tenants",
            //    columns: new[] { "id", "connection_string", "database_name", "is_active", "key", "name" },
            //    values: new object[,]
            //    {
            //        { new Guid("00000000-0000-0000-0000-000000000001"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_Roomchecking;Pooling=true;", "hgtest_Roomchecking", true, "Roomchecking", "Roomchecking" },
            //        { new Guid("00000000-0000-0000-0000-000000000002"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_test_group_1;Pooling=true;", "hgtest_test_group_1", true, "test_group_1", "Test hotel group 1" },
            //        { new Guid("00000000-0000-0000-0000-000000000003"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_test_group_2;Pooling=true;", "hgtest_test_group_2", true, "test_group_2", "Test hotel group 2" },
            //        { new Guid("00000000-0000-0000-0000-000000000004"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_test_group_3;Pooling=true;", "hgtest_test_group_3", true, "test_group_3", "Test hotel group 3" }
            //    });

            migrationBuilder.CreateIndex(
                name: "ix_open_iddict_applications_client_id",
                table: "open_iddict_applications",
                column: "client_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_open_iddict_authorizations_application_id_status_subject_ty~",
                table: "open_iddict_authorizations",
                columns: new[] { "application_id", "status", "subject", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_open_iddict_scopes_name",
                table: "open_iddict_scopes",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_open_iddict_tokens_application_id_status_subject_type",
                table: "open_iddict_tokens",
                columns: new[] { "application_id", "status", "subject", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_open_iddict_tokens_authorization_id",
                table: "open_iddict_tokens",
                column: "authorization_id");

            migrationBuilder.CreateIndex(
                name: "ix_open_iddict_tokens_reference_id",
                table: "open_iddict_tokens",
                column: "reference_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "open_iddict_scopes");

            migrationBuilder.DropTable(
                name: "open_iddict_tokens");

            migrationBuilder.DropTable(
                name: "open_iddict_authorizations");

            migrationBuilder.DropTable(
                name: "open_iddict_applications");

            //migrationBuilder.DeleteData(
            //    table: "hotel_group_tenants",
            //    keyColumn: "id",
            //    keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            //migrationBuilder.DeleteData(
            //    table: "hotel_group_tenants",
            //    keyColumn: "id",
            //    keyValue: new Guid("00000000-0000-0000-0000-000000000002"));

            //migrationBuilder.DeleteData(
            //    table: "hotel_group_tenants",
            //    keyColumn: "id",
            //    keyValue: new Guid("00000000-0000-0000-0000-000000000003"));

            //migrationBuilder.DeleteData(
            //    table: "hotel_group_tenants",
            //    keyColumn: "id",
            //    keyValue: new Guid("00000000-0000-0000-0000-000000000004"));

            migrationBuilder.DropColumn(
                name: "hotel_access_type_key",
                table: "asp_net_roles");

            //migrationBuilder.InsertData(
            //    table: "hotel_group_tenants",
            //    columns: new[] { "id", "connection_string", "database_name", "is_active", "key", "name" },
            //    values: new object[,]
            //    {
            //        { new Guid("6fb8062a-a790-4525-b0fc-dd9a4bb371da"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_Roomchecking;Pooling=true;", "hgtest_Roomchecking", true, "Roomchecking", "Roomchecking" },
            //        { new Guid("307fc9c6-4c8c-4022-9068-6a0ab0ceb6c1"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_test_group_1;Pooling=true;", "hgtest_test_group_1", true, "test_group_1", "Test hotel group 1" },
            //        { new Guid("92afb035-d29d-44c6-948c-12d40677c282"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_test_group_2;Pooling=true;", "hgtest_test_group_2", true, "test_group_2", "Test hotel group 2" },
            //        { new Guid("15493b3b-0aa7-4fcf-8d09-0f22bd076510"), "User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_test_group_3;Pooling=true;", "hgtest_test_group_3", true, "test_group_3", "Test hotel group 3" }
            //    });
        }
    }
}
