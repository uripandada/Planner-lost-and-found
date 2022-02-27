using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.ImportPreview.Commands.SaveImportPreviewUsers;
using Planner.Application.ImportPreview.Commands.UploadImportPreviewUsers;
using Planner.Common.Data;
using Planner.Domain.Values;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
    public class ImportPreviewUserController : BaseController
    {
        //[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Users)]
        public async Task<ImportUsersPreview> UploadUsersFromFile(IFormFile file)
        {
            var command = new UploadImportPreviewUsersCommand
            {
                File = file 
            };
            return await this.Mediator.Send(command);
        }

        [HttpPost]
        //[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Users)]
        public async Task<ProcessResponse<IEnumerable<SaveUserImportResult>>> SaveUsers(SaveImportPreviewUsersCommand importPreviewUserResults)
        {
            return await this.Mediator.Send(importPreviewUserResults);
        }
    }
}
