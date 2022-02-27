using Planner.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure
{
	public class HotelLocalDateProvider
	{
		public static string GetAvailableTimeZoneId(string windowsTimeZoneId, string ianaTimeZoneId)
		{
			try
			{
				var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
				return windowsTimeZoneId;
			}
			catch (Exception e)
			{
				return ianaTimeZoneId;
			}
		}
		public static TimeZoneInfo GetAvailableTimeZoneInfo(string windowsTimeZoneId, string ianaTimeZoneId)
		{
			try
			{
				var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
				return timeZoneInfo;
			}
			catch (Exception e)
			{
				return TimeZoneInfo.FindSystemTimeZoneById(ianaTimeZoneId);
			}
		}

		public async Task<DateTime> GetHotelCurrentLocalDate(IDatabaseContext databaseContext, string hotelId, bool includeTime = false)
		{
			var hotel = await databaseContext.Hotels.FindAsync(hotelId);
			if (hotel == null)
			{
				throw new Exception($"Unable to find hotel with id: {hotelId}");
			}

			var timeZoneInfo = (TimeZoneInfo)null;
			try
			{
				timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(hotel.WindowsTimeZoneId);
			}
			catch (Exception e)
			{
				timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(hotel.IanaTimeZoneId);
			}

			var nowUtc = DateTime.UtcNow;
			var localHotelDate = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZoneInfo);

			if (includeTime)
			{
				return localHotelDate;
			}
			else
			{
				return localHotelDate.Date;
			}
		}
	}
}
