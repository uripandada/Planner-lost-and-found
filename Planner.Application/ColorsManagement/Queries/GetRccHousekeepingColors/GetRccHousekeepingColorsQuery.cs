using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common;
using Planner.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ColorsManagement.Queries.GetRccHousekeepingColors
{
	public class RccHousekeepingStatusColorDetails
	{
		public RccHousekeepingStatusCode RccCode { get; set; }
		public string ColorHex { get; set; }
		public string Description { get; set; }
	}

	public class GetRccHousekeepingColorsQuery: IRequest<IEnumerable<RccHousekeepingStatusColorDetails>>
	{
	}

	public class GetRccHousekeepingColorsQueryHandler : IRequestHandler<GetRccHousekeepingColorsQuery, IEnumerable<RccHousekeepingStatusColorDetails>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public GetRccHousekeepingColorsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<IEnumerable<RccHousekeepingStatusColorDetails>> Handle(GetRccHousekeepingColorsQuery request, CancellationToken cancellationToken)
		{
			var colors = await this._databaseContext.RccHousekeepingStatusColors.ToDictionaryAsync(c => c.RccCode);
			var rccHousekeepingStatuses = Enum.GetValues(typeof(RccHousekeepingStatusCode)).Cast<RccHousekeepingStatusCode>();
			var statusColors = new List<RccHousekeepingStatusColorDetails>();

			foreach(var status in rccHousekeepingStatuses)
			{
				var color = "454545";
				if (colors.ContainsKey(status))
				{
					color = colors[status].ColorHex;
				}
				else
				{
					if (COLORS.ROOM_HOUSEKEEPING_STATUSES.ContainsKey(status))
					{
						color = COLORS.ROOM_HOUSEKEEPING_STATUSES[status];
					}
				}

				var description = "Unknown";
				if (COLORS.ROOM_HOUSEKEEPING_STATUS_DESCRIPTIONS.ContainsKey(status))
				{
					description = COLORS.ROOM_HOUSEKEEPING_STATUS_DESCRIPTIONS[status];
				}

				statusColors.Add(new RccHousekeepingStatusColorDetails 
				{ 
					ColorHex = color,
					RccCode = status,
					Description = description,
				});
			}

			return statusColors;

		}
	}
}
