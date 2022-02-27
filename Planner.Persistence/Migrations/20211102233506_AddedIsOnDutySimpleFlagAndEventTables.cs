using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedIsOnDutySimpleFlagAndEventTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<bool>(
            //    name: "is_on_duty",
            //    table: "asp_net_users",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);


            migrationBuilder.AddColumn<bool>(name: "is_on_duty", table: "asp_net_users", nullable: true);
            migrationBuilder.Sql("UPDATE public.asp_net_users SET is_on_duty = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_on_duty", table: "asp_net_users", nullable: false);

            migrationBuilder.CreateTable(
                name: "cleaning_history_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cleaning_plan_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    room_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    type = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    old_data = table.Column<string>(type: "text", nullable: true),
                    new_data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cleaning_history_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_cleaning_history_events_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cleaning_history_events_cleaning_plan_items_cleaning_plan_ite~",
                        column: x => x.cleaning_plan_item_id,
                        principalTable: "cleaning_plan_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cleaning_history_events_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "room_history_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    room_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    type = table.Column<string>(type: "text", nullable: false),
                    at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    old_data = table.Column<string>(type: "text", nullable: true),
                    new_data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_history_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_history_events_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_room_history_events_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_history_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    old_data = table.Column<string>(type: "text", nullable: true),
                    new_data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_history_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_history_events_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_history_events_cleaning_plan_item_id",
                table: "cleaning_history_events",
                column: "cleaning_plan_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_history_events_room_id",
                table: "cleaning_history_events",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_history_events_user_id",
                table: "cleaning_history_events",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_history_events_room_id",
                table: "room_history_events",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_history_events_user_id",
                table: "room_history_events",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_history_events_user_id",
                table: "user_history_events",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cleaning_history_events");

            migrationBuilder.DropTable(
                name: "room_history_events");

            migrationBuilder.DropTable(
                name: "user_history_events");

            migrationBuilder.DropColumn(
                name: "is_on_duty",
                table: "asp_net_users");
        }
    }
}
