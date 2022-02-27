using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Planner.Application.RoomMessages.Queries.GetComplexRoomMessageDetails
{
	public class GetComplexRoomMessageDetailsQuery: IRequest<RoomMessageDetails>
	{
		public string HotelId { get; set; }
		public Guid Id { get; set; }
	}
	public class GetComplexRoomMessageDetailsQueryHandler : IRequestHandler<GetComplexRoomMessageDetailsQuery, RoomMessageDetails>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetComplexRoomMessageDetailsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<RoomMessageDetails> Handle(GetComplexRoomMessageDetailsQuery request, CancellationToken cancellationToken)
		{
			var message = await this._databaseContext.RoomMessages.FindAsync(request.Id);
			var messageDates = await this._databaseContext.RoomMessageDates.Where(d => d.RoomMessageId == message.Id).ToListAsync();
			var messageFilters = await this._databaseContext.RoomMessageFilters.Where(d => d.RoomMessageId == message.Id).ToListAsync();

			return new RoomMessageDetails
			{
				Id = message.Id,
				Dates = messageDates.Select(md => md.Date).ToArray(),
				DateType = message.DateType,
				ForType = message.ForType,
				IntervalEndDate = message.IntervalEndDate,
				IntervalNumberOfDays = message.IntervalNumberOfDays,
				IntervalStartDate = message.IntervalStartDate,
				IsDeleted = message.IsDeleted,
				Message = message.Message,
				ReservationOnArrivalDate = message.ReservationOnArrivalDate,
				ReservationOnDepartureDate = message.ReservationOnDepartureDate,
				ReservationOnStayDates = message.ReservationOnStayDates,
				RoomMessageFilters = messageFilters.Select(mf => new RoomMessageDetailsFilter 
				{
					Id = mf.Id,
					ReferenceDescription = mf.ReferenceDescription,
					ReferenceId = mf.ReferenceId,
					ReferenceName = mf.ReferenceName,
					ReferenceType = mf.ReferenceType,
				}).ToArray(),
				Type = message.Type,
			};
		}
	}
}
