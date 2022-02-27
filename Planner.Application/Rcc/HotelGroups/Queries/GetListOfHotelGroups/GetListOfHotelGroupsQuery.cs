using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Rcc.HotelGroups.Queries.GetListOfHotelGroups
{
	public class RccHotelGroup
	{
		public Guid Id { get; set; }
		public string Key { get; set; }
	}

	public class GetListOfHotelGroupsQuery : IRequest<IEnumerable<RccHotelGroup>>
	{

	}

	public class GetListOfHotelGroupsQueryHandler : IRequestHandler<GetListOfHotelGroupsQuery, IEnumerable<RccHotelGroup>>, IAmWebApplicationHandler
	{
		private readonly IMasterDatabaseContext _masterDatabaseContext;

		public GetListOfHotelGroupsQueryHandler(IMasterDatabaseContext masterDatabaseContext)
		{
			this._masterDatabaseContext = masterDatabaseContext;
		}

		public async Task<IEnumerable<RccHotelGroup>> Handle(GetListOfHotelGroupsQuery request, CancellationToken cancellationToken)
		{
			return await this._masterDatabaseContext.HotelGroupTenants
				.Select(t => new RccHotelGroup
				{
					Id = t.Id,
					Key = t.Key,
				})
				.ToListAsync();
		}
	}
}
