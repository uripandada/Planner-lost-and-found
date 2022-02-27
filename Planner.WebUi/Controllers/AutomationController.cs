using Microsoft.AspNetCore.Mvc;
using Planner.Application.ServicesJobs.Commands.SetRoomsStatusCommand;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class AutomationController : BaseController
	{
		[HttpPost]
		public async Task<bool> ChangeNightlyRoomStatuses(SetRoomsStatusCommand request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
