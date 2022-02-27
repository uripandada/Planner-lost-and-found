using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Queries.GetTaskHistory
{
	public class TaskHistoryItemViewModel
	{
		public TaskHistoryItemViewModel()
		{
			this.Changes = new List<TaskPropertyChangeViewModel>();
		}

		public Guid Id { get; set; }
		public string Message { get; set; }
		public string CreatedByUserFullName { get; set; }
		public string CreatedByInitials { get; set; }
		public bool HasAvatar { get; set; }
		public string AvatarUrl { get; set; }
		public string CreatedAtString { get; set; }
		public List<TaskPropertyChangeViewModel> Changes { get; set; }
	}

	public class TaskPropertyChangeViewModel
	{
		public string PropertyName { get; set; }
		public string OldValue { get; set; }
		public string NewValue { get; set; }
	}

	public class GetTaskHistoryQuery : IRequest<PageOf<TaskHistoryItemViewModel>>
	{
		public Guid TaskId { get; set; }
	}

	public class GetTaskHistoryQueryHandler : IRequestHandler<GetTaskHistoryQuery, PageOf<TaskHistoryItemViewModel>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetTaskHistoryQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<PageOf<TaskHistoryItemViewModel>> Handle(GetTaskHistoryQuery request, CancellationToken cancellationToken)
		{
			var history = await this._databaseContext
				.SystemTaskHistorys
				.Include(h => h.CreatedBy)
				.Where(h => h.SystemTaskId == request.TaskId)
				.ToListAsync();

			var viewModels = history.OrderByDescending(h => h.CreatedAt).Select(h => new TaskHistoryItemViewModel
			{
				CreatedAtString = h.CreatedAt.ToString("f"),
				CreatedByUserFullName = $"{h.CreatedBy.FirstName} {h.CreatedBy.LastName}",
				Id = h.Id,
				Message = h.Message,
				AvatarUrl = null,
				HasAvatar = false,
				CreatedByInitials = $"{(h.CreatedBy.FirstName.IsNull() ? "" : h.CreatedBy.FirstName[0].ToString())}{(h.CreatedBy.LastName.IsNull() ? "" : h.CreatedBy.LastName[0].ToString())}",
				Changes = this._GetPropertyChanges(h)
			}).ToArray();

			return new PageOf<TaskHistoryItemViewModel>
			{
				Items = viewModels,
				TotalNumberOfItems = viewModels.Length
			};
		}

		private List<TaskPropertyChangeViewModel> _GetPropertyChanges(SystemTaskHistory h)
		{
			var changes = new List<TaskPropertyChangeViewModel>();

			foreach(var oldAction in h.OldData.Actions)
			{
				var newAction = h.NewData.Actions.FirstOrDefault(a => a.AssetId == oldAction.AssetId && a.AssetGroupId == oldAction.AssetGroupId && a.ActionName == oldAction.ActionName);

				if(newAction == null)
				{
					// Action was removed 
					changes.Add(new TaskPropertyChangeViewModel
					{
						PropertyName = $"Asset action",
						OldValue = $"{oldAction.ActionName} {oldAction.AssetQuantity}x{oldAction.AssetName}",
						NewValue = null
					});
				}
				else
				{
					// Action was possibly updated
					if(oldAction.AssetQuantity != newAction.AssetQuantity)
					{
						changes.Add(new TaskPropertyChangeViewModel
						{
							PropertyName = "Asset action quantity",
							OldValue = $"{oldAction.ActionName} {oldAction.AssetQuantity}x{oldAction.AssetName}",
							NewValue = $"{newAction.ActionName} {newAction.AssetQuantity}x{newAction.AssetName}",
						});
					}
				}
			}


			foreach (var newAction in h.NewData.Actions)
			{
				var oldAction = h.OldData.Actions.FirstOrDefault(a => a.AssetId == newAction.AssetId && a.AssetGroupId == newAction.AssetGroupId && a.ActionName == newAction.ActionName);
				if (oldAction == null)
				{
					// Action was added
					changes.Add(new TaskPropertyChangeViewModel
					{
						PropertyName = $"Asset action",
						OldValue = null,
						NewValue = $"{newAction.ActionName} {newAction.AssetQuantity}x{newAction.AssetName}",
					});
				}
			}

			//if (h.OldData.ActionName != h.NewData.ActionName)
			//{
			//	changes.Add(new TaskPropertyChangeViewModel
			//	{
			//		PropertyName = "Action",
			//		OldValue = h.OldData.ActionName,
			//		NewValue = h.NewData.ActionName
			//	});
			//}
			//if (h.OldData.AssetId != h.NewData.AssetId)
			//{
			//	changes.Add(new TaskPropertyChangeViewModel
			//	{
			//		PropertyName = "Asset ID",
			//		OldValue = h.OldData.AssetId.ToString(),
			//		NewValue = h.NewData.AssetId.ToString()
			//	});
			//}
			//if (h.OldData.AssetModelId != h.NewData.AssetModelId)
			//{
			//	changes.Add(new TaskPropertyChangeViewModel
			//	{
			//		PropertyName = "Asset model ID",
			//		OldValue = h.OldData.AssetModelId.HasValue ? h.OldData.AssetModelId.Value.ToString() : "N/A",
			//		NewValue = h.NewData.AssetModelId.HasValue ? h.NewData.AssetModelId.Value.ToString() : "N/A"
			//	});

			//}
			//if (h.OldData.AssetName != h.NewData.AssetName)
			//{
			//	changes.Add(new TaskPropertyChangeViewModel
			//	{
			//		PropertyName = "Asset",
			//		OldValue = h.OldData.AssetName,
			//		NewValue = h.NewData.AssetName
			//	});

			//}
			if (h.OldData.EventKey != h.NewData.EventKey)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Event",
					OldValue = h.OldData.EventKey,
					NewValue = h.NewData.EventKey
				});

			}
			if (h.OldData.EventModifierKey != h.NewData.EventModifierKey)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Event modifier",
					OldValue = h.OldData.EventModifierKey,
					NewValue = h.NewData.EventModifierKey
				});

			}
			if (h.OldData.EventTimeKey != h.NewData.EventTimeKey)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Event time",
					OldValue = h.OldData.EventTimeKey,
					NewValue = h.NewData.EventTimeKey
				});

			}
			//if (h.OldData.FloorId != h.NewData.FloorId)
			//{
			//	changes.Add(new TaskPropertyChangeViewModel
			//	{
			//		PropertyName = "Floor ID",
			//		OldValue = h.OldData.FloorId.HasValue ? h.OldData.FloorId.Value.ToString() : "N/A",
			//		NewValue = h.NewData.FloorId.HasValue ? h.NewData.FloorId.Value.ToString() : "N/A"
			//	});

			//}
			//if (h.OldData.HotelId != h.NewData.HotelId)
			//{
			//	changes.Add(new TaskPropertyChangeViewModel
			//	{
			//		PropertyName = "Hotel ID",
			//		OldValue = h.OldData.HotelId,
			//		NewValue = h.NewData.HotelId
			//	});

			//}
			if (h.OldData.IsManuallyModified != h.NewData.IsManuallyModified)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Manually modified",
					OldValue = h.OldData.IsManuallyModified.ToString(),
					NewValue = h.NewData.IsManuallyModified.ToString()
				});

			}
			if (h.OldData.MustBeFinishedByAllWhos != h.NewData.MustBeFinishedByAllWhos)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Must be finished by everyone",
					OldValue = h.OldData.MustBeFinishedByAllWhos.ToString(),
					NewValue = h.NewData.MustBeFinishedByAllWhos.ToString()
				});

			}
			if (h.OldData.RecurringTypeKey != h.NewData.RecurringTypeKey)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Recurring type",
					OldValue = h.OldData.RecurringTypeKey,
					NewValue = h.NewData.RecurringTypeKey
				});

			}
			if (h.OldData.RepeatsForKey != h.NewData.RepeatsForKey)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Repeats for",
					OldValue = h.OldData.RepeatsForKey,
					NewValue = h.NewData.RepeatsForKey
				});

			}
			//if (h.OldData.ReservationId != h.NewData.ReservationId)
			//{
			//	changes.Add(new TaskPropertyChangeViewModel
			//	{
			//		PropertyName = "Reservation ID",
			//		OldValue = h.OldData.ReservationId,
			//		NewValue = h.NewData.ReservationId
			//	});

			//}
			//if (h.OldData.RoomId != h.NewData.RoomId)
			//{
			//	changes.Add(new TaskPropertyChangeViewModel
			//	{
			//		PropertyName = "Room ID",
			//		OldValue = h.OldData.RoomId.HasValue ? h.OldData.RoomId.Value.ToString() : "N/A",
			//		NewValue = h.NewData.RoomId.HasValue ? h.NewData.RoomId.Value.ToString() : "N/A"
			//	});

			//}
			if (h.OldData.StartsAt != h.NewData.StartsAt)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Starts at",
					OldValue = h.OldData.StartsAt.ToString("O"),
					NewValue = h.NewData.StartsAt.ToString("O")
				});

			}
			if (h.OldData.StatusKey != h.NewData.StatusKey)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Status",
					OldValue = h.OldData.StatusKey,
					NewValue = h.NewData.StatusKey
				});

			}
			if (h.OldData.TypeKey != h.NewData.TypeKey)
			{

				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Type",
					OldValue = h.OldData.TypeKey,
					NewValue = h.NewData.TypeKey
				});
			}
			if (h.OldData.UserId != h.NewData.UserId)
			{

				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "User ID",
					OldValue = h.OldData.UserId.ToString(),
					NewValue = h.NewData.UserId.ToString()
				});
			}
			if (h.OldData.WhereTypeKey != h.NewData.WhereTypeKey)
			{

				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Where type",
					OldValue = h.OldData.WhereTypeKey,
					NewValue = h.NewData.WhereTypeKey
				});
			}

			if (h.OldData.Credits != h.NewData.Credits)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Credits",
					OldValue = h.OldData.Credits.ToString(),
					NewValue = h.NewData.Credits.ToString()
				});
			}
			if (h.OldData.Price != h.NewData.Price)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Price",
					OldValue = h.OldData.Price.ToString(),
					NewValue = h.NewData.Price.ToString()
				});
			}
			if (h.OldData.IsBlockingCleaningUntilFinished != h.NewData.IsBlockingCleaningUntilFinished)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Block clean",
					OldValue = h.OldData.IsBlockingCleaningUntilFinished.ToString(),
					NewValue = h.NewData.IsBlockingCleaningUntilFinished.ToString()
				});
			}
			if (h.OldData.IsGuestRequest != h.NewData.IsGuestRequest)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Guest request",
					OldValue = h.OldData.IsGuestRequest.ToString(),
					NewValue = h.NewData.IsGuestRequest.ToString()
				});
			}
			if (h.OldData.IsMajorNotificationRaisedWhenFinished != h.NewData.IsMajorNotificationRaisedWhenFinished)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Raise notification",
					OldValue = h.OldData.IsMajorNotificationRaisedWhenFinished.ToString(),
					NewValue = h.NewData.IsMajorNotificationRaisedWhenFinished.ToString()
				});
			}
			if (h.OldData.IsRescheduledEveryDayUntilFinished != h.NewData.IsRescheduledEveryDayUntilFinished)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Rescheduled until finished",
					OldValue = h.OldData.IsRescheduledEveryDayUntilFinished.ToString(),
					NewValue = h.NewData.IsRescheduledEveryDayUntilFinished.ToString()
				});
			}
			if (h.OldData.IsShownInNewsFeed != h.NewData.IsShownInNewsFeed)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Shown in news feed",
					OldValue = h.OldData.IsShownInNewsFeed.ToString(),
					NewValue = h.NewData.IsShownInNewsFeed.ToString()
				});
			}
			if (h.OldData.PriorityKey != h.NewData.PriorityKey)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "Priority",
					OldValue = h.OldData.PriorityKey,
					NewValue = h.NewData.PriorityKey
				});
			}



			if (h.OldData.FromReservationId != h.NewData.FromReservationId)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "From Reservation ID",
					OldValue = h.OldData.FromReservationId.IsNotNull() ? h.OldData.FromReservationId : "N/A",
					NewValue = h.NewData.FromReservationId.IsNotNull() ? h.NewData.FromReservationId : "N/A",
				});
			}
			if (h.OldData.FromWarehouseId != h.NewData.FromWarehouseId)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "From Warehouse ID",
					OldValue = h.OldData.FromWarehouseId.HasValue ? h.OldData.FromWarehouseId.Value.ToString() : "N/A",
					NewValue = h.NewData.FromWarehouseId.HasValue ? h.NewData.FromWarehouseId.Value.ToString() : "N/A",
				});
			}
			if (h.OldData.FromRoomId != h.NewData.FromRoomId)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "From Room ID",
					OldValue = h.OldData.FromRoomId.HasValue ? h.OldData.FromRoomId.Value.ToString() : "N/A",
					NewValue = h.NewData.FromRoomId.HasValue ? h.NewData.FromRoomId.Value.ToString() : "N/A",
				});
			}
			if (h.OldData.FromHotelName != h.NewData.FromHotelName)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "From Hotel",
					OldValue = h.OldData.FromHotelName.IsNotNull() ? h.OldData.FromHotelName : "N/A",
					NewValue = h.NewData.FromHotelName.IsNotNull() ? h.NewData.FromHotelName : "N/A",
				});
			}
			if (h.OldData.FromHotelId != h.NewData.FromHotelId)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "From Hotel ID",
					OldValue = h.OldData.FromHotelId.IsNotNull() ? h.OldData.FromHotelId : "N/A",
					NewValue = h.NewData.FromHotelId.IsNotNull() ? h.NewData.FromHotelId : "N/A",
				});
			}
			if (h.OldData.FromName != h.NewData.FromName)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "From",
					OldValue = h.OldData.FromName.IsNotNull() ? h.OldData.FromName : "N/A",
					NewValue = h.NewData.FromName.IsNotNull() ? h.NewData.FromName : "N/A",
				});
			}
			if (h.OldData.ToReservationId != h.NewData.ToReservationId)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "To Reservation ID",
					OldValue = h.OldData.ToReservationId.IsNotNull() ? h.OldData.ToReservationId : "N/A",
					NewValue = h.NewData.ToReservationId.IsNotNull() ? h.NewData.ToReservationId : "N/A",
				});
			}
			if (h.OldData.ToWarehouseId != h.NewData.ToWarehouseId)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "To Warehouse ID",
					OldValue = h.OldData.ToWarehouseId.HasValue ? h.OldData.ToWarehouseId.Value.ToString() : "N/A",
					NewValue = h.NewData.ToWarehouseId.HasValue ? h.NewData.ToWarehouseId.Value.ToString() : "N/A",
				});
			}
			if (h.OldData.ToRoomId != h.NewData.ToRoomId)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "To Room ID",
					OldValue = h.OldData.ToRoomId.HasValue ? h.OldData.ToRoomId.Value.ToString() : "N/A",
					NewValue = h.NewData.ToRoomId.HasValue ? h.NewData.ToRoomId.Value.ToString() : "N/A",
				});
			}
			if (h.OldData.ToHotelName != h.NewData.ToHotelName)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "To Hotel",
					OldValue = h.OldData.ToHotelName.IsNotNull() ? h.OldData.ToHotelName : "N/A",
					NewValue = h.NewData.ToHotelName.IsNotNull() ? h.NewData.ToHotelName : "N/A",
				});
			}
			if (h.OldData.ToHotelId != h.NewData.ToHotelId)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "To Hotel ID",
					OldValue = h.OldData.ToHotelId.IsNotNull() ? h.OldData.ToHotelId : "N/A",
					NewValue = h.NewData.ToHotelId.IsNotNull() ? h.NewData.ToHotelId : "N/A",
				});
			}
			if (h.OldData.ToName != h.NewData.ToName)
			{
				changes.Add(new TaskPropertyChangeViewModel
				{
					PropertyName = "To",
					OldValue = h.OldData.ToName.IsNotNull() ? h.OldData.ToName : "N/A",
					NewValue = h.NewData.ToName.IsNotNull() ? h.NewData.ToName : "N/A",
				});
			}









			return changes;
		}
	}
}
