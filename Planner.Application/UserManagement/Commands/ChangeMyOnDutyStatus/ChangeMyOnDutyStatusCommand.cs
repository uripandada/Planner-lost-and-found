using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Planner.Application.UserManagement.Commands.ChangeMyOnDutyStatus
{
	public class ChangeMyOnDutyStatusCommand: IRequest<ProcessResponse>
	{
		public string HotelId { get; set; }
		public bool IsOnDuty { get; set; }
	}

	public class ChangeMyOnDutyStatusCommandHandler : IRequestHandler<ChangeMyOnDutyStatusCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ISystemEventsService _eventsService;
		private readonly Guid _hotelGroupId;
		private readonly Guid _userId;

		public ChangeMyOnDutyStatusCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor, ISystemEventsService eventsService)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
			this._hotelGroupId = this._httpContextAccessor.HttpContext.User.HotelGroupId();
			this._eventsService = eventsService;
			this._userId = this._httpContextAccessor.UserId();
		}

		public async Task<ProcessResponse> Handle(ChangeMyOnDutyStatusCommand request, CancellationToken cancellationToken)
		{
			var hotelLocalDateProvider = new HotelLocalDateProvider();
			var now = await hotelLocalDateProvider.GetHotelCurrentLocalDate(this._databaseContext, request.HotelId, true);

			var user = await this._databaseContext.Users
				.Where(u => u.Id == this._userId)
				.FirstOrDefaultAsync();

			var isOnDuty = request.IsOnDuty;

			user.IsOnDuty = isOnDuty;
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			if (user.IsOnDuty)
			{
				await this._eventsService.UserCameOnDuty(this._hotelGroupId, user.Id, now);
			}
			else
			{
				await this._eventsService.UserCameOffDuty(this._hotelGroupId, user.Id, now);
			}

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = isOnDuty ? "On duty." : "Off duty.",
			};
		}
	}
}
