using Microsoft.AspNetCore.Mvc;
using Planner.Application.ExternalApi.UserGroups.Queries.ExternalGetListOfUserGroupsQuery;
using Planner.Common.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers.External
{
	public class UserGroupController : BaseExternalApiController
	{
		[HttpGet]
		public async Task<ProcessResponseSimple<IEnumerable<ExternalUserGroup>>> GetListOfUserGroups([FromQuery] ExternalGetListOfUserGroupsQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
