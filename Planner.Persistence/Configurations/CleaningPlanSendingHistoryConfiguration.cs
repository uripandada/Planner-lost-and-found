using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class CleaningPlanSendingHistoryConfiguration : IEntityTypeConfiguration<CleaningPlanSendingHistory>
    {
        public void Configure(EntityTypeBuilder<CleaningPlanSendingHistory> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(CleaningPlanSendingHistory.Id))
                .IsRequired();

            builder.Property(a => a.SentAt)
                .HasColumnName(nameof(CleaningPlanSendingHistory.SentAt))
                .IsRequired();

            builder.Property(a => a.SentById)
                .HasColumnName(nameof(CleaningPlanSendingHistory.SentById))
                .IsRequired();

            builder
                .HasOne(a => a.SentBy)
                .WithMany()
                .HasForeignKey(a => a.SentById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(a => a.CleaningPlanId)
                .HasColumnName(nameof(CleaningPlanSendingHistory.CleaningPlanId))
                .IsRequired();

            builder.Property(a => a.CleaningPlanJson)
                .HasColumnName(nameof(CleaningPlanSendingHistory.CleaningPlanJson));

            builder
                .HasOne(a => a.CleaningPlan)
                .WithMany(b => b.SendingHistory)
                .HasForeignKey(a => a.CleaningPlanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
