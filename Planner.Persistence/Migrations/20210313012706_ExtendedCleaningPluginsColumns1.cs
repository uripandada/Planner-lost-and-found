using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedCleaningPluginsColumns1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<bool>(
            //    name: "is_active",
            //    table: "cleaning_plugins",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_active", table: "cleaning_plugins", nullable: true);
            migrationBuilder.Sql("UPDATE public.cleaning_plugins SET is_active = true;");
            migrationBuilder.AlterColumn<bool>(name: "is_active", table: "cleaning_plugins", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_top_rule",
            //    table: "cleaning_plugins",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_top_rule", table: "cleaning_plugins", nullable: true);
            migrationBuilder.Sql("UPDATE public.cleaning_plugins SET is_top_rule = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_top_rule", table: "cleaning_plugins", nullable: false);

            //migrationBuilder.AddColumn<int>(
            //    name: "ordinal_number",
            //    table: "cleaning_plugins",
            //    nullable: false,
            //    defaultValue: 0);

            migrationBuilder.AddColumn<bool>(name: "ordinal_number", table: "cleaning_plugins", nullable: true);
            migrationBuilder.Sql("UPDATE public.cleaning_plugins SET ordinal_number = 0;");
            migrationBuilder.AlterColumn<bool>(name: "ordinal_number", table: "cleaning_plugins", nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "cleaning_plugins");

            migrationBuilder.DropColumn(
                name: "is_top_rule",
                table: "cleaning_plugins");

            migrationBuilder.DropColumn(
                name: "ordinal_number",
                table: "cleaning_plugins");
        }
    }
}
