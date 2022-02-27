using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomManagement.Queries.GetUnassignedRooms
{
	public class UnassignedRoomData
	{
		public Guid Id { get; set; }
		public string ExternalId { get; set; }
		public string Name { get; set; }
		public string TypeKey { get; set; }
		public IEnumerable<GetFullRoomHierarchy.FullRoomHierarchyBedData> Beds { get; set; }
	}

	public class GetUnassignedRoomsQuery : GetPageRequest, IRequest<PageOf<UnassignedRoomData>>
	{
		public string HotelId { get; set; }
		public string SortKey { get; set; }
	}

	public class GetUnassignedRoomsQueryHandler : IRequestHandler<GetUnassignedRoomsQuery, PageOf<UnassignedRoomData>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetUnassignedRoomsQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<PageOf<UnassignedRoomData>> Handle(GetUnassignedRoomsQuery request, CancellationToken cancellationToken)
		{
			var roomsQuery = this._databaseContext.Rooms
				.Where(r => r.FloorId == null && r.BuildingId == null)
				.AsQueryable();

			if (request.HotelId.IsNotNull())
			{
				roomsQuery = roomsQuery.Where(r => r.HotelId == request.HotelId);
			}

			var roomsCount = 0;

			if(request.Skip > 0 || request.Take > 0)
			{
				roomsCount = await roomsQuery.CountAsync();
			}

			if (request.Skip > 0)
			{
				roomsQuery = roomsQuery.Skip(request.Skip);
			}

			if(request.Take > 0)
			{
				roomsQuery = roomsQuery.Take(request.Take);
			}

			var rooms = await roomsQuery
				.OrderBy(r => r.Name)
				.Select(r => new UnassignedRoomData 
				{ 
					ExternalId = r.ExternalId,
					Id = r.Id,
					Name = r.Name,
					TypeKey = r.TypeKey,
					Beds = r.RoomBeds.Select(rb => new GetFullRoomHierarchy.FullRoomHierarchyBedData 
					{ 
						ExternalId = rb.ExternalId,
						Id = rb.Id,
						Name = rb.Name,
					}).ToArray()
				})
				.ToListAsync();

			if(request.Skip == 0 && request.Take == 0)
			{
				roomsCount = rooms.Count;
			}

			return new PageOf<UnassignedRoomData>
			{
				Items = rooms,
				TotalNumberOfItems = roomsCount
			};
		}
	}
}
