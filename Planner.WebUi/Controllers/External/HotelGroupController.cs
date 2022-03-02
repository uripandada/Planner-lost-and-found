using Microsoft.AspNetCore.Mvc;
using Planner.Application.ExternalApi.HotelGroups.Queries.ExternalGetListOfHotelGroups;
using Planner.Common.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers.External
{
	public class HotelGroupController : BaseExternalApiController
	{
		[HttpGet]
		public async Task<ProcessResponseSimple<IEnumerable<ExternalHotelGroup>>> GetListOfHotelGroups([FromQuery] ExternalGetListOfHotelGroupsQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
