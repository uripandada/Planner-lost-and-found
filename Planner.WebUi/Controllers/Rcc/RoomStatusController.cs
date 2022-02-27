using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Rcc.RoomStatuses;
using Planner.Application.Rcc.RoomStatuses.Queries.GetHotelGroupRoomStatuses;
using Planner.Application.Rcc.RoomStatuses.Queries.GetHotelRoomStatuses;
using Planner.Application.Rcc.RoomStatuses.Queries.SendDifferentialRoomStatusChanges;
using Planner.Application.Rcc.RoomStatuses.Queries.SendFullRoomStatusChanges;
using Planner.Common.Data;
using System;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers.Rcc
{

	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class RoomStatusController : BaseRccApiController
	{
		[HttpPost]
		public async Task<RccHotelGroupRoomStatusChanges> GetHotelGroupRoomStatuses([FromBody] GetHotelGroupRoomStatusesQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<RccHotelRoomStatusChanges> GetHotelRoomStatuses([FromBody] GetHotelRoomStatusesQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponse> SendDifferentialRoomStatusChanges([FromBody] SendDifferentialRoomStatusChangesQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponse> SendFullRoomStatusChangesQuery([FromBody] SendFullRoomStatusChangesQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
