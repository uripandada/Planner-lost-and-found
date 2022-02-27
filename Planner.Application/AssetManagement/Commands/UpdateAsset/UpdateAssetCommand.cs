using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.AssetManagement.Commands.UpdateAsset
{
	public class UpdateAssetResponse : ProcessResponse
	{
		public Guid? QrCodeFileId { get; set; }
		public Guid? PrimaryImageFileId { get; set; }
	}

	public class UpdateAssetCommand : IRequest<UpdateAssetResponse>
	{
		public Guid Id { get; set; }
		public Guid? AssetGroupId { get; set; }
		public Guid? AssetSubGroupId { get; set; }
		public string Name { get; set; }
		public bool IsBulk { get; set; }
		public string SerialNumber { get; set; }
		public IEnumerable<UpdateAssetTagData> Tags { get; set; }
		public UpdateAssetFileData QrCodeFile { get; set; }
		public UpdateAssetFileData PrimaryImageFile { get; set; }
	}

	public class UpdateAssetTagData
	{
		public string Key { get; set; }
		public string Value { get; set; }

	}

	public class UpdateAssetFileData
	{
		public Guid? Id { get; set; }
		public string FileName { get; set; }
	}

	public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, UpdateAssetResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		private readonly IFileService _fileService;

		public UpdateAssetCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, IFileService fileService)
		{
			this._fileService = fileService;
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<UpdateAssetResponse> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
		{
			var asset = await this._databaseContext.Assets
				.Include(a => a.AssetFiles)
				.Include(a => a.AssetTags)
				.Where(a => a.Id == request.Id)
				.FirstOrDefaultAsync();

			var allTags = (await this._databaseContext.Tags.ToListAsync()).ToDictionary(t => t.Key.ToLower());

			var filesToInsert = new List<Domain.Entities.File>();
			var filesToDelete = new List<Domain.Entities.File>();
			var assetFilesToInsert = new List<Domain.Entities.AssetFile>();
			var assetFilesToDelete = new List<Domain.Entities.AssetFile>();

			var primaryImageFileId = asset.AssetFiles.FirstOrDefault(af => af.IsPrimaryImage)?.FileId;
			var qrCodeFileId = asset.AssetFiles.FirstOrDefault(af => af.IsQrCodeImage)?.FileId;

			if (request.PrimaryImageFile != null)
			{
				if (request.PrimaryImageFile.Id.HasValue && request.PrimaryImageFile.Id.Value != Guid.Empty)
				{
					// UPDATE THE FILE
					var saveFileResult = await this._fileService.ProcessNewAssetFile(request.Id, request.PrimaryImageFile.FileName);
					var file = await this._databaseContext.Files.FirstOrDefaultAsync(f => f.Id == primaryImageFileId);
					file.FileData = await System.IO.File.ReadAllBytesAsync(saveFileResult.FilePath);
					file.FileName = request.PrimaryImageFile.FileName;
					file.ModifiedAt = DateTime.UtcNow;
					file.ModifiedById = this._userId;
				}
				else
				{
					// INSERT THE FILE
					var saveFileResult = await this._fileService.ProcessNewAssetFile(request.Id, request.PrimaryImageFile.FileName);
					primaryImageFileId = Guid.NewGuid();
					filesToInsert.Add(new Domain.Entities.File
					{
						CreatedAt = DateTime.UtcNow,
						CreatedById = this._userId,
						FileData = await System.IO.File.ReadAllBytesAsync(saveFileResult.FilePath),
						FileName = request.PrimaryImageFile.FileName,
						Id = primaryImageFileId.Value,
						ModifiedAt = DateTime.UtcNow,
						ModifiedById = this._userId,
						FileTypeKey = "ASSET_PRIMARY_IMAGE"
					});
					assetFilesToInsert.Add(new Domain.Entities.AssetFile
					{
						AssetId = asset.Id,
						FileId = primaryImageFileId.Value,
						IsPrimaryImage = true,
						IsQrCodeImage = false,
					});
				}
			}
			else
			{
				// DELETE THE FILE IF IT EXISTS
				if (primaryImageFileId.HasValue)
				{
					var file = await this._databaseContext
						.Files
						.FirstOrDefaultAsync(f => f.Id == primaryImageFileId.Value);

					var assetFiles = await this._databaseContext
						.AssetFiles
						.Where(af => af.FileId == primaryImageFileId.Value)
						.ToArrayAsync();

					if(file != null)
					{
						filesToDelete.Add(file);
					}

					if (assetFiles.Any())
					{
						assetFilesToDelete.AddRange(assetFiles);
					}
				}
			}

			if(request.QrCodeFile != null)
			{
				if (request.QrCodeFile.Id.HasValue && request.QrCodeFile.Id.Value != Guid.Empty)
				{
					// UPDATE THE FILE
					var saveFileResult = await this._fileService.ProcessNewQrCodeFile(request.Id, request.QrCodeFile.FileName);
					var file = await this._databaseContext.Files.FirstOrDefaultAsync(f => f.Id == request.QrCodeFile.Id);
					file.FileData = await System.IO.File.ReadAllBytesAsync(saveFileResult.FilePath);
					file.FileName = request.QrCodeFile.FileName;
					file.ModifiedAt = DateTime.UtcNow;
					file.ModifiedById = this._userId;
				}
				else
				{
					// INSERT THE FILE
					var saveFileResult = await this._fileService.ProcessNewQrCodeFile(request.Id, request.QrCodeFile.FileName);
					qrCodeFileId = Guid.NewGuid();
					filesToInsert.Add(new Domain.Entities.File
					{
						CreatedAt = DateTime.UtcNow,
						CreatedById = this._userId,
						FileData = await System.IO.File.ReadAllBytesAsync(saveFileResult.FilePath),
						FileName = request.QrCodeFile.FileName,
						Id = qrCodeFileId.Value,
						ModifiedAt = DateTime.UtcNow,
						ModifiedById = this._userId,
						FileTypeKey = "ASSET_QR_CODE"
					});
					assetFilesToInsert.Add(new Domain.Entities.AssetFile
					{
						AssetId = asset.Id,
						FileId = qrCodeFileId.Value,
						IsPrimaryImage = false,
						IsQrCodeImage = true,
					});
				}
			}
			else
			{
				// DELETE THE FILE IF IT EXISTS
				if (qrCodeFileId.HasValue)
				{
					var file = await this._databaseContext
						.Files
						.FirstOrDefaultAsync(f => f.Id == qrCodeFileId.Value);

					var assetFiles = await this._databaseContext
						.AssetFiles
						.Where(af => af.FileId == qrCodeFileId.Value)
						.ToArrayAsync();

					if(file != null)
					{
						filesToDelete.Add(file);
					}
					if (assetFiles.Any())
					{
						assetFilesToDelete.AddRange(assetFiles);
					}
				}
			}

			var existingAssetTagsMap = asset.AssetTags.ToDictionary(at => at.TagKey.ToLower());
			var tagsToInsert = new List<Domain.Entities.Tag>();
			var assetTagsToInsert = new List<Domain.Entities.AssetTag>();
			var assetTagsToDelete = new List<Domain.Entities.AssetTag>();
			var checkedTagKeys = new HashSet<string>();

			foreach(var t in request.Tags)
			{
				var tagKey = t.Key.ToLower();
				if (existingAssetTagsMap.ContainsKey(tagKey))
				{
					checkedTagKeys.Add(tagKey);
					continue;
				}

				if (!allTags.ContainsKey(tagKey))
				{
					tagsToInsert.Add(new Domain.Entities.Tag
					{
						Key = t.Key,
						Value = t.Value,
						CreatedAt = DateTime.UtcNow,
						CreatedById = this._userId,
						ModifiedAt = DateTime.UtcNow,
						ModifiedById = this._userId
					});
				}

				assetTagsToInsert.Add(new Domain.Entities.AssetTag { AssetId = asset.Id, TagKey = t.Key });
			}

			foreach(var assetTag in existingAssetTagsMap.Values)
			{
				if (!checkedTagKeys.Contains(assetTag.TagKey.ToLower()))
				{
					assetTagsToDelete.Add(assetTag);
				}
			}

			asset.Name = request.Name;
			asset.ModifiedById = this._userId;
			asset.ModifiedAt = DateTime.UtcNow;
			asset.AssetGroupId = request.AssetGroupId;
			asset.AssetSubGroupId = request.AssetSubGroupId;
			asset.SerialNumber = request.SerialNumber;
			asset.IsBulk = request.IsBulk;

			using (var transaction = await this._databaseContext.Database.BeginTransactionAsync())
			{
				if (tagsToInsert.Any())
				{
					await this._databaseContext.Tags.AddRangeAsync(tagsToInsert);
				}
				if (assetTagsToInsert.Any())
				{
					await this._databaseContext.AssetTags.AddRangeAsync(assetTagsToInsert);
				}
				if (assetTagsToDelete.Any())
				{
					this._databaseContext.AssetTags.RemoveRange(assetTagsToDelete);
				}
				if (filesToInsert.Any())
				{
					await this._databaseContext.Files.AddRangeAsync(filesToInsert);
				}
				if (assetFilesToInsert.Any())
				{
					await this._databaseContext.AssetFiles.AddRangeAsync(assetFilesToInsert);
				}
				if (assetFilesToDelete.Any())
				{
					this._databaseContext.AssetFiles.RemoveRange(assetFilesToDelete);
				}
				if (filesToDelete.Any())
				{
					this._databaseContext.Files.RemoveRange(filesToDelete);
				}

				await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);
			}

			return new UpdateAssetResponse
			{
				PrimaryImageFileId = primaryImageFileId,
				QrCodeFileId = qrCodeFileId,
				HasError = false,
				IsSuccess = true,
				Message = "Asset updated."
			};
		}
	}
}
