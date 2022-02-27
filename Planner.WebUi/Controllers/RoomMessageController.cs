using Microsoft.AspNetCore.Mvc;
using Planner.Application.RoomMessages;
using Planner.Application.RoomMessages.Commands.DeleteMessage;
using Planner.Application.RoomMessages.Commands.InsertComplexMessage;
using Planner.Application.RoomMessages.Commands.InsertSimpleMessage;
using Planner.Application.RoomMessages.Commands.UpdateComplexMessage;
using Planner.Application.RoomMessages.Commands.UpdateSimpleMessage;
using Planner.Application.RoomMessages.Queries.GetComplexRoomMessageDetails;
using Planner.Application.RoomMessages.Queries.GetPageOfRoomMessages;
using Planner.Application.RoomMessages.Queries.GetRoomMessagesFilterValues;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class RoomMessageController : BaseController
	{
		[HttpPost]
		public async Task<PageOf<RoomMessageListItem>> GetPageOfRoomMessages([FromBody] GetPageOfRoomMessagesQuery request)
		{
			return await this.Mediator.Send(request);
		}
		[HttpPost]
		public async Task<RoomMessageFilterValues> GetRoomMessagesFilterValues([FromBody] GetRoomMessagesFilterValuesQuery request)
		{
			return await this.Mediator.Send(request);
		}
		[HttpPost]
		public async Task<RoomMessageDetails> GetComplexRoomMessageDetails([FromBody] GetComplexRoomMessageDetailsQuery request)
		{
			return await this.Mediator.Send(request);
		}
		[HttpPost]
		public async Task<ProcessResponse<Guid>> InsertSimpleMessage([FromBody] InsertSimpleMessageCommand request)
		{
			return await this.Mediator.Send(request);
		}
		[HttpPost]
		public async Task<ProcessResponse<Guid>> InsertComplexMessage([FromBody] InsertComplexMessageCommand request)
		{
			return await this.Mediator.Send(request);
		}
		[HttpPost]
		public async Task<ProcessResponse> UpdateComplexMessage([FromBody] UpdateComplexMessageCommand request)
		{
			return await this.Mediator.Send(request);
		}
		[HttpPost]
		public async Task<ProcessResponse> UpdateSimpleMessage([FromBody] UpdateSimpleMessageCommand request)
		{
			return await this.Mediator.Send(request);
		}
		[HttpPost]
		public async Task<ProcessResponse> DeleteMessage([FromBody] DeleteMessageCommand request)
		{
			return await this.Mediator.Send(request);
		}

	}
}
