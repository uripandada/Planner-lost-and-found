using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedBuildingTypeKey2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "user_sub_groups",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 64, DateTimeKind.Unspecified).AddTicks(1774), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 864, DateTimeKind.Unspecified).AddTicks(5906), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_sub_groups",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 64, DateTimeKind.Unspecified).AddTicks(1162), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 864, DateTimeKind.Unspecified).AddTicks(5470), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "user_groups",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 63, DateTimeKind.Unspecified).AddTicks(4478), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 863, DateTimeKind.Unspecified).AddTicks(7802), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_groups",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 63, DateTimeKind.Unspecified).AddTicks(4173), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 863, DateTimeKind.Unspecified).AddTicks(7444), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "rooms",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 62, DateTimeKind.Unspecified).AddTicks(7514), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 863, DateTimeKind.Unspecified).AddTicks(959), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "rooms",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 62, DateTimeKind.Unspecified).AddTicks(7189), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 863, DateTimeKind.Unspecified).AddTicks(538), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "floors",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 59, DateTimeKind.Unspecified).AddTicks(1206), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 859, DateTimeKind.Unspecified).AddTicks(3350), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "floors",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 59, DateTimeKind.Unspecified).AddTicks(849), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 859, DateTimeKind.Unspecified).AddTicks(3031), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "buildings",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 58, DateTimeKind.Unspecified).AddTicks(3795), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 858, DateTimeKind.Unspecified).AddTicks(6127), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "buildings",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 58, DateTimeKind.Unspecified).AddTicks(3418), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 858, DateTimeKind.Unspecified).AddTicks(5753), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "type_key",
                table: "buildings",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "areas",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 56, DateTimeKind.Unspecified).AddTicks(1706), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 856, DateTimeKind.Unspecified).AddTicks(4316), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "areas",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 55, DateTimeKind.Unspecified).AddTicks(3052), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 855, DateTimeKind.Unspecified).AddTicks(5013), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type_key",
                table: "buildings");

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "user_sub_groups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 864, DateTimeKind.Unspecified).AddTicks(5906), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 64, DateTimeKind.Unspecified).AddTicks(1774), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_sub_groups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 864, DateTimeKind.Unspecified).AddTicks(5470), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 64, DateTimeKind.Unspecified).AddTicks(1162), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "user_groups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 863, DateTimeKind.Unspecified).AddTicks(7802), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 63, DateTimeKind.Unspecified).AddTicks(4478), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_groups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 863, DateTimeKind.Unspecified).AddTicks(7444), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 63, DateTimeKind.Unspecified).AddTicks(4173), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "rooms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 863, DateTimeKind.Unspecified).AddTicks(959), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 62, DateTimeKind.Unspecified).AddTicks(7514), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "rooms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 863, DateTimeKind.Unspecified).AddTicks(538), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 62, DateTimeKind.Unspecified).AddTicks(7189), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "floors",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 859, DateTimeKind.Unspecified).AddTicks(3350), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 59, DateTimeKind.Unspecified).AddTicks(1206), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "floors",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 859, DateTimeKind.Unspecified).AddTicks(3031), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 59, DateTimeKind.Unspecified).AddTicks(849), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "buildings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 858, DateTimeKind.Unspecified).AddTicks(6127), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 58, DateTimeKind.Unspecified).AddTicks(3795), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "buildings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 858, DateTimeKind.Unspecified).AddTicks(5753), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 58, DateTimeKind.Unspecified).AddTicks(3418), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 856, DateTimeKind.Unspecified).AddTicks(4316), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 56, DateTimeKind.Unspecified).AddTicks(1706), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "areas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 42, 11, 855, DateTimeKind.Unspecified).AddTicks(5013), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 9, 15, 18, 44, 17, 55, DateTimeKind.Unspecified).AddTicks(3052), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
