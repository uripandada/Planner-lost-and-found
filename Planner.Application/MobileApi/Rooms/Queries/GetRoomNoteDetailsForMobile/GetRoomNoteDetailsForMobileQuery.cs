using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.MobileApi.Rooms.Queries.GetListOfRoomNotesForMobile;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Rooms.Queries.GetRoomNoteDetailsForMobile
{
	public class MobileRoomNoteDetails: MobileRoomNote
	{
	}

	public class GetRoomNoteDetailsForMobileQuery: IRequest<MobileRoomNoteDetails>
	{
		public string HotelId { get; set; }
		public Guid Id { get; set; }
	}
	public class GetRoomNoteDetailsForMobileQueryHandler : IRequestHandler<GetRoomNoteDetailsForMobileQuery, MobileRoomNoteDetails>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetRoomNoteDetailsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<MobileRoomNoteDetails> Handle(GetRoomNoteDetailsForMobileQuery request, CancellationToken cancellationToken)
		{
			var roomNote = await this._databaseContext.RoomNotes
				.Include(rn => rn.CreatedBy)
				.Include(rn => rn.Task)
				.Include(rn => rn.Room)
				.Where(rn => rn.Id == request.Id && rn.Room.HotelId == request.HotelId)
				.FirstOrDefaultAsync();

			return new MobileRoomNoteDetails
			{
				Application = roomNote.Application,
				Attendant_status = "",
				Date_ts = roomNote.CreatedAt.ConvertToTimestamp(),
				Expiration = roomNote.Expiration,
				Hotel_id = roomNote.Room.HotelId,
				Id = roomNote.Id,
				Image = "",
				Is_archived = roomNote.IsArchived ? 1 : 0,
				Last_room_update = null,
				Note = roomNote.Note,
				Room_housekeeping = "",
				Room_id = roomNote.RoomId,
				Room_name = roomNote.Room.Name,
				Room_status = "",
				Task_uuid = roomNote.TaskId,
				User_first_name = roomNote.CreatedBy.FirstName,
				User_id = roomNote.CreatedById,
				User_last_name = roomNote.CreatedBy.LastName,
				User_username = roomNote.CreatedBy.UserName,
			};
		}
	}
}
