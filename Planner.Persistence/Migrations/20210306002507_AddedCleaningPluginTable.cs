using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Planner.Domain.Entities;

namespace Planner.Persistence.Migrations
{
    public partial class AddedCleaningPluginTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cleaning_plugins",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    hotel_id = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    data = table.Column<CleaningPluginJson>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cleaning_plugins", x => x.id);
                    table.ForeignKey(
                        name: "fk_cleaning_plugins_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cleaning_plugins_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cleaning_plugins_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plugins_created_by_id",
                table: "cleaning_plugins",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plugins_hotel_id",
                table: "cleaning_plugins",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plugins_modified_by_id",
                table: "cleaning_plugins",
                column: "modified_by_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cleaning_plugins");
        }
    }
}
