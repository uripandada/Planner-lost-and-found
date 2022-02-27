using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Rooms.Queries.GetListOfRoomNotesForMobile
{
	public class MobileRoomNote
	{
		public Guid Id { get; set; }
		public string Hotel_id { get; set; }
        public Guid Room_id { get; set; }
        public string Room_name { get; set; }
        public string Room_status { get; set; } // 
        public string Room_housekeeping { get; set; }
        public string Attendant_status { get; set; }
        public int? Last_room_update { get; set; }
        public long? Date_ts { get; set; }
        public string Note { get; set; }
        public string Image { get; set; } = "";
        public Guid User_id { get; set; }
        public string User_username { get; set; }
        public string User_first_name { get; set; }
        public string User_last_name { get; set; }
        public string Application { get; set; }
        public DateTime? Expiration { get; set; }
        public int Is_archived { get; set; } = 0;
        public Guid? Task_uuid { get; set; } = null;
	}

	public class GetListOfRoomNotesForMobileQuery: IRequest<IEnumerable<MobileRoomNote>>
	{
        public string HotelId { get; set; }
	}

	public class GetListOfRoomNotesForMobileQueryHandler : IRequestHandler<GetListOfRoomNotesForMobileQuery, IEnumerable<MobileRoomNote>>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetListOfRoomNotesForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<IEnumerable<MobileRoomNote>> Handle(GetListOfRoomNotesForMobileQuery request, CancellationToken cancellationToken)
		{
			var roomNotes = await this._databaseContext.RoomNotes
				.Include(rn => rn.CreatedBy)
				.Include(rn => rn.Task)
				.Include(rn => rn.Room)
				.Where(rn => rn.Room.HotelId == request.HotelId)
				.ToArrayAsync();
				
			return roomNotes.Select(rn => new MobileRoomNote 
			{ 
				Application = rn.Application,
				Attendant_status = "",
				Date_ts = rn.CreatedAt.ConvertToTimestamp(),
				Expiration = rn.Expiration,
				Hotel_id = rn.Room.HotelId,
				Id = rn.Id,
				Image = "",
				Is_archived = rn.IsArchived ? 1 : 0,
				Last_room_update = null,
				Note = rn.Note,
				Room_housekeeping = "",
				Room_id = rn.RoomId,
				Room_name = rn.Room.Name,
				Room_status = "",
				Task_uuid = rn.TaskId,
				User_first_name = rn.CreatedBy.FirstName,
				User_id = rn.CreatedById,
				User_last_name = rn.CreatedBy.LastName,
				User_username = rn.CreatedBy.UserName,
			}).ToArray();
		}
	}
}
