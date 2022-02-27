using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetTaskConfigurationDetails;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Common.Helpers;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Commands.InsertTaskConfiguration
{
	public interface ISystemTaskGenerator
	{
		Task<IEnumerable<Domain.Entities.ExtendedSystemTask>> GenerateTasks(TaskType taskType, Domain.Entities.SystemTaskConfiguration config);
		Domain.Entities.SystemTaskConfiguration GenerateTaskConfiguration(SaveTaskConfigurationRequest r);

		SystemTaskHistory GenerateTaskHistory(string changeByKey, string message, SystemTask t, SystemTaskHistoryData oldData, SystemTaskHistoryData newData);
		SystemTaskHistoryData GenerateTaskHistoryData(SystemTask task);
	}

	public class SystemTaskGenerator : ISystemTaskGenerator
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IFileService _fileService;
		private readonly Guid _userId;

		public SystemTaskGenerator(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, IFileService fileService)
		{
			this._databaseContext = databaseContext;
			this._fileService = fileService;
			this._userId = contextAccessor.UserId();
		}

		private class SystemTaskWhere
		{
			public Guid? RoomId { get; set; }
			public string RoomName { get; set; }
			public Guid? BuildingId { get; set; }
			public string BuildingName { get; set; }
			public string HotelId { get; set; }
			public string HotelName { get; set; }
			public Guid? FloorId { get; set; }
			public string FloorName { get; set; }
			public string ReservationId { get; set; }
			public string ReservationGuestName { get; set; }
			public Guid? WarehouseId { get; set; }
			public string WarehouseName { get; set; }
			public string WhereTypeKey { get; set; }
		}

		private class TaskRecurringDateRange
		{
			public TaskRecurringDateRange()
			{
				this.Dates = new List<DateTime>();
				this.MaxOccurences = -1;
				this.UseOccurences = false;
				this.UseDateInterval = false;
				this.UseDateList = false;
			}

			public DateTime StartDate { get; set; }
			public DateTime EndDate { get; set; }
			public List<DateTime> Dates { get; set; }
			public int MaxOccurences { get; set; }
			public bool UseOccurences { get; set; }
			public bool UseDateInterval { get; set; }
			public bool UseDateList { get; set; }
		}


		public Domain.Entities.SystemTaskConfiguration GenerateTaskConfiguration(SaveTaskConfigurationRequest r)
		{
			var configId = Guid.NewGuid();
			var config = new Domain.Entities.SystemTaskConfiguration
			{
				CreatedAt = DateTime.UtcNow,
				CreatedById = this._userId,
				Id = configId,
				ModifiedAt = DateTime.UtcNow,
				ModifiedById = this._userId,
				Data = new Domain.Entities.SystemTaskConfigurationData
				{
					Comment = r.Comment,
					Credits = r.Credits,
					IsGuestRequest = r.IsGuestRequest,
					PriorityKey = r.PriorityKey,
					IsMajorNotificationRaisedWhenFinished = r.IsMajorNotificationRaisedWhenFinished,
					IsRescheduledEveryDayUntilFinished = r.IsRescheduledEveryDayUntilFinished,
					IsShownInNewsFeed = r.IsShownInNewsFeed,
					IsBlockingCleaningUntilFinished = r.IsBlockingCleaningUntilFinished,
					Price = r.Price,
					WhatsTypeKey = r.WhatsTypeKey,
					//Files = r.Files.Select(f => new Domain.Entities.SystemTaskConfigurationFile
					//{
					//	FileName = f.FileName,
					//	FileUrl = this._fileService.GetTaskConfigurationFileUrl(configId, f.FileName),
					//	FileId = Guid.NewGuid()
					//}).ToArray(),
					Files = new Domain.Entities.SystemTaskConfigurationFile[0],
					FilestackImageUrls = r.FilestackImageUrls == null ? new string[0] : r.FilestackImageUrls,
					RecurringTaskTypeKey = r.RecurringTaskTypeKey,
					RepeatsForKey = null,
					TaskTypeKey = r.TaskTypeKey,
					MustBeFinishedByAllWhos = r.MustBeFinishedByAllWhos,
					Whats = r.Whats.Select(what => new Domain.Entities.SystemTaskConfigurationWhat
					{
						ActionName = what.ActionName,
						AssetId = what.AssetId,
						AssetGroupId = what.AssetGroupId,
						AssetGroupName = what.AssetGroupName,
						AssetName = what.AssetName,
						AssetQuantity = what.AssetQuantity,
						IsActionSystemDefined = what.IsActionSystemDefined,
						SystemDefinedActionIdentifierKey = what.SystemDefinedActionIdentifierKey,
						SystemDefinedActionTypeKey = what.SystemDefinedActionTypeKey,
					}),
					WhereFrom = r.FromReferenceId.IsNotNull() ? new Domain.Entities.SystemTaskConfigurationWhere
					{
						ReferenceId = r.FromReferenceId,
						ReferenceName = r.FromReferenceName,
						TypeDescription = "",
						TypeKey = r.FromReferenceTypeKey,
					} : null,
					WhereTo = r.ToReferenceId.IsNotNull() ? new Domain.Entities.SystemTaskConfigurationWhere
					{
						ReferenceId = r.ToReferenceId,
						ReferenceName = r.ToReferenceName,
						TypeDescription = "",
						TypeKey = r.ToReferenceTypeKey,
					} : null,
					Wheres = r.Wheres.Select(w => new Domain.Entities.SystemTaskConfigurationWhere
					{
						ReferenceId = w.ReferenceId,
						ReferenceName = w.ReferenceName,
						TypeDescription = w.TypeDescription,
						TypeKey = w.TypeKey
					}).ToArray(),
					Whos = r.Whos.Select(w => new Domain.Entities.SystemTaskConfigurationWho
					{
						ReferenceId = w.ReferenceId,
						TypeKey = w.TypeKey,
						ReferenceName = w.ReferenceName,
						TypeDescription = w.TypeDescription
					}).ToArray()
				}
			};

			var taskType = (TaskType)Enum.Parse(typeof(TaskType), r.TaskTypeKey);
			switch (taskType)
			{
				case TaskType.SINGLE:
					config.Data.StartsAtTimes = new DateTime[] { DateTimeHelper.ParseIsoDate(r.SingleTaskOptions.StartsAtString) };
					break;
				case TaskType.BALANCED:
					config.Data.StartsAtTimes = new DateTime[] { DateTimeHelper.ParseIsoDate(r.BalancedTaskOptions.StartsAtString) };
					config.Data.EndsAtTime = DateTimeHelper.ParseIsoDate(r.BalancedTaskOptions.EndsAtString);
					config.Data.ExcludeHolidays = r.BalancedTaskOptions.ExcludeHolidays;
					config.Data.ExcludeWeekends = r.BalancedTaskOptions.ExcludeWeekends;
					config.Data.PostponeWhenRoomIsOccupied = r.BalancedTaskOptions.PostponeWhenRoomIsOccupied;
					break;
				case TaskType.EVENT:
					config.Data.StartsAtTimes = new DateTime[] { DateTimeHelper.ParseIsoDate(r.EventTaskOptions.StartsAtString) };
					config.Data.EventKey = r.EventTaskOptions.EventKey;
					config.Data.EventModifierKey = r.EventTaskOptions.EventModifierKey;
					config.Data.EventTimeKey = r.EventTaskOptions.EventTimeKey;
					config.Data.RepeatsForKey = r.EventTaskOptions.RepeatsForKey;
					config.Data.RepeatsForNrDays = r.EventTaskOptions.RepeatsForNrDays;
					config.Data.RepeatsForNrOccurences = r.EventTaskOptions.RepeatsForNrOccurences;
					config.Data.RepeatsUntilTime = DateTimeHelper.ParseIsoDate(r.EventTaskOptions.RepeatsUntilTimeString);
					break;
				case TaskType.RECURRING:
					var recurringTaskType = (RecurringTaskType)Enum.Parse(typeof(RecurringTaskType), r.RecurringTaskTypeKey);

					switch (recurringTaskType)
					{
						case RecurringTaskType.DAILY:
							config.Data.RepeatsForKey = r.DailyRecurringTaskOptions.RepeatsForKey; ;
							config.Data.RepeatsForNrDays = r.DailyRecurringTaskOptions.RepeatsForNrDays;
							config.Data.RepeatsForNrOccurences = r.DailyRecurringTaskOptions.RepeatsForNrOccurences;
							config.Data.RepeatsUntilTime = DateTimeHelper.ParseIsoDate(r.DailyRecurringTaskOptions.RepeatsUntilTimeString);
							config.Data.StartsAtTimes = new DateTime[] { DateTimeHelper.ParseIsoDate(r.DailyRecurringTaskOptions.StartsAtString) };
							config.Data.RecurringTaskRepeatTimes = new Domain.Entities.SystemTaskRecurringTimeOptions[]
							{
								new Domain.Entities.SystemTaskRecurringTimeOptions
								{
									Key = "daily",
									RepeatTimes = r.DailyRecurringTaskOptions.RepeatTimes
								}
							};
							break;
						case RecurringTaskType.WEEKLY:
							config.Data.RepeatsForKey = r.WeeklyRecurringTaskOptions.RepeatsForKey;
							config.Data.RepeatsForNrDays = r.WeeklyRecurringTaskOptions.RepeatsForNrDays;
							config.Data.RepeatsForNrOccurences = r.WeeklyRecurringTaskOptions.RepeatsForNrOccurences;
							config.Data.RepeatsUntilTime = DateTimeHelper.ParseIsoDate(r.WeeklyRecurringTaskOptions.RepeatsUntilTimeString);
							config.Data.StartsAtTimes = new DateTime[] { DateTimeHelper.ParseIsoDate(r.WeeklyRecurringTaskOptions.StartsAtString) };
							config.Data.RecurringTaskRepeatTimes = r.WeeklyRecurringTaskOptions.WeeklyRecurrences.Select(w => new Domain.Entities.SystemTaskRecurringTimeOptions
							{
								Key = w.DayKey,
								RepeatTimes = w.RepeatTimes
							}).ToArray();
							break;
						case RecurringTaskType.MONTHLY:
							config.Data.RepeatsForKey = r.MonthlyRecurringTaskOptions.RepeatsForKey;
							config.Data.RepeatsForNrDays = r.MonthlyRecurringTaskOptions.RepeatsForNrDays;
							config.Data.RepeatsForNrOccurences = r.MonthlyRecurringTaskOptions.RepeatsForNrOccurences;
							config.Data.RepeatsUntilTime = DateTimeHelper.ParseIsoDate(r.MonthlyRecurringTaskOptions.RepeatsUntilTimeString);
							config.Data.StartsAtTimes = new DateTime[] { DateTimeHelper.ParseIsoDate(r.MonthlyRecurringTaskOptions.StartsAtString) };
							config.Data.RecurringTaskRepeatTimes = r.MonthlyRecurringTaskOptions.MonthlyRecurrences.Select(w => new Domain.Entities.SystemTaskRecurringTimeOptions
							{
								Key = w.NthOfMonth.ToString(),
								RepeatTimes = w.RepeatTimes
							}).ToArray();
							break;
						case RecurringTaskType.SPECIFIC_TIME:
							config.Data.StartsAtTimes = r.SpecificTimesRecurringTaskOptions.StartsAtStrings.Select(s => DateTimeHelper.ParseIsoDate((s))).ToArray();
							break;
						case RecurringTaskType.EVERY:
							config.Data.RepeatsForKey = r.RecurringEveryTaskOptions.RepeatsForKey;
							config.Data.RepeatsForNrDays = r.RecurringEveryTaskOptions.RepeatsForNrDays;
							config.Data.RepeatsForNrOccurences = r.RecurringEveryTaskOptions.RepeatsForNrOccurences;
							config.Data.RepeatsUntilTime = DateTimeHelper.ParseIsoDate(r.RecurringEveryTaskOptions.RepeatsUntilTimeString);
							config.Data.StartsAtTimes = new DateTime[] { DateTimeHelper.ParseIsoDate(r.RecurringEveryTaskOptions.StartsAtString) };
							config.Data.RecurringEveryNumberOfDays = r.RecurringEveryTaskOptions.EveryNumberOfDays;
							break;
					}
					break;
			}

			return config;
		}


		public async Task<IEnumerable<Domain.Entities.ExtendedSystemTask>> GenerateTasks(TaskType taskType, Domain.Entities.SystemTaskConfiguration config)
		{
			// Before this method returns it should see if there are any active cleanings today for planned attendants and send appropriate notifications.

			var tasks = (IEnumerable<Domain.Entities.ExtendedSystemTask>)null;
			switch (taskType)
			{
				case TaskType.SINGLE:
					tasks = await this._generateSingleTasks(config);
					break;
				case TaskType.RECURRING:
					tasks = await this._generateRecurringTasks(config);
					break;
				case TaskType.EVENT:
					tasks = await this._generateEventTasks(config);
					break;
				case TaskType.BALANCED:
					tasks = await this._generateBalancedTasks(config);
					break;
				default:
					throw new NotSupportedException($"{taskType.ToString()} is not supported for task generation.");
			}

			return tasks;
		}

		private async Task<IEnumerable<Domain.Entities.ExtendedSystemTask>> _generateBalancedTasks(Domain.Entities.SystemTaskConfiguration config)
		{
			var startsAtDate = config.Data.StartsAtTimes.First().Date;
			var endsAtDate = config.Data.EndsAtTime.Value.Date;

			var users = await this._loadSpecificWhos(config.Data.Whos);
			//var wheres = await this._loadSpecificWheres(config.Data.Wheres);

			var wheresResult = await this._LoadWhereFromsAndTos(config);

			var numberOfTasks = users.Count() * wheresResult.WhereFroms.Count() * wheresResult.WhereTos.Count();
			var numberOfDates = endsAtDate.Subtract(startsAtDate).Days + 1;

			var numberOfTasksPerDay = (int)(numberOfTasks / numberOfDates); // Number of tasks that must be done each day. No need for balancing since there are more tasks for days.
			var numberOfTasksToBalance = numberOfTasks % numberOfDates; // Only the rest of the tasks must be balanced. (If there are less tasks than days, everything will be balanced)

			var balancedDates = numberOfTasksToBalance == 0 ? new HashSet<DateTime>() : Common.Helpers.CleaningBalancer.GetBalancedDates(startsAtDate, endsAtDate, numberOfTasksToBalance).ToHashSet();

			var tasksQueue = new Queue<ExtendedSystemTask>();
			foreach (var user in users)
			{
				foreach (var whereFrom in wheresResult.WhereFroms)
				{
					foreach (var whereTo in wheresResult.WhereTos)
					{
						var taskId = Guid.NewGuid();
						var task = new ExtendedSystemTask
						{
							Actions = config.Data.Whats.Select(w => new SystemTaskAction
							{
								Id = Guid.NewGuid(),
								ActionName = w.ActionName,
								AssetId = w.AssetId,
								AssetGroupId = w.AssetGroupId,
								AssetGroupName = w.AssetGroupName,
								AssetName = w.AssetName,
								AssetQuantity = w.AssetQuantity,
								SystemTaskId = taskId,
							}).ToArray(),
							CreatedAt = DateTime.UtcNow,
							CreatedBy = null,
							CreatedById = this._userId,
							EventKey = null,
							EventModifierKey = null,
							EventTimeKey = null,
							History = new List<SystemTaskHistory>(),
							Id = taskId,
							IsManuallyModified = false,
							ModifiedAt = DateTime.UtcNow,
							ModifiedBy = null,
							ModifiedById = this._userId,
							StartsAt = DateTime.MinValue, // THIS VALUE IS CHANGED IN A LATER STEP (AFTER THE BALANCING IS CALCULATED).
							StatusKey = TaskStatusType.PENDING.ToString(),
							SystemTaskConfiguration = null,
							SystemTaskConfigurationId = config.Id,
							WhereTypeKey = wheresResult.WhereTypeKey,
							MustBeFinishedByAllWhos = config.Data.MustBeFinishedByAllWhos,
							TypeKey = config.Data.TaskTypeKey,
							RecurringTypeKey = config.Data.RecurringTaskTypeKey,
							RepeatsForKey = config.Data.RepeatsForKey,
							UserId = user.Id,
							IsForPlannedAttendant = user.IsForPlannedAttendant,
							UserFullName = $"{user.FirstName} {user.LastName}",
							UserUsername = user.UserName,
							Price = config.Data.Price,
							IsBlockingCleaningUntilFinished = config.Data.IsBlockingCleaningUntilFinished,
							IsShownInNewsFeed = config.Data.IsShownInNewsFeed,
							IsRescheduledEveryDayUntilFinished = config.Data.IsRescheduledEveryDayUntilFinished,
							IsMajorNotificationRaisedWhenFinished = config.Data.IsMajorNotificationRaisedWhenFinished,
							PriorityKey = config.Data.PriorityKey,
							IsGuestRequest = config.Data.IsGuestRequest,
							Credits = config.Data.Credits,

							FromBuildingId = whereFrom.BuildingId,
							FromBuildingName = whereFrom.BuildingName,
							FromFloorId = whereFrom.FloorId,
							FromFloorName = whereFrom.FloorName,
							FromHotelId = whereFrom.HotelId,
							FromHotelName = whereFrom.HotelName,
							FromReservationGuestName = whereFrom.ReservationGuestName,
							FromReservationId = whereFrom.ReservationId,
							FromRoomId = whereFrom.RoomId,
							FromRoomName = whereFrom.RoomName,
							FromWarehouseId = whereFrom.WarehouseId,
							FromWarehouseName = whereFrom.WarehouseName,

							ToBuildingId = whereTo.BuildingId,
							ToBuildingName = whereTo.BuildingName,
							ToFloorId = whereTo.FloorId,
							ToFloorName = whereTo.FloorName,
							ToHotelId = whereTo.HotelId,
							ToHotelName = whereTo.HotelName,
							ToReservationGuestName = whereTo.ReservationGuestName,
							ToReservationId = whereTo.ReservationId,
							ToRoomId = whereTo.RoomId,
							ToRoomName = whereTo.RoomName,
							ToWarehouseId = whereTo.WarehouseId,
							ToWarehouseName = whereTo.WarehouseName,

							ToName = null,
							FromName = null,

							Comment = config.Data.Comment,
						};

						this._setTaskFromToNames(task);

						tasksQueue.Enqueue(task);
					}
				}
			}

			//var balancedTaskCounters = new Dictionary<DateTime, int>();
			var tasks = new List<ExtendedSystemTask>();
			for (DateTime date = startsAtDate; date <= endsAtDate; date = date.AddDays(1))
			{
				var numberOfTasksForTheDate = numberOfTasksPerDay;
				if (balancedDates.Contains(date))
				{
					numberOfTasksForTheDate += 1;
				}

				for (int i = 0; i < numberOfTasksForTheDate; i++)
				{
					var task = tasksQueue.Dequeue();
					if (task != null)
					{
						task.StartsAt = date;
						tasks.Add(task);
					}
				}
			}

			return tasks;
		}

		private class _LoadWheresResult
		{
			public string WhereTypeKey { get; set; }
			public IEnumerable<SystemTaskWhere> WhereFroms { get; set; }
			public IEnumerable<SystemTaskWhere> WhereTos { get; set; }
		}

		private async Task<_LoadWheresResult> _LoadWhereFromsAndTos(Domain.Entities.SystemTaskConfiguration config)
		{
			var result = new _LoadWheresResult();

			if (config.Data.WhatsTypeKey == "LIST")
			{
				result.WhereTypeKey = "TO";
				result.WhereFroms = new SystemTaskWhere[]
				{
					new SystemTaskWhere
					{
						BuildingId = null,
						BuildingName = null,
						FloorId = null,
						FloorName = null,
						HotelId = null,
						HotelName = null,
						ReservationId = null,
						RoomId = null,
						RoomName = null,
						WhereTypeKey = null,
						WarehouseId = null,
						WarehouseName = null,
						ReservationGuestName = null,
					}
				};
				result.WhereTos = await this._loadSpecificWheres(config.Data.Wheres);
			}
			else if (config.Data.WhatsTypeKey == "FROM_TO")
			{
				result.WhereTypeKey = "FROM_TO";
				result.WhereFroms = await this._loadSpecificWheres(new SystemTaskConfigurationWhere[] { config.Data.WhereFrom });
				result.WhereTos = await this._loadSpecificWheres(new SystemTaskConfigurationWhere[] { config.Data.WhereTo });
			}

			return result;
		}

		private void _setTaskFromToNames(ExtendedSystemTask task)
		{
			if (task.FromWarehouseId.HasValue)
			{
				task.FromName = task.FromWarehouseName;
			}
			else if (task.FromReservationId.IsNotNull())
			{
				task.FromName = task.FromReservationGuestName;
			}
			else if (task.FromRoomId.HasValue)
			{
				task.FromName = task.FromRoomName;
			}
			else
			{
				task.FromName = task.FromHotelName;
			}

			if (task.ToWarehouseId.HasValue)
			{
				task.ToName = task.ToWarehouseName;
			}
			else if (task.ToReservationId.IsNotNull())
			{
				task.ToName = task.ToReservationGuestName;
			}
			else if (task.ToRoomId.HasValue)
			{
				task.ToName = task.ToRoomName;
			}
			else
			{
				task.ToName = task.ToHotelName;
			}
		}

		private async Task<IEnumerable<Domain.Entities.ExtendedSystemTask>> _generateSingleTasks(Domain.Entities.SystemTaskConfiguration config)
		{
			var singleTasks = new List<ExtendedSystemTask>();

			var users = await this._loadSpecificWhos(config.Data.Whos);

			var wheresResult = await this._LoadWhereFromsAndTos(config);

			foreach (var user in users)
			{
				foreach (var whereFrom in wheresResult.WhereFroms)
				{
					foreach (var whereTo in wheresResult.WhereTos)
					{
						var taskId = Guid.NewGuid();
						var task = new ExtendedSystemTask
						{
							Actions = config.Data.Whats.Select(w => new SystemTaskAction
							{
								Id = Guid.NewGuid(),
								ActionName = w.ActionName,
								AssetId = w.AssetId,
								AssetGroupId = w.AssetGroupId,
								AssetGroupName = w.AssetGroupName,
								AssetName = w.AssetName,
								AssetQuantity = w.AssetQuantity,
								SystemTaskId = taskId,
							}).ToArray(),
							CreatedAt = DateTime.UtcNow,
							CreatedBy = null,
							CreatedById = this._userId,
							EventKey = null,
							EventModifierKey = null,
							EventTimeKey = null,
							History = new List<SystemTaskHistory>(),
							Id = taskId,
							IsManuallyModified = false,
							ModifiedAt = DateTime.UtcNow,
							ModifiedBy = null,
							ModifiedById = this._userId,
							StartsAt = config.Data.StartsAtTimes.First(),
							StatusKey = TaskStatusType.PENDING.ToString(),
							SystemTaskConfiguration = null,
							SystemTaskConfigurationId = config.Id,
							WhereTypeKey = wheresResult.WhereTypeKey,
							MustBeFinishedByAllWhos = config.Data.MustBeFinishedByAllWhos,
							TypeKey = config.Data.TaskTypeKey,
							RecurringTypeKey = config.Data.RecurringTaskTypeKey,
							RepeatsForKey = config.Data.RepeatsForKey,
							UserId = user.Id,
							IsForPlannedAttendant = user.IsForPlannedAttendant,
							UserFullName = $"{user.FirstName} {user.LastName}",
							UserUsername = user.UserName,
							Price = config.Data.Price,
							IsBlockingCleaningUntilFinished = config.Data.IsBlockingCleaningUntilFinished,
							IsShownInNewsFeed = config.Data.IsShownInNewsFeed,
							IsRescheduledEveryDayUntilFinished = config.Data.IsRescheduledEveryDayUntilFinished,
							IsMajorNotificationRaisedWhenFinished = config.Data.IsMajorNotificationRaisedWhenFinished,
							PriorityKey = config.Data.PriorityKey,
							IsGuestRequest = config.Data.IsGuestRequest,
							Credits = config.Data.Credits,

							FromBuildingId = whereFrom.BuildingId,
							FromBuildingName = whereFrom.BuildingName,
							FromFloorId = whereFrom.FloorId,
							FromFloorName = whereFrom.FloorName,
							FromHotelId = whereFrom.HotelId,
							FromHotelName = whereFrom.HotelName,
							FromReservationGuestName = whereFrom.ReservationGuestName,
							FromReservationId = whereFrom.ReservationId,
							FromRoomId = whereFrom.RoomId,
							FromRoomName = whereFrom.RoomName,
							FromWarehouseId = whereFrom.WarehouseId,
							FromWarehouseName = whereFrom.WarehouseName,

							ToBuildingId = whereTo.BuildingId,
							ToBuildingName = whereTo.BuildingName,
							ToFloorId = whereTo.FloorId,
							ToFloorName = whereTo.FloorName,
							ToHotelId = whereTo.HotelId,
							ToHotelName = whereTo.HotelName,
							ToReservationGuestName = whereTo.ReservationGuestName,
							ToReservationId = whereTo.ReservationId,
							ToRoomId = whereTo.RoomId,
							ToRoomName = whereTo.RoomName,
							ToWarehouseId = whereTo.WarehouseId,
							ToWarehouseName = whereTo.WarehouseName,

							ToName = null,
							FromName = null,

							Comment = config.Data.Comment,
						};

						this._setTaskFromToNames(task);

						singleTasks.Add(task);
					}
				}
			}

			return singleTasks;
		}

		//public class SpecificWhos
		//{
		//	public IEnumerable<User> Users { get; set; }
		//	public bool IsForPlannedAttendant { get; set; }	
		//}

		public class SpecificWho
		{
			public Guid? Id { get; set; }
			public string UserName { get; set; }
			public string FirstName { get; set; }
			public string LastName { get; set; }
			public bool IsForPlannedAttendant { get; set; }
		}

		private async Task<IEnumerable<SpecificWho>> _loadSpecificWhos(IEnumerable<SystemTaskConfigurationWho> whos)
		{
			var groupIds = new List<Guid>();
			//var roleIds = new List<Guid>();
			var subGroupIds = new List<Guid>();
			var userIds = new List<Guid>();

			var users = new List<SpecificWho>();

			var isForPlannedAttendant = false;


			foreach (var who in whos)
			{
				var whoType = (TaskWhoType)Enum.Parse(typeof(TaskWhoType), who.TypeKey);
				switch (whoType)
				{
					case TaskWhoType.GROUP:
						groupIds.Add(new Guid(who.ReferenceId));
						break;
					//case TaskWhoType.ROLE:
					//	roleIds.Add(new Guid(who.ReferenceId));
					//	break;
					case TaskWhoType.SUBGROUP:
						subGroupIds.Add(new Guid(who.ReferenceId));
						break;
					case TaskWhoType.UNKNOWN:
						break;
					case TaskWhoType.USER:
						userIds.Add(new Guid(who.ReferenceId));
						break;
					case TaskWhoType.PLANNED_ATTENDANT:
						isForPlannedAttendant = true;
						break;
				}
			}

			if (groupIds.Any())
			{
				var uids = await this._databaseContext
					.UserGroups
					.Where(g => groupIds.Contains(g.Id))
					.SelectMany(ug => ug.UserSubGroups)
					.SelectMany(ug => ug.Users)
					.Select(u => u.Id)
					.ToListAsync();

				userIds.AddRange(uids);
			}

			//if (roleIds.Any())
			//{
			//	var uids = await this._databaseContext
			//		.UserRoles
			//		.Where(ur => roleIds.Contains(ur.RoleId))
			//		.Select(ur => ur.UserId)
			//		.ToListAsync();

			//	userIds.AddRange(uids);
			//}

			if (subGroupIds.Any())
			{
				var uids = await this._databaseContext
					.UserSubGroups
					.Where(g => subGroupIds.Contains(g.Id))
					.SelectMany(ug => ug.Users)
					.Select(u => u.Id)
					.ToListAsync();

				userIds.AddRange(uids);
			}

			if (userIds.Any())
			{
				var ids = userIds.Distinct().ToArray();
				users = await this._databaseContext.Users.Where(u => ids.Contains(u.Id)).Select(u => new SpecificWho 
				{ 
					Id = u.Id,
					FirstName = u.FirstName,
					LastName = u.LastName,
					UserName = u.UserName,
					IsForPlannedAttendant = false
				}).ToListAsync();
			}

			if (isForPlannedAttendant)
			{
				users.Add(new SpecificWho
				{
					Id = null,
					UserName = "Planned attendant",
					FirstName = "Planned attendant",
					LastName = "",
					IsForPlannedAttendant = true,
				});
			}

			return users;
		}

		private async Task<IEnumerable<SystemTaskWhere>> _loadSpecificWheres(IEnumerable<SystemTaskConfigurationWhere> data)
		{
			var wheres = new List<SystemTaskWhere>();

			var buildingIds = new List<Guid>();
			var hotelIds = new List<string>();
			var floorIds = new List<Guid>();
			var reservationIds = new List<string>();
			var roomIds = new List<Guid>();
			var warehouseIds = new List<Guid>();

			foreach (var w in data)
			{
				var whereType = (TaskWhereType)Enum.Parse(typeof(TaskWhereType), w.TypeKey);
				switch (whereType)
				{
					case TaskWhereType.BUILDING:
						buildingIds.Add(new Guid(w.ReferenceId));
						break;
					case TaskWhereType.FLOOR:
						floorIds.Add(new Guid(w.ReferenceId));
						break;
					case TaskWhereType.HOTEL:
						hotelIds.Add(w.ReferenceId);
						break;
					case TaskWhereType.RESERVATION:
						reservationIds.Add(w.ReferenceId);
						break;
					case TaskWhereType.ROOM:
						roomIds.Add(new Guid(w.ReferenceId));
						break;
					case TaskWhereType.WAREHOUSE:
						warehouseIds.Add(new Guid(w.ReferenceId));
						break;
					case TaskWhereType.UNKNOWN:
						break;
				}
			}

			if (warehouseIds.Any())
			{
				var ws = await this._databaseContext.Warehouses.Where(w => warehouseIds.Contains(w.Id))
					.Select(w => new SystemTaskWhere
					{
						HotelId = w.HotelId,
						HotelName = w.Hotel.Name,
						WarehouseId = w.Id,
						WarehouseName = w.Name,
						FloorId = w.FloorId == null ? null : w.FloorId,
						FloorName = w.FloorId == null ? null : w.Floor.Name,
						WhereTypeKey = TaskWhereType.WAREHOUSE.ToString()
					}).ToListAsync();

				wheres.AddRange(ws);
			}

			if (hotelIds.Any())
			{
				//var t = await this._databaseContext.Hotels.Where(h => hotelIds.Contains(h.Id))
				//	.Select(h => new SystemTaskWhere
				//	{
				//		HotelId = h.Id,
				//		HotelName = h.Name,
				//		WhereTypeKey = TaskWhereType.HOTEL.ToString()
				//	}).ToListAsync();
				var t = await this._databaseContext.Rooms.Where(r => hotelIds.Contains(r.HotelId))
					.Select(r => new SystemTaskWhere
					{
						BuildingId = r.BuildingId,
						BuildingName = r.Building.Name,
						FloorId = r.FloorId,
						FloorName = r.Floor.Name,
						HotelId = r.HotelId,
						HotelName = r.Hotel.Name,
						RoomId = r.Id,
						RoomName = r.Name,
						WhereTypeKey = TaskWhereType.ROOM.ToString()
					}).ToListAsync();

				wheres.AddRange(t);
			}

			if (buildingIds.Any())
			{
				//var t = await this._databaseContext.Buildings.Where(b => buildingIds.Contains(b.Id))
				//	.Select(b => new SystemTaskWhere
				//	{
				//		BuildingId = b.Id,
				//		BuildingName = b.Name,
				//		HotelId = b.HotelId,
				//		HotelName = b.Hotel.Name,
				//		WhereTypeKey = TaskWhereType.BUILDING.ToString()
				//	}).ToListAsync();
				var t = await this._databaseContext.Rooms.Where(r => r.BuildingId.HasValue && buildingIds.Contains(r.BuildingId.Value))
					.Select(r => new SystemTaskWhere
					{
						BuildingId = r.BuildingId,
						BuildingName = r.Building.Name,
						FloorId = r.FloorId,
						FloorName = r.Floor.Name,
						HotelId = r.HotelId,
						HotelName = r.Hotel.Name,
						RoomId = r.Id,
						RoomName = r.Name,
						WhereTypeKey = TaskWhereType.ROOM.ToString()
					}).ToListAsync();

				wheres.AddRange(t);
			}

			if (floorIds.Any())
			{
				//var t = await this._databaseContext.Floors.Where(f => floorIds.Contains(f.Id))
				//	.Select(f => new SystemTaskWhere
				//	{
				//		BuildingId = f.BuildingId,
				//		BuildingName = f.Building.Name,
				//		FloorId = f.Id,
				//		FloorName = f.Name,
				//		HotelId = f.HotelId,
				//		HotelName = f.Hotel.Name,
				//		WhereTypeKey = TaskWhereType.FLOOR.ToString()
				//	}).ToListAsync();
				var t = await this._databaseContext.Rooms.Where(r => r.FloorId.HasValue && floorIds.Contains(r.FloorId.Value))
					.Select(r => new SystemTaskWhere
					{
						BuildingId = r.BuildingId,
						BuildingName = r.Building.Name,
						FloorId = r.FloorId,
						FloorName = r.Floor.Name,
						HotelId = r.HotelId,
						HotelName = r.Hotel.Name,
						RoomId = r.Id,
						RoomName = r.Name,
						WhereTypeKey = TaskWhereType.ROOM.ToString()
					}).ToListAsync();

				wheres.AddRange(t);
			}

			if (roomIds.Any())
			{
				var t = await this._databaseContext.Rooms.Where(r => roomIds.Contains(r.Id))
					.Select(r => new SystemTaskWhere
					{
						BuildingId = r.BuildingId,
						BuildingName = r.Building.Name,
						FloorId = r.FloorId,
						FloorName = r.Floor.Name,
						HotelId = r.HotelId,
						HotelName = r.Hotel.Name,
						RoomId = r.Id,
						RoomName = r.Name,
						WhereTypeKey = TaskWhereType.ROOM.ToString()
					}).ToListAsync();

				wheres.AddRange(t);
			}

			if (reservationIds.Any())
			{
				var t = await this._databaseContext
					.Reservations
					.Where(r => reservationIds.Contains(r.Id))
					.Select(r => new SystemTaskWhere
					{
						ReservationId = r.Id,
						RoomId = r.RoomId,
						RoomName = r.Room.Name,
						BuildingId = r.Room.BuildingId,
						BuildingName = r.Room.Building.Name,
						FloorId = r.Room.FloorId,
						FloorName = r.Room.Floor.Name,
						HotelId = r.HotelId,
						HotelName = r.Hotel.Name,
						WhereTypeKey = TaskWhereType.RESERVATION.ToString(),
						ReservationGuestName = r.GuestName,
					}).ToListAsync();

				//var t = await this._databaseContext.Rooms.Where(r => r.Reservations.Any(res => reservationIds.Contains(res.Id)))
				//	.Select(r => new SystemTaskWhere
				//	{
				//		BuildingId = r.BuildingId,
				//		BuildingName = r.Building.Name,
				//		FloorId = r.FloorId,
				//		FloorName = r.Floor.Name,
				//		HotelId = r.HotelId,
				//		HotelName = r.Hotel.Name,
				//		RoomId = r.Id,
				//		RoomName = r.Name,
				//		WhereTypeKey = TaskWhereType.ROOM.ToString()
				//	}).ToListAsync();

				wheres.AddRange(t);
			}

			return wheres;
		}



		private async Task<IEnumerable<Domain.Entities.ExtendedSystemTask>> _generateRecurringTasks(Domain.Entities.SystemTaskConfiguration config)
		{
			var recurringTasks = new List<ExtendedSystemTask>();
			var dateRange = this._getRecurringDateRange(config);
			var recurringTaskType = (RecurringTaskType)Enum.Parse(typeof(RecurringTaskType), config.Data.RecurringTaskTypeKey);

			var users = await this._loadSpecificWhos(config.Data.Whos);
			//var wheres = await this._loadSpecificWheres(config.Data.Wheres);

			var wheresResult = await this._LoadWhereFromsAndTos(config);

			switch (recurringTaskType)
			{
				case RecurringTaskType.DAILY:
					if (dateRange.UseOccurences)
					{
						var dailyStartsAt = config.Data.StartsAtTimes.First();
						var dailyTaskDate = new DateTime(dailyStartsAt.Year, dailyStartsAt.Month, dailyStartsAt.Day);
						var numberOfGeneratedDailyTasks = 0;
						var repeatTimes = config.Data.RecurringTaskRepeatTimes.First().RepeatTimes.Select(rt => TimeSpan.Parse(rt));

						while (numberOfGeneratedDailyTasks <= dateRange.MaxOccurences)
						{
							foreach (var time in repeatTimes)
							{
								var taskTime = dailyTaskDate.Add(time);

								foreach (var user in users)
								{ 
									foreach (var whereFrom in wheresResult.WhereFroms)
									{
										foreach (var whereTo in wheresResult.WhereTos)
										{
											if (numberOfGeneratedDailyTasks <= dateRange.MaxOccurences)
											{
												break;
											}

											var taskId = Guid.NewGuid();
											var task = new ExtendedSystemTask
											{
												Actions = config.Data.Whats.Select(w => new SystemTaskAction
												{
													Id = Guid.NewGuid(),
													ActionName = w.ActionName,
													AssetId = w.AssetId,
													AssetGroupId = w.AssetGroupId,
													AssetGroupName = w.AssetGroupName,
													AssetName = w.AssetName,
													AssetQuantity = w.AssetQuantity,
													SystemTaskId = taskId,
												}).ToArray(),
												CreatedAt = DateTime.UtcNow,
												CreatedBy = null,
												CreatedById = this._userId,
												EventKey = null,
												EventModifierKey = null,
												EventTimeKey = null,
												History = new List<SystemTaskHistory>(),
												Id = taskId,
												IsManuallyModified = false,
												ModifiedAt = DateTime.UtcNow,
												ModifiedBy = null,
												ModifiedById = this._userId,
												StartsAt = taskTime,
												StatusKey = TaskStatusType.PENDING.ToString(),
												SystemTaskConfiguration = null,
												SystemTaskConfigurationId = config.Id,
												WhereTypeKey = wheresResult.WhereTypeKey,
												MustBeFinishedByAllWhos = config.Data.MustBeFinishedByAllWhos,
												TypeKey = config.Data.TaskTypeKey,
												RecurringTypeKey = config.Data.RecurringTaskTypeKey,
												RepeatsForKey = config.Data.RepeatsForKey,
												UserId = user.Id,
												IsForPlannedAttendant = user.IsForPlannedAttendant,
												UserFullName = $"{user.FirstName} {user.LastName}",
												UserUsername = user.UserName,
												Price = config.Data.Price,
												IsBlockingCleaningUntilFinished = config.Data.IsBlockingCleaningUntilFinished,
												IsShownInNewsFeed = config.Data.IsShownInNewsFeed,
												IsRescheduledEveryDayUntilFinished = config.Data.IsRescheduledEveryDayUntilFinished,
												IsMajorNotificationRaisedWhenFinished = config.Data.IsMajorNotificationRaisedWhenFinished,
												PriorityKey = config.Data.PriorityKey,
												IsGuestRequest = config.Data.IsGuestRequest,
												Credits = config.Data.Credits,

												FromBuildingId = whereFrom.BuildingId,
												FromBuildingName = whereFrom.BuildingName,
												FromFloorId = whereFrom.FloorId,
												FromFloorName = whereFrom.FloorName,
												FromHotelId = whereFrom.HotelId,
												FromHotelName = whereFrom.HotelName,
												FromReservationGuestName = whereFrom.ReservationGuestName,
												FromReservationId = whereFrom.ReservationId,
												FromRoomId = whereFrom.RoomId,
												FromRoomName = whereFrom.RoomName,
												FromWarehouseId = whereFrom.WarehouseId,
												FromWarehouseName = whereFrom.WarehouseName,

												ToBuildingId = whereTo.BuildingId,
												ToBuildingName = whereTo.BuildingName,
												ToFloorId = whereTo.FloorId,
												ToFloorName = whereTo.FloorName,
												ToHotelId = whereTo.HotelId,
												ToHotelName = whereTo.HotelName,
												ToReservationGuestName = whereTo.ReservationGuestName,
												ToReservationId = whereTo.ReservationId,
												ToRoomId = whereTo.RoomId,
												ToRoomName = whereTo.RoomName,
												ToWarehouseId = whereTo.WarehouseId,
												ToWarehouseName = whereTo.WarehouseName,

												ToName = null,
												FromName = null,

												Comment = config.Data.Comment,
											};

											this._setTaskFromToNames(task);

											recurringTasks.Add(task);

											numberOfGeneratedDailyTasks++;
										}
									}
								}
							}
						}
					}
					else
					{
						var dailyDates = this._generateDateList(dateRange.StartDate, dateRange.EndDate);
						var repeatTimes = config.Data.RecurringTaskRepeatTimes.First().RepeatTimes.Select(rt => TimeSpan.Parse(rt));

						foreach (var d in dailyDates)
						{
							var dailyTaskDate = new DateTime(d.Year, d.Month, d.Day);
							foreach (var time in repeatTimes)
							{
								var taskTime = dailyTaskDate.Add(time);

								foreach (var user in users)
								{
									foreach (var whereFrom in wheresResult.WhereFroms)
									{
										foreach (var whereTo in wheresResult.WhereTos)
										{
											var taskId = Guid.NewGuid();
											var task = new ExtendedSystemTask
											{

												Actions = config.Data.Whats.Select(w => new SystemTaskAction
												{
													Id = Guid.NewGuid(),
													ActionName = w.ActionName,
													AssetId = w.AssetId,
													AssetGroupId = w.AssetGroupId,
													AssetGroupName = w.AssetGroupName,
													AssetName = w.AssetName,
													AssetQuantity = w.AssetQuantity,
													SystemTaskId = taskId,
												}).ToArray(),
												CreatedAt = DateTime.UtcNow,
												CreatedBy = null,
												CreatedById = this._userId,
												EventKey = null,
												EventModifierKey = null,
												EventTimeKey = null,
												History = new List<SystemTaskHistory>(),
												Id = taskId,
												IsManuallyModified = false,
												ModifiedAt = DateTime.UtcNow,
												ModifiedBy = null,
												ModifiedById = this._userId,
												StartsAt = taskTime,
												StatusKey = TaskStatusType.PENDING.ToString(),
												SystemTaskConfiguration = null,
												SystemTaskConfigurationId = config.Id,
												WhereTypeKey = wheresResult.WhereTypeKey,
												MustBeFinishedByAllWhos = config.Data.MustBeFinishedByAllWhos,
												TypeKey = config.Data.TaskTypeKey,
												RecurringTypeKey = config.Data.RecurringTaskTypeKey,
												RepeatsForKey = config.Data.RepeatsForKey,
												UserId = user.Id,
												IsForPlannedAttendant = user.IsForPlannedAttendant,
												UserFullName = $"{user.FirstName} {user.LastName}",
												UserUsername = user.UserName,
												Credits = config.Data.Credits,
												Price = config.Data.Price,
												PriorityKey = config.Data.PriorityKey,
												IsBlockingCleaningUntilFinished = config.Data.IsBlockingCleaningUntilFinished,
												IsShownInNewsFeed = config.Data.IsShownInNewsFeed,
												IsRescheduledEveryDayUntilFinished = config.Data.IsRescheduledEveryDayUntilFinished,
												IsMajorNotificationRaisedWhenFinished = config.Data.IsMajorNotificationRaisedWhenFinished,
												IsGuestRequest = config.Data.IsGuestRequest,

												FromBuildingId = whereFrom.BuildingId,
												FromBuildingName = whereFrom.BuildingName,
												FromFloorId = whereFrom.FloorId,
												FromFloorName = whereFrom.FloorName,
												FromHotelId = whereFrom.HotelId,
												FromHotelName = whereFrom.HotelName,
												FromReservationGuestName = whereFrom.ReservationGuestName,
												FromReservationId = whereFrom.ReservationId,
												FromRoomId = whereFrom.RoomId,
												FromRoomName = whereFrom.RoomName,
												FromWarehouseId = whereFrom.WarehouseId,
												FromWarehouseName = whereFrom.WarehouseName,

												ToBuildingId = whereTo.BuildingId,
												ToBuildingName = whereTo.BuildingName,
												ToFloorId = whereTo.FloorId,
												ToFloorName = whereTo.FloorName,
												ToHotelId = whereTo.HotelId,
												ToHotelName = whereTo.HotelName,
												ToReservationGuestName = whereTo.ReservationGuestName,
												ToReservationId = whereTo.ReservationId,
												ToRoomId = whereTo.RoomId,
												ToRoomName = whereTo.RoomName,
												ToWarehouseId = whereTo.WarehouseId,
												ToWarehouseName = whereTo.WarehouseName,

												ToName = null,
												FromName = null,

												Comment = config.Data.Comment,
											};

											this._setTaskFromToNames(task);

											recurringTasks.Add(task);
										}
									}
								}
							}
						}
					}
					break;
				case RecurringTaskType.WEEKLY:
					var weeklyStartsAt = config.Data.StartsAtTimes.First();
					var weeklyTaskDate = new DateTime(weeklyStartsAt.Year, weeklyStartsAt.Month, weeklyStartsAt.Day);
					var weekDayKeys = new string[] { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" };
					var startsAtDayOfWeek = weeklyStartsAt.DayOfWeek;
					var dayOfWeekIndexMap = new Dictionary<DayOfWeek, int>
					{
						{ DayOfWeek.Monday, 0 },
						{ DayOfWeek.Tuesday, 1 },
						{ DayOfWeek.Wednesday, 2 },
						{ DayOfWeek.Thursday, 3 },
						{ DayOfWeek.Friday, 4 },
						{ DayOfWeek.Saturday, 5 },
						{ DayOfWeek.Sunday, 6 },
					};
					var initialStartsAtDayOfWeekIndex = dayOfWeekIndexMap[weeklyStartsAt.DayOfWeek];

					if (dateRange.UseOccurences)
					{
						var numberOfGeneratedWeeklyTasks = 0;
						var initialWeekDayIndex = initialStartsAtDayOfWeekIndex;

						while (numberOfGeneratedWeeklyTasks < dateRange.MaxOccurences)
						{
							for (int weekDayIndex = initialWeekDayIndex; weekDayIndex < 7; weekDayIndex++)
							{
								if (numberOfGeneratedWeeklyTasks >= dateRange.MaxOccurences)
								{
									break;
								}

								var weekDayKey = weekDayKeys[weekDayIndex];

								var repeatTimes = config.Data.RecurringTaskRepeatTimes.First(rt => rt.Key == weekDayKey).RepeatTimes.Select(rt => TimeSpan.Parse(rt));
								foreach (var time in repeatTimes)
								{
									if (numberOfGeneratedWeeklyTasks >= dateRange.MaxOccurences)
									{
										break;
									}

									var taskTime = weeklyTaskDate.Add(time);

									foreach (var user in users)
									{
										if (numberOfGeneratedWeeklyTasks >= dateRange.MaxOccurences)
										{
											break;
										}

										foreach (var whereFrom in wheresResult.WhereFroms)
										{
											if (numberOfGeneratedWeeklyTasks >= dateRange.MaxOccurences)
											{
												break;
											}

											foreach (var whereTo in wheresResult.WhereTos)
											{
												if (numberOfGeneratedWeeklyTasks >= dateRange.MaxOccurences)
												{
													break;
												}

												var taskId = Guid.NewGuid();
												var task = new ExtendedSystemTask
												{
													Actions = config.Data.Whats.Select(w => new SystemTaskAction
													{
														Id = Guid.NewGuid(),
														ActionName = w.ActionName,
														AssetId = w.AssetId,
														AssetGroupId = w.AssetGroupId,
														AssetGroupName = w.AssetGroupName,
														AssetName = w.AssetName,
														AssetQuantity = w.AssetQuantity,
														SystemTaskId = taskId,
													}).ToArray(),
													CreatedAt = DateTime.UtcNow,
													CreatedBy = null,
													CreatedById = this._userId,
													EventKey = null,
													EventModifierKey = null,
													EventTimeKey = null,
													History = new List<SystemTaskHistory>(),
													Id = taskId,
													IsManuallyModified = false,
													ModifiedAt = DateTime.UtcNow,
													ModifiedBy = null,
													ModifiedById = this._userId,
													StartsAt = taskTime,
													StatusKey = TaskStatusType.PENDING.ToString(),
													SystemTaskConfiguration = null,
													SystemTaskConfigurationId = config.Id,
													WhereTypeKey = wheresResult.WhereTypeKey,
													MustBeFinishedByAllWhos = config.Data.MustBeFinishedByAllWhos,
													TypeKey = config.Data.TaskTypeKey,
													RecurringTypeKey = config.Data.RecurringTaskTypeKey,
													RepeatsForKey = config.Data.RepeatsForKey,
													UserId = user.Id,
													IsForPlannedAttendant = user.IsForPlannedAttendant,
													UserFullName = $"{user.FirstName} {user.LastName}",
													UserUsername = user.UserName,
													Credits = config.Data.Credits,
													Price = config.Data.Price,
													PriorityKey = config.Data.PriorityKey,
													IsBlockingCleaningUntilFinished = config.Data.IsBlockingCleaningUntilFinished,
													IsShownInNewsFeed = config.Data.IsShownInNewsFeed,
													IsRescheduledEveryDayUntilFinished = config.Data.IsRescheduledEveryDayUntilFinished,
													IsMajorNotificationRaisedWhenFinished = config.Data.IsMajorNotificationRaisedWhenFinished,
													IsGuestRequest = config.Data.IsGuestRequest,

													FromBuildingId = whereFrom.BuildingId,
													FromBuildingName = whereFrom.BuildingName,
													FromFloorId = whereFrom.FloorId,
													FromFloorName = whereFrom.FloorName,
													FromHotelId = whereFrom.HotelId,
													FromHotelName = whereFrom.HotelName,
													FromReservationGuestName = whereFrom.ReservationGuestName,
													FromReservationId = whereFrom.ReservationId,
													FromRoomId = whereFrom.RoomId,
													FromRoomName = whereFrom.RoomName,
													FromWarehouseId = whereFrom.WarehouseId,
													FromWarehouseName = whereFrom.WarehouseName,

													ToBuildingId = whereTo.BuildingId,
													ToBuildingName = whereTo.BuildingName,
													ToFloorId = whereTo.FloorId,
													ToFloorName = whereTo.FloorName,
													ToHotelId = whereTo.HotelId,
													ToHotelName = whereTo.HotelName,
													ToReservationGuestName = whereTo.ReservationGuestName,
													ToReservationId = whereTo.ReservationId,
													ToRoomId = whereTo.RoomId,
													ToRoomName = whereTo.RoomName,
													ToWarehouseId = whereTo.WarehouseId,
													ToWarehouseName = whereTo.WarehouseName,

													ToName = null,
													FromName = null,

													Comment = config.Data.Comment,
												};

												this._setTaskFromToNames(task);

												recurringTasks.Add(task);
												numberOfGeneratedWeeklyTasks++;
											}
										}
									}
								}

								weeklyTaskDate = weeklyTaskDate.AddDays(1);
							}

							initialWeekDayIndex = 0; // IMPORTANT TO RESET AFTER FIRST WEEK LOOP!
						}
					}
					else
					{
						var weeklyDates = this._generateDateList(dateRange.StartDate, dateRange.EndDate);
						foreach (var d in weeklyDates)
						{
							weeklyTaskDate = new DateTime(d.Year, d.Month, d.Day);
							var weekDayKey = weekDayKeys[dayOfWeekIndexMap[weeklyTaskDate.DayOfWeek]];

							var repeatTimes = config.Data.RecurringTaskRepeatTimes.First(t => t.Key == weekDayKey).RepeatTimes.Select(rt => TimeSpan.Parse(rt));
							foreach (var time in repeatTimes)
							{
								var taskTime = weeklyTaskDate.Add(time);

								foreach (var user in users)
								{
									foreach (var whereFrom in wheresResult.WhereFroms)
									{ 
										foreach (var whereTo in wheresResult.WhereTos)
										{
											var taskId = Guid.NewGuid();
											var task = new ExtendedSystemTask
											{

												Actions = config.Data.Whats.Select(w => new SystemTaskAction
												{
													Id = Guid.NewGuid(),
													ActionName = w.ActionName,
													AssetId = w.AssetId,
													AssetGroupId = w.AssetGroupId,
													AssetGroupName = w.AssetGroupName,
													AssetName = w.AssetName,
													AssetQuantity = w.AssetQuantity,
													SystemTaskId = taskId,
												}).ToArray(),
												CreatedAt = DateTime.UtcNow,
												CreatedBy = null,
												CreatedById = this._userId,
												EventKey = null,
												EventModifierKey = null,
												EventTimeKey = null,
												History = new List<SystemTaskHistory>(),
												Id = taskId,
												IsManuallyModified = false,
												ModifiedAt = DateTime.UtcNow,
												ModifiedBy = null,
												ModifiedById = this._userId,
												StartsAt = taskTime,
												StatusKey = TaskStatusType.PENDING.ToString(),
												SystemTaskConfiguration = null,
												SystemTaskConfigurationId = config.Id,
												WhereTypeKey = wheresResult.WhereTypeKey,
												MustBeFinishedByAllWhos = config.Data.MustBeFinishedByAllWhos,
												TypeKey = config.Data.TaskTypeKey,
												RecurringTypeKey = config.Data.RecurringTaskTypeKey,
												RepeatsForKey = config.Data.RepeatsForKey,
												UserId = user.Id,
												IsForPlannedAttendant = user.IsForPlannedAttendant,
												UserFullName = $"{user.FirstName} {user.LastName}",
												UserUsername = user.UserName,
												Credits = config.Data.Credits,
												Price = config.Data.Price,
												PriorityKey = config.Data.PriorityKey,
												IsBlockingCleaningUntilFinished = config.Data.IsBlockingCleaningUntilFinished,
												IsShownInNewsFeed = config.Data.IsShownInNewsFeed,
												IsRescheduledEveryDayUntilFinished = config.Data.IsRescheduledEveryDayUntilFinished,
												IsMajorNotificationRaisedWhenFinished = config.Data.IsMajorNotificationRaisedWhenFinished,
												IsGuestRequest = config.Data.IsGuestRequest,

												FromBuildingId = whereFrom.BuildingId,
												FromBuildingName = whereFrom.BuildingName,
												FromFloorId = whereFrom.FloorId,
												FromFloorName = whereFrom.FloorName,
												FromHotelId = whereFrom.HotelId,
												FromHotelName = whereFrom.HotelName,
												FromReservationGuestName = whereFrom.ReservationGuestName,
												FromReservationId = whereFrom.ReservationId,
												FromRoomId = whereFrom.RoomId,
												FromRoomName = whereFrom.RoomName,
												FromWarehouseId = whereFrom.WarehouseId,
												FromWarehouseName = whereFrom.WarehouseName,

												ToBuildingId = whereTo.BuildingId,
												ToBuildingName = whereTo.BuildingName,
												ToFloorId = whereTo.FloorId,
												ToFloorName = whereTo.FloorName,
												ToHotelId = whereTo.HotelId,
												ToHotelName = whereTo.HotelName,
												ToReservationGuestName = whereTo.ReservationGuestName,
												ToReservationId = whereTo.ReservationId,
												ToRoomId = whereTo.RoomId,
												ToRoomName = whereTo.RoomName,
												ToWarehouseId = whereTo.WarehouseId,
												ToWarehouseName = whereTo.WarehouseName,

												ToName = null,
												FromName = null,

												Comment = config.Data.Comment,
											};

											this._setTaskFromToNames(task);

											recurringTasks.Add(task);
										}
									}
								}
							}
						}
					}
					break;
				case RecurringTaskType.MONTHLY:
					var monthlyStartsAt = config.Data.StartsAtTimes.First();
					var monthlyTaskDate = new DateTime(monthlyStartsAt.Year, monthlyStartsAt.Month, 1);
					var initialStartsAtDayOfMonth = monthlyStartsAt.Day;
					var monthCounter = 0;

					var repeatTimesMap = config.Data.RecurringTaskRepeatTimes.ToDictionary(rt => int.Parse(rt.Key), rt => rt.RepeatTimes.Select(rt => TimeSpan.Parse(rt)));

					if (dateRange.UseOccurences)
					{
						var numberOfGeneratedMonthlyTasks = 0;
						while (numberOfGeneratedMonthlyTasks < dateRange.MaxOccurences)
						{
							foreach (var rtkvp in repeatTimesMap)
							{
								if (rtkvp.Key < initialStartsAtDayOfMonth) continue;

								foreach (var time in rtkvp.Value)
								{
									var taskTime = DateTime.MinValue;
									try
									{
										taskTime = new DateTime(monthlyTaskDate.Year, monthlyTaskDate.Month, rtkvp.Key);
									}
									catch (Exception ex)
									{
										// This means that the current month has impossible condition - e.g. 30th of February.
										// This can happen because the user configures the interface generally for all months
										// so the interface enables you to create tasks for every month day including 31st of each month.

										// Just skip tasks times like this.
										continue;
									}

									taskTime = taskTime.Add(time);

									foreach (var user in users)
									{
										foreach (var whereFrom in wheresResult.WhereFroms)
										{
											foreach (var whereTo in wheresResult.WhereTos)
											{
												if (numberOfGeneratedMonthlyTasks >= dateRange.MaxOccurences)
												{
													break;
												}

												var taskId = Guid.NewGuid();
												var task = new ExtendedSystemTask
												{
													Actions = config.Data.Whats.Select(w => new SystemTaskAction
													{
														Id = Guid.NewGuid(),
														ActionName = w.ActionName,
														AssetId = w.AssetId,
														AssetGroupId = w.AssetGroupId,
														AssetGroupName = w.AssetGroupName,
														AssetName = w.AssetName,
														AssetQuantity = w.AssetQuantity,
														SystemTaskId = taskId,
													}).ToArray(),
													CreatedAt = DateTime.UtcNow,
													CreatedBy = null,
													CreatedById = this._userId,
													EventKey = null,
													EventModifierKey = null,
													EventTimeKey = null,
													History = new List<SystemTaskHistory>(),
													Id = taskId,
													IsManuallyModified = false,
													ModifiedAt = DateTime.UtcNow,
													ModifiedBy = null,
													ModifiedById = this._userId,
													StartsAt = taskTime,
													StatusKey = TaskStatusType.PENDING.ToString(),
													SystemTaskConfiguration = null,
													SystemTaskConfigurationId = config.Id,
													WhereTypeKey = wheresResult.WhereTypeKey,
													MustBeFinishedByAllWhos = config.Data.MustBeFinishedByAllWhos,
													TypeKey = config.Data.TaskTypeKey,
													RecurringTypeKey = config.Data.RecurringTaskTypeKey,
													RepeatsForKey = config.Data.RepeatsForKey,
													UserId = user.Id,
													IsForPlannedAttendant = user.IsForPlannedAttendant,
													UserFullName = $"{user.FirstName} {user.LastName}",
													UserUsername = user.UserName,
													Credits = config.Data.Credits,
													Price = config.Data.Price,
													PriorityKey = config.Data.PriorityKey,
													IsBlockingCleaningUntilFinished = config.Data.IsBlockingCleaningUntilFinished,
													IsShownInNewsFeed = config.Data.IsShownInNewsFeed,
													IsRescheduledEveryDayUntilFinished = config.Data.IsRescheduledEveryDayUntilFinished,
													IsMajorNotificationRaisedWhenFinished = config.Data.IsMajorNotificationRaisedWhenFinished,
													IsGuestRequest = config.Data.IsGuestRequest,

													FromBuildingId = whereFrom.BuildingId,
													FromBuildingName = whereFrom.BuildingName,
													FromFloorId = whereFrom.FloorId,
													FromFloorName = whereFrom.FloorName,
													FromHotelId = whereFrom.HotelId,
													FromHotelName = whereFrom.HotelName,
													FromReservationGuestName = whereFrom.ReservationGuestName,
													FromReservationId = whereFrom.ReservationId,
													FromRoomId = whereFrom.RoomId,
													FromRoomName = whereFrom.RoomName,
													FromWarehouseId = whereFrom.WarehouseId,
													FromWarehouseName = whereFrom.WarehouseName,

													ToBuildingId = whereTo.BuildingId,
													ToBuildingName = whereTo.BuildingName,
													ToFloorId = whereTo.FloorId,
													ToFloorName = whereTo.FloorName,
													ToHotelId = whereTo.HotelId,
													ToHotelName = whereTo.HotelName,
													ToReservationGuestName = whereTo.ReservationGuestName,
													ToReservationId = whereTo.ReservationId,
													ToRoomId = whereTo.RoomId,
													ToRoomName = whereTo.RoomName,
													ToWarehouseId = whereTo.WarehouseId,
													ToWarehouseName = whereTo.WarehouseName,

													ToName = null,
													FromName = null,

													Comment = config.Data.Comment,
												};

												this._setTaskFromToNames(task);

												recurringTasks.Add(task);
												numberOfGeneratedMonthlyTasks++;
											}

											if (numberOfGeneratedMonthlyTasks >= dateRange.MaxOccurences)
											{
												break;
											}
										}
											
										if (numberOfGeneratedMonthlyTasks >= dateRange.MaxOccurences)
										{
											break;
										}
									}

									if (numberOfGeneratedMonthlyTasks >= dateRange.MaxOccurences)
									{
										break;
									}
								}
							}

							initialStartsAtDayOfMonth = 0;
							monthCounter++;
							monthlyTaskDate = monthlyTaskDate.AddMonths(1);
						}
					}
					else
					{
						var monthlyDates = this._generateDateList(dateRange.StartDate, dateRange.EndDate);
						var tasksMap = config.Data.RecurringTaskRepeatTimes.ToDictionary(rt => int.Parse(rt.Key), rt => rt.RepeatTimes.Select(rt => TimeSpan.Parse(rt)));

						foreach (var d in monthlyDates)
						{
							if (!tasksMap.ContainsKey(d.Day)) continue;

							foreach (var time in tasksMap[d.Day])
							{
								var taskTime = d.Add(time);

								foreach (var user in users)
								{
									foreach (var whereFrom in wheresResult.WhereFroms)
									{
										foreach (var whereTo in wheresResult.WhereTos)
										{
											var taskId = Guid.NewGuid();
											var task = new ExtendedSystemTask
											{
												Actions = config.Data.Whats.Select(w => new SystemTaskAction
												{
													Id = Guid.NewGuid(),
													ActionName = w.ActionName,
													AssetId = w.AssetId,
													AssetGroupId = w.AssetGroupId,
													AssetGroupName = w.AssetGroupName,
													AssetName = w.AssetName,
													AssetQuantity = w.AssetQuantity,
													SystemTaskId = taskId,
												}).ToArray(),
												CreatedAt = DateTime.UtcNow,
												CreatedBy = null,
												CreatedById = this._userId,
												EventKey = null,
												EventModifierKey = null,
												EventTimeKey = null,
												History = new List<SystemTaskHistory>(),
												Id = taskId,
												IsManuallyModified = false,
												ModifiedAt = DateTime.UtcNow,
												ModifiedBy = null,
												ModifiedById = this._userId,
												StartsAt = taskTime,
												StatusKey = TaskStatusType.PENDING.ToString(),
												SystemTaskConfiguration = null,
												SystemTaskConfigurationId = config.Id,
												WhereTypeKey = wheresResult.WhereTypeKey,
												MustBeFinishedByAllWhos = config.Data.MustBeFinishedByAllWhos,
												TypeKey = config.Data.TaskTypeKey,
												RecurringTypeKey = config.Data.RecurringTaskTypeKey,
												RepeatsForKey = config.Data.RepeatsForKey,
												UserId = user.Id,
												IsForPlannedAttendant = user.IsForPlannedAttendant,
												UserFullName = $"{user.FirstName} {user.LastName}",
												UserUsername = user.UserName,
												Credits = config.Data.Credits,
												Price = config.Data.Price,
												PriorityKey = config.Data.PriorityKey,
												IsBlockingCleaningUntilFinished = config.Data.IsBlockingCleaningUntilFinished,
												IsShownInNewsFeed = config.Data.IsShownInNewsFeed,
												IsRescheduledEveryDayUntilFinished = config.Data.IsRescheduledEveryDayUntilFinished,
												IsMajorNotificationRaisedWhenFinished = config.Data.IsMajorNotificationRaisedWhenFinished,
												IsGuestRequest = config.Data.IsGuestRequest,

												FromBuildingId = whereFrom.BuildingId,
												FromBuildingName = whereFrom.BuildingName,
												FromFloorId = whereFrom.FloorId,
												FromFloorName = whereFrom.FloorName,
												FromHotelId = whereFrom.HotelId,
												FromHotelName = whereFrom.HotelName,
												FromReservationGuestName = whereFrom.ReservationGuestName,
												FromReservationId = whereFrom.ReservationId,
												FromRoomId = whereFrom.RoomId,
												FromRoomName = whereFrom.RoomName,
												FromWarehouseId = whereFrom.WarehouseId,
												FromWarehouseName = whereFrom.WarehouseName,

												ToBuildingId = whereTo.BuildingId,
												ToBuildingName = whereTo.BuildingName,
												ToFloorId = whereTo.FloorId,
												ToFloorName = whereTo.FloorName,
												ToHotelId = whereTo.HotelId,
												ToHotelName = whereTo.HotelName,
												ToReservationGuestName = whereTo.ReservationGuestName,
												ToReservationId = whereTo.ReservationId,
												ToRoomId = whereTo.RoomId,
												ToRoomName = whereTo.RoomName,
												ToWarehouseId = whereTo.WarehouseId,
												ToWarehouseName = whereTo.WarehouseName,

												ToName = null,
												FromName = null,

												Comment = config.Data.Comment,
											};

											this._setTaskFromToNames(task);

											recurringTasks.Add(task);
										}
									}
										
								}
							}
						}
					}

					break;
				case RecurringTaskType.SPECIFIC_TIME:
					foreach (var taskTime in config.Data.StartsAtTimes)
					{
						foreach (var user in users)
						{
							foreach (var whereFrom in wheresResult.WhereFroms)
							{
								foreach (var whereTo in wheresResult.WhereTos)
								{
									var taskId = Guid.NewGuid();
									var task = new ExtendedSystemTask
									{
										Actions = config.Data.Whats.Select(w => new SystemTaskAction
										{
											Id = Guid.NewGuid(),
											ActionName = w.ActionName,
											AssetId = w.AssetId,
											AssetGroupId = w.AssetGroupId,
											AssetGroupName = w.AssetGroupName,
											AssetName = w.AssetName,
											AssetQuantity = w.AssetQuantity,
											SystemTaskId = taskId,
										}).ToArray(),
										CreatedAt = DateTime.UtcNow,
										CreatedBy = null,
										CreatedById = this._userId,
										EventKey = null,
										EventModifierKey = null,
										EventTimeKey = null,
										History = new List<SystemTaskHistory>(),
										Id = taskId,
										IsManuallyModified = false,
										ModifiedAt = DateTime.UtcNow,
										ModifiedBy = null,
										ModifiedById = this._userId,
										StartsAt = taskTime,
										StatusKey = TaskStatusType.PENDING.ToString(),
										SystemTaskConfiguration = null,
										SystemTaskConfigurationId = config.Id,
										WhereTypeKey = wheresResult.WhereTypeKey,
										MustBeFinishedByAllWhos = config.Data.MustBeFinishedByAllWhos,
										TypeKey = config.Data.TaskTypeKey,
										RecurringTypeKey = config.Data.RecurringTaskTypeKey,
										RepeatsForKey = config.Data.RepeatsForKey,
										UserId = user.Id,
										IsForPlannedAttendant = user.IsForPlannedAttendant,
										UserFullName = $"{user.FirstName} {user.LastName}",
										UserUsername = user.UserName,
										Credits = config.Data.Credits,
										Price = config.Data.Price,
										PriorityKey = config.Data.PriorityKey,
										IsBlockingCleaningUntilFinished = config.Data.IsBlockingCleaningUntilFinished,
										IsShownInNewsFeed = config.Data.IsShownInNewsFeed,
										IsRescheduledEveryDayUntilFinished = config.Data.IsRescheduledEveryDayUntilFinished,
										IsMajorNotificationRaisedWhenFinished = config.Data.IsMajorNotificationRaisedWhenFinished,
										IsGuestRequest = config.Data.IsGuestRequest,

										FromBuildingId = whereFrom.BuildingId,
										FromBuildingName = whereFrom.BuildingName,
										FromFloorId = whereFrom.FloorId,
										FromFloorName = whereFrom.FloorName,
										FromHotelId = whereFrom.HotelId,
										FromHotelName = whereFrom.HotelName,
										FromReservationGuestName = whereFrom.ReservationGuestName,
										FromReservationId = whereFrom.ReservationId,
										FromRoomId = whereFrom.RoomId,
										FromRoomName = whereFrom.RoomName,
										FromWarehouseId = whereFrom.WarehouseId,
										FromWarehouseName = whereFrom.WarehouseName,

										ToBuildingId = whereTo.BuildingId,
										ToBuildingName = whereTo.BuildingName,
										ToFloorId = whereTo.FloorId,
										ToFloorName = whereTo.FloorName,
										ToHotelId = whereTo.HotelId,
										ToHotelName = whereTo.HotelName,
										ToReservationGuestName = whereTo.ReservationGuestName,
										ToReservationId = whereTo.ReservationId,
										ToRoomId = whereTo.RoomId,
										ToRoomName = whereTo.RoomName,
										ToWarehouseId = whereTo.WarehouseId,
										ToWarehouseName = whereTo.WarehouseName,

										ToName = null,
										FromName = null,

										Comment = config.Data.Comment,
									};

									this._setTaskFromToNames(task);

									recurringTasks.Add(task);
								}
							}
								
						}
					}
					break;
				case RecurringTaskType.EVERY:

					foreach (var d in dateRange.Dates)
					{
						foreach (var user in users)
						{
							foreach (var whereFrom in wheresResult.WhereFroms)
							{
								foreach (var whereTo in wheresResult.WhereTos)
								{
									var taskId = Guid.NewGuid();
									var task = new ExtendedSystemTask
									{
										Actions = config.Data.Whats.Select(w => new SystemTaskAction
										{
											Id = Guid.NewGuid(),
											ActionName = w.ActionName,
											AssetId = w.AssetId,
											AssetGroupId = w.AssetGroupId,
											AssetGroupName = w.AssetGroupName,
											AssetName = w.AssetName,
											AssetQuantity = w.AssetQuantity,
											SystemTaskId = taskId,
										}).ToArray(),
										CreatedAt = DateTime.UtcNow,
										CreatedBy = null,
										CreatedById = this._userId,
										EventKey = null,
										EventModifierKey = null,
										EventTimeKey = null,
										History = new List<SystemTaskHistory>(),
										Id = taskId,
										IsManuallyModified = false,
										ModifiedAt = DateTime.UtcNow,
										ModifiedBy = null,
										ModifiedById = this._userId,
										StartsAt = d,
										StatusKey = TaskStatusType.PENDING.ToString(),
										SystemTaskConfiguration = null,
										SystemTaskConfigurationId = config.Id,
										WhereTypeKey = wheresResult.WhereTypeKey,
										MustBeFinishedByAllWhos = config.Data.MustBeFinishedByAllWhos,
										TypeKey = config.Data.TaskTypeKey,
										RecurringTypeKey = config.Data.RecurringTaskTypeKey,
										RepeatsForKey = config.Data.RepeatsForKey,
										UserId = user.Id,
										IsForPlannedAttendant = user.IsForPlannedAttendant,
										UserFullName = $"{user.FirstName} {user.LastName}",
										UserUsername = user.UserName,
										Credits = config.Data.Credits,
										Price = config.Data.Price,
										PriorityKey = config.Data.PriorityKey,
										IsBlockingCleaningUntilFinished = config.Data.IsBlockingCleaningUntilFinished,
										IsShownInNewsFeed = config.Data.IsShownInNewsFeed,
										IsRescheduledEveryDayUntilFinished = config.Data.IsRescheduledEveryDayUntilFinished,
										IsMajorNotificationRaisedWhenFinished = config.Data.IsMajorNotificationRaisedWhenFinished,
										IsGuestRequest = config.Data.IsGuestRequest,

										FromBuildingId = whereFrom.BuildingId,
										FromBuildingName = whereFrom.BuildingName,
										FromFloorId = whereFrom.FloorId,
										FromFloorName = whereFrom.FloorName,
										FromHotelId = whereFrom.HotelId,
										FromHotelName = whereFrom.HotelName,
										FromReservationGuestName = whereFrom.ReservationGuestName,
										FromReservationId = whereFrom.ReservationId,
										FromRoomId = whereFrom.RoomId,
										FromRoomName = whereFrom.RoomName,
										FromWarehouseId = whereFrom.WarehouseId,
										FromWarehouseName = whereFrom.WarehouseName,

										ToBuildingId = whereTo.BuildingId,
										ToBuildingName = whereTo.BuildingName,
										ToFloorId = whereTo.FloorId,
										ToFloorName = whereTo.FloorName,
										ToHotelId = whereTo.HotelId,
										ToHotelName = whereTo.HotelName,
										ToReservationGuestName = whereTo.ReservationGuestName,
										ToReservationId = whereTo.ReservationId,
										ToRoomId = whereTo.RoomId,
										ToRoomName = whereTo.RoomName,
										ToWarehouseId = whereTo.WarehouseId,
										ToWarehouseName = whereTo.WarehouseName,

										ToName = null,
										FromName = null,

										Comment = config.Data.Comment,
									};

									this._setTaskFromToNames(task);

									recurringTasks.Add(task);
								}
							}

						}
					}

					break;
			}

			return recurringTasks;
		}

		private async Task<IEnumerable<Domain.Entities.ExtendedSystemTask>> _generateEventTasks(Domain.Entities.SystemTaskConfiguration config)
		{
			var eventTasks = new List<ExtendedSystemTask>();

			// FIRST YOU MUST FIND ALL EVENTS AND THEN CREATE StartsAt TASKS! -> THIS IMPLEMENTATION IS NOT CORRECT!!!!!
			// FIRST YOU MUST FIND ALL EVENTS AND THEN CREATE StartsAt TASKS! -> THIS IMPLEMENTATION IS NOT CORRECT!!!!!
			// FIRST YOU MUST FIND ALL EVENTS AND THEN CREATE StartsAt TASKS! -> THIS IMPLEMENTATION IS NOT CORRECT!!!!!
			// FIRST YOU MUST FIND ALL EVENTS AND THEN CREATE StartsAt TASKS! -> THIS IMPLEMENTATION IS NOT CORRECT!!!!!
			// FIRST YOU MUST FIND ALL EVENTS AND THEN CREATE StartsAt TASKS! -> THIS IMPLEMENTATION IS NOT CORRECT!!!!!
			// FIRST YOU MUST FIND ALL EVENTS AND THEN CREATE StartsAt TASKS! -> THIS IMPLEMENTATION IS NOT CORRECT!!!!!
			// FIRST YOU MUST FIND ALL EVENTS AND THEN CREATE StartsAt TASKS! -> THIS IMPLEMENTATION IS NOT CORRECT!!!!!
			// FIRST YOU MUST FIND ALL EVENTS AND THEN CREATE StartsAt TASKS! -> THIS IMPLEMENTATION IS NOT CORRECT!!!!!
			// FIRST YOU MUST FIND ALL EVENTS AND THEN CREATE StartsAt TASKS! -> THIS IMPLEMENTATION IS NOT CORRECT!!!!!
			// FIRST YOU MUST FIND ALL EVENTS AND THEN CREATE StartsAt TASKS! -> THIS IMPLEMENTATION IS NOT CORRECT!!!!!
			// FIRST YOU MUST FIND ALL EVENTS AND THEN CREATE StartsAt TASKS! -> THIS IMPLEMENTATION IS NOT CORRECT!!!!!

			var users = await this._loadSpecificWhos(config.Data.Whos);
			var wheresResult = await this._LoadWhereFromsAndTos(config);

			foreach (var user in users)
			{
				foreach (var whereFrom in wheresResult.WhereFroms)
				{
					foreach (var whereTo in wheresResult.WhereTos)
					{
						var taskId = Guid.NewGuid();
						var task = new ExtendedSystemTask
						{
							Id = taskId,
							Actions = config.Data.Whats.Select(w => new SystemTaskAction
							{
								Id = Guid.NewGuid(),
								ActionName = w.ActionName,
								AssetId = w.AssetId,
								AssetGroupId = w.AssetGroupId,
								AssetGroupName = w.AssetGroupName,
								AssetName = w.AssetName,
								AssetQuantity = w.AssetQuantity,
								SystemTaskId = taskId,
							}).ToArray(),
							CreatedAt = DateTime.UtcNow,
							CreatedBy = null,
							CreatedById = this._userId,
							EventKey = config.Data.EventKey,
							EventModifierKey = config.Data.EventModifierKey,
							EventTimeKey = config.Data.EventTimeKey,
							History = new List<SystemTaskHistory>(),
							IsManuallyModified = false,
							ModifiedAt = DateTime.UtcNow,
							ModifiedBy = null,
							ModifiedById = this._userId,
							StartsAt = config.Data.StartsAtTimes.First(),
							StatusKey = TaskStatusType.PENDING.ToString(),
							SystemTaskConfiguration = null,
							SystemTaskConfigurationId = config.Id,
							WhereTypeKey = wheresResult.WhereTypeKey,
							MustBeFinishedByAllWhos = config.Data.MustBeFinishedByAllWhos,
							TypeKey = config.Data.TaskTypeKey,
							RecurringTypeKey = config.Data.RecurringTaskTypeKey,
							RepeatsForKey = config.Data.RepeatsForKey,
							UserId = user.Id,
							IsForPlannedAttendant = user.IsForPlannedAttendant,
							UserFullName = $"{user.FirstName} {user.LastName}",
							UserUsername = user.UserName,
							Credits = config.Data.Credits,
							Price = config.Data.Price,
							PriorityKey = config.Data.PriorityKey,
							IsBlockingCleaningUntilFinished = config.Data.IsBlockingCleaningUntilFinished,
							IsShownInNewsFeed = config.Data.IsShownInNewsFeed,
							IsRescheduledEveryDayUntilFinished = config.Data.IsRescheduledEveryDayUntilFinished,
							IsMajorNotificationRaisedWhenFinished = config.Data.IsMajorNotificationRaisedWhenFinished,
							IsGuestRequest = config.Data.IsGuestRequest,

							FromBuildingId = whereFrom.BuildingId,
							FromBuildingName = whereFrom.BuildingName,
							FromFloorId = whereFrom.FloorId,
							FromFloorName = whereFrom.FloorName,
							FromHotelId = whereFrom.HotelId,
							FromHotelName = whereFrom.HotelName,
							FromReservationGuestName = whereFrom.ReservationGuestName,
							FromReservationId = whereFrom.ReservationId,
							FromRoomId = whereFrom.RoomId,
							FromRoomName = whereFrom.RoomName,
							FromWarehouseId = whereFrom.WarehouseId,
							FromWarehouseName = whereFrom.WarehouseName,

							ToBuildingId = whereTo.BuildingId,
							ToBuildingName = whereTo.BuildingName,
							ToFloorId = whereTo.FloorId,
							ToFloorName = whereTo.FloorName,
							ToHotelId = whereTo.HotelId,
							ToHotelName = whereTo.HotelName,
							ToReservationGuestName = whereTo.ReservationGuestName,
							ToReservationId = whereTo.ReservationId,
							ToRoomId = whereTo.RoomId,
							ToRoomName = whereTo.RoomName,
							ToWarehouseId = whereTo.WarehouseId,
							ToWarehouseName = whereTo.WarehouseName,

							ToName = null,
							FromName = null,

							Comment = config.Data.Comment,
						};

						this._setTaskFromToNames(task);

						eventTasks.Add(task);
					}
				}
			}

			return eventTasks;
		}

		private TaskRecurringDateRange _getRecurringDateRange(SystemTaskConfiguration config)
		{
			var dateRange = new TaskRecurringDateRange();

			if (config.Data.RecurringTaskTypeKey.IsNotNull())
			{
				var recurringType = (RecurringTaskType)Enum.Parse(typeof(RecurringTaskType), config.Data.RecurringTaskTypeKey);
				switch (recurringType)
				{
					case RecurringTaskType.DAILY:
						dateRange.StartDate = config.Data.StartsAtTimes.First();
						dateRange.EndDate = dateRange.StartDate;

						switch (config.Data.RepeatsForKey)
						{
							case "NUMBER_OF_DAYS":
								dateRange.EndDate = dateRange.EndDate.AddDays(config.Data.RepeatsForNrDays.Value);
								dateRange.UseDateInterval = true;
								break;
							case "NUMBER_OF_OCCURENCES":
								dateRange.MaxOccurences = config.Data.RepeatsForNrOccurences.Value;
								dateRange.UseOccurences = true;
								break;
							case "SPECIFIC_DATE":
								dateRange.EndDate = config.Data.RepeatsUntilTime.Value;
								dateRange.UseDateInterval = true;
								break;
						}

						break;
					case RecurringTaskType.WEEKLY:
						dateRange.StartDate = config.Data.StartsAtTimes.First();
						dateRange.EndDate = dateRange.StartDate;

						switch (config.Data.RepeatsForKey)
						{
							case "NUMBER_OF_DAYS":
								dateRange.EndDate = dateRange.EndDate.AddDays(config.Data.RepeatsForNrDays.Value);
								dateRange.UseDateInterval = true;
								break;
							case "NUMBER_OF_OCCURENCES":
								dateRange.MaxOccurences = config.Data.RepeatsForNrOccurences.Value;
								dateRange.UseOccurences = true;
								break;
							case "SPECIFIC_DATE":
								dateRange.EndDate = config.Data.RepeatsUntilTime.Value;
								dateRange.UseDateInterval = true;
								break;
						}

						break;
					case RecurringTaskType.MONTHLY:
						dateRange.StartDate = config.Data.StartsAtTimes.First();
						dateRange.EndDate = dateRange.StartDate;

						switch (config.Data.RepeatsForKey)
						{
							case "NUMBER_OF_DAYS":
								//dateRange.EndDate = new DateTime(dateRange.StartDate.Year, dateRange.StartDate.Month, 1).AddMonths(config.Data.RepeatsForNrDays.Value);
								//dateRange.EndDate = dateRange.EndDate.AddDays(DateTime.DaysInMonth(dateRange.EndDate.Year, dateRange.EndDate.Month));
								dateRange.EndDate = dateRange.EndDate.AddDays(config.Data.RepeatsForNrDays.Value);
								dateRange.UseDateInterval = true;
								break;
							case "NUMBER_OF_OCCURENCES":
								dateRange.MaxOccurences = config.Data.RepeatsForNrOccurences.Value;
								dateRange.UseOccurences = true;
								break;
							case "SPECIFIC_DATE":
								dateRange.EndDate = config.Data.RepeatsUntilTime.Value;
								dateRange.UseDateInterval = true;
								break;
						}

						break;

					case RecurringTaskType.SPECIFIC_TIME:
						dateRange.Dates = config.Data.StartsAtTimes.ToList();
						dateRange.UseDateList = true;
						break;

					case RecurringTaskType.EVERY:
						dateRange.StartDate = config.Data.StartsAtTimes.First();
						dateRange.EndDate = dateRange.StartDate;

						switch (config.Data.RepeatsForKey)
						{
							case "NUMBER_OF_DAYS":
								var nrFromDate = dateRange.StartDate;
								var nrToDate = nrFromDate.AddDays(config.Data.RepeatsForNrDays.Value);
								var nrDates = new List<DateTime>();
								if (nrFromDate < nrToDate)
								{
									var nrCurrentDate = nrFromDate;
									while (nrCurrentDate <= nrToDate)
									{
										nrDates.Add(nrCurrentDate);
										nrCurrentDate = nrCurrentDate.AddDays(config.Data.RecurringEveryNumberOfDays);
									}
								}

								if (nrDates.Any())
								{
									dateRange.EndDate = nrDates.Last();
								}
								else
								{
									dateRange.EndDate = nrToDate;
								}

								dateRange.Dates = nrDates;
								dateRange.UseDateList = true;
								break;
							case "NUMBER_OF_OCCURENCES":
								var noFromDate = dateRange.StartDate;
								var noCurrentAt = noFromDate;
								var noDates = new List<DateTime>();

								if (config.Data.RepeatsForNrOccurences.Value > 0)
								{
									noDates.Add(noCurrentAt);
									dateRange.EndDate = noCurrentAt;

									for (int oIndex = 1; oIndex < config.Data.RepeatsForNrOccurences.Value; oIndex++)
									{
										noCurrentAt = noCurrentAt.AddDays(config.Data.RecurringEveryNumberOfDays);
										noDates.Add(noCurrentAt);
										dateRange.EndDate = noCurrentAt;
									}
								}

								dateRange.Dates = noDates;
								dateRange.UseDateList = true;
								break;
							case "SPECIFIC_DATE":
								var sFromDate = dateRange.StartDate;
								var sToDate = config.Data.RepeatsUntilTime.Value;
								var sDates = new List<DateTime>();
								if(config.Data.RecurringEveryNumberOfDays > 0 && sFromDate <= sToDate)
								{
									var currentAt = sFromDate;
									while(currentAt <= sToDate)
									{
										sDates.Add(currentAt);
										currentAt = currentAt.AddDays(config.Data.RecurringEveryNumberOfDays);
									}
								}

								if (sDates.Any())
								{
									dateRange.EndDate = sDates.Last();
								}
								else
								{
									dateRange.EndDate = sToDate;
								}

								dateRange.Dates = sDates;
								dateRange.UseDateList = true;
								break;
						}

						break;

					default:
						throw new NotSupportedException($"{recurringType.ToString()} recurring type is not supported for creating recurring date range. Implementation is missing");
				}
			}

			return dateRange;
		}

		private IEnumerable<DateTime> _generateDateList(DateTime startDate, DateTime endDate)
		{
			var dates = new List<DateTime>();

			var date = startDate;
			while (date < endDate)
			{
				dates.Add(date);
				date = date.AddDays(1);
			}

			return dates;
		}
		public SystemTaskHistory GenerateTaskHistory(string changeByKey, string message, SystemTask t, SystemTaskHistoryData oldData, SystemTaskHistoryData newData)
		{
			return new SystemTaskHistory
			{
				Id = Guid.NewGuid(),
				ChangedByKey = changeByKey, // ADMIN
				CreatedAt = DateTime.UtcNow,
				CreatedBy = null,
				CreatedById = this._userId,
				Message = message,
				SystemTaskId = t.Id,
				NewData = newData,
				OldData = oldData
			};
		}
		public SystemTaskHistoryData GenerateTaskHistoryData(SystemTask t)
		{
			return new SystemTaskHistoryData
			{
				Actions = t.Actions.Select(w => new SystemTaskHistoryActionData
				{
					Id = w.Id,
					ActionName = w.ActionName,
					AssetId = w.AssetId,
					AssetGroupId = w.AssetGroupId,
					AssetGroupName = w.AssetGroupName,
					AssetName = w.AssetName,
					AssetQuantity = w.AssetQuantity,
				}).ToArray(),
				EventKey = t.EventKey,
				EventModifierKey = t.EventModifierKey,
				EventTimeKey = t.EventTimeKey,
				IsManuallyModified = t.IsManuallyModified,
				StartsAt = t.StartsAt,
				StatusKey = t.StatusKey,
				MustBeFinishedByAllWhos = t.MustBeFinishedByAllWhos,
				RecurringTypeKey = t.RecurringTypeKey,
				RepeatsForKey = t.RepeatsForKey,
				TypeKey = t.TypeKey,
				UserId = t.UserId,
				IsForPlannedAttendant = t.IsForPlannedAttendant,
				WhereTypeKey = t.WhereTypeKey,
				Credits = t.Credits,
				IsGuestRequest = t.IsGuestRequest,
				PriorityKey = t.PriorityKey,
				IsMajorNotificationRaisedWhenFinished = t.IsMajorNotificationRaisedWhenFinished,
				IsRescheduledEveryDayUntilFinished = t.IsRescheduledEveryDayUntilFinished,
				IsShownInNewsFeed = t.IsShownInNewsFeed,
				IsBlockingCleaningUntilFinished = t.IsBlockingCleaningUntilFinished,
				Price = t.Price,
				FromName = t.FromName,
				ToName = t.ToName,
				FromHotelId = t.FromHotelId,
				FromHotelName = t.FromHotel == null ? null : t.FromHotel.Name,
				FromReservationId = t.FromReservationId,
				FromRoomId = t.FromRoomId,
				FromWarehouseId = t.FromWarehouseId,
				ToHotelId = t.ToHotelId,
				ToHotelName = t.ToHotel == null ? null : t.ToHotel.Name,
				ToReservationId = t.ToReservationId,
				ToRoomId = t.ToRoomId,
				ToWarehouseId = t.ToWarehouseId,

				Comment = t.Comment,
			};
		}
	}


	public class SaveSingleTaskOptions
	{
		public Guid Id { get; set; }
		public string StartsAtString { get; set; }
	}

	public class SaveDailyRecurringTaskOptions
	{
		public Guid Id { get; set; }
		public string StartsAtString { get; set; }
		public string[] RepeatTimes { get; set; }

		public string RepeatsForKey { get; set; }
		public int? RepeatsForNrOccurences { get; set; }
		public int? RepeatsForNrDays { get; set; }
		public string RepeatsUntilTimeString { get; set; }
	}

	public class SaveWeeklyRecurringTaskItemOptions : WeeklyRecurringTaskItemOptions
	{
	}

	public class SaveMonthlyRecurringTaskOptions
	{
		public Guid Id { get; set; }
		public string StartsAtString { get; set; }
		public IEnumerable<SaveMonthlyRecurringTaskItemOptions> MonthlyRecurrences { get; set; }
		public string RepeatsForKey { get; set; }
		public int? RepeatsForNrOccurences { get; set; }
		public int? RepeatsForNrDays { get; set; }
		public string RepeatsUntilTimeString { get; set; }
	}

	public class SaveWeeklyRecurringTaskOptions
	{
		public Guid Id { get; set; }
		public string StartsAtString { get; set; }
		public IEnumerable<SaveWeeklyRecurringTaskItemOptions> WeeklyRecurrences { get; set; }

		public string RepeatsForKey { get; set; }
		public int? RepeatsForNrOccurences { get; set; }
		public int? RepeatsForNrDays { get; set; }
		public string RepeatsUntilTimeString { get; set; }
	}
	public class SaveRecurringEveryTaskOptions
	{
		public string StartsAtString { get; set; }
		public int EveryNumberOfDays { get; set; }
		public string RepeatsForKey { get; set; }
		public int? RepeatsForNrOccurences { get; set; }
		public int? RepeatsForNrDays { get; set; }
		public string RepeatsUntilTimeString { get; set; }
	}

	public class SaveMonthlyRecurringTaskItemOptions : MonthlyRecurringTaskItemOptions
	{
	}
	public class SaveSpecificTimesRecurringTaskOptions
	{
		public Guid Id { get; set; }
		public IEnumerable<string> StartsAtStrings { get; set; }
	}
	public class SaveEventTaskOptions
	{
		public Guid Id { get; set; }
		public string StartsAtString { get; set; }

		public string EventModifierKey { get; set; } // nullable
		public string EventKey { get; set; } // nullable
		public string EventTimeKey { get; set; } // nullable
		public string RepeatsForKey { get; set; }
		public int? RepeatsForNrOccurences { get; set; }
		public int? RepeatsForNrDays { get; set; }
		public string RepeatsUntilTimeString { get; set; }
	}

	public class SaveBalancedTaskOptions
	{
		public Guid Id { get; set; }
		public string StartsAtString { get; set; }
		public string EndsAtString { get; set; }
		public bool ExcludeWeekends { get; set; }
		public bool ExcludeHolidays { get; set; }
		public bool PostponeWhenRoomIsOccupied { get; set; }
	}
	public class SaveTaskFileData : TaskFileData
	{
	}
	public class SaveTaskFileDetailsData : TaskFileDetailsData
	{
	}
	public class SaveTaskWhatData : TaskWhatData
	{
	}
	public class SaveTaskWhereMoveData : TaskWhereMoveData
	{
	}

	public class SaveTaskWhoData : TaskWhoData
	{
	}
	public class SaveTaskWhereData : TaskWhereData
	{
	}
	public class InsertTaskConfigurationResult
	{
		public Guid Id { get; set; }
		public IEnumerable<InsertTaskConfigurationFileResult> Files { get; set; }
	}

	public class InsertTaskConfigurationFileResult
	{
		public Guid FileId { get; set; }
		public string FileName { get; set; }
		public string FileUrl { get; set; }
	}

	public class SaveTaskConfigurationRequest
	{
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
		public IEnumerable<SaveTaskWhatData> Whats { get; set; }

		public IEnumerable<SaveTaskWhoData> Whos { get; set; }
		public IEnumerable<SaveTaskWhereData> Wheres { get; set; }
		public IEnumerable<SaveTaskFileData> Files { get; set; }
		public IEnumerable<string> FilestackImageUrls { get; set; }
		public SaveSingleTaskOptions SingleTaskOptions { get; set; }
		public SaveBalancedTaskOptions BalancedTaskOptions { get; set; }
		public SaveDailyRecurringTaskOptions DailyRecurringTaskOptions { get; set; }
		public SaveWeeklyRecurringTaskOptions WeeklyRecurringTaskOptions { get; set; }
		public SaveMonthlyRecurringTaskOptions MonthlyRecurringTaskOptions { get; set; }
		public SaveSpecificTimesRecurringTaskOptions SpecificTimesRecurringTaskOptions { get; set; }
		public SaveEventTaskOptions EventTaskOptions { get; set; }
		public SaveRecurringEveryTaskOptions RecurringEveryTaskOptions { get; set; }
	}

	public class InsertTaskConfigurationCommand : SaveTaskConfigurationRequest, IRequest<ProcessResponse<InsertTaskConfigurationResult>>
	{
	}

	public class InsertTaskConfigurationCommandHandler : IRequestHandler<InsertTaskConfigurationCommand, ProcessResponse<InsertTaskConfigurationResult>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IFileService _fileService;
		private readonly ISystemTaskGenerator _systemTaskGenerator;
		private readonly ISystemEventsService _systemEventsService;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;

		public InsertTaskConfigurationCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, IFileService fileService, ISystemTaskGenerator systemTaskGenerator, ISystemEventsService systemEventsService)
		{
			this._databaseContext = databaseContext;
			this._fileService = fileService;
			this._systemTaskGenerator = systemTaskGenerator;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HotelGroupId();
			this._systemEventsService = systemEventsService;
		}

		public async Task<ProcessResponse<InsertTaskConfigurationResult>> Handle(InsertTaskConfigurationCommand request, CancellationToken cancellationToken)
		{
			var taskConfiguration = this._systemTaskGenerator.GenerateTaskConfiguration(request);
			var taskType = (TaskType)Enum.Parse(typeof(TaskType), request.TaskTypeKey);
			var tasks = await this._systemTaskGenerator.GenerateTasks(taskType, taskConfiguration);
			var taskHistories = this._generateHistories(tasks);

			//var filesToInsert = new List<Domain.Entities.File>();
			// copy files from temp storage to actual storage
			//foreach (var file in taskConfiguration.Data.Files)
			//{
			//	var tempFileStoragePath = this._fileService.GetTemporaryFileUploadPath(file.FileName);
			//	var fileStorageDirectory = this._fileService.GetTaskConfigurationFileStoragePath(taskConfiguration.Id);
			//	var fileStoragePath = this._fileService.GetTaskConfigurationFileStoragePath(taskConfiguration.Id, file.FileName);
			//	if (!System.IO.Directory.Exists(fileStorageDirectory))
			//	{
			//		System.IO.Directory.CreateDirectory(fileStorageDirectory);
			//	}
			//	System.IO.File.Copy(tempFileStoragePath, fileStoragePath, true);

			//	var fileBackup = new File
			//	{
			//		Id = Guid.NewGuid(),
			//		FileName = file.FileName,
			//		FileTypeKey = "TASK_CONFIGURATION",
			//		FileData = await System.IO.File.ReadAllBytesAsync(fileStoragePath),
			//		CreatedAt = DateTime.UtcNow,
			//		CreatedById = this._userId,
			//		ModifiedAt = DateTime.UtcNow,
			//		ModifiedById = this._userId
			//	};

			//	filesToInsert.Add(fileBackup);

			//	file.FileUrl = this._fileService.GetTaskConfigurationFileUrl(taskConfiguration.Id, file.FileName);
			//	file.FileId = fileBackup.Id;
			//}

			var tasksForPlannedAttendants = tasks.Where(t => t.IsForPlannedAttendant).ToArray();
			await this._UpdateTodaysTasksForPlannedAttendants(tasksForPlannedAttendants);

			await this._databaseContext.SystemTaskConfigurations.AddAsync(taskConfiguration);
			await this._databaseContext.SystemTasks.AddRangeAsync(tasks);
			await this._databaseContext.SystemTaskHistorys.AddRangeAsync(taskHistories);
			//await this._databaseContext.Files.AddRangeAsync(filesToInsert);

			await this._databaseContext.SaveChangesAsync(cancellationToken);


			var userIds = new HashSet<Guid>();
			var taskIds = new List<Guid>();
			foreach(var task in tasks)
			{
				if(task.UserId.HasValue && !userIds.Contains(task.UserId.Value)) userIds.Add(task.UserId.Value);
				taskIds.Add(task.Id);
			}

			await this._systemEventsService.TasksChanged(this._hotelGroupId, userIds, taskIds, "You have new tasks.");

			return new ProcessResponse<InsertTaskConfigurationResult>
			{
				HasError = false,
				IsSuccess = true,
				Message = "Task(s) created.",
				Data = new InsertTaskConfigurationResult
				{
					Id = taskConfiguration.Id,
					Files = new InsertTaskConfigurationFileResult[0],
					//taskConfiguration.Data.Files.Select(f => new InsertTaskConfigurationFileResult
					//{
					//	FileId = f.FileId,
					//	FileName = f.FileName,
					//	FileUrl = f.FileUrl
					//})
				}
			};
		}


		/// <summary>
		/// TODO: FIX THIS METHOD TO INCLUDE BEDS
		/// </summary>
		/// <param name="tasksForPlannedAttendants"></param>
		/// <returns></returns>
		private async Task _UpdateTodaysTasksForPlannedAttendants(IEnumerable<ExtendedSystemTask> tasksForPlannedAttendants)
		{
			if (tasksForPlannedAttendants != null && tasksForPlannedAttendants.Any())
			{
				var nowUtc = DateTime.UtcNow;
				var hotelIds = tasksForPlannedAttendants.SelectMany(t => new string[] { t.FromHotelId, t.ToHotelId }).Where(hotelId => hotelId != null).Distinct().ToArray();
				var hotels = await this._databaseContext.Hotels.Where(h => hotelIds.Contains(h.Id)).ToDictionaryAsync(h => h.Id);
				var localHotelDates = new Dictionary<string, DateTime>();

				foreach (var hotel in hotels.Values)
				{
					var timeZoneId = HotelLocalDateProvider.GetAvailableTimeZoneId(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
					var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
					localHotelDates.Add(hotel.Id, TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZoneInfo));
				}

				foreach (var t in tasksForPlannedAttendants)
				{
					var localHotelDate = DateTime.MinValue;
					if (t.ToHotelId != null && localHotelDates.ContainsKey(t.ToHotelId))
					{
						localHotelDate = localHotelDates[t.ToHotelId];
					}
					else if (t.FromHotelId != null && localHotelDates.ContainsKey(t.FromHotelId))
					{
						localHotelDate = localHotelDates[t.FromHotelId];
					}
					else
					{
						continue;
					}

					// if it is the todays task.
					if (t.StartsAt.Date == localHotelDate.Date)
					{
						if (!t.ToRoomId.HasValue && !t.FromRoomId.HasValue)
						{
							continue;
						}

						var roomId = t.ToRoomId.HasValue ? t.ToRoomId.Value : t.FromRoomId.Value;
						var date = localHotelDate.Date;

						// Find out if there is anyone cleaning this room today!
						var activeCleaning = await this._databaseContext.Cleanings.Where(c => c.IsActive && c.RoomId == roomId && c.StartsAt.Date == date).FirstOrDefaultAsync();

						// There are no active cleanings
						if (activeCleaning == null) continue;

						// Find active cleaning cleaning group
						var cleaningGroup = await this._databaseContext.CleaningPlanGroups.Include(cpg => cpg.Cleaner).Where(g => g.Items.Any(gi => gi.CleaningId == activeCleaning.Id)).FirstOrDefaultAsync();

						// Cleaning group doesn't exist (for some reason)
						if (cleaningGroup == null || cleaningGroup.Cleaner == null) continue;

						// Assign a task to the cleaner
						t.UserId = cleaningGroup.Cleaner.Id;
						t.UserFullName = $"{cleaningGroup.Cleaner.FirstName} {cleaningGroup.Cleaner.LastName}";
						t.UserUsername = cleaningGroup.Cleaner.UserName;
					}
				}
			}
		}

		private IEnumerable<Domain.Entities.SystemTaskHistory> _generateHistories(IEnumerable<SystemTask> tasks)
		{
			return tasks.Select(t => new SystemTaskHistory
			{
				Id = Guid.NewGuid(),
				ChangedByKey = "ADMIN",
				CreatedAt = DateTime.UtcNow,
				CreatedBy = null,
				CreatedById = this._userId,
				Message = "Task created.",
				SystemTaskId = t.Id,
				NewData = new SystemTaskHistoryData
				{
					Actions = t.Actions.Select(w => new SystemTaskHistoryActionData
					{
						Id = w.Id,
						ActionName = w.ActionName,
						AssetId = w.AssetId,
						AssetGroupId = w.AssetGroupId,
						AssetGroupName = w.AssetGroupName,
						AssetName = w.AssetName,
						AssetQuantity = w.AssetQuantity,
					}).ToArray(),
					EventKey = t.EventKey,
					EventModifierKey = t.EventModifierKey,
					EventTimeKey = t.EventTimeKey,
					IsManuallyModified = t.IsManuallyModified,
					StartsAt = t.StartsAt,
					StatusKey = t.StatusKey,
					MustBeFinishedByAllWhos = t.MustBeFinishedByAllWhos,
					RecurringTypeKey = t.RecurringTypeKey,
					RepeatsForKey = t.RepeatsForKey,
					TypeKey = t.TypeKey,
					UserId = t.UserId,
					WhereTypeKey = t.WhereTypeKey,
					Credits = t.Credits,
					Price = t.Price,
					IsBlockingCleaningUntilFinished = t.IsBlockingCleaningUntilFinished,
					IsShownInNewsFeed = t.IsShownInNewsFeed,
					IsRescheduledEveryDayUntilFinished = t.IsRescheduledEveryDayUntilFinished,
					IsMajorNotificationRaisedWhenFinished = t.IsMajorNotificationRaisedWhenFinished,
					PriorityKey = t.PriorityKey,
					IsGuestRequest = t.IsGuestRequest,
					FromName = t.FromName,
					ToName = t.ToName,
					FromHotelId = t.FromHotelId,
					FromHotelName = t.FromHotel == null ? null : t.FromHotel.Name,
					FromReservationId = t.FromReservationId,
					FromRoomId = t.FromRoomId,
					FromWarehouseId = t.FromWarehouseId,
					ToHotelId = t.ToHotelId,
					ToHotelName = t.ToHotel == null ? null : t.ToHotel.Name,
					ToReservationId = t.ToReservationId,
					ToRoomId = t.ToRoomId,
					ToWarehouseId = t.ToWarehouseId,
				},
				OldData = new SystemTaskHistoryData
				{
					Actions = t.Actions.Select(w => new SystemTaskHistoryActionData
					{
						Id = w.Id,
						ActionName = w.ActionName,
						AssetId = w.AssetId,
						AssetGroupId = w.AssetGroupId,
						AssetGroupName = w.AssetGroupName,
						AssetName = w.AssetName,
						AssetQuantity = w.AssetQuantity,
					}).ToArray(),
					EventKey = t.EventKey,
					EventModifierKey = t.EventModifierKey,
					EventTimeKey = t.EventTimeKey,
					IsManuallyModified = t.IsManuallyModified,
					StartsAt = t.StartsAt,
					StatusKey = t.StatusKey,
					MustBeFinishedByAllWhos = t.MustBeFinishedByAllWhos,
					RecurringTypeKey = t.RecurringTypeKey,
					RepeatsForKey = t.RepeatsForKey,
					TypeKey = t.TypeKey,
					UserId = t.UserId,
					WhereTypeKey = t.WhereTypeKey,
					Credits = t.Credits,
					Price = t.Price,
					IsBlockingCleaningUntilFinished = t.IsBlockingCleaningUntilFinished,
					IsShownInNewsFeed = t.IsShownInNewsFeed,
					IsRescheduledEveryDayUntilFinished = t.IsRescheduledEveryDayUntilFinished,
					IsMajorNotificationRaisedWhenFinished = t.IsMajorNotificationRaisedWhenFinished,
					PriorityKey = t.PriorityKey,
					IsGuestRequest = t.IsGuestRequest,
					FromName = t.FromName,
					ToName = t.ToName,
					FromHotelId = t.FromHotelId,
					FromHotelName = t.FromHotel == null ? null : t.FromHotel.Name,
					FromReservationId = t.FromReservationId,
					FromRoomId = t.FromRoomId,
					FromWarehouseId = t.FromWarehouseId,
					ToHotelId = t.ToHotelId,
					ToHotelName = t.ToHotel == null ? null : t.ToHotel.Name,
					ToReservationId = t.ToReservationId,
					ToRoomId = t.ToRoomId,
					ToWarehouseId = t.ToWarehouseId,
				}
			}).ToArray();
		}
	}
}
