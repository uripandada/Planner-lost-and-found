using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomManagement.Queries.GetRoomDetails
{
	public class RoomDetailsBed
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string ExternalId { get; set; }
	}

	public class RoomDetailsData
	{
		public Guid Id { get; set; }
		public Guid? BuildingId { get; set; }
		public Guid? FloorId { get; set; }
		public string ExternalId { get; set; }
		public string Name { get; set; }
		public string TypeKey { get; set; }
		public int OrdinalNumber { get; set; }

		public string FloorSectionName { get; set; }
		public string FloorSubSectionName { get; set; }
		public Guid? CategoryId { get; set; }
		public string CategoryName { get; set; }
		public IEnumerable<RoomDetailsBed> Beds { get; set; }

	}
	public class GetRoomDetailsQuery : IRequest<RoomDetailsData>
	{
		public Guid Id { get; set; }
	}

	public class GetRoomDetailsQueryHandler : IRequestHandler<GetRoomDetailsQuery, RoomDetailsData>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetRoomDetailsQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<RoomDetailsData> Handle(GetRoomDetailsQuery request, CancellationToken cancellationToken)
		{
			var room = await this._databaseContext
				.Rooms
				.Select(r => new RoomDetailsData
				{
					Id = r.Id,
					TypeKey = r.TypeKey,
					ExternalId = r.ExternalId,
					Name = r.Name,
					OrdinalNumber = r.OrdinalNumber,
					BuildingId = r.BuildingId,
					FloorId = r.FloorId,
					CategoryId = r.CategoryId,
					CategoryName = r.Category.Name,
					FloorSectionName = r.FloorSectionName,
					FloorSubSectionName = r.FloorSubSectionName,
					Beds = r.RoomBeds.Select(rb => new RoomDetailsBed
					{
						ExternalId = rb.ExternalId,
						Id = rb.Id,
						Name = rb.Name,
					}).ToArray()
				})
				.Where(r => r.Id == request.Id)
				.FirstOrDefaultAsync();

			return room;
		}
	}
}
