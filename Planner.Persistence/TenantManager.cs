using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Planner.Application.Interfaces;
using Planner.Common.Shared;

namespace Planner.Persistence
{
	public class TenantManager : ITenantManager
	{
		public IDatabaseContext CreateDatabaseContext(string connectionString, IOptions<OperationalStoreOptions> operationalStoreOptions, IHotelGroupTenantProvider hotelGroupTenantProvider = null)
		{
			var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
			optionsBuilder.UseNpgsql(connectionString);

			return new DatabaseContext(optionsBuilder.Options, operationalStoreOptions, hotelGroupTenantProvider);
		}
	}
}
