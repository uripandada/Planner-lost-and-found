using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedAssetFileAndTagTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "assets",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTime>(nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    hotel_id = table.Column<string>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    is_available_to_maintenance = table.Column<bool>(nullable: false),
                    is_available_to_housekeeping = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_assets", x => x.id);
                    table.ForeignKey(
                        name: "fk_assets_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_assets_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_assets_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    key = table.Column<string>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTime>(nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    hotel_id = table.Column<string>(nullable: false),
                    value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.key);
                    table.ForeignKey(
                        name: "fk_tags_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tags_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tags_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTime>(nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    hotel_id = table.Column<string>(nullable: false),
                    file_name = table.Column<string>(nullable: false),
                    file_data = table.Column<byte[]>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_files", x => x.id);
                    table.ForeignKey(
                        name: "fk_files_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_files_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_files_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_files_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "asset_tags",
                columns: table => new
                {
                    asset_id = table.Column<Guid>(nullable: false),
                    tag_key = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asset_tags", x => new { x.asset_id, x.tag_key });
                    table.ForeignKey(
                        name: "fk_asset_tags_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_asset_tags_tags_tag_key",
                        column: x => x.tag_key,
                        principalTable: "tags",
                        principalColumn: "key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_asset_tags_tag_key",
                table: "asset_tags",
                column: "tag_key");

            migrationBuilder.CreateIndex(
                name: "ix_assets_created_by_id",
                table: "assets",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_assets_hotel_id",
                table: "assets",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_assets_modified_by_id",
                table: "assets",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_files_asset_id",
                table: "files",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_files_created_by_id",
                table: "files",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_files_hotel_id",
                table: "files",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_files_modified_by_id",
                table: "files",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_created_by_id",
                table: "tags",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_hotel_id",
                table: "tags",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_modified_by_id",
                table: "tags",
                column: "modified_by_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asset_tags");

            migrationBuilder.DropTable(
                name: "files");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "assets");
        }
    }
}
