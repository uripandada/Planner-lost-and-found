using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Rcc.Hotels.Queries.GetListOfHotelGroupHotels
{
	public class RccHotel
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}

	public class GetListOfHotelGroupHotelsQuery: IRequest<IEnumerable<RccHotel>>
	{
		public string HotelGroupKey { get; set; }
	}

	public class GetListOfHotelGroupHotelsQueryHandler : IRequestHandler<GetListOfHotelGroupHotelsQuery, IEnumerable<RccHotel>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetListOfHotelGroupHotelsQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<IEnumerable<RccHotel>> Handle(GetListOfHotelGroupHotelsQuery request, CancellationToken cancellationToken)
		{
			if (!this._databaseContext.DoesHotelGroupExist(request.HotelGroupKey))
			{
				return new RccHotel[0];
			}

			this._databaseContext.SetTenantKey(request.HotelGroupKey);

			return await this._databaseContext.Hotels
				.Select(h => new RccHotel 
				{ 
					Id = h.Id,
					Name = h.Name,
				})
				.ToListAsync();
		}
	}
}
