using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class RemovedAssetGroupNameUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "ix_asset_groups_name",
            //    table: "asset_groups");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateIndex(
            //    name: "ix_asset_groups_name",
            //    table: "asset_groups",
            //    column: "name",
            //    unique: true);
        }
    }
}
