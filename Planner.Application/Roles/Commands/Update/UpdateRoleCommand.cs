using MediatR;
using Microsoft.AspNetCore.Identity;
using Planner.Common.Data;
using Planner.Domain.Entities;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Roles.Commands.Update
{
    public class UpdateRoleCommand : IRequest<ProcessResponse>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string HotelAccessTypeKey { get; set; }

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
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ProcessResponse>, IAmWebApplicationHandler
    {
        private readonly RoleManager<Role> roleManager;

        public UpdateRoleCommandHandler(RoleManager<Role> roleManager)
        {
            this.roleManager = roleManager;
        }

        public async Task<ProcessResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await this.roleManager.FindByIdAsync(request.Id);

            if (!role.IsSystemRole)
            {
                role.Name = request.Name;
                role.NormalizedName = request.Name.ToUpper();
                role.HotelAccessTypeKey = request.HotelAccessTypeKey;
            }

            var result = await this.roleManager.UpdateAsync(role);

            var claims = await this.roleManager.GetClaimsAsync(role);

            #region Settings Claims 
            await UpdateClaims(request.RoomsClaim, ClaimsKeys.SettingsClaimKeys.Rooms, role, claims);
            await UpdateClaims(request.AssetsClaim, ClaimsKeys.SettingsClaimKeys.Assets, role, claims);
            await UpdateClaims(request.UsersClaim, ClaimsKeys.SettingsClaimKeys.Users, role, claims);
            await UpdateClaims(request.RoleManagementClaim, ClaimsKeys.SettingsClaimKeys.RoleManagement, role, claims);
            await UpdateClaims(request.RoomCategoriesClaim, ClaimsKeys.SettingsClaimKeys.RoomCategories, role, claims);
            await UpdateClaims(request.HotelSettingClaim, ClaimsKeys.SettingsClaimKeys.HotelSettings, role, claims);
            #endregion

            #region Management Claims
            await UpdateClaims(request.RoomInsightsClaim, ClaimsKeys.ManagementClaimKeys.RoomInsights, role, claims);
            await UpdateClaims(request.UserInsightsClaim, ClaimsKeys.ManagementClaimKeys.UserInsights, role, claims);
            await UpdateClaims(request.TasksClaim, ClaimsKeys.ManagementClaimKeys.Tasks, role, claims);
            await UpdateClaims(request.ReservationClaim, ClaimsKeys.ManagementClaimKeys.Reservations, role, claims);
            await UpdateClaims(request.CleaningPlannerClaim, ClaimsKeys.ManagementClaimKeys.CleaningPlanner, role, claims);
            await UpdateClaims(request.CleaningCalendarClaim, ClaimsKeys.ManagementClaimKeys.CleaningCalendar, role, claims);
            await UpdateClaims(request.ReservationCalendarClaim, ClaimsKeys.ManagementClaimKeys.ReservationCalendar, role, claims);
            await UpdateClaims(request.LostAndFoundClaim, ClaimsKeys.ManagementClaimKeys.LostAndFound, role, claims);
            await UpdateClaims(request.OnGuardClaim, ClaimsKeys.ManagementClaimKeys.OnGuard, role, claims);
            #endregion

            if (result.Succeeded)
            {
                return new ProcessResponse
                {
                    IsSuccess = true,
                    Message = $"Role {role.Name} updated"
                };
            }

            return new ProcessResponse
            {
                IsSuccess = false,
                HasError = true,
                Message = result.Errors.First().Description
            };

        }

        private async Task UpdateClaims(bool requestClaimValue, string claimKey,  Role role, IList<Claim> claims)
        {
            if (claims.Any(x => x.Value == claimKey) && !requestClaimValue)
            {
                await this.roleManager.RemoveClaimAsync(role, claims.Single(x => x.Value == claimKey));
            }
            else if (!claims.Any(x => x.Value == claimKey) && requestClaimValue)
            {
                await this.roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(claimKey, claimKey, "string", ClaimsKeys.ISSUER));
            }
        }

    }
}
