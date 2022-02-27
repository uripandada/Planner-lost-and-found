using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.AssetManagement.Commands.InsertAssetGroupActions;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.AssetManagement.Queries.GetAssetGroupActions
{
	public class GetAssetGroupActionsQuery : IRequest<AssetActionData[]>
	{
		public Guid AssetGroupId { get; set; }
	}

	public class GetAssetGroupActionsQueryHandler : IRequestHandler<GetAssetGroupActionsQuery, AssetActionData[]>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetAssetGroupActionsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<AssetActionData[]> Handle(GetAssetGroupActionsQuery request, CancellationToken cancellationToken)
		{
			var assetActionsQuery = this._databaseContext.AssetActions.Where(aa => aa.AssetGroupId == request.AssetGroupId).AsQueryable();

			var items = await assetActionsQuery
				.Select(aa => new AssetActionData
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
					DefaultAssignedToUserSubGroupId = aa.DefaultAssignedToUserSubGroupId,
					DefaultAssignedToUserGroupId = aa.DefaultAssignedToUserGroupId,
					SystemActionTypeKey = aa.SystemActionTypeKey,
					SystemDefinedActionIdentifierKey = aa.SystemDefinedActionIdentifierKey,
				})
				.ToArrayAsync();

			return items;
		}
	}
}
