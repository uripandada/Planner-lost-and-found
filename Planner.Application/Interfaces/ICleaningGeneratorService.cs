using Planner.Application.CleaningPlans.Queries.GetCleaningPlanDetails;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.Application.Interfaces
{
	public interface ICleaningGeneratorService
	{
		Task<IEnumerable<CleaningTimelineItemData>> GenerateCleanings(string hotelId, bool isToday, DateTime cleaningDate);
		/// <summary>
		/// WARNING!!! THIS METHOD EXPECTS GenerateCleanings TO BE CALLED BEFORE BECAUSE IT RELIES ON SOME LOCAL STATE.
		/// WARNING!!! THIS METHOD EXPECTS GenerateCleanings TO BE CALLED BEFORE BECAUSE IT RELIES ON SOME LOCAL STATE.
		/// WARNING!!! THIS METHOD EXPECTS GenerateCleanings TO BE CALLED BEFORE BECAUSE IT RELIES ON SOME LOCAL STATE.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="isToday"></param>
		/// <param name="cleaningDate"></param>
		/// <returns></returns>
		IEnumerable<CleaningTimelineItemData> CreateTimelineCleanings(IEnumerable<Planner.Domain.Entities.CleaningPlanItem> items, DateTime cleaningDateUtc, string timeZoneId);
		/// <summary>
		/// WARNING!!! THIS METHOD EXPECTS GenerateCleanings TO BE CALLED BEFORE BECAUSE IT RELIES ON SOME LOCAL STATE.
		/// WARNING!!! THIS METHOD EXPECTS GenerateCleanings TO BE CALLED BEFORE BECAUSE IT RELIES ON SOME LOCAL STATE.
		/// WARNING!!! THIS METHOD EXPECTS GenerateCleanings TO BE CALLED BEFORE BECAUSE IT RELIES ON SOME LOCAL STATE.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="isToday"></param>
		/// <param name="cleaningDate"></param>
		/// <returns></returns>
		IEnumerable<PlannedCleaningTimelineItemData> CreatePlannedTimelineCleanings(IEnumerable<Planner.Domain.Entities.CleaningPlanItem> items, DateTime cleaningDateUtc, string timeZoneId);
	}

}
