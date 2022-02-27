using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Files.Commands;
using Planner.Application.RoomCategoryManagement.Commands.DeleteRoomCategory;
using Planner.Application.RoomCategoryManagement.Commands.InsertRoomCategory;
using Planner.Application.RoomCategoryManagement.Commands.UpdateRoomCategory;
using Planner.Application.RoomCategoryManagement.Queries.GetPageOfRoomCategories;
using Planner.Application.RoomCategoryManagement.Queries.GetRoomCategoryDetails;
using Planner.Common.Data;
using Planner.Domain.Values;
using System;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class RoomCategoryManagementController : BaseController
	{
		[HttpPost]
		public async Task<PageOf<RoomCategoryGridItemViewModel>> GetPageOfRoomCategories([FromBody] GetPageOfRoomCategoriesQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<RoomCategoryDetailsViewModel> GetRoomCategoryDetails([FromBody] GetRoomCategoryDetailsQuery request)
		{
			return await this.Mediator.Send(request);
		}

		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.RoomCategories)]
		[HttpPost]
		public async Task<ProcessResponse<Guid>> InsertRoomCategory([FromBody] InsertRoomCategoryCommand request)
		{
			if (!this.ModelState.IsValid)
			{
				var result = new ProcessResponse<Guid>();
				this._populateErrorModelState(result, "Unable to insert category.");
				return result;
			}

			return await this.Mediator.Send(request);
		}

		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.RoomCategories)]
		[HttpPost]
		public async Task<ProcessResponse> UpdateRoomCategory([FromBody] UpdateRoomCategoryCommand request)
		{
			if (!this.ModelState.IsValid)
			{
				var result = new ProcessResponse<Guid>();
				this._populateErrorModelState(result, "Unable to update category.");
				return result;
			}

			return await this.Mediator.Send(request);
		}


		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.RoomCategories)]
		[HttpPost]
		public async Task<ProcessResponse> DeleteRoomCategory([FromBody] DeleteRoomCategoryCommand request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
