using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class CleaningPlanGroupAvailabilityIntervalConfiguration : IEntityTypeConfiguration<CleaningPlanGroupAvailabilityInterval>
    {
        public void Configure(EntityTypeBuilder<CleaningPlanGroupAvailabilityInterval> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(CleaningPlanGroupAvailabilityInterval.Id))
                .IsRequired();

            builder.Property(a => a.CleaningPlanGroupId)
                .HasColumnName(nameof(CleaningPlanGroupAvailabilityInterval.CleaningPlanGroupId))
                .IsRequired();

            builder.Property(a => a.From)
                .HasColumnName(nameof(CleaningPlanGroupAvailabilityInterval.From))
                .HasColumnType("time")
                .IsRequired();

            builder.Property(a => a.To)
                .HasColumnName(nameof(CleaningPlanGroupAvailabilityInterval.To))
                .HasColumnType("time")
                .IsRequired();

            builder
                .HasOne(a => a.CleaningPlanGroup)
                .WithMany(g => g.AvailabilityIntervals)
                .HasForeignKey(a => a.CleaningPlanGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    



















}
