using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.ImportPreview.Commands.SaveImportPreviewAssetActions;
using Planner.Application.ImportPreview.Commands.SaveImportPreviewAssets;
using Planner.Application.ImportPreview.Commands.UploadImportPreviewAssetActions;
using Planner.Application.ImportPreview.Commands.UploadImportPreviewAssets;
using Planner.Common.Data;
using Planner.Domain.Values;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class ImportPreviewAssetController : BaseController
	{
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Assets)]
		public async Task<ImportAssetsPreview> UploadAssetsFromFile(IFormFile file)
		{
			var command = new UploadImportPreviewAssetsCommand
			{
				File = file
			};
			return await this.Mediator.Send(command);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Assets)]
		public async Task<ProcessResponse<IEnumerable<SaveAssetImportResult>>> SaveAssets(SaveImportPreviewAssetsCommand command)
		{
			return await this.Mediator.Send(command);
		}

		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Assets)]
		public async Task<ImportAssetActionsPreview> UploadAssetActionsFromFile(IFormFile file)
		{
			var command = new UploadImportPreviewAssetActionsCommand
			{
				File = file
			};
			return await this.Mediator.Send(command);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Assets)]
		public async Task<ProcessResponse<IEnumerable<SaveAssetActionImportResult>>> SaveAssetActions(SaveImportPreviewAssetActionsCommand command)
		{
			return await this.Mediator.Send(command);
		}
	}
}
