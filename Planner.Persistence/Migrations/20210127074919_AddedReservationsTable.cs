using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Planner.Domain.Entities;

namespace Planner.Persistence.Migrations
{
    public partial class AddedReservationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reservations",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    room_name = table.Column<string>(nullable: true),
                    pmsroom_name = table.Column<string>(nullable: false),
                    guest_name = table.Column<string>(nullable: false),
                    check_in = table.Column<DateTime>(nullable: true),
                    actual_check_in = table.Column<DateTime>(nullable: true),
                    check_out = table.Column<DateTime>(nullable: true),
                    actual_check_out = table.Column<DateTime>(nullable: true),
                    rcc_reservation_status_key = table.Column<string>(nullable: false),
                    number_of_adults = table.Column<int>(nullable: false),
                    number_of_children = table.Column<int>(nullable: false),
                    number_of_infants = table.Column<int>(nullable: false),
                    pms_note = table.Column<string>(nullable: true),
                    vip = table.Column<string>(nullable: true),
                    other_properties = table.Column<ReservationOtherProperties>(type: "jsonb", nullable: true),
                    hotel_id = table.Column<string>(nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    is_synchronized_from_rcc = table.Column<bool>(nullable: false),
                    last_time_modified_by_synchronization = table.Column<DateTime>(nullable: true),
                    synchronized_at = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservations", x => x.id);
                    table.ForeignKey(
                        name: "fk_reservations_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_reservations_hotel_id",
                table: "reservations",
                column: "hotel_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reservations");
        }
    }
}
