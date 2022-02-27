using Microsoft.AspNetCore.Mvc;
using Planner.Application.Admin.CleaningCalendar.Queries.GetWeeklyCleaningCalendar;
using System.Threading.Tasks;

namespace Planner.WebAdminUi.Controllers
{
	public class CleaningCalendarController : BaseController
	{
		[HttpPost]
		public async Task<CleaningCalendarIntervalResult> GetWeeklyCleaningCalendar(GetWeeklyCleaningCalendarQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
