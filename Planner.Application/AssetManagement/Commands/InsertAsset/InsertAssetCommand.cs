using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.AssetManagement.Commands.InsertAsset
{
	public class InsertAssetResponse : ProcessResponse
	{
		public Guid AssetId { get; set; }
		public Guid AssetGroupId { get; set; }
		public Guid? QrCodeFileId { get; set; }
		public Guid? PrimaryImageFileId { get; set; }
	}

	public class InsertAssetCommand : IRequest<InsertAssetResponse>
	{
		public bool IsSimpleAsset { get; set; }

		public Guid? AssetGroupId { get; set; }
		public Guid? AssetSubGroupId { get; set; }
		public string Name { get; set; }
		public bool IsBulk { get; set; }
		public string SerialNumber { get; set; }
		public IEnumerable<InsertAssetTagData> Tags { get; set; }
		public InsertAssetFileData QrCodeFile { get; set; }
		public InsertAssetFileData PrimaryImageFile { get; set; }
	}

	public class InsertAssetTagData
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}

	public class InsertAssetFileData
	{
		public string FileName { get; set; }
	}

	public class InsertAssetCommandHandler : IRequestHandler<InsertAssetCommand, InsertAssetResponse>, IAmWebApplicationHandler
	{
		private readonly IFileService _fileService;
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public InsertAssetCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, IFileService fileService)
		{
			this._fileService = fileService;
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<InsertAssetResponse> Handle(InsertAssetCommand request, CancellationToken cancellationToken)
		{
			var tagKeys = request.Tags.Select(t => t.Key).ToArray();

			var existingTagsSet = (await this._databaseContext
					.Tags
					.Where(t => tagKeys.Contains(t.Key))
					.ToListAsync())
				.Select(t => t.Key.ToLower())
				.ToHashSet();

			var tagsToInsert = new List<Tag>();
			foreach(var tag in request.Tags)
			{
				if (!existingTagsSet.Contains(tag.Key.ToLower()))
				{
					tagsToInsert.Add(new Tag
					{
						Key = tag.Key,
						Value = tag.Value,
						CreatedAt = DateTime.UtcNow,
						CreatedById = this._userId,
						ModifiedAt = DateTime.UtcNow,
						ModifiedById = this._userId
					});
				}
			}
			var assetId = Guid.NewGuid();
			var storageDirectory = this._fileService.GetAssetFileStoragePath(assetId);
			if (!Directory.Exists(storageDirectory)) Directory.CreateDirectory(storageDirectory);

			var filesToInsert = new List<Domain.Entities.File>();
			var assetFilesToInsert = new List<Domain.Entities.AssetFile>();
			var primaryImageFile = (Domain.Entities.File)null;
			var qrCodeFile = (Domain.Entities.File)null;

			if (request.PrimaryImageFile != null)
			{
				var processResult = await this._fileService.ProcessNewAssetFile(assetId, request.PrimaryImageFile.FileName);
				var fileId = Guid.NewGuid();

				primaryImageFile = new Domain.Entities.File
				{
					CreatedAt = DateTime.UtcNow,
					CreatedById = this._userId,
					FileData = await System.IO.File.ReadAllBytesAsync(processResult.FilePath),
					FileName = request.PrimaryImageFile.FileName,
					Id = fileId,
					ModifiedAt = DateTime.UtcNow,
					ModifiedById = this._userId,
					FileTypeKey = "ASSET_PRIMARY_IMAGE",
				};

				filesToInsert.Add(primaryImageFile);

				assetFilesToInsert.Add(new Domain.Entities.AssetFile
				{
					AssetId = assetId,
					FileId = fileId,
					IsPrimaryImage = true,
					IsQrCodeImage = false,
				});
			}

			if (request.QrCodeFile != null)
			{
				var processResult = await this._fileService.ProcessNewAssetFile(assetId, request.QrCodeFile.FileName);
				var fileId = Guid.NewGuid();

				qrCodeFile = new Domain.Entities.File
				{
					CreatedAt = DateTime.UtcNow,
					CreatedById = this._userId,
					FileData = await System.IO.File.ReadAllBytesAsync(processResult.FilePath),
					FileName = request.QrCodeFile.FileName,
					Id = fileId,
					ModifiedAt = DateTime.UtcNow,
					ModifiedById = this._userId,
					FileTypeKey = "ASSET_QR_CODE",
				};

				filesToInsert.Add(qrCodeFile);

				assetFilesToInsert.Add(new Domain.Entities.AssetFile
				{
					AssetId = assetId,
					FileId = fileId,
					IsPrimaryImage = false,
					IsQrCodeImage = true,
				});
			}

			var assetTagsToInsert = request.Tags.Select(t => new AssetTag 
			{ 
				AssetId = assetId,
				TagKey = t.Key
			});


			var asset = new Asset
			{
				Id = assetId,
				CreatedAt = DateTime.UtcNow,
				CreatedById = this._userId,
				ModifiedById = this._userId,
				ModifiedAt = DateTime.UtcNow,
				Name = request.Name,
				AssetGroupId = request.AssetGroupId,
				AssetSubGroupId = request.AssetSubGroupId,
				SerialNumber = request.SerialNumber,
				IsBulk = request.IsBulk,
			};

			var assetGroup = (Domain.Entities.AssetGroup)null;

			// Only simple assets get an autogenerated asset group
			if (request.IsSimpleAsset)
			{
				assetGroup = new AssetGroup
				{
					CreatedAt = DateTime.UtcNow,
					CreatedById = this._userId,
					Id = Guid.NewGuid(),
					ModifiedAt = DateTime.UtcNow,
					ModifiedById = this._userId,
					Name = request.Name,
					TypeKey = AssetGroupType.SIMPLE.ToString(),
				};

				asset.AssetGroupId = assetGroup.Id;
			}

			using (var transaction = await this._databaseContext.Database.BeginTransactionAsync())
			{
				if (tagsToInsert.Any())
				{
					await this._databaseContext.Tags.AddRangeAsync(tagsToInsert);
				}

				if (filesToInsert.Any())
				{
					await this._databaseContext.Files.AddRangeAsync(filesToInsert);
				}

				if(assetGroup != null)
				{
					await this._databaseContext.AssetGroups.AddAsync(assetGroup);
				}

				await this._databaseContext.Assets.AddAsync(asset);

				if (assetTagsToInsert.Any())
				{
					await this._databaseContext.AssetTags.AddRangeAsync(assetTagsToInsert);
				}

				if (assetFilesToInsert.Any())
				{
					await this._databaseContext.AssetFiles.AddRangeAsync(assetFilesToInsert);
				}

				await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);
			}

			return new InsertAssetResponse
			{
				AssetGroupId = asset.AssetGroupId.Value,
				AssetId = asset.Id,
				PrimaryImageFileId = primaryImageFile == null ? null : primaryImageFile.Id, 
				QrCodeFileId = qrCodeFile == null ? null : qrCodeFile.Id, 
				HasError = false,
				IsSuccess = true,
				Message = "Asset inserted"
			};
		}
	}
}
