using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Planner.Persistence.Migrations
{
    public partial class ChangeCategoryTableNameToLostAndFoundCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_lost_and_founds_categorys_lost_and_found_category_id",
                table: "lost_and_founds");

            migrationBuilder.DropTable(
                name: "categorys");

            migrationBuilder.CreateTable(
                name: "lost_and_found_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    expiration_days = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lost_and_found_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_lost_and_found_categories_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_lost_and_found_categories_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_lost_and_found_categories_created_by_id",
                table: "lost_and_found_categories",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_lost_and_found_categories_modified_by_id",
                table: "lost_and_found_categories",
                column: "modified_by_id");

            migrationBuilder.AddForeignKey(
                name: "fk_lost_and_founds_lost_and_found_categories_lost_and_found_categ~",
                table: "lost_and_founds",
                column: "lost_and_found_category_id",
                principalTable: "lost_and_found_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_lost_and_founds_lost_and_found_categories_lost_and_found_categ~",
                table: "lost_and_founds");

            migrationBuilder.DropTable(
                name: "lost_and_found_categories");

            migrationBuilder.CreateTable(
                name: "categorys",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    expiration_days = table.Column<int>(type: "integer", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categorys", x => x.id);
                    table.ForeignKey(
                        name: "fk_categorys_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_categorys_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_categorys_created_by_id",
                table: "categorys",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_categorys_modified_by_id",
                table: "categorys",
                column: "modified_by_id");

            migrationBuilder.AddForeignKey(
                name: "fk_lost_and_founds_categorys_lost_and_found_category_id",
                table: "lost_and_founds",
                column: "lost_and_found_category_id",
                principalTable: "categorys",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
