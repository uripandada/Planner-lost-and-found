using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedCleaningPlanAndItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cleaning_plans",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    hotel_id = table.Column<string>(nullable: false),
                    date = table.Column<DateTime>(nullable: false),
                    is_sent = table.Column<bool>(nullable: false),
                    sent_at = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cleaning_plans", x => x.id);
                    table.ForeignKey(
                        name: "fk_cleaning_plans_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cleaning_plans_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cleaning_plans_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cleaning_plan_items",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    cleaning_plan_id = table.Column<Guid>(nullable: false),
                    cleaner_id = table.Column<Guid>(nullable: false),
                    room_id = table.Column<Guid>(nullable: false),
                    starts_at = table.Column<DateTimeOffset>(nullable: false),
                    ends_at = table.Column<DateTimeOffset>(nullable: false),
                    duration_sec = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cleaning_plan_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_cleaning_plan_items_asp_net_users_cleaner_id",
                        column: x => x.cleaner_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cleaning_plan_items_cleaning_plans_cleaning_plan_id",
                        column: x => x.cleaning_plan_id,
                        principalTable: "cleaning_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cleaning_plan_items_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_items_cleaner_id",
                table: "cleaning_plan_items",
                column: "cleaner_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_items_cleaning_plan_id",
                table: "cleaning_plan_items",
                column: "cleaning_plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_items_room_id",
                table: "cleaning_plan_items",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plans_created_by_id",
                table: "cleaning_plans",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plans_hotel_id",
                table: "cleaning_plans",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plans_modified_by_id",
                table: "cleaning_plans",
                column: "modified_by_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cleaning_plan_items");

            migrationBuilder.DropTable(
                name: "cleaning_plans");
        }
    }
}
