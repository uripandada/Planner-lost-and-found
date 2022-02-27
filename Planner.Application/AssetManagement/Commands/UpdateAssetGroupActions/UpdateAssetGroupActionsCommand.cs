using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.AssetManagement.Commands.InsertAssetGroupActions;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.AssetManagement.Commands.UpdateAssetGroupActions
{
	public class UpdateAssetGroupActionItem
	{
		public Guid? Id { get; set; }
		public string Name { get; set; }
		//public bool IsSystemDefined { get; set; }
		public string QuickOrTimedKey { get; set; }
		public string PriorityKey { get; set; }
		public Guid? DefaultAssignedToUserId { get; set; }
		public Guid? DefaultAssignedToUserGroupId { get; set; }
		public Guid? DefaultAssignedToUserSubGroupId { get; set; }
		public int? Credits { get; set; }
		public decimal? Price { get; set; }

		public bool IsSystemDefined { get; set; }
		public string SystemActionTypeKey { get; set; }
		public string SystemDefinedActionIdentifierKey { get; set; }
	}
	public class UpdateAssetGroupActionsCommand : IRequest<ProcessResponse<AssetActionData[]>>
	{
		public Guid AssetGroupId { get; set; }
		public IEnumerable<UpdateAssetGroupActionItem> Actions { get; set; }
	}
	public class UpdateAssetGroupActionsCommandHandler : IRequestHandler<UpdateAssetGroupActionsCommand, ProcessResponse<AssetActionData[]>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public UpdateAssetGroupActionsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<AssetActionData[]>> Handle(UpdateAssetGroupActionsCommand request, CancellationToken cancellationToken)
		{
			var existingActions = (IEnumerable<AssetAction>)null;

			existingActions = await this._databaseContext.AssetActions.Where(aa => aa.AssetGroupId == request.AssetGroupId).ToArrayAsync();

			var actionsToInsert = new List<AssetAction>();
			var actionsToUpdate = new List<AssetAction>();
			var actionsToDelete = new List<AssetAction>();

			foreach (var existingAction in existingActions)
			{
				var updatedAction = request.Actions.Where(a => a.Id.HasValue && a.Id.Value == existingAction.Id).FirstOrDefault();

				if (updatedAction == null)
				{
					actionsToDelete.Add(existingAction);
				}
				else
				{
					existingAction.ModifiedAt = DateTime.UtcNow;
					existingAction.ModifiedById = this._userId;
					existingAction.Name = updatedAction.Name;
					existingAction.Credits = updatedAction.Credits;
					existingAction.DefaultAssignedToUserId = updatedAction.DefaultAssignedToUserId;
					existingAction.Price = updatedAction.Price;
					existingAction.PriorityKey = updatedAction.PriorityKey;
					existingAction.QuickOrTimedKey = updatedAction.QuickOrTimedKey;
					existingAction.DefaultAssignedToUserGroupId = updatedAction.DefaultAssignedToUserGroupId;
					existingAction.DefaultAssignedToUserSubGroupId = updatedAction.DefaultAssignedToUserSubGroupId;
					existingAction.IsSystemDefined = updatedAction.IsSystemDefined;
					existingAction.SystemActionTypeKey = updatedAction.SystemActionTypeKey;
					existingAction.SystemDefinedActionIdentifierKey = updatedAction.SystemDefinedActionIdentifierKey;

					actionsToUpdate.Add(existingAction);
				}
			}

			foreach (var newAction in request.Actions.Where(a => !a.Id.HasValue))
			{
				var action = new AssetAction
				{
					Id = Guid.NewGuid(),
					AssetGroupId = request.AssetGroupId,
					CreatedAt = DateTime.UtcNow,
					CreatedById = this._userId,
					ModifiedAt = DateTime.UtcNow,
					ModifiedById = this._userId,
					Name = newAction.Name,
					QuickOrTimedKey = newAction.QuickOrTimedKey,
					DefaultAssignedToUserId = newAction.DefaultAssignedToUserId,
					Credits = newAction.Credits,
					IsSystemDefined = newAction.IsSystemDefined,
					SystemActionTypeKey = newAction.SystemActionTypeKey,
					Price = newAction.Price,
					PriorityKey = newAction.PriorityKey,
					DefaultAssignedToUserGroupId = newAction.DefaultAssignedToUserGroupId,
					DefaultAssignedToUserSubGroupId = newAction.DefaultAssignedToUserSubGroupId,
					SystemDefinedActionIdentifierKey = newAction.SystemDefinedActionIdentifierKey,
				};
				actionsToInsert.Add(action);

				newAction.Id = action.Id; // WARNING: THIS IS A SMALL HACK SO IT IS EASIER TO BUILD A RESPONSE. REQUEST PARAMETERS SHOULDN'T BE MODIFIED!!!!!!!
			}

			if (actionsToInsert.Any())
			{
				await this._databaseContext.AssetActions.AddRangeAsync(actionsToInsert);
			}
			if (actionsToDelete.Any())
			{
				this._databaseContext.AssetActions.RemoveRange(actionsToDelete);
			}

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse<AssetActionData[]>
			{
				Data = request.Actions.Select(a => new AssetActionData
				{
					Id = a.Id.Value,
					AssetGroupId = request.AssetGroupId,
					Name = a.Name,
					QuickOrTimedKey = a.QuickOrTimedKey,
					DefaultAssignedToUserId = a.DefaultAssignedToUserId,
					Credits = a.Credits,
					Price = a.Price,
					PriorityKey = a.PriorityKey,
					DefaultAssignedToUserGroupId = a.DefaultAssignedToUserGroupId,
					DefaultAssignedToUserSubGroupId = a.DefaultAssignedToUserSubGroupId, 
					IsSystemDefined = a.IsSystemDefined,
					SystemActionTypeKey = a.SystemActionTypeKey,
					SystemDefinedActionIdentifierKey = a.SystemDefinedActionIdentifierKey,
				}).ToArray(),
				HasError = false,
				IsSuccess = true,
				Message = "Actions updated."
			};
		}
	}
}
