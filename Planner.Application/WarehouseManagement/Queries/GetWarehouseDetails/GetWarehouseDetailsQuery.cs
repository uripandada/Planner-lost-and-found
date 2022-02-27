using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.WarehouseManagement.Queries.GetWarehouseDetails
{
	public class WarehouseDetailsData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public bool IsCentralWarehouse { get; set; }
		public Guid? FloorId { get; set; }
		public string FloorName { get; set; }
		public string HotelId { get; set; }
		public string HotelName { get; set; }
	}

	public class GetWarehouseDetailsQuery : IRequest<WarehouseDetailsData>
	{
		public Guid? Id { get; set; }
		public string HotelId { get; set; }
		public Guid? FloorId { get; set; }
	}
	public class GetWarehouseDetailsQueryHandler : IRequestHandler<GetWarehouseDetailsQuery, WarehouseDetailsData>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetWarehouseDetailsQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<WarehouseDetailsData> Handle(GetWarehouseDetailsQuery request, CancellationToken cancellationToken)
		{
			if (request.Id.HasValue)
			{
				return await this._databaseContext
					.Warehouses
					.Where(w => w.Id == request.Id)
					.Select(w => new WarehouseDetailsData 
					{ 
						FloorId = w.FloorId,
						Id = w.Id,
						FloorName = w.Floor.Name,
						Name = w.Name,
						HotelId = w.HotelId,
						HotelName = w.Hotel.Name,
						IsCentralWarehouse = w.IsCentral
					})
					.FirstOrDefaultAsync();
			}

			var hotelName = (string)null;
			var floorName = (string)null;

			if (request.HotelId.IsNotNull())
			{
				var hotel = await this._databaseContext.Hotels.FindAsync(request.HotelId);
				if(hotel != null)
				{
					hotelName = hotel.Name;
				}
			}

			if (request.FloorId.HasValue)
			{
				var floor = await this._databaseContext.Floors.FindAsync(request.FloorId.Value);
				if(floor != null)
				{
					floorName = floor.Name;
				}
			}

			return new WarehouseDetailsData
			{
				Id = Guid.Empty,
				Name = null,
				FloorId = request.FloorId,
				FloorName = floorName,
				HotelId = request.HotelId,
				HotelName = hotelName,
				IsCentralWarehouse = false
			};
		}
	}
}
