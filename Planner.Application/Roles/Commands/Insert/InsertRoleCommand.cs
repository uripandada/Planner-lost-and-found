using MediatR;
using Microsoft.AspNetCore.Identity;
using Planner.Common.Data;
using Planner.Domain.Entities;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Roles.Commands.Insert
{
    public class InsertRoleCommand : IRequest<ProcessResponse<Guid>>
    {
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

    public class InsertRoleCommandHandler : IRequestHandler<InsertRoleCommand, ProcessResponse<Guid>>, IAmWebApplicationHandler
    {
        private readonly RoleManager<Role> roleManager;

        public InsertRoleCommandHandler(RoleManager<Role> roleManager)
        {
            this.roleManager = roleManager;
        }

        public async Task<ProcessResponse<Guid>> Handle(InsertRoleCommand request, CancellationToken cancellationToken)
        {
            var newRole = new Role();
            newRole.Id = Guid.NewGuid();
            newRole.Name = request.Name;
            newRole.NormalizedName = request.Name.ToUpper();
            newRole.IsSystemRole = false;
            newRole.HotelAccessTypeKey = request.HotelAccessTypeKey;

            var result = await roleManager.CreateAsync(newRole);

            #region Settings Claims
            await AddClaim(request.RoomsClaim, ClaimsKeys.SettingsClaimKeys.Rooms, newRole);
            await AddClaim(request.AssetsClaim, ClaimsKeys.SettingsClaimKeys.Assets, newRole);
            await AddClaim(request.UsersClaim, ClaimsKeys.SettingsClaimKeys.Users, newRole);
            await AddClaim(request.RoleManagementClaim, ClaimsKeys.SettingsClaimKeys.RoleManagement, newRole);
            await AddClaim(request.RoomCategoriesClaim, ClaimsKeys.SettingsClaimKeys.RoomCategories, newRole);
            await AddClaim(request.HotelSettingClaim, ClaimsKeys.SettingsClaimKeys.HotelSettings, newRole);
            #endregion

            #region Management Claims
            await AddClaim(request.RoomInsightsClaim, ClaimsKeys.ManagementClaimKeys.RoomInsights, newRole);
            await AddClaim(request.UserInsightsClaim, ClaimsKeys.ManagementClaimKeys.UserInsights, newRole);
            await AddClaim(request.TasksClaim, ClaimsKeys.ManagementClaimKeys.Tasks, newRole);
            await AddClaim(request.ReservationClaim, ClaimsKeys.ManagementClaimKeys.Reservations, newRole);
            await AddClaim(request.CleaningPlannerClaim, ClaimsKeys.ManagementClaimKeys.CleaningPlanner, newRole);
            await AddClaim(request.CleaningCalendarClaim, ClaimsKeys.ManagementClaimKeys.CleaningCalendar, newRole);
            await AddClaim(request.ReservationCalendarClaim, ClaimsKeys.ManagementClaimKeys.ReservationCalendar, newRole);
            await AddClaim(request.LostAndFoundClaim, ClaimsKeys.ManagementClaimKeys.LostAndFound, newRole);
            await AddClaim(request.OnGuardClaim, ClaimsKeys.ManagementClaimKeys.OnGuard, newRole);
            #endregion

            if (result.Succeeded)
            {
                return new ProcessResponse<Guid>
                {
                    Data = newRole.Id,
                    IsSuccess = true,
                    Message = $"Role {request.Name} created"
                };
            }
            return new ProcessResponse<Guid>
            {
                IsSuccess = false,
                HasError = true,
                Message = result.Errors.First().Description
            };
        }

        private async Task AddClaim(bool requestClaimValue, string claimValue, Role newRole)
        {
            if (requestClaimValue)
            {
                await this.roleManager.AddClaimAsync(newRole, new System.Security.Claims.Claim(claimValue, claimValue, "string", ClaimsKeys.ISSUER));
            }
        }
    }
}
