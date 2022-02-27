using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class RemovedReferenceToHotelIdFromFilesAndTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_files_hotels_hotel_id",
                table: "files");

            migrationBuilder.DropForeignKey(
                name: "fk_tags_hotels_hotel_id",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "ix_tags_hotel_id",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "ix_files_hotel_id",
                table: "files");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "files");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "hotel_id",
                table: "tags",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "hotel_id",
                table: "files",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_tags_hotel_id",
                table: "tags",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_files_hotel_id",
                table: "files",
                column: "hotel_id");

            migrationBuilder.AddForeignKey(
                name: "fk_files_hotels_hotel_id",
                table: "files",
                column: "hotel_id",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tags_hotels_hotel_id",
                table: "tags",
                column: "hotel_id",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
