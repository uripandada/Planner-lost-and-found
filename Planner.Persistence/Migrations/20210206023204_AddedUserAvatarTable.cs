using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedUserAvatarTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "application_user_avatar",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    file_name = table.Column<string>(nullable: false),
                    file_url = table.Column<string>(nullable: true),
                    file = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_user_avatar", x => x.id);
                    table.ForeignKey(
                        name: "fk_application_user_avatar_asp_net_users_id",
                        column: x => x.id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_user_avatar");
        }
    }
}
