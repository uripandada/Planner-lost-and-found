using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class SystemTaskConfig : IEntityTypeConfiguration<SystemTask>
	{
		public void Configure(EntityTypeBuilder<SystemTask> builder)
		{
			builder.ConfigureChangeTrackingBaseEntity();

			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(SystemTask.Id))
				.IsRequired();

			builder
				.Property(a => a.TypeKey)
				.HasColumnName(nameof(SystemTask.TypeKey))
				.IsRequired();

			builder
				.Property(a => a.RepeatsForKey)
				.HasColumnName(nameof(SystemTask.RepeatsForKey));

			builder
				.Property(a => a.RecurringTypeKey)
				.HasColumnName(nameof(SystemTask.RecurringTypeKey));

			builder
				.Property(a => a.MustBeFinishedByAllWhos)
				.HasColumnName(nameof(SystemTask.MustBeFinishedByAllWhos))
				.IsRequired();

			builder
				.Property(a => a.Credits)
				.HasColumnName(nameof(SystemTask.Credits))
				.IsRequired();

			builder
				.Property(a => a.Price)
				.HasColumnName(nameof(SystemTask.Price))
				.IsRequired();

			builder
				.Property(a => a.PriorityKey)
				.HasColumnName(nameof(SystemTask.PriorityKey))
				.IsRequired();

			builder
				.Property(a => a.IsGuestRequest)
				.HasColumnName(nameof(SystemTask.IsGuestRequest))
				.IsRequired();

			builder
				.Property(a => a.IsShownInNewsFeed)
				.HasColumnName(nameof(SystemTask.IsShownInNewsFeed))
				.IsRequired();

			builder
				.Property(a => a.IsRescheduledEveryDayUntilFinished)
				.HasColumnName(nameof(SystemTask.IsRescheduledEveryDayUntilFinished))
				.IsRequired();

			builder
				.Property(a => a.IsMajorNotificationRaisedWhenFinished)
				.HasColumnName(nameof(SystemTask.IsMajorNotificationRaisedWhenFinished))
				.IsRequired();

			builder
				.Property(a => a.IsBlockingCleaningUntilFinished)
				.HasColumnName(nameof(SystemTask.IsBlockingCleaningUntilFinished))
				.IsRequired();

			builder
				.Property(a => a.UserId)
				.HasColumnName(nameof(SystemTask.UserId))
				.IsRequired(false);

			builder
				.Property(a => a.IsForPlannedAttendant)
				.HasColumnName(nameof(SystemTask.IsForPlannedAttendant))
				.IsRequired(true);

			builder
				.Property(a => a.WhereTypeKey)
				.HasColumnName(nameof(SystemTask.WhereTypeKey))
				.IsRequired();

			builder
				.Property(a => a.FromReservationId)
				.HasColumnName(nameof(SystemTask.FromReservationId));

			builder
				.Property(a => a.FromWarehouseId)
				.HasColumnName(nameof(SystemTask.FromWarehouseId));

			builder
				.Property(a => a.FromRoomId)
				.HasColumnName(nameof(SystemTask.FromRoomId));

			builder
				.Property(a => a.FromHotelId)
				.HasColumnName(nameof(SystemTask.FromHotelId));

			builder
				.Property(a => a.FromName)
				.HasColumnName(nameof(SystemTask.FromName));

			builder
				.Property(a => a.ToReservationId)
				.HasColumnName(nameof(SystemTask.ToReservationId));

			builder
				.Property(a => a.ToWarehouseId)
				.HasColumnName(nameof(SystemTask.ToWarehouseId));

			builder
				.Property(a => a.ToRoomId)
				.HasColumnName(nameof(SystemTask.ToRoomId));

			builder
				.Property(a => a.ToHotelId)
				.HasColumnName(nameof(SystemTask.ToHotelId));

			builder
				.Property(a => a.ToName)
				.HasColumnName(nameof(SystemTask.ToName));

			builder
				.Property(a => a.EventModifierKey)
				.HasColumnName(nameof(SystemTask.EventModifierKey));

			builder
				.Property(a => a.EventKey)
				.HasColumnName(nameof(SystemTask.EventKey));

			builder
				.Property(a => a.EventTimeKey)
				.HasColumnName(nameof(SystemTask.EventTimeKey));

			builder
				.Property(a => a.StatusKey)
				.HasColumnName(nameof(SystemTask.StatusKey))
				.IsRequired();

			builder
				.Property(a => a.StartsAt)
				.HasColumnName(nameof(SystemTask.StartsAt))
				.IsRequired();

			builder
				.Property(a => a.SystemTaskConfigurationId)
				.HasColumnName(nameof(SystemTask.SystemTaskConfigurationId))
				.IsRequired();

			builder
				.Property(a => a.IsManuallyModified)
				.HasColumnName(nameof(SystemTask.IsManuallyModified))
				.IsRequired();
			
			builder
				.Property(a => a.Comment)
				.HasColumnName(nameof(SystemTask.Comment));

			builder
				.HasOne(a => a.FromReservation)
				.WithMany()
				.HasForeignKey(a => a.FromReservationId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.FromWarehouse)
				.WithMany()
				.HasForeignKey(a => a.FromWarehouseId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.FromRoom)
				.WithMany()
				.HasForeignKey(a => a.FromRoomId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.FromHotel)
				.WithMany()
				.HasForeignKey(a => a.FromHotelId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.ToReservation)
				.WithMany()
				.HasForeignKey(a => a.ToReservationId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.ToWarehouse)
				.WithMany()
				.HasForeignKey(a => a.ToWarehouseId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.ToRoom)
				.WithMany()
				.HasForeignKey(a => a.ToRoomId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.ToHotel)
				.WithMany()
				.HasForeignKey(a => a.ToHotelId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.User)
				.WithMany()
				.HasForeignKey(a => a.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(t => t.Actions)
				.WithOne(h => h.SystemTask)
				.HasForeignKey(h => h.SystemTaskId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(t => t.History)
				.WithOne(h => h.SystemTask)
				.HasForeignKey(h => h.SystemTaskId)
				.OnDelete(DeleteBehavior.Restrict);
			builder
				.HasMany(t => t.Messages)
				.WithOne(h => h.SystemTask)
				.HasForeignKey(h => h.SystemTaskId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(t => t.SystemTaskConfiguration)
				.WithMany(c => c.Tasks)
				.HasForeignKey(t => t.SystemTaskConfigurationId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
