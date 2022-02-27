using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ModifiedColumnsOfCleaningPluginTable1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plugins_asp_net_users_created_by_id",
                table: "cleaning_plugins");

            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plugins_asp_net_users_modified_by_id",
                table: "cleaning_plugins");

            migrationBuilder.DropIndex(
                name: "ix_cleaning_plugins_created_by_id",
                table: "cleaning_plugins");

            migrationBuilder.DropIndex(
                name: "ix_cleaning_plugins_modified_by_id",
                table: "cleaning_plugins");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "cleaning_plugins");

            migrationBuilder.DropColumn(
                name: "created_by_id",
                table: "cleaning_plugins");

            migrationBuilder.DropColumn(
                name: "modified_at",
                table: "cleaning_plugins");

            migrationBuilder.DropColumn(
                name: "modified_by_id",
                table: "cleaning_plugins");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "cleaning_plugins",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<Guid>(
                name: "created_by_id",
                table: "cleaning_plugins",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "cleaning_plugins",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<Guid>(
                name: "modified_by_id",
                table: "cleaning_plugins",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plugins_created_by_id",
                table: "cleaning_plugins",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_cleaning_plugins_modified_by_id",
                table: "cleaning_plugins",
                column: "modified_by_id");

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plugins_asp_net_users_created_by_id",
                table: "cleaning_plugins",
                column: "created_by_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plugins_asp_net_users_modified_by_id",
                table: "cleaning_plugins",
                column: "modified_by_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
