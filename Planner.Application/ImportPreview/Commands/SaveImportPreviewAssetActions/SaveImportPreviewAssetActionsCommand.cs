using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.ImportPreview.Commands.UploadImportPreviewAssetActions;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ImportPreview.Commands.SaveImportPreviewAssetActions
{
	public class SaveAssetActionImportResult: ImportAssetActionPreview
	{
		public Guid? ActionId { get; set; }
	}

	public class SaveImportPreviewAssetActionsCommand : IRequest<ProcessResponse<IEnumerable<SaveAssetActionImportResult>>>
	{
		public IEnumerable<ImportAssetActionPreview> AssetActions { get; set; }
	}

	public class SaveImportPreviewAssetActionsCommandHandler : IRequestHandler<SaveImportPreviewAssetActionsCommand, ProcessResponse<IEnumerable<SaveAssetActionImportResult>>>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public SaveImportPreviewAssetActionsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<IEnumerable<SaveAssetActionImportResult>>> Handle(SaveImportPreviewAssetActionsCommand request, CancellationToken cancellationToken)
		{
			var assetGroups = await this._databaseContext.AssetGroups.Include(ag => ag.AssetActions).ToArrayAsync();
			var assetGroupsMap = new Dictionary<string, AssetGroup>();
			foreach (var assetGroup in assetGroups)
			{
				var key = assetGroup.Name.Trim().ToLower();
				if (!assetGroupsMap.ContainsKey(key))
				{
					assetGroupsMap.Add(key, assetGroup);
				}
			}

			var assetActionsToInsert = new List<AssetAction>();
			var assetActionsToUpdate = new List<AssetAction>();
			var results = new List<SaveAssetActionImportResult>();

			foreach(var assetAction in request.AssetActions)
			{
				var assetKey = assetAction.Asset.Trim().ToLower();
				var assetGroup = assetGroupsMap[assetKey];

				var assetActionKey = assetAction.Action.Trim().ToLower();
				var existingAssetAction = assetGroup.AssetActions.FirstOrDefault(aa => aa.Name.Trim().ToLower() == assetActionKey);
				if(existingAssetAction == null)
				{
					existingAssetAction = new AssetAction 
					{ 
						CreatedAt = DateTime.UtcNow,
						CreatedById = this._userId,
						ModifiedAt = DateTime.UtcNow,
						ModifiedById = this._userId,
						AssetGroupId = assetGroup.Id,
						Id = Guid.NewGuid(),
						Credits = assetAction.Credits,
						IsSystemDefined = false,
						Name = assetAction.Action.Trim(),
						Price = assetAction.Price,
						PriorityKey = assetAction.Priority,
						QuickOrTimedKey = assetAction.Type.Trim().ToUpper(),
						SystemActionTypeKey = "NONE",
						SystemDefinedActionIdentifierKey = "NONE",
					};

					assetActionsToInsert.Add(existingAssetAction);
				}
				else
				{
					existingAssetAction.ModifiedAt = DateTime.UtcNow;
					existingAssetAction.ModifiedById = this._userId;
					existingAssetAction.Price = assetAction.Price;
					existingAssetAction.Credits = assetAction.Credits;
					existingAssetAction.QuickOrTimedKey = assetAction.Type;
					existingAssetAction.PriorityKey = assetAction.Priority;
				}

				results.Add(new SaveAssetActionImportResult
				{
					Action = existingAssetAction.Name,
					ActionId = existingAssetAction.Id,
					Asset = assetGroup.Name,
					Credits = existingAssetAction.Credits,
					HasError = false,
					Message = "Asset saved.",
					Price = existingAssetAction.Price,
					Priority = existingAssetAction.PriorityKey,
					Type = existingAssetAction.QuickOrTimedKey,
				});
			}

			using (var transaction = await this._databaseContext.Database.BeginTransactionAsync())
			{
				if (assetActionsToInsert.Any())
				{
					await this._databaseContext.AssetActions.AddRangeAsync(assetActionsToInsert);
				}

				await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);
			}

			if (results.Any(r => r.HasError))
			{
				return new ProcessResponse<IEnumerable<SaveAssetActionImportResult>>()
				{
					Data = results,
					HasError = true,
					IsSuccess = false,
					Message = "Importing asset actions failed."
				};
			}
			else
			{
				return new ProcessResponse<IEnumerable<SaveAssetActionImportResult>>()
				{
					Data = results,
					HasError = false,
					IsSuccess = true,
					Message = "Asset actions imported."
				};
			}
		}
	}
}

