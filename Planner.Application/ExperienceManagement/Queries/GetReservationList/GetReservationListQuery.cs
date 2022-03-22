using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ReservationManagement.Queries.GetReservationList
{
	public class ReservationViewModel
	{
		public string Id { get; set; }
		public string RoomName { get; set; }
		public string GuestName { get; set; }
		public DateTime? CheckIn { get; set; }
		public DateTime? CheckOut { get; set; }
		public string Vip { get; set; }
		public string Group { get; set; }
	}

	public class GetReservationListQuery : GetPageRequest, IRequest<PageOf<ReservationViewModel>>
	{
	}

	public class GetReservationListQueryHandler : GetPageRequest, IRequestHandler<GetReservationListQuery, PageOf<ReservationViewModel>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetReservationListQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<PageOf<ReservationViewModel>> Handle(GetReservationListQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.Reservations.AsQueryable();

			var reservationStautuses = new List<string>();
			reservationStautuses.Add("STAY");
			reservationStautuses.Add("DEP");

			query = query.Where(r => reservationStautuses.Contains(r.ReservationStatusKey));
			query = query.Where(r => r.CheckIn < DateTime.Today && r.CheckOut >= DateTime.Today).Where(r => r.Room.Category != null && !r.Room.Category.IsPrivate && r.Room.BuildingId != null && r.Room.FloorId != null).OrderBy(r=> r.RoomName);
			
			var reservations = await query.ToArrayAsync();

			var response = new PageOf<ReservationViewModel>
			{
				TotalNumberOfItems = reservations.Length,
				Items = reservations.Select(d => new ReservationViewModel
				{
					Id = d.Id,
					RoomName = d.RoomName,
					GuestName = d.GuestName,
					CheckIn = d.CheckIn,
					CheckOut = d.CheckOut,
					Vip = d.Vip,
					Group = d.Group
				}).ToArray()
			};

			return response;
		}
	}
}
