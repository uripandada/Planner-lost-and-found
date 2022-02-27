using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Common.Enums;
using Planner.Domain.Entities;
using System;

namespace Planner.Persistence.Configurations
{
	public class AutomaticHousekeepingUpdateSettingsConfiguration : IEntityTypeConfiguration<AutomaticHousekeepingUpdateSettings>
    {
        public void Configure(EntityTypeBuilder<AutomaticHousekeepingUpdateSettings> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.Id))
                .IsRequired();
            
            builder.Property(a => a.CreatedAt)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.CreatedAt))
                .IsRequired()
                .HasDefaultValueSql("now()");

            builder.Property(a => a.HotelId)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.HotelId))
                .IsRequired();

            builder
                .HasOne(a => a.Hotel)
                .WithMany()
                .HasForeignKey(a => a.HotelId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Property(a => a.Dirty)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.Dirty))
                .IsRequired();

            builder.Property(a => a.Clean)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.Clean))
                .IsRequired();

            builder.Property(a => a.CleanNeedsInspection)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.CleanNeedsInspection))
                .IsRequired();

            builder.Property(a => a.Inspected)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.Inspected))
                .IsRequired();

            builder.Property(a => a.Vacant)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.Vacant))
                .IsRequired();

            builder.Property(a => a.Occupied)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.Occupied))
                .IsRequired();

            builder.Property(a => a.DoNotDisturb)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.DoNotDisturb))
                .IsRequired();

            builder.Property(a => a.DoDisturb)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.DoDisturb))
                .IsRequired();

            builder.Property(a => a.OutOfService)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.OutOfService))
                .IsRequired();

            builder.Property(a => a.InService)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.InService))
                .IsRequired();

            builder.Property(a => a.RoomNameRegex)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.RoomNameRegex))
                .IsRequired();

            builder.Property(a => a.UpdateStatusAtTime)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.UpdateStatusAtTime))
                .IsRequired();

            builder.Property(a => a.UpdateStatusTo)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.UpdateStatusTo))
                .HasConversion(
                    a => a.ToString(),
                    a => (AutomaticHousekeepingUpdateCleaningStatusTo)Enum.Parse(typeof(AutomaticHousekeepingUpdateCleaningStatusTo), a)
                );

            builder.Property(a => a.UpdateStatusWhen)
                .HasColumnName(nameof(AutomaticHousekeepingUpdateSettings.UpdateStatusWhen))
                .HasConversion(
                    a => a.ToString(),
                    a => (AutomaticHousekeepingUpdateCleaningStatusWhen)Enum.Parse(typeof(AutomaticHousekeepingUpdateCleaningStatusWhen), a)
                );
        }
    }
}
