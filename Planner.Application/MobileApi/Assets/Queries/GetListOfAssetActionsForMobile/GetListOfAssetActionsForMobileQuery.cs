using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.MobileApi.Assets.Queries.GetListOfAssetsForMobile;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Assets.Queries.GetListOfAssetActionsForMobile
{
	public class MobileAssetAction
	{
		public Guid Id { get; set; }
		public Guid? AssetGroupId { get; set; }
		public string AssetGroupName { get; set; }
		public string Name { get; set; }
		public string QuickOrTimedKey { get; set; }
		public string PriorityKey { get; set; }
		public Guid? DefaultAssignedToUserId { get; set; }
		public Guid? DefaultAssignedToUserGroupId { get; set; }
		public Guid? DefaultAssignedToUserSubGroupId { get; set; }
		public int? Credits { get; set; }
		public decimal? Price { get; set; }

		public bool IsSystemDefined { get; set; }

		/// <summary>
		/// Described by enum: SystemActionType
		///   LOCATION_CHANGE
		///   NONE
		/// </summary>
		public string SystemActionTypeKey { get; set; }

		/// <summary>
		/// Described by enum: SystemDefinedActionIdentifier
		///   WAREHOUSE_TO_WAREHOUSE,
		///   ROOM_TO_WAREHOUSE,
		///   WAREHOUSE_TO_ROOM,
		///   ROOM_TO_ROOM,
		///   NONE,
		/// </summary>
		public string SystemDefinedActionIdentifierKey { get; set; }
	}

	public class GetListOfAssetActionsForMobileQuery : IRequest<IEnumerable<MobileAssetAction>>
	{

	}

	public class GetListOfAssetActionsForMobileQueryHandler : IRequestHandler<GetListOfAssetActionsForMobileQuery, IEnumerable<MobileAssetAction>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		private readonly IFileService _fileService;

		public GetListOfAssetActionsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, IFileService fileService)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
			this._fileService = fileService;
		}
		public async Task<IEnumerable<MobileAssetAction>> Handle(GetListOfAssetActionsForMobileQuery request, CancellationToken cancellationToken)
		{
			var assetGroups = await this._databaseContext
				   .AssetGroups
				   .ToListAsync();

			var assetActions = (await this._databaseContext
				.AssetActions
				.ToListAsync())
				.GroupBy(aa => aa.AssetGroupId)
				.ToDictionary(group => group.Key, group => group.ToArray());

			var result = new List<MobileAssetAction>();
			foreach (var group in assetGroups)
			{
				var groupActions = assetActions.ContainsKey(group.Id) ? assetActions[group.Id] : new Domain.Entities.AssetAction[0];

				foreach (var a in groupActions)
				{
					result.Add(new MobileAssetAction
					{
						Id = a.Id,
						QuickOrTimedKey = a.QuickOrTimedKey,
						Credits = a.Credits,
						DefaultAssignedToUserGroupId = a.DefaultAssignedToUserGroupId,
						DefaultAssignedToUserId = a.DefaultAssignedToUserId,
						DefaultAssignedToUserSubGroupId = a.DefaultAssignedToUserSubGroupId,
						IsSystemDefined = a.IsSystemDefined,
						Name = a.Name,
						Price = a.Price,
						PriorityKey = a.PriorityKey,
						SystemActionTypeKey = a.SystemActionTypeKey,
						SystemDefinedActionIdentifierKey = a.SystemDefinedActionIdentifierKey,
						AssetGroupId = group.Id,
						AssetGroupName = group.Name,
					});
				}
			}

			return result;
		}
	}
}
