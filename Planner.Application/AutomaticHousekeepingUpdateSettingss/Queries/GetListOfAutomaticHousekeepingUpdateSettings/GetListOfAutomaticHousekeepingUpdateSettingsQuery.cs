using System;
using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Planner.Application.AutomaticHousekeepingUpdateSettingss.Models;
using Planner.Application.Interfaces;
using Planner.Application.Admin;

namespace Ostermann.Application.AutomaticHousekeepingUpdateSettingss.Queries.GetListOfAutomaticHousekeepingUpdateSettings
{
	public class GetListOfAutomaticHousekeepingUpdateSettingsQuery : IRequest<IEnumerable<AutomaticHousekeepingUpdateSettingsListItem>>
	{
		public string HotelId { get; set; }
	}

	public class GetListOfAutomaticHousekeepingUpdateSettingsQueryHandler : IRequestHandler<GetListOfAutomaticHousekeepingUpdateSettingsQuery, IEnumerable<AutomaticHousekeepingUpdateSettingsListItem>>, IAmAdminApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public GetListOfAutomaticHousekeepingUpdateSettingsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<IEnumerable<AutomaticHousekeepingUpdateSettingsListItem>> Handle(GetListOfAutomaticHousekeepingUpdateSettingsQuery request, CancellationToken cancellationToken)
		{
			return await this._databaseContext.AutomaticHousekeepingUpdateSettingss
				.Where(au => au.HotelId == request.HotelId)
				.OrderBy(x => x.CreatedAt)
				.Select(x => new AutomaticHousekeepingUpdateSettingsListItem
				{
					Id = x.Id,
					Dirty = x.Dirty,
					Clean = x.Clean,
					CleanNeedsInspection = x.CleanNeedsInspection,
					Inspected = x.Inspected,
					Vacant = x.Vacant,
					Occupied = x.Occupied,
					DoNotDisturb = x.DoNotDisturb,
					DoDisturb = x.DoDisturb,
					OutOfService = x.OutOfService,
					InService = x.InService,
					RoomNameRegex = x.RoomNameRegex,
					UpdateStatusTo = x.UpdateStatusTo,
					UpdateStatusWhen = x.UpdateStatusWhen,
					UpdateStatusAtTime = x.UpdateStatusAtTime,
				}
			).ToArrayAsync();
		}
	}
}
