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

namespace Ostermann.Application.AutomaticHousekeepingUpdateSettingss.Queries.GetAutomaticHousekeepingUpdateSettingsDetails
{
	public class GetAutomaticHousekeepingUpdateSettingsDetailsQuery: IRequest<AutomaticHousekeepingUpdateSettingsDetails>
	{
		public Guid Id { get; set; }
	}

	public class GetAutomaticHousekeepingUpdateSettingsDetailsQueryHandler: IRequestHandler<GetAutomaticHousekeepingUpdateSettingsDetailsQuery, AutomaticHousekeepingUpdateSettingsDetails>, IAmAdminApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public GetAutomaticHousekeepingUpdateSettingsDetailsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<AutomaticHousekeepingUpdateSettingsDetails> Handle(GetAutomaticHousekeepingUpdateSettingsDetailsQuery request, CancellationToken cancellationToken)
		{
			var settings = await this._databaseContext.AutomaticHousekeepingUpdateSettingss.FindAsync(request.Id);

			if (settings == null) throw new Exception($"Unable to find case automatic housekeeing update settings details: {request.Id}");

			return new AutomaticHousekeepingUpdateSettingsDetails
			{
				Id = settings.Id,
				Dirty = settings.Dirty,
				Clean = settings.Clean,
				CleanNeedsInspection = settings.CleanNeedsInspection,
				Inspected = settings.Inspected,
				Vacant = settings.Vacant,
				Occupied = settings.Occupied,
				DoNotDisturb = settings.DoNotDisturb,
				DoDisturb = settings.DoDisturb,
				OutOfService = settings.OutOfService,
				InService = settings.InService,
				RoomNameRegex = settings.RoomNameRegex,
				UpdateStatusTo = settings.UpdateStatusTo,
				UpdateStatusWhen = settings.UpdateStatusWhen,
				UpdateStatusAtTime = settings.UpdateStatusAtTime,
			};
		}
	}
}
