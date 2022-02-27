using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Common.Enums;
using Planner.Domain.Entities;
using System;

namespace Planner.Persistence.Configurations
{
	public class CleaningHistoryEventConfiguration : IEntityTypeConfiguration<CleaningHistoryEvent>
	{
		public void Configure(EntityTypeBuilder<CleaningHistoryEvent> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(CleaningHistoryEvent.Id))
				.IsRequired();

			builder.Property(a => a.Message)
				.HasColumnName(nameof(CleaningHistoryEvent.Message))
				.IsRequired();

			builder.Property(a => a.RoomId)
				.HasColumnName(nameof(CleaningHistoryEvent.RoomId))
				.IsRequired();
			
			builder.Property(a => a.CleaningId)
				.HasColumnName(nameof(CleaningHistoryEvent.CleaningId))
				.IsRequired();

			builder.Property(a => a.At)
				.HasColumnName(nameof(CleaningHistoryEvent.At))
				.IsRequired();

			builder.Property(a => a.UserId)
				.HasColumnName(nameof(CleaningHistoryEvent.UserId));

			builder.Property(a => a.OldData)
				.HasColumnName(nameof(CleaningHistoryEvent.OldData));

			builder.Property(a => a.NewData)
				.HasColumnName(nameof(CleaningHistoryEvent.NewData));

			builder.Property(a => a.Status)
			 .HasColumnName(nameof(CleaningHistoryEvent.Status))
			 .IsRequired()
			 .HasConversion(enumValue => enumValue.ToString(),
				stringValue => (CleaningProcessStatus)Enum.Parse(typeof(CleaningProcessStatus), stringValue));
			
			builder.Property(a => a.Type)
			 .HasColumnName(nameof(CleaningHistoryEvent.Type))
			 .IsRequired()
			 .HasConversion(enumValue => enumValue.ToString(),
				stringValue => (CleaningEventType)Enum.Parse(typeof(CleaningEventType), stringValue));

			builder
				.HasOne(a => a.User)
				.WithMany(u => u.CleaningHistoryEvents)
				.HasForeignKey(a => a.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.Room)
				.WithMany(r => r.CleaningHistoryEvents)
				.HasForeignKey(a => a.RoomId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.Cleaning)
				.WithMany(r => r.CleaningHistoryEvents)
				.HasForeignKey(a => a.CleaningId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
