using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Planner.Application.MobileApi.RoomCategories.Queries.GetListOfRoomCategoriesForMobile;
using Planner.Application.MobileApi.RoomCategories.Queries.GetRoomCategoryDetailsForMobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Planner.WebUi.Controllers.AttendantMobile
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class RoomCategoryController : BaseMobileApiController
	{
		[HttpPost]
		public async Task<MobileRoomCategoryDetails> GetRoomCategoryDetails([FromBody] GetRoomCategoryDetailsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<MobileRoomCategory>> GetListOfRoomCategories([FromBody] GetListOfRoomCategoriesForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}

}
