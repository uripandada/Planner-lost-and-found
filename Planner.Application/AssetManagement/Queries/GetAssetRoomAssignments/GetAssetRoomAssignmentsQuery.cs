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

namespace Planner.Application.AssetManagement.Queries.GetAssetRoomAssignments
{
	public class AssetRoomAssignmentsViewModel
	{
		public Guid AssetId { get; set; }
		public Guid? AssetModelId { get; set; }
		public string HotelId { get; set; }
		public IEnumerable<AssetRoomAssignmentsGroupViewModel> BuildingGroups { get; set; }
	}
	public class AssetRoomAssignmentsGroupData
	{
		public Guid? BuildingId { get; set; }
		public string BuildingName { get; set; }
		public Guid RoomId { get; set; }
		public string RoomName { get; set; }
		//public int Quantity { get; set; }
		//public IEnumerable<AssetRoomAssignmentViewModel> RoomAssignments { get; set; }
	}
	
	public class AssetRoomAssignmentsGroupViewModel
	{
		public Guid? BuildingId { get; set; }
		public string BuildingName { get; set; }
		public IEnumerable<AssetRoomAssignmentViewModel> RoomAssignments { get; set; }
	}

	public class AssetRoomAssignmentViewModel
	{
		public Guid RoomId { get; set; }
		public string RoomName { get; set; }
		public int Quantity { get; set; }
		public bool IsAssignedTo { get; set; }
	}

	public class GetAssetRoomAssignmentsQuery : IRequest<AssetRoomAssignmentsViewModel>
	{
		public Guid AssetId { get; set; }
		public Guid? AssetModelId { get; set; }
		public string HotelId { get; set; }
		public string Keywords { get; set; }
	}

	public class GetAssetRoomAssignmentsQueryHandler : IRequestHandler<GetAssetRoomAssignmentsQuery, AssetRoomAssignmentsViewModel>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetAssetRoomAssignmentsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<AssetRoomAssignmentsViewModel> Handle(GetAssetRoomAssignmentsQuery request, CancellationToken cancellationToken)
		{
			//var dataMap = (await this._databaseContext
			//	.Rooms
			//	.Where(r => r.HotelId == request.HotelId && r.BuildingId.HasValue)
			//	.Select(r => new AssetRoomAssignmentsGroupData
			//	{
			//		BuildingName = r.Building.Name,
			//		BuildingId = r.BuildingId,
			//		RoomId = r.Id,
			//		RoomName = r.Name
			//	})
			//	.ToArrayAsync()
			//	).ToDictionary(r => r.RoomId);

			//var assignmentGroupsMap = dataMap.Values.GroupBy(d => new { d.BuildingId, d.BuildingName })
			//	.ToDictionary(
			//		g => g.Key.BuildingId,
			//		g => {
			//			return new
			//			{
			//				BuildingName = g.Key.BuildingName,
			//				RoomsMap = g.ToDictionary(g => g.RoomId, g => new AssetRoomAssignmentViewModel
			//				{
			//					RoomId = g.RoomId,
			//					Quantity = 0,
			//					RoomName = g.RoomName
			//				})
			//			};
			//		}
			//	);

			//if (request.AssetModelId.HasValue)
			//{
			//	var roomAssignments = await this._databaseContext
			//		.RoomAssetModels
			//		.Where(r => r.AssetModelId == request.AssetModelId.Value && r.Room.HotelId == request.HotelId)
			//		.ToArrayAsync();

			//	foreach(var ra in roomAssignments)
			//	{
			//		var buildingId = dataMap[ra.RoomId].BuildingId;
			//		assignmentGroupsMap[buildingId].RoomsMap[ra.RoomId].Quantity = ra.Quantity;
			//		assignmentGroupsMap[buildingId].RoomsMap[ra.RoomId].IsAssignedTo = true;
			//	}
			//}
			//else
			//{
			//	var roomAssignments = await this._databaseContext
			//		.RoomAssets
			//		.Where(r => r.AssetId == request.AssetId && r.Room.HotelId == request.HotelId)
			//		.ToArrayAsync();

			//	foreach (var ra in roomAssignments)
			//	{
			//		var buildingId = dataMap[ra.RoomId].BuildingId;
			//		assignmentGroupsMap[buildingId].RoomsMap[ra.RoomId].Quantity = ra.Quantity;
			//		assignmentGroupsMap[buildingId].RoomsMap[ra.RoomId].IsAssignedTo = true;
			//	}
			//}

			//var buildingGroups = new List<AssetRoomAssignmentsGroupViewModel>();
			//foreach(var assignmentGroupPair in assignmentGroupsMap)
			//{
			//	var buildingId = assignmentGroupPair.Key;
			//	var buildingGroup = new AssetRoomAssignmentsGroupViewModel
			//	{
			//		BuildingId = buildingId,
			//		BuildingName = assignmentGroupsMap[buildingId].BuildingName,
			//		RoomAssignments = assignmentGroupsMap[buildingId].RoomsMap.Values
			//	};
			//	buildingGroups.Add(buildingGroup);
			//}

			return new AssetRoomAssignmentsViewModel
			{
				AssetId = request.AssetId,
				AssetModelId = request.AssetModelId,
				BuildingGroups = new AssetRoomAssignmentsGroupViewModel[0],
				HotelId = request.HotelId
			};
		}
	}
}
