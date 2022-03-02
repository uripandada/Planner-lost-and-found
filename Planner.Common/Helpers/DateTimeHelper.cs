using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Common.Helpers
{
	public static class DateTimeHelper
	{
		public static DateTime ParseIsoDate(string dateString)
		{
			return DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// If the sent date is not monday, return first monday before that monday.
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static DateTime GetMonday(DateTime dt)
		{
			if(dt.DayOfWeek == DayOfWeek.Monday)
            {
				return dt;
            }

			int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
			return dt.AddDays(-1 * diff).Date;
		}

		/// <summary>
		/// If the sent date is not monday, return first monday before that monday.
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static DateTime GetNextSunday(DateTime dt)
		{
			var d = dt.AddDays(0);
			
			if(d.DayOfWeek == DayOfWeek.Sunday)
            {
				return d;
            }

			do
			{

				d = d.AddDays(1);
			} while (d.DayOfWeek != DayOfWeek.Sunday);

			return d;
		}

		public static DateTime GetLastDayOfMonth(int year, int month)
        {
			return new DateTime(year, month, DateTime.DaysInMonth(year, month));
        }
	}
}
