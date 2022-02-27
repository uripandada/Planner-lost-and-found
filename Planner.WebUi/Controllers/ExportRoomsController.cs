using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Export.Commands.ExportRooms;
using Planner.Domain.Values;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class ExportRoomsController : BaseController
	{
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Rooms)]
		public async Task<FileContentResult> ExportRoomsFromDatabase()
		{
			var command = new ExportRoomsCommand();

			var result = await this.Mediator.Send(command);
			return File(result, "application/vnd.ms-excel", "rooms.csv");
		}
	}
}
