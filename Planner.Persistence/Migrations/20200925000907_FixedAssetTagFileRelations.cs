using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class FixedAssetTagFileRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_files_assets_asset_id",
                table: "files");

            migrationBuilder.DropIndex(
                name: "ix_files_asset_id",
                table: "files");

            migrationBuilder.DropColumn(
                name: "asset_id",
                table: "files");

            migrationBuilder.CreateTable(
                name: "asset_files",
                columns: table => new
                {
                    asset_id = table.Column<Guid>(nullable: false),
                    file_id = table.Column<Guid>(nullable: false),
                    is_primary_image = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asset_files", x => new { x.asset_id, x.file_id });
                    table.ForeignKey(
                        name: "fk_asset_files_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_asset_files_files_file_id",
                        column: x => x.file_id,
                        principalTable: "files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_asset_files_file_id",
                table: "asset_files",
                column: "file_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asset_files");

            migrationBuilder.AddColumn<Guid>(
                name: "asset_id",
                table: "files",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_files_asset_id",
                table: "files",
                column: "asset_id");

            migrationBuilder.AddForeignKey(
                name: "fk_files_assets_asset_id",
                table: "files",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
