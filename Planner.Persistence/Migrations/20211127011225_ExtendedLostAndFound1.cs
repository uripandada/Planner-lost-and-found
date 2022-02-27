using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedLostAndFound1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "type_of_loss",
                table: "lost_and_founds",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "lost_and_founds",
                type: "text",
                nullable: false,
                defaultValue: "Unknown",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "lost_and_founds",
                type: "integer",
                nullable: false,
                defaultValue: 12,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "hotel_id",
                table: "lost_and_founds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "lost_and_founds",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_closed",
                table: "lost_and_founds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "lost_and_founds",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "rcc_status",
                table: "lost_and_founds",
                type: "text",
                nullable: false,
                defaultValue: "UNKNOWN");

            migrationBuilder.CreateIndex(
                name: "ix_lost_and_founds_hotel_id",
                table: "lost_and_founds",
                column: "hotel_id");

            migrationBuilder.AddForeignKey(
                name: "fk_lost_and_founds_hotels_hotel_id",
                table: "lost_and_founds",
                column: "hotel_id",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_lost_and_founds_hotels_hotel_id",
                table: "lost_and_founds");

            migrationBuilder.DropIndex(
                name: "ix_lost_and_founds_hotel_id",
                table: "lost_and_founds");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "lost_and_founds");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "lost_and_founds");

            migrationBuilder.DropColumn(
                name: "is_closed",
                table: "lost_and_founds");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "lost_and_founds");

            migrationBuilder.DropColumn(
                name: "rcc_status",
                table: "lost_and_founds");

            migrationBuilder.AlterColumn<int>(
                name: "type_of_loss",
                table: "lost_and_founds",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "lost_and_founds",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Unknown");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "lost_and_founds",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 12);
        }
    }
}
