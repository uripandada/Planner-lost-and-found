using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.UserManagement.Models;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.UserManagement.Queries.GetGroupsAndSubGroups
{
    public class GetGroupsAndSubGroupsQuery : IRequest<ProcessResponse<FullGroupHierarchyData>>
    {

    }

    public class GetGroupsAndSubGroupsQueryHandler : IRequestHandler<GetGroupsAndSubGroupsQuery, ProcessResponse<FullGroupHierarchyData>>, IAmWebApplicationHandler
    {
        private readonly IDatabaseContext databaseContext;

        public GetGroupsAndSubGroupsQueryHandler(IDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<ProcessResponse<FullGroupHierarchyData>> Handle(GetGroupsAndSubGroupsQuery request, CancellationToken cancellationToken)
        {
            var data = await this.databaseContext.UserGroups
                                 .Include(x => x.UserSubGroups)
                                 .OrderBy(x => x.Name)
                                 .ToListAsync();

            var result = new FullGroupHierarchyData();
            result.Groups = GroupHierarchyData.CreateLists(data);

            return new ProcessResponse<FullGroupHierarchyData>
            {
                Data = result,
                IsSuccess = true
            };
        }
    }
}
