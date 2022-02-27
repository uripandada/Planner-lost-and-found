using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using Planner.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Collections.Generic;
using Planner.Common.Extensions;
using Npgsql;

namespace Planner.Application.Interfaces
{

	public interface IDatabaseContext : IDisposable
	{
		void SetTenantId(Guid hotelGroupId);
		Guid GetTenantId(string hotelGroupKey);
		bool SetTenantKey(string hotelGroupKey);
		bool DoesHotelGroupExist(Guid hotelGroupId);
		bool DoesHotelGroupExist(string hotelGroupKey);
		Common.Shared.HotelGroupTenantData HotelGroupTenant { get; }

		DbSet<CleaningPlugin> CleaningPlugins { get; set; }
		DbSet<NumberOfTasksPerUser> NumberOfTasksPerUser { get; set; }

		DbSet<PersistedGrant> PersistedGrants { get; set; }
		DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

		DbSet<Domain.Entities.Hotel> Hotels { get; set; }
		DbSet<Domain.Entities.Settings> Settings { get; set; }

		DbSet<Domain.Entities.Area> Areas { get; set; }
		DbSet<Domain.Entities.Building> Buildings { get; set; }
		DbSet<Domain.Entities.Floor> Floors { get; set; }
		DbSet<Domain.Entities.Room> Rooms { get; set; }
		DbSet<Domain.Entities.RoomCategory> RoomCategorys { get; set; }
		DbSet<Domain.Entities.Warehouse> Warehouses { get; set; }

		//DbSet<Domain.Entities.RoomWithHotelStructureView> RoomsWithHotelStructureView { get; set; }

		DbSet<Domain.Entities.Tag> Tags { get; set; }
		DbSet<Domain.Entities.File> Files { get; set; }
		DbSet<Domain.Entities.Asset> Assets { get; set; }
		DbSet<Domain.Entities.AssetGroup> AssetGroups { get; set; }
		DbSet<Domain.Entities.AssetTag> AssetTags { get; set; }
		DbSet<Domain.Entities.AssetFile> AssetFiles { get; set; }


		DbSet<Domain.Entities.AssetAction> AssetActions { get; set; }
		DbSet<Domain.Entities.RoomAssetModel> RoomAssetModels { get; set; }
		DbSet<Domain.Entities.AssetModel> AssetModels { get; set; }


		DbSet<Inventory> Inventories { get; set; }
		DbSet<InventoryAssetStatus> InventoryAssetStatuses { get; set; }
		DbSet<RoomAssetUsage> RoomAssetUsages { get; set; }
		DbSet<WarehouseAssetAvailability> WarehouseAssetAvailabilities { get; set; }
		DbSet<WarehouseDocument> WarehouseDocuments { get; set; }
		DbSet<WarehouseDocumentArchive> WarehouseDocumentArchives { get; set; }



		DbSet<UserGroup> UserGroups { get; set; }
		DbSet<UserSubGroup> UserSubGroups { get; set; }
		DbSet<User> Users { get; set; }
		DbSet<IdentityUserRole<Guid>> UserRoles { get; set; }
		DbSet<IdentityRoleClaim<Guid>> RoleClaims { get; set; }

		DbSet<IdentityUserClaim<Guid>> UserClaims { get; set; }
		DbSet<Role> Roles { get; set; }
		DbSet<ApplicationUserAvatar> ApplicationUserAvatar { get; set; }


		DbSet<CleaningPlan> CleaningPlans { get; set; }
		DbSet<CleaningPlanCpsatConfiguration> CleaningPlanCpsatConfigurations { get; set; }
		DbSet<CleaningPlanItem> CleaningPlanItems { get; set; }
		DbSet<PlannableCleaningPlanItem> PlannableCleaningPlanItems { get; set; }
		DbSet<CleaningPlanGroup> CleaningPlanGroups { get; set; }
		DbSet<CleaningPlanGroupAvailabilityInterval> CleaningPlanGroupAvailabilityIntervals { get; set; }
		//DbSet<CleaningPlanGroupFloorAffinity> CleaningPlanGroupFloorAffinities { get; set; }
		DbSet<CleaningPlanGroupAffinity> CleaningPlanGroupAffinities { get; set; }

