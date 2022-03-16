using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Files.Commands;
using Planner.Application.ExperienceCompensationManagement.Commands.DeleteExperienceCompensation;
using Planner.Application.ExperienceCompensationManagement.Commands.InsertExperienceCompensation;
using Planner.Application.ExperienceCompensationManagement.Commands.UpdateExperienceCompensation;
using Planner.Application.ExperienceCompensationManagement.Queries.GetPageOfExperienceCompensations;
using Planner.Application.ExperienceCompensationManagement.Queries.GetExperienceCompensationDetails;
using Planner.Common.Data;
using Planner.Domain.Values;
using System;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class ExperienceCompensationManagementController : BaseController
	{
		[HttpPost]
		public async Task<PageOf<ExperienceCompensationGridItemViewModel>> GetPageOfExperienceCategories([FromBody] GetPageOfExperienceCategoriesQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ExperienceCompensationDetailsViewModel> GetExperienceCompensationDetails([FromBody] GetExperienceCompensationDetailsQuery request)
		{
			return await this.Mediator.Send(request);
		}

		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.ExperienceCategories)]
		[HttpPost]
		public async Task<ProcessResponse<Guid>> InsertExperienceCompensation([FromBody] InsertExperienceCompensationCommand request)
		{
			if (!this.ModelState.IsValid)
			{
				var result = new ProcessResponse<Guid>();
				this._populateErrorModelState(result, "Unable to insert Experience Compensation.");
				return result;
			}

			return await this.Mediator.Send(request);
		}

		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.ExperienceCategories)]
		[HttpPost]
		public async Task<ProcessResponse> UpdateExperienceCompensation([FromBody] UpdateExperienceCompensationCommand request)
		{
			if (!this.ModelState.IsValid)
			{
				var result = new ProcessResponse<Guid>();
				this._populateErrorModelState(result, "Unable to update Experience Compensation.");
				return result;
			}

			return await this.Mediator.Send(request);
		}


		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.ExperienceCategories)]
		[HttpPost]
		public async Task<ProcessResponse> DeleteExperienceCompensation([FromBody] DeleteExperienceCompensationCommand request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
