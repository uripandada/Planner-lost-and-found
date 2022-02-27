using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedWarehouseGroupAndAssetTableExtensions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asset_models_assets_asset_id",
                table: "asset_models");

            migrationBuilder.DropColumn(
                name: "available_quantity",
                table: "assets");

            migrationBuilder.DropColumn(
                name: "uses_models",
                table: "assets");

            migrationBuilder.AddColumn<Guid>(
                name: "asset_group_id",
                table: "assets",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "asset_sub_group_id",
                table: "assets",
                type: "uuid",
                nullable: true);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_bulk",
            //    table: "assets",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_bulk", table: "assets", nullable: true);
            migrationBuilder.Sql("UPDATE public.assets SET is_bulk = true;");
            migrationBuilder.AlterColumn<bool>(name: "is_bulk", table: "assets", nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "serial_number",
                table: "assets",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "asset_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_asset_group_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    type_key = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asset_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_asset_groups_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_asset_groups_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_asset_groups_asset_groups_parent_asset_group_id",
                        column: x => x.parent_asset_group_id,
                        principalTable: "asset_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_assets_asset_group_id",
                table: "assets",
                column: "asset_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_assets_asset_sub_group_id",
                table: "assets",
                column: "asset_sub_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_groups_created_by_id",
                table: "asset_groups",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_groups_modified_by_id",
                table: "asset_groups",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_groups_parent_asset_group_id",
                table: "asset_groups",
                column: "parent_asset_group_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asset_models_assets_asset_id",
                table: "asset_models",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_assets_asset_groups_asset_group_id",
                table: "assets",
                column: "asset_group_id",
                principalTable: "asset_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_assets_asset_groups_asset_sub_group_id",
                table: "assets",
                column: "asset_sub_group_id",
                principalTable: "asset_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asset_models_assets_asset_id",
                table: "asset_models");

            migrationBuilder.DropForeignKey(
                name: "fk_assets_asset_groups_asset_group_id",
                table: "assets");

            migrationBuilder.DropForeignKey(
                name: "fk_assets_asset_groups_asset_sub_group_id",
                table: "assets");

            migrationBuilder.DropTable(
                name: "asset_groups");

            migrationBuilder.DropIndex(
                name: "ix_assets_asset_group_id",
                table: "assets");

            migrationBuilder.DropIndex(
                name: "ix_assets_asset_sub_group_id",
                table: "assets");

            migrationBuilder.DropColumn(
                name: "asset_group_id",
                table: "assets");

            migrationBuilder.DropColumn(
                name: "asset_sub_group_id",
                table: "assets");

            migrationBuilder.DropColumn(
                name: "is_bulk",
                table: "assets");

            migrationBuilder.DropColumn(
                name: "serial_number",
                table: "assets");

            migrationBuilder.AddColumn<int>(
                name: "available_quantity",
                table: "assets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "uses_models",
                table: "assets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "fk_asset_models_assets_asset_id",
                table: "asset_models",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
