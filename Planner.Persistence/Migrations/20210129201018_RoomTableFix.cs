using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class RoomTableFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_rooms_hotels_hotel_id1",
                table: "rooms");

            migrationBuilder.DropIndex(
                name: "ix_rooms_hotel_id1",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "hotel_id1",
                table: "rooms");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "hotel_id1",
                table: "rooms",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_rooms_hotel_id1",
                table: "rooms",
                column: "hotel_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_rooms_hotels_hotel_id1",
                table: "rooms",
                column: "hotel_id1",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
