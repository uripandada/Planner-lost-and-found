using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Application.UserManagement.Models;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.UserManagement.Commands.InsertSubGroup
{
    public class InsertSubGroupCommand : IRequest<ProcessResponse<SubGroupHierarchyData>>
    {
        public string Name { get; set; }
        public Guid GroupId { get; set; }
    }

    public class InsertSubGroupCommandHandler : IRequestHandler<InsertSubGroupCommand, ProcessResponse<SubGroupHierarchyData>>, IAmWebApplicationHandler
    {
        private readonly IDatabaseContext databaseContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public InsertSubGroupCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
        {
            this.databaseContext = databaseContext;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ProcessResponse<SubGroupHierarchyData>> Handle(InsertSubGroupCommand request, CancellationToken cancellationToken)
        {
            var newSubGroup = new Domain.Entities.UserSubGroup
            {
                CreatedAt = DateTime.Now,
                CreatedById = this.httpContextAccessor.UserId(),
                ModifiedAt = DateTime.Now,
                ModifiedById = this.httpContextAccessor.UserId(),
                UserGroupId = request.GroupId,
                Name = request.Name
            };

            this.databaseContext.UserSubGroups.Add(newSubGroup);

            await this.databaseContext.SaveChangesAsync(cancellationToken);

            return new ProcessResponse<SubGroupHierarchyData>
            {
                Data = SubGroupHierarchyData.Create(newSubGroup),
                IsSuccess = true,
                Message = "Subgroup added succesfully"
            };
        }
    }
}
