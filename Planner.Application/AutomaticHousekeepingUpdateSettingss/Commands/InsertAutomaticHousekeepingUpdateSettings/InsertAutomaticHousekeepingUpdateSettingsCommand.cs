using System;
using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Domain.Entities;
using Planner.Application.AutomaticHousekeepingUpdateSettingss.Models;
using Planner.Common.Data;
using Planner.Application.Admin;

namespace Planner.Application.AutomaticHousekeepingUpdateSettingss.Commands.InsertAutomaticHousekeepingUpdateSettings
{
	public class InsertAutomaticHousekeepingUpdateSettingsCommand: SaveAutomaticHousekeepingUpdateSettings, IRequest<ProcessResponse<Guid>>
	{
	}

	public class InsertAutomaticHousekeepingUpdateSettingsCommandHandler: IRequestHandler<InsertAutomaticHousekeepingUpdateSettingsCommand, ProcessResponse<Guid>>, IAmAdminApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public InsertAutomaticHousekeepingUpdateSettingsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse<Guid>> Handle(InsertAutomaticHousekeepingUpdateSettingsCommand request, CancellationToken cancellationToken)
		{
			var _automaticHousekeepingUpdateSettings = new Planner.Domain.Entities.AutomaticHousekeepingUpdateSettings
			{
				Id = Guid.NewGuid(),
				Dirty = request.Dirty,
				Clean = request.Clean,
				CleanNeedsInspection = request.CleanNeedsInspection,
				Inspected = request.Inspected,
				Vacant = request.Vacant,
				Occupied = request.Occupied,
				DoNotDisturb = request.DoNotDisturb,
				DoDisturb = request.DoDisturb,
				OutOfService = request.OutOfService,
				InService = request.InService,
				RoomNameRegex = request.RoomNameRegex,
				UpdateStatusTo = request.UpdateStatusTo,
				UpdateStatusWhen = request.UpdateStatusWhen,
				UpdateStatusAtTime = request.UpdateStatusAtTime,
				HotelId = request.HotelId,
			};

			await this._databaseContext.AutomaticHousekeepingUpdateSettingss.AddAsync(_automaticHousekeepingUpdateSettings, cancellationToken);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse<Guid>
			{
				Data = _automaticHousekeepingUpdateSettings.Id,
				IsSuccess = true,
				Message = "Inserted."
			};
		}
	}
}
