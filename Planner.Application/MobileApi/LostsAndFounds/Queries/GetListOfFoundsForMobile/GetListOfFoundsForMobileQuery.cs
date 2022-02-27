using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Common;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.LostsAndFounds.Queries.GetListOfFoundsForMobile
{
	public class MobileFoundItem
	{
		public Guid Id { get; set; }
		public string Hotel_id { get; set; }
		public Guid? Room_id { get; set; }
		public string Room_name { get; set; }
		public string Location { get; set; }
		public string Guest_name { get; set; }
		public long Date_ts { get; set; }
		public string Name_or_description { get; set; }
		public string Category { get; set; }
		public string Image { get; set; } // 'https://www.filepicker.io/api/file/0IhRvfa5R8C9OWApWF62'
		public string Log_date { get; set; } // '2015-10-26'
		public string Pending_message { get; set; }
		public Guid User_id { get; set; }
		public string User_username { get; set; }
		public string User_email { get; set; }
		public string User_first_name { get; set; }
		public string User_last_name { get; set; }
		public Guid Last_user_id { get; set; }
		public string Held { get; set; }
		public string Status { get; set; } = "waiting";
		public int Is_closed { get; set; } = 0;
		public string Reference { get; set; }
		public string Notes { get; set; }
		public string Signature { get; set; }
		public string Added_image_one { get; set; }
		public string Added_image_two { get; set; }
	}

	public class GetListOfFoundsForMobileQuery : IRequest<IEnumerable<MobileFoundItem>>
	{
		public string HotelId { get; set; }
	}

	public class GetListOfFoundsForMobileQueryHandler : IRequestHandler<GetListOfFoundsForMobileQuery, IEnumerable<MobileFoundItem>>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetListOfFoundsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<IEnumerable<MobileFoundItem>> Handle(GetListOfFoundsForMobileQuery request, CancellationToken cancellationToken)
		{
			var foundItems = await this._databaseContext.LostAndFounds
				.Where(lf => !lf.IsDeleted && lf.RoomId.HasValue && lf.Room.HotelId == request.HotelId && lf.Type == Domain.Values.LostAndFoundRecordType.Found)
				.Select(lf => new MobileFoundItem 
				{
					Category = "",
					Date_ts = lf.CreatedAt.ToUnixTimeStamp(),
					Guest_name = lf.ReservationId == null ? "" : lf.Reservation.GuestName,
					Held = "",
					Hotel_id = request.HotelId,
					Id = lf.Id,
					Is_closed = 0,
					User_id = lf.CreatedBy.Id,
					User_last_name = lf.CreatedBy.LastName,
					User_first_name = lf.CreatedBy.FirstName,
					User_email = lf.CreatedBy.Email,
					User_username = lf.CreatedBy.UserName,
					Last_user_id = lf.ModifiedBy.Id,
					Location = "",
					Added_image_one = null,
					Added_image_two = null,
					Image = lf.ImageUrl,
					Name_or_description = lf.Description,
					Room_id = lf.RoomId.Value,
					Room_name = lf.Room.Name,
					Notes = "",
					Pending_message = "",
					Reference = "",
					Signature = "",
					Status = lf.Status.ToString(),
					Log_date = lf.CreatedAt.ToString("yyyy-MM-dd"),
				})
				.ToArrayAsync();

			return foundItems;
		}
	}
}
