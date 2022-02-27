using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Common;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Floors.Queries.GetListOfFloorsForMobile
{
	public class MobileFloor
	{
		public Guid Id { get; set; }
		public string Number { get; set; }
		public string Description { get; set; }
		public string HotelId { get; set; } = null;
		public string LastAction { get; set; } = null;
		public DateTime LastDate { get; set; } = DateTime.UtcNow;
	}

	public class GetListOfFloorsForMobileQuery: IRequest<IEnumerable<MobileFloor>>
	{
		public string HotelId { get; set; }
	}

	public class GetListOfFloorsForMobileQueryHandler : IRequestHandler<GetListOfFloorsForMobileQuery, IEnumerable<MobileFloor>>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetListOfFloorsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<IEnumerable<MobileFloor>> Handle(GetListOfFloorsForMobileQuery request, CancellationToken cancellationToken)
		{
			var buildings = await this._databaseContext.Buildings
				.Include(b => b.Floors)
				.Where(b => b.HotelId == request.HotelId)
				.OrderBy(b => b.Name)
				.ToArrayAsync();

			var d = new Application.MobileApi.Shared.Models.FloorForMobile();

			var floors = new List<MobileFloor>();
			foreach(var b in buildings)
			{
				foreach(var f in b.Floors.OrderBy(f => f.OrdinalNumber).ToArray())
				{
					floors.Add(new MobileFloor 
					{ 
						Id = f.Id,
						Number = f.Number.ToString(),
						Description = $"{b.Name} - {f.Name} {f.Number}",
						HotelId = f.HotelId,
						LastAction = d.LastAction,
						LastDate = d.LastDate,
					});
				}
			}

			return floors;
		}
	}
}
