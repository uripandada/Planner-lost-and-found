using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ExtendedCleaningPlanSendingHistory1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_status_asp_net_users_id",
                table: "user_status");

            migrationBuilder.DropForeignKey(
                name: "fk_user_status_rooms_room_id",
                table: "user_status");

            migrationBuilder.DropForeignKey(
                name: "fk_user_status_history_asp_net_users_user_id",
                table: "user_status_history");

            migrationBuilder.DropForeignKey(
                name: "fk_user_status_history_rooms_room_id",
                table: "user_status_history");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_status_history",
                table: "user_status_history");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_status",
                table: "user_status");

            migrationBuilder.RenameTable(
                name: "user_status_history",
                newName: "user_status_histories");

            migrationBuilder.RenameTable(
                name: "user_status",
                newName: "user_statuses");

            migrationBuilder.RenameIndex(
                name: "ix_user_status_history_user_id",
                table: "user_status_histories",
                newName: "ix_user_status_histories_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_user_status_history_room_id",
                table: "user_status_histories",
                newName: "ix_user_status_histories_room_id");

            migrationBuilder.RenameIndex(
                name: "ix_user_status_room_id",
                table: "user_statuses",
                newName: "ix_user_statuses_room_id");

            migrationBuilder.AddColumn<string>(
                name: "cleaning_plan_json",
                table: "cleaning_plan_sending_histories",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_status_histories",
                table: "user_status_histories",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_statuses",
                table: "user_statuses",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_status_histories_asp_net_users_user_id",
                table: "user_status_histories",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_status_histories_rooms_room_id",
                table: "user_status_histories",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_statuses_asp_net_users_id",
                table: "user_statuses",
                column: "id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_statuses_rooms_room_id",
                table: "user_statuses",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_status_histories_asp_net_users_user_id",
                table: "user_status_histories");

            migrationBuilder.DropForeignKey(
                name: "fk_user_status_histories_rooms_room_id",
                table: "user_status_histories");

            migrationBuilder.DropForeignKey(
                name: "fk_user_statuses_asp_net_users_id",
                table: "user_statuses");

            migrationBuilder.DropForeignKey(
                name: "fk_user_statuses_rooms_room_id",
                table: "user_statuses");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_statuses",
                table: "user_statuses");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_status_histories",
                table: "user_status_histories");

            migrationBuilder.DropColumn(
                name: "cleaning_plan_json",
                table: "cleaning_plan_sending_histories");

            migrationBuilder.RenameTable(
                name: "user_statuses",
                newName: "user_status");

            migrationBuilder.RenameTable(
                name: "user_status_histories",
                newName: "user_status_history");

            migrationBuilder.RenameIndex(
                name: "ix_user_statuses_room_id",
                table: "user_status",
                newName: "ix_user_status_room_id");

            migrationBuilder.RenameIndex(
                name: "ix_user_status_histories_user_id",
                table: "user_status_history",
                newName: "ix_user_status_history_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_user_status_histories_room_id",
                table: "user_status_history",
                newName: "ix_user_status_history_room_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_status",
                table: "user_status",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_status_history",
                table: "user_status_history",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_status_asp_net_users_id",
                table: "user_status",
                column: "id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_status_rooms_room_id",
                table: "user_status",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_status_history_asp_net_users_user_id",
                table: "user_status_history",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_status_history_rooms_room_id",
                table: "user_status_history",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
