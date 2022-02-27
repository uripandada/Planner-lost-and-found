using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class SystemTaskConfigurationConfig : IEntityTypeConfiguration<SystemTaskConfiguration>
	{
		public void Configure(EntityTypeBuilder<SystemTaskConfiguration> builder)
		{
			builder.ConfigureChangeTrackingBaseEntity();

			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(SystemTaskConfiguration.Id))
				.IsRequired();

			builder.Property(a => a.Data)
				.HasColumnName(nameof(SystemTaskConfiguration.Data))
				.HasColumnType("jsonb")
				.IsRequired();

			builder
				.HasMany(a => a.Tasks)
				.WithOne(t => t.SystemTaskConfiguration)
				.HasForeignKey(t => t.SystemTaskConfigurationId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}

}
