using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.CleaningPlans.Queries.GetCleaningPlanDetails;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Helpers;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.CleaningPlans.Commands.CreateCustomPlannableCleanings
{
	public class CreateCustomPlannableCleaningsCommand : IRequest<ProcessResponse<IEnumerable<CleaningTimelineItemData>>>
	{
		public bool IsToday { get; set; }
		public Guid CleaningPlanId { get; set; }
		public string Description { get; set; }
		public int Credits { get; set; }
		public IEnumerable<TaskWhereData> Wheres { get; set; }
	}

	public class CreateCustomPlannableCleaningsCommandHandler : IRequestHandler<CreateCustomPlannableCleaningsCommand, ProcessResponse<IEnumerable<CleaningTimelineItemData>>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CreateCustomPlannableCleaningsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse<IEnumerable<CleaningTimelineItemData>>> Handle(CreateCustomPlannableCleaningsCommand request, CancellationToken cancellationToken)
		{
			var cleanings = new List<CleaningPlanItem>();
			var cleaningTimelineItems = new List<CleaningTimelineItemData>();
			var cleaningPlan = await this._databaseContext.CleaningPlans.FindAsync(request.CleaningPlanId);
			var hotel = await this._databaseContext.Hotels.FindAsync(cleaningPlan.HotelId);
			var rooms = await this._loadRooms(request.Wheres, request.IsToday, cleaningPlan.Date.Date);

			var timeZoneId = Infrastructure.HotelLocalDateProvider.GetAvailableTimeZoneId(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
			var cleaningDate = cleaningPlan.Date;
			var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			var cleaningDateUtc = TimeZoneInfo.ConvertTimeToUtc(cleaningDate, timeZoneInfo);

			foreach (var room in rooms)
			{
				var c = new Domain.Entities.CleaningPlanItem
				{
					CleaningPlanId = request.CleaningPlanId,
					CleaningPluginId = null,
					Description = request.Description,
					Credits = request.Credits,
					Id = Guid.NewGuid(),
					IsActive = true,
					IsCustom = true,
					IsPlanned = false,
					IsPostponed = false,
					RoomId = room.Id,
					IsChangeSheets = false,
					IsPriority = request.IsToday ? room.IsCleaningPriority : false,
					CleaningPlanGroupId = null,
					CleaningId = null,
					DurationSec = null,
					EndsAt = null,
					IsPostponee = false,
					IsPostponer = false,
					PostponeeCleaningPlanItemId = null,
					PostponerCleaningPlanItemId = null,
					RoomBedId = null,
					StartsAt = null,
				};
				cleanings.Add(c);

				var cleaningTimelineItem = new CleaningTimelineItemData
				{
					CleaningPluginId = c.CleaningPluginId,
					CleaningPluginName = c.Description,
					Credits = c.Credits,
					Id = c.Id.ToString(),
					IsActive = c.IsActive,
					IsCustom = c.IsCustom,
					IsDoNotDisturb = room.IsDoNotDisturb,
					IsOutOfOrder = room.IsOutOfOrder,
					IsPostponed = false,
					IsRoomAssigned = room.FloorId.HasValue,
					IsTaskGuestRequest = false,
					IsTaskHighPriority = false,
					IsTaskLowPriority = false,
					ItemTypeKey = "CLEANING",
					Price = 0,
					Reservations = new CleaningTimelineItemReservationData[0],
					RoomId = room.Id,
					TaskDescription = "",
					Tasks = new CleaningTimelineItemTaskData[0],
					Title = room.Name,
					IsChangeSheets = c.IsChangeSheets,
					IsPriority = c.IsPriority,
					CleaningDescription = c.Description,
					CleaningStatus = CleaningProcessStatus.DRAFT,
					InspectedByFullName = null,
					InspectedById = null,
					IsInspected = false,
					IsInspectionRequired = false,
					IsInspectionSuccess = false,
					IsReadyForInspection = false,
					IsSent = false,
				};
				cleaningTimelineItem.RefreshCleaningStatus(cleaningDateUtc, timeZoneId, room);

				cleaningTimelineItems.Add(cleaningTimelineItem);
			}

			await this._databaseContext.CleaningPlanItems.AddRangeAsync(cleanings, cancellationToken);
			await this._databaseContext.SaveChangesAsync(cancellationToken);
			return new ProcessResponse<IEnumerable<CleaningTimelineItemData>>
			{
				Data = cleaningTimelineItems,
				HasError = false,
				IsSuccess = true,
				Message = "Cleanings created.",
			};
		}


		private async Task<IEnumerable<RoomWithHotelStructureView>> _loadRooms(IEnumerable<TaskWhereData> data, bool isTodaysCleaningPlan, DateTime cleaningDate)
		{
			var buildingIds = new List<Guid>();
			var hotelIds = new List<string>();
			var floorIds = new List<Guid>();
			var reservationIds = new List<string>();
			var roomIds = new List<Guid>();
			var bedIds = new List<Guid>();

			foreach (var w in data)
			{
				switch (w.TypeKey)
				{
					case "BUILDING":
						buildingIds.Add(new Guid(w.ReferenceId));
						break;
					case "FLOOR":
						floorIds.Add(new Guid(w.ReferenceId));
						break;
					case "HOTEL":
						hotelIds.Add(w.ReferenceId);
						break;
					case "RESERVATION":
						reservationIds.Add(w.ReferenceId);
						break;
					case "ROOM":
						roomIds.Add(new Guid(w.ReferenceId));
						break;
					case "BED":
						bedIds.Add(new Guid(w.ReferenceId));
						break;
					case "WAREHOUSE":
					case "UNKNOWN":
						break;
				}
			}

			var rooms = await this._databaseContext.Rooms.GetRoomsWithStructureAndActiveReservationsQuery(null, cleaningDate, hotelIds, buildingIds, floorIds, reservationIds, roomIds, false, false, true).ToArrayAsync();


			//var activeBedReservations = (await this._databaseContext.Reservations.GetActiveReservationsForBedsQuery(hotelId, cleaningDate).ToArrayAsync()).GroupBy(r => r.RoomId.Value).ToDictionary(r => r.Key, r => r.ToArray());

			//foreach (var roomReservationsPair in activeBedReservations)
			//{
			//	var roomId = roomReservationsPair.Key;
			//	if (!roomsMap.ContainsKey(roomId)) continue;

			//	var room = roomsMap[roomId];

			//	if (room.RoomBeds == null) continue;

			//	var bedReservations = roomReservationsPair.Value.GroupBy(r => r.RoomBedId.Value).ToDictionary(r => r.Key, r => r.ToArray());


			//	foreach (var bedReservationsPair in bedReservations)
			//	{
			//		var bedId = bedReservationsPair.Key;
			//		var bed = room.RoomBeds.FirstOrDefault(b => b.Id == bedId);

			//		if (bed == null) continue;

			//		bed.Reservations = bedReservationsPair.Value;
			//	}
			//}

			return rooms;
		}

	}
	public static class MoreEnumerable
	{
		/// <summary>
		/// Returns all distinct elements of the given source, where "distinctness"
		/// is determined via a projection and the default equality comparer for the projected type.
		/// </summary>
		/// <remarks>
		/// This operator uses deferred execution and streams the results, although
		/// a set of already-seen keys is retained. If a key is seen multiple times,
		/// only the first element with that key is returned.
		/// </remarks>
		/// <typeparam name="TSource">Type of the source sequence</typeparam>
		/// <typeparam name="TKey">Type of the projected element</typeparam>
		/// <param name="source">Source sequence</param>
		/// <param name="keySelector">Projection for determining "distinctness"</param>
		/// <returns>A sequence consisting of distinct elements from the source sequence,
		/// comparing them by the specified key projection.</returns>

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector)
		{
			return source.DistinctBy(keySelector, null);
		}

		/// <summary>
		/// Returns all distinct elements of the given source, where "distinctness"
		/// is determined via a projection and the specified comparer for the projected type.
		/// </summary>
		/// <remarks>
		/// This operator uses deferred execution and streams the results, although
		/// a set of already-seen keys is retained. If a key is seen multiple times,
		/// only the first element with that key is returned.
		/// </remarks>
		/// <typeparam name="TSource">Type of the source sequence</typeparam>
		/// <typeparam name="TKey">Type of the projected element</typeparam>
		/// <param name="source">Source sequence</param>
		/// <param name="keySelector">Projection for determining "distinctness"</param>
		/// <param name="comparer">The equality comparer to use to determine whether or not keys are equal.
		/// If null, the default equality comparer for <c>TSource</c> is used.</param>
		/// <returns>A sequence consisting of distinct elements from the source sequence,
		/// comparing them by the specified key projection.</returns>

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer = null)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

			return _(); IEnumerable<TSource> _()
			{
				var knownKeys = new HashSet<TKey>(comparer);
				foreach (var element in source)
				{
					if (knownKeys.Add(keySelector(element)))
						yield return element;
				}
			}
		}
	}
}
