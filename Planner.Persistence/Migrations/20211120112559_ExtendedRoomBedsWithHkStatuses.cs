using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedRoomBedsWithHkStatuses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<bool>(
            //    name: "is_clean",
            //    table: "room_beds",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);
            
            migrationBuilder.AddColumn<bool>(name: "is_clean", table: "room_beds", nullable: true);
            migrationBuilder.Sql("UPDATE public.room_beds SET is_clean = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_clean", table: "room_beds", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_cleaning_in_progress",
            //    table: "room_beds",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_cleaning_in_progress", table: "room_beds", nullable: true);
            migrationBuilder.Sql("UPDATE public.room_beds SET is_cleaning_in_progress = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_cleaning_in_progress", table: "room_beds", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_do_not_disturb",
            //    table: "room_beds",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_do_not_disturb", table: "room_beds", nullable: true);
            migrationBuilder.Sql("UPDATE public.room_beds SET is_do_not_disturb = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_do_not_disturb", table: "room_beds", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_occupied",
            //    table: "room_beds",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_occupied", table: "room_beds", nullable: true);
            migrationBuilder.Sql("UPDATE public.room_beds SET is_occupied = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_occupied", table: "room_beds", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_out_of_order",
            //    table: "room_beds",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_out_of_order", table: "room_beds", nullable: true);
            migrationBuilder.Sql("UPDATE public.room_beds SET is_out_of_order = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_out_of_order", table: "room_beds", nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_clean",
                table: "room_beds");

            migrationBuilder.DropColumn(
                name: "is_cleaning_in_progress",
                table: "room_beds");

            migrationBuilder.DropColumn(
                name: "is_do_not_disturb",
                table: "room_beds");

            migrationBuilder.DropColumn(
                name: "is_occupied",
                table: "room_beds");

            migrationBuilder.DropColumn(
                name: "is_out_of_order",
                table: "room_beds");
        }
    }
}
