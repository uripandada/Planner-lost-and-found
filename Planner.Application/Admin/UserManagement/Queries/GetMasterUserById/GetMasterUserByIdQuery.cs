using MediatR;
using Microsoft.AspNetCore.Identity;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Admin.UserManagement.Models;
using Planner.Common.Data;
using Planner.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.UserManagement.Queries.GetMasterUserById
{
	public class GetMasterUserByIdQuery : IRequest<ProcessResponse<MasterUserModel>>
    {
        public Guid Id { get; set; }
    }

    public class GetMasterUserByIdQueryHandler : IRequestHandler<GetMasterUserByIdQuery, ProcessResponse<MasterUserModel>>, IAmAdminApplicationHandler
    {
        private readonly UserManager<MasterUser> userManager;
        private readonly IMasterDatabaseContext databaseContext;

        public GetMasterUserByIdQueryHandler(UserManager<MasterUser> userManager, IMasterDatabaseContext databaseContext)
        {
            this.userManager = userManager;
            this.databaseContext = databaseContext;
        }

        public async Task<ProcessResponse<MasterUserModel>> Handle(GetMasterUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await this.userManager.FindByIdAsync(request.Id.ToString());

            if (user != null)
            {
                var result = new MasterUserModel
                {
                    Email = user.Email,
                    Id = user.Id,
                    UserName = user.UserName
                };

                return new ProcessResponse<MasterUserModel>
                {
                    Data = result,
                    IsSuccess = true
                };
            }

            return new ProcessResponse<MasterUserModel>
            {
                IsSuccess = false,
                Message = "User not found!"
            };
        }
    }
}
