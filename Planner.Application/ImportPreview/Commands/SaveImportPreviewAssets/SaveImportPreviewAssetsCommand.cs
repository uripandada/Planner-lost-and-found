using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.ImportPreview.Commands.UploadImportPreviewAssets;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ImportPreview.Commands.SaveImportPreviewAssets
{
    public class SaveAssetImportResult
    {
        public Guid? Id { get; set; }
        public string AssetName { get; set; }
        public string AssetGroupName { get; set; }
        public bool HasErrors { get; set; }
        public string Message { get; set; }
    }

    public class SaveImportPreviewAssetsCommand : IRequest<ProcessResponse<IEnumerable<SaveAssetImportResult>>>
    {
        public IEnumerable<ImportAssetPreview> Assets { get; set; }
    }

    public class SaveImportPreviewAssetsCommandHandler : IRequestHandler<SaveImportPreviewAssetsCommand, ProcessResponse<IEnumerable<SaveAssetImportResult>>>, IAmWebApplicationHandler
    {
        private IDatabaseContext _databaseContext;
        private readonly Guid _userId;
        
        public SaveImportPreviewAssetsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
        {
            this._databaseContext = databaseContext;
            this._userId = contextAccessor.UserId();
        }
        
        public async Task<ProcessResponse<IEnumerable<SaveAssetImportResult>>> Handle(SaveImportPreviewAssetsCommand request, CancellationToken cancellationToken)
        {
            var results = new List<SaveAssetImportResult>();

            var assetGroups = await this._databaseContext
                .AssetGroups
                .Include(ag => ag.GroupAssets)
                .ThenInclude(a => a.AssetTags)
                .ToListAsync();

            var tags = await this._databaseContext.Tags.ToListAsync();

            var assetGroupsToInsert = new List<AssetGroup>();
            var assetsToInsert = new List<Asset>();
            var assetsToUpdate = new List<Asset>();
            var tagsToInsert = new List<Tag>();
            var assetTagsToInsert = new List<AssetTag>();
            var assetTagsToDelete = new List<AssetTag>();

            var tagKeys = new HashSet<string>();
            foreach(var asset in request.Assets)
			{
                var assetTagsKeys = asset.AssetTags.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach(var key in assetTagsKeys)
				{
					if (!tagKeys.Contains(key))
					{
                        tagKeys.Add(key);
					}
				}
			}

            foreach(var tagKey in tagKeys)
			{
                var existingTag = tags.FirstOrDefault(t => t.Key == tagKey);
                if(existingTag == null)
				{
                    existingTag = new Tag
                    {
                        Key = tagKey,
                        CreatedById = this._userId,
                        CreatedAt = DateTime.UtcNow,
                        ModifiedById = this._userId,
                        ModifiedAt = DateTime.UtcNow,
                        Value = tagKey,
                    };

                    tags.Add(existingTag);
                    tagsToInsert.Add(existingTag);
				}
			}

            foreach(var assetPreview in request.Assets)
			{
                var assetGroup = (AssetGroup)null;
				if (assetPreview.AssetGroupName.IsNotNull())
				{
                    var assetGroupKey = assetPreview.AssetGroupName.Trim().ToLower();
                    assetGroup = assetGroups.FirstOrDefault(ag => ag.Name.Trim().ToLower() == assetGroupKey);

                    if(assetGroup == null)
					{
                        assetGroup = new AssetGroup
                        {
                            Id = Guid.NewGuid(),
                            CreatedById = this._userId,
                            CreatedAt = DateTime.UtcNow,
                            ModifiedById = this._userId,
                            ModifiedAt = DateTime.UtcNow,
                            Name = assetPreview.AssetGroupName.Trim(),
                            ParentAssetGroupId = null,
                            TypeKey = "GROUP",
                            GroupAssets = new List<Asset>(),
                        };

                        assetGroupsToInsert.Add(assetGroup);
                        assetGroups.Add(assetGroup);
					}
				}
				else
				{
                    var assetGroupKey = assetPreview.AssetName.Trim().ToLower();
                    assetGroup = assetGroups.FirstOrDefault(ag => ag.Name.Trim().ToLower() == assetGroupKey);

                    if(assetGroup == null)
					{
                        assetGroup = new AssetGroup
                        {
                            Id = Guid.NewGuid(),
                            CreatedById = this._userId,
                            CreatedAt = DateTime.UtcNow,
                            ModifiedById = this._userId,
                            ModifiedAt = DateTime.UtcNow,
                            Name = assetPreview.AssetName.Trim(),
                            ParentAssetGroupId = null,
                            TypeKey = "SIMPLE",
                            GroupAssets = new List<Asset>(),
                        };

                        assetGroupsToInsert.Add(assetGroup);
                        assetGroups.Add(assetGroup);
                    }
                }

                var assetKey = assetPreview.AssetName.Trim().ToLower();
                var existingAsset = assetGroup.GroupAssets.FirstOrDefault(a => a.Name.Trim().ToLower() == assetKey);

                if(existingAsset == null)
				{
                    var asset = new Asset
                    {
                        Id = Guid.NewGuid(),
                        AssetGroupId = assetGroup == null ? null : assetGroup.Id,
                        AssetSubGroupId = null,
                        CreatedById = this._userId,
                        CreatedAt = DateTime.UtcNow,
                        ModifiedById = this._userId,
                        ModifiedAt = DateTime.UtcNow,
                        IsBulk = assetPreview.IsBulk,
                        Name = assetPreview.AssetName.Trim(),
                        SerialNumber = assetPreview.SerialNumber,
                        AssetTags = new List<AssetTag>(),
                    };

                    assetsToInsert.Add(asset);

                    var assetTagsKeys = assetPreview.AssetTags.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var assetTagKey in assetTagsKeys)
                    {
                        assetTagsToInsert.Add(new AssetTag
                        {
                            AssetId = asset.Id,
                            TagKey = assetTagKey,
                        });
                    }

                    if (assetGroup != null)
                    {
                        var gasts = new List<Asset>(assetGroup.GroupAssets == null ? new Asset[0] : assetGroup.GroupAssets);
                        gasts.Add(existingAsset);
                    }
                }
                else
				{
                    assetsToUpdate.Add(existingAsset);

                    existingAsset.AssetGroupId = assetGroup == null ? null : assetGroup.Id;
                    existingAsset.IsBulk = assetPreview.IsBulk;
                    existingAsset.ModifiedAt = DateTime.UtcNow;
                    existingAsset.ModifiedById = this._userId;
                    existingAsset.SerialNumber = assetPreview.SerialNumber;

                    if (assetGroup != null && !assetGroup.GroupAssets.Any(ga => ga.Name.Trim().ToLower() == existingAsset.Name.Trim().ToLower()))
                    {
                        var gasts = new List<Asset>(assetGroup.GroupAssets == null ? new Asset[0] : assetGroup.GroupAssets);
                        gasts.Add(existingAsset);

                        assetGroup.GroupAssets = gasts;
                    }

                    var assetTagsKeys = assetPreview.AssetTags.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var assetTagKey in assetTagsKeys)
                    {
                        if(existingAsset.AssetTags.Any(at => at.TagKey == assetTagKey))
						{
                            continue;
						}

                        assetTagsToInsert.Add(new AssetTag
                        {
                            AssetId = existingAsset.Id,
                            TagKey = assetTagKey,
                        });
                    }

                    foreach(var existingAssetTag in existingAsset.AssetTags)
					{
						if (assetTagsKeys.Any(t => t == existingAssetTag.TagKey))
						{
                            continue;
						}

                        assetTagsToDelete.Add(existingAssetTag);
					}
                }
            }

			using (var transaction = await this._databaseContext.Database.BeginTransactionAsync())
			{
				if (tagsToInsert.Any())
				{
                    await this._databaseContext.Tags.AddRangeAsync(tagsToInsert);
				}

				if (assetGroupsToInsert.Any())
                {
                    await this._databaseContext.AssetGroups.AddRangeAsync(assetGroupsToInsert);
                }

				if (assetsToInsert.Any())
				{
                    await this._databaseContext.Assets.AddRangeAsync(assetsToInsert);
                }

				if (assetTagsToInsert.Any())
				{
                    await this._databaseContext.AssetTags.AddRangeAsync(assetTagsToInsert);
                }

				if (assetTagsToDelete.Any())
				{
                    this._databaseContext.AssetTags.RemoveRange(assetTagsToDelete);
				}

                await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);
			}


            if (results.Any(r => r.HasErrors))
            {
                return new ProcessResponse<IEnumerable<SaveAssetImportResult>>()
                {
                    Data = results,
                    HasError = true,
                    IsSuccess = false,
                    Message = "Importing assets failed."
                };
            }
            else
            {
                return new ProcessResponse<IEnumerable<SaveAssetImportResult>>()
                {
                    Data = results,
                    HasError = false,
                    IsSuccess = true,
                    Message = "Assets imported."
                };
            }
        }
    }
}

