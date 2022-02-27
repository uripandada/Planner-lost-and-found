using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Planner.WebUi.Security
{
    public sealed class ProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<User> _userClaimsPrincipalFactory;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;

        public ProfileService(IUserClaimsPrincipalFactory<User> userClaimsPrincipalFactory, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            string sub = context.Subject.GetSubjectId();
            User user = await userManager.FindByIdAsync(sub);
            ClaimsPrincipal userClaims = await _userClaimsPrincipalFactory.CreateAsync(user);

            List<Claim> claims = userClaims.Claims.ToList();
            claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();

            if (userManager.SupportsUserRole)
            {
                IList<string> roles = await userManager.GetRolesAsync(user);
                foreach (var roleName in roles)
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, roleName));
                    if (roleManager.SupportsRoleClaims)
                    {
                        Role role = await roleManager.FindByNameAsync(roleName);
                        if (role != null)
                        {
                            claims.AddRange(await roleManager.GetClaimsAsync(role));
                        }
                    }
                }
            }

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            string sub = context.Subject.GetSubjectId();
            User user = await userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
