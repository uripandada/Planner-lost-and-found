using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class MultitenancyChanges1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "asp_net_users");

            migrationBuilder.CreateTable(
                name: "plugin",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    fully_qualified_csharp_type_name = table.Column<string>(nullable: false),
                    type_key = table.Column<string>(nullable: false),
                    is_used_by_default = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plugin", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTime>(nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    hotel_id = table.Column<string>(nullable: true),
                    clean_only_dirty_rooms = table.Column<bool>(nullable: false),
                    clean_arrivals = table.Column<bool>(nullable: false),
                    clean_stays = table.Column<bool>(nullable: false),
                    clean_departures = table.Column<bool>(nullable: false),
                    clean_vacants = table.Column<bool>(nullable: false),
                    default_check_in_time = table.Column<string>(nullable: false),
                    default_check_out_time = table.Column<string>(nullable: false),
                    default_attendant_start_time = table.Column<string>(nullable: false),
                    default_attendant_end_time = table.Column<string>(nullable: false),
                    default_attendant_max_credits = table.Column<int>(nullable: false),
                    reserve_between_cleanings = table.Column<int>(nullable: false),
                    travel_reserve = table.Column<int>(nullable: false),
                    show_hours_in_worker_planner = table.Column<bool>(nullable: false),
                    use_order_in_planning = table.Column<bool>(nullable: false),
                    show_cleaning_delays = table.Column<bool>(nullable: false),
                    allow_postpone_cleanings = table.Column<bool>(nullable: false),
                    email_addresses_for_sending_plan = table.Column<string>(nullable: false),
                    send_plan_to_attendants_by_email = table.Column<bool>(nullable: false),
                    from_email_address = table.Column<string>(nullable: false),
                    fix_planned_activities_while_filtering = table.Column<bool>(nullable: false),
                    use_groups = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_settings", x => x.id);
                    table.ForeignKey(
                        name: "fk_settings_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_settings_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_settings_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "hotel_plugin",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    hotel_id = table.Column<string>(nullable: false),
                    plugin_id = table.Column<Guid>(nullable: false),
                    plugin_data = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_hotel_plugin", x => x.id);
                    table.ForeignKey(
                        name: "fk_hotel_plugin_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_hotel_plugin_plugin_plugin_id",
                        column: x => x.plugin_id,
                        principalTable: "plugin",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_hotel_plugin_hotel_id",
                table: "hotel_plugin",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "ix_hotel_plugin_plugin_id",
                table: "hotel_plugin",
                column: "plugin_id");

            migrationBuilder.CreateIndex(
                name: "ix_settings_created_by_id",
                table: "settings",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_settings_hotel_id",
                table: "settings",
                column: "hotel_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_settings_modified_by_id",
                table: "settings",
                column: "modified_by_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hotel_plugin");

            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.DropTable(
                name: "plugin");

            migrationBuilder.AddColumn<string>(
                name: "hotel_id",
                table: "asp_net_users",
                type: "text",
                nullable: true);
        }
    }
}
