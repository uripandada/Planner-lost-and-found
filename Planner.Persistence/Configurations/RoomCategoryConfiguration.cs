using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class RoomCategoryConfiguration : IEntityTypeConfiguration<RoomCategory>
    {
        public void Configure(EntityTypeBuilder<RoomCategory> builder)
        {
            builder.ConfigureChangeTrackingBaseEntity();

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(RoomCategory.Id))
                .IsRequired();

            builder.Property(a => a.Name)
                .HasColumnName(nameof(RoomCategory.Name))
                .IsRequired();

            builder.Property(a => a.IsPrivate)
                .HasColumnName(nameof(RoomCategory.IsPrivate))
                .IsRequired();

            builder.Property(a => a.IsPublic)
                .HasColumnName(nameof(RoomCategory.IsPublic))
                .IsRequired();

            builder.Property(a => a.IsDefaultForReservationSync)
                .HasColumnName(nameof(RoomCategory.IsDefaultForReservationSync))
                .IsRequired();

            builder.Property(a => a.IsSystemDefaultForReservationSync)
                .HasColumnName(nameof(RoomCategory.IsSystemDefaultForReservationSync))
                .IsRequired();

            builder
                .HasMany(c => c.Rooms)
                .WithOne(r => r.Category)
                .HasForeignKey(r => r.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
