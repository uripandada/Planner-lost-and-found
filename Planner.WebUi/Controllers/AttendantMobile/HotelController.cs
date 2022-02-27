using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Planner.Application.MobileApi.Hotels.Queries.GetHotelDetailsForMobile;
using Planner.Application.MobileApi.Hotels.Queries.GetListOfHotelsForMobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Planner.WebUi.Controllers.AttendantMobile
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class HotelController : BaseMobileApiController
	{
		[HttpPost]
		public async Task<MobileHotelDetails> GetHotelDetails([FromBody] GetHotelDetailsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<MobileHotel>> GetListOfHotels([FromBody] GetListOfHotelsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
