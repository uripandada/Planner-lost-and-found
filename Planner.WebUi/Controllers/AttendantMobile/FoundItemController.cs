using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Planner.Application.MobileApi.LostsAndFounds.Queries.GetListOfFoundsForMobile;
using Planner.Application.MobileApi.LostsAndFounds.Commands.InsertFoundForMobile;
using Planner.Application.MobileApi.LostsAndFounds.Commands.UpdateFoundForMobile;
using Planner.Application.MobileApi.LostsAndFounds.Commands.UpdateFoundImageForMobile;
using Planner.Common.Data;
using Planner.Application.MobileApi.LostsAndFounds.Commands.DeleteFoundForMobile;

namespace Planner.WebUi.Controllers.AttendantMobile
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class FoundItemController : BaseMobileApiController
	{
		[HttpPost]
		public async Task<IEnumerable<MobileFoundItem>> GetListOfFoundItems([FromBody] GetListOfFoundsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<MobileFoundItem> InsertFoundItem([FromBody] InsertFoundForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<MobileFoundItem> UpdateFoundItem([FromBody] UpdateFoundForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<SimpleProcessResponse> UpdateFoundItemImage([FromBody] UpdateFoundImageForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<SimpleProcessResponse> DeleteFoundItem([FromBody] DeleteFoundForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
