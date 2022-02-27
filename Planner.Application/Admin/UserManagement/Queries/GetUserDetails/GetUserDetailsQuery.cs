using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.UserManagement.Queries.GetUserDetails
{
	public class UserDetailsData
	{
		public Guid Id { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
	}

	public class GetUserDetailsQuery : IRequest<UserDetailsData>
	{
		public Guid Id { get; set; }
	}
	public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery, UserDetailsData>, IAmAdminApplicationHandler
	{
		private readonly IMasterDatabaseContext databaseContext;

		public GetUserDetailsQueryHandler(IMasterDatabaseContext databaseContext)
		{
			this.databaseContext = databaseContext;
		}

		public async Task<UserDetailsData> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
		{
			return await this.databaseContext
				.Users
				.Where(u => u.Id == request.Id)
				.Select(u => new UserDetailsData
				{
					Id = u.Id,
					Email = u.Email,
					Username = u.UserName
				})
				.FirstOrDefaultAsync();
		}
	}
}
