using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Planner.Persistence.Migrations
{
    public partial class AddedExperienceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "experiences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    room_name = table.Column<string>(type: "text", nullable: false),
                    guest_name = table.Column<string>(type: "text", nullable: false),
                    check_in = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    check_out = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    reservation_id = table.Column<string>(type: "text", nullable: false),
                    vip = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "0"),
                    description = table.Column<string>(type: "text", nullable: true),
                    actions = table.Column<string>(type: "text", nullable: true),
                    group = table.Column<string>(type: "text", nullable: true),
                    internal_follow_up = table.Column<string>(type: "text", nullable: true),
                    experience_category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    experience_compensation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_experiences", x => x.id);
                    table.ForeignKey(
                        name: "fk_experiences_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_experiences_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_experiences_experience_categories_experience_category_id",
                        column: x => x.experience_category_id,
                        principalTable: "experience_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_experiences_experience_compensations_experience_compensation_id",
                        column: x => x.experience_compensation_id,
                        principalTable: "experience_compensations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_experiences_created_by_id",
                table: "experiences",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_experiences_modified_by_id",
                table: "experiences",
                column: "modified_by_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "experience_categories");
        }
    }
}
