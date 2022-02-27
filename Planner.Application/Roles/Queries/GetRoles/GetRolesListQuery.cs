using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.Roles.Queries.GetRoleById;
using Planner.Common.Data;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Roles.Queries.GetRoles
{
    public class RoleListModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsSystemRole { get; set; }
        public string HotelAccessTypeKey { get; set; }
        public string HotelAccessTypeDescription { get; set; }
    }

    public class GetRolesListQuery : IRequest<ProcessResponse<IEnumerable<RoleListModel>>>
    {
    }

    public class GetRolesListQueryHandler : IRequestHandler<GetRolesListQuery, ProcessResponse<IEnumerable<RoleListModel>>>, IAmWebApplicationHandler
    {
        public RoleManager<Domain.Entities.Role> roleManager;

        public GetRolesListQueryHandler(RoleManager<Role> roleManager)
        {
            this.roleManager = roleManager;
        }

        public async Task<ProcessResponse<IEnumerable<RoleListModel>>> Handle(GetRolesListQuery request, CancellationToken cancellationToken)
        {
            var roles = await roleManager.Roles.ToListAsync();

            return new ProcessResponse<IEnumerable<RoleListModel>>
            {
                Data = roles.OrderBy(x => x.Name).Select(x => new RoleListModel
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    IsSystemRole = x.IsSystemRole,
                    HotelAccessTypeKey = x.HotelAccessTypeKey,
                    HotelAccessTypeDescription = RoleModel.GetHotelAccessTypeDescription(x.HotelAccessTypeKey)
                }).ToList(),
                IsSuccess = true
            };
        }

    }
}
