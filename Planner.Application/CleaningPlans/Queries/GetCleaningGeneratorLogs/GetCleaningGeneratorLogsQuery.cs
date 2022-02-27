using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.CleaningPlans.Queries.GetCleaningGeneratorLogs
{
	public class CleaningGeneratorLogItem
	{
		public Guid GenerationId { get; set; }
		public DateTime At { get; set; }
		public DateTime CleaningPlanDate { get; set; }
		public string Message { get; set; }
		public string RoomDescription { get; set; }
		public string ReservationsDescription { get; set; }
		public string ReservationsEventsDescription { get; set; }
		public string PluginEventsDescription { get; set; }
		public string OrderedPluginsDescription { get; set; }
		public string CleaningEventsDescription { get; set; }
		public string CleaningsDescription { get; set; }
		public string HotelId { get; set; }
		public string HotelName { get; set; }
	}

	public class GetCleaningGeneratorLogsQuery: IRequest<IEnumerable<CleaningGeneratorLogItem>>
	{
		public DateTime CleaningDate { get; set; }
		public Guid? GenerationId { get; set; }
		public string HotelId { get; set; }
	}

	public class GetCleaningGeneratorLogsQueryHandler : IRequestHandler<GetCleaningGeneratorLogsQuery, IEnumerable<CleaningGeneratorLogItem>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetCleaningGeneratorLogsQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<IEnumerable<CleaningGeneratorLogItem>> Handle(GetCleaningGeneratorLogsQuery request, CancellationToken cancellationToken)
		{
			var cleaningDate = request.CleaningDate.Date;
			var query = this._databaseContext.CleaningGeneratorLogs.Where(l => l.HotelId == request.HotelId && l.CleaningPlanDate == cleaningDate).AsQueryable();

			if (request.GenerationId.HasValue)
			{
				query = query.Where(q => q.GenerationId == request.GenerationId.Value);
			}

			var hotels = await this._databaseContext.Hotels.Select(h => new { h.Id, h.Name }).ToDictionaryAsync(h => h.Id);
			var results = await query.OrderBy(q => q.At).Select(q => new CleaningGeneratorLogItem
			{
				At = q.At,
				CleaningEventsDescription = q.CleaningEventsDescription,
				CleaningPlanDate = q.CleaningPlanDate,
				CleaningsDescription = q.CleaningsDescription,
				GenerationId = q.GenerationId,
				Message = q.Message,
				OrderedPluginsDescription = q.OrderedPluginsDescription,
				PluginEventsDescription = q.PluginEventsDescription,
				ReservationsDescription = q.ReservationsDescription,
				ReservationsEventsDescription = q.ReservationsEventsDescription,
				RoomDescription = q.RoomDescription,
				HotelId = q.HotelId,
			}).ToArrayAsync();

			foreach(var result in results)
			{
				if (hotels.ContainsKey(result.HotelId))
				{
					result.HotelName = hotels[result.HotelId].Name;
				}
			}

			return results;
		}
	}
}