		DbSet<LostAndFound> LostAndFounds { get; set; }
		DbSet<LostAndFoundFile> LostAndFoundFiles { get; set; }
		DbSet<OnGuard> OnGuards { get; set; }
		DbSet<OnGuardFile> OnGuardFiles { get; set; }



		DbSet<SystemTask> SystemTasks { get; set; }
		DbSet<SystemTaskAction> SystemTaskActions { get; set; }
		DbSet<SystemTaskConfiguration> SystemTaskConfigurations { get; set; }
		DbSet<SystemTaskHistory> SystemTaskHistorys { get; set; }
		DbSet<SystemTaskMessage> SystemTaskMessages { get; set; }
		DbSet<Reservation> Reservations { get; set; }

		DbSet<CleaningInspection> CleaningInspections { get; set; }
		//DbSet<CleaningPlanItemEvent> CleaningPlanItemEvents { get; set; }
		DbSet<CleaningPlanSendingHistory> CleaningPlanSendingHistories { get; set; }
		DbSet<AutomaticHousekeepingUpdateCycle> HousekeepingNightlyUpdateCycles { get; set; }
		DbSet<AutomaticHousekeepingUpdateSettings> AutomaticHousekeepingUpdateSettingss { get; set; }
		DbSet<RccProduct> RccProducts { get; set; }
		//DbSet<UserStatus> UserStatuses { get; set; }
		//DbSet<UserStatusHistory> UserStatusHistories { get; set; }

		DbSet<RoomHistoryEvent> RoomHistoryEvents { get; set; }
		DbSet<UserHistoryEvent> UserHistoryEvents { get; set; }
		DbSet<CleaningHistoryEvent> CleaningHistoryEvents { get; set; }


		DbSet<RoomNote> RoomNotes { get; set; }
		DbSet<RoomBed> RoomBeds { get; set; }
		DbSet<Cleaning> Cleanings { get; set; }

		DbSet<RccHousekeepingStatusColor> RccHousekeepingStatusColors { get; set; }

		DbSet<RoomMessage> RoomMessages { get; set; }
		DbSet<RoomMessageFilter> RoomMessageFilters { get; set; }
		DbSet<RoomMessageDate> RoomMessageDates { get; set; }
		DbSet<RoomMessageRoom> RoomMessageRooms { get; set; }
		DbSet<RoomMessageReservation> RoomMessageReservations { get; set; }

		DbSet<CleaningGeneratorLog> CleaningGeneratorLogs { get; set; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken);

		DatabaseFacade Database { get; }
	}

	public static class DbSetQueryExtensions
	{
		public static IQueryable<Reservation> GetActiveReservationsForBedsQuery(this DbSet<Domain.Entities.Reservation> reservationSet, string hotelId, DateTime cleaningDate)
		{
			var movedCleaningDate = cleaningDate.AddDays(1);
			var reservationsQuery = reservationSet.Where(r =>
				r.HotelId == hotelId &&
				r.RoomId != null &&
				r.RoomBedId != null &&
				((r.ActualCheckIn != null && r.ActualCheckIn < movedCleaningDate) || (r.CheckIn != null && r.CheckIn < movedCleaningDate)) &&
				((r.ActualCheckOut != null && r.ActualCheckOut >= cleaningDate) || (r.CheckOut != null && r.CheckOut >= cleaningDate))
			);

			return reservationsQuery;
		}

