using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.AssetManagement.Queries.GetAssetDetails
{
	public class AssetDetailsGroupData
	{
		public Guid Id { get; set; }
		public Guid? ParentAssetGroupId { get; set; }
		public string Name { get; set; }
		public string TypeKey { get; set; }
		public IEnumerable<AssetDetailsSubGroupData> SubGroups { get; set; }
		public IEnumerable<AssetData> Assets { get; set; }
	}

	public class AssetDetailsSubGroupData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public IEnumerable<AssetData> Assets { get; set; }
	}

	public class AssetData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public bool IsBulk { get; set; }
		public string SerialNumber { get; set; }
		public IEnumerable<AssetDetailsTagData> Tags { get; set; }
		//public IEnumerable<AssetDetailsFileData> Files { get; set; }
		public AssetDetailsFileData ImageFileData { get; set; }
		public AssetDetailsFileData QrCodeFileData { get; set; }

	}
	public class AssetDetailsData
	{
		//// 1. SIMPLE - Asset group is just a dummy group for the sake of consistency.
		//// 2. SIMPLE_TRACKS_SERIALS  - Asset group has assets and empty subgroups, assets must track serial numbers.
		//// 3. GROUPED - Asset group has assets and empty subgroups.
		//// 4. GROUPED_TRACKS_SERIALS - asset group has empty assets and subgroups. Subgroups have assets that must track serial numbers.
		//public string TypeKey { get; set; }
		public AssetDetailsGroupData AssetGroup { get; set; }
	}

	public class AssetDetailsModelData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Quantity { get; set; }
	}

	public class AssetDetailsFileData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }
		public bool IsImage { get; set; }
		public string Extension { get; set; }

		public bool IsPrimaryImage { get; set; }
		public bool IsQrCodeImage { get; set; }
	}
	public class AssetDetailsTagData
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}

	public class GetAssetDetailsQuery : IRequest<AssetDetailsData>
	{
		public Guid? AssetId { get; set; }
		public Guid? AssetGroupId { get; set; }
	}
	public class GetAssetDetailsQueryHandler : IRequestHandler<GetAssetDetailsQuery, AssetDetailsData>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		private readonly IFileService _fileService;

		public GetAssetDetailsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, IFileService fileService)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
			this._fileService = fileService;
		}

		public async Task<AssetDetailsData> Handle(GetAssetDetailsQuery request, CancellationToken cancellationToken)
		{
			if (request.AssetGroupId.HasValue && request.AssetGroupId.Value != Guid.Empty)
			{
				return await this._LoadAssetGroup(request.AssetGroupId.Value);
			}
			else if (request.AssetId.HasValue && request.AssetId.Value != Guid.Empty)
			{
				return await this._LoadSingleAsset(request.AssetId.Value, cancellationToken);
			}
			else
			{
				return null;
			}
		}

		private async Task<Dictionary<Guid, List<AssetDetailsFileData>>> _LoadAssetFiles(IEnumerable<Domain.Entities.AssetFile> assetFiles)
		{
			var fileIds = assetFiles.Select(af => af.FileId).Distinct().ToArray();
			var files = await this._databaseContext.Files.Where(f => fileIds.Contains(f.Id)).Select(f => new
			{
				Id = f.Id,
				Name = f.FileName
			}).ToListAsync();

			var assetFilesMap = new Dictionary<Guid, List<AssetDetailsFileData>>();
			foreach (var f in files)
			{
				var fileTypeData = this._fileService.DetermineFileType(f.Name);
				var af = assetFiles.Where(a => a.FileId == f.Id).ToArray();

				foreach(var assetFile in af)
				{
					var filesData = (List<AssetDetailsFileData>)null;
					if (assetFilesMap.ContainsKey(assetFile.AssetId))
					{
						filesData = assetFilesMap[assetFile.AssetId];
					}
					else
					{
						filesData = new List<AssetDetailsFileData>();
						assetFilesMap.Add(assetFile.AssetId, filesData);
					}

					filesData.Add(new AssetDetailsFileData
					{
						Id = f.Id,
						Name = f.Name,
						Extension = fileTypeData.Extension,
						IsImage = fileTypeData.FileType == FileTypes.IMAGE,
						Url = this._fileService.GetAssetFileUrl(assetFile.AssetId, f.Name),
						IsPrimaryImage = assetFile.IsPrimaryImage,
						IsQrCodeImage = assetFile.IsQrCodeImage,
					});
				}
			}

			return assetFilesMap;
		}

		private async Task<AssetDetailsData> _LoadSingleAsset(Guid assetId, CancellationToken cancellationToken)
		{
			var asset = await this._databaseContext
				.Assets
				.Where(a => a.Id == assetId)
				.FirstOrDefaultAsync();

			if (asset == null)
			{
				return null;
			}

			// TODO: REMOVE THIS if statement AT SOME POINT IN THE FUTURE
			// WARNING! This if is used only to "fix" the missing data after migrations.
			// WARNING! This if is used only to "fix" the missing data after migrations.
			// WARNING! This if is used only to "fix" the missing data after migrations.
			if (!asset.AssetGroupId.HasValue)
			{
				var newAssetGroup = new Domain.Entities.AssetGroup 
				{ 
					Id = Guid.NewGuid(),
					CreatedAt = DateTime.UtcNow,
					CreatedById = this._userId,
					ModifiedAt = DateTime.UtcNow,
					ModifiedById = this._userId,
					Name = asset.Name,
					TypeKey = "SIMPLE",	
				};

				await this._databaseContext.AssetGroups.AddAsync(newAssetGroup, cancellationToken);
				asset.AssetGroupId = newAssetGroup.Id;
				await this._databaseContext.SaveChangesAsync(cancellationToken);
			}

			return await this._LoadAssetGroup(asset.AssetGroupId.Value);
		}
		//private async Task<AssetDetailsData> _LoadSingleAsset(Guid assetId)
		//{
		//	var asset = await this._databaseContext
		//		.Assets
		//		.Include(a => a.AssetFiles)
		//		.Include(a => a.AssetTags)
		//		.Where(a => a.Id == assetId)
		//		.FirstOrDefaultAsync();

		//	if(asset == null)
		//	{
		//		return null;
		//	}

		//	if (asset.AssetGroupId.HasValue)
		//	{
		//		return await this._LoadAssetGroup(asset.AssetGroupId.Value);
		//	}

		//	var assetFilesMap = await this._LoadAssetFiles(asset.AssetFiles);

		//	return new AssetDetailsData 
		//	{
		//		AssetGroup = new AssetDetailsGroupData
		//		{
		//			Id = Guid.Empty,
		//			IsTrackingAssetsBySerialNumber = false,
		//			UsesModels = false,
		//			Name = "N/A",
		//			Assets = new List<AssetData> 
		//			{ 
		//				new AssetData 
		//				{ 
		//					Id = asset.Id,
		//					Name = asset.Name,
		//					SerialNumber = asset.SerialNumber,
		//					Files = assetFilesMap.ContainsKey(asset.Id) ? assetFilesMap[asset.Id] : new List<AssetDetailsFileData>(),
		//					Tags = asset.AssetTags.Select(at => new AssetDetailsTagData
		//					{
		//						Key = at.TagKey,
		//						Value = at.TagKey
		//					}).ToArray(),                       
		//				}
		//			},
		//		},
		//		TypeKey = AssetType.SIMPLE.ToString()
		//	};
		//}

		private async Task<AssetDetailsData> _LoadAssetGroup(Guid assetGroupId)
		{
			var assetGroup = await this._databaseContext
				.AssetGroups
				.Include(ag => ag.ChildAssetGroups)
				.Where(ag => ag.Id == assetGroupId)
				.FirstOrDefaultAsync();

			var childAssetGroupsMap = assetGroup.ChildAssetGroups.ToDictionary(ag => ag.Id);

			if(assetGroup == null)
			{
				return null;
			}


			var assets = await this._databaseContext
				.Assets
				.Include(a => a.AssetFiles.Where(af => af.IsPrimaryImage == true || af.IsQrCodeImage == true))
				.Include(a => a.AssetTags)
				.Where(a => a.AssetGroupId != null && a.AssetGroupId == assetGroupId)
				.ToArrayAsync();

			var assetFiles = assets.SelectMany(a => a.AssetFiles).ToArray();
			var assetFilesMap = await this._LoadAssetFiles(assetFiles);

			var assetDetails = new AssetDetailsData
			{
				AssetGroup = new AssetDetailsGroupData
				{
					Id = assetGroup.Id,
					ParentAssetGroupId = assetGroup.ParentAssetGroupId,
					Name = assetGroup.Name,
					TypeKey = assetGroup.TypeKey,
					Assets = new AssetData[0],
					SubGroups = new AssetDetailsSubGroupData[0]
				}
			};

			assetDetails.AssetGroup.Assets = assets.Where(a => !a.AssetSubGroupId.HasValue).Select(asset => new AssetData
			{
				Id = asset.Id,
				Name = asset.Name,
				SerialNumber = asset.SerialNumber,
				IsBulk = asset.IsBulk,
				ImageFileData = assetFilesMap.ContainsKey(asset.Id) ? assetFilesMap[asset.Id].FirstOrDefault(i => i.IsPrimaryImage) : null,
				QrCodeFileData = assetFilesMap.ContainsKey(asset.Id) ? assetFilesMap[asset.Id].FirstOrDefault(i => i.IsQrCodeImage) : null,
				Tags = asset.AssetTags.Select(at => new AssetDetailsTagData
				{
					Key = at.TagKey,
					Value = at.TagKey
				}).ToArray(),
			}).ToArray();
			
			var groupedAssetsMap = assets.Where(a => a.AssetSubGroupId.HasValue).GroupBy(a => a.AssetSubGroupId.Value).ToDictionary(a => a.Key, a => a.ToArray());
			var subGroups = new List<AssetDetailsSubGroupData>();
			assetDetails.AssetGroup.SubGroups = subGroups;

			foreach(var subGroup in assetGroup.ChildAssetGroups)
			{
				var subGroupAssets = groupedAssetsMap.ContainsKey(subGroup.Id) ? groupedAssetsMap[subGroup.Id] : new Domain.Entities.Asset[0];
				subGroups.Add(new AssetDetailsSubGroupData
				{
					Id = subGroup.Id,
					Name = subGroup.Name,
					Assets = subGroupAssets.Select(asset => new AssetData
					{
						Id = asset.Id,
						Name = asset.Name,
						SerialNumber = asset.SerialNumber,
						IsBulk = asset.IsBulk,
						ImageFileData = assetFilesMap.ContainsKey(asset.Id) ? assetFilesMap[asset.Id].FirstOrDefault(i => i.IsPrimaryImage) : null,
						QrCodeFileData = assetFilesMap.ContainsKey(asset.Id) ? assetFilesMap[asset.Id].FirstOrDefault(i => i.IsQrCodeImage) : null,
						Tags = asset.AssetTags.Select(at => new AssetDetailsTagData
						{
							Key = at.TagKey,
							Value = at.TagKey
						}).ToArray(),
					}).ToArray()
				});
			}
			

			return assetDetails;
		}

	}
}
