using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Common.Enums;
using Planner.Domain.Entities;
using System;

namespace Planner.Persistence.Configurations
{
	public class RoomMessageConfiguration : IEntityTypeConfiguration<RoomMessage>
	{
		public void Configure(EntityTypeBuilder<RoomMessage> builder)
		{
			builder.ConfigureBaseEntity();

			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(RoomMessage.Id))
				.IsRequired();

			builder.Property(a => a.Message)
				.HasColumnName(nameof(RoomMessage.Message))
				.IsRequired();

			builder.Property(a => a.IsDeleted)
				.HasColumnName(nameof(RoomMessage.IsDeleted))
				.IsRequired();

			builder.Property(a => a.Type)
				.HasColumnName(nameof(RoomMessage.Type))
				.IsRequired()
				.HasDefaultValue(RoomMessageType.SIMPLE)
				.HasConversion(a => a.ToString(), a => (RoomMessageType)Enum.Parse(typeof(RoomMessageType), a));

			builder.Property(a => a.ForType)
				.HasColumnName(nameof(RoomMessage.ForType))
				.IsRequired()
				.HasDefaultValue(RoomMessageForType.PLACES)
				.HasConversion(a => a.ToString(), a => (RoomMessageForType)Enum.Parse(typeof(RoomMessageForType), a));

			builder.Property(a => a.DateType)
				.HasColumnName(nameof(RoomMessage.DateType))
				.IsRequired()
				.HasDefaultValue(RoomMessageDateType.SPECIFIC_DATES)
				.HasConversion(a => a.ToString(), a => (RoomMessageDateType)Enum.Parse(typeof(RoomMessageDateType), a));

			builder.Property(a => a.IntervalStartDate)
				.HasColumnName(nameof(RoomMessage.IntervalStartDate))
				.HasColumnType("date");

			builder.Property(a => a.IntervalEndDate)
				.HasColumnName(nameof(RoomMessage.IntervalEndDate))
				.HasColumnType("date");

			builder.Property(a => a.IntervalNumberOfDays)
				.HasColumnName(nameof(RoomMessage.IntervalNumberOfDays));

			builder.Property(a => a.ReservationOnArrivalDate)
				.HasColumnName(nameof(RoomMessage.ReservationOnArrivalDate));

			builder.Property(a => a.ReservationOnDepartureDate)
				.HasColumnName(nameof(RoomMessage.ReservationOnDepartureDate));

			builder.Property(a => a.ReservationOnStayDates)
				.HasColumnName(nameof(RoomMessage.ReservationOnStayDates));

			builder
				.HasMany(m => m.RoomMessageFilters)
				.WithOne(mi => mi.RoomMessage)
				.HasForeignKey(mi => mi.RoomMessageId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(m => m.RoomMessageDates)
				.WithOne(mi => mi.RoomMessage)
				.HasForeignKey(mi => mi.RoomMessageId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(m => m.RoomMessageRooms)
				.WithOne(mi => mi.RoomMessage)
				.HasForeignKey(mi => mi.RoomMessageId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(m => m.RoomMessageReservations)
				.WithOne(mi => mi.RoomMessage)
				.HasForeignKey(mi => mi.RoomMessageId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
	public class RoomMessageFilterConfiguration : IEntityTypeConfiguration<RoomMessageFilter>
	{
		public void Configure(EntityTypeBuilder<RoomMessageFilter> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(RoomMessageFilter.Id))
				.IsRequired();

			builder.Property(a => a.RoomMessageId)
				.HasColumnName(nameof(RoomMessageFilter.RoomMessageId))
				.IsRequired();

			builder
				.HasOne(a => a.RoomMessage)
				.WithMany(rm => rm.RoomMessageFilters)
				.HasForeignKey(a => a.RoomMessageId)
				.OnDelete(DeleteBehavior.Restrict);


			builder.Property(a => a.ReferenceId)
				.HasColumnName(nameof(RoomMessageFilter.ReferenceId))
				.IsRequired();

			builder.Property(a => a.ReferenceType)
				.HasColumnName(nameof(RoomMessageFilter.ReferenceType))
				.IsRequired()
				.HasDefaultValue(RoomMessageFilterReferenceType.OTHERS)
				.HasConversion(a => a.ToString(), a => (RoomMessageFilterReferenceType)Enum.Parse(typeof(RoomMessageFilterReferenceType), a));

			builder.Property(a => a.ReferenceName)
				.HasColumnName(nameof(RoomMessageFilter.ReferenceName))
				.IsRequired();

			builder.Property(a => a.ReferenceDescription)
				.HasColumnName(nameof(RoomMessageFilter.ReferenceDescription))
				.IsRequired();
		}
	}
	public class RoomMessageDateConfiguration : IEntityTypeConfiguration<RoomMessageDate>
	{
		public void Configure(EntityTypeBuilder<RoomMessageDate> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(RoomMessageDate.Id))
				.IsRequired();

			builder.Property(a => a.Date)
				.HasColumnName(nameof(RoomMessageDate.Date))
				.HasColumnType("date")
				.IsRequired();

			builder.Property(a => a.RoomMessageId)
				.HasColumnName(nameof(RoomMessageDate.RoomMessageId))
				.IsRequired();

			builder
				.HasOne(a => a.RoomMessage)
				.WithMany(rm => rm.RoomMessageDates)
				.HasForeignKey(a => a.RoomMessageId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
	public class RoomMessageRoomConfiguration : IEntityTypeConfiguration<RoomMessageRoom>
	{
		public void Configure(EntityTypeBuilder<RoomMessageRoom> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(RoomMessageRoom.Id))
				.IsRequired();

			builder.Property(a => a.Date)
				.HasColumnName(nameof(RoomMessageRoom.Date))
				.HasColumnType("date")
				.IsRequired();

			builder.Property(a => a.RoomMessageId)
				.HasColumnName(nameof(RoomMessageRoom.RoomMessageId))
				.IsRequired();

			builder.Property(a => a.RoomId)
				.HasColumnName(nameof(RoomMessageRoom.RoomId))
				.IsRequired();

			builder
				.HasOne(a => a.RoomMessage)
				.WithMany(rm => rm.RoomMessageRooms)
				.HasForeignKey(a => a.RoomMessageId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.Room)
				.WithMany()
				.HasForeignKey(a => a.RoomId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
	public class RoomMessageReservationConfiguration : IEntityTypeConfiguration<RoomMessageReservation>
	{
		public void Configure(EntityTypeBuilder<RoomMessageReservation> builder)
		{
			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(RoomMessageReservation.Id))
				.IsRequired();

			builder.Property(a => a.Date)
				.HasColumnName(nameof(RoomMessageReservation.Date))
				.HasColumnType("date")
				.IsRequired();

			builder.Property(a => a.RoomMessageId)
				.HasColumnName(nameof(RoomMessageReservation.RoomMessageId))
				.IsRequired();

			builder.Property(a => a.ReservationId)
				.HasColumnName(nameof(RoomMessageReservation.ReservationId))
				.IsRequired();

			builder
				.HasOne(a => a.RoomMessage)
				.WithMany(rm => rm.RoomMessageReservations)
				.HasForeignKey(a => a.RoomMessageId)
				.OnDelete(DeleteBehavior.Restrict);
			
			builder
				.HasOne(a => a.Reservation)
				.WithMany()
				.HasForeignKey(a => a.ReservationId)
				.OnDelete(DeleteBehavior.Restrict);

		}
	}

}
