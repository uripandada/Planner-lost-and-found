using Microsoft.EntityFrameworkCore.Migrations;

namespace Planner.Persistence.Migrations
{
    public partial class ChangedToCascadeDeletesCleaningPlanDependantTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                table: "asp_net_role_claims");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_claims_asp_net_users_user_id",
                table: "asp_net_user_claims");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_logins_asp_net_users_user_id",
                table: "asp_net_user_logins");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                table: "asp_net_user_roles");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_roles_asp_net_users_user_id",
                table: "asp_net_user_roles");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                table: "asp_net_user_tokens");

            migrationBuilder.DropForeignKey(
                name: "fk_asset_files_files_file_id",
                table: "asset_files");

            migrationBuilder.DropForeignKey(
                name: "fk_asset_tags_tags_tag_key",
                table: "asset_tags");

            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plan_group_availability_intervals_cleaning_plan_gr~",
                table: "cleaning_plan_group_availability_intervals");

            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plan_group_floor_affinities_cleaning_plan_groups_c~",
                table: "cleaning_plan_group_floor_affinities");

            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plan_groups_cleaning_plans_cleaning_plan_id",
                table: "cleaning_plan_groups");

            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plan_items_cleaning_plan_groups_cleaning_plan_grou~",
                table: "cleaning_plan_items");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                table: "asp_net_role_claims",
                column: "role_id",
                principalTable: "asp_net_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_claims_asp_net_users_user_id",
                table: "asp_net_user_claims",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_logins_asp_net_users_user_id",
                table: "asp_net_user_logins",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                table: "asp_net_user_roles",
                column: "role_id",
                principalTable: "asp_net_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_roles_asp_net_users_user_id",
                table: "asp_net_user_roles",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                table: "asp_net_user_tokens",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asset_files_files_file_id",
                table: "asset_files",
                column: "file_id",
                principalTable: "files",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asset_tags_tags_tag_key",
                table: "asset_tags",
                column: "tag_key",
                principalTable: "tags",
                principalColumn: "key",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plan_group_availability_intervals_cleaning_plan_gr~",
                table: "cleaning_plan_group_availability_intervals",
                column: "cleaning_plan_group_id",
                principalTable: "cleaning_plan_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plan_group_floor_affinities_cleaning_plan_groups_c~",
                table: "cleaning_plan_group_floor_affinities",
                column: "cleaning_plan_group_id",
                principalTable: "cleaning_plan_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plan_groups_cleaning_plans_cleaning_plan_id",
                table: "cleaning_plan_groups",
                column: "cleaning_plan_id",
                principalTable: "cleaning_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plan_items_cleaning_plan_groups_cleaning_plan_grou~",
                table: "cleaning_plan_items",
                column: "cleaning_plan_group_id",
                principalTable: "cleaning_plan_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                table: "asp_net_role_claims");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_claims_asp_net_users_user_id",
                table: "asp_net_user_claims");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_logins_asp_net_users_user_id",
                table: "asp_net_user_logins");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                table: "asp_net_user_roles");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_roles_asp_net_users_user_id",
                table: "asp_net_user_roles");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                table: "asp_net_user_tokens");

            migrationBuilder.DropForeignKey(
                name: "fk_asset_files_files_file_id",
                table: "asset_files");

            migrationBuilder.DropForeignKey(
                name: "fk_asset_tags_tags_tag_key",
                table: "asset_tags");

            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plan_group_availability_intervals_cleaning_plan_gr~",
                table: "cleaning_plan_group_availability_intervals");

            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plan_group_floor_affinities_cleaning_plan_groups_c~",
                table: "cleaning_plan_group_floor_affinities");

            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plan_groups_cleaning_plans_cleaning_plan_id",
                table: "cleaning_plan_groups");

            migrationBuilder.DropForeignKey(
                name: "fk_cleaning_plan_items_cleaning_plan_groups_cleaning_plan_grou~",
                table: "cleaning_plan_items");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                table: "asp_net_role_claims",
                column: "role_id",
                principalTable: "asp_net_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_claims_asp_net_users_user_id",
                table: "asp_net_user_claims",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_logins_asp_net_users_user_id",
                table: "asp_net_user_logins",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                table: "asp_net_user_roles",
                column: "role_id",
                principalTable: "asp_net_roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_roles_asp_net_users_user_id",
                table: "asp_net_user_roles",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                table: "asp_net_user_tokens",
                column: "user_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_asset_files_files_file_id",
                table: "asset_files",
                column: "file_id",
                principalTable: "files",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_asset_tags_tags_tag_key",
                table: "asset_tags",
                column: "tag_key",
                principalTable: "tags",
                principalColumn: "key",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plan_group_availability_intervals_cleaning_plan_gr~",
                table: "cleaning_plan_group_availability_intervals",
                column: "cleaning_plan_group_id",
                principalTable: "cleaning_plan_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plan_group_floor_affinities_cleaning_plan_groups_c~",
                table: "cleaning_plan_group_floor_affinities",
                column: "cleaning_plan_group_id",
                principalTable: "cleaning_plan_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plan_groups_cleaning_plans_cleaning_plan_id",
                table: "cleaning_plan_groups",
                column: "cleaning_plan_id",
                principalTable: "cleaning_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_cleaning_plan_items_cleaning_plan_groups_cleaning_plan_grou~",
                table: "cleaning_plan_items",
                column: "cleaning_plan_group_id",
                principalTable: "cleaning_plan_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
