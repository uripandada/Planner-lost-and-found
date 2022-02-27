using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomManagement.Queries.GetRoomHistory
{
	public class RoomHistoryItem
	{
		public Guid Id { get; set; }
		public Guid RoomId { get; set; }
		public Guid? BedId { get; set; }
		public DateTime At { get; set; }
		public string AtDateString { get; set; }
		public string AtTimeString { get; set; }
		public string UserName { get; set; }
		public string Description { get; set; }
		public Common.Enums.RoomEventType Type { get; set; }
	}

	public class GetRoomHistoryQuery: IRequest<IEnumerable<RoomHistoryItem>>
	{
		public Guid RoomId { get; set; }
		public Guid? BedId { get; set; }
	}

	public class GetRoomHistoryQueryHandler: IRequestHandler<GetRoomHistoryQuery, IEnumerable<RoomHistoryItem>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetRoomHistoryQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<IEnumerable<RoomHistoryItem>> Handle(GetRoomHistoryQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.RoomHistoryEvents.Where(e => e.RoomId == request.RoomId).AsQueryable();

			if (request.BedId.HasValue)
			{
				query = query.Where(e => e.RoomBedId == request.BedId.Value);
			}

			var events = await query.OrderByDescending(e => e.At).Take(50).Select(e => new RoomHistoryItem 
			{ 
				At = e.At,
				BedId = e.RoomBedId,
				Description = e.Message,
				Id = e.Id,
				RoomId = e.RoomId,
				UserName = e.UserId == null ? "System" : $"{e.User.FirstName} {e.User.LastName}",
				AtDateString = e.At.ToString("dddd, dd MMMM yyyy"),
				AtTimeString = e.At.ToString("HH:mm"),
				Type = e.Type,
			}).ToArrayAsync();

			foreach(var e in events)
			{
				if(e.Type == Common.Enums.RoomEventType.PMS_EVENT)
				{
					e.UserName = "PMS event";
				} 
				else if(e.Type.ToString().StartsWith("RCCSYNC"))
				{
					e.UserName = "Reservation sync";
				}
			}

			return events;
		}
	}
}
