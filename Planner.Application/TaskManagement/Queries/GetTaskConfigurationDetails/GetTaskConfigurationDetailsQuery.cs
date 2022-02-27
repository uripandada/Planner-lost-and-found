using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Queries.GetTaskConfigurationDetails
{
	public class SingleTaskOptions
	{
		public Guid Id { get; set; }
		public DateTime StartsAt { get; set; }
	}

	public class DailyRecurringTaskOptions
	{
		public Guid Id { get; set; }
		public DateTime StartsAt { get; set; }
		public string[] RepeatTimes { get; set; }

		public string RepeatsForKey { get; set; }
		public int? RepeatsForNrOccurences { get; set; }
		public int? RepeatsForNrDays { get; set; }
		public DateTime? RepeatsUntilTime { get; set; }
	}

	public class WeeklyRecurringTaskOptions
	{
		public Guid Id { get; set; }
		public DateTime StartsAt { get; set; }
		public IEnumerable<WeeklyRecurringTaskItemOptions> WeeklyRecurrences { get; set; }

		public string RepeatsForKey { get; set; }
		public int? RepeatsForNrOccurences { get; set; }
		public int? RepeatsForNrDays { get; set; }
		public DateTime? RepeatsUntilTime { get; set; }
	}

	public class WeeklyRecurringTaskItemOptions
	{
		public Guid Id { get; set; }
		public string DayKey { get; set; }
		public string[] RepeatTimes { get; set; }
	}

	public class MonthlyRecurringTaskOptions
	{
		public Guid Id { get; set; }
		public DateTime StartsAt { get; set; }
		public IEnumerable<MonthlyRecurringTaskItemOptions> MonthlyRecurrences { get; set; }
		public string RepeatsForKey { get; set; }
		public int? RepeatsForNrOccurences { get; set; }
		public int? RepeatsForNrDays { get; set; }
		public DateTime? RepeatsUntilTime { get; set; }
	}

	public class MonthlyRecurringTaskItemOptions
	{
		public Guid Id { get; set; }
		public int NthOfMonth { get; set; }
		public string[] RepeatTimes { get; set; }
	}

	public class SpecificTimesRecurringTaskOptions
	{
		public Guid Id { get; set; }
		public IEnumerable<DateTime> StartsAt { get; set; }
	}
	public class RecurringEveryTaskOptions
	{
		public Guid Id { get; set; }
		public DateTime StartsAt { get; set; }
		public int EveryNumberOfDays { get; set; }
		public string RepeatsForKey { get; set; }
		public int? RepeatsForNrOccurences { get; set; }
		public int? RepeatsForNrDays { get; set; }
		public DateTime? RepeatsUntilTime { get; set; }
	}

	public class EventTaskOptions
	{
		public Guid Id { get; set; }
		public DateTime StartsAt { get; set; }

		public string EventModifierKey { get; set; } // nullable
		public string EventKey { get; set; } // nullable
		public string EventTimeKey { get; set; } // nullable
		public string RepeatsForKey { get; set; }
		public int? RepeatsForNrOccurences { get; set; }
		public int? RepeatsForNrDays { get; set; }
		public DateTime? RepeatsUntilTime { get; set; }
	}

	public class BalancedTaskOptions
	{
		public Guid Id { get; set; }
		public DateTime StartsAt { get; set; }
		public DateTime EndsAt { get; set; }
		public bool ExcludeWeekends { get; set; }
		public bool ExcludeHolidays { get; set; }
		public bool PostponeWhenRoomIsOccupied { get; set; }
	}


	public class TaskFileData
	{
		public string FileName { get; set; }
		public bool IsNew { get; set; }
	}


	public class TaskFileDetailsData
	{
		public string FileName { get; set; }
		public bool IsNew { get; set; }
		public string FileUrl { get; set; }
	}


	public class TaskWhatData
	{
		public int AssetQuantity { get; set; }
		public string ActionName { get; set; }
		public string AssetName { get; set; }
		//public string AltAssetNames { get; set; }
		public string AssetGroupName { get; set; }
		public Guid AssetId { get; set; }
		public Guid AssetGroupId { get; set; }

		public bool IsActionSystemDefined { get; set; }

		/// <summary>
		/// Described by enum: SystemActionType
		///   LOCATION_CHANGE
		///   NONE
		/// </summary>
		public string SystemDefinedActionTypeKey { get; set; }

		/// <summary>
		/// Described by enum: SystemDefinedActionIdentifier
		///   WAREHOUSE_TO_WAREHOUSE,
		///   ROOM_TO_WAREHOUSE,
		///   WAREHOUSE_TO_ROOM,
		///   ROOM_TO_ROOM,
		///   NONE,
		/// </summary>
		public string SystemDefinedActionIdentifierKey { get; set; }

		public Guid? DefaultAssignedUserId { get; set; }
	}

	/// <summary>
	/// This class is used on tasks when the task action is system defined and requires a defined
	/// "From" and "To" locations. 
	/// From and To locations can be either rooms or warehouses depending on the business case.
	/// </summary>
	public class TaskWhereMoveData
	{
		public Guid ReferenceId { get; set; } // Id of the room/warehouse
		public string ReferenceName { get; set; } // Name of the room/warehouse
		public string ReferenceTypeKey { get; set; } // Type key - ROOM, WAREHOUSE
	}

	public class TaskConfigurationDetailsSummaryData
	{
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
	}

	public class TaskConfigurationDetailsData
	{
		public Guid Id { get; set; }
		public string Comment { get; set; }
		public string TaskTypeKey { get; set; }
		public string RecurringTaskTypeKey { get; set; }
		public bool MustBeFinishedByAllWhos { get; set; }

		public int Credits { get; set; }
		public decimal Price { get; set; }
		public string PriorityKey { get; set; }
		public bool IsGuestRequest { get; set; }
		public bool IsShownInNewsFeed { get; set; }
		public bool IsRescheduledEveryDayUntilFinished { get; set; }
		public bool IsMajorNotificationRaisedWhenFinished { get; set; }
		public bool IsBlockingCleaningUntilFinished { get; set; }

		/// <summary>
		/// Defined what type of "What" is selected. A type of "What" is defined by the selected asset action.
		/// Currently only system defined asset actions SystemActionTypeKey == "LOCATION_CHANGE"
		/// Values: FROM_TO, LIST.
		/// Default value: LIST.
		/// </summary>
		public string WhatsTypeKey { get; set; }

		/// <summary>
		/// Used if WhatsTypeKey == "FROM_TO"
		/// </summary>
		public string FromReferenceId { get; set; }
		/// <summary>
		/// Used if WhatsTypeKey == "FROM_TO"
		/// </summary>
		public string FromReferenceName { get; set; }
		/// <summary>
		/// Used if WhatsTypeKey == "FROM_TO"
		/// </summary>
		public string FromReferenceTypeKey { get; set; }
		/// <summary>
		/// Used if WhatsTypeKey == "FROM_TO"
		/// </summary>
		public string ToReferenceId { get; set; }
		/// <summary>
		/// Used if WhatsTypeKey == "FROM_TO"
		/// </summary>
		public string ToReferenceName { get; set; }
		/// <summary>
		/// Used if WhatsTypeKey == "FROM_TO"
		/// </summary>
		public string ToReferenceTypeKey { get; set; }

		/// <summary>
		/// Used if WhatsTypeKey == "LIST"
		/// </summary>
		public IEnumerable<TaskWhatData> Whats { get; set; }

		public IEnumerable<TaskWhoData> Whos { get; set; }
		public IEnumerable<TaskWhereData> Wheres { get; set; }
		public IEnumerable<TaskFileDetailsData> Files { get; set; }
		public string[] FilestackImageUrls { get; set; }

		public SingleTaskOptions SingleTaskOptions { get; set; }
		public DailyRecurringTaskOptions DailyRecurringTaskOptions { get;set;}
		public WeeklyRecurringTaskOptions WeeklyRecurringTaskOptions { get; set; }
		public MonthlyRecurringTaskOptions MonthlyRecurringTaskOptions { get; set; }
		public SpecificTimesRecurringTaskOptions SpecificTimesRecurringTaskOptions { get; set; }
		public EventTaskOptions EventTaskOptions { get; set; }
		public BalancedTaskOptions BalancedTaskOptions { get; set; }
		public RecurringEveryTaskOptions RecurringEveryTaskOptions { get; set; }

		public TaskConfigurationDetailsSummaryData Summary { get; set; }
	}

	public class GetTaskConfigurationDetailsQuery : IRequest<TaskConfigurationDetailsData>
	{
		public Guid Id { get; set; }
		public bool LoadSummary { get; set; }
	}

	public class GetTaskConfigurationDetailsQueryHandler : IRequestHandler<GetTaskConfigurationDetailsQuery, TaskConfigurationDetailsData>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly RoleManager<Role> _roleManager;
		private readonly UserManager<User> _userManager;
		private readonly Guid _userId;

		public GetTaskConfigurationDetailsQueryHandler(IDatabaseContext databaseContext, RoleManager<Role> roleManager, UserManager<User> userManager, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userManager = userManager;
			this._roleManager = roleManager;
			this._userId = contextAccessor.UserId();
		}

		public async Task<TaskConfigurationDetailsData> Handle(GetTaskConfigurationDetailsQuery request, CancellationToken cancellationToken)
		{
			var config = (SystemTaskConfiguration)null;
			var summary = new TaskConfigurationDetailsSummaryData();
			if (request.LoadSummary)
			{
				config = await this._databaseContext.SystemTaskConfigurations.Include(stc => stc.Tasks).Where(tc => tc.Id == request.Id).FirstOrDefaultAsync();

				foreach (var t in config.Tasks)
				{
					summary.NumberOfTasks++;
					switch (t.StatusKey)
					{
						case nameof(TaskStatusType.PENDING):
							summary.NumberOfPendingTasks++;
							break;
						case nameof(TaskStatusType.CANCELLED):
							summary.NumberOfCancelledTasks++;
							break;
						case nameof(TaskStatusType.FINISHED):
							summary.NumberOfFinishedTasks++;
							break;
						case nameof(TaskStatusType.PAUSED):
							summary.NumberOfPausedTasks++;
							break;
						case nameof(TaskStatusType.STARTED):
							summary.NumberOfStartedTasks++;
							break;
						case nameof(TaskStatusType.VERIFIED):
							summary.NumberOfVerifiedTasks++;
							break;
						case nameof(TaskStatusType.WAITING):
							summary.NumberOfWaitingTasks++;
							break;
					}
				}

				var nrOfTotalTasks = summary.NumberOfTasks - summary.NumberOfCancelledTasks;
				if (nrOfTotalTasks > 0)
				{
					summary.CompletionFactor = ((summary.NumberOfFinishedTasks + summary.NumberOfVerifiedTasks) / (decimal)nrOfTotalTasks);
					summary.VerificationFactor = (summary.NumberOfVerifiedTasks) / (decimal)nrOfTotalTasks;
				}
				else
				{
					if (summary.NumberOfCancelledTasks > 0)
					{
						summary.CompletionFactor = 1;
						summary.VerificationFactor = 1;
					}
				}

				summary.CompletionPercentString = String.Format("{0:P0}", summary.CompletionFactor);

				if (summary.CompletionFactor < 0.5m)
					summary.CompletionStatus = "STARTING";
				else if (summary.CompletionFactor < 0.8m)
					summary.CompletionStatus = "IN_PROGRESS";
				else if (summary.CompletionFactor < 1m)
					summary.CompletionStatus = "ALMOST_COMPLETE";
				else
					summary.CompletionStatus = "COMPLETE";

				summary.VerificationPercentString = String.Format("{0:P0}", summary.VerificationFactor);

				if (summary.VerificationFactor < 0.5m)
					summary.VerificationStatus = "STARTING";
				else if (summary.VerificationFactor < 0.8m)
					summary.VerificationStatus = "IN_PROGRESS";
				else if (summary.VerificationFactor < 1m)
					summary.VerificationStatus = "ALMOST_COMPLETE";
				else
					summary.VerificationStatus = "COMPLETE";

				summary.IsCompleted = summary.CompletionFactor == 1m;
			}
			else
			{
				config = await this._databaseContext.SystemTaskConfigurations.FindAsync(request.Id);
			}

			var configDetails = new TaskConfigurationDetailsData
			{
				Id = config.Id,
				Comment = config.Data.Comment,
				RecurringTaskTypeKey = config.Data.RecurringTaskTypeKey,
				TaskTypeKey = config.Data.TaskTypeKey,
				MustBeFinishedByAllWhos = config.Data.MustBeFinishedByAllWhos,
				Credits = config.Data.Credits,
				IsBlockingCleaningUntilFinished = config.Data.IsBlockingCleaningUntilFinished,
				IsGuestRequest = config.Data.IsGuestRequest,
				PriorityKey = config.Data.PriorityKey,
				IsMajorNotificationRaisedWhenFinished = config.Data.IsMajorNotificationRaisedWhenFinished,
				IsRescheduledEveryDayUntilFinished = config.Data.IsRescheduledEveryDayUntilFinished,
				IsShownInNewsFeed = config.Data.IsShownInNewsFeed,
				Price = config.Data.Price,
				WhatsTypeKey = config.Data.WhatsTypeKey,
				FromReferenceId = null,
				FromReferenceName = null,
				FromReferenceTypeKey = null,
				ToReferenceId = null,
				ToReferenceName = null,
				ToReferenceTypeKey = null,
				Summary = summary,
				Whats = config.Data.Whats.Select(what => new TaskWhatData
				{
					AssetQuantity = what.AssetQuantity,
					ActionName = what.ActionName,
					AssetId = what.AssetId,
					AssetGroupId = what.AssetGroupId,
					AssetGroupName = what.AssetGroupName,
					AssetName = what.AssetName,
					//AltAssetNames = "", // This is not important here. Alt asset names are used only for search on the front end and is just a list of comma separated asset tags.
					IsActionSystemDefined = what.IsActionSystemDefined,
					SystemDefinedActionIdentifierKey = what.SystemDefinedActionIdentifierKey,
					SystemDefinedActionTypeKey = what.SystemDefinedActionTypeKey,
					//DefaultAssignedUserId = what.DefaultAssignedUserId,
				}),
				Wheres = config.Data.Wheres.Select(w => new TaskWhereData 
				{ 
					ReferenceId = w.ReferenceId,
					ReferenceName = w.ReferenceName,
					TypeDescription = w.TypeDescription,
					TypeKey	 = w.TypeKey
				}).ToArray(),
				Whos = config.Data.Whos.Select(w => new TaskWhoData 
				{
					ImageUrl = "",
					TypeKey = w.TypeKey,
					TypeDescription = w.TypeDescription,
					ReferenceName = w.ReferenceName,
					ReferenceId	 = w.ReferenceId,
				}).ToArray(),
				Files = new TaskFileDetailsData[0], 
				FilestackImageUrls = config.Data.FilestackImageUrls == null ? new string[0] : config.Data.FilestackImageUrls.ToArray(),
				//config.Data.Files.Select(f => new TaskFileDetailsData 
				//{
				//	FileName = f.FileName,
				//	FileUrl = f.FileUrl,
				//	IsNew = false
				//}).ToArray(),
			};

			if (config.Data.WhereFrom != null)
			{
				configDetails.FromReferenceId = config.Data.WhereFrom.ReferenceId;
				configDetails.FromReferenceName = config.Data.WhereFrom.ReferenceName;
				configDetails.FromReferenceTypeKey = config.Data.WhereFrom.TypeKey;
			}

			if (config.Data.WhereTo != null)
			{
				configDetails.ToReferenceId = config.Data.WhereTo.ReferenceId;
				configDetails.ToReferenceName = config.Data.WhereTo.ReferenceName;
				configDetails.ToReferenceTypeKey = config.Data.WhereTo.TypeKey;
			}

			var taskType = (TaskType)Enum.Parse(typeof(TaskType), config.Data.TaskTypeKey);
			switch (taskType)
			{
				case TaskType.EVENT:
					this._populateEventTaskOptions(configDetails, config);
					break;
				case TaskType.RECURRING:
					this._populateRecurringTaskOptions(configDetails, config);
					break;
				case TaskType.SINGLE:
					this._populateSingleTaskOptions(configDetails, config);
					break;
				case TaskType.BALANCED:
					this._populateBalancedTaskOptions(configDetails, config);
					break;
			}

			return configDetails;
		}

		private void _populateSingleTaskOptions(TaskConfigurationDetailsData data, SystemTaskConfiguration config)
		{
			data.SingleTaskOptions = new SingleTaskOptions
			{
				Id = Guid.Empty,
				StartsAt = config.Data.StartsAtTimes.First()
			};
		}

		private void _populateBalancedTaskOptions(TaskConfigurationDetailsData data, SystemTaskConfiguration config)
		{
			data.BalancedTaskOptions = new BalancedTaskOptions
			{
				Id = Guid.Empty,
				StartsAt = config.Data.StartsAtTimes.First(),
				EndsAt = config.Data.EndsAtTime.Value,
				ExcludeHolidays = config.Data.ExcludeHolidays ?? false,
				ExcludeWeekends = config.Data.ExcludeWeekends ?? false,
				PostponeWhenRoomIsOccupied = config.Data.PostponeWhenRoomIsOccupied ?? false,
			};
		}

		private void _populateRecurringTaskOptions(TaskConfigurationDetailsData data, SystemTaskConfiguration config)
		{
			var recurringTaskType = (RecurringTaskType)Enum.Parse(typeof(RecurringTaskType), config.Data.RecurringTaskTypeKey);

			switch (recurringTaskType)
			{
				case RecurringTaskType.DAILY:
					data.DailyRecurringTaskOptions = new DailyRecurringTaskOptions
					{
						Id = Guid.Empty,
						RepeatsForNrDays = config.Data.RepeatsForNrDays,
						RepeatsForNrOccurences = config.Data.RepeatsForNrOccurences,
						RepeatsUntilTime = config.Data.RepeatsUntilTime,
						StartsAt = config.Data.StartsAtTimes.First(),
						RepeatTimes = config.Data.RecurringTaskRepeatTimes.First().RepeatTimes,
						RepeatsForKey = config.Data.RepeatsForKey
					};
					break;
				case RecurringTaskType.WEEKLY:
					data.WeeklyRecurringTaskOptions = new WeeklyRecurringTaskOptions
					{
						Id = Guid.Empty,
						RepeatsForNrDays = config.Data.RepeatsForNrDays,
						RepeatsForNrOccurences = config.Data.RepeatsForNrOccurences,
						RepeatsUntilTime = config.Data.RepeatsUntilTime,
						StartsAt = config.Data.StartsAtTimes.First(),
						RepeatsForKey = config.Data.RepeatsForKey,
						WeeklyRecurrences = config.Data.RecurringTaskRepeatTimes.Select(rt => new WeeklyRecurringTaskItemOptions 
						{ 
							Id = Guid.Empty,
							DayKey = rt.Key,
							RepeatTimes = rt.RepeatTimes,
						}).ToArray()
					};
					break;
				case RecurringTaskType.MONTHLY:
					data.MonthlyRecurringTaskOptions = new MonthlyRecurringTaskOptions
					{
						Id = Guid.Empty,
						RepeatsForNrDays = config.Data.RepeatsForNrDays,
						RepeatsForNrOccurences = config.Data.RepeatsForNrOccurences,
						RepeatsUntilTime = config.Data.RepeatsUntilTime,
						StartsAt = config.Data.StartsAtTimes.First(),
						RepeatsForKey = config.Data.RepeatsForKey,
						MonthlyRecurrences = config.Data.RecurringTaskRepeatTimes.Select(rt => new MonthlyRecurringTaskItemOptions
						{
							Id = Guid.Empty,
							NthOfMonth = int.Parse(rt.Key),
							RepeatTimes = rt.RepeatTimes,
						}).ToArray()
					};
					break;
				case RecurringTaskType.SPECIFIC_TIME:
					data.SpecificTimesRecurringTaskOptions = new SpecificTimesRecurringTaskOptions
					{
						Id = Guid.Empty,
						StartsAt = config.Data.StartsAtTimes
					};
					break;
				case RecurringTaskType.EVERY:
					data.RecurringEveryTaskOptions = new RecurringEveryTaskOptions
					{
						Id = Guid.Empty,
						RepeatsForNrDays = config.Data.RepeatsForNrDays,
						RepeatsForNrOccurences = config.Data.RepeatsForNrOccurences,
						RepeatsUntilTime = config.Data.RepeatsUntilTime,
						StartsAt = config.Data.StartsAtTimes.First(),
						RepeatsForKey = config.Data.RepeatsForKey,
						EveryNumberOfDays = config.Data.RecurringEveryNumberOfDays,
					};
					break;
			}
		}

		private void _populateEventTaskOptions(TaskConfigurationDetailsData data, SystemTaskConfiguration config)
		{
			data.EventTaskOptions = new EventTaskOptions
			{
				Id = Guid.Empty,
				EventKey = config.Data.EventKey,
				EventModifierKey = config.Data.EventModifierKey,
				EventTimeKey = config.Data.EventTimeKey,
				StartsAt = config.Data.StartsAtTimes.First(),
				RepeatsForKey = config.Data.RepeatsForKey,
				RepeatsForNrDays = config.Data.RepeatsForNrDays,
				RepeatsForNrOccurences = config.Data.RepeatsForNrOccurences,
				RepeatsUntilTime = config.Data.RepeatsUntilTime,
			};
		}
	}
}
