using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Planner.Persistence.Migrations
{
    public partial class AddedExperienceCompensationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "experience_compensations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    modified_by_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_experience_compensations", x => x.id);
                    table.ForeignKey(
                        name: "fk_experience_compensations_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_experience_compensations_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_experience_compensations_created_by_id",
                table: "experience_compensations",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_experience_compensations_modified_by_id",
                table: "experience_compensations",
                column: "modified_by_id");

            migrationBuilder.AddForeignKey(
                name: "fk_experiences_experience_compensations_experience_categ~",
                table: "experiences",
                column: "experience_compensation_id",
                principalTable: "experience_compensations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_experiences_experience_compensations_experience_categ~",
                table: "experiences");

            migrationBuilder.DropTable(
                name: "experience_compensations");
        }
    }
}
