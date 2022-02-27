using MediatR;
using Planner.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.AssetManagement.Queries.GetSystemDefinedAssetActions
{
	public class SystemDefinedAssetActionProvider
	{
		public IEnumerable<SystemDefinedAssetAction> GetList()
		{
			/// WARNING: DO NOT CHANGE THESE VALUES. ONLY ADD NEW IF NECESSARY!
			/// WARNING: DO NOT CHANGE THESE VALUES. ONLY ADD NEW IF NECESSARY!
			/// WARNING: DO NOT CHANGE THESE VALUES. ONLY ADD NEW IF NECESSARY!
			/// WARNING: DO NOT CHANGE THESE VALUES. ONLY ADD NEW IF NECESSARY!
			return new SystemDefinedAssetAction[]
			{
				new SystemDefinedAssetAction
				{
					TypeKey = SystemActionType.LOCATION_CHANGE.ToString(),
					Key = "ROOM_TO_ROOM",
					Name = "Move between rooms"
				},
				new SystemDefinedAssetAction
				{
					TypeKey = SystemActionType.LOCATION_CHANGE.ToString(),
					Key = "WAREHOUSE_TO_WAREHOUSE",
					Name = "Move between warehouses"
				},
				new SystemDefinedAssetAction
				{
					TypeKey = SystemActionType.LOCATION_CHANGE.ToString(),
					Key = "WAREHOUSE_TO_ROOM",
					Name = "Bring to room"
				},
				new SystemDefinedAssetAction
				{
					TypeKey = SystemActionType.LOCATION_CHANGE.ToString(),
					Key = "ROOM_TO_WAREHOUSE",
					Name = "Return to warehouse"
				},
			};
		}
	}

	public class SystemDefinedAssetAction
	{
		public string Key { get; set; }
		public string TypeKey { get; set; }
		public string Name { get; set; }
	}

	public class GetSystemDefinedAssetActionsQuery : IRequest<IEnumerable<SystemDefinedAssetAction>>
	{
	}

	public class GetSystemDefinedAssetActionsQueryHandler : IRequestHandler<GetSystemDefinedAssetActionsQuery, IEnumerable<SystemDefinedAssetAction>>, IAmWebApplicationHandler
	{
		public GetSystemDefinedAssetActionsQueryHandler()
		{

		}

		public async Task<IEnumerable<SystemDefinedAssetAction>> Handle(GetSystemDefinedAssetActionsQuery request, CancellationToken cancellationToken)
		{
			var provider = new SystemDefinedAssetActionProvider();
			return provider.GetList();
		}
	}
}
