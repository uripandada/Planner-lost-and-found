using MediatR;
using Microsoft.AspNetCore.Identity;
using Planner.Common.Data;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.UserManagement.Commands.DeactivateUser
{
	public class DeactivateUserCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
	}
	public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, ProcessResponse>, IAmAdminApplicationHandler
	{
		private readonly UserManager<MasterUser> userManager;

		public DeactivateUserCommandHandler(UserManager<MasterUser> userManager)
		{
			this.userManager = userManager;
		}

		public async Task<ProcessResponse> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
		{
			var user = await this.userManager.FindByIdAsync(request.Id.ToString());

			if (user == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to find the user to update."
				};
			}

			throw new NotImplementedException();

			//var result = await this.userManager.

			//return new ProcessResponse
			//{
			//	HasError = false,
			//	IsSuccess = true,
			//	Message = "User deactivated."
			//};
		}
	}
}
