using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Planner.Common;
using Planner.Domain.Values;

namespace Planner.Persistence.Migrations
{
    public partial class RoleManualChangesAddedMoreSystemRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "modified_at",
            //    table: "cleanings");

            var updateRolesSql = $@"

                UPDATE public.asp_net_roles
                SET 
                    name = '{SystemDefaults.Roles.Administrator.Name}',
                    normalized_name = '{SystemDefaults.Roles.Administrator.NormalizedName}',
                    concurrency_stamp = '{Guid.NewGuid()}',
                    hotel_access_type_key = 'ALL'
                WHERE normalized_name = 'ADMIN'
                AND NOT EXISTS(SELECT ir.* FROM public.asp_net_roles ir WHERE ir.name = '{SystemDefaults.Roles.Administrator.Name}');

                UPDATE public.asp_net_roles
                SET 
                    name = '{SystemDefaults.Roles.Maintenance.Name}',
                    normalized_name = '{SystemDefaults.Roles.Maintenance.NormalizedName}',
                    concurrency_stamp = '{Guid.NewGuid()}',
                    hotel_access_type_key = 'MULTIPLE'
                WHERE normalized_name = 'TECH'
                AND NOT EXISTS(SELECT ir.* FROM public.asp_net_roles ir WHERE ir.name = '{SystemDefaults.Roles.Maintenance.Name}');

                UPDATE public.asp_net_roles
                SET 
                    name = '{SystemDefaults.Roles.Attendant.Name}',
                    normalized_name = '{SystemDefaults.Roles.Attendant.NormalizedName}',
                    concurrency_stamp = '{Guid.NewGuid()}',
                    hotel_access_type_key = 'SINGLE'
                WHERE normalized_name = 'CLEANER'
                AND NOT EXISTS(SELECT ir.* FROM public.asp_net_roles ir WHERE ir.name = '{SystemDefaults.Roles.Attendant.Name}');

            ";

            //var inspectorRoleId = Guid.NewGuid();
            //var receptionistRoleId = Guid.NewGuid();

            //var inspectorRoleName = SystemDefaults.Roles.Inspector.Name;
            //var receptionistRoleName = SystemDefaults.Roles.Receptionist.Name;

            //var addRolesSql = $@"
            //    INSERT INTO public.asp_net_roles 
            //        (id, name, normalized_name, concurrency_stamp, is_system_role, hotel_access_type_key)
            //    VALUES 
            //        ('{receptionistRoleId.ToString()}', '{SystemDefaults.Roles.Receptionist.Name}', '{SystemDefaults.Roles.Receptionist.NormalizedName}', '{Guid.NewGuid().ToString()}', true, 'SINGLE')
            //    ON CONFLICT DO NOTHING;

            //    UPDATE public.asp_net_roles 
            //    SET 
            //        name = '{SystemDefaults.Roles.Receptionist.Name}',
            //        normalized_name = '{SystemDefaults.Roles.Receptionist.NormalizedName}',
            //        concurrency_stamp = '{Guid.NewGuid().ToString()}',
            //        is_system_role = true,
            //        hotel_access_type_key = 'SINGLE'
            //    WHERE
            //        normalized_name = '{SystemDefaults.Roles.Receptionist.NormalizedName}';


            //    INSERT INTO public.asp_net_roles 
            //        (id, name, normalized_name, concurrency_stamp, is_system_role, hotel_access_type_key)
            //    VALUES 
            //        ('{inspectorRoleId.ToString()}', '{SystemDefaults.Roles.Inspector.Name}', '{SystemDefaults.Roles.Inspector.NormalizedName}', '{Guid.NewGuid().ToString()}', true, 'SINGLE')
            //    ON CONFLICT DO NOTHING;

