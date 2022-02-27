using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class FixOnGuardLostAndFoundFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_on_guard_files",
                table: "on_guard_files");

            migrationBuilder.DropIndex(
                name: "ix_on_guard_files_on_guard_id",
                table: "on_guard_files");

            migrationBuilder.DropPrimaryKey(
                name: "pk_lost_and_found_files",
                table: "lost_and_found_files");

            migrationBuilder.DropIndex(
                name: "ix_lost_and_found_files_lost_and_found_id",
                table: "lost_and_found_files");

            migrationBuilder.DropColumn(
                name: "id",
                table: "on_guard_files");

            migrationBuilder.DropColumn(
                name: "file",
                table: "on_guard_files");

            migrationBuilder.DropColumn(
                name: "file_name",
                table: "on_guard_files");

            migrationBuilder.DropColumn(
                name: "file_url",
                table: "on_guard_files");

            migrationBuilder.DropColumn(
                name: "id",
                table: "lost_and_found_files");

            migrationBuilder.DropColumn(
                name: "file",
                table: "lost_and_found_files");

            migrationBuilder.DropColumn(
                name: "file_name",
                table: "lost_and_found_files");

            migrationBuilder.DropColumn(
                name: "file_url",
                table: "lost_and_found_files");

            migrationBuilder.AddColumn<Guid>(
                name: "file_id",
                table: "on_guard_files",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "lost_and_founds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "place",
                table: "lost_and_founds",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "file_id",
                table: "lost_and_found_files",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "pk_on_guard_files",
                table: "on_guard_files",
                columns: new[] { "on_guard_id", "file_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_lost_and_found_files",
                table: "lost_and_found_files",
                columns: new[] { "lost_and_found_id", "file_id" });

            migrationBuilder.CreateIndex(
                name: "ix_on_guard_files_file_id",
                table: "on_guard_files",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "ix_lost_and_found_files_file_id",
                table: "lost_and_found_files",
                column: "file_id");

            migrationBuilder.AddForeignKey(
                name: "fk_lost_and_found_files_files_file_id",
                table: "lost_and_found_files",
                column: "file_id",
                principalTable: "files",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_on_guard_files_files_file_id",
                table: "on_guard_files",
                column: "file_id",
                principalTable: "files",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_lost_and_found_files_files_file_id",
                table: "lost_and_found_files");

            migrationBuilder.DropForeignKey(
                name: "fk_on_guard_files_files_file_id",
                table: "on_guard_files");

            migrationBuilder.DropPrimaryKey(
                name: "pk_on_guard_files",
                table: "on_guard_files");

            migrationBuilder.DropIndex(
                name: "ix_on_guard_files_file_id",
                table: "on_guard_files");

            migrationBuilder.DropPrimaryKey(
                name: "pk_lost_and_found_files",
                table: "lost_and_found_files");

            migrationBuilder.DropIndex(
                name: "ix_lost_and_found_files_file_id",
                table: "lost_and_found_files");

            migrationBuilder.DropColumn(
                name: "file_id",
                table: "on_guard_files");

            migrationBuilder.DropColumn(
                name: "description",
                table: "lost_and_founds");

            migrationBuilder.DropColumn(
                name: "place",
                table: "lost_and_founds");

            migrationBuilder.DropColumn(
                name: "file_id",
                table: "lost_and_found_files");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "on_guard_files",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<byte[]>(
                name: "file",
                table: "on_guard_files",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "file_name",
                table: "on_guard_files",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "file_url",
                table: "on_guard_files",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "lost_and_found_files",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<byte[]>(
                name: "file",
                table: "lost_and_found_files",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "file_name",
                table: "lost_and_found_files",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "file_url",
                table: "lost_and_found_files",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_on_guard_files",
                table: "on_guard_files",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_lost_and_found_files",
                table: "lost_and_found_files",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_on_guard_files_on_guard_id",
                table: "on_guard_files",
                column: "on_guard_id");

            migrationBuilder.CreateIndex(
                name: "ix_lost_and_found_files_lost_and_found_id",
                table: "lost_and_found_files",
                column: "lost_and_found_id");
        }
    }
}
