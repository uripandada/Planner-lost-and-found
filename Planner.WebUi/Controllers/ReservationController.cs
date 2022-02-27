using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Reservations.Commands.SynchronizeReservations;
using Planner.Application.Reservations.Queries.GetPageOfReservations;
using Planner.Common.Data;
using Planner.Domain.Values;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class ReservationController : BaseController
	{
		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.Reservations)]
		public async Task<ProcessResponse<SynchronizeReservationsResult>> SynchronizeReservationsFromRcc([FromBody] SynchronizeReservationsCommand query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpPost]
		public async Task<PageOf<ReservationGridData>> GetPageOfReservations([FromBody] GetPageOfReservationsQuery query)
		{
			return await this.Mediator.Send(query);
		}
	}
}
