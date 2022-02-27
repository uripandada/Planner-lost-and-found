using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Planner.Persistence.Migrations
{
    public partial class MigrationsFix1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE asset_actions DROP COLUMN IF EXISTS asset_id; ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
