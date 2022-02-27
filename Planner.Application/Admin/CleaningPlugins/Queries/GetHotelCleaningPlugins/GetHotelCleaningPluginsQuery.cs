using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.CleaningPlugins.Queries.GetHotelCleaningPlugins
{
	public class CleaningPluginGridData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string TypeKey { get; set; }
		public string TypeDescription { get; set; }
		public bool IsActive { get; set; }
		public bool IsTopRule { get; set; }
		public int OrdinalNumber { get; set; }

	}

	public class GetHotelCleaningPluginsQuery : IRequest<IEnumerable<CleaningPluginGridData>>
	{
		public string HotelId { get; set; }
	}

	public class GetHotelCleaningPluginsQueryHandler : IRequestHandler<GetHotelCleaningPluginsQuery, IEnumerable<CleaningPluginGridData>>, IAmAdminApplicationHandler
	{
		private readonly IDatabaseContext databaseContext;

		public GetHotelCleaningPluginsQueryHandler(IDatabaseContext databaseContext)
		{
			this.databaseContext = databaseContext;
		}

		public async Task<IEnumerable<CleaningPluginGridData>> Handle(GetHotelCleaningPluginsQuery request, CancellationToken cancellationToken)
		{
			return await this.databaseContext
				.CleaningPlugins
				.Where(cp => cp.HotelId == request.HotelId)
				.Select(cp => new CleaningPluginGridData
				{
					Id = cp.Id,
					Name = cp.Name,
					TypeDescription = null,
					TypeKey = cp.Data.TypeKey,
					IsActive = cp.IsActive,
					IsTopRule = cp.IsTopRule,
					OrdinalNumber = cp.OrdinalNumber
				})
				.OrderBy(cp => cp.OrdinalNumber)
				.ToArrayAsync();
		}
	}
}
