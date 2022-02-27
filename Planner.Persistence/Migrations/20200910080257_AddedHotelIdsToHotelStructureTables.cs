using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedHotelIdsToHotelStructureTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "user_sub_groups",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 464, DateTimeKind.Unspecified).AddTicks(1937), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 7, DateTimeKind.Unspecified).AddTicks(9353), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_sub_groups",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 464, DateTimeKind.Unspecified).AddTicks(1189), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 7, DateTimeKind.Unspecified).AddTicks(9014), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "user_groups",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 463, DateTimeKind.Unspecified).AddTicks(4326), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 7, DateTimeKind.Unspecified).AddTicks(2490), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_groups",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 463, DateTimeKind.Unspecified).AddTicks(4009), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 7, DateTimeKind.Unspecified).AddTicks(2181), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "rooms",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 462, DateTimeKind.Unspecified).AddTicks(7128), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 6, DateTimeKind.Unspecified).AddTicks(5472), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "rooms",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 462, DateTimeKind.Unspecified).AddTicks(6804), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 6, DateTimeKind.Unspecified).AddTicks(5150), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "floors",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 458, DateTimeKind.Unspecified).AddTicks(9563), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 2, DateTimeKind.Unspecified).AddTicks(9117), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "floors",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 458, DateTimeKind.Unspecified).AddTicks(9257), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 2, DateTimeKind.Unspecified).AddTicks(8822), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "hotel_id",
                table: "floors",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "buildings",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 458, DateTimeKind.Unspecified).AddTicks(2347), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 2, DateTimeKind.Unspecified).AddTicks(2425), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "buildings",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 458, DateTimeKind.Unspecified).AddTicks(1980), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 2, DateTimeKind.Unspecified).AddTicks(2063), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "hotel_id",
                table: "buildings",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "areas",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 456, DateTimeKind.Unspecified).AddTicks(121), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 0, DateTimeKind.Unspecified).AddTicks(845), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "areas",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 455, DateTimeKind.Unspecified).AddTicks(1031), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 3, 999, DateTimeKind.Unspecified).AddTicks(1971), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "hotel_id",
                table: "areas",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_floors_hotel_id",
                table: "floors",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_buildings_hotel_id",
                table: "buildings",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_areas_hotel_id",
                table: "areas",
                column: "hotel_id");

            migrationBuilder.AddForeignKey(
                name: "fk_areas_hotels_hotel_id",
                table: "areas",
                column: "hotel_id",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_buildings_hotels_hotel_id",
                table: "buildings",
                column: "hotel_id",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_floors_hotels_hotel_id",
                table: "floors",
                column: "hotel_id",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_areas_hotels_hotel_id",
                table: "areas");

            migrationBuilder.DropForeignKey(
                name: "fk_buildings_hotels_hotel_id",
                table: "buildings");

            migrationBuilder.DropForeignKey(
                name: "fk_floors_hotels_hotel_id",
                table: "floors");

            migrationBuilder.DropIndex(
                name: "ix_floors_hotel_id",
                table: "floors");

            migrationBuilder.DropIndex(
                name: "ix_buildings_hotel_id",
                table: "buildings");

            migrationBuilder.DropIndex(
                name: "ix_areas_hotel_id",
                table: "areas");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "floors");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "buildings");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "areas");

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "user_sub_groups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 7, DateTimeKind.Unspecified).AddTicks(9353), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 464, DateTimeKind.Unspecified).AddTicks(1937), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_sub_groups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 7, DateTimeKind.Unspecified).AddTicks(9014), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 464, DateTimeKind.Unspecified).AddTicks(1189), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "user_groups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 7, DateTimeKind.Unspecified).AddTicks(2490), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 463, DateTimeKind.Unspecified).AddTicks(4326), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_groups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 7, DateTimeKind.Unspecified).AddTicks(2181), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 463, DateTimeKind.Unspecified).AddTicks(4009), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "rooms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 6, DateTimeKind.Unspecified).AddTicks(5472), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 462, DateTimeKind.Unspecified).AddTicks(7128), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "rooms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 6, DateTimeKind.Unspecified).AddTicks(5150), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 462, DateTimeKind.Unspecified).AddTicks(6804), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "floors",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 2, DateTimeKind.Unspecified).AddTicks(9117), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 458, DateTimeKind.Unspecified).AddTicks(9563), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "floors",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 2, DateTimeKind.Unspecified).AddTicks(8822), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 458, DateTimeKind.Unspecified).AddTicks(9257), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "buildings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 2, DateTimeKind.Unspecified).AddTicks(2425), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 458, DateTimeKind.Unspecified).AddTicks(2347), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "buildings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 2, DateTimeKind.Unspecified).AddTicks(2063), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 458, DateTimeKind.Unspecified).AddTicks(1980), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 0, DateTimeKind.Unspecified).AddTicks(845), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 456, DateTimeKind.Unspecified).AddTicks(121), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 3, 999, DateTimeKind.Unspecified).AddTicks(1971), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 8, 2, 57, 455, DateTimeKind.Unspecified).AddTicks(1031), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
