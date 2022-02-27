using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Export.Commands.ExportAssetActions;
using Planner.Application.Export.Commands.ExportAssets;
using Planner.Domain.Values;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class ExportAssetsController : BaseController
	{
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Assets)]
		public async Task<FileContentResult> ExportAssetsFromDatabase()
		{
			var command = new ExportAssetsCommand();
			var result = await this.Mediator.Send(command);
			return File(result, "application/vnd.ms-excel", "assets.csv");
		}

		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Assets)]
		public async Task<FileContentResult> ExportAssetActionsFromDatabase()
		{
			var command = new ExportAssetActionsCommand();
			var result = await this.Mediator.Send(command);
			return File(result, "application/vnd.ms-excel", "asset-actions.csv");
		}
	}
}
