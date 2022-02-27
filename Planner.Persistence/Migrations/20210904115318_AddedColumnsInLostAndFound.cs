using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedColumnsInLostAndFound : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_lost_and_founds_asp_net_users_created_by_id",
                table: "lost_and_founds");

            migrationBuilder.DropForeignKey(
                name: "fk_lost_and_founds_asp_net_users_modified_by_id",
                table: "lost_and_founds");

            migrationBuilder.DropColumn(
                name: "place",
                table: "lost_and_founds");

            migrationBuilder.AlterColumn<string>(
                name: "reference_number",
                table: "lost_and_founds",
                type: "text",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "lost_and_founds",
                type: "text",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "lost_and_founds",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "lost_and_founds",
                type: "text",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "lost_and_founds",
                type: "text",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "lost_and_founds",
                type: "text",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "lost_and_founds",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "lost_and_founds",
                type: "text",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "street",
                table: "lost_and_founds",
                type: "text",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "lost_and_founds",
                type: "text",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "lost_and_founds",
                type: "text",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "lost_and_founds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "postal_code",
                table: "lost_and_founds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reservation_id",
                table: "lost_and_founds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "room_id",
                table: "lost_and_founds",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_lost_and_founds_reservation_id",
                table: "lost_and_founds",
                column: "reservation_id");

            migrationBuilder.CreateIndex(
                name: "ix_lost_and_founds_room_id",
                table: "lost_and_founds",
                column: "room_id");

            migrationBuilder.AddForeignKey(
                name: "fk_lost_and_founds_asp_net_users_created_by_id",
                table: "lost_and_founds",
                column: "created_by_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_lost_and_founds_asp_net_users_modified_by_id",
                table: "lost_and_founds",
                column: "modified_by_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_lost_and_founds_reservations_reservation_id",
                table: "lost_and_founds",
                column: "reservation_id",
                principalTable: "reservations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_lost_and_founds_rooms_room_id",
                table: "lost_and_founds",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_lost_and_founds_asp_net_users_created_by_id",
                table: "lost_and_founds");

            migrationBuilder.DropForeignKey(
                name: "fk_lost_and_founds_asp_net_users_modified_by_id",
                table: "lost_and_founds");

            migrationBuilder.DropForeignKey(
                name: "fk_lost_and_founds_reservations_reservation_id",
                table: "lost_and_founds");

            migrationBuilder.DropForeignKey(
                name: "fk_lost_and_founds_rooms_room_id",
                table: "lost_and_founds");

            migrationBuilder.DropIndex(
                name: "ix_lost_and_founds_reservation_id",
                table: "lost_and_founds");

            migrationBuilder.DropIndex(
                name: "ix_lost_and_founds_room_id",
                table: "lost_and_founds");

            migrationBuilder.DropColumn(
                name: "city",
                table: "lost_and_founds");

            migrationBuilder.DropColumn(
                name: "country",
                table: "lost_and_founds");

            migrationBuilder.DropColumn(
                name: "email",
                table: "lost_and_founds");

            migrationBuilder.DropColumn(
                name: "postal_code",
                table: "lost_and_founds");

            migrationBuilder.DropColumn(
                name: "reservation_id",
                table: "lost_and_founds");

            migrationBuilder.DropColumn(
                name: "room_id",
                table: "lost_and_founds");

            migrationBuilder.DropColumn(
                name: "street",
                table: "lost_and_founds");

            migrationBuilder.AddColumn<Guid>(
                 name: "place",
                 table: "lost_and_founds",
                 type: "text",
                 nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "reference_number",
                table: "lost_and_founds",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "phone_number",
                table: "lost_and_founds",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "lost_and_founds",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "lost_and_founds",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "first_name",
                table: "lost_and_founds",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "lost_and_founds",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "lost_and_founds",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "lost_and_founds",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "fk_lost_and_founds_asp_net_users_created_by_id",
                table: "lost_and_founds",
                column: "created_by_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_lost_and_founds_asp_net_users_modified_by_id",
                table: "lost_and_founds",
                column: "modified_by_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
