using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Planner.Common.Extensions
{
	public static class StringExtensions
	{
		public static string ToSnakeCase(this string input)
		{
			if (string.IsNullOrEmpty(input)) { return input; }

			var startUnderscores = Regex.Match(input, @"^_+");
			return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
		}

		public static bool IsNull(this string value)
		{
			return string.IsNullOrWhiteSpace(value);
		}

		public static bool IsNotNull(this string value)
		{
			return !string.IsNullOrWhiteSpace(value);
		}

		public static IEnumerable<int> AllIndexesOf(this string str, string value)
		{
			if (value.IsNull())
				throw new ArgumentException("the string to find may not be empty", "value");

			for (int index = 0; ; index += value.Length)
			{
				index = str.IndexOf(value, index);
				if (index == -1)
					break;
				yield return index;
			}
		}

		public static TimeSpan FromSimpleTimeStringToTimeSpan(this string str)
		{
			var parts = str.Split(":");
			return new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]), 0);
		}
	}
}
