using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class SimplifiedUserOnDutyStatusExtendedEventHistoryTables1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cleaning_plan_item_events");

            migrationBuilder.DropTable(
                name: "user_status_histories");

            migrationBuilder.DropTable(
                name: "user_statuses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cleaning_plan_item_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cleaning_plan_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    event_type = table.Column<string>(type: "text", nullable: false),
                    is_system_event = table.Column<bool>(type: "boolean", nullable: false),
                    message = table.Column<string>(type: "text", nullable: true),
                    new_state = table.Column<string>(type: "text", nullable: true),
                    old_state = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cleaning_plan_item_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_cleaning_plan_item_events_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cleaning_plan_item_events_cleaning_plan_items_cleaning_plan~",
                        column: x => x.cleaning_plan_item_id,
                        principalTable: "cleaning_plan_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_status_histories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    room_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    status_description = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_status_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_status_histories_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_status_histories_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_statuses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    room_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    status_description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_statuses", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_statuses_asp_net_users_id",
                        column: x => x.id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_statuses_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_item_events_cleaning_plan_item_id",
                table: "cleaning_plan_item_events",
                column: "cleaning_plan_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plan_item_events_created_by_id",
                table: "cleaning_plan_item_events",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_status_histories_room_id",
                table: "user_status_histories",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_status_histories_user_id",
                table: "user_status_histories",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_statuses_room_id",
                table: "user_statuses",
                column: "room_id");
        }
    }
}
