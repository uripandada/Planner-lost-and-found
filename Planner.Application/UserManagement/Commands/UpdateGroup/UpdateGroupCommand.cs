using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.UserManagement.Models;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.UserManagement.Commands.UpdateGroup
{
	public class UpdateGroupCommand : IRequest<ProcessResponse<GroupHierarchyData>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, ProcessResponse<GroupHierarchyData>>, IAmWebApplicationHandler
    {
        private readonly IDatabaseContext databaseContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UpdateGroupCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
        {
            this.databaseContext = databaseContext;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ProcessResponse<GroupHierarchyData>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            var groupToUpdate = await this.databaseContext.UserGroups.SingleAsync(e => e.Id == request.Id);

            groupToUpdate.Name = request.Name;
            groupToUpdate.ModifiedAt = DateTime.UtcNow;
            groupToUpdate.ModifiedById = this.httpContextAccessor.UserId();

            await this.databaseContext.SaveChangesAsync(cancellationToken);

            return new ProcessResponse<GroupHierarchyData>
            {
                Data = GroupHierarchyData.Create(groupToUpdate),
                IsSuccess = true,
                Message = $"Group {groupToUpdate.Name} updated"
            };
        }
    }
}
