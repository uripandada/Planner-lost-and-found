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

namespace Planner.Application.AutomaticHousekeepingUpdateSettingss.Commands.DeleteAutomaticHousekeepingUpdateSettings
{
	public class DeleteAutomaticHousekeepingUpdateSettingsCommand: IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
	}

	public class DeleteAutomaticHousekeepingUpdateSettingsCommandHandler: IRequestHandler<DeleteAutomaticHousekeepingUpdateSettingsCommand, ProcessResponse>, IAmAdminApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public DeleteAutomaticHousekeepingUpdateSettingsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse> Handle(DeleteAutomaticHousekeepingUpdateSettingsCommand request, CancellationToken cancellationToken)
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

			this._databaseContext.AutomaticHousekeepingUpdateSettingss.Remove(_automaticHousekeepingUpdateSettings);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				IsSuccess = true,
				Message = "Deleted.",
			};
		}
	}
}
