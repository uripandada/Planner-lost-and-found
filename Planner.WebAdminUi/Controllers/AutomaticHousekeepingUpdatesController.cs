using Microsoft.AspNetCore.Mvc;
using Ostermann.Application.AutomaticHousekeepingUpdateSettingss.Queries.GetListOfAutomaticHousekeepingUpdateSettings;
using Planner.Application.AutomaticHousekeepingUpdateSettingss.Commands.DeleteAutomaticHousekeepingUpdateSettings;
using Planner.Application.AutomaticHousekeepingUpdateSettingss.Commands.InsertAutomaticHousekeepingUpdateSettings;
using Planner.Application.AutomaticHousekeepingUpdateSettingss.Commands.UpdateAutomaticHousekeepingUpdateSettings;
using Planner.Application.AutomaticHousekeepingUpdateSettingss.Models;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebAdminUi.Controllers
{
	public class AutomaticHousekeepingUpdatesController : BaseController
    {
        [HttpPost]
        public async Task<IEnumerable<AutomaticHousekeepingUpdateSettingsListItem>> GetListOfAutomaticHousekeepingUpdateSettings(GetListOfAutomaticHousekeepingUpdateSettingsQuery request)
        {
            return await this.Mediator.Send(request);
        }
        
        [HttpPost]
        public async Task<ProcessResponse<Guid>> InsertAutomaticHousekeepingUpdateSettings(InsertAutomaticHousekeepingUpdateSettingsCommand request)
        {
            return await this.Mediator.Send(request);
        }
        
        [HttpPost]
        public async Task<ProcessResponse> UpdateAutomaticHousekeepingUpdateSettings(UpdateAutomaticHousekeepingUpdateSettingsCommand request)
        {
            return await this.Mediator.Send(request);
        }
        
        [HttpPost]
        public async Task<ProcessResponse> DeleteAutomaticHousekeepingUpdateSettings(DeleteAutomaticHousekeepingUpdateSettingsCommand request)
        {
            return await this.Mediator.Send(request);
        }
    }	
}
