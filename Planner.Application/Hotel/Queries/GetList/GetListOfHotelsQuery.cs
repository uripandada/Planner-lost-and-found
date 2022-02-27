using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Hotel.Queries.GetList
{
	public class HotelItemData
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}

	public class GetListOfHotelsQuery : IRequest<HotelItemData[]>
	{
	}

	public class GetListOfHotelsQueryHandler : IRequestHandler<GetListOfHotelsQuery, HotelItemData[]>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public GetListOfHotelsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<HotelItemData[]> Handle(GetListOfHotelsQuery request, CancellationToken cancellationToken)
		{
			var user = this._httpContextAccessor.HttpContext.User;
			var query = this._databaseContext.Hotels.AsQueryable();

			if (!user.HasClaim(System.Security.Claims.ClaimTypes.Role, SystemDefaults.Roles.Administrator.Name))
			{
				var hotelIds = user.Claims.Where(c => c.Type == "hotel_id").Select(c => c.Value).ToArray();
				if (hotelIds.Any())
				{
					if (!hotelIds.Contains("ALL"))
					{
						query = query.Where(hotel => hotelIds.Contains(hotel.Id));
					}
				}
				else
				{
					return new HotelItemData[0];
				}
			}

			return await query.Select(h => new HotelItemData { Id = h.Id, Name = h.Name }).ToArrayAsync();
		}
	}
}
