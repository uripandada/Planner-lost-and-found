using Microsoft.AspNetCore.Mvc;
using Planner.Application.Dashboard.Queries.GetRoomViewDashboard;
using Planner.Application.Dashboard.Queries.GetRoomViewDashboardFilterValues;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class DashboardController : BaseController
	{
		[HttpPost]
		public async Task<RoomViewDashboard> GetRoomViewDashboard(GetRoomViewDashboardQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<MasterFilterGroup>> GetRoomViewDashboardFilterValues(GetRoomViewDashboardFilterValuesQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
