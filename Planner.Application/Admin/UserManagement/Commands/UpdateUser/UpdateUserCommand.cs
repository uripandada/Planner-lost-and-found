using MediatR;
using Microsoft.AspNetCore.Identity;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.UserManagement.Commands.UpdateUser
{
	public class UpdateUserCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
		public string Email { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
	}
	public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ProcessResponse>, IAmAdminApplicationHandler
	{
		private readonly UserManager<MasterUser> userManager;

		public UpdateUserCommandHandler(UserManager<MasterUser> userManager)
		{
			this.userManager = userManager;
		}

		public async Task<ProcessResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
		{
			var user = await this.userManager.FindByIdAsync(request.Id.ToString());

			if(user == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to find the user to update."
				};
			}

			user.Email = request.Email;
			user.UserName = request.UserName;

			var createResult = await this.userManager.UpdateAsync(user);
			if (!createResult.Succeeded)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = string.Join(", ", createResult.Errors.Select(e => e.Description).ToArray())
				};
			}

			if (request.Password.IsNotNull())
			{
				var resetPasswordToken = await this.userManager.GeneratePasswordResetTokenAsync(user);
				var resetPasswordResult = await this.userManager.ResetPasswordAsync(user, resetPasswordToken, request.Password);

				if (!resetPasswordResult.Succeeded)
				{
					return new ProcessResponse
					{
						HasError = true,
						IsSuccess = false,
						Message = "User updated but the password change failed. " + string.Join(", ", resetPasswordResult.Errors.Select(e => e.Description).ToArray())
					};
				}
			}

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "User updated."
			};
		}
	}
}
