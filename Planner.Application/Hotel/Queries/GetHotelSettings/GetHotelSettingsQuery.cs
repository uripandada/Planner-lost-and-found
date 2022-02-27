using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.Interfaces.Mapping;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Hotel.Queries.GetHotelSettings
{
	public class HotelSettingsPluginData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int OrdinalNumber { get; set; }
	}

	public class HotelSettingsData : IHaveCustomMapping
	{
		public Guid? Id { get; set; }
		public string HotelId { get; set; }
		public string DefaultCheckInTime { get; set; }
		public string DefaultCheckOutTime { get; set; }
		public string DefaultAttendantStartTime { get; set; }
		public string DefaultAttendantEndTime { get; set; }
		public int? DefaultAttendantMaxCredits { get; set; }
		public int? ReserveBetweenCleanings { get; set; }
		public int? TravelReserve { get; set; }
		public bool ShowHoursInWorkerPlanner { get; set; }
		public bool UseOrderInPlanning { get; set; }
		public bool ShowCleaningDelays { get; set; }
		public bool AllowPostponeCleanings { get; set; }

		// THESE THREE PROPERTIES BELOW SHOULD BE GROUPED SOMEHOW
		public string EmailAddressesForSendingPlan { get; set; }
		public bool SendPlanToAttendantsByEmail { get; set; }
		public string FromEmailAddress { get; set; }

		public bool CleanHostelRoomBedsInGroups { get; set; }

		public string WindowsTimeZoneId { get; set; }
		public string IanaTimeZoneId { get; set; }

		public IEnumerable<HotelSettingsPluginData> Plugins { get; set; }

		public bool BuildingsDistanceMatrixExists { get; set; }
		public bool LevelsDistanceMatrixExists { get; set; }

		public int BuildingAward { get; set; }
		public int LevelAward { get; set; }
		public int RoomAward { get; set; }

		public int LevelTime { get; set; }
		public int CleaningTime { get; set; }

		public int WeightLevelChange { get; set; }
		public int WeightCredits { get; set; }
		public decimal MinutesPerCredit { get; set; }
		public int MinCreditsForMultipleCleanersCleaning { get; set; }

		public void CreateMappings(Profile configuration)
		{
			configuration.CreateMap<Domain.Entities.Settings, HotelSettingsData>();
		}
	}

	public class GetHotelSettingsQuery : IRequest<ProcessResponse<HotelSettingsData>>
	{
		public string HotelId { get; set; }
	}

	public class GetHotelSettingsQueryHandler : IRequestHandler<GetHotelSettingsQuery, ProcessResponse<HotelSettingsData>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private Guid _userId;

		public GetHotelSettingsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<HotelSettingsData>> Handle(GetHotelSettingsQuery request, CancellationToken cancellationToken)
		{
			using(var transaction = await this._databaseContext.Database.BeginTransactionAsync(cancellationToken))
			{
				var settings = await this._databaseContext.Settings
			   .Where(x => x.HotelId == request.HotelId)
			   .Select(s => new HotelSettingsData
			   {
				   AllowPostponeCleanings = s.AllowPostponeCleanings,
				   DefaultAttendantEndTime = s.DefaultAttendantEndTime,
				   DefaultAttendantMaxCredits = s.DefaultAttendantMaxCredits,
				   DefaultAttendantStartTime = s.DefaultAttendantStartTime,
				   DefaultCheckInTime = s.DefaultCheckInTime,
				   DefaultCheckOutTime = s.DefaultCheckOutTime,
				   EmailAddressesForSendingPlan = s.EmailAddressesForSendingPlan,
				   FromEmailAddress = s.FromEmailAddress,
				   HotelId = s.HotelId,
				   Id = s.Id,
				   ReserveBetweenCleanings = s.ReserveBetweenCleanings,
				   SendPlanToAttendantsByEmail = s.SendPlanToAttendantsByEmail,
				   ShowCleaningDelays = s.ShowCleaningDelays,
				   ShowHoursInWorkerPlanner = s.ShowHoursInWorkerPlanner,
				   TravelReserve = s.TravelReserve,
				   UseOrderInPlanning = s.UseOrderInPlanning,
				   IanaTimeZoneId = s.Hotel.IanaTimeZoneId,
				   WindowsTimeZoneId = s.Hotel.WindowsTimeZoneId,
				   CleanHostelRoomBedsInGroups = s.CleanHostelRoomBedsInGroups,
				   BuildingAward = s.BuildingAward,
				   CleaningTime = s.CleaningTime,
				   LevelTime = s.LevelTime,
				   RoomAward = s.RoomAward,
				   LevelAward = s.LevelAward,
				   LevelsDistanceMatrixExists = s.LevelsDistanceMatrix != null,
				   BuildingsDistanceMatrixExists = s.BuildingsDistanceMatrix != null,

				   MinCreditsForMultipleCleanersCleaning = s.MinCreditsForMultipleCleanersCleaning,
				   MinutesPerCredit = s.MinutesPerCredit,
				   WeightCredits = s.WeightCredits,
				   WeightLevelChange = s.WeightLevelChange,

				   Plugins = s.Hotel.CleaningPlugins.Select(cp => new HotelSettingsPluginData
				   {
					   Id = cp.Id,
					   Name = cp.Name,
					   OrdinalNumber = cp.OrdinalNumber
				   })
				   .OrderBy(cp => cp.OrdinalNumber).ToArray()
			   })
			   .SingleOrDefaultAsync();

				if (settings == null)
				{
					var dateProvider = new HotelLocalDateProvider();
					var dateTime = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, request.HotelId, true);

					var hotel = await this._databaseContext.Hotels.FindAsync(request.HotelId);
					var hotelPlugins = await this._databaseContext.CleaningPlugins
						.Where(cp => cp.HotelId == request.HotelId)
						.Select(cp => new HotelSettingsPluginData
						{
							Id = cp.Id,
							Name = cp.Name,
							OrdinalNumber = cp.OrdinalNumber
						})
						.OrderBy(cp => cp.OrdinalNumber)
						.ToArrayAsync();

					// TODO: This should be extracted to some kind of default values provider
					settings = new HotelSettingsData
					{
						Id = Guid.NewGuid(),
						HotelId = request.HotelId,
						WindowsTimeZoneId = hotel.WindowsTimeZoneId,
						IanaTimeZoneId = hotel.IanaTimeZoneId,
						Plugins = hotelPlugins,
						DefaultAttendantStartTime = "08:00",
						DefaultAttendantEndTime = "20:00",
						DefaultAttendantMaxCredits = null,
						AllowPostponeCleanings = false,
						DefaultCheckInTime = "14:00",
						DefaultCheckOutTime = "10:00",
						EmailAddressesForSendingPlan = null,
						FromEmailAddress = null,
						SendPlanToAttendantsByEmail = false,
						ShowCleaningDelays = false,
						ShowHoursInWorkerPlanner = false,
						ReserveBetweenCleanings = null,
						TravelReserve = null,
						UseOrderInPlanning = false,
						CleanHostelRoomBedsInGroups = false,
						BuildingAward = 0,
						LevelAward = 0,
						RoomAward = 0,
						LevelTime = 0,
						CleaningTime = 0,
						BuildingsDistanceMatrixExists = false,
						LevelsDistanceMatrixExists = false,
						MinCreditsForMultipleCleanersCleaning = 0,
						WeightLevelChange = 0,
						WeightCredits = 0,
						MinutesPerCredit = 0,
					};


					var newSettings = new Domain.Entities.Settings
					{
						Id = settings.Id.Value,
						CleaningTime = settings.CleaningTime,
						LevelTime = settings.LevelTime,
						AllowPostponeCleanings = settings.AllowPostponeCleanings,
						BuildingAward = settings.BuildingAward,
						BuildingsDistanceMatrix = null,
						CleanHostelRoomBedsInGroups = settings.CleanHostelRoomBedsInGroups,
						DefaultAttendantEndTime = settings.DefaultAttendantEndTime,
						DefaultAttendantMaxCredits = settings.DefaultAttendantMaxCredits,
						DefaultAttendantStartTime = settings.DefaultAttendantStartTime,
						DefaultCheckInTime = settings.DefaultCheckInTime,
						DefaultCheckOutTime = settings.DefaultCheckOutTime,
						EmailAddressesForSendingPlan = settings.EmailAddressesForSendingPlan,
						FromEmailAddress = settings.FromEmailAddress,
						HotelId = settings.HotelId,
						LevelAward = settings.LevelAward,
						CreatedAt = dateTime,
						CreatedById = this._userId,
						LevelsDistanceMatrix = null,
						ModifiedAt = dateTime,
						ModifiedById = this._userId,
						ReserveBetweenCleanings = settings.ReserveBetweenCleanings,
						SendPlanToAttendantsByEmail = settings.SendPlanToAttendantsByEmail,
						RoomAward = settings.RoomAward,
						ShowCleaningDelays = settings.ShowCleaningDelays,
						ShowHoursInWorkerPlanner = settings.ShowHoursInWorkerPlanner,
						TravelReserve = settings.TravelReserve,
						UseGroups = false,
						UseOrderInPlanning = settings.UseOrderInPlanning,
						MinutesPerCredit = settings.MinutesPerCredit,
						WeightCredits = settings.WeightCredits,
						WeightLevelChange = settings.WeightLevelChange,
						MinCreditsForMultipleCleanersCleaning = settings.MinCreditsForMultipleCleanersCleaning,
					};

					await this._databaseContext.Settings.AddAsync(newSettings);
					await this._databaseContext.SaveChangesAsync(cancellationToken);
					await transaction.CommitAsync(cancellationToken);
				}

				return new ProcessResponse<HotelSettingsData>()
				{
					Data = settings,
					IsSuccess = true
				};
			}
		}
	}
}
