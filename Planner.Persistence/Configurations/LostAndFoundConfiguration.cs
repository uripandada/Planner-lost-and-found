using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;
using Planner.Domain.Values;
using System;

namespace Planner.Persistence.Configurations
{
	public class LostAndFoundConfiguration : IEntityTypeConfiguration<LostAndFound>
	{
		public void Configure(EntityTypeBuilder<LostAndFound> builder)
		{
			builder.ConfigureChangeTrackingBaseEntity();

			builder.HasKey(x => x.Id);

			builder.Property(a => a.Id)
				.HasColumnName(nameof(LostAndFound.Id))
				.IsRequired();

			builder.Property(a => a.Description)
				.HasColumnName(nameof(LostAndFound.Description));

			builder.Property(a => a.ImageUrl)
				.HasColumnName(nameof(LostAndFound.ImageUrl));

			builder.Property(a => a.IsDeleted)
				.HasColumnName(nameof(LostAndFound.IsDeleted))
				.IsRequired();

			builder.Property(a => a.IsClosed)
				.HasColumnName(nameof(LostAndFound.IsClosed))
				.IsRequired();

			builder.Property(a => a.HotelId)
				.HasColumnName(nameof(LostAndFound.HotelId));

			builder
				.HasOne(a => a.Hotel)
				.WithMany()
				.HasForeignKey(a => a.HotelId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(a => a.FirstName)
			   .HasColumnName(nameof(LostAndFound.FirstName));

			builder.Property(a => a.LastName)
			   .HasColumnName(nameof(LostAndFound.LastName));

			builder.Property(a => a.Address)
			  .HasColumnName(nameof(LostAndFound.Address));

			builder.Property(a => a.City)
			  .HasColumnName(nameof(LostAndFound.City));

			builder.Property(a => a.PostalCode)
			  .HasColumnName(nameof(LostAndFound.PostalCode));

			builder.Property(a => a.Country)
			  .HasColumnName(nameof(LostAndFound.Country));

			builder.Property(a => a.PhoneNumber)
			  .HasColumnName(nameof(LostAndFound.PhoneNumber));

			builder.Property(a => a.Email)
			  .HasColumnName(nameof(LostAndFound.Email));

			builder.Property(a => a.ReferenceNumber)
			  .HasColumnName(nameof(LostAndFound.ReferenceNumber));

			builder.Property(a => a.Notes)
			 .HasColumnName(nameof(LostAndFound.Notes));

			builder.Property(a => a.RoomId)
			  .HasColumnName(nameof(LostAndFound.RoomId));

			builder
				.HasOne(a => a.Room)
				.WithMany()
				.HasForeignKey(a => a.RoomId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(a => a.ReservationId)
			 .HasColumnName(nameof(LostAndFound.ReservationId));

			builder
				.HasOne(a => a.Reservation)
				.WithMany()
				.HasForeignKey(a => a.ReservationId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(a => a.LostOn)
			 .HasColumnName(nameof(LostAndFound.LostOn));

			builder.Property(a => a.TypeOfLoss)
			 .HasColumnName(nameof(LostAndFound.TypeOfLoss));

			builder.Property(a => a.Status)
				.HasColumnName(nameof(LostAndFound.Status))
				.IsRequired()
				.HasDefaultValue(LostAndFoundStatus.Unknown);
				// TODO: CREATE A MIGRAITON WITH THIS: .HasDefaultValue(LostAndFoundStatus.WaitingRoomMaid);

			builder.Property(a => a.Type)
				.HasColumnName(nameof(LostAndFound.Type))
				.IsRequired()
				.HasDefaultValue(LostAndFoundRecordType.Unknown)
				.HasConversion(a => a.ToString(), a => (LostAndFoundRecordType)Enum.Parse(typeof(LostAndFoundRecordType), a));

			builder.Property(a => a.RccStatus)
				.HasColumnName(nameof(LostAndFound.RccStatus))
				.IsRequired()
				.HasDefaultValue(RccLostAndFoundStatus.UNKNOWN)
				.HasConversion(a => a.ToString(), a => (RccLostAndFoundStatus)Enum.Parse(typeof(RccLostAndFoundStatus), a));
		}
	}
}