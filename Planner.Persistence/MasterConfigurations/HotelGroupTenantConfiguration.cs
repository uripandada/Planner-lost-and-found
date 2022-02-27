using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities.Master;
using System;

namespace Planner.Persistence.MasterConfigurations
{
	public class HotelGroupTenantConfiguration : IEntityTypeConfiguration<HotelGroupTenant>
	{
		public void Configure(EntityTypeBuilder<HotelGroupTenant> builder)
		{
			builder.HasKey(tenant => tenant.Id);

			builder.Property(tenant => tenant.Id)
				.HasColumnName(nameof(HotelGroupTenant.Id))
				.IsRequired();

			builder.Property(tenant => tenant.Name)
				.HasColumnName(nameof(HotelGroupTenant.Name))
				.IsRequired();

			builder.Property(tenant => tenant.Key)
				.HasColumnName(nameof(HotelGroupTenant.Key))
				.IsRequired();

			builder.Property(tenant => tenant.ConnectionString)
				.HasColumnName(nameof(HotelGroupTenant.ConnectionString))
				.IsRequired();

			builder.Property(tenant => tenant.IsActive)
				.HasColumnName(nameof(HotelGroupTenant.IsActive))
				.IsRequired();

			builder.Property(tenant => tenant.DatabaseName)
				.HasColumnName(nameof(HotelGroupTenant.DatabaseName))
				.IsRequired();

			builder.HasIndex(tenant => tenant.Key)
				.IsUnique();

			#region DataSeed
			var roomchecking = new HotelGroupTenant
			{
				Id = new Guid("00000000-0000-0000-0000-000000000001"),
				IsActive = true,
				Name = "Roomchecking",
				Key = "Roomchecking",
				DatabaseName = "hgtest_Roomchecking",
			};
			var group1 = new HotelGroupTenant
			{
				Id = new Guid("00000000-0000-0000-0000-000000000002"),
				IsActive = true,
				Name = "Test hotel group 1",
				Key = "test_group_1",
				DatabaseName = "hgtest_test_group_1",
			};
			var group2 = new HotelGroupTenant
			{
				Id = new Guid("00000000-0000-0000-0000-000000000003"),
				IsActive = true,
				Name = "Test hotel group 2",
				Key = "test_group_2",
				DatabaseName = "hgtest_test_group_2",
			};
			var group3 = new HotelGroupTenant
			{
				Id = new Guid("00000000-0000-0000-0000-000000000004"),
				IsActive = true,
				Name = "Test hotel group 3",
				Key = "test_group_3",
				DatabaseName = "hgtest_test_group_3",
			};

			roomchecking.ConnectionString = $"User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_{roomchecking.Key};Pooling=true;";
			group1.ConnectionString = $"User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_{group1.Key};Pooling=true;";
			group2.ConnectionString = $"User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_{group2.Key};Pooling=true;";
			group3.ConnectionString = $"User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_{group3.Key};Pooling=true;";
			#endregion

			builder.HasData(new HotelGroupTenant[] {
				roomchecking,
				group1,
				group2,
				group3,
			});
		}
	}
	
	public class ExternalClientSecretKeyConfiguration : IEntityTypeConfiguration<ExternalClientSecretKey>
	{
		public void Configure(EntityTypeBuilder<ExternalClientSecretKey> builder)
		{
			builder.HasKey(c => new { ClientId = c.ClientId, Key = c.Key });

			builder.Property(c => c.ClientId)
				.HasColumnName(nameof(ExternalClientSecretKey.ClientId))
				.IsRequired();

			builder.Property(c => c.Key)
				.HasColumnName(nameof(ExternalClientSecretKey.Key))
				.IsRequired();

			builder.Property(c => c.IsActive)
				.HasColumnName(nameof(ExternalClientSecretKey.IsActive))
				.IsRequired();

			builder.HasData(new ExternalClientSecretKey[] 
			{
				new ExternalClientSecretKey { Key = "testing-rcc-secret-key", ClientId = "RCC", IsActive = true },	
			});
		}
	}
}
