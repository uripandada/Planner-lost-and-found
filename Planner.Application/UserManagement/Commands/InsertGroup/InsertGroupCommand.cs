using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Application.UserManagement.Models;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.UserManagement.Commands.InsertGroup
{
    public class InsertGroupCommand : IRequest<ProcessResponse<GroupHierarchyData>>
    {
        public string Name { get; set; }
    }

    public class InsertGroupCommandHandler : IRequestHandler<InsertGroupCommand, ProcessResponse<GroupHierarchyData>>, IAmWebApplicationHandler
    {
        private readonly IDatabaseContext databaseContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        public InsertGroupCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
        {
            this.databaseContext = databaseContext;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ProcessResponse<GroupHierarchyData>> Handle(InsertGroupCommand request, CancellationToken cancellationToken)
        {
            var newGroup = new Domain.Entities.UserGroup
            {
                CreatedAt = DateTime.Now,
                CreatedById = this.httpContextAccessor.UserId(),
                ModifiedAt = DateTime.Now,
                ModifiedById = this.httpContextAccessor.UserId(),
                Name = request.Name
            };

            this.databaseContext.UserGroups.Add(newGroup);

            await this.databaseContext.SaveChangesAsync(cancellationToken);

            return new ProcessResponse<GroupHierarchyData>
            {
                Data = GroupHierarchyData.Create(newGroup),
                IsSuccess = true,
                Message = "Group added succesfully"
            };


        }
    }
}
