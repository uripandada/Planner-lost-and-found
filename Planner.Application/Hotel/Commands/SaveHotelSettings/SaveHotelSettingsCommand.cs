using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Planner.Application.Interfaces;
using Planner.Application.Interfaces.Mapping;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Hotel.Commands.SaveHotelSettings
{
	public class SaveHotelSettingsCommand : IRequest<ProcessResponse<string>>, IHaveCustomMapping
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
			configuration.CreateMap<SaveHotelSettingsCommand, Domain.Entities.Settings>();
		}
	}

	public class SaveHotelSettingsCommandHandler : IRequestHandler<SaveHotelSettingsCommand, ProcessResponse<string>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext databaseContext;
		private readonly IHttpContextAccessor httpContextAccessor;

		public SaveHotelSettingsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this.databaseContext = databaseContext;
			this.httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse<string>> Handle(SaveHotelSettingsCommand request, CancellationToken cancellationToken)
		{
			var settings = await this.databaseContext.Settings.FindAsync(request.Id);

			// TODO: Refactor so the properties are set only once.
			if (settings != null)
			{
				settings.ModifiedById = this.httpContextAccessor.UserId();
				settings.ModifiedAt = DateTime.UtcNow;

				settings.ReserveBetweenCleanings = request.ReserveBetweenCleanings;
				settings.AllowPostponeCleanings = request.AllowPostponeCleanings;
				settings.DefaultAttendantEndTime = request.DefaultAttendantEndTime;
				settings.DefaultAttendantMaxCredits = request.DefaultAttendantMaxCredits;
				settings.DefaultAttendantStartTime = request.DefaultAttendantStartTime;
				settings.DefaultCheckInTime = request.DefaultCheckInTime;
				settings.DefaultCheckOutTime = request.DefaultCheckOutTime;
				settings.EmailAddressesForSendingPlan = request.EmailAddressesForSendingPlan;
				settings.FromEmailAddress = request.FromEmailAddress;
				settings.SendPlanToAttendantsByEmail = request.SendPlanToAttendantsByEmail;
				settings.ShowCleaningDelays = request.ShowCleaningDelays;
				settings.ShowHoursInWorkerPlanner = request.ShowHoursInWorkerPlanner;
				settings.TravelReserve = request.TravelReserve;
				settings.UseGroups = false;
				settings.UseOrderInPlanning = request.UseOrderInPlanning;
				settings.CleanHostelRoomBedsInGroups = request.CleanHostelRoomBedsInGroups;

				//settings.BuildingsDistanceMatrix = request.BuildingsDistanceMatrix;
				//settings.LevelsDistanceMatrix = request.LevelsDistanceMatrix;
				settings.BuildingAward = request.BuildingAward;
				settings.LevelAward = request.LevelAward;
				settings.RoomAward = request.RoomAward;
				settings.LevelTime = request.LevelTime;
				settings.CleaningTime = request.CleaningTime;

				settings.WeightLevelChange = request.WeightLevelChange;
				settings.WeightCredits = request.WeightCredits;
				settings.MinutesPerCredit = request.MinutesPerCredit;
				settings.MinCreditsForMultipleCleanersCleaning = request.MinCreditsForMultipleCleanersCleaning;
			}
			else
			{
				settings = new Domain.Entities.Settings();
				settings.Id = Guid.NewGuid();
				settings.CreatedById = this.httpContextAccessor.UserId();
				settings.ModifiedById = this.httpContextAccessor.UserId();
				settings.ModifiedAt = DateTime.UtcNow;
				settings.CreatedAt = DateTime.UtcNow;
				settings.HotelId = request.HotelId;

				settings.ReserveBetweenCleanings = request.ReserveBetweenCleanings;
				settings.AllowPostponeCleanings = request.AllowPostponeCleanings;
				settings.DefaultAttendantEndTime = request.DefaultAttendantEndTime;
				settings.DefaultAttendantMaxCredits = request.DefaultAttendantMaxCredits;
				settings.DefaultAttendantStartTime = request.DefaultAttendantStartTime;
				settings.DefaultCheckInTime = request.DefaultCheckInTime;
				settings.DefaultCheckOutTime = request.DefaultCheckOutTime;
				settings.EmailAddressesForSendingPlan = request.EmailAddressesForSendingPlan;
				settings.FromEmailAddress = request.FromEmailAddress;
				settings.SendPlanToAttendantsByEmail = request.SendPlanToAttendantsByEmail;
				settings.ShowCleaningDelays = request.ShowCleaningDelays;
				settings.ShowHoursInWorkerPlanner = request.ShowHoursInWorkerPlanner;
				settings.TravelReserve = request.TravelReserve;
				settings.UseGroups = false;
				settings.UseOrderInPlanning = request.UseOrderInPlanning;
				settings.CleanHostelRoomBedsInGroups = request.CleanHostelRoomBedsInGroups;

				settings.BuildingsDistanceMatrix = null;
				settings.LevelsDistanceMatrix = null;
				settings.BuildingAward = request.BuildingAward;
				settings.LevelAward = request.LevelAward;
				settings.RoomAward = request.RoomAward;
				settings.LevelTime = request.LevelTime;
				settings.CleaningTime = request.CleaningTime;

				settings.WeightLevelChange = request.WeightLevelChange;
				settings.WeightCredits = request.WeightCredits;
				settings.MinutesPerCredit = request.MinutesPerCredit;
				settings.MinCreditsForMultipleCleanersCleaning = request.MinCreditsForMultipleCleanersCleaning;

				await this.databaseContext.Settings.AddAsync(settings);
			}

			await databaseContext.SaveChangesAsync(cancellationToken);
			return new ProcessResponse<string>
			{
				Data = settings.Id.ToString(),
				IsSuccess = true,
				Message = "Hotel settings saved."
			};
		}
	}
}
