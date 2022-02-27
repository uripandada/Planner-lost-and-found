using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedHotelStructureTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "user_sub_groups",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 7, DateTimeKind.Unspecified).AddTicks(9353), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_sub_groups",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 7, DateTimeKind.Unspecified).AddTicks(9014), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "user_groups",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 7, DateTimeKind.Unspecified).AddTicks(2490), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_groups",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 7, DateTimeKind.Unspecified).AddTicks(2181), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateTable(
                name: "areas",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false, defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 3, 999, DateTimeKind.Unspecified).AddTicks(1971), new TimeSpan(0, 0, 0, 0, 0))),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTime>(nullable: false, defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 0, DateTimeKind.Unspecified).AddTicks(845), new TimeSpan(0, 0, 0, 0, 0))),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_areas", x => x.id);
                    table.ForeignKey(
                        name: "fk_areas_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_areas_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false, defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 2, DateTimeKind.Unspecified).AddTicks(2063), new TimeSpan(0, 0, 0, 0, 0))),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTime>(nullable: false, defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 2, DateTimeKind.Unspecified).AddTicks(2425), new TimeSpan(0, 0, 0, 0, 0))),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    address = table.Column<string>(nullable: true),
                    latitude = table.Column<long>(nullable: true),
                    longitude = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_buildings", x => x.id);
                    table.ForeignKey(
                        name: "fk_buildings_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_buildings_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "floors",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false, defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 2, DateTimeKind.Unspecified).AddTicks(8822), new TimeSpan(0, 0, 0, 0, 0))),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTime>(nullable: false, defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 2, DateTimeKind.Unspecified).AddTicks(9117), new TimeSpan(0, 0, 0, 0, 0))),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    number = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_floors", x => x.id);
                    table.ForeignKey(
                        name: "fk_floors_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_floors_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "hotels",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    modified_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_hotels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false, defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 6, DateTimeKind.Unspecified).AddTicks(5150), new TimeSpan(0, 0, 0, 0, 0))),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTime>(nullable: false, defaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 6, DateTimeKind.Unspecified).AddTicks(5472), new TimeSpan(0, 0, 0, 0, 0))),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    hotel_id = table.Column<string>(nullable: true),
                    external_id = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    type_key = table.Column<string>(nullable: true),
                    area_id = table.Column<Guid>(nullable: true),
                    building_id = table.Column<Guid>(nullable: true),
                    floor_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rooms", x => x.id);
                    table.ForeignKey(
                        name: "fk_rooms_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_rooms_buildings_building_id",
                        column: x => x.building_id,
                        principalTable: "buildings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_rooms_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_rooms_floors_floor_id",
                        column: x => x.floor_id,
                        principalTable: "floors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_rooms_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_rooms_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_sub_groups_hotel_id",
                table: "user_sub_groups",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_groups_hotel_id",
                table: "user_groups",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_areas_created_by_id",
                table: "areas",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_areas_modified_by_id",
                table: "areas",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_buildings_created_by_id",
                table: "buildings",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_buildings_modified_by_id",
                table: "buildings",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_floors_created_by_id",
                table: "floors",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_floors_modified_by_id",
                table: "floors",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_area_id",
                table: "rooms",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_building_id",
                table: "rooms",
                column: "building_id");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_created_by_id",
                table: "rooms",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_floor_id",
                table: "rooms",
                column: "floor_id");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_hotel_id",
                table: "rooms",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_modified_by_id",
                table: "rooms",
                column: "modified_by_id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_groups_hotels_hotel_id",
                table: "user_groups",
                column: "hotel_id",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_sub_groups_hotels_hotel_id",
                table: "user_sub_groups",
                column: "hotel_id",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_groups_hotels_hotel_id",
                table: "user_groups");

            migrationBuilder.DropForeignKey(
                name: "fk_user_sub_groups_hotels_hotel_id",
                table: "user_sub_groups");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "areas");

            migrationBuilder.DropTable(
                name: "buildings");

            migrationBuilder.DropTable(
                name: "floors");

            migrationBuilder.DropTable(
                name: "hotels");

            migrationBuilder.DropIndex(
                name: "ix_user_sub_groups_hotel_id",
                table: "user_sub_groups");

            migrationBuilder.DropIndex(
                name: "ix_user_groups_hotel_id",
                table: "user_groups");

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "user_sub_groups",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 7, DateTimeKind.Unspecified).AddTicks(9353), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_sub_groups",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 7, DateTimeKind.Unspecified).AddTicks(9014), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "user_groups",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 7, DateTimeKind.Unspecified).AddTicks(2490), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_groups",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 10, 4, 26, 4, 7, DateTimeKind.Unspecified).AddTicks(2181), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
