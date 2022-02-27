using MediatR;
using Planner.Application.Admin;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TimeZones.Queries.GetListOfWindowsTimeZones
{
	public class TimeZoneData
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}

	public class GetListOfWindowsTimeZonesQuery : IRequest<IEnumerable<TimeZoneData>>
	{
	}

	public class GetListOfWindowsTimeZonesQueryHandler : IRequestHandler<GetListOfWindowsTimeZonesQuery, IEnumerable<TimeZoneData>>, IAmAdminApplicationHandler
	{
		public async Task<IEnumerable<TimeZoneData>> Handle(GetListOfWindowsTimeZonesQuery request, CancellationToken cancellationToken)
		{
			return TimeZoneConverter.TZConvert.KnownWindowsTimeZoneIds.Select(tz => new TimeZoneData
			{ 
				Id = tz,
				Name = tz,
			}).ToArray();
		}
	}
}
