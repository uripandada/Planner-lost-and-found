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

namespace Planner.Application.AutomaticHousekeepingUpdateSettingss.Commands.UpdateAutomaticHousekeepingUpdateSettings
{
	public class UpdateAutomaticHousekeepingUpdateSettingsCommand: SaveAutomaticHousekeepingUpdateSettings, IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
	}

	public class UpdateAutomaticHousekeepingUpdateSettingsCommandHandler: IRequestHandler<UpdateAutomaticHousekeepingUpdateSettingsCommand, ProcessResponse>, IAmAdminApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UpdateAutomaticHousekeepingUpdateSettingsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse> Handle(UpdateAutomaticHousekeepingUpdateSettingsCommand request, CancellationToken cancellationToken)
		{
			var _automaticHousekeepingUpdateSettings = await this._databaseContext.AutomaticHousekeepingUpdateSettingss.FindAsync(request.Id);

			if (_automaticHousekeepingUpdateSettings == null)
			{
				return new ProcessResponse
				{
					IsSuccess = false,
					Message = "Entity doesn't exist.",
				};
			}

			_automaticHousekeepingUpdateSettings.Dirty = request.Dirty;
			_automaticHousekeepingUpdateSettings.Clean = request.Clean;
			_automaticHousekeepingUpdateSettings.CleanNeedsInspection = request.CleanNeedsInspection;
			_automaticHousekeepingUpdateSettings.Inspected = request.Inspected;
			_automaticHousekeepingUpdateSettings.Vacant = request.Vacant;
			_automaticHousekeepingUpdateSettings.Occupied = request.Occupied;
			_automaticHousekeepingUpdateSettings.DoNotDisturb = request.DoNotDisturb;
			_automaticHousekeepingUpdateSettings.DoDisturb = request.DoDisturb;
			_automaticHousekeepingUpdateSettings.OutOfService = request.OutOfService;
			_automaticHousekeepingUpdateSettings.InService = request.InService;
			_automaticHousekeepingUpdateSettings.RoomNameRegex = request.RoomNameRegex;
			_automaticHousekeepingUpdateSettings.UpdateStatusTo = request.UpdateStatusTo;
			_automaticHousekeepingUpdateSettings.UpdateStatusWhen = request.UpdateStatusWhen;
			_automaticHousekeepingUpdateSettings.UpdateStatusAtTime = request.UpdateStatusAtTime;

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				IsSuccess = true,
				Message = "Updated.",
			};
		}
	}
}
