using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.MobileApi.LostsAndFounds.Commands.InsertFoundForMobile;
using Planner.Application.MobileApi.LostsAndFounds.Queries.GetListOfFoundsForMobile;
using Planner.Common.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.LostsAndFounds.Commands.UpdateFoundForMobile
{
	public class UpdateFoundForMobileCommand: SaveFoundForMobileCommand, IRequest<MobileFoundItem>
	{
		public Guid Id { get; set; }
		public bool Is_closed { get; set; }
	}

	public class UpdateFoundForMobileCommandHandler : IRequestHandler<UpdateFoundForMobileCommand, MobileFoundItem>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public UpdateFoundForMobileCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<MobileFoundItem> Handle(UpdateFoundForMobileCommand request, CancellationToken cancellationToken)
		{
			var dateProvider = new HotelLocalDateProvider();
			var dateTime = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, request.HotelId, true);

			var foundItem = await this._databaseContext.LostAndFounds
				.Include(lf => lf.Room)
				.Where(lf => lf.Id == request.Id).FirstOrDefaultAsync();

			if (foundItem == null)
			{
				return null;
			}
			foundItem.RccStatus = request.Status.IsNotNull() && Domain.Values.RccLostAndFoundStatusKeys.Statuses.ContainsKey(request.Status) ? Domain.Values.RccLostAndFoundStatusKeys.Statuses[request.Status] : Domain.Values.RccLostAndFoundStatus.UNKNOWN;
			foundItem.ImageUrl = request.Image;
			foundItem.IsClosed = request.Is_closed;
			foundItem.Description = request.Name_or_description;
			foundItem.RoomId = request.Room_id;
			foundItem.Notes = request.Notes;
			foundItem.ReferenceNumber = request.Reference;
			if (foundItem.HotelId.IsNull())
			{
				foundItem.HotelId = request.HotelId;
			}

			foundItem.ModifiedById = this._userId;
			foundItem.ModifiedAt = dateTime;

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
				Id = request.Id,
				Image = request.Image,
				Is_closed = request.Is_closed ? 1 : 0,
				Last_user_id = user.Id,
				Reference = request.Reference,
				Room_id = request.Room_id,
				Location = request.Location,
				Log_date = dateTime.ToString("yyyy-MM-dd"),
				Name_or_description = request.Name_or_description,
				Notes = request.Notes,
				Pending_message = request.Pending_message,
				Room_name = foundItem.Room == null ? "" : foundItem.Room.Name,
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
