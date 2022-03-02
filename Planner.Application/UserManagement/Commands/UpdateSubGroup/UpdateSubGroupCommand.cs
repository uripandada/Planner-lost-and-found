using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.UserManagement.Models;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.UserManagement.Commands.UpdateSubGroup
{
    public class UpdateSubGroupCommand : IRequest<ProcessResponse<SubGroupHierarchyData>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid GroupId { get; set; }

    }

    public class UpdateSubGroupCommandHandler : IRequestHandler<UpdateSubGroupCommand, ProcessResponse<SubGroupHierarchyData>>, IAmWebApplicationHandler
    {
        private readonly IDatabaseContext databaseContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UpdateSubGroupCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
        {
            this.databaseContext = databaseContext;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ProcessResponse<SubGroupHierarchyData>> Handle(UpdateSubGroupCommand request, CancellationToken cancellationToken)
        {
            var subGroupToUpdate = await this.databaseContext.UserSubGroups.SingleAsync(e => e.Id == request.Id);

            subGroupToUpdate.Name = request.Name;
            subGroupToUpdate.UserGroupId = request.GroupId;
            subGroupToUpdate.ModifiedAt = DateTime.UtcNow;
            subGroupToUpdate.ModifiedById = this.httpContextAccessor.UserId();

            await this.databaseContext.SaveChangesAsync(cancellationToken);

            return new ProcessResponse<SubGroupHierarchyData>
            {
                Data = SubGroupHierarchyData.Create(subGroupToUpdate),
                IsSuccess = true,
                Message = $"Subgroup {subGroupToUpdate.Name} updated"
            };
        }
    }
}
