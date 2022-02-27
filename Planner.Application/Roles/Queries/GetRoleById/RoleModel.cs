using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Planner.Application.Roles.Queries.GetRoleById
{
    public class RoleModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsSystemRole { get; set; }
        public string HotelAccessTypeKey { get; set; }
        public string HotelAccessTypeDescription { get; set; }

        #region Settings Claims
        public bool RoomsClaim { get; set; }
        public bool AssetsClaim { get; set; }
        public bool UsersClaim { get; set; }
        public bool RoleManagementClaim { get; set; }
        public bool RoomCategoriesClaim { get; set; }
        public bool HotelSettingClaim { get; set; }
        #endregion


        #region Management Claims
        public bool RoomInsightsClaim { get; set; }
        public bool UserInsightsClaim { get; set; }
        public bool TasksClaim { get; set; }
        public bool ReservationClaim { get; set; }
        public bool CleaningPlannerClaim { get; set; }
        public bool CleaningCalendarClaim { get; set; }
        public bool ReservationCalendarClaim { get; set; }
        public bool LostAndFoundClaim { get; set; }
        public bool OnGuardClaim { get; set; }

        #endregion

        public static string GetHotelAccessTypeDescription(string hotelAccessTypeKey)
        {
            switch (hotelAccessTypeKey)
            {
                case "ALL":
                    return "User can access all hotels.";
                case "MULTIPLE":
                    return "User can access specified hotels.";
                case "SINGLE":
                    return "User can access a single hotel.";
            }

            return "Unknown";
        }

        public static Expression<Func<Domain.Entities.Role, IEnumerable<Claim>, RoleModel>> Projection
        {
            get
            {
                return (role, claims) => new RoleModel
                {
                    Id = role.Id,
                    IsSystemRole = role.IsSystemRole,
                    Name = role.Name,
                    HotelAccessTypeKey = role.HotelAccessTypeKey,
                    HotelAccessTypeDescription = GetHotelAccessTypeDescription(role.HotelAccessTypeKey),

                    RoomsClaim = claims.Any(x => x.Value == ClaimsKeys.SettingsClaimKeys.Rooms),
                    AssetsClaim = claims.Any(x=>x.Value == ClaimsKeys.SettingsClaimKeys.Assets),
                    UsersClaim = claims.Any(x => x.Value == ClaimsKeys.SettingsClaimKeys.Users),
                    RoleManagementClaim = claims.Any(x => x.Value == ClaimsKeys.SettingsClaimKeys.RoleManagement),
                    RoomCategoriesClaim = claims.Any(x => x.Value == ClaimsKeys.SettingsClaimKeys.RoomCategories),
                    HotelSettingClaim = claims.Any(x => x.Value == ClaimsKeys.SettingsClaimKeys.HotelSettings),

                    RoomInsightsClaim = claims.Any(x => x.Value == ClaimsKeys.ManagementClaimKeys.RoomInsights),
                    UserInsightsClaim = claims.Any(x => x.Value == ClaimsKeys.ManagementClaimKeys.UserInsights),
                    TasksClaim = claims.Any(x => x.Value == ClaimsKeys.ManagementClaimKeys.Tasks),
                    ReservationClaim = claims.Any(x => x.Value == ClaimsKeys.ManagementClaimKeys.Reservations),
                    CleaningPlannerClaim = claims.Any(x => x.Value == ClaimsKeys.ManagementClaimKeys.CleaningPlanner),
                    CleaningCalendarClaim = claims.Any(x => x.Value == ClaimsKeys.ManagementClaimKeys.CleaningCalendar),
                    ReservationCalendarClaim = claims.Any(x => x.Value == ClaimsKeys.ManagementClaimKeys.ReservationCalendar),
                    LostAndFoundClaim = claims.Any(x => x.Value == ClaimsKeys.ManagementClaimKeys.LostAndFound),
                    OnGuardClaim = claims.Any(x => x.Value == ClaimsKeys.ManagementClaimKeys.OnGuard),
                };
            }
        }

        public static RoleModel Create(Domain.Entities.Role role, IEnumerable<Claim> claims)
        {
            return Projection.Compile().Invoke(role, claims);

        }
    }
}
