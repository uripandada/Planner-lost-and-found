using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.ImportPreview.Commands.SaveImportPreviewRooms;
using Planner.Application.ImportPreview.Commands.UploadImportPreviewRooms;
using Planner.Common.Data;
using Planner.Domain.Values;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
    public class ImportPreviewRoomController : BaseController
    {
        //[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Rooms)]
        public async Task<ImportRoomsPreview> UploadRoomsFromFile(IFormFile file)
        {
            var command = new UploadImportPreviewRoomsCommand
            {
                File = file 
            };
            return await this.Mediator.Send(command);
        }

        [HttpPost]
        //[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Rooms)]
        public async Task<ProcessResponse<IEnumerable<SaveRoomImportResult>>> SaveRooms(SaveImportPreviewRoomsCommand command)
        {
            return await this.Mediator.Send(command);
        }
    }
}
