using Microsoft.AspNetCore.Mvc;
using Planner.Application.ExternalApi.Hotels.Queries.ExternalGetListOfHotels;
using Planner.Common.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers.External
{
	public class HotelController : BaseExternalApiController
	{
		[HttpGet]
		public async Task<ProcessResponseSimple<IEnumerable<ExternalHotel>>> GetListOfHotels([FromQuery] ExternalGetListOfHotelsQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
