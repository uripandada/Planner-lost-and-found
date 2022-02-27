using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.UserManagement.Commands.ChangeMyOnDutyStatus;
using Planner.Application.UserManagement.Commands.DeleteGroup;
using Planner.Application.UserManagement.Commands.DeleteSubGroup;
using Planner.Application.UserManagement.Commands.DeleteUser;
using Planner.Application.UserManagement.Commands.InsertGroup;
using Planner.Application.UserManagement.Commands.InsertSubGroup;
using Planner.Application.UserManagement.Commands.InsertUser;
using Planner.Application.UserManagement.Commands.UpdateGroup;
using Planner.Application.UserManagement.Commands.UpdateSubGroup;
using Planner.Application.UserManagement.Commands.UpdateUser;
using Planner.Application.UserManagement.Models;
using Planner.Application.UserManagement.Queries.GetFullUserHierarchy;
using Planner.Application.UserManagement.Queries.GetGroupsAndSubGroups;
using Planner.Application.UserManagement.Queries.GetListOfCleaners;
using Planner.Application.UserManagement.Queries.GetListOfUsers;
using Planner.Application.UserManagement.Queries.GetUserByIdQuery;
using Planner.Common.Data;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class UserManagementController : BaseController
	{
		[HttpPost]
		public async Task<FullGroupHierarchyData> GetFullUserGroupsHierarchy([FromBody] GetFullUserGroupsHierarchyQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpGet]
		public async Task<ActionResult<ProcessResponse<FullGroupHierarchyData>>> GetGroupsAndSubGroups()
		{
			var result = await this.Mediator.Send(new GetGroupsAndSubGroupsQuery());
			return this.Ok(result);
		}


		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Users)]
		public async Task<ActionResult<ProcessResponse<GroupHierarchyData>>> InsertGroup([FromBody] InsertGroupCommand request)
		{
			var result = await this.Mediator.Send(request);
			return this.Ok(result);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Users)]
		public async Task<ActionResult<ProcessResponse<GroupHierarchyData>>> UpdateGroup([FromBody] UpdateGroupCommand request)
		{
			var result = await this.Mediator.Send(request);
			return this.Ok(result);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Users)]
		public async Task<ActionResult<ProcessResponse>> DeleteGroup([FromBody] DeleteGroupCommand request)
		{
			var result = await this.Mediator.Send(request);
			return this.Ok(result);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Users)]
		public async Task<ActionResult<ProcessResponse<SubGroupHierarchyData>>> InsertSubGroup([FromBody] InsertSubGroupCommand request)
		{
			var result = await this.Mediator.Send(request);
			return this.Ok(result);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Users)]
		public async Task<ActionResult<ProcessResponse<SubGroupHierarchyData>>> UpdateSubGroup([FromBody] UpdateSubGroupCommand request)
		{
			var result = await this.Mediator.Send(request);
			return this.Ok(result);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Users)]
		public async Task<ActionResult<ProcessResponse>> DeleteSubGroup([FromBody] DeleteSubGroupCommand request)
		{
			var result = await this.Mediator.Send(request);
			return this.Ok(result);
		}

		[HttpGet]
		public async Task<ActionResult<ProcessResponse<UserModel>>> GetUserById(Guid id)
		{
			var result = await this.Mediator.Send(new GetUserByIdQuery { Id = id });
			return this.Ok(result);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Users)]
		public async Task<ActionResult<ProcessResponse<UserHierarchyData>>> InsertUser([FromBody] InsertUserCommand request)
		{
			if (!this.ModelState.IsValid)
			{
				var errorResult = new ProcessResponse<UserHierarchyData>();
				this._populateErrorModelState(errorResult, "Unable to insert user.");
				return Ok(errorResult);
			}

			var result = await this.Mediator.Send(request);
			return this.Ok(result);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Users)]
		public async Task<ActionResult<ProcessResponse<UserHierarchyData>>> UpdateUser([FromBody] UpdateUserCommand request)
		{
			if (!this.ModelState.IsValid)
			{
				var errorResult = new ProcessResponse<UserHierarchyData>();
				this._populateErrorModelState(errorResult, "Unable to update user.");
				return Ok(errorResult);
			}

			var result = await this.Mediator.Send(request);
			return this.Ok(result);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Users)]
		public async Task<DeleteProcessResponse> DeleteUser([FromBody] DeleteUserCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<UserListItemData>> GetListOfUsers([FromBody] GetListOfUsersQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<CleanerListItemData>> GetListOfCleaners([FromBody] GetListOfCleanersQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponse> ChangeMyOnDutyStatus([FromBody] ChangeMyOnDutyStatusCommand request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
