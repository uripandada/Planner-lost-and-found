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

namespace Planner.Application.MobileApi.Hotels.Queries.GetListOfHotelsForMobile
{
	public class MobileHotel
	{
		public string Id { get; set; } = Guid.Empty.ToString();   // "_id": "55d06baa99295b3a52000000",
		public string Name { get; set; } = "NULL hotel"; // "name": "Hotel Royal",
	}

	public class GetListOfHotelsForMobileQuery: IRequest<IEnumerable<MobileHotel>>
	{

	}

	public class GetListOfHotelsForMobileQueryHandler : IRequestHandler<GetListOfHotelsForMobileQuery, IEnumerable<MobileHotel>>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetListOfHotelsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<IEnumerable<MobileHotel>> Handle(GetListOfHotelsForMobileQuery request, CancellationToken cancellationToken)
		{
			var hotels = await this._databaseContext.Hotels.ToArrayAsync();

			return hotels.Select(h => new MobileHotel 
			{ 
				Id = h.Id,
				Name = h.Name,
			}).ToArray();
		}
	}
}
