using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.ExternalApi.Cleanings.Commands.ExternalRequestCleaning;
using Planner.Application.ExternalApi.Rooms.Commands.ExternalDndOff;
using Planner.Application.ExternalApi.Rooms.Commands.ExternalDndOn;
using Planner.Application.ExternalApi.Tasks.Commands.ExternalInsertCompactTask;
using Planner.Common.Data;
using System;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers.External
{
	public class TaskController : BaseExternalApiController
	{
		[HttpPost]
		public async Task<ProcessResponseSimple<Guid>> Create([FromBody] ExternalInsertCompactTaskCommand request)
		{
			return await this.Mediator.Send(request);
		}
	}

	//public class DndController : BaseExternalApiController
	//{
	//	[HttpGet]
	//	public async Task<ProcessResponse> On(ExternalDndOnCommand request)
	//	{
	//		return await this.Mediator.Send(request);
	//	}

	//	[HttpGet]
	//	public async Task<ProcessResponse> Off(ExternalDndOffCommand request)
	//	{
	//		return await this.Mediator.Send(request);
	//	}
	//}

	//public class CleaningController : BaseExternalApiController
	//{
	//	[HttpGet]
	//	public async Task<ProcessResponse<Guid>> RequestNewCleaning(ExternalRequestCleaningCommand request)
	//	{
	//		return await this.Mediator.Send(request);
	//	}
	//}
}
