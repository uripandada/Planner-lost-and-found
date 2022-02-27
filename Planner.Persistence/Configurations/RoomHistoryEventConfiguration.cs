using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Common.Enums;
using Planner.Domain.Entities;
using System;

namespace Planner.Persistence.Configurations
{
	public class RoomHistoryEventConfiguration : IEntityTypeConfiguration<RoomHistoryEvent>
	{
		public void Configure(EntityTypeBuilder<RoomHistoryEvent> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(RoomHistoryEvent.Id))
				.IsRequired();
			
			builder.Property(a => a.Message)
				.HasColumnName(nameof(RoomHistoryEvent.Message))
				.IsRequired();
			
			builder.Property(a => a.RoomId)
				.HasColumnName(nameof(RoomHistoryEvent.RoomId))
				.IsRequired();
			
			builder.Property(a => a.RoomBedId)
				.HasColumnName(nameof(RoomHistoryEvent.RoomBedId));
			
			builder.Property(a => a.At)
				.HasColumnName(nameof(RoomHistoryEvent.At))
				.IsRequired();
			
			builder.Property(a => a.UserId)
				.HasColumnName(nameof(RoomHistoryEvent.UserId));
			
			builder.Property(a => a.OldData)
				.HasColumnName(nameof(RoomHistoryEvent.OldData));
			
			builder.Property(a => a.NewData)
				.HasColumnName(nameof(RoomHistoryEvent.NewData));

			builder.Property(a => a.Type)
			 .HasColumnName(nameof(RoomHistoryEvent.Type))
			 .IsRequired()
			 .HasConversion(enumValue => enumValue.ToString(),
				stringValue => (RoomEventType)Enum.Parse(typeof(RoomEventType), stringValue));

			builder
				.HasOne(a => a.User)
				.WithMany(u => u.RoomHistoryEvents)
				.HasForeignKey(a => a.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.Room)
				.WithMany(r => r.RoomHistoryEvents)
				.HasForeignKey(a => a.RoomId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.RoomBed)
				.WithMany(r => r.RoomHistoryEvents)
				.HasForeignKey(a => a.RoomBedId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
