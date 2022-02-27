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

namespace Planner.Application.MobileApi.Rooms.Commands.UpdateRoomNoteForMobile
{
	public class UpdateRoomNoteForMobileCommand : IRequest<MobileRoomNote>
	{
		public string HotelId { get; set; }
		public Guid RoomId { get; set; }
		public Guid Id { get; set; }
	}
	public class UpdateRoomNoteForMobileCommandHandler : IRequestHandler<UpdateRoomNoteForMobileCommand, MobileRoomNote>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public UpdateRoomNoteForMobileCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<MobileRoomNote> Handle(UpdateRoomNoteForMobileCommand request, CancellationToken cancellationToken)
		{
			using (var transaction = await this._databaseContext.Database.BeginTransactionAsync(cancellationToken))
			{
				var roomNote = await this._databaseContext.RoomNotes.Include(rc => rc.Room).Include(rc => rc.CreatedBy).Where(rn => rn.Id == request.Id && rn.RoomId == request.RoomId && rn.Room.HotelId == request.HotelId).FirstOrDefaultAsync();
				if (roomNote != null)
				{
					roomNote.IsArchived = true;

					await this._databaseContext.SaveChangesAsync(cancellationToken);
					await transaction.CommitAsync(cancellationToken);
				}

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
}
