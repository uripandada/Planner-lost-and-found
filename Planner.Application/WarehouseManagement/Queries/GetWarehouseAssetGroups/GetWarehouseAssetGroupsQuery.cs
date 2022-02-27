using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.AssetManagement.Queries.GetAssetAvailabilityAndUsage;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.WarehouseManagement.Queries.GetWarehouseAssetGroups
{
	public class WarehouseAssetGroup
	{
		public bool IsSimple { get; set; }
		public Guid Id { get; set; }
		public string Name { get; set; }
		public List<WarehouseAsset> Assets { get; set; }
	}
	public class WarehouseAsset
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public bool IsBulk { get; set; }
		public string SerialNumber { get; set; }
		public string ImageUrl { get; set; }


		public int AvailableQuantity { get; set; }
		public int ReservedQuantity { get; set; }
		public int TotalQuantity { get; set; }
	}

	public class GetWarehouseAssetGroupsQuery : IRequest<IEnumerable<WarehouseAssetGroup>>
	{
		public Guid WarehouseId { get; set; }
	}

	public class GetWarehouseAssetGroupsQueryHandler : IRequestHandler<GetWarehouseAssetGroupsQuery, IEnumerable<WarehouseAssetGroup>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		private readonly IFileService _fileService;

		public GetWarehouseAssetGroupsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, IFileService fileService)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
			this._fileService = fileService;
		}

		public async Task<IEnumerable<WarehouseAssetGroup>> Handle(GetWarehouseAssetGroupsQuery request, CancellationToken cancellationToken)
		{
			//var availabilities = this._databaseContext
			//	.WarehouseAssetAvailabilities

			//	.AsQueryable();


			var query = this._databaseContext
				.Assets
				.Where(a => a.WarehouseAvailabilities.Any(wa => wa.WarehouseId == request.WarehouseId))
				.AsQueryable();

			var assets = await query.Select(a => new
			{
				Availability = a.WarehouseAvailabilities.Where(wa => wa.WarehouseId == request.WarehouseId).Select(wa => new
				{
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

			var assetGroupsMap = new Dictionary<Guid, WarehouseAssetGroup>();

			foreach (var asset in assets)
			{
				var assetGroupId = asset.AssetGroupId ?? Guid.Empty;
				var assetGroup = (WarehouseAssetGroup)null;
				if (assetGroupsMap.ContainsKey(assetGroupId))
				{
					assetGroup = assetGroupsMap[assetGroupId];
				}
				else
				{
					assetGroup = new WarehouseAssetGroup
					{
						Assets = new List<WarehouseAsset>(),
						Id = assetGroupId,
						IsSimple = asset.AssetGroupTypeKey != "GROUP",
						Name = asset.AssetGroupName ?? "Ungrouped",
					};
					assetGroupsMap.Add(assetGroup.Id, assetGroup);
				}

				var primaryImage = asset.PrimaryImages.FirstOrDefault();
				var primaryImageUrl = (string)null;
				if (primaryImage != null)
				{
					primaryImageUrl = this._fileService.GetAssetFileUrl(asset.AssetId, primaryImage.FileName);
				}

				var availableQuantity = 0;
				var reservedQuantity = 0;
				foreach(var a in asset.Availability)
				{
					availableQuantity += a.AvailableQuantity;
					reservedQuantity += a.ReservedQuantity;
				}

				assetGroup.Assets.Add(new WarehouseAsset 
				{ 
					AvailableQuantity = availableQuantity,
					ReservedQuantity = reservedQuantity,
					Id = asset.AssetId,
					ImageUrl = primaryImageUrl,
					IsBulk = asset.AssetIsBulk,
					Name = asset.AssetName,
					SerialNumber = asset.AssetSerialNumber,
					TotalQuantity = availableQuantity + reservedQuantity,
				});
			}

			return assetGroupsMap.Values.OrderBy(v => v.Name).ToArray();
		}
	}
}
