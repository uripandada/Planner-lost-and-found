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

namespace Planner.Application.WarehouseManagement.Queries.GetListOfWarehouses
{
	public class WarehouseData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public bool IsCentralWarehouse { get; set; }
		public Guid? FloorId { get; set; }
		public string FloorName { get; set; }
		public string HotelId { get; set; }
		public string HotelName { get; set; }
	}

	public class GetListOfWarehousesQuery : IRequest<IEnumerable<WarehouseData>>
	{
		public string HotelId { get; set; }
	}

	public class GetListOfWarehousesQueryHandler : IRequestHandler<GetListOfWarehousesQuery, IEnumerable<WarehouseData>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetListOfWarehousesQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<IEnumerable<WarehouseData>> Handle(GetListOfWarehousesQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.Warehouses.AsQueryable();

			if (request.HotelId.IsNotNull())
			{
				query = query.Where(w => w.HotelId != null && w.HotelId == request.HotelId);
			}

			return await query
				.Select(w => new WarehouseData
				{
					FloorId = w.FloorId,
					Id = w.Id,
					FloorName = w.Floor.Name,
					Name = w.Name,
					HotelId = w.HotelId,
					HotelName = w.Hotel.Name,
					IsCentralWarehouse = w.IsCentral
				})
				.ToArrayAsync();
		}
	}

}
