using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.TaskManagement.Commands.CancelTask;
using Planner.Application.TaskManagement.Commands.CancelTasksByConfiguration;
using Planner.Application.TaskManagement.Commands.ChangeTaskStatus;
using Planner.Application.TaskManagement.Commands.ClaimTask;
using Planner.Application.TaskManagement.Commands.InsertTaskConfiguration;
using Planner.Application.TaskManagement.Commands.ReassignTask;
using Planner.Application.TaskManagement.Commands.RejectTask;
using Planner.Application.TaskManagement.Commands.SendTaskMessage;
using Planner.Application.TaskManagement.Commands.UpdateTask;
using Planner.Application.TaskManagement.Commands.UpdateTaskConfiguration;
using Planner.Application.TaskManagement.Queries.GetAllWheres;
using Planner.Application.TaskManagement.Queries.GetCalendarTasks;
using Planner.Application.TaskManagement.Queries.GetMonthlyTasks;
using Planner.Application.TaskManagement.Queries.GetMonthlyTasksGraphsData;
using Planner.Application.TaskManagement.Queries.GetPageOfTaskConfigurations;
using Planner.Application.TaskManagement.Queries.GetPageOfTaskConfigurationsForGrid;
using Planner.Application.TaskManagement.Queries.GetPageOfTasks;
using Planner.Application.TaskManagement.Queries.GetPageOfTasksForGrid;
using Planner.Application.TaskManagement.Queries.GetPageOfWeeklyTasks;
using Planner.Application.TaskManagement.Queries.GetTaskConfigurationCancelPreview;
using Planner.Application.TaskManagement.Queries.GetTaskConfigurationDetails;
using Planner.Application.TaskManagement.Queries.GetTaskConfigurationSavePreview;
using Planner.Application.TaskManagement.Queries.GetTaskDetails;
using Planner.Application.TaskManagement.Queries.GetTaskHistory;
using Planner.Application.TaskManagement.Queries.GetTaskMessages;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class TasksManagementController : BaseController
	{
		[HttpPost]
		public async Task<TasksData> GetTasksData([FromBody] GetTasksDataQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<TaskConfigurationDetailsData> GetTaskConfigurationDetails([FromBody] GetTaskConfigurationDetailsQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<TaskDetailsData> GetTaskDetails([FromBody] GetTaskDetailsQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.Tasks)]
		public async Task<ProcessResponse<InsertTaskConfigurationResult>> InsertTaskConfiguration([FromBody] InsertTaskConfigurationCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.Tasks)]
		public async Task<ProcessResponse<UpdateTaskConfigurationResult>> UpdateTaskConfiguration([FromBody] UpdateTaskConfigurationCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<PageOf<TaskGridItemData>> GetPage([FromBody] GetPageOfTasksQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<PageOf<TaskConfigurationGridItemData>> GetPageOfTaskConfigurations([FromBody] GetPageOfTaskConfigurationsQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<WeeklyTasksViewModel> GetWeeklyPage([FromBody] GetPageOfWeeklyTasksQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.Tasks)]
		public async Task<TaskConfigurationSavePreview> GetTaskSavePreview([FromBody] GetTaskConfigurationSavePreviewQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.Tasks)]
		public async Task<TaskConfigurationCancelPreview> GetTaskConfigurationCancelPreview([FromBody] GetTaskConfigurationCancelPreviewQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.Tasks)]
		public async Task<ProcessResponse> CancelTasksByConfiguration([FromBody] CancelTasksByConfigurationCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<PageOf<TaskHistoryItemViewModel>> GetTaskHistory([FromBody] GetTaskHistoryQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<TaskMessagesViewModel> GetTaskMessages([FromBody] GetTaskMessagesQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.Tasks)]
		public async Task<ProcessResponse> ChangeTaskStatus([FromBody] ChangeTaskStatusCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.Tasks)]
		public async Task<ProcessResponse<TaskMessageViewModel>> SendTaskMessage([FromBody] SendTaskMessageCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.Tasks)]
		public async Task<ProcessResponse> UpdateTask([FromBody] UpdateTaskCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponse<Guid>> ReassignTask([FromBody] ReassignTaskCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.Tasks)]
		public async Task<ProcessResponse> CancelTask([FromBody] CancelTaskCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<MonthlyTasksGraphsViewModel> GetMonthlyTasksGraphData([FromBody] GetMonthlyTasksGraphsDataQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<TaskGridItemData[]> GetMonthlyTasks([FromBody] GetMonthlyTasksQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<IEnumerable<ExtendedWhereData>> GetAllWheres([FromBody] GetAllWheresQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponse> ClaimTask([FromBody] ClaimTaskCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<ProcessResponse> RejectTask([FromBody] RejectTaskCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<PageOf<TaskConfigurationGridItem>> GetPageOfTaskConfigurationsForGrid([FromBody] GetPageOfTaskConfigurationsForGridQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<PageOf<TaskGridItem>> GetPageOfTasksForGrid([FromBody] GetPageOfTasksForGridQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		public async Task<TasksCalendar> GetCalendarTasks([FromBody] GetCalendarTasksQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
