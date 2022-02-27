using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.HotelGroup.Queries.GetHotelGroups
{
	public class HotelGroupGridData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public bool IsActive { get; set; }
	}

	public class GetHotelGroupsQuery : IRequest<HotelGroupGridData[]>
	{
	}

	public class GetHotelGroupsQueryHandler : IRequestHandler<GetHotelGroupsQuery, HotelGroupGridData[]>, IAmAdminApplicationHandler
	{
		private readonly IMasterDatabaseContext databaseContext;

		public GetHotelGroupsQueryHandler(IMasterDatabaseContext databaseContext)
		{
			this.databaseContext = databaseContext;
		}

		public async Task<HotelGroupGridData[]> Handle(GetHotelGroupsQuery request, CancellationToken cancellationToken)
		{
			return await this.databaseContext
				.HotelGroupTenants
				.Select(t => new HotelGroupGridData 
				{ 
					Id = t.Id,
					Name = t.Name,
					IsActive = t.IsActive
				})
				.OrderBy(t => t.Name).ToArrayAsync();
		}
	}
}
