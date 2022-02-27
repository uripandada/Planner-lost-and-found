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

namespace Planner.Application.AssetManagement.Queries.GetPageOfAssets
{
	public class AssetGridItemData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public bool IsBulk { get; set; }
		public string SerialNumber { get; set; }
		public Guid? AssetGroupId { get; set; }
		public string AssetGroupName { get; set; }
		public Guid? AssetSubGroupId { get; set; }
		public string AssetSubGroupName { get; set; }
		public string[] Tags { get; set; }

		public bool HasImage { get; set; }
		public string FileName { get; set; }
		public string ImageUrl { get; set; }

		public int AvailableQuantity { get; set; }
		public int ReservedQuantity { get; set; }
		public int InUseQuantity { get; set; }
		public int TotalQuantity { get; set; }
	}

	public class GetPageOfAssetsQuery : IRequest<PageOf<AssetGridItemData>>
	{
		public int Skip { get; set; }
		public int Take { get; set; }
		public string SortKey { get; set; }
		public string Keywords { get; set; }
		public Guid? AssetGroupId { get; set; }
	}

	public class GetPageOfAssetsQueryHandler : IRequestHandler<GetPageOfAssetsQuery, PageOf<AssetGridItemData>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IFileService _fileService;
		private readonly Guid _userId;

		public GetPageOfAssetsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, IFileService fileService)
		{
			this._fileService = fileService;
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<PageOf<AssetGridItemData>> Handle(GetPageOfAssetsQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext
				.Assets
				.Include(a => a.AssetFiles)
				.Include(a => a.WarehouseAvailabilities)
				.Include(a => a.RoomUsages)
				.Include(a => a.AssetGroup)
				.Include(a => a.AssetSubGroup)
				.Include(a => a.AssetTags)
				.ThenInclude(att => att.Tag)
				.AsQueryable();

			if (request.Keywords.IsNotNull())
			{
				var keywordsVaue = request.Keywords.ToLower();
				query = query.Where(a => a.Name.ToLower().Contains(keywordsVaue) || a.AssetTags.Any(t => t.Tag.Value.ToLower().Contains(keywordsVaue)));
			}

			if (request.AssetGroupId.HasValue)
			{
				query = query.Where(a => a.AssetGroupId != null && a.AssetGroupId == request.AssetGroupId.Value);
			}

			switch (request.SortKey)
			{
				case "NAME_ASC":
					query = query.OrderBy(a => a.Name);
					break;
				case "NAME_DESC":
					query = query.OrderByDescending(a => a.Name);
					break;
				case "CREATED_AT_ASC":
					query = query.OrderBy(a => a.CreatedAt);
					break;
				case "CREATED_AT_DESC":
					query = query.OrderByDescending(a => a.CreatedAt);
					break;
				default:
					query = query.OrderByDescending(a => a.CreatedAt);
					break;
			}

			var totalNumberOfAssets = 0;

			if (request.Take > 0)
			{
				totalNumberOfAssets = await query.CountAsync();
			}

			if (request.Skip > 0)
			{
				query = query.Skip(request.Skip);
			}


			if (request.Take > 0)
			{
				query = query.Take(request.Take);
			}

			var assets = await query.ToListAsync();

			var mainImageFileIds = assets
				.SelectMany(a => a.AssetFiles)
				.Where(af => af.IsPrimaryImage)
				.Select(af => af.FileId)
				.ToArray();

			var files = (await this._databaseContext.Files
				.Where(f => mainImageFileIds.Contains(f.Id))
				.Select(f => new { 
					Id = f.Id,
					FileName = f.FileName
				})
				.ToListAsync())
				.ToDictionary(f => f.Id);

			// TODO: IMPROVE SETTING MAIN IMAGE URL BY PRESETTING THE VALUE ON THE ASSET UPON ASSET INSERT/UPDATE SO THIS CALCULATION IS NOT REALLY NEEDED.
			// TODO: IMPROVE SETTING MAIN IMAGE URL BY PRESETTING THE VALUE ON THE ASSET UPON ASSET INSERT/UPDATE SO THIS CALCULATION IS NOT REALLY NEEDED.
			// TODO: IMPROVE SETTING MAIN IMAGE URL BY PRESETTING THE VALUE ON THE ASSET UPON ASSET INSERT/UPDATE SO THIS CALCULATION IS NOT REALLY NEEDED.
			var assetsData = new List<AssetGridItemData>();
			foreach(var asset in assets)
			{
				var mainImageAssetFile = asset.AssetFiles.FirstOrDefault(af => af.IsPrimaryImage);
				var mainImageFileName = "";
				var mainImageUrl = "";
				var hasImage = false;

				if(mainImageAssetFile != null && files.ContainsKey(mainImageAssetFile.FileId))
				{
					mainImageFileName = files[mainImageAssetFile.FileId].FileName;
					mainImageUrl = this._fileService.GetAssetFileUrl(asset.Id, mainImageFileName);

					var mainImagePath = this._fileService.GetAssetFileStoragePath(asset.Id, mainImageFileName);
					hasImage = System.IO.File.Exists(mainImagePath);
				}

				var assetGridItem = new AssetGridItemData
				{
					Id = asset.Id,
					FileName = mainImageFileName,
					ImageUrl = mainImageUrl,
					Name = asset.Name,
					HasImage = hasImage,
					AssetGroupId = asset.AssetGroupId,
					AssetGroupName = asset.AssetGroup == null ? null : asset.AssetGroup.Name,
					AssetSubGroupId = asset.AssetSubGroupId,
					AssetSubGroupName = asset.AssetSubGroup == null ? null : asset.AssetSubGroup.Name,
					IsBulk = asset.IsBulk,
					SerialNumber = asset.SerialNumber,
					Tags = asset.AssetTags.Select(at => at.Tag.Value).ToArray(),
					AvailableQuantity = 0,
					ReservedQuantity = 0,
					InUseQuantity = 0,
					TotalQuantity= 0,
				};

				foreach (var availability in asset.WarehouseAvailabilities)
				{
					assetGridItem.AvailableQuantity += availability.Quantity;
					assetGridItem.ReservedQuantity += availability.ReservedQuantity;
				}

				foreach (var usage in asset.RoomUsages)
				{
					assetGridItem.InUseQuantity += usage.Quantity;
				}

				assetGridItem.TotalQuantity = assetGridItem.AvailableQuantity + assetGridItem.ReservedQuantity + assetGridItem.InUseQuantity;

				assetsData.Add(assetGridItem);
			}

			if (request.Take == 0)
			{
				totalNumberOfAssets = assetsData.Count;
			}

			return new PageOf<AssetGridItemData>
			{
				Items = assetsData,
				TotalNumberOfItems = totalNumberOfAssets
			};
		}
	}
}