		public static IQueryable<RoomWithHotelStructureView> GetRoomsWithStructureAndActiveReservationsQuery(this DbSet<Domain.Entities.Room> roomsSet, string hotelId, DateTime cleaningDate, IEnumerable<string> hotelIds, IEnumerable<Guid> buildingIds
			, IEnumerable<Guid> floorIds, IEnumerable<string> reservationIds, IEnumerable<Guid> roomIds, bool excludeTemporaryRooms, bool isToday, bool onlyActiveReservations)
		{
			var movedCleaningDate = cleaningDate.AddDays(1);
			
			var sqlQuery = $"SELECT r.* FROM public.rooms r ";
			var orClauses = new List<string>();

			var hotelIdsParameters = new NpgsqlParameter("@hotelIds", new string[0]);
			var buildingIdsParameters = new NpgsqlParameter("@buildingIds", new Guid[0]);
			var floorIdsParameters = new NpgsqlParameter("@floorIds", new Guid[0]);
			var roomIdsParameters = new NpgsqlParameter("@roomIds", new Guid[0]);
			var reservationIdsParameters = new NpgsqlParameter("@reservationIds", new string[0]);


			if (hotelIds != null && hotelIds.Any())
			{
				orClauses.Add("r.hotel_id = ANY(@hotelIds)");
				hotelIdsParameters.Value = hotelIds;
			}
			if (buildingIds != null && buildingIds.Any())
			{
				orClauses.Add("r.building_id = ANY(@buildingIds)");
				buildingIdsParameters.Value = buildingIds;
			}
			if (floorIds != null && floorIds.Any())
			{
				orClauses.Add("r.floor_id = ANY(@floorIds)");
				floorIdsParameters.Value = floorIds;
			}
			if (reservationIds != null && reservationIds.Any())
			{
				orClauses.Add("EXISTS(SELECT rr.* FROM public.reservations rr WHERE rr.room_id = r.id AND rr.id = ANY(@reservationIds))");
				reservationIdsParameters.Value = reservationIds;
			}
			if (roomIds != null && roomIds.Any())
			{
				orClauses.Add("r.id = ANY(@roomIds)");
				roomIdsParameters.Value = roomIds;
			}

			//// TODO: REMOVE THISSSSSS
			//// TODO: REMOVE THISSSSSS
			//// TODO: REMOVE THISSSSSS
			//// TODO: REMOVE THISSSSSS
			//// TODO: REMOVE THISSSSSS
			//// TODO: REMOVE THISSSSSS
			//orClauses.Add("r.name = '1530'");

			if (orClauses.Any())
			{
				sqlQuery = sqlQuery + " WHERE " + string.Join(" OR ", orClauses) + " ";
			}

			var roomsQuery = roomsSet.FromSqlRaw(sqlQuery, hotelIdsParameters, buildingIdsParameters, floorIdsParameters, reservationIdsParameters, roomIdsParameters).AsQueryable();

			if (excludeTemporaryRooms)
			{
				roomsQuery = roomsQuery.Where(r => r.FloorId != null && r.BuildingId != null);
			}

			if (hotelId.IsNotNull())
			{
				roomsQuery = roomsQuery.Where(r => r.HotelId == hotelId);
			}

			//if (onlyActiveRooms)
			//{
			//	roomsQuery = roomsQuery.Where(r => r.)
			//}


			return roomsQuery
			.Select(r => new RoomWithHotelStructureView
			{
				Id = r.Id,
				HotelId = r.HotelId,
				AreaId = r.AreaId,
				AreaName = r.Area == null ? null : r.Area.Name,
				Name = r.Name,
				BuildingId = r.BuildingId,
				BuildingName = r.Building == null ? null : r.Building.Name,
				CategoryId = r.CategoryId,
				CategoryName = r.Category == null ? null : r.Category.Name,
				CreatedById = r.CreatedById,
				CreatedAt = r.CreatedAt,
				ExternalId = r.ExternalId,
				FloorId = r.FloorId,
				FloorName = r.Floor == null ? null : r.Floor.Name,
				FloorNumber = r.Floor == null ? -999 : r.Floor.OrdinalNumber,
				Category = r.Category,
				OrdinalNumber = r.OrdinalNumber,
				FloorSectionName = r.FloorSectionName,
				FloorSubSectionName = r.FloorSubSectionName,
				HotelName = r.Hotel.Name,
				IsAutogeneratedFromReservationSync = r.IsAutogeneratedFromReservationSync,
				IsClean = r.IsClean,
				IsDoNotDisturb = r.IsDoNotDisturb,
				IsOccupied = r.IsOccupied,
				IsOutOfOrder = r.IsOutOfOrder,
				ModifiedAt = r.ModifiedAt,
				ModifiedById = r.ModifiedById,
				TypeKey = r.TypeKey,
				IsCleaningPriority = r.IsCleaningPriority,
				RoomBeds = r.RoomBeds.ToArray(),
				Reservations = r.Reservations.Where(rr =>
					// This query selects active reservations per room for a given time period
					((rr.ActualCheckIn != null && rr.ActualCheckIn < movedCleaningDate) || (rr.ActualCheckIn == null && rr.CheckIn != null && rr.CheckIn < movedCleaningDate))
					&&
					((rr.ActualCheckOut != null && rr.ActualCheckOut >= cleaningDate) || (rr.ActualCheckOut == null && rr.CheckOut != null && rr.CheckOut >= cleaningDate))
					&&
					((onlyActiveReservations && rr.IsActive) || (!onlyActiveReservations))
					).ToArray(),
			});
		}
		
