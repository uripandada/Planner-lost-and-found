using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Common.Enums;
using Planner.Domain.Entities;
using System;

namespace Planner.Persistence.Configurations
{
	public class UserHistoryEventConfiguration : IEntityTypeConfiguration<UserHistoryEvent>
    {
        public void Configure(EntityTypeBuilder<UserHistoryEvent> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(UserHistoryEvent.Id))
                .IsRequired();

            builder.Property(a => a.Message)
                .HasColumnName(nameof(UserHistoryEvent.Message))
                .IsRequired();

            builder.Property(a => a.At)
                .HasColumnName(nameof(UserHistoryEvent.At))
                .IsRequired();

            builder.Property(a => a.UserId)
                .HasColumnName(nameof(UserHistoryEvent.UserId));

            builder.Property(a => a.OldData)
                .HasColumnName(nameof(UserHistoryEvent.OldData));

            builder.Property(a => a.NewData)
                .HasColumnName(nameof(UserHistoryEvent.NewData));

            builder.Property(a => a.Type)
             .HasColumnName(nameof(UserHistoryEvent.Type))
             .IsRequired()
             .HasConversion(enumValue => enumValue.ToString(),
                stringValue => (UserEventType)Enum.Parse(typeof(UserEventType), stringValue));

            builder
                .HasOne(a => a.User)
                .WithMany(u => u.UserHistoryEvents)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
