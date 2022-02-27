using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
	{
		public void Configure(EntityTypeBuilder<Reservation> builder)
		{

			builder.HasKey(a => a.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(Reservation.Id))
				.IsRequired();

			builder.Property(a => a.ActualCheckIn)
				.HasColumnName(nameof(Reservation.ActualCheckIn));

			builder.Property(a => a.ActualCheckOut)
				.HasColumnName(nameof(Reservation.ActualCheckOut));

			builder.Property(a => a.CheckIn)
				.HasColumnName(nameof(Reservation.CheckIn));

			builder.Property(a => a.CheckOut)
				.HasColumnName(nameof(Reservation.CheckOut));

			builder.Property(a => a.IsActive)
				.HasColumnName(nameof(Reservation.IsActive))
				.IsRequired();

			builder.Property(a => a.IsSynchronizedFromRcc)
				.HasColumnName(nameof(Reservation.IsSynchronizedFromRcc))
				.IsRequired();

			builder.Property(a => a.LastTimeModifiedBySynchronization)
				.HasColumnName(nameof(Reservation.LastTimeModifiedBySynchronization));

			builder.Property(a => a.GuestName)
				.HasColumnName(nameof(Reservation.GuestName))
				.IsRequired();

			builder.Property(a => a.NumberOfAdults)
				.HasColumnName(nameof(Reservation.NumberOfAdults))
				.IsRequired();

			builder.Property(a => a.NumberOfChildren)
				.HasColumnName(nameof(Reservation.NumberOfChildren))
				.IsRequired();

			builder.Property(a => a.NumberOfInfants)
				.HasColumnName(nameof(Reservation.NumberOfInfants))
				.IsRequired();

			builder.Property(a => a.PmsNote)
				.HasColumnName(nameof(Reservation.PmsNote));
			
			builder.Property(a => a.Group)
				.HasColumnName(nameof(Reservation.Group));

			builder.Property(a => a.PMSRoomName)
				.HasColumnName(nameof(Reservation.PMSRoomName));
			builder.Property(a => a.BedName)
				.HasColumnName(nameof(Reservation.BedName));
			builder.Property(a => a.PMSBedName)
				.HasColumnName(nameof(Reservation.PMSBedName));

			builder.Property(a => a.RccReservationStatusKey)
				.HasColumnName(nameof(Reservation.RccReservationStatusKey))
				.IsRequired();
			
			builder.Property(a => a.IsActiveToday)
				.HasColumnName(nameof(Reservation.IsActiveToday))
				.IsRequired();
			
			builder.Property(a => a.ReservationStatusKey)
				.HasColumnName(nameof(Reservation.ReservationStatusKey));
			
			builder.Property(a => a.ReservationStatusDescription)
				.HasColumnName(nameof(Reservation.ReservationStatusDescription));

			builder.Property(a => a.RoomName)
				.HasColumnName(nameof(Reservation.RoomName));

			builder.Property(a => a.SynchronizedAt)
				.HasColumnName(nameof(Reservation.SynchronizedAt));

			builder.Property(a => a.Vip)
				.HasColumnName(nameof(Reservation.Vip));

			builder.Property(a => a.OtherProperties)
				.HasColumnName(nameof(Reservation.OtherProperties))
				.HasColumnType("jsonb");

			builder.Property(a => a.HotelId)
				.HasColumnName(nameof(Reservation.HotelId))
				.IsRequired();

			builder.Property(a => a.RoomId)
				.HasColumnName(nameof(Reservation.RoomId));
			
			builder.Property(a => a.RoomBedId)
				.HasColumnName(nameof(Reservation.RoomBedId));

			builder
				.HasOne(a => a.Room)
				.WithMany(r => r.Reservations)
				.HasForeignKey(a => a.RoomId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.RoomBed)
				.WithMany(bed => bed.Reservations)
				.HasForeignKey(a => a.RoomBedId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(a => a.Hotel)
				.WithMany()
				.HasForeignKey(a => a.HotelId)
				.OnDelete(DeleteBehavior.Restrict);

		}
	}

}
