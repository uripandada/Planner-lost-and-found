using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenIddict.EntityFrameworkCore.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.Interfaces
{
	public interface IMasterDatabaseContext
	{
		DbSet<OpenIddictEntityFrameworkCoreAuthorization> Authorization { get; set; }
		DbSet<OpenIddictEntityFrameworkCoreApplication> Application { get; set; }
		DbSet<OpenIddictEntityFrameworkCoreToken> Token { get; set; }
		DbSet<OpenIddictEntityFrameworkCoreScope> Scope { get; set; }

		DbSet<Domain.Entities.Master.HotelGroupTenant> HotelGroupTenants { get; set; }
		DbSet<Domain.Entities.Master.ExternalClientSecretKey> ExternalClientSecretKeys { get; set; }
		DbSet<Domain.Entities.MasterUser> Users { get; set; }
		Task<int> SaveChangesAsync(CancellationToken cancellationToken);

		DatabaseFacade Database { get; }
	}
}
