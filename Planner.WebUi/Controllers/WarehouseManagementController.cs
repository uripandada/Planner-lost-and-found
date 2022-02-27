using Microsoft.AspNetCore.Mvc;
using Planner.Application.WarehouseManagement.Commands.DeleteWarehouse;
using Planner.Application.WarehouseManagement.Commands.DispatchAssetFromWarehouse;
using Planner.Application.WarehouseManagement.Commands.InsertWarehouse;
using Planner.Application.WarehouseManagement.Commands.ReceiveAssetToWarehouse;
using Planner.Application.WarehouseManagement.Commands.UpdateWarehouse;
using Planner.Application.WarehouseManagement.Queries.GetListOfWarehouses;
using Planner.Application.WarehouseManagement.Queries.GetPageOfWarehouseHistory;
using Planner.Application.WarehouseManagement.Queries.GetPageOfWarehouseInventoryArchives;
using Planner.Application.WarehouseManagement.Queries.GetWarehouseAssetGroups;
using Planner.Application.WarehouseManagement.Queries.GetWarehouseDetails;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class WarehouseManagementController : BaseController
	{
		[HttpPost]
		public async Task<IEnumerable<WarehouseData>> GetListOfWarehouses(GetListOfWarehousesQuery query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpPost]
		public async Task<WarehouseDetailsData> GetWarehouseDetails(GetWarehouseDetailsQuery query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpPost]
		public async Task<ProcessResponse<Guid>> InsertWarehouse(InsertWarehouseCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponse> UpdateWarehouse(UpdateWarehouseCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponse> DeleteWarehouse(DeleteWarehouseCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponse> ReceiveAssetToWarehouse(ReceiveAssetToWarehouseCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponse> DispatchAssetFromWarehouse(DispatchAssetFromWarehouseCommand request)
		{
			return await this.Mediator.Send(request);
		}

		/// <summary>
		/// This method really returns the current asset balance on the specified warehouse.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IEnumerable<WarehouseAssetGroup>> GetWarehouseAssetGroups(GetWarehouseAssetGroupsQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<PageOf<WarehouseInventoryArchiveItem>> GetPageOfWarehouseInventoryArchives(GetPageOfWarehouseInventoryArchivesQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<PageOf<WarehouseHistoryItem>> GetPageOfWarehouseHistory(GetPageOfWarehouseHistoryQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
