using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class UpgradeToDotNet5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "consumed_time",
                table: "persisted_grants",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "persisted_grants",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "session_id",
                table: "persisted_grants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "device_codes",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "session_id",
                table: "device_codes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_persisted_grants_subject_id_session_id_type",
                table: "persisted_grants",
                columns: new[] { "subject_id", "session_id", "type" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_persisted_grants_subject_id_session_id_type",
                table: "persisted_grants");

            migrationBuilder.DropColumn(
                name: "consumed_time",
                table: "persisted_grants");

            migrationBuilder.DropColumn(
                name: "description",
                table: "persisted_grants");

            migrationBuilder.DropColumn(
                name: "session_id",
                table: "persisted_grants");

            migrationBuilder.DropColumn(
                name: "description",
                table: "device_codes");

            migrationBuilder.DropColumn(
                name: "session_id",
                table: "device_codes");
        }
    }
}
