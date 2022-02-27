using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.AssetManagement.Commands.InsertAssetGroupActions
{
	public class AssetActionData
	{
		public Guid AssetGroupId { get; set; }
		public Guid Id { get; set; }
		public string Name { get; set; }
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

	public class InsertAssetGroupActionItem
	{
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

	public class InsertAssetGroupActionsCommand : IRequest<ProcessResponse<AssetActionData[]>>
	{
		public Guid AssetGroupId { get; set; }
		public IEnumerable<InsertAssetGroupActionItem> Actions { get; set; }
	}

	public class InsertAssetGroupActionsCommandHandler : IRequestHandler<InsertAssetGroupActionsCommand, ProcessResponse<AssetActionData[]>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public InsertAssetGroupActionsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<AssetActionData[]>> Handle(InsertAssetGroupActionsCommand request, CancellationToken cancellationToken)
		{
			var assetActions = request.Actions.Select(a => new AssetAction
			{
				Id = Guid.NewGuid(),
				CreatedAt = DateTime.UtcNow,
				CreatedById = this._userId,
				ModifiedAt = DateTime.UtcNow,
				ModifiedById = this._userId,
				Name = a.Name,
				AssetGroupId = request.AssetGroupId,
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
			}).ToArray();

			await this._databaseContext.AssetActions.AddRangeAsync(assetActions);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse<AssetActionData[]>
			{
				Data = assetActions.Select(aa => new AssetActionData
				{
					Id = aa.Id,
					Name = aa.Name,
					AssetGroupId = aa.AssetGroupId,
					QuickOrTimedKey = aa.QuickOrTimedKey,
					DefaultAssignedToUserId = aa.DefaultAssignedToUserId,
					Credits = aa.Credits,
					IsSystemDefined = aa.IsSystemDefined,
					Price = aa.Price,
					PriorityKey = aa.PriorityKey,
					DefaultAssignedToUserGroupId = aa.DefaultAssignedToUserGroupId,
					DefaultAssignedToUserSubGroupId = aa.DefaultAssignedToUserSubGroupId,
					SystemActionTypeKey = aa.SystemActionTypeKey,
					SystemDefinedActionIdentifierKey = aa.SystemDefinedActionIdentifierKey,
				}).ToArray(),
				HasError = false,
				IsSuccess = true,
				Message = "Actions inserted."
			};
		}
	}
}
