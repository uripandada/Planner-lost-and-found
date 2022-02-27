using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{
	public class FloorConfiguration : IEntityTypeConfiguration<Floor>
    {
        public void Configure(EntityTypeBuilder<Floor> builder)
        {
            builder.ConfigureBaseEntity();

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName(nameof(Floor.Id))
                .IsRequired();

            builder.Property(a => a.Name)
                .HasColumnName(nameof(Floor.Name))
                .IsRequired();

            builder.Property(a => a.Number)
                .HasColumnName(nameof(Floor.Number))
                .IsRequired();

            builder.Property(f => f.OrdinalNumber)
                .HasColumnName(nameof(Floor.OrdinalNumber))
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(f => f.BuildingId)
                .HasColumnName(nameof(Floor.BuildingId))
                .IsRequired();

            builder
                .HasOne(f => f.Building)
                .WithMany(b => b.Floors)
                .HasForeignKey(f => f.BuildingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(a => a.Rooms)
                .WithOne(r => r.Floor)
                .HasForeignKey(r => r.FloorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(a => a.Warehouses)
                .WithOne(r => r.Floor)
                .HasForeignKey(r => r.FloorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
