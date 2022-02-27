using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.FloorAffinities.Queries.GetListOfFloorAffinities
{

	//public class FloorAffinityData
	//{
	//	public string BuildingId { get; set; }
	//	public string BuildingName { get; set; }
	//	public string FloorId { get; set; }
	//	public string FloorName { get; set; }
	//	public int FloorNumber { get; set; }
	//}

	//public class GetListOfFloorAffinitiesQuery : IRequest<IEnumerable<FloorAffinityData>>
	//{
	//	public string HotelId { get; set; }
	//}

	//public class GetListOfFloorAffinitiesQueryHandler : IRequestHandler<GetListOfFloorAffinitiesQuery, IEnumerable<FloorAffinityData>>, IAmWebApplicationHandler
	//{
	//	private readonly IDatabaseContext _databaseContext;

	//	public GetListOfFloorAffinitiesQueryHandler(IDatabaseContext databaseContext)
	//	{
	//		this._databaseContext = databaseContext;
	//	}

	//	public async Task<IEnumerable<FloorAffinityData>> Handle(GetListOfFloorAffinitiesQuery request, CancellationToken cancellationToken)
	//	{
	//		return await this._databaseContext
	//			.Floors
	//			.Where(f => f.Building.HotelId == request.HotelId)
	//			.Select(f => new FloorAffinityData
	//			{
	//				BuildingId = f.BuildingId.ToString(),
	//				BuildingName = f.Building.Name,
	//				FloorId = f.Id.ToString(),
	//				FloorName = f.Name,
	//				FloorNumber = f.Number
	//			})
	//			.ToArrayAsync();
	//	}
	//}
}
