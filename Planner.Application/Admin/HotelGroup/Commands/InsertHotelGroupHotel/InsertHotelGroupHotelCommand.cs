using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.HotelGroup.Commands.InsertHotelGroupHotel
{
	public class SaveHotelGroupHotelData
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string WindowsTimeZoneId { get; set; }
	}

	public class InsertHotelGroupHotelCommand : SaveHotelGroupHotelData, IRequest<ProcessResponse>
	{
	}
	public class InsertHotelGroupHotelCommandHandler : IRequestHandler<InsertHotelGroupHotelCommand, ProcessResponse>, IAmAdminApplicationHandler
	{
		private IDatabaseContext _databaseContext;

		public InsertHotelGroupHotelCommandHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<ProcessResponse> Handle(InsertHotelGroupHotelCommand request, CancellationToken cancellationToken)
		{
			var ianaTimeZoneId = TimeZoneConverter.TZConvert.WindowsToIana(request.WindowsTimeZoneId);

			var hotel = new Domain.Entities.Hotel
			{
				CreatedAt = DateTime.UtcNow,
				IanaTimeZoneId = ianaTimeZoneId,
				WindowsTimeZoneId = request.WindowsTimeZoneId,
				Id = request.Id,
				ModifiedAt = DateTime.UtcNow,
				Name = request.Name,
			};

			var createdByUser = await this._databaseContext.Users.FirstOrDefaultAsync(u => u.UserName == "rcadmin");
			if(createdByUser == null)
			{
				createdByUser = await this._databaseContext.Users.FirstOrDefaultAsync();
			}

			if(createdByUser == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "The hotel group has no users. You have to have at least one user before you can create hotels.",
				};
			}

			var hotelSettings = new Domain.Entities.Settings
			{
				AllowPostponeCleanings = true,
				CreatedAt = DateTime.UtcNow,
				CreatedById = createdByUser.Id,
				ModifiedAt = DateTime.UtcNow,
				ModifiedById = createdByUser.Id,
				DefaultAttendantEndTime = "16:00",
				DefaultAttendantStartTime = "08:00",
				DefaultAttendantMaxCredits = null,
				DefaultCheckInTime = "14:00",
				DefaultCheckOutTime = "10:00",
				EmailAddressesForSendingPlan = null,
				FromEmailAddress = null,
				Id = Guid.NewGuid(),
				HotelId = hotel.Id,
				ReserveBetweenCleanings = 5,
				SendPlanToAttendantsByEmail = false,
				ShowCleaningDelays = false,
				ShowHoursInWorkerPlanner = false,
				TravelReserve = 10,
				UseGroups = false,
				UseOrderInPlanning = false,
			};

			await this._databaseContext.Hotels.AddAsync(hotel);
			await this._databaseContext.Settings.AddAsync(hotelSettings);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Hotel created"
			};
		}
	}
}
