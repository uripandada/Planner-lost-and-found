using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Planner.Application.Interfaces;
using Planner.Application.UserManagement.Models;
using Planner.Common;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.UserManagement.Queries.GetUserByIdQuery
{
    public class GetUserByIdQuery : IRequest<ProcessResponse<UserModel>>
    {
        public Guid Id { get; set; }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ProcessResponse<UserModel>>, IAmWebApplicationHandler
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IDatabaseContext databaseContext;
        private readonly IFileService _fileService;

        public GetUserByIdQueryHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IDatabaseContext databaseContext, IFileService fileService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.databaseContext = databaseContext;
            this._fileService = fileService;
        }

        public async Task<ProcessResponse<UserModel>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await this.userManager.FindByIdAsync(request.Id.ToString());

            if (user != null)
            {
                var result = UserModel.Create(user);
                var roles = await this.userManager.GetRolesAsync(user);
                var roleName = roles.Single();
                var role = await roleManager.FindByNameAsync(roleName);

                result.RoleId = role.Id.ToString();
                if (role.Name == SystemDefaults.Roles.Administrator.Name)
                {
                    result.HotelIds = await this.databaseContext.Hotels.Select(x => x.Id).ToArrayAsync();
                }
                else
                {
                    var claims = await this.userManager.GetClaimsAsync(user);
                    result.HotelIds = claims.Where(x => x.Type == ClaimsKeys.HotelId).Select(x => x.Value).ToArray();
                }

                var avatarUrl = await this.databaseContext.ApplicationUserAvatar.Where(ua => ua.Id == request.Id).Select(ua => ua.FileUrl).FirstOrDefaultAsync();
                result.AvatarImageUrl = avatarUrl;

                return new ProcessResponse<UserModel>
                {
                    Data = result,
                    IsSuccess = true
                };
            }

            return new ProcessResponse<UserModel>
            {
                IsSuccess = false,
                Message = "User not found!"
            };
        }
    }
}
