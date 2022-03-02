using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.MobileApi.Users.Commands.InsertUserForMobile;
using Planner.Application.MobileApi.Users.Commands.UpdateOnDutyStatusForMobile;
using Planner.Application.MobileApi.Users.Queries.GetListOfHotelGroupUsersForHotelForMobile;
using Planner.Application.MobileApi.Users.Queries.GetListOfHotelGroupUsersForMobile;
using Planner.Application.MobileApi.Users.Queries.GetListOfUserGroupsForMobile;
using Planner.Application.MobileApi.Users.Queries.GetListOfUsersForMobile;
using Planner.Application.MobileApi.Users.Queries.GetUserDetailsForMobile;
using Planner.Application.MobileApi.Users.Queries.GetUserGroupDetailsForMobile;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers.AttendantMobile
{
	public class UserController : BaseMobileApiController
	{
		[AllowAnonymous]
		[HttpPost]
		public async Task<MobileUsersPreview> GetAvailableUsers([FromBody]GetListOfHotelGroupUsersForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}
		
		[AllowAnonymous]
		[HttpPost]
		public async Task<MobileUsersPreview> GetAvailableUsersForHotel([FromBody]GetListOfHotelGroupUsersForHotelForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpPost]
		public async Task<UserDetailsForMobile> GetDetails([FromBody] GetUserDetailsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpPost]
		public async Task<UserDetailsForMobile> GetMyDetails([FromBody] GetMyUserDetailsForMobileQuery request)
		{
			var userDetailsRequest = new GetUserDetailsForMobileQuery
			{
				Id = this.HttpContext.UserId()
			};

			return await this.Mediator.Send(userDetailsRequest);
		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpPost]
		public async Task<IEnumerable<MobileUser>> GetListOfUsers([FromBody] GetListOfUsersForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpPost]
		public async Task<MobileUser> UpdateOnDutyStatus([FromBody] UpdateOnDutyStatusForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpPost]
		public async Task<IEnumerable<MobileUserGroup>> GetListOfUserGroups([FromBody] GetListOfUserGroupsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpPost]
		public async Task<MobileUserGroupDetails> GetUserGroupDetails([FromBody] GetUserGroupDetailsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpPost]
		public async Task<ProcessResponseSimple<Guid>> InsertUser([FromBody] InsertUserForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
