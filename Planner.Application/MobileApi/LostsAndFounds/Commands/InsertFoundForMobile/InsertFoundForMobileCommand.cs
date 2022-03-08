using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.MobileApi.LostsAndFounds.Queries.GetListOfFoundsForMobile;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.LostsAndFounds.Commands.InsertFoundForMobile
{
	public class SaveFoundForMobileCommand
	{
		public string HotelId { get; set; }
		public Guid Room_id { get; set; }
		public string Guest_name { get; set; }
		public string Location { get; set; }
		public string Category { get; set; }
		public string Name_or_description { get; set; }
		public string Image { get; set; }
		//public Guid? User_id { get; set; }
		public string Status { get; set; }
		public string Pending_message { get; set; }
		public string Notes { get; set; }
		public string Reference { get; set; }

	}

	public class InsertFoundForMobileCommand: SaveFoundForMobileCommand, IRequest<MobileFoundItem>
	{
	}

	public class InsertFoundForMobileCommandHandler : IRequestHandler<InsertFoundForMobileCommand, MobileFoundItem>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public InsertFoundForMobileCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<MobileFoundItem> Handle(InsertFoundForMobileCommand request, CancellationToken cancellationToken)
		{
			var dateProvider = new HotelLocalDateProvider();
			var dateTime = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, request.HotelId, true);

			var foundItem = new LostAndFound
			{
				Id = Guid.NewGuid(),
				IsClosed = false,
				IsDeleted = false,
				CreatedAt = dateTime,
				CreatedById = this._userId,
				ModifiedAt = dateTime,
				ModifiedById = this._userId,
				RccStatus = request.Status.IsNotNull() && Domain.Values.RccLostAndFoundStatusKeys.Statuses.ContainsKey(request.Status) ? Domain.Values.RccLostAndFoundStatusKeys.Statuses[request.Status] : Domain.Values.RccLostAndFoundStatus.UNKNOWN,
				FoundStatus = Domain.Values.FoundStatus.WaitingRoomMaid,
				GuestStatus = Domain.Values.GuestStatus.Unclaimed,
				DeliveryStatus = 0,
				OtherStatus = 0,
				Type = Domain.Values.LostAndFoundRecordType.Found,
				TypeOfLoss = Domain.Values.TypeOfLoss.Employee,
				
				Description = request.Name_or_description,
				ImageUrl = request.Image,

				Address = null,
				City = null,
				Country = null,
				Email = null,
				Files = null,
				FirstName = null,
				HotelId = request.HotelId,
				Hotel = null,
				LastName = null,
				LostOn = dateTime,
				Notes = request.Notes,
				PhoneNumber = null,
				PostalCode = null,
				ReferenceNumber = request.Reference,
				ReservationId = null,
				Reservation = null,
				Room = null,
				RoomId = request.Room_id,


			};

			var room = await this._databaseContext.Rooms.FindAsync(request.Room_id);

			await this._databaseContext.LostAndFounds.AddAsync(foundItem);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			var user = await this._databaseContext.Users.FindAsync(this._userId);

			return new MobileFoundItem
			{
				Added_image_one = null,
				Added_image_two = null,
				Category = request.Category,
				Date_ts = dateTime.ToUnixTimeStamp(),
				Guest_name = request.Guest_name,
				Held = "",
				Hotel_id = request.HotelId,
				Id = foundItem.Id,
				Image = request.Image,
				Is_closed = foundItem.IsClosed ? 1 : 0,
				Last_user_id = user.Id,
				Reference = request.Reference,
				Room_id = request.Room_id,
				Location = request.Location,
				Log_date = dateTime.ToString("yyyy-MM-dd"),
				Name_or_description = request.Name_or_description,
				Notes = request.Notes,
				Pending_message = request.Pending_message,
				Room_name = room == null ? "" : room.Name,
				Signature = "",
				Status = Domain.Values.RccLostAndFoundStatusKeys.RccStatuses[foundItem.RccStatus],
				User_email = user.Email,
				User_first_name = user.FirstName,
				User_id = user.Id,
				User_last_name = user.LastName,
				User_username = user.UserName
			};
		}
	}
}
