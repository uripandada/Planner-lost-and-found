using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Commands.InsertTaskConfiguration;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Commands.UpdateTaskConfiguration
{
	public class UpdateTaskConfigurationResult
	{
		//public IEnumerable<InsertTaskConfigurationFileResult> Files { get; set; }
	}

	public class UpdateTaskConfigurationCommand : SaveTaskConfigurationRequest, IRequest<ProcessResponse<UpdateTaskConfigurationResult>>
	{
		public Guid Id { get; set; }
	}

	public class UpdateTaskConfigurationCommandHandler : IRequestHandler<UpdateTaskConfigurationCommand, ProcessResponse<UpdateTaskConfigurationResult>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IFileService _fileService;
		private readonly ISystemTaskGenerator _systemTaskGenerator;
		private readonly ISystemEventsService _systemEventsService;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;

		public UpdateTaskConfigurationCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, IFileService fileService, ISystemTaskGenerator systemTaskGenerator, ISystemEventsService systemEventsService)
		{
			this._databaseContext = databaseContext;
			this._fileService = fileService;
			this._systemTaskGenerator = systemTaskGenerator;
			this._systemEventsService = systemEventsService;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HotelGroupId();
		}

		public async Task<ProcessResponse<UpdateTaskConfigurationResult>> Handle(UpdateTaskConfigurationCommand request, CancellationToken cancellationToken)
		{
			var taskConfiguration = this._systemTaskGenerator.GenerateTaskConfiguration(request);
			taskConfiguration.Id = request.Id;

			var taskType = (TaskType)Enum.Parse(typeof(TaskType), request.TaskTypeKey);
			var newTasks = await this._systemTaskGenerator.GenerateTasks(taskType, taskConfiguration);

			var existingTaskConfiguration = await this._databaseContext
				.SystemTaskConfigurations
				.Include(stc => stc.Tasks)
				.ThenInclude(t => t.Actions)
				.Where(stc => stc.Id == request.Id)
				.FirstOrDefaultAsync();

			//var hotel

			if(existingTaskConfiguration == null)
			{
				return new ProcessResponse<UpdateTaskConfigurationResult>
				{
					Data = null,
					HasError = true,
					IsSuccess = false,
					Message = "Update failed. Unable to find task configuration."
				};
			}

			//var now = DateTime.UtcNow;
			var tasksToCancel = new List<SystemTask>();
			var tasksThatArePassed = new List<SystemTask>();
			var tasksThatAreManuallyModified = new List<SystemTask>();
			var tasksInProgress = new List<SystemTask>();

			var tasksToCreate = new List<SystemTask>();
			var tasksThatAreNewButPassed = new List<SystemTask>();

			var taskHistories = new List<SystemTaskHistory>();

			// C: Extract hotel ids from the tasks in order to load proper time zones
			var hotelIdsSet = new HashSet<string>();
			foreach(var task in existingTaskConfiguration.Tasks)
			{
				if (task.FromHotelId.IsNotNull())
				{
					// FROM HOTEL IS ONLY SET ON TASKS THAT HAVE FROM-TO
					// -> IN THAT CASE, THE TASK MUST BE IN THE FROM HOTEL TIME ZONE
					if (!hotelIdsSet.Contains(task.FromHotelId))
					{
						hotelIdsSet.Add(task.FromHotelId);
					}
				}
				else if(task.ToHotelId.IsNotNull())
				{
					// THIS SHOULD ALWAYS BE THE CASE
					if (!hotelIdsSet.Contains(task.ToHotelId))
					{
						hotelIdsSet.Add(task.ToHotelId);
					}
				}
			}
			// /C: Extract hotel ids


			// C: Load hotel time zones from the DB
			var hotelIds = hotelIdsSet.ToArray();
			var hotelTimeZonesMap = await this._databaseContext
				.Hotels
				.Where(h => hotelIds.Contains(h.Id))
				.Select(h => new HotelTimeZone 
				{ 
					HotelId = h.Id,
					IanaTimeZoneId = h.IanaTimeZoneId,
					WindowsTimeZoneId = h.WindowsTimeZoneId
				})
				.ToDictionaryAsync(htz => htz.HotelId, htz => htz);

			foreach(var htz in hotelTimeZonesMap.Values)
			{
				var timeZoneId = HotelLocalDateProvider.GetAvailableTimeZoneId(htz.WindowsTimeZoneId, htz.IanaTimeZoneId);
				htz.CurrentHotelTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneConverter.TZConvert.GetTimeZoneInfo(timeZoneId));
			}
			// /C: Load hotel time zones

			foreach(var task in existingTaskConfiguration.Tasks)
			{
				var currentHotelTime = task.FromHotelId.IsNotNull() ? hotelTimeZonesMap[task.FromHotelId].CurrentHotelTime : hotelTimeZonesMap[task.ToHotelId].CurrentHotelTime;

				if (task.IsManuallyModified)
				{
					tasksThatAreManuallyModified.Add(task);
					continue;
				}

				if(task.StartsAt < currentHotelTime)
				{
					tasksThatArePassed.Add(task);
					//continue;
				}

				if(task.StatusKey == TaskStatusType.PENDING.ToString() || task.StatusKey == TaskStatusType.WAITING.ToString())
				{
					tasksToCancel.Add(task);
				}
				else
				{
					tasksInProgress.Add(task);
				}
			}

			foreach(var task in newTasks)
			{
				var currentHotelTime = task.FromHotelId.IsNotNull() ? hotelTimeZonesMap[task.FromHotelId].CurrentHotelTime : hotelTimeZonesMap[task.ToHotelId].CurrentHotelTime;

				if (task.StartsAt < currentHotelTime)
				{
					tasksThatAreNewButPassed.Add(task);
				}
				else
				{
					tasksToCreate.Add(task);
				}
			}

			existingTaskConfiguration.Data = taskConfiguration.Data;
			existingTaskConfiguration.ModifiedAt = DateTime.UtcNow;
			existingTaskConfiguration.ModifiedById = this._userId;

			var tasksForPlannedAttendants = new List<SystemTask>();
			
			foreach (var task in tasksToCreate)
			{
				var taskHistoryData = this._systemTaskGenerator.GenerateTaskHistoryData(task);
				var taskHistory = this._systemTaskGenerator.GenerateTaskHistory("ADMIN", "Task created on task configuration update.", task, taskHistoryData, taskHistoryData);
				taskHistories.Add(taskHistory);

				if (task.IsForPlannedAttendant) tasksForPlannedAttendants.Add(task);
			}

			foreach (var task in tasksThatAreNewButPassed)
			{
				var taskHistoryData = this._systemTaskGenerator.GenerateTaskHistoryData(task);
				var taskHistory = this._systemTaskGenerator.GenerateTaskHistory("ADMIN", "Task created on task configuration update.", task, taskHistoryData, taskHistoryData);
				taskHistories.Add(taskHistory);

				if (task.IsForPlannedAttendant) tasksForPlannedAttendants.Add(task);
			}

			foreach (var taskToCancel in tasksToCancel)
			{
				var oldTaskHistoryData = this._systemTaskGenerator.GenerateTaskHistoryData(taskToCancel);

				taskToCancel.StatusKey = TaskStatusType.CANCELLED.ToString();
				taskToCancel.ModifiedAt = DateTime.UtcNow;
				taskToCancel.ModifiedById = this._userId;

				var newTaskHistoryData = this._systemTaskGenerator.GenerateTaskHistoryData(taskToCancel);

				var taskHistory = this._systemTaskGenerator.GenerateTaskHistory("ADMIN", "Task cancelled on task configuration update.", taskToCancel, oldTaskHistoryData, newTaskHistoryData);
				taskHistories.Add(taskHistory);
			}

			if (taskHistories.Any())
			{
				if (tasksForPlannedAttendants.Any())
				{
					await this._UpdateTodaysTasksForPlannedAttendants(tasksForPlannedAttendants);
				}

				if (tasksToCreate.Any())
				{
					await this._databaseContext.SystemTasks.AddRangeAsync(tasksToCreate);
				}

				if (tasksThatAreNewButPassed.Any())
				{
					await this._databaseContext.SystemTasks.AddRangeAsync(tasksThatAreNewButPassed);
				}

				await this._databaseContext.SystemTaskHistorys.AddRangeAsync(taskHistories);
				await this._databaseContext.SaveChangesAsync(cancellationToken);
			}

			var taskIds = new List<Guid>();
			var userIds = new HashSet<Guid>();

			foreach(var task in tasksToCreate)
			{
				if (task.UserId.HasValue && !userIds.Contains(task.UserId.Value)) userIds.Add(task.UserId.Value);
				taskIds.Add(task.Id);
			}

			foreach(var task in tasksThatAreNewButPassed)
			{
				if (task.UserId.HasValue && !userIds.Contains(task.UserId.Value)) userIds.Add(task.UserId.Value);
				taskIds.Add(task.Id);
			}

			foreach(var task in tasksInProgress)
			{
				if (task.UserId.HasValue && !userIds.Contains(task.UserId.Value)) userIds.Add(task.UserId.Value);
				taskIds.Add(task.Id);
			}

			foreach(var task in tasksThatAreManuallyModified)
			{
				if (task.UserId.HasValue && !userIds.Contains(task.UserId.Value)) userIds.Add(task.UserId.Value);
				taskIds.Add(task.Id);
			}

			foreach(var task in tasksThatArePassed)
			{
				if (task.UserId.HasValue && !userIds.Contains(task.UserId.Value)) userIds.Add(task.UserId.Value);
				taskIds.Add(task.Id);
			}

			foreach(var task in tasksToCancel)
			{
				if (task.UserId.HasValue && !userIds.Contains(task.UserId.Value)) userIds.Add(task.UserId.Value);
				taskIds.Add(task.Id);
			}

			await this._systemEventsService.TasksChanged(this._hotelGroupId, userIds, taskIds, "Your tasks have changed");

			return new ProcessResponse<UpdateTaskConfigurationResult>
			{
				HasError = false,
				IsSuccess = true,
				Message = "Task(s) updated.",
				Data = new UpdateTaskConfigurationResult
				{
					//Files = taskConfiguration.Data.Files.Select(f => new InsertTaskConfigurationFileResult
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
		private async Task _UpdateTodaysTasksForPlannedAttendants(IEnumerable<SystemTask> tasksForPlannedAttendants)
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
						var cleaningGroup = await this._databaseContext.CleaningPlanGroups.Where(g => g.Items.Any(gi => gi.CleaningId == activeCleaning.Id)).FirstOrDefaultAsync();

						// Cleaning group doesn't exist (for some reason)
						if (cleaningGroup == null) continue;

						// Assign a task to the cleaner
						t.UserId = cleaningGroup.CleanerId;
					}
				}
			}
		}

	}
}
