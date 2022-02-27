using MediatR;
using Microsoft.AspNetCore.Identity;
using Planner.Common.Data;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Roles.Commands.Delete
{
    public class DeleteRoleCommand : IRequest<ProcessResponse>
    {
        public string RoleId { get; set; }
    }

    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, ProcessResponse>, IAmWebApplicationHandler
    {
        private readonly RoleManager<Role> roleManager;

        public DeleteRoleCommandHandler(RoleManager<Role> roleManager)
        {
            this.roleManager = roleManager;
        }

        public async Task<ProcessResponse> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var roleToDelete = await this.roleManager.FindByIdAsync(request.RoleId);
            if (roleToDelete.IsSystemRole)
            {
                return new ProcessResponse
                {
                    IsSuccess = false,
                    Message = "Deleting system roles is not allowed"
                };
            }

            var result = await this.roleManager.DeleteAsync(roleToDelete);
            if (result.Succeeded)
            {
                return new ProcessResponse
                {
                    IsSuccess = true,
                    Message = $"Role {roleToDelete.Name} deleted"
                };
            }

            return new ProcessResponse
            {
                IsSuccess = false,
                HasError = true,
                Message = result.Errors.First().Description
            };
        }
    }
}
