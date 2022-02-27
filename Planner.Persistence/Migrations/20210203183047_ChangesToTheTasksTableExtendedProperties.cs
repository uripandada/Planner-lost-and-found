using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ChangesToTheTasksTableExtendedProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(name: "credits", table: "system_tasks", nullable: true);
            migrationBuilder.Sql("UPDATE public.system_tasks SET credits = 0;");
            migrationBuilder.AlterColumn<bool>(name: "credits", table: "system_tasks", nullable: false);

            //migrationBuilder.AddColumn<int>(
            //    name: "credits",
            //    table: "system_tasks",
            //    nullable: false,
            //    defaultValue: 0);

            migrationBuilder.AddColumn<bool>(name: "is_blocking_cleaning_until_finished", table: "system_tasks", nullable: true);
            migrationBuilder.Sql("UPDATE public.system_tasks SET is_blocking_cleaning_until_finished = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_blocking_cleaning_until_finished", table: "system_tasks", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_blocking_cleaning_until_finished",
            //    table: "system_tasks",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_guest_request", table: "system_tasks", nullable: true);
            migrationBuilder.Sql("UPDATE public.system_tasks SET is_guest_request = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_guest_request", table: "system_tasks", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_guest_request",
            //    table: "system_tasks",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_major_notification_raised_when_finished", table: "system_tasks", nullable: true);
            migrationBuilder.Sql("UPDATE public.system_tasks SET is_major_notification_raised_when_finished = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_major_notification_raised_when_finished", table: "system_tasks", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_major_notification_raised_when_finished",
            //    table: "system_tasks",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_rescheduled_every_day_until_finished", table: "system_tasks", nullable: true);
            migrationBuilder.Sql("UPDATE public.system_tasks SET is_rescheduled_every_day_until_finished = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_rescheduled_every_day_until_finished", table: "system_tasks", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_rescheduled_every_day_until_finished",
            //    table: "system_tasks",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "is_shown_in_news_feed", table: "system_tasks", nullable: true);
            migrationBuilder.Sql("UPDATE public.system_tasks SET is_shown_in_news_feed = false;");
            migrationBuilder.AlterColumn<bool>(name: "is_shown_in_news_feed", table: "system_tasks", nullable: false);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_shown_in_news_feed",
            //    table: "system_tasks",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<bool>(name: "price", table: "system_tasks", nullable: true);
            migrationBuilder.Sql("UPDATE public.system_tasks SET price = 0;");
            migrationBuilder.AlterColumn<bool>(name: "price", table: "system_tasks", nullable: false);

            //migrationBuilder.AddColumn<decimal>(
            //    name: "price",
            //    table: "system_tasks",
            //    nullable: false,
            //    defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(name: "priority_key", table: "system_tasks", nullable: true);
            migrationBuilder.Sql("UPDATE public.system_tasks SET priority_key = 'NORMAL';");
            migrationBuilder.AlterColumn<bool>(name: "priority_key", table: "system_tasks", nullable: false);

            //migrationBuilder.AddColumn<string>(
            //    name: "priority_key",
            //    table: "system_tasks",
            //    nullable: false,
            //    defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "credits",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "is_blocking_cleaning_until_finished",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "is_guest_request",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "is_major_notification_raised_when_finished",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "is_rescheduled_every_day_until_finished",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "is_shown_in_news_feed",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "price",
                table: "system_tasks");

            migrationBuilder.DropColumn(
                name: "priority_key",
                table: "system_tasks");
        }
    }
}
