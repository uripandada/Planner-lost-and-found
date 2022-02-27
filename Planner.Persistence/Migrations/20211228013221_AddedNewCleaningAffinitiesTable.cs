using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedNewCleaningAffinitiesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cleaning_plan_group_affinities",
                columns: table => new
                {
                    cleaning_plan_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reference_id = table.Column<string>(type: "text", nullable: false),
                    affinity_type = table.Column<string>(type: "text", nullable: false, defaultValue: "UNKNOWN")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cleaning_plan_group_affinities", x => new { x.reference_id, x.cleaning_plan_group_id });
                    table.ForeignKey(
                        name: "fk_cleaning_plan_group_affinities_cleaning_plan_groups_cleanin~",
                        column: x => x.cleaning_plan_group_id,
                        principalTable: "cleaning_plan_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_group_affinities_cleaning_plan_group_id",
                table: "cleaning_plan_group_affinities",
                column: "cleaning_plan_group_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cleaning_plan_group_affinities");
        }
    }
}
