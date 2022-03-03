using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Files.Commands;
using Planner.Application.CategoryManagement.Commands.DeleteCategory;
using Planner.Application.CategoryManagement.Commands.InsertCategory;
using Planner.Application.CategoryManagement.Commands.UpdateCategory;
using Planner.Application.CategoryManagement.Queries.GetPageOfCategories;
using Planner.Application.CategoryManagement.Queries.GetCategoryDetails;
using Planner.Common.Data;
using Planner.Domain.Values;
using System;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class CategoryManagementController : BaseController
	{
		[HttpPost]
		public async Task<PageOf<CategoryGridItemViewModel>> GetPageOfCategories([FromBody] GetPageOfCategoriesQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<CategoryDetailsViewModel> GetCategoryDetails([FromBody] GetCategoryDetailsQuery request)
		{
			return await this.Mediator.Send(request);
		}

		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Categories)]
		[HttpPost]
		public async Task<ProcessResponse<Guid>> InsertCategory([FromBody] InsertCategoryCommand request)
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
		public async Task<ProcessResponse> UpdateCategory([FromBody] UpdateCategoryCommand request)
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
		public async Task<ProcessResponse> DeleteCategory([FromBody] DeleteCategoryCommand request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
