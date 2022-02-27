using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class HousekeepingNightlyUpdateCycleConfiguration : IEntityTypeConfiguration<AutomaticHousekeepingUpdateCycle>
	{
		public void Configure(EntityTypeBuilder<AutomaticHousekeepingUpdateCycle> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(AutomaticHousekeepingUpdateCycle.Id))
				.IsRequired();

			builder.Property(a => a.StartedAt)
				.HasColumnName(nameof(AutomaticHousekeepingUpdateCycle.StartedAt))
				.IsRequired();

			builder.Property(a => a.EndedAt)
				.HasColumnName(nameof(AutomaticHousekeepingUpdateCycle.EndedAt));

			builder.Property(a => a.InProgress)
				.HasColumnName(nameof(AutomaticHousekeepingUpdateCycle.InProgress))
				.IsRequired();

			builder
				.Property(a => a.StateChanges)
				.HasColumnName(nameof(AutomaticHousekeepingUpdateCycle.StateChanges));
		}
	}
}
