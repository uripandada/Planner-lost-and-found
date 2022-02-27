using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedRoomTableWithStatusFlags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(name: "is_clean", table: "rooms", nullable: true);
            migrationBuilder.Sql("UPDATE public.rooms SET is_clean = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_clean", table:"rooms", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_clean",
            //    table: "rooms",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_do_not_disturb", table: "rooms", nullable: true);
            migrationBuilder.Sql("UPDATE public.rooms SET is_do_not_disturb = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_do_not_disturb", table: "rooms", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_do_not_disturb",
            //    table: "rooms",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_occupied", table: "rooms", nullable: true);
            migrationBuilder.Sql("UPDATE public.rooms SET is_occupied = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_occupied", table: "rooms", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_occupied",
            //    table: "rooms",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_out_of_order", table: "rooms", nullable: true);
            migrationBuilder.Sql("UPDATE public.rooms SET is_out_of_order = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_out_of_order", table: "rooms", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_out_of_order",
            //    table: "rooms",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "occupied_by_reservation_id",
                table: "rooms",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_rooms_occupied_by_reservation_id",
                table: "rooms",
                column: "occupied_by_reservation_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_rooms_reservations_occupied_by_reservation_id",
                table: "rooms",
                column: "occupied_by_reservation_id",
                principalTable: "reservations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_rooms_reservations_occupied_by_reservation_id",
                table: "rooms");

            migrationBuilder.DropIndex(
                name: "ix_rooms_occupied_by_reservation_id",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "is_clean",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "is_do_not_disturb",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "is_occupied",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "is_out_of_order",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "occupied_by_reservation_id",
                table: "rooms");
        }
    }
}
