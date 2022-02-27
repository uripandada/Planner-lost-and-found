using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class FixLostAndFound1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE lost_and_founds SET is_closed = false WHERE is_closed IS NULL;");
            migrationBuilder.Sql("ALTER TABLE lost_and_founds ALTER COLUMN is_closed DROP DEFAULT;");
            
            migrationBuilder.Sql("UPDATE lost_and_founds SET is_deleted = false WHERE is_deleted IS NULL;");
            migrationBuilder.Sql("ALTER TABLE lost_and_founds ALTER COLUMN is_deleted DROP DEFAULT;");

            migrationBuilder.Sql("ALTER TABLE lost_and_founds ALTER COLUMN country DROP NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE lost_and_founds ALTER COLUMN city DROP NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE lost_and_founds ALTER COLUMN address DROP NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE lost_and_founds ALTER COLUMN phone_number DROP NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE lost_and_founds ALTER COLUMN reference_number DROP NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE lost_and_founds ALTER COLUMN first_name DROP NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE lost_and_founds ALTER COLUMN last_name DROP NOT NULL;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
