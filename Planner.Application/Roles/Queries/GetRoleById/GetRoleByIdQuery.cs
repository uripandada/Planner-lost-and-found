using MediatR;
using Microsoft.AspNetCore.Identity;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Domain.Entities;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Roles.Queries.GetRoleById
{
    public class GetRoleByIdQuery : IRequest<ProcessResponse<RoleModel>>
    {
        public string RoleId { get; set; }

    }

    public class GetRoleByIdQuerrHandler : IRequestHandler<GetRoleByIdQuery, ProcessResponse<RoleModel>>, IAmWebApplicationHandler
    {
        private readonly RoleManager<Role> roleManager;

        public GetRoleByIdQuerrHandler(RoleManager<Role> roleManager)
        {
            this.roleManager = roleManager;
        }

        public async Task<ProcessResponse<RoleModel>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await roleManager.FindByIdAsync(request.RoleId);
            var roleClaims = await roleManager.GetClaimsAsync(role);
            
            var result = RoleModel.Create(role, roleClaims);

            return new ProcessResponse<RoleModel>
            {
                Data = result,
                IsSuccess = true
            };
        }
    }
}
