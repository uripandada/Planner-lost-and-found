using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
	{
		public void Configure(EntityTypeBuilder<Hotel> builder)
		{
			builder.HasKey(h => h.Id);

			builder.Property(h => h.Id)
				.HasColumnName(nameof(Hotel.Id))
				.IsRequired();

			builder.Property(h => h.Name)
				.HasColumnName(nameof(Hotel.Name))
				.IsRequired();

			builder.Property(h => h.WindowsTimeZoneId)
				.HasColumnName(nameof(Hotel.WindowsTimeZoneId))
				.IsRequired()
				.HasDefaultValue("GMT Standard Time");

			builder.Property(h => h.IanaTimeZoneId)
				.HasColumnName(nameof(Hotel.IanaTimeZoneId))
				.IsRequired()
				.HasDefaultValue("Etc/GMT");

			builder.Property(h => h.CreatedAt)
				.HasColumnName(nameof(Hotel.CreatedAt))
				.IsRequired()
				.HasDefaultValueSql("now()");

			builder.Property(h => h.ModifiedAt)
				.HasColumnName(nameof(Hotel.ModifiedAt))
				.IsRequired()
				.HasDefaultValueSql("now()");

			builder
				.HasMany(h => h.Rooms)
				.WithOne(r => r.Hotel)
				.HasForeignKey(r => r.HotelId)
				.OnDelete(DeleteBehavior.Restrict);
			
			builder
				.HasMany(h => h.Buildings)
				.WithOne(r => r.Hotel)
				.HasForeignKey(r => r.HotelId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasMany(h => h.CleaningPlugins)
				.WithOne(r => r.Hotel)
				.HasForeignKey(r => r.HotelId)
				.OnDelete(DeleteBehavior.Restrict);

			builder
				.HasOne(h => h.Settings)
				.WithOne(s => s.Hotel)
				.HasForeignKey<Settings>(s => s.HotelId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
