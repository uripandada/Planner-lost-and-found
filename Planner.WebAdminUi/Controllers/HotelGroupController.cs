using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Planner.Application.Admin.HotelGroup.Commands.DeleteHotelGroup;
using Planner.Application.Admin.HotelGroup.Commands.DeleteHotelGroupHotel;
using Planner.Application.Admin.HotelGroup.Commands.InsertHotelGroup;
using Planner.Application.Admin.HotelGroup.Commands.InsertHotelGroupHotel;
using Planner.Application.Admin.HotelGroup.Commands.UpdateHotelGroup;
using Planner.Application.Admin.HotelGroup.Commands.UpdateHotelGroupHotel;
using Planner.Application.Admin.HotelGroup.Queries.GetHotelGroupDetails;
using Planner.Application.Admin.HotelGroup.Queries.GetHotelGroups;
using Planner.Application.Admin.HotelGroup.Queries.GetPageOfHotelGroupAssets;
using Planner.Application.Admin.HotelGroup.Queries.GetPageOfHotelGroupHotelReservations;
using Planner.Application.Admin.HotelGroup.Queries.GetPageOfHotelGroupHotelRooms;
using Planner.Application.Admin.HotelGroup.Queries.GetPageOfHotelGroupHotels;
using Planner.Application.Admin.HotelGroup.Queries.GetPageOfHotelGroups;
using Planner.Application.Admin.HotelGroup.Queries.GetPageOfHotelGroupUsers;
using Planner.Application.TimeZones.Queries.GetListOfWindowsTimeZones;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebAdminUi.Controllers
{
	public class HotelGroupController : BaseController
	{
		private IOptions<OperationalStoreOptions> _operationalStoreOptions;

		public HotelGroupController(IOptions<OperationalStoreOptions> operationalStoreOptions)
		{
			this._operationalStoreOptions = operationalStoreOptions;
		}

		[HttpPost]
		public async Task<HotelGroupDetailsData> GetHotelGroupDetails(GetHotelGroupDetailsQuery query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpPost]
		public async Task<HotelGroupGridData[]> GetHotelGroups(GetHotelGroupsQuery query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpPost]
		public async Task<PageOf<HotelGroupAssetData>> GetPageOfHotelGroupAssets(GetPageOfHotelGroupAssetsQuery query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpPost]
		public async Task<PageOf<HotelGroupHotelReservationData>> GetPageOfHotelGroupHotelReservations(GetPageOfHotelGroupHotelReservationsQuery query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpPost]
		public async Task<PageOf<HotelGroupHotelRoomsData>> GetPageOfHotelGroupHotelRooms(GetPageOfHotelGroupHotelRoomsQuery query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpPost]
		public async Task<PageOf<HotelGroupHotelData>> GetPageOfHotelGroupHotels(GetPageOfHotelGroupHotelsQuery query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpPost]
		public async Task<PageOf<HotelGroupData>> GetPageOfHotelGroups(GetPageOfHotelGroupsQuery query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpPost]
		public async Task<PageOf<HotelGroupUserData>> GetPageOfHotelGroupUsers(GetPageOfHotelGroupUsersQuery query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpPost]
		public async Task<ProcessResponse> DeleteHotelGroup(DeleteHotelGroupCommand command)
		{
			return await this.Mediator.Send(command);
		}

		[HttpPost]
		public async Task<ProcessResponse> DeleteHotelGroupHotel(DeleteHotelGroupHotelCommand command)
		{
			return await this.Mediator.Send(command);
		}

		[HttpPost]
		public async Task<ProcessResponse<Guid>> InsertHotelGroup(InsertHotelGroupCommand command)
		{
			if (!this.ModelState.IsValid)
			{
				var result = new ProcessResponse<Guid>();
				this._populateErrorModelState(result, "Invalid hotel group insert data");
				return result;
			}

			return await this.Mediator.Send(command);
		}

		[HttpPost]
		public async Task<ProcessResponse> InsertHotelGroupHotel(InsertHotelGroupHotelCommand command)
		{
			if (!this.ModelState.IsValid)
			{
				var result = new ProcessResponse();
				this._populateErrorModelState(result, "Invalid hotel insert data");
				return result;
			}

			return await this.Mediator.Send(command);
		}

		[HttpPost]
		public async Task<ProcessResponse> UpdateHotelGroup(UpdateHotelGroupCommand command)
		{
			if (!this.ModelState.IsValid)
			{
				var result = new ProcessResponse();
				this._populateErrorModelState(result, "Invalid hotel group update data");
				return result;
			}

			return await this.Mediator.Send(command);
		}

		[HttpPost]
		public async Task<ProcessResponse> UpdateHotelGroupHotel(UpdateHotelGroupHotelCommand command)
		{
			if (!this.ModelState.IsValid)
			{
				var result = new ProcessResponse();
				this._populateErrorModelState(result, "Invalid hotel update data");
				return result;
			}

			return await this.Mediator.Send(command);
		}

		[HttpPost]
		public async Task<IEnumerable<TimeZoneData>> GetListOfWindowsTimeZones(GetListOfWindowsTimeZonesQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
