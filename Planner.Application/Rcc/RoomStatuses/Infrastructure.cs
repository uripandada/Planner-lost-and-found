using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Application.Rcc.RoomStatuses
{
	public class RccRoomStatusProvider
	{
		public async Task<RccHotelRoomStatusChanges> LoadStatuses(IDatabaseContext databaseContext, string hotelId, IEnumerable<Domain.Entities.Room> rooms)
		{
			var hotel = await databaseContext.Hotels.FindAsync(hotelId);
			var modifiedByIds = rooms.Where(r => r.ModifiedById.HasValue).Select(m => m.ModifiedById.Value).ToArray();
			var userNamesMap = await databaseContext.Users.Select(u => new
			{
				u.Id,
				u.UserName,
			}).ToDictionaryAsync(u => u.Id, u => u.UserName);

			var hotelLocalDateProvider = new HotelLocalDateProvider();
			var timeZoneId = HotelLocalDateProvider.GetAvailableTimeZoneId(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
			var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			var hotelLocalDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);

			return new RccHotelRoomStatusChanges
			{
				HotelId = hotelId,
				RoomStatuses = rooms.Select(r => new RccRoomStatusData
				{
					RoomName = r.ExternalId,
					HkCode = r.RccHousekeepingStatus.ToString(),
					LastUpdate = hotelLocalDateTime,
					RsCode = r.RccRoomStatus.ToString(),
					UpdateUsername = r.ModifiedById.HasValue ? userNamesMap[r.ModifiedById.Value] : "UPDATED_BY_SYSTEM",
				}).ToArray(),
			};
		}

		public async Task<RccHotelGroupRoomStatusChanges> LoadHotelGroupStatuses(IDatabaseContext databaseContext, string hotelGroupKey, bool includeTemporaryRooms, bool onlyBedSpaces)
		{
			if (!databaseContext.DoesHotelGroupExist(hotelGroupKey))
			{
				return null;
			}

			databaseContext.SetTenantKey(hotelGroupKey);

			var hotels = await databaseContext.Hotels.ToListAsync();

			var roomsQuery = databaseContext.Rooms.AsQueryable();

			if (!includeTemporaryRooms)
			{
				roomsQuery = roomsQuery.Where(r => r.FloorId != null && r.BuildingId != null);
			}

			if (onlyBedSpaces)
			{
				roomsQuery = roomsQuery.Where(r => r.Category != null && r.Category.IsPrivate);
			}

			var roomsMap = (await roomsQuery.ToListAsync())
				.GroupBy(r => r.HotelId)
				.ToDictionary(group => group.Key, group => group.ToArray());

			var rccRoomStatusProvider = new RccRoomStatusProvider();
			var hotelChanges = new List<RccHotelRoomStatusChanges>();

			var changes = new RccHotelGroupRoomStatusChanges
			{
				At = DateTime.UtcNow,
				HotelGroupId = databaseContext.HotelGroupTenant.Id,
				HotelGroupKey = databaseContext.HotelGroupTenant.Key,
				HotelRoomStatuses = hotelChanges,
			};

			foreach (var hotel in hotels)
			{
				var rooms = roomsMap.ContainsKey(hotel.Id) ? roomsMap[hotel.Id] : new Domain.Entities.Room[0];

				if (!rooms.Any())
				{
					continue;
				}

				var hotelRoomStatuses = await rccRoomStatusProvider.LoadStatuses(databaseContext, hotel.Id, rooms);
				hotelChanges.Add(hotelRoomStatuses);
			}

			return changes;
		}
	}
}
