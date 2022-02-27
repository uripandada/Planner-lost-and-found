using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class CleaningPlanConfiguration : IEntityTypeConfiguration<CleaningPlan>
	{
		public void Configure(EntityTypeBuilder<CleaningPlan> builder)
		{
			builder.ConfigureBaseEntity();

			builder.HasKey(cp => cp.Id);

			builder.Property(cp => cp.Id)
				.HasColumnName(nameof(CleaningPlan.Id))
				.IsRequired();

			builder.Property(cp => cp.Date)
				.HasColumnName(nameof(CleaningPlan.Date))
				.HasColumnType("date")
				.IsRequired();

			builder.Property(cp => cp.IsSent)
				.HasColumnName(nameof(CleaningPlan.IsSent))
				.IsRequired();

			builder.Property(cp => cp.SentAt)
				.HasColumnName(nameof(CleaningPlan.SentAt));

			builder.Property(cp => cp.SentById)
				.HasColumnName(nameof(CleaningPlan.SentById));

			builder
				.HasOne(cp => cp.SentBy)
				.WithMany()
				.HasForeignKey(cp => cp.SentById)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(cp => cp.SendingHistory)
				.WithOne(sh => sh.CleaningPlan)
				.HasForeignKey(sh => sh.CleaningPlanId);

			builder
				.HasIndex(cp => new { cp.HotelId, cp.Date })
				.IsUnique();

			builder
				.HasMany(cp => cp.Groups)
				.WithOne(cpi => cpi.CleaningPlan)
				.HasForeignKey(cpi => cpi.CleaningPlanId)
				.OnDelete(DeleteBehavior.Cascade);
			
			builder
				.HasMany(cp => cp.UngroupedItems)
				.WithOne(cpi => cpi.CleaningPlan)
				.HasForeignKey(cpi => cpi.CleaningPlanId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(cp => cp.UngroupedItems)
				.WithOne(cpi => cpi.CleaningPlan)
				.HasForeignKey(cpi => cpi.CleaningPlanId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(cp => cp.CleaningPlanCpsatConfiguration)
				.WithOne(c => c.CleaningPlan)
				.HasForeignKey<CleaningPlanCpsatConfiguration>(cp => cp.Id)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(p => p.Cleanings)
				.WithOne(cpi => cpi.CleaningPlan)
				.HasForeignKey(cpi => cpi.CleaningPlanId)
				.OnDelete(DeleteBehavior.Restrict);

		}
	}
}
