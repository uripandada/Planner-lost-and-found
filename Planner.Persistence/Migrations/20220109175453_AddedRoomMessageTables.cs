using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedRoomMessageTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "room_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false, defaultValue: "SIMPLE"),
                    for_type = table.Column<string>(type: "text", nullable: false, defaultValue: "PLACES"),
                    date_type = table.Column<string>(type: "text", nullable: false, defaultValue: "SPECIFIC_DATES"),
                    interval_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    interval_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    interval_number_of_days = table.Column<int>(type: "integer", nullable: true),
                    reservation_on_arrival_date = table.Column<bool>(type: "boolean", nullable: true),
                    reservation_on_departure_date = table.Column<bool>(type: "boolean", nullable: true),
                    reservation_on_stay_dates = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    hotel_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_messages_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_room_messages_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_room_messages_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "room_message_dates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTime>(type: "date", nullable: false),
                    room_message_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_message_dates", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_message_dates_room_messages_room_message_id",
                        column: x => x.room_message_id,
                        principalTable: "room_messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "room_message_filters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    room_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reference_id = table.Column<string>(type: "text", nullable: false),
                    reference_type = table.Column<string>(type: "text", nullable: false, defaultValue: "OTHERS"),
                    reference_name = table.Column<string>(type: "text", nullable: false),
                    reference_description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_message_filters", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_message_filters_room_messages_room_message_id",
                        column: x => x.room_message_id,
                        principalTable: "room_messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "room_message_reservations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTime>(type: "date", nullable: false),
                    room_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reservation_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_message_reservations", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_message_reservations_reservations_reservation_id",
                        column: x => x.reservation_id,
                        principalTable: "reservations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_room_message_reservations_room_messages_room_message_id",
                        column: x => x.room_message_id,
                        principalTable: "room_messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "room_message_rooms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTime>(type: "date", nullable: false),
                    room_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    room_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_message_rooms", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_message_rooms_room_messages_room_message_id",
                        column: x => x.room_message_id,
                        principalTable: "room_messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_room_message_rooms_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_room_message_dates_room_message_id",
                table: "room_message_dates",
                column: "room_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_message_filters_room_message_id",
                table: "room_message_filters",
                column: "room_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_message_reservations_reservation_id",
                table: "room_message_reservations",
                column: "reservation_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_message_reservations_room_message_id",
                table: "room_message_reservations",
                column: "room_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_message_rooms_room_id",
                table: "room_message_rooms",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_message_rooms_room_message_id",
                table: "room_message_rooms",
                column: "room_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_messages_created_by_id",
                table: "room_messages",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_messages_hotel_id",
                table: "room_messages",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_messages_modified_by_id",
                table: "room_messages",
                column: "modified_by_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "room_message_dates");

            migrationBuilder.DropTable(
                name: "room_message_filters");

            migrationBuilder.DropTable(
                name: "room_message_reservations");

            migrationBuilder.DropTable(
                name: "room_message_rooms");

            migrationBuilder.DropTable(
                name: "room_messages");
        }
    }
}
