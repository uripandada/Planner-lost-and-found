using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planner.Common.Helpers
{
	public class CleaningBalancer
	{
		public static void TestBalancing()
		{
			for (int numberOfCleanings = 1; numberOfCleanings <= 5; numberOfCleanings++)
			{
				for (int numberOfNights = 1; numberOfNights <= 10; numberOfNights++)
				{
					var cleanDaysAwayFromZero = _BalanceCleaningsOverNights(numberOfCleanings, numberOfNights, BalancingRoundingType.AWAY_FROM_ZERO);
					var cleanDaysToZero = _BalanceCleaningsOverNights(numberOfCleanings, numberOfNights, BalancingRoundingType.TO_ZERO);
					var cleanDaysToEven = _BalanceCleaningsOverNights(numberOfCleanings, numberOfNights, BalancingRoundingType.TO_EVEN);
					var cleanDaysCutOff = _BalanceCleaningsOverNights(numberOfCleanings, numberOfNights, BalancingRoundingType.CUT_OFF);

					Console.WriteLine($"Cleanings: {numberOfCleanings}, Nights: {numberOfNights}");
					if (!cleanDaysAwayFromZero.Any())
					{
						Console.WriteLine("There are no cleanings for this combination");
						Console.WriteLine("--------------------------------------------------------");
						Console.WriteLine("");
						continue;
					}

					Console.WriteLine($"                ARR {(string.Join(" ", Enumerable.Range(1, numberOfNights).Select(nn => $"-{nn}-")))}");
					Console.WriteLine($"Away from zero: -A- {(string.Join(" ", Enumerable.Range(1, numberOfNights).Select(nn => nn < 10 ? (cleanDaysAwayFromZero.Contains(nn) ? $"-C-" : "---") : (cleanDaysAwayFromZero.Contains(nn) ? $"-CC-" : "----"))))}");
					Console.WriteLine($"       To zero: -A- {(string.Join(" ", Enumerable.Range(1, numberOfNights).Select(nn => nn < 10 ? (cleanDaysToZero.Contains(nn) ? $"-C-" : "---") : (cleanDaysToZero.Contains(nn) ? $"-CC-" : "----"))))}");
					Console.WriteLine($"       To even: -A- {(string.Join(" ", Enumerable.Range(1, numberOfNights).Select(nn => nn < 10 ? (cleanDaysToEven.Contains(nn) ? $"-C-" : "---") : (cleanDaysToEven.Contains(nn) ? $"-CC-" : "----"))))}");
					Console.WriteLine($"       Cut off: -A- {(string.Join(" ", Enumerable.Range(1, numberOfNights).Select(nn => nn < 10 ? (cleanDaysCutOff.Contains(nn) ? $"-C-" : "---") : (cleanDaysCutOff.Contains(nn) ? $"-CC-" : "----"))))}");
					Console.WriteLine("--------------------------------------------------------");
					Console.WriteLine("");
				}
			}
		}

		public static IEnumerable<DateTime> GetBalancedDates(DateTime fromDate, DateTime toDate, int numberOfDates)
		{
			return BalanceCleaningsOverDateInterval(fromDate, toDate, numberOfDates, false);
		}

		public static IEnumerable<DateTime> BalanceCleaningsOverDateInterval(DateTime fromDate, DateTime toDate, int numberOfCleanings, bool postponeSundayCleaningToMonday)
		{
			//var fromDate = fromTime.Date;
			//var toDate = toTime.Date;
			var numberOfNights = (int)Math.Round((toDate - fromDate).TotalDays);

			var isCleaningsOdd = (numberOfCleanings % 2) == 1;
			var isNightsOdd = (numberOfCleanings % 2) == 1;
			var roundingType = BalancingRoundingType.AWAY_FROM_ZERO; // AWAY_FROM_ZERO or TO_EVEN
			if (isCleaningsOdd || (!isCleaningsOdd && isNightsOdd))
			{
				// TO_ZERO or CUT_OFF
				roundingType = BalancingRoundingType.TO_ZERO;
			}

			var cleaningDays = _BalanceCleaningsOverNights(numberOfCleanings, numberOfNights, roundingType);
			var cleaningDates = new List<DateTime>();
			foreach (var cleaningDay in cleaningDays)
			{
				if (cleaningDays.Contains(cleaningDay))
				{
					var cleaningDate = fromDate.AddDays(cleaningDay);

					if (postponeSundayCleaningToMonday && cleaningDate.DayOfWeek == DayOfWeek.Sunday)
					{
						// next stay day
						var nextStayDay = cleaningDay + 1;

						// SKIP CLEANING - cleaning on monday already exists
						if (cleaningDays.Contains(nextStayDay))
						{
							continue;
						}

						var nextStayDate = cleaningDate.AddDays(1);

						// SKIP CLEANING - monday has departure cleaning or the guest already checked out
						if (nextStayDate >= toDate)
						{
							continue;
						}

						cleaningDates.Add(nextStayDate);
					}
					else
					{
						cleaningDates.Add(cleaningDate);
					}
				}
			}

			return cleaningDates;
		}

		// MULTIPLE ROUNDING STRATEGIES when calculating cleaning days
		// 1. Round to zero
		// 2. Round away from zero
		// 3. Round to even
		// 4. Cut off decimal part
		private static IEnumerable<int> _BalanceCleaningsOverNights(int cleanings, int nights, BalancingRoundingType roundingType)
		{
			if (nights == 0)
			{
				return new int[0];
			}

			if (cleanings >= (nights - 1))
			{
				return Enumerable.Range(1, nights - 1);
			}

			// Dividing by cleanings + 1 beacuse we want to split the stay into elements of the same duration -> 5 nights stay, 2 cleanings -> split number of nights (duration) into 
			// 3 elements of the same duration of 1.06 days. The time of n-th cleaning is at (1 + (n * 1.6666)) which means by the algorithm:
			// - 1st cleaning must be after 1+(1.6666) days = 2.6666 days -> cut off decimal part = 2nd day
			// - 2nd cleaning must be after 1+(2*1.6666) days = 4.3332 days -> cut off decimal part = 4th day
			// -----------------------
			// ARR -C- --- -C- --- DEP
			//  1   2   3   4   5   6
			// -----------------------
			var growthFactor = nights / (cleanings + 1m);

			var cleaningDays = new int[cleanings];
			var cleaningDayFactor = 0m;

			switch (roundingType)
			{
				case BalancingRoundingType.AWAY_FROM_ZERO:
					for (int cleaningIndex = 0; cleaningIndex < cleanings; cleaningIndex++)
					{
						cleaningDayFactor += growthFactor;
						cleaningDays[cleaningIndex] = (int)Math.Round(cleaningDayFactor, 0, MidpointRounding.AwayFromZero);
					}
					break;

				case BalancingRoundingType.TO_ZERO:
					for (int cleaningIndex = 0; cleaningIndex < cleanings; cleaningIndex++)
					{
						cleaningDayFactor += growthFactor;
						cleaningDays[cleaningIndex] = (int)Math.Round(cleaningDayFactor, 0, MidpointRounding.ToZero);
					}
					break;

				case BalancingRoundingType.TO_EVEN:
					for (int cleaningIndex = 0; cleaningIndex < cleanings; cleaningIndex++)
					{
						cleaningDayFactor += growthFactor;
						cleaningDays[cleaningIndex] = (int)Math.Round(cleaningDayFactor, 0, MidpointRounding.ToEven);
					}
					break;

				case BalancingRoundingType.CUT_OFF:
					for (int cleaningIndex = 0; cleaningIndex < cleanings; cleaningIndex++)
					{
						cleaningDayFactor += growthFactor;
						cleaningDays[cleaningIndex] = (int)cleaningDayFactor;
					}
					break;
			}

			return cleaningDays;
		}

		private enum BalancingRoundingType
		{
			TO_ZERO,
			AWAY_FROM_ZERO,
			TO_EVEN,
			CUT_OFF
		}
	}
}
