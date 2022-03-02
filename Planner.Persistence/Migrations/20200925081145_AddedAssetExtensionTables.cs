using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedAssetExtensionTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "uses_models",
                table: "assets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "asset_actions",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    hotel_id = table.Column<string>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    is_blocking = table.Column<bool>(nullable: false),
                    is_high_priority = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asset_actions", x => x.id);
                    table.ForeignKey(
                        name: "fk_asset_actions_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_asset_actions_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_asset_actions_assets_id",
                        column: x => x.id,
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_asset_actions_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "asset_models",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    hotel_id = table.Column<string>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asset_models", x => x.id);
                    table.ForeignKey(
                        name: "fk_asset_models_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_asset_models_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_asset_models_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_asset_models_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "room_assets",
                columns: table => new
                {
                    room_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_assets", x => new { x.room_id, x.asset_id });
                    table.ForeignKey(
                        name: "fk_room_assets_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_room_assets_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "room_asset_models",
                columns: table => new
                {
                    room_id = table.Column<Guid>(nullable: false),
                    asset_model_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_asset_models", x => new { x.room_id, x.asset_model_id });
                    table.ForeignKey(
                        name: "fk_room_asset_models_asset_models_asset_model_id",
                        column: x => x.asset_model_id,
                        principalTable: "asset_models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_room_asset_models_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_asset_actions_created_by_id",
                table: "asset_actions",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_actions_hotel_id",
                table: "asset_actions",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_actions_modified_by_id",
                table: "asset_actions",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_models_asset_id",
                table: "asset_models",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_models_created_by_id",
                table: "asset_models",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_models_hotel_id",
                table: "asset_models",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_models_modified_by_id",
                table: "asset_models",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_asset_models_asset_model_id",
                table: "room_asset_models",
                column: "asset_model_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_assets_asset_id",
                table: "room_assets",
                column: "asset_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asset_actions");

            migrationBuilder.DropTable(
                name: "room_asset_models");

            migrationBuilder.DropTable(
                name: "room_assets");

            migrationBuilder.DropTable(
                name: "asset_models");

            migrationBuilder.DropColumn(
                name: "uses_models",
                table: "assets");
        }
    }
}
