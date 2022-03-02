using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class LostAndFound : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lost_and_founds",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTimeOffset>(nullable: false),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    first_name = table.Column<string>(nullable: true),
                    last_name = table.Column<string>(nullable: true),
                    address = table.Column<string>(nullable: true),
                    phone_number = table.Column<string>(nullable: true),
                    reference_number = table.Column<string>(nullable: true),
                    lost_on = table.Column<DateTime>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    type_of_loss = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lost_and_founds", x => x.id);
                    table.ForeignKey(
                        name: "fk_lost_and_founds_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_lost_and_founds_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lost_and_found_files",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    file_name = table.Column<string>(nullable: true),
                    file_url = table.Column<string>(nullable: true),
                    file = table.Column<byte[]>(nullable: true),
                    lost_and_found_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lost_and_found_files", x => x.id);
                    table.ForeignKey(
                        name: "fk_lost_and_found_files_lost_and_founds_lost_and_found_id",
                        column: x => x.lost_and_found_id,
                        principalTable: "lost_and_founds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_lost_and_found_files_lost_and_found_id",
                table: "lost_and_found_files",
                column: "lost_and_found_id");

            migrationBuilder.CreateIndex(
                name: "ix_lost_and_founds_created_by_id",
                table: "lost_and_founds",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_lost_and_founds_modified_by_id",
                table: "lost_and_founds",
                column: "modified_by_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lost_and_found_files");

            migrationBuilder.DropTable(
                name: "lost_and_founds");
        }
    }
}
