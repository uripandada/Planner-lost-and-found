using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Files.Commands;
using Planner.Application.ExperienceCategoryManagement.Commands.DeleteExperienceCategory;
using Planner.Application.ExperienceCategoryManagement.Commands.InsertExperienceCategory;
using Planner.Application.ExperienceCategoryManagement.Commands.UpdateExperienceCategory;
using Planner.Application.ExperienceCategoryManagement.Queries.GetPageOfExperienceCategories;
using Planner.Application.ExperienceCategoryManagement.Queries.GetExperienceCategoryDetails;
using Planner.Common.Data;
using Planner.Domain.Values;
using System;
using System.Threading.Tasks;
using Planner.Application.ExperienceCategoryManagement.Queries.GetList;

namespace Planner.WebUi.Controllers
{
	public class ExperienceCategoryManagementController : BaseController
	{
		[HttpPost]
		public async Task<ExperienceCategoryItemData[]> GetList(GetListOfExperienceCategoriesQuery query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpPost]
		public async Task<PageOf<ExperienceCategoryGridItemViewModel>> GetPageOfExperienceCategories([FromBody] GetPageOfExperienceCategoriesQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ExperienceCategoryDetailsViewModel> GetExperienceCategoryDetails([FromBody] GetExperienceCategoryDetailsQuery request)
		{
			return await this.Mediator.Send(request);
		}

		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.ExperienceCategories)]
		[HttpPost]
		public async Task<ProcessResponse<Guid>> InsertExperienceCategory([FromBody] InsertExperienceCategoryCommand request)
		{
			if (!this.ModelState.IsValid)
			{
				var result = new ProcessResponse<Guid>();
				this._populateErrorModelState(result, "Unable to insert category.");
				return result;
			}

			return await this.Mediator.Send(request);
		}

		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.ExperienceCategories)]
		[HttpPost]
		public async Task<ProcessResponse> UpdateExperienceCategory([FromBody] UpdateExperienceCategoryCommand request)
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
		public async Task<ProcessResponse> DeleteExperienceCategory([FromBody] DeleteExperienceCategoryCommand request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
