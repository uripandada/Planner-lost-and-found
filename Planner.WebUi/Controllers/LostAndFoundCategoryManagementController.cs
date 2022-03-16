using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Files.Commands;
using Planner.Application.LostAndFoundCategoryManagement.Commands.DeleteLostAndFoundCategory;
using Planner.Application.LostAndFoundCategoryManagement.Commands.InsertLostAndFoundCategory;
using Planner.Application.LostAndFoundCategoryManagement.Commands.UpdateLostAndFoundCategory;
using Planner.Application.LostAndFoundCategoryManagement.Queries.GetPageOfLostAndFoundCategories;
using Planner.Application.LostAndFoundCategoryManagement.Queries.GetLostAndFoundCategoryDetails;
using Planner.Common.Data;
using Planner.Domain.Values;
using System;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class LostAndFoundCategoryManagementController : BaseController
	{
		[HttpPost]
		public async Task<PageOf<LostAndFoundCategoryGridItemViewModel>> GetPageOfLostAndFoundCategories([FromBody] GetPageOfLostAndFoundCategoriesQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<LostAndFoundCategoryDetailsViewModel> GetLostAndFoundCategoryDetails([FromBody] GetLostAndFoundCategoryDetailsQuery request)
		{
			return await this.Mediator.Send(request);
		}

		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Categories)]
		[HttpPost]
		public async Task<ProcessResponse<Guid>> InsertLostAndFoundCategory([FromBody] InsertLostAndFoundCategoryCommand request)
		{
			if (!this.ModelState.IsValid)
			{
				var result = new ProcessResponse<Guid>();
				this._populateErrorModelState(result, "Unable to insert category.");
				return result;
			}

			return await this.Mediator.Send(request);
		}

		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Categories)]
		[HttpPost]
		public async Task<ProcessResponse> UpdateLostAndFoundCategory([FromBody] UpdateLostAndFoundCategoryCommand request)
		{
			if (!this.ModelState.IsValid)
			{
				var result = new ProcessResponse<Guid>();
				this._populateErrorModelState(result, "Unable to update category.");
				return result;
			}

			return await this.Mediator.Send(request);
		}


		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Categories)]
		[HttpPost]
		public async Task<ProcessResponse> DeleteLostAndFoundCategory([FromBody] DeleteLostAndFoundCategoryCommand request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
