using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Planner.Application.MobileApi.Reservations.Queries.GetListOfReservationsForMobile;

namespace Planner.WebUi.Controllers.AttendantMobile
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class ReservationController : BaseMobileApiController
	{
		[HttpPost]
		public async Task<IEnumerable<MobileReservation>> GetListOfReservations([FromBody] GetListOfReservationsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
