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

namespace Planner.Application.AssetManagement.Queries.GetAssetAvailabilityAndUsage
{
	public class AssetGroupAvailability
	{
		public bool IsSimple { get; set; }
		public Guid Id { get; set; }
		public string Name { get; set; }
		public List<AssetAvailability> Assets { get; set; }
	}
	public class AssetAvailability
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public bool IsBulk { get; set; }
		public string SerialNumber { get; set; }
		public string ImageUrl { get; set; }


		public List<AssetAvailabilityItem> Availabilities { get; set; }

		public int AvailableQuantity { get; set; }
		public int ReservedQuantity { get; set; }
		public int InUseQuantity { get; set; }
		//public IEnumerable<AssetAvailabilityWarehouse> Warehouses { get; set; }
		//public IEnumerable<AssetAvailabilityRoom> Rooms { get; set; }
	}
	public class AssetAvailabilityItem
	{
		public string HotelId { get; set; }
		public string HotelName { get; set; }
		public Guid? BuildingId { get; set; }
		public string BuildingName { get; set; }
		public Guid? FloorId { get; set; }
		public string FloorName { get; set; }

		public Guid Id { get; set; }
		public string Name { get; set; }
		public string TypeKey { get; set; } // WAREHOUSE, ROOM
		public bool IsCentralWarehouse { get; set; }

		public int AvailableQuantity { get; set; }
		public int ReservedQuantity { get; set; }
		public int InUseQuantity { get; set; }

		//public IEnumerable<AssetAvailabilityWarehouse> Warehouses { get; set; }
		//public IEnumerable<AssetAvailabilityRoom> Rooms { get; set; }
	}

	//public class AssetAvailabilityWarehouse
	//{
	//	public string HotelId { get; set; }
	//	public string HotelName { get; set; }
	//	public Guid? BuildingId { get; set; }
	//	public string BuildingName { get; set; }
	//	public Guid? FloorId { get; set; }
	//	public string FloorName { get; set; }
	//	public Guid WarehouseId { get; set; }
	//	public string WarehouseName { get; set; }
	//	public bool IsCentralWarehouse { get; set; }
	//	public IEnumerable<AssetAvailabilityGroup> AssetGroups { get; set; }
	//}

	//public class AssetAvailabilityRoom
	//{
	//	public string HotelId { get; set; }
	//	public string HotelName { get; set; }
	//	public Guid? BuildingId { get; set; }
	//	public string BuildingName { get; set; }
	//	public Guid? FloorId { get; set; }
	//	public string FloorName { get; set; }
	//	public Guid RoomId { get; set; }
	//	public string RoomName { get; set; }
	//	public IEnumerable<AssetUsageGroup> AssetGroups { get; set; }
	//}

	//public class AssetAvailabilityGroup
	//{
	//	public Guid? AssetGroupId { get; set; }
	//	public string AssetGroupName { get; set; }
	//	public bool IsSimpleGroup { get; set; }
	//	public IEnumerable<AssetAvailabilityData> Availability { get; set; }
	//}

	//public class AssetUsageGroup
	//{
	//	public Guid? AssetGroupId { get; set; }
	//	public string AssetGroupName { get; set; }
	//	public bool IsSimpleGroup { get; set; }
	//	public IEnumerable<AssetUsageData> Usage { get; set; }
	//}

	//public class AssetAvailabilityData
	//{
	//	public Guid Id { get; set; }
	//	public string Name { get; set; }
	//	public bool IsBulk { get; set; }
	//	public string SerialNumber { get; set; }
	//	public string ImageUrl { get; set; }

	//	public int AvailableQuantity { get; set; }
	//	public int ReservedQuantity { get; set; }
	//}

	//public class AssetUsageData
	//{
	//	public Guid Id { get; set; }
	//	public string Name { get; set; }
	//	public bool IsBulk { get; set; }
	//	public string SerialNumber { get; set; }
	//	public string ImageUrl { get; set; }

	//	public int InUseQuantity { get; set; }
	//}

	public class GetAssetAvailabilityAndUsageQuery : IRequest<IEnumerable<AssetGroupAvailability>>
	{
		public Guid? AssetGroupId { get; set; }
		public Guid? AssetId { get; set; }
		//public Guid? WarehouseId { get; set; }
	}

	public class GetAssetAvailabilityAndUsageQueryHandler : IRequestHandler<GetAssetAvailabilityAndUsageQuery, IEnumerable<AssetGroupAvailability>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		private readonly IFileService _fileService;

		public GetAssetAvailabilityAndUsageQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, IFileService fileService)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
			this._fileService = fileService;
		}

		public async Task<IEnumerable<AssetGroupAvailability>> Handle(GetAssetAvailabilityAndUsageQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.Assets.AsQueryable();

			if (request.AssetGroupId.HasValue)
			{
				query = query.Where(a => a.AssetGroupId == request.AssetGroupId.Value);
			}

			if (request.AssetId.HasValue)
			{
				query = query.Where(a => a.Id == request.AssetId.Value);
			}

			var assets = await query.Select(a => new
			{
				Usage = a.RoomUsages.Select(ru => new // request.WarehouseId == null just skips the loading of the usages if the warehouse id is set.
				{
					RoomHotelId = ru.Room.HotelId,
					RoomHotelName = ru.Room.Hotel.Name,
					RoomBuildingId = ru.Room.BuildingId,
					RoomBuildingName = ru.Room.Building == null ? "" : ru.Room.Building.Name,
					RoomFloorId = ru.Room.FloorId,
					RoomFloorName = ru.Room.Floor == null ? "" : ru.Room.Floor.Name,
					RoomId = ru.RoomId,
					RoomName = ru.Room.Name,
					InUseQuantity = ru.Quantity,
				}).ToArray(),
				Availability = a.WarehouseAvailabilities.Select(wa => new
				{
					IsCentralWarehouse = wa.Warehouse.IsCentral,
					WarehouseHotelId = wa.Warehouse.HotelId,
					WarehouseHotelName = wa.Warehouse.Hotel.Name,
					WarehouseBuildingId = wa.Warehouse.Floor == null ? Guid.Empty : wa.Warehouse.Floor.BuildingId,
					WarehouseBuildingName = wa.Warehouse.Floor == null ? "" : wa.Warehouse.Floor.Building.Name,
					WarehouseFloorId = wa.Warehouse.FloorId,
					WarehouseFloorName = wa.Warehouse.Floor == null ? "" : wa.Warehouse.Floor.Name,
					WarehouseId = wa.WarehouseId,
					WarehouseName = wa.Warehouse.Name,
					AvailableQuantity = wa.Quantity,
					ReservedQuantity = wa.ReservedQuantity,
				}).ToArray(),
				PrimaryImages = a.AssetFiles.Where(af => af.IsPrimaryImage).Select(af => new { FileId = af.FileId, FileName = af.File.FileName }).ToArray(),
				AssetId = a.Id,
				AssetIsBulk = a.IsBulk,
				AssetName = a.Name,
				AssetSerialNumber = a.SerialNumber,
				AssetGroupId = a.AssetGroupId,
				AssetGroupName = a.AssetGroup.Name,
				AssetGroupTypeKey = a.AssetGroup.TypeKey,
			}).ToArrayAsync();

			var assetGroupsMap = new Dictionary<Guid, AssetGroupAvailability>();

			foreach(var asset in assets)
			{
				var assetGroupId = asset.AssetGroupId ?? Guid.Empty;
				var assetGroup = (AssetGroupAvailability)null;
				if (assetGroupsMap.ContainsKey(assetGroupId))
				{
					assetGroup = assetGroupsMap[assetGroupId];
				}
				else
				{
					assetGroup = new AssetGroupAvailability
					{
						Assets = new List<AssetAvailability>(),
						Id = assetGroupId,
						IsSimple = asset.AssetGroupTypeKey != "GROUP",
						Name = asset.AssetGroupName ?? "Ungrouped",
					};
					assetGroupsMap.Add(assetGroup.Id, assetGroup);
				}

				var primaryImage = asset.PrimaryImages.FirstOrDefault();
				var primaryImageUrl = (string)null;
				if(primaryImage != null)
				{
					primaryImageUrl = this._fileService.GetAssetFileUrl(asset.AssetId, primaryImage.FileName);
				}

				var assetAvailabilityUsage = new AssetAvailability
				{
					Id = asset.AssetId,
					ImageUrl = primaryImageUrl,
					IsBulk = asset.AssetIsBulk,
					Name = asset.AssetName,
					SerialNumber = asset.AssetSerialNumber,
					Availabilities = new List<AssetAvailabilityItem>(),
					AvailableQuantity = 0,
					InUseQuantity = 0,
					ReservedQuantity = 0,
				};

				assetGroup.Assets.Add(assetAvailabilityUsage);

				foreach (var availability in asset.Availability)
				{
					assetAvailabilityUsage.Availabilities.Add(new AssetAvailabilityItem 
					{ 
						AvailableQuantity = availability.AvailableQuantity,
						ReservedQuantity = availability.ReservedQuantity,
						InUseQuantity = 0,
						BuildingId = availability.WarehouseBuildingId,
						BuildingName = availability.WarehouseBuildingName,
						FloorId = availability.WarehouseFloorId,
						FloorName = availability.WarehouseFloorName,
						HotelId = availability.WarehouseHotelId,
						HotelName = availability.WarehouseHotelName,
						Id = availability.WarehouseId,
						Name = availability.WarehouseName,
						IsCentralWarehouse = availability.IsCentralWarehouse,
						TypeKey = "WAREHOUSE",
					});

					assetAvailabilityUsage.AvailableQuantity += availability.AvailableQuantity;
					assetAvailabilityUsage.ReservedQuantity += availability.ReservedQuantity;
				}
				foreach (var usage in asset.Usage)
				{
					assetAvailabilityUsage.Availabilities.Add(new AssetAvailabilityItem
					{
						AvailableQuantity = 0,
						ReservedQuantity = 0,
						InUseQuantity = usage.InUseQuantity,
						BuildingId = usage.RoomBuildingId,
						BuildingName = usage.RoomBuildingName,
						FloorId = usage.RoomFloorId,
						FloorName = usage.RoomFloorName,
						HotelId = usage.RoomHotelId,
						HotelName = usage.RoomHotelName,
						Id = usage.RoomId,
						Name = usage.RoomName,
						IsCentralWarehouse = false,
						TypeKey = "ROOM",
					});

					assetAvailabilityUsage.InUseQuantity += usage.InUseQuantity;
				}
			}

			return assetGroupsMap.Values.OrderBy(v => v.Name).ToArray();
		}
	}
}
