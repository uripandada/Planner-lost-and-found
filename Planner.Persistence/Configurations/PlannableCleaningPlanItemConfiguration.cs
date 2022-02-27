using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class PlannableCleaningPlanItemConfiguration : IEntityTypeConfiguration<PlannableCleaningPlanItem>
	{
		public void Configure(EntityTypeBuilder<PlannableCleaningPlanItem> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(PlannableCleaningPlanItem.Id))
				.IsRequired();

			builder.Property(a => a.CleaningPluginName)
				.HasColumnName(nameof(PlannableCleaningPlanItem.CleaningPluginName));

			builder.Property(a => a.Credits)
				.HasColumnName(nameof(PlannableCleaningPlanItem.Credits));

			builder.Property(a => a.IsActive)
				.HasColumnName(nameof(PlannableCleaningPlanItem.IsActive))
				.IsRequired();

			builder.Property(a => a.IsCustom)
				.HasColumnName(nameof(PlannableCleaningPlanItem.IsCustom))
				.IsRequired();

			builder.Property(a => a.IsPostponed)
				.HasColumnName(nameof(PlannableCleaningPlanItem.IsPostponed))
				.IsRequired();

			builder.Property(a => a.IsPlanned)
				.HasColumnName(nameof(PlannableCleaningPlanItem.IsPlanned))
				.IsRequired();

			builder.Property(a => a.IsChangeSheets)
				.HasColumnName(nameof(PlannableCleaningPlanItem.IsChangeSheets))
				.IsRequired();

			builder.Property(a => a.CleaningPlanId)
				.HasColumnName(nameof(PlannableCleaningPlanItem.CleaningPlanId))
				.IsRequired();

			builder
				.HasOne(cpi => cpi.CleaningPlan)
				.WithMany()
				.HasForeignKey(cpi => cpi.CleaningPlanId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(a => a.CleaningPluginId)
				.HasColumnName(nameof(PlannableCleaningPlanItem.CleaningPluginId));

			builder
				.HasOne(cpi => cpi.CleaningPlugin)
				.WithMany()
				.HasForeignKey(cpi => cpi.CleaningPluginId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(a => a.RoomId)
				.HasColumnName(nameof(PlannableCleaningPlanItem.RoomId))
				.IsRequired();

			builder
				.HasOne(cpi => cpi.Room)
				.WithMany()
				.HasForeignKey(cpi => cpi.RoomId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(a => a.PostponedFromPlannableCleaningPlanItemId)
				.HasColumnName(nameof(PlannableCleaningPlanItem.PostponedFromPlannableCleaningPlanItemId));

			builder
				.HasOne(cpi => cpi.PostponedFromPlannableCleaningPlanItem)
				.WithMany(cpii => cpii.PostponedToPlannableCleaningPlanItems)
				.HasForeignKey(cpi => cpi.PostponedFromPlannableCleaningPlanItemId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
