using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.AssetManagement.Commands.DeleteAsset;
using Planner.Application.AssetManagement.Commands.InsertAsset;
using Planner.Application.AssetManagement.Commands.InsertAssetGroup;
using Planner.Application.AssetManagement.Commands.InsertAssetGroupActions;
using Planner.Application.AssetManagement.Commands.UpdateAsset;
using Planner.Application.AssetManagement.Commands.UpdateAssetGroup;
using Planner.Application.AssetManagement.Commands.UpdateAssetGroupActions;
using Planner.Application.AssetManagement.Queries.GetAssetAvailabilityAndUsage;
using Planner.Application.AssetManagement.Queries.GetAssetDetails;
using Planner.Application.AssetManagement.Queries.GetAssetGroupActions;
using Planner.Application.AssetManagement.Queries.GetAssetRoomAssignments;
using Planner.Application.AssetManagement.Queries.GetAssetTags;
using Planner.Application.AssetManagement.Queries.GetPageOfAssets;
using Planner.Application.AssetManagement.Queries.GetSystemDefinedAssetActions;
using Planner.Common.Data;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
    public class AssetManagementController : BaseController
    {
        [HttpPost]
        public async Task<AssetDetailsData> GetAssetDetails(GetAssetDetailsQuery request)
        {
            return await this.Mediator.Send(request);
        }

        [HttpPost]
        public async Task<IEnumerable<AssetGroupAvailability>> GetAssetAvailabilityAndUsage(GetAssetAvailabilityAndUsageQuery request)
        {
            return await this.Mediator.Send(request);
        }

        [HttpPost]
        public async Task<IEnumerable<TagItemData>> GetAssetTags(GetAssetTagsQuery request)
        {
            return await this.Mediator.Send(request);
        }

        [HttpPost]
        public async Task<PageOf<AssetGridItemData>> GetPageOfAssets(GetPageOfAssetsQuery request)
        {
            return await this.Mediator.Send(request);
        }

        [HttpPost]
        //[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Assets)]
        public async Task<InsertAssetResponse> InsertAsset(InsertAssetCommand request)
        {
            return await this.Mediator.Send(request);
        }

        [HttpPost]
        //[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Assets)]
        public async Task<ProcessResponse<Guid>> InsertAssetGroup(InsertAssetGroupCommand request)
        {
            return await this.Mediator.Send(request);
        }

        [HttpPost]
        //[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Assets)]
        public async Task<UpdateAssetResponse> UpdateAsset(UpdateAssetCommand request)
        {
            return await this.Mediator.Send(request);
        }

        [HttpPost]
        //[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Assets)]
        public async Task<ProcessResponse> UpdateAssetGroup(UpdateAssetGroupCommand request)
        {
            return await this.Mediator.Send(request);
        }

        [HttpPost]
        //[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Assets)]
        public async Task<ProcessResponse> DeleteAsset(DeleteAssetCommand request)
        {
            return await this.Mediator.Send(request);
        }

        [HttpPost]
        public async Task<AssetActionData[]> GetAssetGroupActions(GetAssetGroupActionsQuery request)
        {
            return await this.Mediator.Send(request);
        }

        [HttpPost]
        //[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Assets)]
        public async Task<ProcessResponse<AssetActionData[]>> InsertAssetGroupActions(InsertAssetGroupActionsCommand request)
        {
            return await this.Mediator.Send(request);
        }

        [HttpPost]
        //[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Assets)]
        public async Task<ProcessResponse<AssetActionData[]>> UpdateAssetGroupActions(UpdateAssetGroupActionsCommand request)
        {
            return await this.Mediator.Send(request);
        }

        [HttpPost]
        public async Task<AssetRoomAssignmentsViewModel> GetAssetRoomAssignments(GetAssetRoomAssignmentsQuery request)
        {
            return await this.Mediator.Send(request);
        }

        [HttpPost]
        public async Task<IEnumerable<SystemDefinedAssetAction>> GetSystemDefinedAssetActions(GetSystemDefinedAssetActionsQuery request)
        {
            return await this.Mediator.Send(request);
        }
    }
}
