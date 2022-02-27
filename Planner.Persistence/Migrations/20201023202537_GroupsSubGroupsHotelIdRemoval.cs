using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class GroupsSubGroupsHotelIdRemoval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_groups_hotels_hotel_id",
                table: "user_groups");

            migrationBuilder.DropForeignKey(
                name: "fk_user_sub_groups_hotels_hotel_id",
                table: "user_sub_groups");

            migrationBuilder.DropIndex(
                name: "ix_user_sub_groups_hotel_id",
                table: "user_sub_groups");

            migrationBuilder.DropIndex(
                name: "ix_user_groups_hotel_id",
                table: "user_groups");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "user_sub_groups");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "user_groups");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "hotel_id",
                table: "user_sub_groups",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "hotel_id",
                table: "user_groups",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_user_sub_groups_hotel_id",
                table: "user_sub_groups",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_groups_hotel_id",
                table: "user_groups",
                column: "hotel_id");

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
    }
}