            //    UPDATE public.asp_net_roles 
            //    SET 
            //        name = '{SystemDefaults.Roles.Inspector.Name}',
            //        normalized_name = '{SystemDefaults.Roles.Inspector.NormalizedName}',
            //        concurrency_stamp = '{Guid.NewGuid().ToString()}',
            //        is_system_role = true,
            //        hotel_access_type_key = 'SINGLE'
            //    WHERE
            //        normalized_name = '{SystemDefaults.Roles.Inspector.NormalizedName}';


            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.SettingsClaimKeys.Rooms}','{ClaimsKeys.SettingsClaimKeys.Rooms}' FROM public.asp_net_roles r WHERE r.name = '{inspectorRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.SettingsClaimKeys.Assets}','{ClaimsKeys.SettingsClaimKeys.Assets}' FROM public.asp_net_roles r WHERE r.name = '{inspectorRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.SettingsClaimKeys.Users}','{ClaimsKeys.SettingsClaimKeys.Users}' FROM public.asp_net_roles r WHERE r.name = '{inspectorRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.SettingsClaimKeys.RoleManagement}','{ClaimsKeys.SettingsClaimKeys.RoleManagement}' FROM public.asp_net_roles r WHERE r.name = '{inspectorRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.SettingsClaimKeys.RoomCategories}','{ClaimsKeys.SettingsClaimKeys.RoomCategories}' FROM public.asp_net_roles r WHERE r.name = '{inspectorRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.SettingsClaimKeys.HotelSettings}','{ClaimsKeys.SettingsClaimKeys.HotelSettings}' FROM public.asp_net_roles r WHERE r.name = '{inspectorRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.SettingsClaimKeys.Colors}','{ClaimsKeys.SettingsClaimKeys.Colors}' FROM public.asp_net_roles r WHERE r.name = '{inspectorRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.RoomInsights}','{ClaimsKeys.ManagementClaimKeys.RoomInsights}' FROM public.asp_net_roles r WHERE r.name = '{inspectorRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.UserInsights}','{ClaimsKeys.ManagementClaimKeys.UserInsights}' FROM public.asp_net_roles r WHERE r.name = '{inspectorRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.Tasks}','{ClaimsKeys.ManagementClaimKeys.Tasks}' FROM public.asp_net_roles r WHERE r.name = '{inspectorRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.Reservations}','{ClaimsKeys.ManagementClaimKeys.Reservations}' FROM public.asp_net_roles r WHERE r.name = '{inspectorRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.CleaningPlanner}','{ClaimsKeys.ManagementClaimKeys.CleaningPlanner}' FROM public.asp_net_roles r WHERE r.name = '{inspectorRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.CleaningCalendar}','{ClaimsKeys.ManagementClaimKeys.CleaningCalendar}' FROM public.asp_net_roles r WHERE r.name = '{inspectorRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.ReservationCalendar}','{ClaimsKeys.ManagementClaimKeys.ReservationCalendar}' FROM public.asp_net_roles r WHERE r.name = '{inspectorRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.LostAndFound}','{ClaimsKeys.ManagementClaimKeys.LostAndFound}' FROM public.asp_net_roles r WHERE r.name = '{inspectorRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.OnGuard}','{ClaimsKeys.ManagementClaimKeys.OnGuard}' FROM public.asp_net_roles r WHERE r.name = '{inspectorRoleName}' ON CONFLICT DO NOTHING;

            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.SettingsClaimKeys.Rooms}','{ClaimsKeys.SettingsClaimKeys.Rooms}' FROM public.asp_net_roles r WHERE r.name = '{receptionistRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.SettingsClaimKeys.Assets}','{ClaimsKeys.SettingsClaimKeys.Assets}' FROM public.asp_net_roles r WHERE r.name = '{receptionistRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.SettingsClaimKeys.Users}','{ClaimsKeys.SettingsClaimKeys.Users}' FROM public.asp_net_roles r WHERE r.name = '{receptionistRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.SettingsClaimKeys.RoleManagement}','{ClaimsKeys.SettingsClaimKeys.RoleManagement}' FROM public.asp_net_roles r WHERE r.name = '{receptionistRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.SettingsClaimKeys.RoomCategories}','{ClaimsKeys.SettingsClaimKeys.RoomCategories}' FROM public.asp_net_roles r WHERE r.name = '{receptionistRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.SettingsClaimKeys.HotelSettings}','{ClaimsKeys.SettingsClaimKeys.HotelSettings}' FROM public.asp_net_roles r WHERE r.name = '{receptionistRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.SettingsClaimKeys.Colors}','{ClaimsKeys.SettingsClaimKeys.Colors}' FROM public.asp_net_roles r WHERE r.name = '{receptionistRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.RoomInsights}','{ClaimsKeys.ManagementClaimKeys.RoomInsights}' FROM public.asp_net_roles r WHERE r.name = '{receptionistRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.UserInsights}','{ClaimsKeys.ManagementClaimKeys.UserInsights}' FROM public.asp_net_roles r WHERE r.name = '{receptionistRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.Tasks}','{ClaimsKeys.ManagementClaimKeys.Tasks}' FROM public.asp_net_roles r WHERE r.name = '{receptionistRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.Reservations}','{ClaimsKeys.ManagementClaimKeys.Reservations}' FROM public.asp_net_roles r WHERE r.name = '{receptionistRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.CleaningPlanner}','{ClaimsKeys.ManagementClaimKeys.CleaningPlanner}' FROM public.asp_net_roles r WHERE r.name = '{receptionistRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.CleaningCalendar}','{ClaimsKeys.ManagementClaimKeys.CleaningCalendar}' FROM public.asp_net_roles r WHERE r.name = '{receptionistRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.ReservationCalendar}','{ClaimsKeys.ManagementClaimKeys.ReservationCalendar}' FROM public.asp_net_roles r WHERE r.name = '{receptionistRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.LostAndFound}','{ClaimsKeys.ManagementClaimKeys.LostAndFound}' FROM public.asp_net_roles r WHERE r.name = '{receptionistRoleName}' ON CONFLICT DO NOTHING;
            //    INSERT INTO public.asp_net_role_claims (role_id, claim_type, claim_value) SELECT r.id ,'{ClaimsKeys.ManagementClaimKeys.OnGuard}','{ClaimsKeys.ManagementClaimKeys.OnGuard}' FROM public.asp_net_roles r WHERE r.name = '{receptionistRoleName}' ON CONFLICT DO NOTHING;
            //";

            //migrationBuilder.Sql(updateRolesSql);
            //migrationBuilder.Sql(addRolesSql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<DateTime>(
            //    name: "modified_at",
            //    table: "cleanings",
            //    type: "timestamp without time zone",
            //    nullable: false,
            //    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
