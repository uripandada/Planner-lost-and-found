using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedAssetFileLinkWithQrCodeFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<bool>(
            //    name: "is_qr_code_image",
            //    table: "asset_files",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_qr_code_image", table: "asset_files", nullable: true);
            migrationBuilder.Sql("UPDATE public.asset_files SET is_qr_code_image = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_qr_code_image", table: "asset_files", nullable: false);

            migrationBuilder.Sql("UPDATE public.asset_files SET is_primary_image = false WHERE is_primary_image IS NULL;");
            migrationBuilder.AlterColumn<bool>(name: "is_primary_image", table: "asset_files", nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_qr_code_image",
                table: "asset_files");
        }
    }
}
