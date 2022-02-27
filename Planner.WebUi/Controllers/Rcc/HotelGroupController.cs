using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Rcc.HotelGroups.Queries.GetListOfHotelGroups;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers.Rcc
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class HotelGroupController : BaseRccApiController
	{
		[HttpPost]
		public async Task<IEnumerable<RccHotelGroup>> GetListOfHotelGroups([FromBody] GetListOfHotelGroupsQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
