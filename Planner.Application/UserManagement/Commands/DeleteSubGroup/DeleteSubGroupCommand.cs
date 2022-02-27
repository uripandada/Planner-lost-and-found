using Google.Maps.Places;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.UserManagement.Commands.DeleteSubGroup
{
    public class DeleteSubGroupCommand : IRequest<ProcessResponse>
    {
        public Guid Id { get; set; }
    }

    public class DeleteSubGroupCommandHandler : IRequestHandler<DeleteSubGroupCommand, ProcessResponse>, IAmWebApplicationHandler
    {
        private readonly IDatabaseContext databaseContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        
        public DeleteSubGroupCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
        {
            this.databaseContext = databaseContext;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ProcessResponse> Handle(DeleteSubGroupCommand request, CancellationToken cancellationToken)
        {
            var subGroupToDelete = await this.databaseContext.UserSubGroups.SingleAsync(e => e.Id == request.Id);

            this.databaseContext.UserSubGroups.Remove(subGroupToDelete);
            await this.databaseContext.SaveChangesAsync(cancellationToken);

            return new ProcessResponse
            {
                IsSuccess = true,
                Message = $"Subgroup {subGroupToDelete.Name} deleted"
            };
        }
    }
}
