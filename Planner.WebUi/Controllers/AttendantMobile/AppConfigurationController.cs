using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Planner.Application.MobileApi.AppConfiguration.Queries.GetAttendantMobileAppConfiguration;
using OpenIddict.Validation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Planner.Application.MobileApi.AppConfiguration.Queries.GetAttendantMobileAppConfigurationForHotel;

namespace Planner.WebUi.Controllers.AttendantMobile
{
	public class AppConfigurationController : BaseMobileApiController
	{
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpPost]
		public async Task<AttendantMobileAppConfiguration> GetAttendantMobileAppConfiguration([FromBody] GetAttendantMobileAppConfigurationQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpPost]
		public async Task<AttendantMobileAppConfiguration> GetAttendantMobileAppConfigurationForHotel([FromBody] GetAttendantMobileAppConfigurationForHotelQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
