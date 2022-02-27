using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.HotelGroup.Queries.GetHotels
{
	public class HotelGridData
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}

	public class GetHotelsQuery : IRequest<HotelGridData[]>
	{
	}

	public class GetHotelQueryHandler : IRequestHandler<GetHotelsQuery, HotelGridData[]>, IAmAdminApplicationHandler
	{
		private readonly IDatabaseContext databaseContext;

		public GetHotelQueryHandler(IDatabaseContext databaseContext)
		{
			this.databaseContext = databaseContext;
		}

		public async Task<HotelGridData[]> Handle(GetHotelsQuery request, CancellationToken cancellationToken)
		{
			return await this.databaseContext
				.Hotels
				.Select(t => new HotelGridData
				{
					Id = t.Id,
					Name = t.Name,
				})
				.OrderBy(t => t.Name)
				.ToArrayAsync();
		}
	}
}
