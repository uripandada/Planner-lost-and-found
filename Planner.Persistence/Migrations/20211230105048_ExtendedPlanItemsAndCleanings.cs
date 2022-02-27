using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedPlanItemsAndCleanings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cleaning_plan_group_floor_affinities");

            migrationBuilder.AddColumn<bool>(
                name: "is_priority",
                table: "cleanings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql($@"ALTER TABLE cleanings ALTER COLUMN is_priority DROP DEFAULT;");

            migrationBuilder.AddColumn<bool>(
                name: "is_priority",
                table: "cleaning_plan_items",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql($@"ALTER TABLE cleaning_plan_items ALTER COLUMN is_priority DROP DEFAULT;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_priority",
                table: "cleanings");

            migrationBuilder.DropColumn(
                name: "is_priority",
                table: "cleaning_plan_items");

            migrationBuilder.CreateTable(
                name: "cleaning_plan_group_floor_affinities",
                columns: table => new
                {
                    floor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cleaning_plan_group_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cleaning_plan_group_floor_affinities", x => new { x.floor_id, x.cleaning_plan_group_id });
                    table.ForeignKey(
                        name: "fk_cleaning_plan_group_floor_affinities_cleaning_plan_groups_c~",
                        column: x => x.cleaning_plan_group_id,
                        principalTable: "cleaning_plan_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_cleaning_plan_group_floor_affinities_floors_floor_id",
                        column: x => x.floor_id,
                        principalTable: "floors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_group_floor_affinities_cleaning_plan_group_id",
                table: "cleaning_plan_group_floor_affinities",
                column: "cleaning_plan_group_id");
        }
    }
}
