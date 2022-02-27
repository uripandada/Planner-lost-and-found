using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Rcc.Hotels.Queries.GetListOfHotelGroupHotels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers.Rcc
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class HotelController : BaseRccApiController
	{
		[HttpPost]
		public async Task<IEnumerable<RccHotel>> GetListOfHotelGroupHotels([FromBody] GetListOfHotelGroupHotelsQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
