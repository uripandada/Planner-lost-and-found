using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.MobileApi.Cleanings.Commands.CreateCustomCleaningForMobile;
using Planner.Application.MobileApi.Cleanings.Commands.UpdateCleaningStatus;
using Planner.Application.MobileApi.Cleanings.Commands.UpdateInspectionStatus;
using Planner.Application.MobileApi.Cleanings.Queries.GetCleaningDetailsForMobile;
using Planner.Application.MobileApi.Cleanings.Queries.GetListOfCleaningsForInspectionForMobile;
using Planner.Application.MobileApi.Cleanings.Queries.GetListOfCleaningsForMobile;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers.AttendantMobile
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class CleaningController : BaseMobileApiController
	{
		[HttpPost]
		public async Task<ProcessResponse<Guid>> InsertCleaning([FromBody] CreateCustomCleaningForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}
		
		[HttpPost]
		public async Task<CleaningDetailsForMobile> GetRoomCleaning([FromBody] GetCleaningDetailsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<CleaningForMobile>> GetListOfCleanings([FromBody] GetListOfCleaningsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<CleaningForMobile>> GetListOfCleaningsForInspection([FromBody] GetListOfCleaningsForInspectionForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}
		
		[HttpPost]
		public async Task<ExtendedMobileRoomDetails> UpdateCleaningStatus([FromBody] UpdateCleaningStatusCommand request)
		{
			return await this.Mediator.Send(request);
		}
		
		[HttpPost]
		public async Task<ExtendedMobileRoomDetails> UpdateInspectionStatus([FromBody] UpdateInspectionStatusCommand request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
