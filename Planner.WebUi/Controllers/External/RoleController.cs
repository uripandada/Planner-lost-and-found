using Microsoft.AspNetCore.Mvc;
using Planner.Application.ExternalApi.Users.Queries.ExternalGetListOfUserRoles;
using Planner.Common.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers.External
{
	public class RoleController : BaseExternalApiController
	{
		[HttpGet]
		public async Task<ProcessResponseSimple<IEnumerable<ExternalUserRole>>> GetListOfRoles([FromQuery] ExternalGetListOfUserRolesQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
