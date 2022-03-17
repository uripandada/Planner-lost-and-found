using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Files.Commands;
using Planner.Application.ExperienceManagement.Commands.Insert;
using Planner.Common.Data;
using Planner.Domain.Values;
using System;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class ExperienceManagementController : BaseController
	{
		/*[HttpPost]
		public async Task<PageOf<ExperienceGridItemViewModel>> GetPageOfExperienceCategories([FromBody] GetPageOfExperienceCategoriesQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ExperienceDetailsViewModel> GetExperienceDetails([FromBody] GetExperienceDetailsQuery request)
		{
			return await this.Mediator.Send(request);
		}*/

		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.ExperienceCategories)]
		[HttpPost]
		public async Task<ProcessResponse<Guid>> InsertExperience([FromBody] InsertExperienceCommand request)
		{
			if (!this.ModelState.IsValid)
			{
				var result = new ProcessResponse<Guid>();
				this._populateErrorModelState(result, "Unable to insert experience.");
				return result;
			}

			return await this.Mediator.Send(request);
		}

		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.ExperienceCategories)]
		/*[HttpPost]
		public async Task<ProcessResponse> UpdateExperience([FromBody] UpdateExperienceCommand request)
		{
			if (!this.ModelState.IsValid)
			{
				var result = new ProcessResponse<Guid>();
				this._populateErrorModelState(result, "Unable to update category.");
				return result;
			}

			return await this.Mediator.Send(request);
		}


		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.ExperienceCategories)]
		[HttpPost]
		public async Task<ProcessResponse> DeleteExperience([FromBody] DeleteExperienceCommand request)
		{
			return await this.Mediator.Send(request);
		}*/
	}
}