		public static IQueryable<RoomBedWithHotelStructureView> GetRoomBedsWithStructureAndActiveReservationsQuery(this DbSet<Domain.Entities.RoomBed> roomBedsSet, string hotelId, DateTime cleaningDate, IEnumerable<Guid> roomBedIds)
		{
			var movedCleaningDate = cleaningDate.AddDays(1);

			var roomBedsQuery = roomBedsSet.AsQueryable();

			if (hotelId.IsNotNull())
			{
				roomBedsQuery = roomBedsQuery.Where(r => r.Room.HotelId == hotelId);
			}
			if (roomBedIds != null && roomBedIds.Any())
			{
				roomBedsQuery = roomBedsQuery.Where(r => roomBedIds.Contains(r.Id));
			}

			return roomBedsQuery
				.Select(bed => new RoomBedWithHotelStructureView
				{
					Id = bed.Id,
					HotelId = bed.Room.HotelId,
					AreaId = bed.Room.AreaId,
					AreaName = bed.Room.Area == null ? null : bed.Room.Area.Name,
					Name = bed.Name,
					BuildingId = bed.Room.BuildingId,
					BuildingName = bed.Room.Building == null ? null : bed.Room.Building.Name,
					CategoryId = bed.Room.CategoryId,
					CategoryName = bed.Room.Category == null ? null : bed.Room.Category.Name,
					CreatedById = bed.Room.CreatedById,
					CreatedAt = bed.Room.CreatedAt,
					ExternalId = bed.ExternalId,
					FloorId = bed.Room.FloorId,
					FloorName = bed.Room.Floor == null ? null : bed.Room.Floor.Name,
					FloorNumber = bed.Room.Floor == null ? -999 : bed.Room.Floor.OrdinalNumber,
					Category = bed.Room.Category,
					OrdinalNumber = bed.Room.OrdinalNumber,
					FloorSectionName = bed.Room.FloorSectionName,
					FloorSubSectionName = bed.Room.FloorSubSectionName,
					HotelName = bed.Room.Hotel.Name,
					IsAutogeneratedFromReservationSync = bed.IsAutogeneratedFromReservationSync,
					IsClean = bed.IsClean,
					IsDoNotDisturb = bed.IsDoNotDisturb,
					IsOccupied = bed.IsOccupied,
					IsOutOfOrder = bed.IsOutOfOrder,
					ModifiedAt = bed.Room.ModifiedAt,
					ModifiedById = bed.Room.ModifiedById,
					IsCleaningPriority = bed.IsCleaningPriority,
					TypeKey = "BED",
					RoomBeds = new RoomBed[0],
					Reservations = bed.Reservations.Where(r =>
						// This query selects active reservations per room for a given time period
						((r.ActualCheckIn != null && r.ActualCheckIn < movedCleaningDate) || (r.CheckIn != null && r.CheckIn < movedCleaningDate))
						&&
						((r.ActualCheckOut != null && r.ActualCheckOut >= cleaningDate) || (r.CheckOut != null && r.CheckOut >= cleaningDate))
					).ToArray(),
				});
		}
	}
}
