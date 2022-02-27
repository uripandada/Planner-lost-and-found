using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ChangedTaskSchemaToSupportFromToWheres1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_system_tasks_asp_net_users_user_id",
                table: "system_tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_system_tasks_buildings_building_id",
                table: "system_tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_system_tasks_floors_floor_id",
                table: "system_tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_system_tasks_hotels_hotel_id",
                table: "system_tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_system_tasks_rooms_room_id",
                table: "system_tasks");

            migrationBuilder.DropIndex(
                name: "ix_system_tasks_hotel_id",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "system_tasks");

            migrationBuilder.RenameColumn(
                name: "room_id",
                table: "system_tasks",
                newName: "to_warehouse_id");

            migrationBuilder.RenameColumn(
                name: "reservation_id",
                table: "system_tasks",
                newName: "to_reservation_id");

            migrationBuilder.RenameColumn(
                name: "floor_id",
                table: "system_tasks",
                newName: "to_room_id");

            migrationBuilder.RenameColumn(
                name: "building_id",
                table: "system_tasks",
                newName: "from_warehouse_id");

            migrationBuilder.RenameIndex(
                name: "ix_system_tasks_room_id",
                table: "system_tasks",
                newName: "ix_system_tasks_to_warehouse_id");

            migrationBuilder.RenameIndex(
                name: "ix_system_tasks_floor_id",
                table: "system_tasks",
                newName: "ix_system_tasks_to_room_id");

            migrationBuilder.RenameIndex(
                name: "ix_system_tasks_building_id",
                table: "system_tasks",
                newName: "ix_system_tasks_from_warehouse_id");

            migrationBuilder.AddColumn<string>(
                name: "from_hotel_id",
                table: "system_tasks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "from_name",
                table: "system_tasks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "from_reservation_id",
                table: "system_tasks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "from_room_id",
                table: "system_tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "to_hotel_id",
                table: "system_tasks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "to_name",
                table: "system_tasks",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_system_tasks_from_hotel_id",
                table: "system_tasks",
                column: "from_hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_tasks_from_reservation_id",
                table: "system_tasks",
                column: "from_reservation_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_tasks_from_room_id",
                table: "system_tasks",
                column: "from_room_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_tasks_to_hotel_id",
                table: "system_tasks",
                column: "to_hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_tasks_to_reservation_id",
                table: "system_tasks",
                column: "to_reservation_id");

            migrationBuilder.AddForeignKey(
                name: "fk_system_tasks_asp_net_users_user_id",
                table: "system_tasks",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_system_tasks_hotels_from_hotel_id",
                table: "system_tasks",
                column: "from_hotel_id",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_system_tasks_hotels_to_hotel_id",
                table: "system_tasks",
                column: "to_hotel_id",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_system_tasks_reservations_from_reservation_id",
                table: "system_tasks",
                column: "from_reservation_id",
                principalTable: "reservations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_system_tasks_reservations_to_reservation_id",
                table: "system_tasks",
                column: "to_reservation_id",
                principalTable: "reservations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_system_tasks_rooms_from_room_id",
                table: "system_tasks",
                column: "from_room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_system_tasks_rooms_to_room_id",
                table: "system_tasks",
                column: "to_room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_system_tasks_warehouses_from_warehouse_id",
                table: "system_tasks",
                column: "from_warehouse_id",
                principalTable: "warehouses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_system_tasks_warehouses_to_warehouse_id",
                table: "system_tasks",
                column: "to_warehouse_id",
                principalTable: "warehouses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_system_tasks_asp_net_users_user_id",
                table: "system_tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_system_tasks_hotels_from_hotel_id",
                table: "system_tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_system_tasks_hotels_to_hotel_id",
                table: "system_tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_system_tasks_reservations_from_reservation_id",
                table: "system_tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_system_tasks_reservations_to_reservation_id",
                table: "system_tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_system_tasks_rooms_from_room_id",
                table: "system_tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_system_tasks_rooms_to_room_id",
                table: "system_tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_system_tasks_warehouses_from_warehouse_id",
                table: "system_tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_system_tasks_warehouses_to_warehouse_id",
                table: "system_tasks");

            migrationBuilder.DropIndex(
                name: "ix_system_tasks_from_hotel_id",
                table: "system_tasks");

            migrationBuilder.DropIndex(
                name: "ix_system_tasks_from_reservation_id",
                table: "system_tasks");

            migrationBuilder.DropIndex(
                name: "ix_system_tasks_from_room_id",
                table: "system_tasks");

            migrationBuilder.DropIndex(
                name: "ix_system_tasks_to_hotel_id",
                table: "system_tasks");

            migrationBuilder.DropIndex(
                name: "ix_system_tasks_to_reservation_id",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "from_hotel_id",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "from_name",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "from_reservation_id",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "from_room_id",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "to_hotel_id",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "to_name",
                table: "system_tasks");

            migrationBuilder.RenameColumn(
                name: "to_warehouse_id",
                table: "system_tasks",
                newName: "room_id");

            migrationBuilder.RenameColumn(
                name: "to_room_id",
                table: "system_tasks",
                newName: "floor_id");

            migrationBuilder.RenameColumn(
                name: "to_reservation_id",
                table: "system_tasks",
                newName: "reservation_id");

            migrationBuilder.RenameColumn(
                name: "from_warehouse_id",
                table: "system_tasks",
                newName: "building_id");

            migrationBuilder.RenameIndex(
                name: "ix_system_tasks_to_warehouse_id",
                table: "system_tasks",
                newName: "ix_system_tasks_room_id");

            migrationBuilder.RenameIndex(
                name: "ix_system_tasks_to_room_id",
                table: "system_tasks",
                newName: "ix_system_tasks_floor_id");

            migrationBuilder.RenameIndex(
                name: "ix_system_tasks_from_warehouse_id",
                table: "system_tasks",
                newName: "ix_system_tasks_building_id");

            migrationBuilder.AddColumn<string>(
                name: "hotel_id",
                table: "system_tasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_system_tasks_hotel_id",
                table: "system_tasks",
                column: "hotel_id");

            migrationBuilder.AddForeignKey(
                name: "fk_system_tasks_asp_net_users_user_id",
                table: "system_tasks",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_system_tasks_buildings_building_id",
                table: "system_tasks",
                column: "building_id",
                principalTable: "buildings",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_system_tasks_floors_floor_id",
                table: "system_tasks",
                column: "floor_id",
                principalTable: "floors",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_system_tasks_hotels_hotel_id",
                table: "system_tasks",
                column: "hotel_id",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_system_tasks_rooms_room_id",
                table: "system_tasks",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
