using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.MasterMigrations
{
    public partial class AddedExternalClientsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "external_client_secret_keys",
                columns: table => new
                {
                    client_id = table.Column<string>(type: "text", nullable: false),
                    key = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_external_client_secret_keys", x => new { x.client_id, x.key });
                });

            migrationBuilder.InsertData(
                table: "external_client_secret_keys",
                columns: new[] { "client_id", "key", "is_active" },
                values: new object[] { "RCC", "testing-rcc-secret-key", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "external_client_secret_keys");
        }
    }
}
