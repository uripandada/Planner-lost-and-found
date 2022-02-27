using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Planner.Application.MobileApi.Floors.Queries.GetFloorDetailsForMobile;
using Planner.Application.MobileApi.Floors.Queries.GetListOfFloorsForMobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Planner.WebUi.Controllers.AttendantMobile
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class FloorController : BaseMobileApiController
	{
		[HttpPost]
		public async Task<MobileFloorDetails> GetFloorDetails([FromBody] GetFloorDetailsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<MobileFloor>> GetListOfFloors([FromBody] GetListOfFloorsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}

}
