using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenIddict.EntityFrameworkCore.Models;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using Planner.Domain.Entities.Master;
using System;
using System.Threading.Tasks;

namespace Planner.Persistence
{
	public class MasterDatabaseContext : IdentityDbContext<MasterUser, Role, Guid>, IMasterDatabaseContext, IPersistedGrantDbContext
	{
		public DbSet<HotelGroupTenant> HotelGroupTenants { get; set; }
		public DbSet<ExternalClientSecretKey> ExternalClientSecretKeys { get; set; }
		public DbSet<PersistedGrant> PersistedGrants { get; set; }
		public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }


		public DbSet<OpenIddictEntityFrameworkCoreAuthorization> Authorization { get; set; }
		public DbSet<OpenIddictEntityFrameworkCoreApplication> Application { get; set; }
		public DbSet<OpenIddictEntityFrameworkCoreToken> Token { get; set; }
		public DbSet<OpenIddictEntityFrameworkCoreScope> Scope { get; set; }


		private readonly IOptions<OperationalStoreOptions> _operationalStoreOptions;
		public MasterDatabaseContext(DbContextOptions<MasterDatabaseContext> options,
			IOptions<OperationalStoreOptions> operationalStoreOptions
			) : base(options)
		{
			this._operationalStoreOptions = operationalStoreOptions;
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.UseOpenIddict();

			modelBuilder.ConfigurePersistedGrantContext(_operationalStoreOptions.Value);

			modelBuilder.UseSerialColumns();

			modelBuilder.ApplyConfigurationsFromAssembly(typeof(MasterDatabaseContext).Assembly, (Type type) =>
			{
				return type.Namespace.Contains("MasterConfigurations");
			});

			foreach (var entity in modelBuilder.Model.GetEntityTypes())
			{
				entity.SetTableName(entity.GetTableName().ToSnakeCase());

				foreach (var property in entity.GetProperties())
				{
					property.SetColumnName(property.GetColumnName().ToSnakeCase());
				}

				foreach (var key in entity.GetKeys())
				{
					key.SetName(key.GetName().ToSnakeCase());
				}

				foreach (var key in entity.GetForeignKeys())
				{
					key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
				}

				foreach (var index in entity.GetIndexes())
				{
					index.SetName(index.GetName().ToSnakeCase());
				}
			}
		}

		Task<int> IPersistedGrantDbContext.SaveChangesAsync() => base.SaveChangesAsync();
	}
}
