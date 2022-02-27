using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Common.Enums;
using Planner.Domain.Entities;
using System;

namespace Planner.Persistence.Configurations
{
	public class CleaningConfiguration : IEntityTypeConfiguration<Cleaning>
	{
		public void Configure(EntityTypeBuilder<Cleaning> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(Cleaning.Id))
				.IsRequired();

			builder.Property(a => a.Description)
				.HasColumnName(nameof(Cleaning.Description));

			builder.Property(a => a.Credits)
				.HasColumnName(nameof(Cleaning.Credits));

			builder.Property(a => a.IsActive)
				.HasColumnName(nameof(Cleaning.IsActive))
				.IsRequired();

			builder.Property(a => a.IsCustom)
				.HasColumnName(nameof(Cleaning.IsCustom))
				.IsRequired();

			builder.Property(a => a.IsPostponed)
				.HasColumnName(nameof(Cleaning.IsPostponed))
				.IsRequired();

			builder.Property(a => a.IsChangeSheets)
				.HasColumnName(nameof(Cleaning.IsChangeSheets))
				.IsRequired();
			
			builder.Property(a => a.IsPlanned)
				.HasColumnName(nameof(Cleaning.IsPlanned))
				.IsRequired();

			builder.Property(a => a.IsPriority)
				.HasColumnName(nameof(Cleaning.IsPriority))
				.IsRequired();

			builder.Property(a => a.IsInspectionRequired)
				.HasColumnName(nameof(Cleaning.IsInspectionRequired))
				.IsRequired();

			builder.Property(a => a.IsReadyForInspection)
				.HasColumnName(nameof(Cleaning.IsReadyForInspection))
				.IsRequired();

			builder.Property(a => a.IsInspected)
				.HasColumnName(nameof(Cleaning.IsInspected))
				.IsRequired();

			builder.Property(a => a.IsInspectionSuccess)
				.HasColumnName(nameof(Cleaning.IsInspectionSuccess))
				.IsRequired();
			
			builder.Property(a => a.InspectedById)
				.HasColumnName(nameof(Cleaning.InspectedById));

			builder
				.HasOne(a => a.InspectedBy)
				.WithMany(user => user.InspectedCleanings)
				.HasForeignKey(a => a.InspectedById)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(a => a.Status)
				.HasColumnName(nameof(Cleaning.Status))
				.IsRequired()
				.HasDefaultValue(CleaningProcessStatus.DRAFT)
				.HasConversion(
					a => a.ToString(),
					a => (CleaningProcessStatus)Enum.Parse(typeof(CleaningProcessStatus), a)
				);

			builder.Property(a => a.StartsAt)
				.HasColumnName(nameof(Cleaning.StartsAt))
				.IsRequired();

			builder.Property(a => a.EndsAt)
				.HasColumnName(nameof(Cleaning.EndsAt))
				.IsRequired();

			builder.Property(a => a.DurationSec)
				.HasColumnName(nameof(Cleaning.DurationSec))
				.IsRequired();


			builder.Property(a => a.CleanerId)
				.HasColumnName(nameof(Cleaning.CleanerId))
				.IsRequired();

			builder
				.HasOne(cpi => cpi.Cleaner)
				.WithMany(user => user.Cleanings)
				.HasForeignKey(cpi => cpi.CleanerId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(a => a.RoomId)
				.HasColumnName(nameof(Cleaning.RoomId))
				.IsRequired();

			builder
				.HasOne(cpi => cpi.Room)
				.WithMany(room => room.Cleanings)
				.HasForeignKey(cpi => cpi.RoomId)
				.OnDelete(DeleteBehavior.Restrict);
			
			builder.Property(a => a.RoomBedId)
			.HasColumnName(nameof(Cleaning.RoomBedId));

			builder
				.HasOne(cpi => cpi.RoomBed)
				.WithMany(room => room.Cleanings)
				.HasForeignKey(cpi => cpi.RoomBedId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(a => a.CleaningPluginId)
				.HasColumnName(nameof(Cleaning.CleaningPluginId));

			builder
				.HasOne(cpi => cpi.CleaningPlugin)
				.WithMany()
				.HasForeignKey(cpi => cpi.CleaningPluginId)
				.OnDelete(DeleteBehavior.Restrict);
			
			builder.Property(a => a.CleaningPlanId)
				.HasColumnName(nameof(Cleaning.CleaningPlanId))
				.IsRequired();

			builder
				.HasOne(cpi => cpi.CleaningPlan)
				.WithMany(p => p.Cleanings)
				.HasForeignKey(cpi => cpi.CleaningPlanId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(i => i.CleaningInspections)
				.WithOne(ci => ci.Cleaning)
				.HasForeignKey(ci => ci.CleaningId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(r => r.CleaningHistoryEvents)
				.WithOne(a => a.Cleaning)
				.HasForeignKey(a => a.CleaningId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(r => r.CleaningPlanItems)
				.WithOne(a => a.Cleaning)
				.HasForeignKey(a => a.CleaningId)
				.OnDelete(DeleteBehavior.Restrict);

			//builder.Property(a => a.ModifiedAt)
			//	.HasColumnName(nameof(Cleaning.ModifiedAt))
			//	.IsRequired()
			//	.HasDefaultValueSql("now()");

		}
	}
}
