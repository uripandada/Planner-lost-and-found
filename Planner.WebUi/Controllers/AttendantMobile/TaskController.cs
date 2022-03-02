using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.MobileApi.Tasks.Commands.InsertTaskForMobile;
using Planner.Application.MobileApi.Tasks.Commands.MoveTaskToDepartureForMobile;
using Planner.Application.MobileApi.Tasks.Commands.ReassignTaskForMobile;
using Planner.Application.MobileApi.Tasks.Commands.UpdateMultipleTaskStatusesForMobile;
using Planner.Application.MobileApi.Tasks.Commands.UpdateTaskStatusForMobile;
using Planner.Application.MobileApi.Tasks.Queries.GetListOfTasksForMobile;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers.AttendantMobile
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class TaskController : BaseMobileApiController
	{
		[HttpPost]
		public async Task<ProcessResponse> InsertTask([FromBody] InsertTaskForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<SimpleProcessResponse> UpdateTaskStatus([FromBody] UpdateTaskStatusForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}
		
		[HttpPost]
		public async Task<SimpleProcessResponse> UpdateMultipleTaskStatuses([FromBody] UpdateMultipleTaskStatusesForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<MobileTask>> GetListOfTasks([FromBody] GetListOfTasksForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponseSimple<Guid>> ReassignTask([FromBody] ReassignTaskForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponseSimple> MoveTaskToDeparture([FromBody] MoveTaskToDepartureForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
