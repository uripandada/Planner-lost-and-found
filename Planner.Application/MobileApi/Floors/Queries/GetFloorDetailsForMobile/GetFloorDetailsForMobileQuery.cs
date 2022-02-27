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

namespace Planner.Application.MobileApi.Floors.Queries.GetFloorDetailsForMobile
{
	public class MobileFloorDetails
	{
		public Guid Id { get; set; }
		public string Number { get; set; }
		public string Description { get; set; }
		public string HotelId { get; set; } = null;
		public string LastAction { get; set; } = null;
		public DateTime LastDate { get; set; } = DateTime.UtcNow;
	}

	public class GetFloorDetailsForMobileQuery : IRequest<MobileFloorDetails>
	{
		public Guid Id { get; set; }
	}

	public class GetFloorDetailsForMobileQueryHandler : IRequestHandler<GetFloorDetailsForMobileQuery, MobileFloorDetails>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetFloorDetailsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<MobileFloorDetails> Handle(GetFloorDetailsForMobileQuery request, CancellationToken cancellationToken)
		{
			var f = await this._databaseContext.Floors
				.Include(f => f.Building)
				.Where(f => f.Id == request.Id)
				.FirstOrDefaultAsync();

			if (f == null) throw new Exception("Unable to find floor details.");

			var d = new Application.MobileApi.Shared.Models.FloorForMobile();

			return new MobileFloorDetails
			{
				Id = f.Id,
				Number = f.Number.ToString(),
				Description = $"{f.Building?.Name} - {f.Name} {f.Number}",
				HotelId = f.HotelId,
				LastAction = d.LastAction,
				LastDate = d.LastDate,
			};
		}
	}
}
