using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Admin.CleaningCalendar.Queries.GetWeeklyCleaningCalendar;
using Planner.Domain.Values;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class CleaningCalendarController : BaseController
	{
		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningCalendar)]

		public async Task<CleaningCalendarIntervalResult> GetWeeklyCleaningCalendar(GetWeeklyCleaningCalendarQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
