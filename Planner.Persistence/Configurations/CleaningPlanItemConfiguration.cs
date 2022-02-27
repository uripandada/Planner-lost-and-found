using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Common.Enums;
using Planner.Domain.Entities;
using System;

namespace Planner.Persistence.Configurations
{
	public class CleaningPlanItemConfiguration : IEntityTypeConfiguration<CleaningPlanItem>
	{
		public void Configure(EntityTypeBuilder<CleaningPlanItem> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(CleaningPlanItem.Id))
				.IsRequired();

			builder.Property(a => a.Description)
				.HasColumnName(nameof(CleaningPlanItem.Description));

			builder.Property(a => a.Credits)
				.HasColumnName(nameof(CleaningPlanItem.Credits));

			builder.Property(a => a.IsActive)
				.HasColumnName(nameof(CleaningPlanItem.IsActive))
				.IsRequired();

			builder.Property(a => a.IsCustom)
				.HasColumnName(nameof(CleaningPlanItem.IsCustom))
				.IsRequired();

			builder.Property(a => a.IsPostponed)
				.HasColumnName(nameof(CleaningPlanItem.IsPostponed))
				.IsRequired();

			builder.Property(a => a.IsChangeSheets)
				.HasColumnName(nameof(CleaningPlanItem.IsChangeSheets))
				.IsRequired();
			
			builder.Property(a => a.IsPlanned)
				.HasColumnName(nameof(CleaningPlanItem.IsPlanned))
				.IsRequired();
			
			builder.Property(a => a.IsPriority)
				.HasColumnName(nameof(CleaningPlanItem.IsPriority))
				.IsRequired();

			builder.Property(a => a.StartsAt)
				.HasColumnName(nameof(CleaningPlanItem.StartsAt));

			builder.Property(a => a.EndsAt)
				.HasColumnName(nameof(CleaningPlanItem.EndsAt));

			builder.Property(a => a.DurationSec)
				.HasColumnName(nameof(CleaningPlanItem.DurationSec));



			builder.Property(a => a.IsPostponer)
				.HasColumnName(nameof(CleaningPlanItem.IsPostponer))
				.IsRequired();

			builder.Property(a => a.IsPostponee)
				.HasColumnName(nameof(CleaningPlanItem.IsPostponee))
				.IsRequired();
			
			builder.Property(a => a.PostponerCleaningPlanItemId)
				.HasColumnName(nameof(CleaningPlanItem.PostponerCleaningPlanItemId));
			
			builder
				.HasOne(a => a.PostponerCleaningPlanItem)
				.WithMany()
				.HasForeignKey(a => a.PostponerCleaningPlanItemId)
				.OnDelete(DeleteBehavior.SetNull);
			
			builder.Property(a => a.PostponeeCleaningPlanItemId)
				.HasColumnName(nameof(CleaningPlanItem.PostponeeCleaningPlanItemId));
			
			builder
				.HasOne(a => a.PostponeeCleaningPlanItem)
				.WithMany()
				.HasForeignKey(a => a.PostponeeCleaningPlanItemId)
				.OnDelete(DeleteBehavior.SetNull);





			builder.Property(a => a.CleaningPlanId)
				.HasColumnName(nameof(CleaningPlanItem.CleaningPlanId))
				.IsRequired();

			builder
				.HasOne(cpi => cpi.CleaningPlan)
				.WithMany(g => g.UngroupedItems)
				.HasForeignKey(cpi => cpi.CleaningPlanId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(a => a.CleaningPlanGroupId)
				.HasColumnName(nameof(CleaningPlanItem.CleaningPlanGroupId));

			builder
				.HasOne(cpi => cpi.CleaningPlanGroup)
				.WithMany(g => g.Items)
				.HasForeignKey(cpi => cpi.CleaningPlanGroupId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Property(a => a.RoomId)
				.HasColumnName(nameof(CleaningPlanItem.RoomId))
				.IsRequired();

			builder
				.HasOne(cpi => cpi.Room)
				.WithMany()
				.HasForeignKey(cpi => cpi.RoomId)
				.OnDelete(DeleteBehavior.Restrict);

			
			builder.Property(a => a.RoomBedId)
				.HasColumnName(nameof(CleaningPlanItem.RoomBedId));

			builder
				.HasOne(cpi => cpi.RoomBed)
				.WithMany()
				.HasForeignKey(cpi => cpi.RoomBedId)
				.OnDelete(DeleteBehavior.Restrict);


			builder.Property(a => a.CleaningPluginId)
				.HasColumnName(nameof(CleaningPlanItem.CleaningPluginId));

			builder
				.HasOne(cpi => cpi.CleaningPlugin)
				.WithMany()
				.HasForeignKey(cpi => cpi.CleaningPluginId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(a => a.CleaningId)
				.HasColumnName(nameof(CleaningPlanItem.CleaningId));

			builder
				.HasOne(cpi => cpi.Cleaning)
				.WithMany(c => c.CleaningPlanItems)
				.HasForeignKey(cpi => cpi.CleaningId)
				.OnDelete(DeleteBehavior.Restrict);

		}
	}
}
