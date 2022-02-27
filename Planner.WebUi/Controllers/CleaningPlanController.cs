using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.CleaningPlans.Commands.ActivatePlannableCleanings;
using Planner.Application.CleaningPlans.Commands.AddPlanItems;
using Planner.Application.CleaningPlans.Commands.AddRemovePlanGroups;
using Planner.Application.CleaningPlans.Commands.CancelPlannableCleanings;
using Planner.Application.CleaningPlans.Commands.ChangePlannableCleaningsCredits;
using Planner.Application.CleaningPlans.Commands.CreateCustomPlannableCleanings;
using Planner.Application.CleaningPlans.Commands.DeleteAndReloadCleaningPlan;
using Planner.Application.CleaningPlans.Commands.DeleteCustomPlannableCleanings;
using Planner.Application.CleaningPlans.Commands.GenerateCpsatCleaningPlan;
using Planner.Application.CleaningPlans.Commands.PostponePlannableCleanings;
using Planner.Application.CleaningPlans.Commands.RemovePlanItems;
using Planner.Application.CleaningPlans.Commands.ResetCleaningPlan;
using Planner.Application.CleaningPlans.Commands.SaveCpsatConfiguration;
using Planner.Application.CleaningPlans.Commands.SendCleaningPlan;
using Planner.Application.CleaningPlans.Commands.UndoPostponePlannableCleanings;
using Planner.Application.CleaningPlans.Commands.UpdateCleaningPlanGroup;
using Planner.Application.CleaningPlans.Commands.UpdateCleaningPlanItems;
using Planner.Application.CleaningPlans.Queries.GetCleaningGeneratorLogs;
using Planner.Application.CleaningPlans.Queries.GetCleaningPlanDetails;
using Planner.Application.CleaningPlans.Queries.GetListOfAffinityGroups;
using Planner.Application.Infrastructure.Signalr.Messages;
using Planner.Common.Data;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class CleaningPlanController : BaseController
	{
		[HttpPost]
		public async Task<CleaningPlanData> GetDetails(GetCleaningPlanDetailsQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse<AddRemoveCleaningPlanGroupsResult>> AddRemoveCleaningPlanGroups(AddRemoveCleaningPlanGroupsCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse<UpdateCleaningPlanGroupResult>> UpdateCleaningPlanGroup(UpdateCleaningPlanGroupCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse> AddCleaningPlanItems(AddCleaningPlanItemsCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse<UpdateCleaningPlanItemsResult>> UpdateCleaningPlanItems(UpdateCleaningPlanItemsCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse> RemoveCleaningPlanItems(RemoveCleaningPlanItemsCommand request)
		{
			return await this.Mediator.Send(request);
		}

		//[HttpPost]
		//public async Task<IEnumerable<CleaningTimelineItemData>> GetAllCleaningsForDate(GetAllCleaningsForDateQuery request)
		//{
		//	return await this.Mediator.Send(request);
		//}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse> ActivatePlannableCleanings(ActivatePlannableCleaningsCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse> CancelPlannableCleanings(CancelPlannableCleaningsCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse<IEnumerable<CleaningTimelineItemData>>> CreateCustomPlannableCleanings(CreateCustomPlannableCleaningsCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse> DeleteCustomPlannableCleanings(DeleteCustomPlannableCleaningsCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse> PostponePlannableCleanings(PostponePlannableCleaningsCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse> UndoPostponePlannableCleanings(UndoPostponePlannableCleaningsCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse> GenerateCpsatCleaningPlan(GenerateCpsatCleaningPlanCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse> ResetCleaningPlan(ResetCleaningPlanCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse<DeleteAndReloadCleaningPlanResult>> DeleteAndReloadCleaningPlan(DeleteAndReloadCleaningPlanCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse> ChangePlannableCleaningsCredits(ChangePlannableCleaningsCreditsCommand request)
		{
			return await this.Mediator.Send(request);
		}


		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse<SendCleaningPlanResponse>> SendCleaningPlan(SendCleaningPlanCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.CleaningPlanner)]
		public async Task<ProcessResponse> SaveCpsatConfiguration(SaveCpsatConfigurationCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<AffinityGroup>> GetListOfAffinityGroups(GetListOfAffinityGroupsQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<CleaningGeneratorLogItem>> GetCleaningGeneratorLogs(GetCleaningGeneratorLogsQuery request)
		{
			return await this.Mediator.Send(request);
		}

		/// <summary>
		/// This action is here just to expose the response classes for NSwag.
		/// DO NOT DELETE THIS.
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public TempCleaningPlanResult GetCleaningPlanProgressMessageSchema()
		{
			return new TempCleaningPlanResult
			{
			};
		}
	}

	/// <summary>
	/// Just for NSwag. DO NOT DELETE!
	/// Just for NSwag. DO NOT DELETE!
	/// Just for NSwag. DO NOT DELETE!
	/// </summary>
	public class TempCleaningPlanResult
	{
		//public CleaningPlanProgressMessage Temp1 { get; set; }
		public CpsatCleaningCalculator.AutoGeneratedPlan Temp2 { get; set; }
		public CpsatAutogeneratedPlan Temp3 { get; set; }

		public RealTimeCleaningPlannerCleaningChangedMessage Temp8 { get; set; }
		public RealTimeRefreshRoomsOverviewDashboardMessage Temp7 { get; set; }
		public RealTimeUserOnDutyChangedMessage Temp9 { get; set; }
		public RealTimeTasksChangedMessage Temp10 { get; set; }
		public RealTimeCpsatCleaningPlanningProgressChangedMessage Temp11 { get; set; }
		public RealTimeCpsatCleaningPlanningFinishedMessage Temp12 { get; set; }
		public RealTimeMessagesChangedMessage Temp13 { get; set; }


	}
}
