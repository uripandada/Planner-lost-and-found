using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.RoomManagement.Commands.AssignRoomsToFloor;
using Planner.Application.RoomManagement.Commands.DeleteBuilding;
using Planner.Application.RoomManagement.Commands.DeleteFloor;
using Planner.Application.RoomManagement.Commands.DeleteRoom;
using Planner.Application.RoomManagement.Commands.InsertBuilding;
using Planner.Application.RoomManagement.Commands.InsertFloor;
using Planner.Application.RoomManagement.Commands.InsertRoom;
using Planner.Application.RoomManagement.Commands.UpdateBuilding;
using Planner.Application.RoomManagement.Commands.UpdateFloor;
using Planner.Application.RoomManagement.Commands.UpdateIsCleaningPriority;
using Planner.Application.RoomManagement.Commands.UpdateIsGuestCurrentlyIn;
using Planner.Application.RoomManagement.Commands.UpdateRoom;
using Planner.Application.RoomManagement.Queries.GetBuildingSimple;
using Planner.Application.RoomManagement.Queries.GetFullRoomHierarchy;
using Planner.Application.RoomManagement.Queries.GetRoomDetails;
using Planner.Application.RoomManagement.Queries.GetRoomHistory;
using Planner.Application.RoomManagement.Queries.GetUnassignedRooms;
using Planner.Common.Data;
using Planner.Domain.Values;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class RoomManagementController : BaseController
	{
		[HttpPost]
		public async Task<BuildingSimpleData> GetBuildingSimple(GetBuildingSimpleQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<RoomDetailsData> GetRoomDetails(GetRoomDetailsQuery request)
		{
			return await this.Mediator.Send(request);
		}
		
		[HttpPost]
		public async Task<IEnumerable<RoomHistoryItem>> GetRoomHistory(GetRoomHistoryQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<FullRoomHierarchyData> GetFullRoomHierarchy(GetFullRoomHierarchyQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Rooms)]
		public async Task<ProcessResponse<InsertBuildingResponse>> InsertBuilding(InsertBuildingCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Rooms)]
		public async Task<ProcessResponse<UpdateBuildingResponse>> UpdateBuilding(UpdateBuildingCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Rooms)]
		public async Task<ProcessResponse> DeleteBuilding(DeleteBuildingCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Rooms)]
		public async Task<ProcessResponse<InsertFloorResponse>> InsertFloor(InsertFloorCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Rooms)]
		public async Task<ProcessResponse<UpdateFloorResponse>> UpdateFloor(UpdateFloorCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Rooms)]
		public async Task<ProcessResponse> DeleteFloor(DeleteFloorCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Rooms)]
		public async Task<ProcessResponse<InsertRoomResponse>> InsertRoom(InsertRoomCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Rooms)]
		public async Task<ProcessResponse<UpdateRoomResponse>> UpdateRoom(UpdateRoomCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Rooms)]
		public async Task<ProcessResponse> DeleteRoom(DeleteRoomCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<PageOf<UnassignedRoomData>> GetPageOfUnassignedRooms(GetUnassignedRoomsQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Rooms)]
		public async Task<ProcessResponse<RoomAssignmentResult[]>> AssignRoomsToFloor(AssignRoomsToFloorCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Rooms)]
		public async Task<ProcessResponse> UpdateIsGuestCurrentlyIn(UpdateIsGuestCurrentlyInCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.Rooms)]
		public async Task<ProcessResponse> UpdateIsCleaningPriority(UpdateIsCleaningPriorityCommand request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
