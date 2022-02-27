using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class CleaningPlanGroupConfiguration : IEntityTypeConfiguration<CleaningPlanGroup>
	{
		public void Configure(EntityTypeBuilder<CleaningPlanGroup> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(CleaningPlanGroup.Id))
				.IsRequired();

			builder.Property(a => a.CleanerId)
				.HasColumnName(nameof(CleaningPlanGroup.CleanerId))
				.IsRequired();

			builder.Property(a => a.SecondaryCleanerId)
				.HasColumnName(nameof(CleaningPlanGroup.SecondaryCleanerId));

			builder.Property(a => a.CleaningPlanId)
				.HasColumnName(nameof(CleaningPlanGroup.CleaningPlanId))
				.IsRequired();

			builder.Property(a => a.MaxCredits)
				.HasColumnName(nameof(CleaningPlanGroup.MaxCredits));

			builder.Property(a => a.MaxDepartures)
				.HasColumnName(nameof(CleaningPlanGroup.MaxDepartures));

			builder.Property(a => a.MustFillAllCredits)
				.HasColumnName(nameof(CleaningPlanGroup.MustFillAllCredits))
				.IsRequired();

			builder.Property(a => a.WeeklyHours)
				.HasColumnName(nameof(CleaningPlanGroup.WeeklyHours));

			builder.Property(a => a.MaxTwins)
				.HasColumnName(nameof(CleaningPlanGroup.MaxTwins));

			builder
				.HasOne(g => g.Cleaner)
				.WithMany()
				.HasForeignKey(g => g.CleanerId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(g => g.SecondaryCleaner)
				.WithMany()
				.HasForeignKey(g => g.SecondaryCleanerId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(g => g.CleaningPlan)
				.WithMany(p => p.Groups)
				.HasForeignKey(g => g.CleaningPlanId)
				.OnDelete(DeleteBehavior.Cascade);

			builder
				.HasMany(g => g.Items)
				.WithOne(i => i.CleaningPlanGroup)
				.HasForeignKey(i => i.CleaningPlanGroupId)
				.OnDelete(DeleteBehavior.Cascade);

			//builder
			//	.HasMany(g => g.FloorAffinities)
			//	.WithOne(i => i.CleaningPlanGroup)
			//	.HasForeignKey(i => i.CleaningPlanGroupId)
			//	.OnDelete(DeleteBehavior.Cascade);

			builder
				.HasMany(g => g.Affinities)
				.WithOne(i => i.CleaningPlanGroup)
				.HasForeignKey(i => i.CleaningPlanGroupId)
				.OnDelete(DeleteBehavior.Cascade);

			builder
				.HasMany(g => g.AvailabilityIntervals)
				.WithOne(i => i.CleaningPlanGroup)
				.HasForeignKey(i => i.CleaningPlanGroupId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
