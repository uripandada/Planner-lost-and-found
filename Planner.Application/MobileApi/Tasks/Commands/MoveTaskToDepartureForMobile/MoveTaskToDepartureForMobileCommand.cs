using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Commands.InsertTaskConfiguration;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Tasks.Commands.MoveTaskToDepartureForMobile
{
	public class MoveTaskToDepartureForMobileCommand : IRequest<ProcessResponseSimple>
	{
		public Guid TaskId { get; set; }
	}

	public class MoveTaskToDepartureForMobileCommandHandler : IRequestHandler<MoveTaskToDepartureForMobileCommand, ProcessResponseSimple>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly ISystemTaskGenerator _systemTaskGenerator;
		private readonly ISystemEventsService _systemEventsService;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;

		public MoveTaskToDepartureForMobileCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemTaskGenerator systemTaskGenerator, ISystemEventsService systemEventsService)
		{
			this._databaseContext = databaseContext;
			this._systemEventsService = systemEventsService;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HotelGroupId();
			this._systemTaskGenerator = systemTaskGenerator;
		}

		public async Task<ProcessResponseSimple> Handle(MoveTaskToDepartureForMobileCommand request, CancellationToken cancellationToken)
		{
			var task = (Domain.Entities.SystemTask)null;

			using (var transaction = await this._databaseContext.Database.BeginTransactionAsync())
			{
				task = await this._databaseContext.SystemTasks
					.Include(t => t.FromHotel)
					.Include(t => t.ToHotel)
					.Include(t => t.Actions)
					.Include(t => t.Messages)
					.Where(t => t.Id == request.TaskId).FirstOrDefaultAsync();

				if (task == null)
				{
					return new ProcessResponseSimple<Guid>
					{
						IsSuccess = false,
						Message = "Unable to find task."
					};
				}

				if (task.StatusKey == TaskStatusType.CANCELLED.ToString())
				{
					return new ProcessResponseSimple<Guid>
					{
						IsSuccess = false,
						Message = "Task is cancelled."
					};
				}
				else if (task.StatusKey == TaskStatusType.FINISHED.ToString() || task.StatusKey == TaskStatusType.VERIFIED.ToString())
				{
					return new ProcessResponseSimple<Guid>
					{
						IsSuccess = false,
						Message = "Task is finished."
					};

				}
				else if (task.StatusKey == TaskStatusType.STARTED.ToString() || task.StatusKey == TaskStatusType.PAUSED.ToString())
				{
					return new ProcessResponseSimple<Guid>
					{
						IsSuccess = false,
						Message = "Task is in progress."
					};
				}

				var oldValue = this._systemTaskGenerator.GenerateTaskHistoryData(task);
				var reservationCheckOutDate = DateTime.UtcNow;

				// If the task if for a reservation, use the check out date of that reservation.
				if (task.ToReservationId.IsNotNull())
				{
					var reservation = await this._databaseContext.Reservations.FindAsync(task.ToReservationId);
					if (!reservation.CheckOut.HasValue)
					{
						return new ProcessResponseSimple<Guid>
						{
							IsSuccess = false,
							Message = "Task reservation doesn't have a check out date."
						};
					}
					reservationCheckOutDate = reservation.CheckOut.Value;
				}
				// Else, if the task is for a room, find an active reservation with the latest check out date.
				else if(task.ToRoomId.HasValue)
				{
					var hotelId = task.ToHotelId;
					var hotel = await this._databaseContext.Hotels.FindAsync(hotelId);
					var timeZoneInfo = HotelLocalDateProvider.GetAvailableTimeZoneInfo(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
					var dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
					var date = dateTime.Date;

					// Find the latest reservation for the room
					var reservation = await this._databaseContext
						.Reservations
						.Where(res =>
							res.RoomId == task.ToRoomId.Value &&
							res.IsActive &&
							res.CheckIn != null &&
							res.CheckIn.Value.Date <= date &&
							res.CheckOut != null &&
							res.CheckOut.Value.Date >= date)
						.OrderByDescending(res => res.CheckOut)
						.FirstOrDefaultAsync();

					if(reservation == null)
					{
						return new ProcessResponseSimple<Guid>
						{
							IsSuccess = false,
							Message = "There are no currently active reseravtions for the task's room."
						};
					}

					reservationCheckOutDate = reservation.CheckOut.Value;
				}

				task.StartsAt = reservationCheckOutDate;
				task.IsManuallyModified = true;
				task.ModifiedAt = DateTime.UtcNow;
				task.ModifiedById = this._userId;

				var newValue = this._systemTaskGenerator.GenerateTaskHistoryData(task);

				var taskHistory = this._systemTaskGenerator.GenerateTaskHistory("ADMIN", "Task moved to departure.", task, oldValue, newValue);

				await this._databaseContext.SystemTaskHistorys.AddAsync(taskHistory);
				await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);
			}

			var taskIds = new Guid[] { task.Id };
			var userIds = new List<Guid>();
			if (task.UserId.HasValue) userIds.Add(task.UserId.Value);

			await this._systemEventsService.TasksChanged(this._hotelGroupId, userIds, taskIds, "Some of your tasks has been moved to departure");

			return new ProcessResponseSimple
			{
				IsSuccess = true,
				Message = "Task moved to departure."
			};
		}
	}
}
