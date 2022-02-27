using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class RemovedOldCleaningPluginTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hotel_plugin");

            migrationBuilder.DropTable(
                name: "plugin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "plugin",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    fully_qualified_csharp_type_name = table.Column<string>(type: "text", nullable: false),
                    is_used_by_default = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    type_key = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plugin", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hotel_plugin",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    hotel_id = table.Column<string>(type: "text", nullable: false),
                    plugin_data = table.Column<string>(type: "jsonb", nullable: true),
                    plugin_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_hotel_plugin", x => x.id);
                    table.ForeignKey(
                        name: "fk_hotel_plugin_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_hotel_plugin_plugin_plugin_id",
                        column: x => x.plugin_id,
                        principalTable: "plugin",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_hotel_plugin_hotel_id",
                table: "hotel_plugin",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_hotel_plugin_plugin_id",
                table: "hotel_plugin",
                column: "plugin_id");
        }
    }
}
