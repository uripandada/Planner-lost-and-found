using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ModifiedTasksSchemaWithMoreSpecificDenormalization1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "where_reference_id",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "where_reference_name",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "where_type_description",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "who_reference_id",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "who_reference_name",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "who_type_description",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "who_type_key",
                table: "system_tasks");

            migrationBuilder.AddColumn<Guid>(
                name: "building_id",
                table: "system_tasks",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "floor_id",
                table: "system_tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hotel_id",
                table: "system_tasks",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "reservation_id",
                table: "system_tasks",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "room_id",
                table: "system_tasks",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "system_tasks",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_system_tasks_building_id",
                table: "system_tasks",
                column: "building_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_tasks_floor_id",
                table: "system_tasks",
                column: "floor_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_tasks_hotel_id",
                table: "system_tasks",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_tasks_room_id",
                table: "system_tasks",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_tasks_user_id",
                table: "system_tasks",
                column: "user_id");

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

            migrationBuilder.AddForeignKey(
                name: "fk_system_tasks_asp_net_users_user_id",
                table: "system_tasks",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropForeignKey(
                name: "fk_system_tasks_asp_net_users_user_id",
                table: "system_tasks");

            migrationBuilder.DropIndex(
                name: "ix_system_tasks_building_id",
                table: "system_tasks");

            migrationBuilder.DropIndex(
                name: "ix_system_tasks_floor_id",
                table: "system_tasks");

            migrationBuilder.DropIndex(
                name: "ix_system_tasks_hotel_id",
                table: "system_tasks");

            migrationBuilder.DropIndex(
                name: "ix_system_tasks_room_id",
                table: "system_tasks");

            migrationBuilder.DropIndex(
                name: "ix_system_tasks_user_id",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "building_id",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "floor_id",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "reservation_id",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "room_id",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "system_tasks");

            migrationBuilder.AddColumn<string>(
                name: "where_reference_id",
                table: "system_tasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "where_reference_name",
                table: "system_tasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "where_type_description",
                table: "system_tasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "who_reference_id",
                table: "system_tasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "who_reference_name",
                table: "system_tasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "who_type_description",
                table: "system_tasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "who_type_key",
                table: "system_tasks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
