using Microsoft.AspNetCore.Mvc;
using Planner.Application.ExternalApi.Users.Commands.ExternalInsertUser;
using Planner.Application.ExternalApi.Users.Queries.ExternalGetListOfUsersQuery;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers.External
{
	public class UserController : BaseExternalApiController
	{
		[HttpGet]
		public async Task<ProcessResponseSimple<IEnumerable<ExternalUser>>> GetListOfUsers([FromQuery] ExternalGetListOfUsersQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponseSimple<Guid>> InsertUser([FromBody] ExternalInsertUserCommand request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
