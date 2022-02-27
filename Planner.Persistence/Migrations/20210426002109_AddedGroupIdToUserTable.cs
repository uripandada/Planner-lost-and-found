using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class AddedGroupIdToUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "user_group_id",
                table: "asp_net_users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_user_group_id",
                table: "asp_net_users",
                column: "user_group_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_user_groups_user_group_id",
                table: "asp_net_users",
                column: "user_group_id",
                principalTable: "user_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_user_groups_user_group_id",
                table: "asp_net_users");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_user_group_id",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "user_group_id",
                table: "asp_net_users");
        }
    }
}
