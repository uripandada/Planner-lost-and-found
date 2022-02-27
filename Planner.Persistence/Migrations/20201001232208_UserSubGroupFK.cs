using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class UserSubGroupFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "user_group_id1",
                table: "user_sub_groups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_sub_groups_user_group_id1",
                table: "user_sub_groups",
                column: "user_group_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_user_sub_groups_user_groups_user_group_id1",
                table: "user_sub_groups",
                column: "user_group_id1",
                principalTable: "user_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_sub_groups_user_groups_user_group_id1",
                table: "user_sub_groups");

            migrationBuilder.DropIndex(
                name: "ix_user_sub_groups_user_group_id1",
                table: "user_sub_groups");

            migrationBuilder.DropColumn(
                name: "user_group_id1",
                table: "user_sub_groups");
        }
    }
}
