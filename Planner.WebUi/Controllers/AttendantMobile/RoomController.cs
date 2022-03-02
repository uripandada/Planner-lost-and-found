using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Planner.Application.MobileApi.Rooms.Queries.GetRoomHousekeepingDetailsForMobile;
using Planner.Application.MobileApi.Rooms.Queries.GetListOfRoomStatusesForMobile;
using Planner.Application.MobileApi.Rooms.Queries.GetRoomStatusDetailsForMobile;
using Planner.Application.MobileApi.Rooms.Queries.GetListOfRoomsForMobile;
using Planner.Application.MobileApi.Rooms.Queries.GetRoomDetailsForMobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Planner.Application.MobileApi.Rooms.Commands.InsertRoomNoteForMobile;
using Planner.Application.MobileApi.Rooms.Commands.UpdateRoomNoteForMobile;
using Planner.Application.MobileApi.Rooms.Commands.DeleteRoomNoteForMobile;
using Planner.Application.MobileApi.Rooms.Queries.GetListOfRoomNotesForMobile;
using Planner.Application.MobileApi.Rooms.Queries.GetRoomNoteDetailsForMobile;
using Planner.Application.MobileApi.Rooms.Queries.GetListOfRoomHousekeepingStatusesForMobile;
using Planner.Common.Data;
using Planner.Application.MobileApi.Rooms.Queries.GetListOfRoomMessagesForMobile;
using Planner.Application.MobileApi.Rooms.Commands.SendRoomMessageForMobile;
using System;
using Planner.Application.MobileApi.Rooms.Queries.GetListOfAllRoomMessagesForMobile;
using Planner.Application.MobileApi.Rooms.Commands.ChangeRoomPriorityForMobile;

namespace Planner.WebUi.Controllers.AttendantMobile
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class RoomController : BaseMobileApiController
	{
		[HttpPost]
		public async Task<MobileRoomDetails> GetRoomDetails([FromBody] GetRoomDetailsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<MobileRoom>> GetListOfRooms([FromBody] GetListOfRoomsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<MobileRoomStatusDetails> GetRoomStatusDetails([FromBody] GetRoomStatusDetailsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<MobileRoomStatus>> GetListOfRoomStatuses([FromBody] GetListOfRoomStatusesForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<MobileRoomHousekeepingStatusDetails> GetRoomHousekeepingDetails([FromBody] GetRoomHousekeepingStatusDetailsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<MobileRoomHousekeepingStatus>> GetListOfRoomHousekeepings([FromBody] GetListOfRoomHousekeepingStatusesForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}


		[HttpPost]
		public async Task<MobileRoomNote> InsertNote([FromBody] InsertRoomNoteForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<MobileRoomNote> UpdateNote([FromBody] UpdateRoomNoteForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<SimpleProcessResponse> DeleteNote([FromBody] DeleteRoomNoteForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<MobileRoomNote>> GetListOfNotes([FromBody] GetListOfRoomNotesForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<MobileRoomNoteDetails> GetNoteDetails([FromBody] GetRoomNoteDetailsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<MobileRoomMessageDetails>> GetListOfAllMessages([FromBody] GetListOfAllRoomMessagesForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}
		
		[HttpPost]
		public async Task<IEnumerable<MobileRoomMessage>> GetListOfRoomMessages([FromBody] GetListOfRoomMessagesForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponse<Guid>> SendMessage([FromBody] SendRoomMessageForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponseSimple> ChangeCleaningPriority([FromBody] ChangeRoomPriorityForMobileCommand request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
