using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Users.Queries.GetUserDetailsForMobile
{
	public class UserDetailsForMobile
	{
		public Guid Id { get; set; }
		public string Username { get; set; }
		public string Name { get; set; }
		public string Language { get; set; }
	}

	public class GetMyUserDetailsForMobileQuery
	{

	}

	public class GetUserDetailsForMobileQuery: IRequest<UserDetailsForMobile>
	{
		public Guid Id { get; set; }
	}

	public class GetUserDetailsForMobileQueryHandler : IRequestHandler<GetUserDetailsForMobileQuery, UserDetailsForMobile>, IAmWebApplicationHandler
	{

		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetUserDetailsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<UserDetailsForMobile> Handle(GetUserDetailsForMobileQuery request, CancellationToken cancellationToken)
		{
			var user = await this._databaseContext.Users.FindAsync(request.Id);

			if(user == null)
			{
				return null;
			}

			return new UserDetailsForMobile
			{
				Id = user.Id,
				Name = $"{user.FirstName} {user.LastName}",
				Username = user.UserName,
				Language = user.Language.IsNotNull() ? user.Language : "en",
			};
		}
	}
}
