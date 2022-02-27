using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedCleaningPlanGroupTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cleaning_plan_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    cleaning_plan_id = table.Column<Guid>(nullable: false),
                    cleaner_id = table.Column<Guid>(nullable: false),
                    max_credits = table.Column<int>(nullable: true),
                    max_departures = table.Column<int>(nullable: true),
                    max_twins = table.Column<int>(nullable: true),
                    weekly_hours = table.Column<int>(nullable: true),
                    must_fill_all_credits = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cleaning_plan_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_cleaning_plan_groups_asp_net_users_cleaner_id",
                        column: x => x.cleaner_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cleaning_plan_groups_cleaning_plans_cleaning_plan_id",
                        column: x => x.cleaning_plan_id,
                        principalTable: "cleaning_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cleaning_plan_group_availability_intervals",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    cleaning_plan_group_id = table.Column<Guid>(nullable: false),
                    from = table.Column<TimeSpan>(type: "time", nullable: false),
                    to = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cleaning_plan_group_availability_intervals", x => x.id);
                    table.ForeignKey(
                        name: "fk_cleaning_plan_group_availability_intervals_cleaning_plan_gr~",
                        column: x => x.cleaning_plan_group_id,
                        principalTable: "cleaning_plan_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cleaning_plan_group_floor_affinities",
                columns: table => new
                {
                    cleaning_plan_group_id = table.Column<Guid>(nullable: false),
                    floor_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cleaning_plan_group_floor_affinities", x => new { x.floor_id, x.cleaning_plan_group_id });
                    table.ForeignKey(
                        name: "fk_cleaning_plan_group_floor_affinities_cleaning_plan_groups_c~",
                        column: x => x.cleaning_plan_group_id,
                        principalTable: "cleaning_plan_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cleaning_plan_group_floor_affinities_floors_floor_id",
                        column: x => x.floor_id,
                        principalTable: "floors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_group_availability_intervals_cleaning_plan_gr~",
                table: "cleaning_plan_group_availability_intervals",
                column: "cleaning_plan_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_group_floor_affinities_cleaning_plan_group_id",
                table: "cleaning_plan_group_floor_affinities",
                column: "cleaning_plan_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_groups_cleaner_id",
                table: "cleaning_plan_groups",
                column: "cleaner_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_groups_cleaning_plan_id",
                table: "cleaning_plan_groups",
                column: "cleaning_plan_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cleaning_plan_group_availability_intervals");

            migrationBuilder.DropTable(
                name: "cleaning_plan_group_floor_affinities");

            migrationBuilder.DropTable(
                name: "cleaning_plan_groups");
        }
    }
}
