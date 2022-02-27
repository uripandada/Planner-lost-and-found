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

namespace Planner.Application.Admin.UserManagement.Commands.InsertUser
{
	public class InsertUserCommand : IRequest<ProcessResponse<Guid>>
	{
		public string Email { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
	}

	public class InsertUserCommandHandler : IRequestHandler<InsertUserCommand, ProcessResponse<Guid>>, IAmAdminApplicationHandler
	{
		private readonly UserManager<MasterUser> userManager;

		public InsertUserCommandHandler(UserManager<MasterUser> userManager)
		{
			this.userManager = userManager;
		}

		public async Task<ProcessResponse<Guid>> Handle(InsertUserCommand request, CancellationToken cancellationToken)
		{
			var newUser = new MasterUser
			{
				Email = request.Email,
				UserName = request.UserName,
			};

			var createResult = await this.userManager.CreateAsync(newUser, request.Password);
			if (createResult.Succeeded)
			{
				var user = await this.userManager.FindByEmailAsync(newUser.Email);
				return new ProcessResponse<Guid>
				{
					Data = user.Id,
					HasError = false,
					IsSuccess = true,
					Message = "User created"
				};
			}
			else
			{
				return new ProcessResponse<Guid>
				{
					Data = Guid.Empty,
					HasError = true,
					IsSuccess = false,
					Message = string.Join(", ", createResult.Errors.Select(e => e.Description).ToArray())
				};
			}
		}
	}
}
