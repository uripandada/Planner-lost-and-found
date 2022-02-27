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
	}
}
