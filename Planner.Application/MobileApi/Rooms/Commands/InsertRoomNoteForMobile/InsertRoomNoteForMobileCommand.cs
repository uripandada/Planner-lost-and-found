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

namespace Planner.Application.MobileApi.Rooms.Commands.InsertRoomNoteForMobile
{
	public class InsertRoomNoteForMobileCommand: IRequest<MobileRoomNote>
	{
        public string Hotel_id { get; set; }
        public Guid Room_id { get; set; }
        public string Note { get; set; }
        public string Image { get; set; }
        public string Application { get; set; }
        public Guid? Task_uuid { get; set; }
    }
	public class InsertRoomNoteForMobileCommandHandler: IRequestHandler<InsertRoomNoteForMobileCommand, MobileRoomNote>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public InsertRoomNoteForMobileCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<MobileRoomNote> Handle(InsertRoomNoteForMobileCommand request, CancellationToken cancellationToken)
		{
			var hotelLocalDateProvider = new Infrastructure.HotelLocalDateProvider();
			var localCurrentTime = await hotelLocalDateProvider.GetHotelCurrentLocalDate(this._databaseContext, request.Hotel_id);

			var roomNote = new Domain.Entities.RoomNote
			{
				Id = Guid.NewGuid(),
				CreatedById = this._userId,
				CreatedAt = localCurrentTime,
				Application = request.Application,
				CreatedBy = null,
				Expiration = null,
				IsArchived = false,
				Note = request.Note,
				Room = null,
				RoomId = request.Room_id,
				Task = null,
				TaskId = request.Task_uuid,
			};

			await this._databaseContext.RoomNotes.AddAsync(roomNote, cancellationToken);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			roomNote.Room = await this._databaseContext.Rooms.FindAsync(roomNote.RoomId);
			roomNote.CreatedBy = await this._databaseContext.Users.FindAsync(roomNote.CreatedById);

			return new MobileRoomNote
			{
				Id = roomNote.Id,
				Application = roomNote.Application,
				Attendant_status = "",
				Date_ts = roomNote.CreatedAt.ConvertToTimestamp(),
				Expiration = roomNote.Expiration,
				Hotel_id = roomNote.Room.HotelId,
				Image = null,
				Is_archived = roomNote.IsArchived ? 1 : 0,
				Last_room_update = null,
				Note = roomNote.Note,
				Room_housekeeping = "",
				Room_id = roomNote.Room.Id,
				Room_name = roomNote.Room.Name,
				Room_status = "",
				Task_uuid = null,
				User_first_name = roomNote.CreatedBy.FirstName,
				User_id = roomNote.CreatedById,
				User_last_name = roomNote.CreatedBy.LastName,
				User_username = roomNote.CreatedBy.UserName,
			};
		}
	}
	
}
