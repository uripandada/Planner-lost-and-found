using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.HotelGroup.Queries.GetHotelGroupDetails
{
	public class HotelGroupDetailsData
	{
		public Guid Id { get; set; } // 32 bit guid
		public string Key { get; set; } // "hotel-royal"
		public string Name { get; set; } // "Hotel Royal"
		public bool IsActive { get; set; }

		public string ConnectionStringUserId { get; set; }
		public string ConnectionStringPassword { get; set; }
		public string ConnectionStringHost { get; set; }
		public string ConnectionStringPort { get; set; }
		public string ConnectionStringDatabase { get; set; }
		public bool ConnectionStringPooling { get; set; }
	}

	public class GetHotelGroupDetailsQuery : IRequest<HotelGroupDetailsData>
	{
		public Guid Id { get; set; }
	}

	public class GetHotelGroupDetailsQueryHandler : IRequestHandler<GetHotelGroupDetailsQuery, HotelGroupDetailsData>, IAmAdminApplicationHandler
	{
		private IMasterDatabaseContext _databaseContext;

		public GetHotelGroupDetailsQueryHandler(IMasterDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<HotelGroupDetailsData> Handle(GetHotelGroupDetailsQuery request, CancellationToken cancellationToken)
		{
			var hotelGroup = await this._databaseContext
				.HotelGroupTenants
				.Where(g => g.Id == request.Id)
				.FirstOrDefaultAsync();

			if (hotelGroup == null)
				return null;

			var connectionStringData = ConnectionStringHelper.ParsePostgreSqlConnectionString(hotelGroup.ConnectionString);

			return new HotelGroupDetailsData
			{
				Id = hotelGroup.Id,
				IsActive = hotelGroup.IsActive,
				Key = hotelGroup.Key,
				Name = hotelGroup.Name,
				ConnectionStringDatabase = connectionStringData.Database,
				ConnectionStringHost = connectionStringData.Host,
				ConnectionStringPassword = "",
				ConnectionStringPooling = connectionStringData.Pooling,
				ConnectionStringPort = connectionStringData.Port,
				ConnectionStringUserId = connectionStringData.UserId,
			};
		}
	}
}
