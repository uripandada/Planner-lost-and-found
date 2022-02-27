using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planner.Domain.Entities;

namespace Planner.Persistence.Configurations
{

	public class BuildingConfiguration : IEntityTypeConfiguration<Building>
    {
        public new void Configure(EntityTypeBuilder<Building> builder)
        {
            builder.ConfigureBaseEntity();

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .HasColumnName(nameof(Building.Id))
                .IsRequired(true);

            builder.Property(b => b.Name)
                .HasColumnName(nameof(Building.Name))
                .IsRequired(true);

            builder.Property(b => b.TypeKey)
                .HasColumnName(nameof(Building.TypeKey))
                .IsRequired(true);

            builder.Property(b => b.Address)
                .HasColumnName(nameof(Building.Address));

            builder.Property(b => b.Latitude)
                .HasColumnName(nameof(Building.Latitude));

            builder.Property(b => b.Longitude)
                .HasColumnName(nameof(Building.Longitude));

            builder.Property(b => b.OrdinalNumber)
                .HasColumnName(nameof(Building.OrdinalNumber))
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(b => b.AreaId)
                .HasColumnName(nameof(Building.AreaId));

            builder
                .HasOne(b => b.Area)
                .WithMany(a => a.Buildings)
                .HasForeignKey(b => b.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(b => b.Floors)
                .WithOne(f => f.Building)
                .HasForeignKey(f => f.BuildingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(b => b.Rooms)
                .WithOne(r => r.Building)
                .HasForeignKey(r => r.BuildingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
