using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Export.Commands.ExportUsers;
using Planner.Domain.Values;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class ExportUsersController : BaseController
	{
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Users)]
		public async Task<FileContentResult> ExportUsersFromDatabase()
		{
			var command = new ExportUsersCommand();

			var result = await this.Mediator.Send(command);
			return File(result, "application/vnd.ms-excel", "users.csv");
		}
	}
}
