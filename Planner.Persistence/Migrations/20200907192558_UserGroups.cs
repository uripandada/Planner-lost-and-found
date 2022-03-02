using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class UserGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "connection_name",
                table: "asp_net_users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                table: "asp_net_users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hotel_id",
                table: "asp_net_users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "language",
                table: "asp_net_users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                table: "asp_net_users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "original_hotel",
                table: "asp_net_users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "registration_number",
                table: "asp_net_users",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "user_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTimeOffset>(nullable: false),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    hotel_id = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_groups_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_groups_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_sub_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    created_by_id = table.Column<Guid>(nullable: false),
                    modified_at = table.Column<DateTimeOffset>(nullable: false),
                    modified_by_id = table.Column<Guid>(nullable: false),
                    hotel_id = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    user_group_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_sub_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_sub_groups_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_sub_groups_asp_net_users_modified_by_id",
                        column: x => x.modified_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_sub_groups_user_groups_user_group_id",
                        column: x => x.user_group_id,
                        principalTable: "user_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_groups_created_by_id",
                table: "user_groups",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_groups_modified_by_id",
                table: "user_groups",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_sub_groups_created_by_id",
                table: "user_sub_groups",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_sub_groups_modified_by_id",
                table: "user_sub_groups",
                column: "modified_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_sub_groups_user_group_id",
                table: "user_sub_groups",
                column: "user_group_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_sub_groups");

            migrationBuilder.DropTable(
                name: "user_groups");

            migrationBuilder.DropColumn(
                name: "connection_name",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "first_name",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "language",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "last_name",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "original_hotel",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "registration_number",
                table: "asp_net_users");
        }
    }
}
