using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Queries.GetPageOfTaskConfigurations
{
	public class TaskConfigurationGridItemData
	{
		public Guid Id { get; set; }
		public string TaskDescription { get; set; }
		public int NumberOfTasks { get; set; }
		public decimal CompletionFactor { get; set; }
		public string CompletionPercentString { get; set; }
		public string CompletionStatus { get; set; }
		public decimal VerificationFactor { get; set; }
		public string VerificationPercentString { get; set; }
		public string VerificationStatus { get; set; }
		public int NumberOfPendingTasks { get; set; }
		public int NumberOfWaitingTasks { get; set; }
		public int NumberOfStartedTasks { get; set; }
		public int NumberOfPausedTasks { get; set; }
		public int NumberOfFinishedTasks { get; set; }
		public int NumberOfVerifiedTasks { get; set; }
		public int NumberOfCancelledTasks { get; set; }
		public bool IsCompleted { get; set; }

		public IEnumerable<TaskConfigurationGridItemActionData> Actions { get; set; }
	}

	public class TaskConfigurationGridItemActionData
	{
		public string AssetName { get; set; }
		public string ActionName { get; set; }
		public int AssetQuantity { get; set; }
	}

	public class GetPageOfTaskConfigurationsQuery : GetPageRequest, IRequest<PageOf<TaskConfigurationGridItemData>>
	{
		public string Keywords { get; set; }
		//public string FromDateString { get; set; }
		//public string ToDateString { get; set; }
		public string SortKey { get; set; }
		//public string StatusKey { get; set; }
		//public string ActionName { get; set; }
		//public Guid? AssetId { get; set; }
		//public Guid? AssetGroupId { get; set; }
		//public TaskWhoData[] Whos { get; set; }
		//public TaskWhereData[] Wheres { get; set; }
		//public bool OnlyMyTasks { get; set; }
	}
	public class GetPageOfTaskConfigurationsQueryHandler : IRequestHandler<GetPageOfTaskConfigurationsQuery, PageOf<TaskConfigurationGridItemData>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetPageOfTaskConfigurationsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<PageOf<TaskConfigurationGridItemData>> Handle(GetPageOfTaskConfigurationsQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext
				.SystemTaskConfigurations
				.Include(st => st.Tasks)
				.AsQueryable();

			//if (request.Keywords.IsNotNull())
			//{
			//	var keywordsValue = request.Keywords.ToLower();
			//	query = query.Where(tc => tc.Data.)
			//}

			var count = await query.CountAsync();

			if (request.SortKey.IsNotNull())
			{
				switch (request.SortKey)
				{
					case "CREATED_AT_ASC":
						query = query.OrderBy(q => q.CreatedAt);
						break;
					case "CREATED_AT_DESC":
						query = query.OrderByDescending(q => q.CreatedAt);
						break;
					default:
						break;
				}
			}

			query = query.Skip(request.Skip).Take(request.Take);

			var taskConfigurations = await query.ToArrayAsync();

			var response = new PageOf<TaskConfigurationGridItemData>
			{
				TotalNumberOfItems = count,
				Items = taskConfigurations.Select(tc => 
				{ 
					var gridItem = new TaskConfigurationGridItemData { Id = tc.Id };
	
					var taskConfigurationDescription = tc.Describe();
					gridItem.TaskDescription = taskConfigurationDescription.Description;
					gridItem.Actions = taskConfigurationDescription.Actions.Select(a => new TaskConfigurationGridItemActionData 
					{ 
						ActionName = a.ActionName,
						AssetName = a.AssetName,
						AssetQuantity = a.AssetQuantity,
					}).ToArray();

					foreach (var t in tc.Tasks)
					{
						gridItem.NumberOfTasks++;
						switch (t.StatusKey)
						{
							case nameof(TaskStatusType.PENDING):
								gridItem.NumberOfPendingTasks++;
								break;
							case nameof(TaskStatusType.CANCELLED):
								gridItem.NumberOfCancelledTasks++;
								break;
							case nameof(TaskStatusType.FINISHED):
								gridItem.NumberOfFinishedTasks++;
								break;
							case nameof(TaskStatusType.PAUSED):
								gridItem.NumberOfPausedTasks++;
								break;
							case nameof(TaskStatusType.STARTED):
								gridItem.NumberOfStartedTasks++;
								break;
							case nameof(TaskStatusType.VERIFIED):
								gridItem.NumberOfVerifiedTasks++;
								break;
							case nameof(TaskStatusType.WAITING):
								gridItem.NumberOfWaitingTasks++;
								break;
						}
					}

					//var sumOfTasksInProgress = gridItem.NumberOfPausedTasks + gridItem.NumberOfPendingTasks + gridItem.NumberOfStartedTasks + gridItem.NumberOfWaitingTasks;
					var nrOfTotalTasks = gridItem.NumberOfTasks - gridItem.NumberOfCancelledTasks;
					if (nrOfTotalTasks > 0)
					{
						gridItem.CompletionFactor = ((gridItem.NumberOfFinishedTasks + gridItem.NumberOfVerifiedTasks) / (decimal)nrOfTotalTasks);
						gridItem.VerificationFactor = (gridItem.NumberOfVerifiedTasks) / (decimal)nrOfTotalTasks;
					}
					else
					{
						if(gridItem.NumberOfCancelledTasks > 0)
						{
							gridItem.CompletionFactor = 1;
							gridItem.VerificationFactor = 1;
						}
					}

					gridItem.CompletionPercentString = String.Format("{0:P0}", gridItem.CompletionFactor);

					if(gridItem.CompletionFactor < 0.5m)
						gridItem.CompletionStatus = "STARTING";
					else if (gridItem.CompletionFactor < 0.8m)
						gridItem.CompletionStatus = "IN_PROGRESS";
					else if (gridItem.CompletionFactor < 1m)
						gridItem.CompletionStatus = "ALMOST_COMPLETE";
					else
						gridItem.CompletionStatus = "COMPLETE";

					gridItem.VerificationPercentString = String.Format("{0:P0}", gridItem.VerificationFactor);

					if (gridItem.VerificationFactor < 0.5m)
						gridItem.VerificationStatus = "STARTING";
					else if (gridItem.VerificationFactor < 0.8m)
						gridItem.VerificationStatus = "IN_PROGRESS";
					else if (gridItem.VerificationFactor < 1m)
						gridItem.VerificationStatus = "ALMOST_COMPLETE";
					else
						gridItem.VerificationStatus = "COMPLETE";

					gridItem.IsCompleted = gridItem.CompletionFactor == 1m;
					return gridItem;
				}).ToArray()
			};		

			return response;
		}
	}
}
