using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.UserManagement.Commands.DeleteGroup
{
    public class DeleteGroupCommand : IRequest<ProcessResponse>
    {
        public Guid Id { get; set; }
    }

    public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, ProcessResponse>, IAmWebApplicationHandler
    {
        private readonly IDatabaseContext databaseContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public DeleteGroupCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
        {
            this.databaseContext = databaseContext;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ProcessResponse> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var groupToDelete = await this.databaseContext.UserGroups.SingleAsync(e => e.Id == request.Id);

            this.databaseContext.UserGroups.Remove(groupToDelete);
            await this.databaseContext.SaveChangesAsync(cancellationToken);

            return new ProcessResponse
            {
                IsSuccess = true,
                Message = $"Group {groupToDelete.Name} deleted"
            };
        }
    }
}
