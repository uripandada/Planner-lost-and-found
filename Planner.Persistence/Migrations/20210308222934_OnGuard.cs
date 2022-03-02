using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class OnGuard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "on_guards",
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
                    identification = table.Column<string>(nullable: true),
                    reference_number = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_on_guards", x => x.id);
                    table.ForeignKey(
                        name: "fk_on_guards_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_on_guards_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "on_guard_files",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    file_name = table.Column<string>(nullable: true),
                    file_url = table.Column<string>(nullable: true),
                    file = table.Column<byte[]>(nullable: true),
                    on_guard_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_on_guard_files", x => x.id);
                    table.ForeignKey(
                        name: "fk_on_guard_files_on_guards_on_guard_id",
                        column: x => x.on_guard_id,
                        principalTable: "on_guards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_on_guard_files_on_guard_id",
                table: "on_guard_files",
                column: "on_guard_id");

            migrationBuilder.CreateIndex(
                name: "ix_on_guards_created_by_id",
                table: "on_guards",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_on_guards_modified_by_id",
                table: "on_guards",
                column: "modified_by_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "on_guard_files");

            migrationBuilder.DropTable(
                name: "on_guards");
        }
    }
}
