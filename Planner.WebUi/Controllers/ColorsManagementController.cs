using Microsoft.AspNetCore.Mvc;
using Planner.Application.ColorsManagement.Commands.UpdateRccHouskeepingColors;
using Planner.Application.ColorsManagement.Queries.GetRccHousekeepingColors;
using Planner.Common.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class ColorsManagementController : BaseController
	{
		[HttpPost]
		public async Task<IEnumerable<RccHousekeepingStatusColorDetails>> GetListOfRccHousekeepingStatusColors(GetRccHousekeepingColorsQuery query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpPost]
		public async Task<ProcessResponse> UpdateRccHousekeepingStatusColors(UpdateRccHouskeepingColorsCommand query)
		{
			return await this.Mediator.Send(query);
		}
	}
}
