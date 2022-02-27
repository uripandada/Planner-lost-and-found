using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Common.Extensions
{
	public static class DateTimeExtensions
	{
		public static long ConvertToTimestamp(this DateTime value)
		{
			return (value.Ticks - 621355968000000000) / 10000000;
		}
		public static long ToUnixTimeStamp(this DateTime value)
		{
			return value.ConvertToTimestamp();
		}
		public static long ConvertToTimestamp(this DateTimeOffset value)
		{
			return (value.Ticks - 621355968000000000) / 10000000;
		}
		public static long ToUnixTimeStamp(this DateTimeOffset value)
		{
			return value.ConvertToTimestamp();
		}
	}
}
