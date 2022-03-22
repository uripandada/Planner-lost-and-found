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

namespace Planner.Application.ReservationManagement.Queries.GetList
{
	public class ReservationViewModel
	{
		
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

			var reservations = await query.ToArrayAsync();

			var response = new PageOf<ReservationViewModel>
			{
				TotalNumberOfItems = reservations.Length,
				Items = reservations.Select(d => new ReservationViewModel
				{
					
				}).ToArray()
			};

			return response;
		}
	}
}
