using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedCleaningGeneratorLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "lost_and_founds",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 12);

            migrationBuilder.CreateTable(
                name: "cleaning_generator_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    generation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    room_description = table.Column<string>(type: "text", nullable: true),
                    reservations_description = table.Column<string>(type: "text", nullable: true),
                    reservations_events_description = table.Column<string>(type: "text", nullable: true),
                    plugin_events_description = table.Column<string>(type: "text", nullable: true),
                    ordered_plugins_description = table.Column<string>(type: "text", nullable: true),
                    cleaning_events_description = table.Column<string>(type: "text", nullable: true),
                    cleanings_description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cleaning_generator_logs", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cleaning_generator_logs");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "lost_and_founds",
                type: "integer",
                nullable: false,
                defaultValue: 12,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);
        }
    }
}
