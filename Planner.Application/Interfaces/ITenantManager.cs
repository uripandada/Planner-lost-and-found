using IdentityServer4.EntityFramework.Options;
using Microsoft.Extensions.Options;
using Planner.Common.Shared;
using System.Linq;
using System.Text;

namespace Planner.Application.Interfaces
{
	public interface ITenantManager
	{
		IDatabaseContext CreateDatabaseContext(string connectionString, IOptions<OperationalStoreOptions> operationalStoreOptions, IHotelGroupTenantProvider hotelGroupTenantProvider = null);
	}

}
